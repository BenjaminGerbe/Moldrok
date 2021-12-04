using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollower : MonoBehaviour
{
    public Transform Player;
    private Vector3 forwardDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        forwardDirection = this.transform.forward;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float angle = Vector3.Angle(forwardDirection,Player.transform.position - this.transform.position);
        
        Debug.Log(angle);

        if (angle < 95)
        {
            this.transform.LookAt(Player.transform.position);
        }
        
      
    }
}
