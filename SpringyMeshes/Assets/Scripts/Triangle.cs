using UnityEngine;

/// <summary>
/// Triangle which is used in collisions with Soft Bodies
/// </summary>
public class Triangle
{
    /// <summary> ID of triangle </summary>
    public int id;
    
    /// <summary> Position of first triangle vertex </summary>
    public Vector3 a;
    /// <summary> Position of second triangle vertex </summary>
    public Vector3 b;
    /// <summary> Position of third triangle vertex </summary>
    public Vector3 c;
    
    /// <summary> Triangle normal vector</summary>
    public Vector3 normal;

    
    /// <summary> EPSilon for usage in computations </summary>
    const float eps = 0.01f;
    
    public Triangle(Vector3 a, Vector3 b, Vector3 c, int id)
    {
        this.id = id;
        
        this.a = a;
        this.b = b;
        this.c = c;
        
        this.normal = Vector3.Cross(b - a, c - a).normalized;
    }
    
    /// <summary>
    /// Decide whether point lies in triangle or not
    /// </summary>
    /// <param name="p">Point to check</param>
    /// <returns>True when triangle contains the point</returns>
    bool pointInTriangle(Vector3 p)
    {
        // Compute vectors        
        Vector3 v0 = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = p - a;

        // Compute dot products
        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        // Compute barycentric coordinates
        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Check if point is in triangle
        return (u >= 0) && (v >= 0) && (u + v < 1);

    }

    /// <summary>
    /// Do intersection of triangle with point.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="v"></param>
    /// <param name="deltaTime"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    public bool intersectPoint(Vector3 p, Vector3 v, float deltaTime, out float f)
    {

        float eps = 0.0001f;
        f = 0;
        float tHit = Vector3.Dot(a - p, normal) / Vector3.Dot(v, normal);
        Debug.Log(id + ": " + tHit + " " + deltaTime);
        // Debug.Log((0.0f <= tHit).ToString() + " " + (tHit < deltaTime).ToString());
        
        // if (0.0f <= tHit + eps && tHit < deltaTime + eps)
        if (0.0f + eps <= tHit && tHit <= deltaTime + eps)
        {
            Debug.Log("Collision with plane detected");
            Vector3 pHit = p + tHit * v;
            if (pointInTriangle(pHit))
            {
                Debug.Log("Collision with triangle detected");
                f = tHit;
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Compute distance of point from triangle
    /// </summary>
    /// <param name="pointPosition">Point position to measure distance to</param>
    /// <returns>Distance of point from triangle</returns>
    public float pointDist(Vector3 pointPosition)
    {
        return (Vector3.Dot(pointPosition - a, normal));
    }
    
    /// <summary>
    /// Compute distance of sphere from triangle
    /// </summary>
    /// <param name="spherePosition">Sphere center position to measure distance to</param>
    /// <param name="sphereRadius">Radius of the sphere</param>
    /// <returns>Distance of sphere from triangle</returns>
    public float sphereDist(Vector3 spherePosition, float sphereRadius)
    {
        return Vector3.Dot(spherePosition - a, normal) - sphereRadius;
    }
}
