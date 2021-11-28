using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetectIronScript : MonoBehaviour
{
    private bool detectItems = false;
    private GameObject GO;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Iron"))
        {
            detectItems = true;
            GO = other.gameObject;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Iron"))
        {
            detectItems = false;
            GO = null;
           
            
            
            
        }
    }

    public GameObject getDetected()
    {
        return GO;
    }
    
}
