using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    const float scale = 1; 

    public  LODinfo[] detailLevels;
    public static float maxViewDst = 800;
    const float viewerMoveThreasholdForCunkupdate = 25f;
    const float sqrviewerMoveThreasholdForCunkupdate = viewerMoveThreasholdForCunkupdate * viewerMoveThreasholdForCunkupdate;


    public Transform viewer;
    public Material mapMaterial;
    Vector2 viewPositionOld;


    [HideInInspector]
    public static Vector2 viewerPosition;
    int chunkSize;
    int chunkVisibleInViewDst;
    static MapGenerator mapGenerator;
    static StructureGenerator structureGenerator;


    Dictionary<Vector2, TerrainChuck> terrainChunkDictionnary = new Dictionary<Vector2, TerrainChuck>();
    static List<TerrainChuck> terrainChucksVisible = new List<TerrainChuck>();

    private void Start()
    {

        

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreadshold;

        mapGenerator = FindObjectOfType<MapGenerator>();
        structureGenerator = FindObjectOfType<StructureGenerator>();

        chunkSize = MapGenerator.mapChunkSize - 1;
  
        viewPositionOld = viewerPosition;
        chunkVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

        UpdateVisibleChucks();

    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;

        if((viewPositionOld - viewerPosition).sqrMagnitude > sqrviewerMoveThreasholdForCunkupdate)
        {
            UpdateVisibleChucks();
            viewPositionOld  = viewerPosition;
        }

   

    }

    void UpdateVisibleChucks()
    {
        for (int i = 0; i < terrainChucksVisible.Count; i++)
        {
            terrainChucksVisible[i].SetVisible(false);

        }
     

        terrainChucksVisible.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunkVisibleInViewDst; yOffset < chunkVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunkVisibleInViewDst; xOffset < chunkVisibleInViewDst; xOffset++)
            {
                Vector2 viewedCunkCoord = new Vector2(currentChunkCoordX + xOffset,currentChunkCoordY + yOffset);

                if (terrainChunkDictionnary.ContainsKey(viewedCunkCoord))
                {
                    TerrainChuck tr = terrainChunkDictionnary[viewedCunkCoord];
                    tr.ChunkUpdate();

                }
                else
                {
                    terrainChunkDictionnary.Add(viewedCunkCoord,new TerrainChuck(viewedCunkCoord, chunkSize,detailLevels,this.transform, mapMaterial) );
                }

            }

        }
    }

    public class TerrainChuck
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        Vector3 postionV3;

        Mapdata mapData;
        bool mapDataRecieved;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LODinfo[] detailLevels;
        LODmesh[] LODmeshes;
        LODmesh collisionLODMesh;
        GameObject childrenObject;
        int previouslod = -1;


        public TerrainChuck(Vector2 coord, int size, LODinfo[] detailLevels,Transform transform, Material material)
        {
            this.detailLevels = detailLevels ;
            position = coord * size;
            bounds =new Bounds(position, Vector2.one * size);
            postionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            childrenObject = new GameObject("Structures");
            childrenObject.transform.SetParent(meshObject.transform);
         

            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshObject.layer = LayerMask.NameToLayer("Ground");

            meshRenderer.material = material;
            meshObject.transform.localScale = Vector3.one * scale; 


            meshObject.transform.position = postionV3 * scale;
            meshObject.transform.SetParent(transform);

            EndlessTerrain.mapGenerator.RequestMapData(position, OnMapDataRecieved);

            LODmeshes = new LODmesh[detailLevels.Length];

            for (int i = 0; i < LODmeshes.Length; i++)
            {
                LODmeshes[i] = new LODmesh(detailLevels[i].LOD, ChunkUpdate,this.position);
                if (detailLevels[i].usForCollider)
                {
                    collisionLODMesh = LODmeshes[i];
                }

            }

            SetVisible(false);
        }

        void OnMapDataRecieved(Mapdata mapData)
        {
            // mapGenerator.RequestMeshData(mapData,1, OnMeshDataRecieved);
            this.mapData = mapData;
            mapDataRecieved = true;

          

            Texture2D texture = TextureGenerator.textureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize,MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;

            ChunkUpdate();

            StructureGenerator.CreateStructure(this.mapData.heightMap, this.postionV3, childrenObject, scale, EndlessTerrain.structureGenerator.BasicMesh, mapGenerator.meshHeightCurve, mapGenerator.meshHeightMutliplier);
            childrenObject.transform.localPosition = new Vector3(0,0,0);
            childrenObject.transform.Rotate(Vector3.up, 90);
        }

        public void ChunkUpdate()
        {


            if (mapDataRecieved)
            {


                float viewerDst = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

                bool visible = viewerDst <= EndlessTerrain.maxViewDst;

                if (visible)
                {
                    int lodIndex = 0;
                    bool findCorrectLod = false;
                    int i = 0;


                    while (!findCorrectLod && i < detailLevels.Length - 1)
                    {

                 

                        if (viewerDst <= detailLevels[i].visibleDstThreadshold)
                        {
                            findCorrectLod = true;
                        }
                        else
                        {
                            lodIndex = i + 1;
                            i++;
                        }

                    }

                    if (lodIndex != previouslod)
                    {
                        LODmesh lodMesh = LODmeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previouslod = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                            meshCollider.sharedMesh = collisionLODMesh.mesh;

                            if (lodIndex == 0)
                            {
                                
                                meshCollider.sharedMesh = lodMesh.mesh;
                                
                               
                            }

                        } else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }

                  

                    terrainChucksVisible.Add(this);



                }

               


                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool getVisible()
        {
            return this.meshObject.activeSelf;
        }


    }

    // responsible for feshing meshs
    class LODmesh
    {

        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
    
        int lod;
        System.Action UpdateCallBack;

        float[,] f;

        public LODmesh(int lod,System.Action updateCallBack,Vector2 position)
        {
            this.lod = lod;
            this.UpdateCallBack = updateCallBack;

             f = new float[239,239];

            for (int y = 0; y < 239; y++)
            {
                for (int x = 0; x < 239; x++)
                {
                    if (position == new Vector2(0,0))
                    {
                        f[x, y] = mapGenerator.chunkZero.GetPixel(x, y).r;
                    }
                    else
                    {
                        f[x, y] = 1;
                    }
                }
            }

        }

        void OnMapDataRecieved(MeshData meshData)
        {

            mesh = meshData.CreateMesh();
            mesh.RecalculateNormals();
            hasMesh = true;
            UpdateCallBack();
        }

        public void RequestMesh(Mapdata mapData)
        {
            hasRequestedMesh = true;

            EndlessTerrain.mapGenerator.RequestMeshData(mapData, lod, OnMapDataRecieved, this.f);
        }


    }

    [System.Serializable]
    public struct LODinfo
    {
        public int LOD;
        public float visibleDstThreadshold;

        public bool usForCollider;

    }

}


