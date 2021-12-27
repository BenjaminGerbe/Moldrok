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

    public Camera CamStep;
    public Transform viewer;
    public Material mapMaterial;
    Vector2 viewPositionOld;
    public int colliderLodIndex;

    [HideInInspector]
    public static Vector2 viewerPosition;
    int chunkSize;
    int chunkVisibleInViewDst;
    static MapGenerator mapGenerator;
    static StructureGenerator structureGenerator;
    private static float colliderGenerationThreshold = 5;
    private static int lastId;
    Dictionary<Vector2, TerrainChuck> terrainChunkDictionnary = new Dictionary<Vector2, TerrainChuck>();
    static List<TerrainChuck> terrainChucksVisible = new List<TerrainChuck>();

    private void Start()
    {
        chunkSize = MapGenerator.mapChunkSize - 1;

        EndlessTerrain.lastId = 0;
        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreadshold;

        mapGenerator = FindObjectOfType<MapGenerator>();
        structureGenerator = FindObjectOfType<StructureGenerator>();

     
  
        viewPositionOld = viewerPosition;
        chunkVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

        UpdateVisibleChucks();
    


    }

    private void Update()
    {
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        CamStep.transform.position =
        new Vector3(currentChunkCoordX * chunkSize, -50, currentChunkCoordY * chunkSize  );
        

        
        
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;

        if((viewPositionOld - viewerPosition).sqrMagnitude > sqrviewerMoveThreasholdForCunkupdate)
        {
            UpdateVisibleChucks();
            viewPositionOld  = viewerPosition;
        }

        if (viewerPosition != viewPositionOld)
        {
            foreach (TerrainChuck chunk in terrainChucksVisible)
            {
                chunk.UpdateCollisionMesh();
            }
        }
   

    }

    void UpdateVisibleChucks()
    {
        for (int i = 0; i < terrainChucksVisible.Count; i++)
        {
            terrainChucksVisible[i].SetVisible(false);

      
            if (viewerPosition == terrainChucksVisible[i].getpos())
            {
                CamStep.transform.position = terrainChucksVisible[i].getpos3();
            }
            

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
                    terrainChunkDictionnary.Add(viewedCunkCoord,new TerrainChuck(viewedCunkCoord, chunkSize,detailLevels,colliderLodIndex,this.transform, mapMaterial) );
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
        private int colliderLODIndex;
        GameObject childrenObject;
        int previouslod = -1;
        int mapChunkSize =  MapGenerator.mapChunkSize - 1;
        bool hasSetCollider;
        int id;
        
        public Vector3 getpos3()
        {
            return this.postionV3;
        }
        
        public Vector2 getpos()
        {
            return this.position;
        }
        

        public TerrainChuck(Vector2 coord, int size, LODinfo[] detailLevels,int colliderLODIndex,Transform transform, Material material)
        {
            this.detailLevels = detailLevels ;
            this.colliderLODIndex = colliderLODIndex;
            position = coord * size;
            bounds =new Bounds(position, Vector2.one * size);
            postionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            childrenObject = new GameObject("Structures");
            childrenObject.transform.SetParent(meshObject.transform);
            EndlessTerrain.lastId++;
            id = EndlessTerrain.lastId;

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
                LODmeshes[i].UpdateCallBack += ChunkUpdate;

                if (i == colliderLODIndex)
                {
                    LODmeshes[i].UpdateCallBack += UpdateCollisionMesh;
             
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

            StructureGenerator.CreateStructure(this.mapData.heightMap, this.position, childrenObject, scale, EndlessTerrain.structureGenerator.BasicMesh, 
                mapGenerator.meshHeightCurve, mapGenerator.meshHeightMutliplier,this.id);
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
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                        
                        
                    }

                 
                    
                    terrainChucksVisible.Add(this);
                    
                    
/*
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
                    */

                  

               



                }

               


                SetVisible(visible);
            }
        }

        public void UpdateCollisionMesh()
        {
            if (!hasSetCollider)
            {
                
          
                float sqrDstFormViewerToEdge = bounds.SqrDistance(viewerPosition);

                if (sqrDstFormViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
                {
                    if (!LODmeshes[colliderLODIndex].hasRequestedMesh)
                    {
                        LODmeshes [colliderLODIndex].RequestMesh(mapData);
                    }
                }
                
                if (sqrDstFormViewerToEdge < colliderGenerationThreshold * colliderGenerationThreshold)
                {
                    if (LODmeshes[colliderLODIndex].hasMesh)
                    {
                        meshCollider.sharedMesh = LODmeshes[colliderLODIndex].mesh;
                        hasSetCollider = true;
                    }
                   
                }
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
        public event System.Action UpdateCallBack;


        private Vector2 pos;

        public LODmesh(int lod,System.Action updateCallBack,Vector2 position)
        {
            this.lod = lod;


            this.pos = position;

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

            EndlessTerrain.mapGenerator.RequestMeshData(mapData, lod, OnMapDataRecieved, pos);
        }


    }

    [System.Serializable]
    public struct LODinfo
    {
        public int LOD;
        public float visibleDstThreadshold;

        public bool usForCollider;

        public float sqrVisibleDstThreshold
        {
            get
            {
                return visibleDstThreadshold * visibleDstThreadshold;
            }
            
        }
    }

}


