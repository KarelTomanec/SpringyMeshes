using UnityEngine;

/// <summary>
/// Soft body represented by mesh
///
/// This mesh has to be watertight, and placed in scene somewhere - it is used instead of extra object loader.
/// </summary>
public class SpringyMeshMesh : SpringyMeshBase
{
    /// <summary> GameObject of model which should be "softified" </summary>
    public GameObject model;

    protected override void InitMeshStuff()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        
        // Take mesh from models gameObject Mesh Filter
        meshFilter.mesh = model.GetComponent<MeshFilter>().mesh;
        
        mesh = meshFilter.mesh;
    }
}