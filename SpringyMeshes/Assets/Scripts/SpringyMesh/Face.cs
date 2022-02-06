using UnityEngine;

/// <summary>
/// Face of soft body object.
/// </summary>
public class Face
{
    /// <summary> Initial area of face </summary>
    public float initialArea;
    /// <summary> Normal vector of face </summary>
    public Vector3 normal;

    /// <summary> First vertex in face </summary>
    private Vertex v1;
    /// <summary> Second vertex in face </summary>
    private Vertex v2;
    /// <summary> Third vertex in face </summary>
    private Vertex v3;
    
    /// <summary> Angle incident to first vertex </summary>
    private float angle1;
    /// <summary> Angle incident to second vertex </summary>
    private float angle2;
    /// <summary> Angle incident to third vertex </summary>
    private float angle3;

    public Face(Vertex v1, Vertex v2, Vertex v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
        
        // Calculate area of that face - used in mass distribution
        initialArea = AreaOfFace();
        
        // Calculate angles incident to vertices
        angle1 = Vector3.Angle(v3.Position - v1.Position, v2.Position - v1.Position);
        angle2 = Vector3.Angle(v1.Position - v2.Position, v3.Position - v2.Position);
        angle3 = Vector3.Angle(v2.Position - v3.Position, v1.Position - v3.Position);
        
        Update();
    }

    public void Update()
    {
        // Recalculate face normal
        normal =  Vector3.Cross( v2.Position - v1.Position,v3.Position - v1.Position).normalized;
    }

    /// <summary>
    ///  Returns angle incident to vertex.
    /// </summary>
    /// <param name="v">Vertex to get angle incident to</param>
    /// <returns>Angle in degrees</returns>
    public float GetAngleByVertex(Vertex v)
    {
        if (v == v1) { return angle1; }
        if (v == v2) { return angle2; }
        if (v == v3) { return angle3; }
        
        Debug.LogError("Given vertex is not present in triangle.");
        return -1;
    }

    /// <summary>
    /// Compute area of face.
    /// </summary>
    /// <returns>Area of face</returns>
    private float AreaOfFace()
    {
        return 0.5f * Vector3.Cross(v2.Position-v1.Position, v3.Position-v1.Position).magnitude;
    }
}