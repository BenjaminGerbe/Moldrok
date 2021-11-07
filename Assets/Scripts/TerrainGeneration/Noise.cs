using UnityEngine;

public static class Noise 
{
    // this class generate and manage Noise

    public enum NormalizeMode { local,Global};



    // function that return a grid of 0 and 1 according to perlrine noise
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight,float scale,int seed, int octaves , float persistance, float lacunarity,Vector2 offset,NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // pnrg : pseudo random number generator
        System.Random prng = new System.Random(seed);
        Vector2[] octavesOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000,100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;

        }


        if(scale <= 0)
        {
            scale = 0.0001f;
        }


        // in the end of this function we return a grid of 0 and 1
        // but the value are to height or to lower of 1 and 0
        // so we normalized them and for that we have to find the max value and the min value of the grid before it be normalized

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

       
        float halfWidth = mapWidth / 2;
        float halfHeight = mapHeight / 2;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {

                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {

                    // to have non interger value  
                    float sampleX = ((x - halfWidth + octavesOffsets[i].x) / scale * frequency) ;
                    float sampleY = ((y- halfHeight + octavesOffsets[i].y) / scale * frequency)  ;

                    //  to have more intersting value it's better if perlin value can have negativ value
                   // so noiseHeight can degrease ( * 2 -1 )
                    float perlineValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlineValue * amplitude;


                    amplitude *= persistance;
                    frequency *= lacunarity;

                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
              
            }
        }



        // we loop again of each value of noiseMap to limit them
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                // this Inverse Lerp method is here to limit the value of noiseMap
                // if the value of noiseMap is greater than maxNoiseHeight the value will be 1
                // and if the value of noiseMap is lower than de minNoiseHeight the value will be 0
                // beceause minNoiseHeight and maxNoiseHeight depend of each chunk and create
                // some decalages we need to change this approche
                if (normalizeMode == NormalizeMode.local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalizeHeight = (noiseMap[x, y] + 1) / (2f) * maxPossibleHeight;
                    noiseMap[x, y] = Mathf.Clamp( normalizeHeight,0,int.MaxValue);
                }

               

            }
        }

        return noiseMap;

    }
}
