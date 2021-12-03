using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTower : MonoBehaviour
{

    public Transform TEL;
    private bool detect = false;
    private GameObject GO;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            GO = other.gameObject;
            detect = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detect = false;
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        
        if (Input.GetKey(KeyCode.E) && detect && GO != null )
        {
           
            GO.transform.position = TEL.position;
        }
    }
}
