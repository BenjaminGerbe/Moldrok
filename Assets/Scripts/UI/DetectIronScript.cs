using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetectIronScript : MonoBehaviour
{
    private bool detectItems = false;
    private GameObject GO;

    private InfoIron II;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Iron"))
        {
            if (other.GetComponent<InfoIron>() == null)
            {
                return;
            }

            II = other.GetComponent<InfoIron>();
            
            
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
            II = null;
        }
    }

    public InfoIron getII()
    {
        return II;
    }
    public GameObject getDetected()
    {
        return GO;
    }
    
}
