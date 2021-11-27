using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchInventory : MonoBehaviour
{
    public GameObject GO;
    public MonoBehaviour View;

    public DetectIronScript DTS;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = GO.activeSelf;
    }

    // Update is called once per frame
    void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.E))
        {
            GO.SetActive(!GO.activeSelf);
            View.enabled = !GO.activeSelf;
            Cursor.visible = GO.activeSelf;
            if(GO.activeSelf)  Cursor.lockState = CursorLockMode.None;
            else  Cursor.lockState = CursorLockMode.Locked;


            if (DTS.getDetected() != null)
            {
                Debug.Log("Items trouvé");
            }
            
            
            
            
            
            
        }
    }
}
