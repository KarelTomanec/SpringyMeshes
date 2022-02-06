using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Class representing state of particle in Soft Body simulation
/// </summary>
public class State
{
    /// <summary> Current particle velocity </summary>
    public Vector3 velocity;
    /// <summary> Current particle position </summary>
    public Vector3 position;
    /// <summary> Current force acting on particle </summary>
    public Vector3 force;
    
    public State()
    {
        velocity = Vector3.zero;
        position = Vector3.zero;
        force = Vector3.zero;
    }

    public State(Vector3 velocity, Vector3 position)
    {
        this.velocity = velocity;
        this.position = position;
        this.force = Vector3.zero;
    }
    public State(Vector3 velocity, Vector3 position, Vector3 force)
    {
        this.velocity = velocity;
        this.force = force;
        this.position = position;
    }

    /// <summary>
    /// Performs update of state with possibilty to use Euler / Leapfrog / Runge-Kutta integration method 
    /// </summary>
    /// <param name="acceleration">Acceleration of particle</param>
    /// <param name="deltaTime">Simulation timestep</param>
    public void Integrate(Vector3 acceleration, float deltaTime)
    {
        // Euler method
        // Vector3 prevVelocity = velocity;
        // velocity = velocity + acceleration * deltaTime;
        // position = position + (prevVelocity + velocity) * 0.5f * deltaTime;
        
        // Leap frog method
        //Vector3 prevVelocity = velocity;
        //Vector3 newPosition = position + prevVelocity * deltaTime * 0.5f;
        //Vector3 newVelocity = prevVelocity + acceleration * deltaTime;
        //newPosition = newPosition + prevVelocity * deltaTime * 0.5f;
        //velocity = newVelocity;
        //position = newPosition;

        // Runge-Kutta4 method
        Vector3 position1 = position;
        Vector3 velocity1 = velocity;
        Vector3 acceleration1 = acceleration;
        
        Vector3 position2 = position1 + velocity1 * deltaTime * 0.5f;
        Vector3 velocity2 = velocity1 + acceleration1 * deltaTime * 0.5f;
        Vector3 acceleration2 = acceleration;
        
        Vector3 position3 = position1 + velocity2 * deltaTime * 0.5f;
        Vector3 velocity3 = velocity1 + acceleration2 * deltaTime * 0.5f;
        Vector3 acceleration3 = acceleration;
        
        
        Vector3 position4 = position1 + velocity3 * deltaTime;
        Vector3 velocity4 = velocity1 + acceleration3 * deltaTime;
        Vector3 acceleration4 = acceleration;
        
        position = position + deltaTime * (velocity1 + 2.0f * (velocity2 + velocity3) + velocity4) / 6.0f;
        velocity = velocity + deltaTime * (acceleration1 + 2.0f * (acceleration2 + acceleration3) + acceleration4) / 6.0f;
    }
}
