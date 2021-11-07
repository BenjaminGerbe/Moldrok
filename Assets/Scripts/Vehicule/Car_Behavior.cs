using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Car_Door))]
[RequireComponent(typeof(Rigidbody))]
public class Car_Behavior : MonoBehaviour
{
    // attributes 
    public  float MouvementSpeed = 0;
    public  float RotationSpeed = 0;
    public float DistanceCheckGround = 0.1f;
    public float MaxVitesse;
    
    public LayerMask GroundLayer;
    
   
    private Rigidbody RB;
    private Car_Door CD;


    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        CD = GetComponent<Car_Door>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CD.GetEnter())
        {
            if (isGrounded())
            {
                Movement();
            }
            Rotation();
           
        }
        
        float angle = Vector3.Angle(this.transform.up, Vector3.up);

        float rotationZ = transform.localEulerAngles.z;
        float rotationX = transform.localEulerAngles.x;
        float rotationY= transform.localEulerAngles.y;


        if (rotationZ <= 330 && rotationZ > 180)
        {
            rotationZ += .15f * Time.fixedTime;
           
        }

        
        if (rotationZ >= 30 && rotationZ < 180)
        {
            rotationZ -= 0.15f * Time.fixedTime;
        }

        
        
        transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);

        
        
    }
    

    private void Movement()
    {
        Vector3 direction = new Vector3();
        Vector3 vec = new Vector3();
        Vector3 localVeloctiy;

        Ray r = new Ray(this.transform.position, -this.transform.up);

        RaycastHit hit;

        if (Physics.Raycast(r, out hit, 1000, GroundLayer))
        {
           vec = Vector3.Cross(hit.normal,this.transform.right).normalized;      
        }




        direction = this.transform.forward * -Input.GetAxisRaw("Vertical") * MouvementSpeed;

        localVeloctiy = transform.InverseTransformVector(RB.velocity);
        
        


  
        
        
        
        
        
        
        
        

        RB.velocity +=  direction;

     
        if (RB.velocity.x > MaxVitesse)
        {
            RB.velocity = new Vector3(MaxVitesse, RB.velocity.y, RB.velocity.z);
        }
        
        if (RB.velocity.x < -MaxVitesse)
        {
            RB.velocity = new Vector3(-MaxVitesse, RB.velocity.y, RB.velocity.z);
        }

        if (RB.velocity.z > MaxVitesse)
        {
            RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y, MaxVitesse);
        }
        
        if (RB.velocity.z < -MaxVitesse)
        {
            RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y, -MaxVitesse);
        }


    }

    private void Rotation()
    {
        this.transform.Rotate(Vector3.up,RotationSpeed * Input.GetAxisRaw("Horizontal"));
    }

    private bool isGrounded()
    {
      
        Ray r = new Ray(this.transform.position,-this.transform.up);

        RaycastHit h;
        
        Debug.DrawRay(this.transform.position,r.direction * DistanceCheckGround,Color.red);
        
        if(Physics.Raycast(r, out h, DistanceCheckGround, GroundLayer))
        {
            return true;
        }


        return false;
    }
    
    
    
}
