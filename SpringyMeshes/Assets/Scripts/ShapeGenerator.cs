using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class providing Meshes for some basic shapes (Cube, Tetrahedron, IcoSphere)
/// </summary>
public static class ShapeGenerator
{
    public enum ShapeType
    {
        Cube,
        Tetrahedron
    };

    /// <summary>
    /// Cube defined by 8 vertices, with given size
    /// </summary>
    public static class Cube
    {
        public static Mesh Create(float size)
        {
            Mesh mesh = new Mesh {
                name = "Cube",
                vertices = new List<Vector3> {
                    new Vector3(0, 0, 0),
                    new Vector3(size, 0, 0),
                    new Vector3(size, size, 0),
                    new Vector3(0, size, 0),
                    new Vector3(0, size, size),
                    new Vector3(size, size, size),
                    new Vector3(size, 0, size),
                    new Vector3(0, 0, size),
                }.ToArray(),
                triangles = new List<int> {
                    0, 2, 1, // f0
                    0, 3, 2, // f1
                    2, 3, 4, // f2
                    2, 4, 5, // f3
                    1, 2, 5, // f4
                    1, 5, 6, // f5
                    0, 7, 3, // f6 
                    7, 4, 3, // f7
                    6, 4, 7, // f8 
                    5, 4, 6, // f9
                    0, 6, 7, // f10 
                    0, 1, 6 // f11
                }.ToArray()
            };

            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }
    }

    /// <summary>
    /// Tetrahedron defined by its 4 vertices and 4 faces with given size
    /// </summary>
    public static class Tetrahedron
    {
        public static Mesh Create(float size)
        {
            Mesh mesh = new Mesh {
                name = "Tetrahedron",
                vertices = new List<Vector3> {
                    new Vector3(0, 0, 0),
                    new Vector3(size, 0, 0),
                    new Vector3(0, 0, size),
                    new Vector3(0, size, 0)
                }.ToArray(),
                triangles = new List<int> {
                    0, 1, 2, // floor
                    0, 3, 1, // XY
                    0, 2, 3, // ZY
                    1, 3, 2
                }.ToArray()
            };

            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }
    }


    /// <summary>
    /// IcoSphere with different level of subdivision and radius
    /// </summary>
    public static class IcoSphere
    {
        // From https://github.com/kaiware007/IcoSphereCreator
        public static Mesh Create(int n, float radius)
        {
            int nn = n * 4;
            int vertexNum = (nn * nn / 16) * 24;
            Vector3[] vertices = new Vector3[vertexNum];
            int[] triangles = new int[vertexNum];
            Vector2[] uv = new Vector2[vertexNum];

            Quaternion[] init_vectors = new Quaternion[24];
            // 0
            init_vectors[0] = new Quaternion(0, 1, 0, 0); //the triangle vertical to (1,1,1)
            init_vectors[1] = new Quaternion(0, 0, 1, 0);
            init_vectors[2] = new Quaternion(1, 0, 0, 0);
            // 1
            init_vectors[3] = new Quaternion(0, -1, 0, 0); //to (1,-1,1)
            init_vectors[4] = new Quaternion(1, 0, 0, 0);
            init_vectors[5] = new Quaternion(0, 0, 1, 0);
            // 2
            init_vectors[6] = new Quaternion(0, 1, 0, 0); //to (-1,1,1)
            init_vectors[7] = new Quaternion(-1, 0, 0, 0);
            init_vectors[8] = new Quaternion(0, 0, 1, 0);
            // 3
            init_vectors[9] = new Quaternion(0, -1, 0, 0); //to (-1,-1,1)
            init_vectors[10] = new Quaternion(0, 0, 1, 0);
            init_vectors[11] = new Quaternion(-1, 0, 0, 0);
            // 4
            init_vectors[12] = new Quaternion(0, 1, 0, 0); //to (1,1,-1)
            init_vectors[13] = new Quaternion(1, 0, 0, 0);
            init_vectors[14] = new Quaternion(0, 0, -1, 0);
            // 5
            init_vectors[15] = new Quaternion(0, 1, 0, 0); //to (-1,1,-1)
            init_vectors[16] = new Quaternion(0, 0, -1, 0);
            init_vectors[17] = new Quaternion(-1, 0, 0, 0);
            // 6
            init_vectors[18] = new Quaternion(0, -1, 0, 0); //to (-1,-1,-1)
            init_vectors[19] = new Quaternion(-1, 0, 0, 0);
            init_vectors[20] = new Quaternion(0, 0, -1, 0);
            // 7
            init_vectors[21] = new Quaternion(0, -1, 0, 0); //to (1,-1,-1)
            init_vectors[22] = new Quaternion(0, 0, -1, 0);
            init_vectors[23] = new Quaternion(1, 0, 0, 0);

            int j = 0; //index on vectors[]

            for (int i = 0; i < 24; i += 3) {
                /*
                 *                   c _________d
                 *    ^ /\           /\        /
                 *   / /  \         /  \      /
                 *  p /    \       /    \    /
                 *   /      \     /      \  /
                 *  /________\   /________\/
                 *     q->       a         b
                 */
                for (int p = 0; p < n; p++) {
                    //edge index 1
                    Quaternion edge_p1 = Quaternion.Lerp(init_vectors[i], init_vectors[i + 2], (float) p / n);
                    Quaternion edge_p2 = Quaternion.Lerp(init_vectors[i + 1], init_vectors[i + 2], (float) p / n);
                    Quaternion edge_p3 = Quaternion.Lerp(init_vectors[i], init_vectors[i + 2], (float) (p + 1) / n);
                    Quaternion edge_p4 = Quaternion.Lerp(init_vectors[i + 1], init_vectors[i + 2], (float) (p + 1) / n);

                    for (int q = 0; q < (n - p); q++) {
                        //edge index 2
                        Quaternion a = Quaternion.Lerp(edge_p1, edge_p2, (float) q / (n - p));
                        Quaternion b = Quaternion.Lerp(edge_p1, edge_p2, (float) (q + 1) / (n - p));
                        Quaternion c, d;
                        if (edge_p3 == edge_p4) {
                            c = edge_p3;
                            d = edge_p3;
                        } else {
                            c = Quaternion.Lerp(edge_p3, edge_p4, (float) q / (n - p - 1));
                            d = Quaternion.Lerp(edge_p3, edge_p4, (float) (q + 1) / (n - p - 1));
                        }

                        triangles[j] = j;
                        vertices[j++] = new Vector3(a.x, a.y, a.z);
                        triangles[j] = j;
                        vertices[j++] = new Vector3(b.x, b.y, b.z);
                        triangles[j] = j;
                        vertices[j++] = new Vector3(c.x, c.y, c.z);
                        if (q < n - p - 1) {
                            triangles[j] = j;
                            vertices[j++] = new Vector3(c.x, c.y, c.z);
                            triangles[j] = j;
                            vertices[j++] = new Vector3(b.x, b.y, b.z);
                            triangles[j] = j;
                            vertices[j++] = new Vector3(d.x, d.y, d.z);
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "IcoSphere";

            CreateUV(n, vertices, uv);
            for (int i = 0; i < vertexNum; i++) {
                vertices[i] *= radius;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            CreateTangents(mesh);

            return mesh;
        }

        static void CreateUV(int n, Vector3[] vertices, Vector2[] uv)
        {
            int tri = n * n; // devided triangle count (1,4,9...)
            int uvLimit = tri * 6; // range of wrap UV.x 
            Debug.Log("tri " + tri + " uvLimit " + uvLimit);

            for (int i = 0; i < vertices.Length; i++) {
                Vector3 v = vertices[i];

                Vector2 textureCoordinates;
                if ((v.x == 0f) && (i < uvLimit)) {
                    textureCoordinates.x = 1f;
                } else {
                    textureCoordinates.x = Mathf.Atan2(v.x, v.z) / (-2f * Mathf.PI);
                }

                if (textureCoordinates.x < 0f) {
                    textureCoordinates.x += 1f;
                }

                textureCoordinates.y = Mathf.Asin(v.y) / Mathf.PI + 0.5f;
                uv[i] = textureCoordinates;
            }

            int tt = tri * 3;
            uv[0 * tt + 0].x = 0.875f;
            uv[1 * tt + 0].x = 0.875f;
            uv[2 * tt + 0].x = 0.125f;
            uv[3 * tt + 0].x = 0.125f;
            uv[4 * tt + 0].x = 0.625f;
            uv[5 * tt + 0].x = 0.375f;
            uv[6 * tt + 0].x = 0.375f;
            uv[7 * tt + 0].x = 0.625f;
        }

        static void CreateTangents(Mesh mesh)
        {
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;
            Vector2[] uv = mesh.uv;
            Vector3[] normals = mesh.normals;

            int triangleCount = triangles.Length;
            int vertexCount = vertices.Length;

            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];

            Vector4[] tangents = new Vector4[vertexCount];

            for (int i = 0; i < triangleCount; i += 3) {
                int i1 = triangles[i + 0];
                int i2 = triangles[i + 1];
                int i3 = triangles[i + 2];

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector2 w1 = uv[i1];
                Vector2 w2 = uv[i2];
                Vector2 w3 = uv[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float r = 1.0f / (s1 * t2 - s2 * t1);

                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }


            for (int i = 0; i < vertexCount; ++i) {
                Vector3 n = normals[i];
                Vector3 t = tan1[i];

                Vector3.OrthoNormalize(ref n, ref t);
                tangents[i].x = t.x;
                tangents[i].y = t.y;
                tangents[i].z = t.z;

                tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
            }

            mesh.tangents = tangents;
        }
    }

    public static class IcoSphereUnity
    {
        private struct TriangleIndices
        {
            public int v1;
            public int v2;
            public int v3;

            public TriangleIndices(int v1, int v2, int v3)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
            }
        }

        // return index of point in the middle of p1 and p2
        private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache,
            float radius)
        {
            // first check if we have it already
            bool firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            long greaterIndex = firstIsSmaller ? p2 : p1;
            long key = (smallerIndex << 32) + greaterIndex;

            int ret;
            if (cache.TryGetValue(key, out ret)) {
                return ret;
            }

            // not in cache, calculate it
            Vector3 point1 = vertices[p1];
            Vector3 point2 = vertices[p2];
            Vector3 middle = new Vector3
            (
                (point1.x + point2.x) / 2f,
                (point1.y + point2.y) / 2f,
                (point1.z + point2.z) / 2f
            );

            // add vertex makes sure point is on unit sphere
            int i = vertices.Count;
            vertices.Add(middle.normalized * radius);

            // store it, return index
            cache.Add(key, i);

            return i;
        }

        public static Mesh Create(int recursionLevel, float radius)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = mesh.vertices;
            List<Vector3> vertList = new List<Vector3>();
            Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

            // create 12 vertices of a icosahedron
            float t = (1f + Mathf.Sqrt(5f)) / 2f;

            vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
            vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
            vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
            vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

            vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
            vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
            vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
            vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

            vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
            vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
            vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
            vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);


            // create 20 triangles of the icosahedron
            List<TriangleIndices> faces = new List<TriangleIndices>();

            // 5 faces around point 0
            faces.Add(new TriangleIndices(0, 11, 5));
            faces.Add(new TriangleIndices(0, 5, 1));
            faces.Add(new TriangleIndices(0, 1, 7));
            faces.Add(new TriangleIndices(0, 7, 10));
            faces.Add(new TriangleIndices(0, 10, 11));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(1, 5, 9));
            faces.Add(new TriangleIndices(5, 11, 4));
            faces.Add(new TriangleIndices(11, 10, 2));
            faces.Add(new TriangleIndices(10, 7, 6));
            faces.Add(new TriangleIndices(7, 1, 8));

            // 5 faces around point 3
            faces.Add(new TriangleIndices(3, 9, 4));
            faces.Add(new TriangleIndices(3, 4, 2));
            faces.Add(new TriangleIndices(3, 2, 6));
            faces.Add(new TriangleIndices(3, 6, 8));
            faces.Add(new TriangleIndices(3, 8, 9));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(4, 9, 5));
            faces.Add(new TriangleIndices(2, 4, 11));
            faces.Add(new TriangleIndices(6, 2, 10));
            faces.Add(new TriangleIndices(8, 6, 7));
            faces.Add(new TriangleIndices(9, 8, 1));


            // refine triangles
            for (int i = 0; i < recursionLevel; i++) {
                List<TriangleIndices> faces2 = new List<TriangleIndices>();
                foreach (var tri in faces) {
                    // replace triangle by 4 triangles
                    int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
                    int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
                    int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

                    faces2.Add(new TriangleIndices(tri.v1, a, c));
                    faces2.Add(new TriangleIndices(tri.v2, b, a));
                    faces2.Add(new TriangleIndices(tri.v3, c, b));
                    faces2.Add(new TriangleIndices(a, b, c));
                }

                faces = faces2;
            }

            mesh.vertices = vertList.ToArray();

            List<int> triList = new List<int>();
            for (int i = 0; i < faces.Count; i++) {
                triList.Add(faces[i].v1);
                triList.Add(faces[i].v2);
                triList.Add(faces[i].v3);
            }

            mesh.triangles = triList.ToArray();
            mesh.uv = new Vector2[vertices.Length];

            Vector3[] normales = new Vector3[vertList.Count];
            for (int i = 0; i < normales.Length; i++)
                normales[i] = vertList[i].normalized;


            mesh.normals = normales;

            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
            //mesh.Optimize();

            return mesh;
        }
    }
}