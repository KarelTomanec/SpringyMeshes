using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Older representation of SoftBody Mesh Model - kept for legacy reasons
/// 
/// Strut is only represented by damped spring - no torsional springs for volume conservation.
/// </summary>
public class SoftBodyModel : SoftBody
{
    public GameObject model;

    protected override void InitObject()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = model.GetComponent<MeshFilter>().mesh;
        mesh = meshFilter.mesh;


        particles = new List<SBNode>();
        dampedSprings = new List<SBDampedSpring>();
        triangles = new List<int>();

        List<VertexHelper> vertexIndexHelper = new List<VertexHelper>();
        var originalVertices = mesh.vertices;
        for (int index = 0; index < originalVertices.Length; index++) {
            bool isNodeClose = vertexIndexHelper.Any(item => {
                float dist = Vector3.Distance(item.position, originalVertices[index]);
                return dist < 0.01f;
            });


            if (!isNodeClose) {
                particles.Add(new SBNode(originalVertices[index], 0.1f));
                vertexIndexHelper.Add(new VertexHelper(originalVertices[index], index, particles.Count - 1));
            } else {
                //Debug.Log("Duplicate of node!");
            }
        }

        Debug.Log("Particle count: " + particles.Count);

        List<EdgeHelper> springsInMesh = new List<EdgeHelper>();
        List<int> editedIndexCache = new List<int>();
        for (int triangleIndex = 0; triangleIndex < mesh.triangles.Length; triangleIndex += 3) {
            int editedIndex0 =
                vertexIndexHelper[
                        FindIndexOfClosestHelper(mesh.vertices[mesh.triangles[triangleIndex]], vertexIndexHelper)]
                    .editedIndex;
            int editedIndex1 =
                vertexIndexHelper[
                        FindIndexOfClosestHelper(mesh.vertices[mesh.triangles[triangleIndex + 1]],
                            vertexIndexHelper)]
                    .editedIndex;
            int editedIndex2 =
                vertexIndexHelper[
                        FindIndexOfClosestHelper(mesh.vertices[mesh.triangles[triangleIndex + 2]],
                            vertexIndexHelper)]
                    .editedIndex;

            editedIndexCache.Add(editedIndex0);
            editedIndexCache.Add(editedIndex1);
            editedIndexCache.Add(editedIndex2);


            if (editedIndex0 == editedIndex1 || editedIndex1 == editedIndex2 || editedIndex0 == editedIndex2) {
                Debug.Log("Dva stejný cigáne");
            }

            EdgeHelper edgeHelper = new EdgeHelper(editedIndex0, editedIndex1);
            if (!springsInMesh.Any(x => x.Equals(edgeHelper))) {
                dampedSprings.Add(new SBDampedSpring(particles[editedIndex0], particles[editedIndex1]));
                springsInMesh.Add(edgeHelper);
            }

            edgeHelper = new EdgeHelper(editedIndex1, editedIndex2);
            if (!springsInMesh.Any(x => x.Equals(edgeHelper))) {
                dampedSprings.Add(new SBDampedSpring(particles[editedIndex1], particles[editedIndex2]));
                springsInMesh.Add(edgeHelper);
            }

            edgeHelper = new EdgeHelper(editedIndex2, editedIndex0);
            if (!springsInMesh.Any(x => x.Equals(edgeHelper))) {
                dampedSprings.Add(new SBDampedSpring(particles[editedIndex2], particles[editedIndex0]));
                springsInMesh.Add(edgeHelper);
            }
        }

        List<HashSet<int>> triangleHelpers = new List<HashSet<int>>();
        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            HashSet<int> tri = new HashSet<int>();
            tri.Add(editedIndexCache[i]);
            tri.Add(editedIndexCache[i + 1]);
            tri.Add(editedIndexCache[i + 2]);

            if (!triangleHelpers.Contains(tri)) {
                triangles.Add(editedIndexCache[i]);
                triangles.Add(editedIndexCache[i + 1]);
                triangles.Add(editedIndexCache[i + 2]);

                triangleHelpers.Add(tri);
            }
        }

        Debug.Log("Spring count: " + dampedSprings.Count);
        Debug.Log("Triangle vertind count: " + triangles.Count);
    }
}