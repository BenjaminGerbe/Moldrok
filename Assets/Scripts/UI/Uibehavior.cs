using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Uibehavior : MonoBehaviour
{
    public Vector2Int Brush = new Vector2Int(1,1);
    public int Width = 4;
    public int Height = 9;
    private Slot[,] Slots;
    private List<Slot> CurrentSlots;
    private Vector2Int currentIdx;
    
    // Start is called before the first frame update
    void Start()
    {
        Slots = new Slot[Width,Height];
        CurrentSlots = new List<Slot>();
        int x = 0;
        int y = 0;
        foreach (Transform tr in transform)
        {
            if (tr.gameObject.GetComponent<Slot>())
            {
                Slots[x,y] = ( tr.gameObject.GetComponent<Slot>());
            }

            x++;
            if (x >= Width)
            {
             
                y++;
            }


            x = x % Width;
            y = y % Height;
        }
        
        
    }

    private void resetAll()
    {
        foreach (Slot slt in CurrentSlots)
        {
            if (!slt.GetHovered())
            {
                slt.Deselected();
            }
          
        }

        CurrentSlots = new List<Slot>();
    }
    
    // Update is called once per frame
    void Update()
    {
        Vector2 idx = new Vector2() ;
        int mirrorIdx = 0;
        
       
        
        for (int x = 0; x<  Width; x++)
        {
            for (int y = 0; y <  Height; y++)
            {
                if (CurrentSlots.Count > 0)
                {
                    if (!CurrentSlots[0].GetHovered())
                    {
                        currentIdx = new Vector2Int(x,y);
                        resetAll();
                    }
                }
             
                if (Slots[x,y].GetHovered() && CurrentSlots.Count < Brush.x * Brush.y)
                {
                    Debug.Log("s");
                    if (CurrentSlots.Count <= 0)
                    {
                        currentIdx = new Vector2Int(x, y);
                        CurrentSlots.Add(Slots[x,y]);
                        
                    }

                    if (Brush.x > 0 && Brush.y > 0)
                    {
                        for (int j = currentIdx.y - (Brush.y); j <= currentIdx.y + Brush.y; j++)
                        {
                            for (int i = currentIdx.x - (Brush.x ); i <= currentIdx.x + Brush.x; i++)
                            {
                                int v = i;
                                int u = j;
                            
                                v = Mathf.Clamp(i, 0, Width-1);
                                u = Mathf.Clamp(j, 0, Height-1);
                         
                            
                                Slots[v,u].Selected();
                                CurrentSlots.Add( Slots[v,u]);
                            }
                        }
                    }
                    
                   

                  
                    
                }
                
            }
        }


    }
}
