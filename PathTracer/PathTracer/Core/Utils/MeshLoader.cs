using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    static class MeshLoader
    {
        public static List<Triangle> LoadMesh(string path, Vector3 position, Vector3 euler, Vector3 scale, List<Shader> shaders)
        {
            Matrix matrix = Matrix.TRS(position, euler, scale);

            return LoadMesh(path, matrix, shaders);
        }

        static List<Triangle> LoadMesh(string path, Matrix matrix, List<Shader> shaders)
        {
            if(System.IO.File.Exists(path) == false)
                return null;
            List<Triangle> triangles = new List<Triangle>();
            int sdindex = -1;
            using (StreamReader reader = new StreamReader(path))
            {

                List<Vector3> vlist = new List<Vector3>();
                List<Vector3> nlist = new List<Vector3>();
                List<Vector2> ulist = new List<Vector2>();
                List<int> flist = new List<int>();

                Vector4[] tangents = null;
                int[] tangentCounts = null;

                while (reader.EndOfStream == false)
                {
                    string line = reader.ReadLine();
                    if (line.Length > 1)
                    {
                        char df0 = line[0];
                        char df1 = line[1];

                        if (df0 == 'v')
                        {
                            string[] sp = line.Substring(2).Trim().Split(' ');
                            if (df1 == 'n')
                            {
                                double nx = -double.Parse(sp[0]);
                                double ny = double.Parse(sp[1]);
                                double nz = double.Parse(sp[2]);
                                nlist.Add(new Vector3(nx, ny, nz));
                            }
                            else if (df1 == 't')
                            {
                                double ux = double.Parse(sp[0]);
                                double uy = double.Parse(sp[1]);
                                ulist.Add(new Vector2(ux, uy));
                            }
                            else
                            {
                                double px = -double.Parse(sp[0]);
                                double py = double.Parse(sp[1]);
                                double pz = double.Parse(sp[2]);
                                vlist.Add(new Vector3(px, py, pz));
                            }
                        }
                        else if (df0 == 'g')
                        {
                            sdindex += 1;
                        }
                        else if (df0 == 'f')
                        {
                            string[] sp = line.Split(' ');
                            string[] face0 = sp[1].Split('/');
                            string[] face1 = sp[2].Split('/');
                            string[] face2 = sp[3].Split('/');

                            int vindex0 = int.Parse(face0[0]) - 1;
                            int vindex1 = int.Parse(face1[0]) - 1;
                            int vindex2 = int.Parse(face2[0]) - 1;

                            int uindex0 = int.Parse(face0[1]) - 1;
                            int uindex1 = int.Parse(face1[1]) - 1;
                            int uindex2 = int.Parse(face2[1]) - 1;

                            int nindex0 = int.Parse(face0[2]) - 1;
                            int nindex1 = int.Parse(face1[2]) - 1;
                            int nindex2 = int.Parse(face2[2]) - 1;

                            Vector3 v0 = matrix.TransformPoint(vlist[vindex0]);
                            Vector3 v1 = matrix.TransformPoint(vlist[vindex2]);
                            Vector3 v2 = matrix.TransformPoint(vlist[vindex1]);

                            Vector3 n0 = matrix.TransformVector(nlist[nindex0]);
                            Vector3 n1 = matrix.TransformVector(nlist[nindex2]);
                            Vector3 n2 = matrix.TransformVector(nlist[nindex1]);

                            Vector2 uv0 = ulist[uindex0];
                            Vector2 uv1 = ulist[uindex2];
                            Vector2 uv2 = ulist[uindex1];

                            Vector4 tangent0 = CalculateTangent(v0, v1, v2, uv0, uv1, uv2, n0);
                            Vector4 tangent1 = CalculateTangent(v0, v1, v2, uv0, uv1, uv2, n1);
                            Vector4 tangent2 = CalculateTangent(v0, v1, v2, uv0, uv1, uv2, n2);

                            if (tangents == null)
                                tangents = new Vector4[nlist.Count];
                            if (tangentCounts == null)
                                tangentCounts = new int[nlist.Count];
                            flist.Add(nindex0);
                            flist.Add(nindex2);
                            flist.Add(nindex1);
                            tangents[nindex0] += tangent0;
                            tangents[nindex2] += tangent1;
                            tangents[nindex1] += tangent2;
                            tangentCounts[nindex0] += 1;
                            tangentCounts[nindex2] += 1;
                            tangentCounts[nindex1] += 1;

                            Vertex vertex0 = new Vertex {position = v0, uv = uv0, normal = n0};
                            Vertex vertex1 = new Vertex {position = v1, uv = uv1, normal = n1};
                            Vertex vertex2 = new Vertex {position = v2, uv = uv2, normal = n2};

                            //if (sdindex >= 0 && sdindex < shaders.Count)
                            {
                                Triangle triangle = new Triangle(vertex0, vertex1, vertex2, shaders[0]);

                                triangles.Add(triangle);
                            }
                        }
                    }
                }

                int tcount = flist.Count / 3;
                for(int i=0;i< tcount; i++)
                {
                    int tangentIndex0 = flist[i * 3];
                    int tangentIndex1 = flist[i * 3 + 1];
                    int tangentIndex2 = flist[i * 3 + 2];
                    Vector4 tangent0 = tangents[tangentIndex0] / tangentCounts[tangentIndex0];
                    Vector4 tangent1 = tangents[tangentIndex1] / tangentCounts[tangentIndex1];
                    Vector4 tangent2 = tangents[tangentIndex2] / tangentCounts[tangentIndex2];

                    Triangle triangle = triangles[i];

                    triangle.vertex0.tangent = tangent0;
                    triangle.vertex1.tangent = tangent1;
                    triangle.vertex2.tangent = tangent2;

                    triangles[i] = triangle;
                }
            }

            return triangles;
        }

        static Vector4 CalculateTangent(Vector3 v0, Vector3 v1, Vector3 v2, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector3 normal)
        {
            double x1 = v1.x - v0.x;
            double x2 = v2.x - v0.x;
            double y1 = v1.y - v0.y;
            double y2 = v2.y - v0.y;
            double z1 = v1.z - v0.z;
            double z2 = v2.z - v0.z;
            double s1 = uv1.x - uv0.x;
            double s2 = uv2.x - uv0.x;
            double t1 = uv1.y - uv0.y;
            double t2 = uv2.y - uv0.y;
            double r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            Vector3 tmp = (sdir - normal * Vector3.Dot(normal, sdir)).normalized;

            double sign = Vector3.Dot(Vector3.Cross(normal, sdir), tdir) > 0.0 ? -1.0 : 1.0;

            return new Vector4(tmp.x, tmp.y, tmp.z, sign);
        }
    }
}
