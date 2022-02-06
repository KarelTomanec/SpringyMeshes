using UnityEngine;

public class Strut
{
    /// <summary> Damped spring springiness constant </summary>
    private float k;
    /// <summary> Damped spring damping constant </summary>
    private float d;
    /// <summary> Torsional spring springiness constant </summary>
    private float kTheta;
    /// <summary> Torsional spring damping constant </summary>
    private float dTheta;
    
    /// <summary> Length of strut when at rest </summary>
    public float restLength;
    /// <summary> Angle of faces incident with strut while at rest </summary>
    public float restAngle;

    /// <summary> Start vertex of strut </summary>
    public Vertex from;
    /// <summary> End vertex of strut </summary>
    public Vertex to;

    /// <summary> One adjacent face </summary>
    public Face face1;
    /// <summary> Second adjacent face </summary>
    public Face face2;

    /// <summary> First opposite vertex (third in one face) </summary>
    public Vertex opposite1;
    /// <summary> Second opposite vertex (third in second face) </summary>
    public Vertex opposite2;

    public Strut(int id, Vertex from, Vertex to)
    {
        this.from = from;
        this.to = to;

        this.restLength = Vector3.Distance(from.Position, to.Position);
    }

    /// <summary>
    /// Computes parameters of strut based on desired parameters
    /// </summary>
    /// <param name="avgLength">Average length of strut in soft body</param>
    /// <param name="T">Damped spring time constant</param>
    /// <param name="P">Damped spring oscillation period</param>
    /// <param name="Ttheta">Torsional spring time constant</param>
    /// <param name="Ptheta">Torsional spring oscillation period</param>
    public void Preprocess(float avgLength, float T, float P, float Ttheta, float Ptheta)
    {
        // Calculate springiness and damping factors
        k = 4.0f * Mathf.PI * Mathf.PI * from.mass / (P * P);
        d = 2.0f * from.mass / T;

        // Scale those factors by strut length 
        k *= restLength / avgLength;
        d *= restLength / avgLength;

        // Calculate stuff needed for computation of torsional factors
        Vector3 h = (to.Position - from.Position).normalized;

        this.restAngle = Mathf.Atan2(Vector3.Dot(Vector3.Cross(face1.normal, face2.normal), h),
            Vector3.Dot(face1.normal, face2.normal));

        Vector3 x02 = from.Position - opposite1.Position;
        Vector3 x03 = from.Position - opposite2.Position;

        Vector3 r_l = x02 - Vector3.Dot(x02, h) * h;
        Vector3 r_r = x03 - Vector3.Dot(x03, h) * h;
        float avgDist = 0.5f * (Vector3.Magnitude(r_l) + Vector3.Magnitude(r_r));

        float mass1 = opposite1.mass;
        float mass2 = opposite2.mass;
        float avgMass = 0.5f * (mass1 + mass2);

        // Compute torsional spring factors
        kTheta = 4.0f * Mathf.PI * Mathf.PI * avgDist * avgDist * avgMass / (Ptheta * Ptheta);
        dTheta = 2.0f * avgMass * avgDist / Ttheta;

        Debug.Log("Kt, Dt: " + kTheta + "    " + dTheta);
    }

    /// <summary>
    /// Comparison function for strut (based on from/to IDs)
    /// </summary>
    /// <param name="id1">Vertex1 ID</param>
    /// <param name="id2">Vertex2 ID</param>
    /// <returns>True when strut is represented by those two vertices</returns>
    public bool IsSame(int id1, int id2)
    {
        return (from.id == id1 && to.id == id2) || (from.id == id2 && to.id == id1);
    }

    /// <summary>
    /// Applies all forces acting in strut
    /// </summary>
    public void ApplyForces()
    {
        float length = Vector3.Distance(from.Position, to.Position);
        Vector3 dir = (to.Position - from.Position).normalized;

        // Apply spring forces
        Vector3 f_s = k * (length - restLength) * dir;
        from.AddForce(f_s);
        to.AddForce(-f_s);

        // Apply damping force
        Vector3 f_d = d * Vector3.Dot((to.Velocity - from.Velocity), dir) * dir;
        from.AddForce(f_d);
        to.AddForce(-f_d);

        // Computation of hinge forces
        Vector3 h = (from.Position - to.Position).normalized;
        Vector3 x02 = from.Position - opposite1.Position;
        Vector3 x03 = from.Position - opposite2.Position;

        // Vectors formed by lofting a perpendicular from the hinge edge
        Vector3 r_l = x02 - Vector3.Dot(x02, h) * h;
        Vector3 r_r = x03 - Vector3.Dot(x03, h) * h;

        Vector3 n_l = face1.normal;
        Vector3 n_r = face2.normal;

        float theta = Mathf.Atan2(Vector3.Dot(Vector3.Cross(n_l, n_r), h), Vector3.Dot(n_l, n_r));
        Vector3 tau_k = kTheta * (theta - restAngle) * h;

        float theta_l = Vector3.Dot(opposite1.Velocity, n_l) / Vector3.Magnitude(r_l);
        float theta_r = Vector3.Dot(opposite2.Velocity, n_r) / Vector3.Magnitude(r_r);

        float d02 = Vector3.Dot(x02, h);
        float d03 = Vector3.Dot(x03, h);

        Vector3 tau_d = -dTheta * (theta_l + theta_r) * h;
        Vector3 tau = tau_k + tau_d;

        Vector3 f2 = n_l * Vector3.Dot(tau, h) / Vector3.Magnitude(r_l);
        Vector3 f3 = n_r * Vector3.Dot(tau, h) / Vector3.Magnitude(r_r);
        Vector3 f1 = -(d02 * f2 + d03 * f3) / Vector3.Magnitude(to.Position - from.Position);
        Vector3 f0 = -(f1 + f2 + f3);

        // Apply computed forces
        from.AddForce(f0);
        to.AddForce(f1);
        opposite1.AddForce(f2);
        opposite2.AddForce(f3);
    }
}