using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Car_Door : MonoBehaviour
{
    private bool detect = false;
    private bool isEnter = false;
    private bool isOut = true;
    private GameObject GO;
    private Collider playerCollider;
    
    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Player") && !isEnter)
        {
            detect = true;
            GO = other.gameObject;
            playerCollider = GO.GetComponent<CapsuleCollider>();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && detect)
        {
            detect = false;
        }
    }


    public bool GetEnter()
    {
        return isEnter;
    }
    
    private void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (detect && Input.GetKeyDown(KeyCode.E))
        {
            isOut = false;
            isEnter = !isEnter;
           
        }
     
        if (detect && isEnter)
        {
            GO.transform.SetParent(this.transform);
            GO.transform.localPosition = new Vector3();
            
            GO.GetComponent<CapsuleCollider>().enabled = false;
        }

        if (detect && !isEnter && !isOut)
        {
            GO.transform.localPosition = new Vector3(1, 0, 0);
            GO.transform.SetParent(null);
            isOut = true;
            GO.GetComponent<CapsuleCollider>().enabled = true;
            GO.transform.localScale = new Vector3(1, 1, 1);
            isEnter = false;
            
        }
        
        
        
    }
}
