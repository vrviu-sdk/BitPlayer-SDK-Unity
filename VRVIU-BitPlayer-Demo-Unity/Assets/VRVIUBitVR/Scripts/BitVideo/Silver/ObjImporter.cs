namespace VRVIU.BitVRPlayer.BitVideo.Silver
{
    using UnityEngine;

    using System.Collections.Generic;
    using System.IO;

    public class ObjImporter
    {
        public static Mesh ImportMesh(string meshString)
        {
            Mesh mesh = new Mesh();

            StringReader reader = new StringReader(meshString);

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            List<int> triangles = new List<int>();

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }
                string[] parts = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    try
                    {
                        switch (parts[0])
                        {
                            case "g":
                                {          // Object name
                                    mesh.name = parts[1];
                                    break;
                                }
                            case "v":
                                {          // Vertex
                                    float x, y, z;
                                    x = float.Parse(parts[1]);
                                    y = float.Parse(parts[2]);
                                    z = -float.Parse(parts[3]);
                                    vertices.Add(new Vector3(x, y, z));
                                    break;
                                }
                            case "vn":
                                {         // Normal
                                    float nx, ny, nz;
                                    nx = -float.Parse(parts[1]);
                                    ny = -float.Parse(parts[2]);
                                    nz = float.Parse(parts[3]);
                                    normals.Add(new Vector3(nx, ny, nz));
                                    break;
                                }
                            case "vt":
                                {          // Texture coordinate
                                    float u, v;
                                    u = float.Parse(parts[1]);
                                    v = float.Parse(parts[2]);
                                    texCoords.Add(new Vector2(u, v));
                                    break;
                                }
                            case "f":
                                {          // Face
                                           // *BW - note, only parsing out the vertex index; not bothering about normal or uv index, assuming the same.
                                    for (int tri = 1; tri < parts.Length; tri++)
                                    {
                                        string[] faceParts = parts[tri].Split('/');
                                        triangles.Add(int.Parse(faceParts[0]) - 1);
                                    }
                                    break;
                                }
                            case "usemtl":      // Material
                                break;
                            case "usemap":      // Map
                                break;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Unable to parse " + line + " exception " + e);
                        return null;
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = texCoords.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            ;

            Debug.Log("Built shape " + mesh.name + " with " + vertices.Count + " vertices, " + normals.Count + " normals, " + texCoords.Count + " uv's and " + triangles.Count + " triangles.");

            return mesh;
        }
    }

}