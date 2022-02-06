using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for creating "cascade" on which something falls on and through
/// </summary>
public class Cascade : MonoBehaviour
{
    /// <summary> Material of pads </summary>
    public Material mat;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<int> triangleIndices;
    public List<Triangle> triangles;

    private void OnEnable()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        
        // Create cascade vertices
        float w = 16.0f;
        float h = 10.0f;
        float r = 5f;
        float d = 14.0f;
        vertices = new List<Vector3>
        {
            // rect 0
            new Vector3(-(d + r), h, 0),
            new Vector3(-(d + r), h, w),
            new Vector3(-r, 0, 0),
            new Vector3(-r, 0, w),
            
            // rect 1
            new Vector3((d + r), 0, w),
            new Vector3((d + r), 0, 0),
            new Vector3(r, -h, w),
            new Vector3(r, -h, 0),
            
            // rect 2
            new Vector3(-(d + r), -h, 0),
            new Vector3(-(d + r), -h, w),
            new Vector3(-r, -2 * h, 0),
            new Vector3(-r, -2 * h, w),

            // rect 3
            new Vector3((d + r), -2 * h, w),
            new Vector3((d + r), -2 * h, 0),
            new Vector3(r, -3 * h, w),
            new Vector3(r, -3 * h, 0),
            
            // ground
            new Vector3(-100, -4 * h, -100),
            new Vector3(-100, -4 * h, 100),
            new Vector3(100, -4 * h, -100),
            new Vector3(100, -4 * h, 100),
        };
        
        triangleIndices = new List<int>
        {
            // rect 0
            0, 1, 2,
            2, 1, 3,
            
            // rect 1
            4, 5, 6,
            6, 5, 7,
            
            // rect 2
            8, 9, 10,
            10, 9, 11,
            
            // rect 3
            12, 13, 14,
            14, 13, 15,
            
            // ground
            16, 17, 18,
            18, 17, 19,
        };
        
        triangles = new List<Triangle>();
        for (int i = 0; i < triangleIndices.Count; i += 3)
        {
            triangles.Add(new Triangle(
                transform.TransformPoint(vertices[triangleIndices[i]]), 
                transform.TransformPoint(vertices[triangleIndices[i + 1]]), 
                transform.TransformPoint(vertices[triangleIndices[i + 2]]),
                i / 3
            ));
        }
    }
    
    private void Update()
    {
        // Only render meshes
        RenderBox();
    }
    
    /// <summary>
    /// Render dynamically created meshes
    /// </summary>
    private void RenderBox()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangleIndices.ToArray();

        mesh.Optimize();
        mesh.RecalculateNormals();
    }

}
