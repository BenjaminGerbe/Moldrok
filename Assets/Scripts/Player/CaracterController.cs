using UnityEngine;

public class CaracterController : MonoBehaviour
{

    [Header("Conpenents")]
    public Transform target;
    public Transform cam;
    public Rigidbody RB;
    public CapsuleCollider CC;
    
    [Header("Values")]
    public float mouvementSpeed = 8f;
    public float maxSpeed = 10f;
    public bool AirControl = false;
    public float JumpForce = 30f;
    public float timerJump = 0.25f;
    public AnimationCurve CurveslopeMultiplayer;
    
    [Header("Ground Values")]
    public float radiusGround = 1f;
    public float offsetSphere = 0;
    public float shellOffset; 
    
 
    //direction camera
    private Vector3 DirectionF;
    private Vector3 DirectionR;
    
    //KeyBoard
    private float directionForward;
    private float directionRight;
    
    //Jump
    private bool Jump;
    private float counterjump;
    private bool isJumping = false;
    private bool startJumpTime = false;
    
    // Ground
    private bool Grounded;
    private bool oldGrounded;
    private Vector3 normalGroundContact;

    
    void Update()
    {
        // I got the values of forward and right victor for the camera
        var forward = cam.forward;
        DirectionF = new Vector3(forward.x, 0, forward.z);
        var right = cam.right;
        DirectionR = new Vector3(right.x, 0, right.z);
        
        directionForward =  Input.GetAxisRaw("Vertical");
        directionRight = Input.GetAxisRaw("Horizontal");
        
        // to swipe vector depending inputs
        DirectionF = DirectionF * directionForward;
        DirectionR = DirectionR * directionRight;
        
        // Jump input
        
        if (Input.GetButtonDown("Jump"))
        {
            Jump = true;
        }
        else
        {
            Jump = false;
        }

        // to manage jump time
        
        if (Jump && !Grounded)
        {
            startJumpTime = true;
        }

        if (startJumpTime)
        {
            counterjump += Time.deltaTime;
        }
        
        
    }

    
    private void FixedUpdate()
    {
        //CheckGround
        GroundCheck();
        
        
        // if the plauer move
        if ( (Mathf.Abs(directionForward) > 0 || Mathf.Abs(directionRight) > 0) && (AirControl || Grounded)  )
        {
            
            // chasles relation for the vector camera
            Vector3 dir = (DirectionR.normalized + DirectionF.normalized);
            
            // aplique theme to a plan paralle to the surface
            Vector3 followDirection = Vector3.ProjectOnPlane(dir, normalGroundContact).normalized;
            
      
            followDirection *= mouvementSpeed;
            
            RB.AddForce(followDirection*SlopeMulitplayer(), ForceMode.Impulse);
            RB.velocity = Vector3.ClampMagnitude(RB.velocity, maxSpeed);
        }
        
        // drag to no slide in slope
        
        if (Grounded)
        {  
            RB.drag = 5;
            
            // manage jump
            if (Jump && !isJumping || (counterjump <= timerJump && counterjump > 0))
            {
                counterjump = 0;
                RB.drag = 0;
                isJumping = true;
             
                RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);
                RB.AddForce(new Vector3(0f, JumpForce, 0f), ForceMode.Impulse);
            }
            
            startJumpTime = false;
        }
        else
        {
            RB.drag = 0;
            
            // to stay stickiy in ground cf RigibdyFristPersonnController
            if (oldGrounded && !isJumping)
            {
                StickToGroundHelper();
            }
            
        }
        
    }

    private void StickToGroundHelper()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(this.target.transform.position,Vector3.down,out hitInfo,Physics.AllLayers))
        {
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
            {
                RB.velocity = Vector3.ProjectOnPlane(RB.velocity, hitInfo.normal);
            }
        }
    }
    
    private float SlopeMulitplayer()
    {
        float angle = Vector3.Angle(normalGroundContact, Vector3.up);
        
        return CurveslopeMultiplayer.Evaluate(angle);
    }
    
    private void GroundCheck()
    {
        oldGrounded = Grounded;
        RaycastHit hitInfo;

        if (Physics.SphereCast(transform.position, radiusGround * (1.0f - shellOffset), Vector3.down, out hitInfo,
            ((CC.height / 2f) - CC.radius) +
            offsetSphere, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            Grounded = true;

            if (!oldGrounded && isJumping)
            {
                isJumping = false;
            }
          
            normalGroundContact = hitInfo.normal;
        }
        else
        {
            Grounded = false;
        }
        
    }
    
    
}
