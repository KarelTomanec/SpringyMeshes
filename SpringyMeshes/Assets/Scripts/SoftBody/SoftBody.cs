using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Older representation of SoftBody Base Class - kept for legacy reasons
/// 
/// Strut is only represented by damped spring - no torsional springs for volume conservation.
/// </summary>
public abstract class SoftBody : MonoBehaviour
{
    public bool drawGizmos = false;
    public Material mat;
    
    [Range(0, 50)]
    public float k = 4.0f * (float) (Math.PI * Math.PI);
    [Range(0, 20)]
    public float d = 0.8f * (float) Math.PI;

    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    protected Mesh mesh;

    protected List<SBNode> particles;
    protected List<SBDampedSpring> dampedSprings;
    protected List<int> triangles;

    protected virtual void OnEnable()
    {
        InitObject();
    }

    protected virtual void Start()
    {
        Render();
    }

    protected virtual void Update()
    {
        Render();
    }

    protected virtual void FixedUpdate()
    {
        // Loop over all of the particles, setting each particle’s force to the accumulation
        // of all external forces acting directly on each particle, such as air drag, friction,
        // or gravity
        // gravity
        for (int i = 0; i < particles.Count; i++) {
            particles[i].AddForce(transform.InverseTransformVector(Physics.gravity * particles[i].Mass));
        }
        
        // air drag
        // float d = 1.0f;
        // for (int i = 0; i < particles.Count; i++)
        // {
        //     particles[i].AddForce(- d  * particles[i].Velocity);
        // }

        // wind
        // float u = UnityEngine.Random.value;
        // float v = UnityEngine.Random.value;
        // for (int i = 0; i < particles.Count; i++)
        // {
        //     particles[i].AddForce(d * (u * Vector3.right + v * Vector3.forward) * 4.0f);
        // }


        // Loop over all of the struts, adding each strut’s spring and damper forces to
        // the forces acting on the two particles it is connected to
        for (int i = 0; i < dampedSprings.Count; i++) {
            dampedSprings[i].ApplyForces();
        }


        // Loop over all of the particles, dividing the particle’s total applied force by its
        // mass to obtain its acceleration
        for (int i = 0; i < particles.Count; i++) {
            particles[i].Tick(Time.fixedDeltaTime, transform);
        }
    }

    protected abstract void InitObject();

    protected virtual void Render()
    {
        mesh.Clear();

        List<Vector3> positions = new List<Vector3>();
        foreach (var node in particles) {
            positions.Add(node.Position);
        }

        mesh.vertices = positions.ToArray();
        // TODO: not necessary to udpate triangles
        mesh.triangles = triangles.ToArray();

        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
    }

    protected virtual void OnDrawGizmos()
    {
        if (drawGizmos) {
            if (particles != null) {
                for (int i = 0; i < particles.Count; i++) {
                    if (particles[i].isAtRestFlag) {
                        Gizmos.color = Color.green;
                    } else {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawSphere(transform.TransformPoint(particles[i].Position), 0.05f);
                }

                for (int i = 0; i < dampedSprings.Count; i++) {
                    Gizmos.color = Color.cyan;
                    Vector3 dir = transform.TransformPoint(dampedSprings[i].GetEndPosition()) -
                                  transform.TransformPoint(dampedSprings[i].GetStartPosition());
                    DrawArrow.ForGizmo(transform.TransformPoint(dampedSprings[i].GetStartPosition()), dir);
                    //Gizmos.DrawLine(transform.TransformPoint(dampedSprings[i].GetStartPosition()), transform.TransformPoint(dampedSprings[i].GetEndPosition()));
                }
            }
        }
    }
    
    protected int FindIndexOfClosestHelper(Vector3 pos, List<VertexHelper> vertexHelpers)
    {
        int index = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < vertexHelpers.Count; i++) {
            float dist = Vector3.Distance(vertexHelpers[i].position, pos);

            if (dist < minDist) {
                minDist = dist;
                index = i;
            }
        }

        return index;
    }
    
    protected struct EdgeHelper
    {
        public int from;
        public int to;

        public EdgeHelper(int f, int t)
        {
            this.from = f;
            this.to = t;
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is EdgeHelper))
                return false;

            EdgeHelper other = (EdgeHelper) obj;
            
            return (this.from == other.from && this.to == other.to) || (this.from == other.to && this.to == other.from);
        }
        
        public override int GetHashCode()
        {
            return this.from.GetHashCode() * 17 + this.to.GetHashCode();
        }
    }

    protected class VertexHelperComparer : IEqualityComparer<VertexHelper>
    {
        public bool Equals(VertexHelper one, VertexHelper two)
        {
            return Vector3.Distance(one.position, two.position) < 0.01f;
        }

        public int GetHashCode(VertexHelper item)
        {
            return item.GetHashCode();
        }
    }

    protected struct VertexHelper
    {
        public Vector3 position;
        public int globalIndex;
        public int editedIndex;

        public VertexHelper(Vector3 position, int globalIndex, int editedIndex)
        {
            this.position = position;
            this.globalIndex = globalIndex;
            this.editedIndex = editedIndex;
        }
    }
}