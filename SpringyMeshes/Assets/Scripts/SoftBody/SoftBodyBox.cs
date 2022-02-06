using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Older representation of SoftBody Cube - kept for legacy reasons
/// 
/// Strut is only represented by damped spring - no torsional springs for volume conservation.
/// </summary>
public class SoftBodyBox : SoftBody
{
    protected override void InitObject()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        float size = 10.0f;
        particles = new List<SBNode>
        {
            new SBNode(new Vector3(0, 0, 0), 1.0f),
            new SBNode(new Vector3(size, 0, 0), 1.0f),
            new SBNode(new Vector3(size, size, 0), 1.0f),
            new SBNode(new Vector3(0, size, 0), 1.0f),
            new SBNode(new Vector3(0, size, size), 1.0f),
            new SBNode(new Vector3(size, size, size), 1.0f),
            new SBNode(new Vector3(size, 0, size), 1.0f),
            new SBNode(new Vector3(0, 0, size), 1.0f),
        };
        //
        // particles[2].IsFixed = true;
        // particles[3].IsFixed = true;
        // particles[4].IsFixed = true;
        // particles[5].IsFixed = true;

        dampedSprings = new List<SBDampedSpring>
        {
            // vertical
            new SBDampedSpring(particles[1], particles[2], k, d),
            new SBDampedSpring(particles[3], particles[0], k, d),
            new SBDampedSpring(particles[5], particles[6], k, d),
            new SBDampedSpring(particles[4], particles[7], k, d),

            // vertical diagonal
            new SBDampedSpring(particles[0], particles[2], k, d),
            new SBDampedSpring(particles[1], particles[3], k, d),
            new SBDampedSpring(particles[1], particles[5], k, d),
            new SBDampedSpring(particles[2], particles[6], k, d),
            new SBDampedSpring(particles[4], particles[6], k, d),
            new SBDampedSpring(particles[5], particles[7], k, d),
            new SBDampedSpring(particles[0], particles[4], k, d),
            new SBDampedSpring(particles[3], particles[7], k, d),


            // horizontal bottom
            new SBDampedSpring(particles[0], particles[1], k, d),
            new SBDampedSpring(particles[1], particles[6], k, d),
            new SBDampedSpring(particles[6], particles[7], k, d),
            new SBDampedSpring(particles[0], particles[7], k, d),

            // horizontal bottom diagonal
            new SBDampedSpring(particles[0], particles[6], k, d),
            new SBDampedSpring(particles[1], particles[7], k, d),

            // horizontal top
            new SBDampedSpring(particles[2], particles[3], k, d),
            new SBDampedSpring(particles[2], particles[5], k, d),
            new SBDampedSpring(particles[3], particles[4], k, d),
            new SBDampedSpring(particles[4], particles[5], k, d),

            // horizontal top diagonal
            new SBDampedSpring(particles[2], particles[4], k, d),
            new SBDampedSpring(particles[3], particles[5], k, d),


            // new SBDampedSpring(particles[0], particles[5], k, d),
            // new SBDampedSpring(particles[1], particles[4], k, d),
            // new SBDampedSpring(particles[3], particles[6], k, d),
            // new SBDampedSpring(particles[2], particles[7], k, d),
        };

        triangles = new List<int>
        {
            0, 2, 1, //face front
            0, 3, 2,
            2, 3, 4, //face top
            2, 4, 5,
            1, 2, 5, //face right
            1, 5, 6,
            0, 7, 4, //face left
            0, 4, 3,
            5, 4, 7, //face back
            5, 7, 6,
            0, 6, 7, //face bottom
            0, 1, 6
        };
    }
}