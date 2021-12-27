using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortionManagerScript : MonoBehaviour
{
    public LifeManager lf;
    public PickMetal PM;
    
    public void buyPortion()
    {
        if (PM.Decriment(1))
        {
            lf.heal(1);
        }
   
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
