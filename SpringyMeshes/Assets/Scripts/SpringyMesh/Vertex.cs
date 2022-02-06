using UnityEngine;

/// <summary>
/// Class representing point in Soft Body
/// </summary>
public class Vertex
{
    /// <summary> EPSilon for usage in computations </summary>
    private const float epsilon = 0.05f;

    // FIXME: should be for each triangle, this is only temporary solution
    /// <summary> coefficient of restitution </summary>
    private float c_r = 0.6f;
    /// <summary> coefficient of friction </summary> 
    private float c_f = 0.3f;


    /// <summary> ID of vertex (for build purposes) </summary>
    public int id;

    /// <summary> Mass of point (fraction of SoftBody mass) </summary>
    public float mass;
    /// <summary> Internal state of Vertex (pos, force, speed) </summary>
    private State state;

    /// <summary> Plane reference for usage in collisions </summary>
    private Plane plane;
    /// <summary> Reference to MyCollider - representation of "cascade" for collisions </summary>
    private Cascade collider;

    /// <summary> Flag for decision whether vertex should be simulated or not </summary>
    public bool isFixed = false;
    /// <summary> Flag for decision whether vertex is at rest </summary>
    public bool isAtRestFlag = false;

    public Vertex(Vector3 position, float mass)
    {
        this.mass = mass;
        state = new State(Vector3.zero, position, Vector3.zero);

        // TODO: this representation of the plane is temporary
        plane = new Plane(Vector3.zero, Vector3.up);
    }

    public Vertex(Vector3 position, float mass, Plane plane)
    {
        this.mass = mass;
        state = new State(Vector3.zero, position, Vector3.zero);

        // TODO: this representation of the plane is temporary
        this.plane = plane;
    }

    public Vertex(Vector3 position, float mass, Cascade collider)
    {
        this.mass = mass;
        state = new State(Vector3.zero, position, Vector3.zero);

        // TODO: this representation of the plane is temporary
        this.collider = collider;
    }

    /// <summary>
    /// Update function when computing collisions with plane
    /// </summary>
    /// <param name="deltaTime">Timestep of simulation</param>
    /// <param name="transform">Transform for usage in point transformation</param>
    public void TickPlane(float deltaTime, Transform transform)
    {
        // When fixed or resting ensure not to move
        if (isFixed) {
            state.velocity = Vector3.zero;
            state.force = Vector3.zero;
            return;
        }

        if (isResting(transform)) {
            state.force = Vector3.zero;
            return;
        }

        // Determine accelerations by Newton’s second law
        Vector3 acceleration = state.force / mass;
        state.force = Vector3.zero;

        // New state = Integration of accelerations over timestep delta
        State newState = new State(state.velocity, state.position);
        newState.Integrate(acceleration, deltaTime);

        // Collision detection with plane
        float distance = plane.pointDist(transform.TransformPoint(state.position));
        float newDistance = plane.pointDist(transform.TransformPoint(newState.position));

        if (newDistance < 0.0f) {
            // Calculate first collision and reintegrate
            float f = distance / (distance - newDistance);
            float deltaTimeCollision = deltaTime * f;

            newState = new State(state.velocity, state.position);
            newState.Integrate(acceleration, deltaTimeCollision);

            // collision response
            Vector3 v_n_minus = Vector3.Dot(newState.velocity, transform.InverseTransformDirection(plane.normal)) *
                                transform.InverseTransformDirection(plane.normal);
            Vector3 v_t_minus = newState.velocity - v_n_minus;
            Vector3 v_n_plus = -c_r *
                               Vector3.Dot(newState.velocity, transform.InverseTransformDirection(plane.normal)) *
                               transform.InverseTransformDirection(plane.normal);
            Vector3 v_t_plus = (1.0f - c_f) * v_t_minus;

            newState.velocity = v_n_plus + v_t_plus;
            newState.Integrate(Vector3.zero, deltaTime - deltaTimeCollision);
        }

        // Update state to new
        state = newState;
    }

    /// <summary>
    /// Update function when computing collisions with triangles
    /// </summary>
    /// <param name="deltaTime">Timestep of simulation</param>
    /// <param name="transform">Transform for usage in point transformation</param>
    public void TickCollider(float deltaTime, Transform transform)
    {
        // When fixed or resting ensure not to move
        if (isFixed) {
            state.velocity = Vector3.zero;
            state.force = Vector3.zero;
            return;
        }

        if (isResting(transform)) {
            state.force = Vector3.zero;
            return;
        }

        // Determine accelerations by Newton’s second law
        Vector3 acceleration = state.force / mass;
        state.force = Vector3.zero;

        // New state = Integration of accelerations over timestep delta
        State newState = new State(state.velocity, state.position);
        newState.Integrate(acceleration, deltaTime);

        // Decide if collision happened
        float min_f = float.PositiveInfinity;
        int triangleId = 0;
        bool collision = false;
        for (int i = 0; i < collider.triangles.Count; i++) {
            float newDistance = collider.triangles[i].pointDist(transform.TransformPoint(newState.position));

            // When distance is too big, then collision surely didnt happen
            if (newDistance > 0.0f || newDistance < -0.2f) {
                continue;
            }

            float f;
            if (collider.triangles[i].intersectPoint(
                transform.TransformPoint(state.position),
                transform.TransformVector(newState.velocity), deltaTime, out f)) {
                collision = true;
                triangleId = i;
                if (min_f > f) {
                    min_f = f;
                }
            }
        }

        // When collision happened do adequate response
        if (collision) {
            float deltaTimeCollision = deltaTime * min_f;

            newState = new State(state.velocity, state.position);
            newState.Integrate(acceleration, min_f);

            // collision response
            Vector3 triNormal = transform.InverseTransformDirection(collider.triangles[triangleId].normal);
            Vector3 v_n_minus = Vector3.Dot(newState.velocity, triNormal) * triNormal;
            Vector3 v_t_minus = newState.velocity - v_n_minus;

            Vector3 v_n_plus = -c_r * Vector3.Dot(newState.velocity, triNormal) * triNormal;
            Vector3 v_t_plus = (1.0f - c_f) * v_t_minus;

            newState.velocity = v_n_plus + v_t_plus;
            newState.Integrate(Vector3.zero, deltaTime - deltaTimeCollision);
        }

        // Update current state to new one
        state = newState;
    }

    /// <summary>
    /// Decides whether vertex is at rest (simulation shouldn't continue - eliminate small movements at rest) 
    /// </summary>
    /// <param name="transform">Transform for usage in point transformation</param>
    /// <returns>True when vertex should rest (not move)</returns>
    bool isResting(Transform transform)
    {
        return false;
        
        if (state.velocity.magnitude < epsilon) {
            if (plane.sphereDist(transform.TransformPoint(state.position), 0.25f) < epsilon) {
                if (Vector3.Dot(state.force, transform.InverseTransformDirection(plane.normal)) < epsilon) {
                    Vector3 fn = Vector3.Dot(transform.InverseTransformDirection(plane.normal), state.force) *
                                 transform.InverseTransformDirection(plane.normal);
                    Vector3 ft = state.force - fn;

                    if (ft.magnitude < fn.magnitude) {
                        isAtRestFlag = true;
                        return true;
                    }
                }
            }
        }

        isAtRestFlag = false;
        return false;
    }

    /// <summary>
    /// Adds force to vertex force
    /// </summary>
    /// <param name="force">Force to add</param>
    public void AddForce(Vector3 force)
    {
        state.force += force;
    }

    public float Mass {
        get => mass;
        set => mass = value;
    }

    public Vector3 Position {
        get => state.position;
        set => state.position = value;
    }

    public Vector3 Force {
        get => state.force;
        set => state.force = value;
    }

    public Vector3 Velocity {
        get => state.velocity;
        set => state.velocity = value;
    }
}