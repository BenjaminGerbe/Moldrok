using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    /// <summary>
    /// This class allow to draw the noise in plane
    /// </summary>

    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRender;

    public void DrawTexture(Texture2D texture)
    {
        

        // we can't use .mat beceause they only call in runtime
        textureRender.sharedMaterial.mainTexture = texture;


        // set the size of the plane same size of the map
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);


    }

    public void DrawMesh(MeshData meshData,Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRender.sharedMaterial.mainTexture = texture;

    }


}
