using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;



public class StructureGenerator : MonoBehaviour
{
    public int seed;
    
    public List<ObjectGenerated> BasicMesh;
    public float AvarangeSlope;
    private MapGenerator mp;

   

    [System.Serializable]
    public struct ObjectGenerated
    {
        
        public GameObject go;
        public Vector3 rotation;
        

    }

    
    private static float findAvarageSlop(float[,] heightMap,Vector2Int pos)
    {
        int startX = pos.x - 5;
        int startY = pos.x - 5;

       
        if (startX < 0 )
        {
            startX = 0;
        }

        if (startY < 0)
        {
            startY = 0;
        }



        int endY = startX + 10;
        int endX = startY + 10;


        if (endY > heightMap.GetLength(0)-10)
        {
            endY = heightMap.GetLength(0)-10;
            startY = endY - 10;
        }

        if (endX > heightMap.GetLength(0)-10)
        {
            endX = heightMap.GetLength(0)-10;
            startX = endX - 10;
        }

        int nb = 0;
        float avg = 0;




        Vector3 pos1 = new Vector3(startX,heightMap[startX,startY] * 10,startY);
        Vector3 pos2 = new Vector3(endX,heightMap[endX,endY] * 10,endY);

        Vector3 dir = pos1 - pos2;

        dir =  dir.normalized;



        //for (int y = startY ;y  < endY; y++)
        //{
        //    for (int x = startX; x < endX; x++)
        //    {
        //        nb++;


        //        Debug.Log(x + " == " + y);

        //        avg += heightMap[y, x];


        //    }

        //}

        //Debug.DrawRay(new Vector3(0,0,0),dir * 100,Color.red,Mathf.Infinity);


        return Mathf.Abs( dir.y);

       // return (Mathf.Abs( ((avg/nb) - heightMap[pos.x,pos.y]))) * 100;
    }


    public static void CreateStructure(float[,] heightMap,Vector3 coord,GameObject go,float scale, List<ObjectGenerated> obj, AnimationCurve AC,float HieghtMultiplication)
    {
      

        if (true)
        {
            System.Random rng = new System.Random();
           


            int x = heightMap.GetLength(0) / 2;
            int y = heightMap.GetLength(0) / 2;
            float v = 0;
            int n = 0;

            do
            {
                x = rng.Next(0, heightMap.GetLength(0));
                y = rng.Next(0, heightMap.GetLength(0));
                n++;

                v = StructureGenerator.findAvarageSlop(heightMap,new Vector2Int(x,y));

                System.Random prng = new System.Random(0);
                ObjectGenerated OG = obj[rng.Next(0, obj.Count - 1)];
                GameObject g = GameObject.Instantiate(OG.go, go.transform.position,Quaternion.identity);

                float hieght = HieghtMultiplication * AC.Evaluate(heightMap[y, x]);

                Vector3 position = new Vector3(x - (heightMap.GetLength(0) * scale) / 2, hieght, y - (heightMap.GetLength(0) * scale) / 2);
                // g.transform.GetChild(5).GetComponent<TextMeshPro>().text = v.ToString();

                g.transform.SetParent(go.transform);
                g.transform.localPosition = position;
                g.transform.Rotate(Vector3.right, OG.rotation.x);
                g.transform.Rotate(Vector3.up ,Random.Range(-180,180));

            } while ( n < 25);

         

            if (true)
            {
               
            }
                    
            //    }
            //}

          
        }
    } 

}
