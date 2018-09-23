using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------
//Marching cubes algorithm optimized for generating indexed unity mesh
//http://paulbourke.net/geometry/polygonise/
//--------------------------------------------------------------------------
public class MarchingCubes
{
    public struct Vector3i
    {
        public int x;
        public int y;
        public int z;

        public Vector3i(int _x, int _y, int _z)
        {
            x = _x; y = _y; z = _z;
        }

        public static Vector3i operator+(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
        }
    }

    //local coordinate of a vertex within a cell
    public static Vector3i[] kVertCoords = new Vector3i[]
    {
        new Vector3i(0,0,0),
        new Vector3i(1,0,0),
        new Vector3i(1,0,1),
        new Vector3i(0,0,1),
        new Vector3i(0,1,0),
        new Vector3i(1,1,0),
        new Vector3i(1,1,1),
        new Vector3i(0,1,1),
    };

    //mapping from index of vert/edge in a cell to index in global table
    //for cell vert/edge numbering, see the marching cubes page
    static int CellVertToGlobalVert(int gridDims, Vector3i cellCoord, int cellVertIdx)
    {
        int vertDims = gridDims + 1;
        Vector3i vertCoord = cellCoord + kVertCoords[cellVertIdx];
        return vertCoord.x + (vertCoord.y + vertDims * vertCoord.z) * vertDims;
    }
    static int CellEdgeToGlobalEdge(int gridDims, Vector3i cellCoord, int cellEdgeIdx)
    {
        return 0;
    }

    //Get vertex position on edge based on 2 corner vertices + 2 field values
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

    public struct TRIANGLE
    {
        public Vector3[] p;
    };

    public struct GRIDCELL
    {
        public Vector3[] p; //grid cell corner positions (8 long)
        public float[] val; //grid cell corner field values (8 long)
    };

    /*
       Given a grid cell and an isolevel, calculate the triangular
       facets required to represent the isosurface through the cell.
       Return the number of triangular facets, the array "triangles"
       will be loaded up with the vertices at most 5 triangular facets.
        0 will be returned if the grid cell is either totally above
       of totally below the isolevel.
    */
    public static int Polygonise(GRIDCELL grid, float isolevel, TRIANGLE[] triangles)
    {
        int i, ntriang;
        int cubeindex;
        Vector3[] vertlist = new Vector3[12];

        int[] edgeTable = MarchingCubesConstants.edgeTable;
        int[,] triTable = MarchingCubesConstants.triTable;

        /*
           Determine the index into the edge table which
           tells us which vertices are inside of the surface
        */
        cubeindex = 0;
        if (grid.val[0] < isolevel) cubeindex |= 1;
        if (grid.val[1] < isolevel) cubeindex |= 2;
        if (grid.val[2] < isolevel) cubeindex |= 4;
        if (grid.val[3] < isolevel) cubeindex |= 8;
        if (grid.val[4] < isolevel) cubeindex |= 16;
        if (grid.val[5] < isolevel) cubeindex |= 32;
        if (grid.val[6] < isolevel) cubeindex |= 64;
        if (grid.val[7] < isolevel) cubeindex |= 128;

        /* Cube is entirely in/out of the surface */
        if (edgeTable[cubeindex] == 0)
            return (0);

        /* Find the vertices where the surface intersects the cube */
        if ((edgeTable[cubeindex] & 1) != 0)
            vertlist[0] = VertexInterp(isolevel, grid.p[0], grid.p[1], grid.val[0], grid.val[1]);
        if ((edgeTable[cubeindex] & 2) != 0)
            vertlist[1] = VertexInterp(isolevel, grid.p[1], grid.p[2], grid.val[1], grid.val[2]);
        if ((edgeTable[cubeindex] & 4) != 0)
            vertlist[2] = VertexInterp(isolevel, grid.p[2], grid.p[3], grid.val[2], grid.val[3]);
        if ((edgeTable[cubeindex] & 8) != 0)
            vertlist[3] = VertexInterp(isolevel, grid.p[3], grid.p[0], grid.val[3], grid.val[0]);
        if ((edgeTable[cubeindex] & 16) != 0)
            vertlist[4] = VertexInterp(isolevel, grid.p[4], grid.p[5], grid.val[4], grid.val[5]);
        if ((edgeTable[cubeindex] & 32) != 0)
            vertlist[5] = VertexInterp(isolevel, grid.p[5], grid.p[6], grid.val[5], grid.val[6]);
        if ((edgeTable[cubeindex] & 64) != 0)
            vertlist[6] = VertexInterp(isolevel, grid.p[6], grid.p[7], grid.val[6], grid.val[7]);
        if ((edgeTable[cubeindex] & 128) != 0)
            vertlist[7] = VertexInterp(isolevel, grid.p[7], grid.p[4], grid.val[7], grid.val[4]);
        if ((edgeTable[cubeindex] & 256) != 0)
            vertlist[8] = VertexInterp(isolevel, grid.p[0], grid.p[4], grid.val[0], grid.val[4]);
        if ((edgeTable[cubeindex] & 512) != 0)
            vertlist[9] = VertexInterp(isolevel, grid.p[1], grid.p[5], grid.val[1], grid.val[5]);
        if ((edgeTable[cubeindex] & 1024) != 0)
            vertlist[10] = VertexInterp(isolevel, grid.p[2], grid.p[6], grid.val[2], grid.val[6]);
        if ((edgeTable[cubeindex] & 2048) != 0)
            vertlist[11] = VertexInterp(isolevel, grid.p[3], grid.p[7], grid.val[3], grid.val[7]);

        /* Create the triangle */
        ntriang = 0;
        for (i = 0; triTable[cubeindex, i] != -1; i += 3)
        {
            triangles[ntriang].p = new Vector3[3];
            triangles[ntriang].p[0] = vertlist[triTable[cubeindex, i]];
            triangles[ntriang].p[1] = vertlist[triTable[cubeindex, i + 1]];
            triangles[ntriang].p[2] = vertlist[triTable[cubeindex, i + 2]];
            ntriang++;
        }

        return (ntriang);
    }

    public delegate float DistanceFunction(Vector3 pos);


    //Build a unity mesh from a distance function
    public static Mesh BuildUnityMesh(Vector3 fieldMin, float fieldSize, int gridDims, DistanceFunction func)
    {
        //calc size of 1 cell
        float cellSize = fieldSize / gridDims;

        //allocate and initialize position / value for every grid vertex
        int vertDims = gridDims + 1;
        Vector3[] gridVertPos = new Vector3[vertDims * vertDims * vertDims];
        float[] gridVertVal = new float[vertDims * vertDims * vertDims];
        for (int x = 0; x < vertDims; x++)
        {
            for (int y = 0; y < vertDims; y++)
            {
                for (int z = 0; z < vertDims; z++)
                {
                    int idx = x + (y + vertDims * z) * vertDims;
                    Vector3 pos = new Vector3(x, y, z);
                    gridVertPos[idx] = pos * cellSize + fieldMin;
                    gridVertVal[idx] = func(gridVertPos[idx]);
                }
            }
        }
                    
        //vertices and indices we build
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        //buffer to contain the generated triangles
        TRIANGLE[] tempTriangles = new TRIANGLE[5];

        //iterate over cells
        GRIDCELL[] cells = new GRIDCELL[gridDims * gridDims * gridDims];
        for (int x = 0; x < gridDims; x++)
        {
            for (int y = 0; y < gridDims; y++)
            {
                for (int z = 0; z < gridDims; z++)
                {
                    //calc cell index 
                    int idx = x + (y + z * gridDims) * gridDims;
                    Vector3i cellCoord = new Vector3i(x, y, z);

                    //fill cell with corners of unit cube
                    GRIDCELL cell = new GRIDCELL();

                    //transform cell corners + calculate field values
                    cell.p = new Vector3[8];
                    cell.val = new float[cell.p.Length];
                    for (int i = 0; i < cell.p.Length; i++)
                    {
                        int globalVertIdx = CellVertToGlobalVert(gridDims, cellCoord, i);
                        cell.p[i] = gridVertPos[globalVertIdx];
                        cell.val[i] = gridVertVal[globalVertIdx];
                    }

                    //polygonise and store vertices (note: had to tweak winding order to make unity happy!)
                    int numTriangles = Polygonise(cell, 0.0f, tempTriangles);
                    for (int i = 0; i < numTriangles; i++)
                    {
                        int vidx = vertices.Count;
                        indices.Add(vidx + 0);
                        indices.Add(vidx + 1);
                        indices.Add(vidx + 2);
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
    }
}
