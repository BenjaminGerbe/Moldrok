using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public Transform Step;
    public SpriteRenderer SPRT;
    public Rigidbody RB;
    public float Timer = 10;
    public SpriteRenderer fade;
    public float speed;
    private float count;
    // Start is called before the first frame update
    void Start()
    {
        count = Timer;
    }

    // Update is called once per frame
    void Update()
    {
      //  fade.color = new Color(0, 0, 0, (fade.color.a + speed * Time.deltaTime)%0.15f);
        SPRT.color = new Color(1,1,1,0);
        if (this.transform.InverseTransformDirection(RB.velocity).z > 0.1 || this.transform.InverseTransformDirection(RB.velocity).z < -0.1)
        {
            count -= Time.deltaTime;

            if (count<0)
            {
                
                SPRT.color = new Color(1,1,1,1);
                count = Timer;
               // Step.localScale = new Vector3( -Step.localScale.x,1,1);
            }
            
        }
        else
        {
            SPRT.color = new Color(1,1,1,0);
            count = Timer;
        }
        
        
    }
}
