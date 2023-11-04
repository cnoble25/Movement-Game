using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float baseMovementSpeed = 500f;
    float movementSpeed = 500f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask wall;
    [SerializeField] int maxExtraJumps = 1;
    [SerializeField] float wallSpeed = 1500f;
    [SerializeField] float sprintSpeed = 1000f;
    Quaternion wallAngle;
    

    bool wallRunning = false;
    bool sprinting = false;
    bool sprintKeyDown = false;
    bool hasWallJumped = false;
    int jumps;
    Vector3 forward;
    Vector3 right;
    float xzAngle;
    Quaternion cameraAngle;
    Vector3 wallForward;
    bool hasAWall = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }
    // Update is called once per frame
    void Update()
    {
        forward = Camera.main.transform.forward;
        right = Camera.main.transform.right;
        // float angleThing = Mathf.Atan(right.x/forward.x)*180f/Mathf.PI
        if(!(forward.x < 0) && !(forward.z < 0)){
           xzAngle = Mathf.Atan(forward.z/forward.x);
        }else if(!(forward.x < 0) && !(forward.z > 0)){
             xzAngle = Mathf.Atan(forward.z/forward.x) + Mathf.PI*2f;
        }
        else{
            xzAngle = Mathf.Atan(forward.z/forward.x) + Mathf.PI;
        }

        // Debug.Log(xzAngle*180/Mathf.PI);

        // Debug.Log(wallForward);
        // Debug.Log(forward.x);
        
        // Debug.Log(Mathf.Atan(forward.x/forward.z)*180f/Mathf.PI);
        //Debug.Log(cameraAngle.y);
        // Debug.Log((Mathf.Asin(forward.x)*2f + Mathf.PI/2f)*180f/Mathf.PI);
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);

        if (isGrounded())
        {
            jumps = maxExtraJumps;
            hasWallJumped = false;
        }
        //---------------------------------------------------------------------------
        if (Input.GetButtonDown("Jump") &&  jumps > 0)
        {
            Jump();
            jumps--;
        }
        //---------------------------------------------------------------------------
        if(touchingWall() && Input.GetButtonDown("Jump"))
        {

            
        }
        //---------------------------------------------------------------------------
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprintKeyDown = true;
        }
        //---------------------------------------------------------------------------
        if (Input.GetKeyUp(KeyCode.LeftShift))
        { 
            sprintKeyDown = false;
        }
        //---------------------------------------------------------------------------
        if (sprintKeyDown && !sprinting && !touchingWall()) 
        {
            movementSpeed = sprintSpeed;
            sprinting = true;
        }
        //---------------------------------------------------------------------------
        if (sprinting && !sprintKeyDown && !touchingWall())
        {
            movementSpeed = baseMovementSpeed;
            sprinting = false;
        }
        //---------------------------------------------------------------------------
        if (sprinting && sprintKeyDown && !touchingWall()){
            movementSpeed = sprintSpeed;
            sprinting = true;
        }
        //---------------------------------------------------------------------------
        if (!touchingWall() && !sprinting)
        {
            movementSpeed = baseMovementSpeed;
        }
        //---------------------------------------------------------------------------
         MPRC();

        if (touchingWall())
        {
            
            float wallAngleX = Mathf.Cos(Mathf.Asin(wallAngle.y) * 2f);
            float wallAngleZ = Mathf.Sin(Mathf.Asin(wallAngle.y) * 2f);
            float angleShit = Mathf.Asin(wallAngle.y) * 2f;
            movementSpeed = wallSpeed;
            if(Input.GetKey("space") && !hasWallJumped){
                rb.velocity = new Vector3(rb.velocity.z, jumpForce, rb.velocity.x);
                hasWallJumped = true;
                jumps = maxExtraJumps;
            }
            else if(!hasWallJumped && hasAWall){
                if ((Input.GetKey(KeyCode.W) && Mathf.Asin(forward.z) > angleShit) || (Input.GetKey(KeyCode.S) && Mathf.Asin(forward.z) < angleShit))
                {
                    rb.velocity = new Vector3(wallAngleZ * movementSpeed / 120f, 0f, wallAngleX * movementSpeed / 120f);
                }
                else
                if ((Input.GetKey(KeyCode.S) && Mathf.Asin(forward.z) >= angleShit) || (Input.GetKey(KeyCode.W) && Mathf.Asin(forward.z) <= angleShit) && !isGrounded())
                {
                    rb.velocity = new Vector3(-1f*wallAngleZ * movementSpeed / 120f, 0f, -1f*wallAngleX * movementSpeed / 120f);
                }
                else if(!isGrounded()) 
                {

                    rb.velocity = new Vector3(0f, 0f, 0f);
                }
            }

            
            Debug.Log(Mathf.Abs(angleShit)*90/Mathf.PI);

        }
        else
        {
            hasWallJumped = false;
        }
        //---------------------------------------------------------------------------



    }

    bool touchingWall()
    {
        Vector3 startPt = new Vector3(wallCheck.position.x, wallCheck.position.y + 0.5f, wallCheck.position.z);
        Vector3 endPt = new Vector3(wallCheck.position.x, wallCheck.position.y - 0.5f, wallCheck.position.z);

        return Physics.CheckCapsule(startPt, endPt, 0.5f, wall);
    }
    
    bool isGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.1f, ground);
    }

    void MPRC() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

       
        //Debug.Log(Mathf.Asin(forward.x)*180f/Math.PI);
        //Debug.Log(Mathf.Asin(forward.z) * 180f / Math.PI);
        

        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 FRVI = verticalInput * forward;
        Vector3 RRVI = horizontalInput * right;
        Vector3 CRM = (FRVI + RRVI)/120;
        rb.velocity = new Vector3(CRM.x * movementSpeed, rb.velocity.y, CRM.z * movementSpeed);

    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy Head")) 
        {
            Destroy(collision.transform.parent.gameObject);
            Jump();
        }

        if(collision.gameObject.CompareTag("Wall")){
            wallAngle = collision.transform.rotation;
            hasAWall = true;
            wallForward = collision.transform.forward;
        }

        
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            hasAWall = false;
        }
    }
}
