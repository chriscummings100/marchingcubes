using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------
//Marching cubes algorithm optimized for generating indexed unity mesh
//http://paulbourke.net/geometry/polygonise/
//--------------------------------------------------------------------------
public class MarchingCubes
{

    public static Vector3 VertexInterp(float isolevel, Vector3 p1, Vector3 p2, float valp1, float valp2)
    {
        //handle p1==surface, p2==surface and p1==p2
        if (Mathf.Abs(isolevel - valp1) < 0.00001)
            return (p1);
        if (Mathf.Abs(isolevel - valp2) < 0.00001)
            return (p2);
        if (Mathf.Abs(valp1 - valp2) < 0.00001)
            return (p1);

        //interpolate
        float t = (isolevel - valp1) / (valp2 - valp1);
        return Vector3.Lerp(p1, p2, t);
    }

    public delegate float DistanceFunction(Vector3 pos);

    //Build a unity mesh from a distance function
    public static Mesh BuildUnityMesh(Vector3 fieldMin, float fieldSize, int gridDims, DistanceFunction func)
    {
        /*
        //vertices and indices we build
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        //buffer to contain the generated triangles
        TRIANGLE[] tempTriangles = new MarchingCubesBasicPort.TRIANGLE[5];

        //calc size of 1 cell
        float cellSize = fieldSize / gridDims;

        //iterate over cells
        GRIDCELL[] cells = new MarchingCubesBasicPort.GRIDCELL[gridDims * gridDims * gridDims];
        for (int x = 0; x < gridDims; x++)
        {
            for (int y = 0; y < gridDims; y++)
            {
                for (int z = 0; z < gridDims; z++)
                {
                    //calc cell index 
                    int idx = x + (y + z * gridDims) * gridDims;

                    //fill cell with corners of unit cube
                    GRIDCELL cell = new GRIDCELL();
                    cell.p = new Vector3[]
                    {
                        new Vector3(x+0,y+0,z+0),
                        new Vector3(x+0,y+0,z+1),
                        new Vector3(x+1,y+0,z+1),
                        new Vector3(x+1,y+0,z+0),
                        new Vector3(x+0,y+1,z+0),
                        new Vector3(x+0,y+1,z+1),
                        new Vector3(x+1,y+1,z+1),
                        new Vector3(x+1,y+1,z+0)
                    };

                    //transform cell corners + calculate field values
                    cell.val = new float[cell.p.Length];
                    for (int i = 0; i < cell.p.Length; i++)
                    {
                        cell.p[i] = cell.p[i] * cellSize + fieldMin;
                        cell.val[i] = func(cell.p[i]);
                    }

                    //polygonise and store vertices (note: had to tweak winding order to make unity happy!)
                    int numTriangles = Polygonise(cell, 0.0f, tempTriangles);
                    for (int i = 0; i < numTriangles; i++)
                    {
                        int vidx = vertices.Count;
                        indices.Add(vidx + 0);
                        indices.Add(vidx + 2);
                        indices.Add(vidx + 1);
                        vertices.Add(tempTriangles[i].p[0]);
                        vertices.Add(tempTriangles[i].p[1]);
                        vertices.Add(tempTriangles[i].p[2]);
                    }
                }
            }
        }

        //build normals using central differences of field
        float epsilon = 0.0001f;
        Vector3 xd = Vector3.right * epsilon;
        Vector3 yd = Vector3.up * epsilon;
        Vector3 zd = Vector3.forward * epsilon;
        List<Vector3> normals = new List<Vector3>(vertices.Count);
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 n = new Vector3(
                func(vertices[i] + xd) - func(vertices[i] - xd),
                func(vertices[i] + yd) - func(vertices[i] - yd),
                func(vertices[i] + zd) - func(vertices[i] - zd));
            normals.Add(n.normalized);
        }

        //fill and return unity mesh
        Mesh m = new Mesh();
        m.Clear();
        m.SetVertices(vertices);
        m.SetNormals(normals);
        m.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        return m;
        */
        return null;
    }
}
