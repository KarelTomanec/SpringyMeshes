using UnityEngine;

/// <summary>
/// Simple plane used in Soft Body simulation for collisions.
/// </summary>
public class Plane
{
    /// <summary> Position of plane </summary>
    public Vector3 position;
    /// <summary> Normal vector of plane </summary>
    public Vector3 normal;

    public Plane(Vector3 position, Vector3 normal)
    {
        this.position = position;
        this.normal = normal;
    }

    /// <summary>
    /// Calculate distance of point from plane.
    /// </summary>
    /// <param name="pointPosition">Position to compute distance to.</param>
    /// <returns>Distance to given position.</returns>
    public float pointDist(Vector3 pointPosition)
    {
        return Vector3.Dot(pointPosition - position, normal);
    }

    /// <summary>
    /// Calculate distance of sphere form plane.
    /// </summary>
    /// <param name="spherePosition">Position of sphere center.</param>
    /// <param name="sphereRadius">Radius of sphere</param>
    /// <returns>Closest distance between sphere and plane</returns>
    public float sphereDist(Vector3 spherePosition, float sphereRadius)
    {
        return Vector3.Dot(spherePosition - position, normal) - sphereRadius;
    }
}