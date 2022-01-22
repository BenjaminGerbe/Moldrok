using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Unity.VisualScripting;
using Random = System.Random;


[RequireComponent(typeof(MapDisplay))]
public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// This class allow to manage the map
    /// </summary>

    // public attributes
    public enum DrawMode { NoiseMap,ColorMap,Mesh};
    public DrawMode drawMode;


    public Noise.NormalizeMode normalizeMode;

 

    public const int mapChunkSize = 239;
    public Texture2D chunkZero;
    [Range(0,6)]
    public int EditorPreviewLOD; // level of details
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public static int GlobalSeed;
    
    public Vector2 Offset;

    public float meshHeightMutliplier;

    public AnimationCurve meshHeightCurve;
    public static bool isStartGenerate;
    public bool autoUpdate;
    
    Queue<MapThreadInfo<Mapdata>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<Mapdata>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    // for colors
    public TerrainType[] regions;
    private Vector3 grav;
    private void Start()
    {
        Random rdn = new Random();
        seed = rdn.Next(int.MinValue, int.MaxValue);
        GlobalSeed = seed;
         grav = Physics.gravity;
        Physics.gravity = Vector3.down *0;
    }

    public void DrawMapInEditer()
    {
        Mapdata mapdata = GenerateMapData(new Vector2(0,0));


        // draw noise Map
        MapDisplay display = GetComponent<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            // we pass the noise map
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapdata.heightMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            //we pass direcly the color map we create earlier
            display.DrawTexture(TextureGenerator.textureFromColorMap(mapdata.colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerainMesh(mapdata.heightMap, meshHeightMutliplier, meshHeightCurve, EditorPreviewLOD), TextureGenerator.textureFromColorMap(mapdata.colorMap, mapChunkSize, mapChunkSize));
        }
    }

    // thread for Map DATA
    public void RequestMapData(Vector2 center,Action<Mapdata> callback)
    {
        // This function is here to call all the function in the delegate in another thread

        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();

    }


    void MapDataThread(Vector2 center,Action<Mapdata> callback)
    {
        // this function call the heavy function,considering MapDataThrad is in another thread that will not 
        // lag in game
        Mapdata mapData = GenerateMapData(center);

        // we can't call the callback functio in the treath so we store back and map data in a que and we will execute it in update method
        // the lock is for avoid .Enqueue is call in difference places
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<Mapdata>(callback, mapData));
        }

    }

    // Thread for Mesh DATA

    // MapData = meshData
    public void RequestMeshData(Mapdata mapData,int lod,Action<MeshData> callback,Vector2 pos)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData,lod,callback,pos);
        };

        new Thread(threadStart).Start();
        if (!MapGenerator.isStartGenerate)
        {

            MapGenerator.isStartGenerate = true;
        }
        
      
    }

 

    void MeshDataThread(Mapdata mapData,int lod, Action<MeshData> callback,Vector2 pos)
    {

        if (pos.x == 0 && pos.y == 0)
        {

            for (int y = 0; y < 239; y++)
            {
                for (int x = 0; x < 239; x++)
                {
                    float x1 = Mathf.Abs(  x - (239 / 2));
                    x1 = Mathf.InverseLerp(0, 120,x1); 
                    float y1 = Mathf.Abs( y - (239 / 2));
                    y1 = Mathf.InverseLerp(0, 120,y1); 
                    
                    
                    float distance = Mathf.Sqrt((x1*x1) + (y1*y1))  ;

                    distance = Mathf.Clamp01(distance);

                    distance = Mathf.InverseLerp(0.25f,0.8f,distance);

                    
                    
                    mapData.heightMap[x, y] *=     distance ;

                }

            }
        }
        MeshData meshData = MeshGenerator.GenerateTerainMesh(mapData.heightMap, meshHeightMutliplier, meshHeightCurve, lod);

       

        lock (mapDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }

    }




    private void Update()
    {
        // we loocking for a function to call
        if(mapDataThreadInfoQueue.Count > 0)
        {
            // and if we find some function we call them
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<Mapdata> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.paramter);
            }
        }
        if (meshDataThreadInfoQueue.Count > 0)
        {
            // and if we find some function we call them
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.paramter);
            }
        }

 
       
        if (MapGenerator.isStartGenerate)
        {
            Physics.gravity = this.grav;
        }
       
        

    }

    Mapdata GenerateMapData(Vector2 center)
    {
        // create noise Map
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseScale,seed,octaves,persistance,lacunarity,Offset + center, normalizeMode);

        


        // Assign color to our noise Map
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0, j = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                // we get the current height of the noise Map and we compare to regions heights to find his color
                float currentHeight = noiseMap[x, y];
                bool trouver = false;
                int i = 0;

          
                while(i < regions.Length && !trouver)
                {

                    if (currentHeight < regions[i].height)
                    {
                        trouver = true;
                
                    }
                    else
                    {

                        colorMap[j] = regions[i].color;
                        i++;
                    }
                }
                j++;
            }
        }


        return new Mapdata(noiseMap,colorMap);
      
    }


    private void OnValidate()
    {



        if (octaves < 0)
        {
            octaves = 0;
        }

    }

    // T for generic

    struct MapThreadInfo<T>
    {
        public  Action<T> callback {  get; private set; }
        public T paramter {  get; private set; }

        public MapThreadInfo(Action<T> callback,T paramter)
        {
            this.callback = callback;
            this.paramter = paramter;
        }

    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public struct Mapdata
{
    public float[,] heightMap { get; private set; }
    public Color[] colorMap { get; private set; }

    public Mapdata(float[,] heightMap,Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }

}

