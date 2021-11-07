using System.Collections;
using UnityEngine;

public class MeshGenerator
{
   public static MeshData GenerateTerainMesh(float[,] heightMap,float heightMultiplier, AnimationCurve _hieghtCurve, int LOD) 
    {
        AnimationCurve heightCurve = new AnimationCurve(_hieghtCurve.keys);

        int meshSimplificationIncrement = (LOD == 0) ? 1 : LOD * 2;

        int borderedSize = heightMap.GetLength(0);
        int meshSize = borderedSize - 2 * meshSimplificationIncrement;
        int meshSizeUnsimplified = borderedSize - 2;

        // we want that the mesh create if center so we have to create this offset
        float topLeftX = (meshSizeUnsimplified - 1) / -2f;
        float topleftZ = (meshSizeUnsimplified - 1) / 2f;

     
        int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;


        MeshData meshData = new MeshData(verticesPerLine);


        int[,] vertexIndicesMap = new int[borderedSize,borderedSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;


        for (int y = 0; y < borderedSize; y+= meshSimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
            {
                bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

                if (isBorderVertex)
                {
                    vertexIndicesMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                }
                else
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }

        }

        for (int y = 0; y < borderedSize; y+= meshSimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x+= meshSimplificationIncrement)
            {

                int vertexIndex = vertexIndicesMap[x, y];

                Vector2 percent = new Vector2( (x - meshSimplificationIncrement) / (float)meshSize,( y - meshSimplificationIncrement )/ (float)meshSize);
                float height = heightMultiplier * heightCurve.Evaluate(heightMap[x, y]);
                Vector3 vertexPosition  = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topleftZ - percent.y * meshSizeUnsimplified);
               
                meshData.AddVertex(vertexPosition,percent,vertexIndex);


                if (x < borderedSize -1 && y < borderedSize -1 )
                {
                    int a = vertexIndicesMap[x, y];
                    int b = vertexIndicesMap[x + meshSimplificationIncrement, y];
                    int c = vertexIndicesMap[x, y + meshSimplificationIncrement];
                    int d = vertexIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];

                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);

                   // meshData.AddTriangle(vertexIndex,vertexIndex+verticesPerLine + 1,vertexIndex + verticesPerLine);
                    // meshData.AddTriangle(vertexIndex + verticesPerLine + 1,vertexIndex,vertexIndex  +1);
                }

                vertexIndex++;

            }
        }

        meshData.BakeNormals();
        return meshData;
    }
}

public class MeshData
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Vector3[] backedNormals;

    Vector3[] borderVertices;
    int[] borderTriangles;


    int borderTraingleIndex;
    int triangleIndex;

    public MeshData(int verticesPerLine)
    {
        vertices = new Vector3[verticesPerLine * verticesPerLine];
        uvs = new Vector2[verticesPerLine * verticesPerLine];
        // a triangle is conpose of three vertices so the numbers of triangles we need is
        // the number of vertices -1 * 6
        triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

        borderVertices = new Vector3[verticesPerLine * 4 * 4];
        borderTriangles = new int[24 * verticesPerLine];

    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            borderVertices[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            borderTriangles[borderTraingleIndex] = a;
            borderTriangles[borderTraingleIndex + 1] = b;
            borderTriangles[borderTraingleIndex + 2] = c;
            borderTraingleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }

    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA,vertexIndexB,vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
            
        }



        int BordertriangleCount = borderTriangles.Length / 3;
        for (int i = 0; i < BordertriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = borderTriangles[normalTriangleIndex];
            int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
            int vertexIndexC = borderTriangles[normalTriangleIndex + 2];



            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            
            if(vertexIndexA >= 0)
            {
                vertexNormals[vertexIndexA] += triangleNormal;
            }

            if (vertexIndexB >= 0)
            {
                vertexNormals[vertexIndexB] += triangleNormal;
            }

            if (vertexIndexC >= 0)
            {
                vertexNormals[vertexIndexC] += triangleNormal;
            }
           
          

        }



        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();

        }


        return vertexNormals;

    }


    Vector3 SurfaceNormalFromIndices(int indexA,int indexB,int indexC)
    {
        Vector3 A = (indexA<0)?borderVertices[-indexA-1] : vertices [indexA];
        Vector3 B = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
        Vector3 C = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

        Vector3 sizeAB = B - A;
        Vector3 sizeAC = C - A;
        return Vector3.Cross(sizeAB, sizeAC).normalized;
    }

    public void BakeNormals()
    {
        backedNormals = CalculateNormals();
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = backedNormals;



        return mesh;
    }

}
