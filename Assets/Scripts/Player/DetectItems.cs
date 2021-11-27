using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class DetectItems : MonoBehaviour
{

    public GameObject UI;
    private SphereCollider SC;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        SC = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UI.activeSelf )
        {
            SC.enabled = true;
        }
        else
        {
            SC.enabled = false;
        }
        
    }
}

struct ItemInformation
{
    
    
    
}

