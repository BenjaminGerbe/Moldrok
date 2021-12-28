using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchInventory : MonoBehaviour
{

    public struct slot
    {
        public string IDItem;
        public bool Active;
    }

    private  List<slot> listslots;

    public PickMetal PM;

    public List<Image> slotsImgs;
    
    public GameObject GO;
    public MonoBehaviour View;

    public DetectIronScript DTS;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = GO.activeSelf;

        listslots = new List<slot>();

        for (int i = 0; i < slotsImgs.Count; i++)
        {
            listslots.Add(new slot());
        }

    }

    public bool RemoveObject()
    {
        bool trouver = false;
        int i = listslots.Count -1;

        while (i >= 0  && !trouver)
        {
            var listslot = listslots[i];
            if (listslot.Active)
            {
                trouver = true;
                listslot.Active = false;
                listslot.IDItem = null;
                slotsImgs[i].sprite = null;
                listslots[i] = listslot;
                PM.Decriment(-1);
            }
            else
            {
                i--;
            }
        }

        return trouver;
    }
    
    public bool FillInventory()
    {
        bool trouver = false;
        int i = 0;

        while (i < listslots.Count && !trouver)
        {
            var listslot = listslots[i];
            if (!listslot.Active)
            {
                trouver = true;
                listslot.Active = true;
                listslot.IDItem = DTS.getII().Id;
                slotsImgs[i].sprite = DTS.getII().texture;
                listslots[i] = listslot;
            }
            else
            {
                i++;
            }
        }

        return trouver;
    }
    

    // Update is called once per frame
    void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.E))
        {
           
            

            if (DTS.getDetected() != null)
            {
                if (FillInventory())
                {
                    Destroy(DTS.getDetected());
                }
          
            }
            else
            {
                GO.SetActive(!GO.activeSelf);
                View.enabled = !GO.activeSelf;
                Cursor.visible = GO.activeSelf;
                if(GO.activeSelf)  Cursor.lockState = CursorLockMode.None;
                else  Cursor.lockState = CursorLockMode.Locked;
            }
        
           
            
          
           
            
        }
    }
}
