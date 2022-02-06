using UnityEngine;

/// <summary>
/// Soft body represented by generated IcoSphere
/// </summary>
public class SpringyMeshIcoSphere : SpringyMeshBase
{
    /// <summary> Number of IcoSphere subdivisions </summary>
    public int subdivisions;
    /// <summary> Radius of IcoSphere </summary>
    public float radius;

    protected override void InitMeshStuff()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        
        // Set mesh as result of ShapeGenerator
        meshFilter.mesh = ShapeGenerator.IcoSphere.Create(subdivisions, radius);

        mesh = meshFilter.mesh;

    }

    protected override void InitObject()
    {
        base.InitObject();
        // vertices[0].isFixed = true;
    }

    protected override void Update()
    {
        Render();
        
        // Allow user to control the sphere
        ProcessInput();
    }
}