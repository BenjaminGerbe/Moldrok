using System.Collections;
using UnityEngine;


// This scripts allow us to create the world.

[RequireComponent(typeof(MeshFilter))]
public class TerrainManager : MonoBehaviour
{

    // public properties 
    public int xSize;
    public int zSize;


    // privates properties
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;


   
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new Vector3[0];


         CreateShape();
        UpdateMesh();
    }

    private void Update()
    {
        
    }

    private void CreateShape()
    {


        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= xSize; z++)
        {
            for (int x = 0; x <= zSize; x++)
            {

                float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 5f;
                vertices[i] = new Vector3(x,y,z);

                i++;

            }
        }


        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;


        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = triangles[tris + 2];
                triangles[tris + 4] = triangles[tris + 1];
                triangles[tris + 5] = triangles[tris + 1] + 1;

                tris += 6;
                vert++;

              

            }
            vert++;
          
        }
      

       





    }

    private void UpdateMesh()
    {

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

   
    }

    private void OnDrawGizmos()
    {
     

    }
}
