using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{

    public LayerMask ground;
    public LayerMask platform;
    public Transform groundCheck;
    public Transform platformCheck;

    bool startCrouch = false;
    bool isCrouching = false;
    bool facingRight = true;
    bool jump = false;
    bool jumpCancel = false;
    bool doubleTap = false;
    bool isGrounded = false;
    bool isPlatformed = false;

    float tapTime;
    float tapDiff = 0.2f;       //tweakable
    float walkSpeed = 10f;      //tweakable
    float runSpeed = 20f;       //tweakable
    float jumpSpeed = 55f;     //tweakable
    float fixJumpSpeed = 55f; //tweakable
    float groundRadius = 0.75f; //tweakable
    float platformRadius = 0.75f; //tweatkable
    float crouchTime;



    Rigidbody2D rb;

    void Flip() //function to flip character sprite when moving in different directions
    {
        //IMPORTANT!!!! Character MUST be facing right at the start of the scene!
        facingRight = !facingRight;  //character is no longer facing right
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale; //reverse axis
    }

    void Walk()
    {
        float move = Input.GetAxisRaw("Horizontal"); //when pressing the horizontal movement button, assign the number '1.0' or '-1.0' to move
        if (move > 0 && !facingRight) //if move is positive and not facing right, flip the character
            Flip();
        if (move < 0 && facingRight) //if move is negative and facing right, flip the character
            Flip();
        if (Input.GetAxisRaw("Vertical") < 0f && (isGrounded || isPlatformed)) //pressing downkey and on the ground or on a platform, stop all movement
            move = 0;
        rb.velocity = new Vector2(move * walkSpeed, rb.velocity.y); //vvelocity is equal to the walkspeed multiplied by 'move'
    }

    void Run()
    {
        float move = Input.GetAxisRaw("Horizontal"); //see walk function
        if (move > 0 && !facingRight) //see walk function...switch from running to walking
        {
            Flip();
            doubleTap = false;
            Walk();
        }
        if (move < 0 && facingRight) //see walk function...switch from running to walking
        {
            Flip();
            doubleTap = false;
            Walk();
        }
        if (Input.GetAxisRaw("Vertical") < 0f && (isGrounded || isPlatformed)) //see walk function....switch from running to walking
        {
            move = 0;
            doubleTap = false;
            Walk();
        }
        rb.velocity = new Vector2(move * runSpeed, rb.velocity.y); //see walk function
    }

    void isJumping(bool jumping)
    {
        if (rb.velocity.y == 0 && (isCrouching || startCrouch)) //if upward velocity is zero and crouching or in the process of crouching, don't jump
        {
            jumpSpeed = 0;
            return;
        }
        if (jumping) //add a upward velocity
        {
            jumpSpeed = fixJumpSpeed;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            jump = false;
        }

        if (!jumping) //change upward velocity if no longer jumping
        {
            if (rb.velocity.y > 0)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 3f); // Change upward velocity by a factor of 1/x, x is tweakable
            jumpCancel = false;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, ground); //check if on the ground
        isPlatformed = Physics2D.OverlapCircle(platformCheck.position, platformRadius, platform); //check if on a platform
        gameObject.layer = 8; //make sure the character stays on layer number 8

        if (Input.GetButtonDown("Jump") && isGrounded)  // Player starts pressing the jump button
            jump = true;
        else if (Input.GetButtonDown("Jump") && isPlatformed && !isCrouching)
            jump = true;
        if (Input.GetButtonUp("Jump") && !isGrounded && !isPlatformed)   // Player stops pressing the button
            jumpCancel = true;

        if (Input.GetButtonDown("Horizontal") && (isGrounded || isPlatformed))
        {
            if (Time.time - tapTime > 0f && Time.time - tapTime < tapDiff) //if the difference between the current time and the time the player first tapped the button is smaller than a certain number, player doublee tapped the button
                doubleTap = true;
            else if (Time.time - tapTime == 0f || Time.time - tapTime > tapDiff) //else player does not double tap button
                doubleTap = false;
            tapTime = Time.time; //make the time that the player presses the horizontal movement button the current time
        }

        if (Input.GetAxisRaw("Vertical") == -1f && !startCrouch && (isGrounded || isPlatformed)) //if the player is pressing the downward button, has not initiated the crouch animation yet AND is on the ground or a platform, initiate crouching
        {
            startCrouch = true;
            crouchTime = Time.realtimeSinceStartup; //make the time that the player pressed the crouch button the current time
            //put crouch animation here
        }
        if (startCrouch) //if the player intiated the crouch animation
        {
            if (Time.realtimeSinceStartup - crouchTime >= 0.25f) //after a quarter of a second, the crouch animation should be completed and the player should be crouching
            {
                isCrouching = true;
            }
        }
        if (isCrouching)
        {
            startCrouch = false;
        }
        if (Input.GetAxisRaw("Vertical") != -1f) //if not pressing the downward button, the character should no longer be crouching
            isCrouching = false;
    }

    void FixedUpdate()
    {
        if (jump)
            isJumping(true);
        if (jumpCancel)
            isJumping(false);

        if (doubleTap) // if player double tapped, run
            Run();
        if (!doubleTap) // if not, walk
            Walk();

        //the following code is meant to be the platform dropping mechanic....currently incomplete
        //the concept is that when the player is crouching on a platform and taps the downward button, the character should ignore all contact between itself and the platform
        if (isCrouching && Input.GetButton("Jump") && isPlatformed && rb.velocity.y == 0f) //under construction
            Physics2D.IgnoreLayerCollision(8, 9);
        if (!isCrouching || !Input.GetButton("Jump")) //under construction
            Physics2D.IgnoreLayerCollision(8, 9, false);
        if (rb.velocity.y < -15f && isPlatformed) //under construction
            Physics2D.IgnoreLayerCollision(8, 9, false);
    }
}