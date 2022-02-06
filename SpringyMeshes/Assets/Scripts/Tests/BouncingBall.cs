using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;

public class BouncingBall : MonoBehaviour
{
    // Start is called before the first frame update
    
    public float mass = 1.0f;
    public Vector3 force= Vector3.zero;
    
    public State state;
    public Vector3 windVelocity = Vector3.zero;
    public float airResistance = 0.0f; // air resistance constant
    public float c_r = 1.0f;
    public float c_f = 0.0f;

    public Vector3 initialVelocity = Vector3.zero;

    private float dilation = 1.0f;
    private Plane plane;

    public GameObject planeMesh;
    [FormerlySerializedAs("collider")] [FormerlySerializedAs("box")] public Cascade cascade;

    private const float epsilon = 0.01f;

    void Start()
    {
        Physics.autoSimulation = false;
        state = new State(initialVelocity, transform.position);
        plane = new Plane(Vector3.zero, planeMesh.transform.up);

    }
    
    private void Update()
    {
        
        // User input
        float acceleration = 50.0f;
        if(Input.GetKey(KeyCode.W)){
            Debug.Log("W");
            force+= acceleration * Vector3.forward * mass;
        }
        if(Input.GetKey(KeyCode.S)){
            Debug.Log("S");
            force+= -acceleration * Vector3.forward * mass;
        }
        if(Input.GetKey(KeyCode.D)){
            Debug.Log("D");
            force+= acceleration * Vector3.right * mass;
        }
        if(Input.GetKey(KeyCode.A)){
            Debug.Log("A");
            force+= -acceleration * Vector3.right * mass;
        }
        if(Input.GetKey(KeyCode.Space)){
            Debug.Log("Space");
            force+= acceleration * Vector3.up * mass;
        }
    }
    
    void FixedUpdate()
    {        
        if (cascade.triangles == null)
        {
            Debug.Log("box null");
        }
        float deltaTime = Time.fixedDeltaTime * dilation;
        
        force += Physics.gravity * mass; // gravity
        force += -airResistance * state.velocity; // air resistance
        // force += airResistance * windVelocity; // wind

        // if (isResting(deltaTime)) {
        //     force = Vector3.zero;
        //     return;
        // }

        // Determine accelerations by Newton’s second law
        Vector3 acceleration = force / mass;

        // new-state = Integration of accelerations over timestep delta
        State newState = new State(state.velocity, state.position);
        newState.Integrate(acceleration, deltaTime);
        
        
        float min_f = float.PositiveInfinity;
        int triangleId = 0;
        bool collision = false;
        for (int i = 0; i < cascade.triangles.Count; i++)
        {
            float f;
            if (cascade.triangles[i].intersectPoint(state.position, newState.velocity, deltaTime, out f))
            {
                collision = true;
                triangleId = i;
                if (min_f > f)
                {
                    min_f = f;
                }

            }
        }

        if (collision)
        {
            float deltaTimeCollision = deltaTime * min_f;
            newState = new State(state.velocity, state.position);   
            newState.Integrate(acceleration, min_f);
                
            // collision response
            Vector3 v_n_minus = Vector3.Dot(newState.velocity, cascade.triangles[triangleId].normal) * cascade.triangles[triangleId].normal;
            Vector3 v_t_minus = newState.velocity - v_n_minus;
                
            Vector3 v_n_plus = -c_r * Vector3.Dot(newState.velocity, cascade.triangles[triangleId].normal) * cascade.triangles[triangleId].normal;
            Vector3 v_t_plus = (1.0f - c_f) * v_t_minus;
                
            newState.velocity = v_n_plus + v_t_plus;
            newState.Integrate(Vector3.zero, deltaTime - deltaTimeCollision);
        }

        // current-state = new-state
        state = newState;
        transform.position = state.position;
        
        force =  Vector3.zero;
    }

    // Update is called once per frame
    // void FixedUpdate()
    // {
    //     float deltaTime = Time.fixedDeltaTime * dilation;
    //     
    //     force += Physics.gravity * mass; // gravity
    //     force += -airResistance * state.velocity; // air resistance
    //     force += airResistance * windVelocity; // wind
    //
    //     if (isResting()) {
    //         force = Vector3.zero;
    //         return;
    //     }
    //
    //     // Determine accelerations by Newton’s second law
    //     Vector3 acceleration = force / mass;
    //
    //     // new-state = Integration of accelerations over timestep delta
    //     State newState = new State(state.velocity, state.position);
    //     newState.Integrate(acceleration, deltaTime);
    //
    //     // collision detection with plane
    //     float distance = plane.sphereDist(state.position, 0.25f);
    //     float newDistance = plane.sphereDist(newState.position, 0.25f);
    //     if (newDistance < 0.0f)
    //     {
    //         // calculate first collision and reintegrate
    //         float f = distance / (distance - newDistance);
    //         float deltaTimeCollision = deltaTime * f;
    //         newState = new State(state.velocity, state.position);
    //         newState.Integrate(acceleration, deltaTimeCollision);
    //
    //         // collision response
    //         Vector3 v_n_minus = Vector3.Dot(newState.velocity, plane.normal) * plane.normal;
    //         Vector3 v_t_minus = newState.velocity - v_n_minus;
    //
    //         Vector3 v_n_plus = -c_r * Vector3.Dot(newState.velocity, plane.normal) * plane.normal;
    //         Vector3 v_t_plus = (1.0f - c_f) * v_t_minus;
    //
    //         newState.velocity = v_n_plus + v_t_plus;
    //         newState.Integrate(Vector3.zero, deltaTime - deltaTimeCollision);
    //     }
    //     
    //     // current-state = new-state
    //     state = newState;
    //     transform.position = state.position;
    //     
    //     force =  Vector3.zero;
    // }

    bool isResting(float deltaTime)
    {
        if (state.velocity.magnitude < epsilon) {
            //Debug.Log("Velocity under epsilon");
            Triangle tri = null;
            bool triangleIntersection = false;
            
            try {
                for (int i = 0; i < cascade.triangles.Count; i++)
                {
                    float f;
                    if (cascade.triangles[i].intersectPoint(state.position, state.velocity, deltaTime, out f))
                    {
                        triangleIntersection = true;
                        tri = cascade.triangles[i];
                        break;
                    }
                }
            }       
            catch (NullReferenceException ex) {
                Debug.Log("Box was not set in the inspector. " + ex.ToString());
            }


            if (triangleIntersection)
            {
                if (tri.pointDist(state.position) < epsilon) {
                    //Debug.Log("Distance under epsilon");
                    if (Vector3.Dot(force, tri.normal) < epsilon) {
                        //Debug.Log("Plane under sphere");
                        Vector3 fn = Vector3.Dot(tri.normal, force) * tri.normal;
                        Vector3 ft = force - fn;

                        if (ft.magnitude < fn.magnitude){ 
                            // object is at rest
                            Debug.Log("Sphere is at rest");
                            return true;
                        }
                    }
                }
            }

        }

        return false;
    }


}
