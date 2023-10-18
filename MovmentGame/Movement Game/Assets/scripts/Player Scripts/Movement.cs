using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    bool sprinting = false;
    bool sprintKeyDown = false;
    int jumps;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);

        if (isGrounded())
        {
            jumps = maxExtraJumps;
        }

       
        if (Input.GetButtonDown("Jump") &&  jumps > 0)
        {
            Jump();
            jumps--;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && !touchingWall())
        {
            sprintKeyDown = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && !touchingWall())
        { 
            sprintKeyDown = false;
        }
        if (sprintKeyDown && !sprinting && !touchingWall()) 
        {
            movementSpeed *= 2f;
            sprinting = true;
        }

        if (sprinting && !sprintKeyDown && !touchingWall())
        {
            movementSpeed *= 0.5f;
            sprinting = false;
        }

        if (touchingWall())
        {
            if(sprinting)
            {
                movementSpeed *= 0.5f;
                sprinting = false;
            }
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            jumps = maxExtraJumps;
            movementSpeed = wallSpeed;
        }
        if (!touchingWall() && !sprinting)
        {
            movementSpeed = baseMovementSpeed;
        }
        //Debug.Log(sprinting);

        MPRC();
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

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
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

        
    }
    
}
