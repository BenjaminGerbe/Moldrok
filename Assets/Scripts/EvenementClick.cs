using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EvenementClick : MonoBehaviour
{
    public UnityEvent UE;
    
    [HideInInspector]
    public bool Active;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Active)
        {
          
      
            UE.Invoke();
        }
    }
}
