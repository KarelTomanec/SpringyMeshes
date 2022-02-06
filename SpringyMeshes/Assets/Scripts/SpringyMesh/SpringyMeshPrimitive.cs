using UnityEngine;

/// <summary>
/// Soft body represented by generated geometry
/// </summary>
public class SpringyMeshPrimitive : SpringyMeshBase
{
    /// <summary> Size of primitive side </summary>
    public float size = 4.0f;
    /// <summary> Type of generated primitive </summary>
    public ShapeGenerator.ShapeType shapeType;
    
    protected override void InitMeshStuff()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();

        // Init mesh as result of ShapeGenerator
        if (shapeType == ShapeGenerator.ShapeType.Cube) {
            meshFilter.mesh = ShapeGenerator.Cube.Create(size);
        } else if (shapeType == ShapeGenerator.ShapeType.Tetrahedron) {
            meshFilter.mesh = ShapeGenerator.Tetrahedron.Create(size);
        }

        mesh = meshFilter.mesh;
    }
}