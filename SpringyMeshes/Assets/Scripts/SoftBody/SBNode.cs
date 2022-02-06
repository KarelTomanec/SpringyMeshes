using UnityEngine;

/// <summary>
/// Older representation of SoftBody Vertex - kept for legacy reasons
/// 
/// Strut is only represented by damped spring - no torsional springs for volume conservation.
/// </summary>
public class SBNode
{
    private const float eps = 0.01f;
    private float mass;
    private State state;
    private Plane plane;

    private float c_r = 1.0f;
    private float c_f = 0.0f;

    private bool isFixed = false;

    private const float epsilon = 0.1f;

    public bool isAtRestFlag = false;

    public SBNode(Vector3 position, float mass)
    {
        state = new State(Vector3.zero, position, Vector3.zero);
        this.mass = mass;
        plane = new Plane(Vector3.zero, Vector3.up);
    }

    public void Tick(float deltaTime, Transform transform) {

        if (isFixed)
        {
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
        
        // new-state = Integration of accelerations over timestep delta
        State newState = new State(state.velocity, state.position);
        newState.Integrate(acceleration, deltaTime);

        // // collision detection with plane
        float distance = plane.pointDist(transform.TransformPoint(state.position));
        float newDistance = plane.pointDist(transform.TransformPoint(newState.position));
        
        if (newDistance < 0.0f)
        {
            // calculate first collision and reintegrate
            float f = distance / (distance - newDistance);
            float deltaTimeCollision = deltaTime * f;
            newState = new State(state.velocity, state.position);
            newState.Integrate(acceleration, deltaTimeCollision);
        
            // collision response
            Vector3 v_n_minus = Vector3.Dot(newState.velocity, transform.InverseTransformDirection(plane.normal)) * transform.InverseTransformDirection(plane.normal);
            Vector3 v_t_minus = newState.velocity - v_n_minus;
        
            Vector3 v_n_plus = -c_r * Vector3.Dot(newState.velocity, transform.InverseTransformDirection(plane.normal)) * transform.InverseTransformDirection(plane.normal);
            Vector3 v_t_plus = (1.0f - c_f) * v_t_minus;
        
            newState.velocity = v_n_plus + v_t_plus;
            newState.Integrate(Vector3.zero, deltaTime - deltaTimeCollision);
        }
        
        // current-state = new-state
        state = newState;
        

    }
    
    bool isResting(Transform transform)
    {
        if (state.velocity.magnitude < epsilon) {
            //Debug.Log("Node Velocity under epsilon");
            if (plane.sphereDist(transform.TransformPoint(state.position) , 0.25f) < epsilon) {
                //Debug.Log("Distance under epsilon");
                if (Vector3.Dot(state.force, transform.InverseTransformDirection(plane.normal)) < epsilon) {
                    //Debug.Log("Plane under sphere");
                    Vector3 fn = Vector3.Dot(transform.InverseTransformDirection(plane.normal), state.force) * transform.InverseTransformDirection(plane.normal);
                    Vector3 ft = state.force - fn;

                    if (ft.magnitude < fn.magnitude){ 
                        // object is at rest
                        Debug.Log("Mesh is at rest");
                        isAtRestFlag = true;
                        return true;
                    }
                }
            }
        }

        isAtRestFlag = false;
        return false;
    }

    public void AddForce(Vector3 force)
    {
        state.force += force;
    }

    public float Mass
    {
        get => mass;
        set => mass = value;
    }

    public Vector3 Position
    {
        get => state.position;
        set => state.position = value;
    }

    public Vector3 Force
    {
        get => state.force;
        set => state.force = value;
    }

    public Vector3 Velocity
    {
        get => state.velocity;
        set => state.velocity = value;
    }

    public bool IsFixed
    {
        get => isFixed;
        set => isFixed = value;
    }
    
}
