using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	
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

	void Flip() 
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	void Walk() 
	{	
		float move = Input.GetAxisRaw("Horizontal");
		if (move > 0 && !facingRight)
			Flip ();
		if (move < 0 && facingRight)
			Flip ();
		if (Input.GetAxisRaw ("Vertical") < 0f && (isGrounded || isPlatformed))
			move = 0;
		rb.velocity = new Vector2 (move * walkSpeed, rb.velocity.y);
	}
	
	void Run() 
	{	
		float move = Input.GetAxisRaw("Horizontal");
		if (move > 0 && !facingRight)
			Flip ();
		if (move < 0 && facingRight)
			Flip ();
		if (Input.GetAxisRaw ("Vertical") < 0f && (isGrounded || isPlatformed)) 
		{
			move = 0;
			doubleTap = false;
			Walk ();
		}
		if (Input.GetButtonDown ("Horizontal")) 
		{
			doubleTap = false;
			Walk();
		}
		if(!isGrounded && !isPlatformed && (rb.velocity.x == (move * runSpeed))) //if your velocity in the x direction in the air is equal to the running velocity
		{ 
			if(Input.GetButtonDown("Horizontal"))  //if you press the horizontal movement button
			{ 
				doubleTap = false; // cancel the double tap
				Walk (); //change to walking speed
			}
		}
		rb.velocity = new Vector2 (move * runSpeed, rb.velocity.y);
	}

	void isJumping(bool jumping)
	{
		if (jumping) 
		{
			if (isCrouching || startCrouch)
				jumpSpeed = 0f;
			else if(!isCrouching)
				jumpSpeed = fixJumpSpeed;
			rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
			jump = false;
		}

		if (!jumping) 
		{
			if (rb.velocity.y > 0) 
				rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 3f);      // Change upward velocity by a factor of 1/x, x is tweakable
			jumpCancel = false;
		}
	}
	
	void Start () 
	{	
		rb = GetComponent<Rigidbody2D> ();
	}
	
	void Update () 
	{
		isGrounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, ground); //check if on the ground
		isPlatformed = Physics2D.OverlapCircle (platformCheck.position, platformRadius, platform); //check if on a platform
		gameObject.layer = 8;

		if (Input.GetButtonDown ("Jump") && isGrounded)  // Player starts pressing the jump button
			jump = true;
		else if (Input.GetButtonDown ("Jump") && isPlatformed && !isCrouching)
			jump = true;
		if (Input.GetButtonUp ("Jump") && !isGrounded && !isPlatformed)   // Player stops pressing the button
			jumpCancel = true;

		if (Input.GetButtonDown ("Horizontal") && (isGrounded || isPlatformed)) 
		{
			if(Time.time - tapTime > 0f && Time.time - tapTime < tapDiff) //player doublee taps button
				doubleTap = true;
			else if(Time.time - tapTime == 0f || Time.time - tapTime > tapDiff) //player does not double tap button
				doubleTap = false;
			tapTime = Time.time;
		} 

		if (Input.GetAxisRaw("Vertical") == -1f && !startCrouch) 
		{
			startCrouch = true;
			crouchTime = Time.realtimeSinceStartup;
			//put crouch animation here
		}
		if(startCrouch)
		{
			if(Time.realtimeSinceStartup - crouchTime >= 0.25f)
			{
				isCrouching = true;
			}
		}
		if (isCrouching) 
		{
			startCrouch = false;
		}
		if (Input.GetAxisRaw ("Vertical") != -1f)
			isCrouching = false;
	}
	
	void FixedUpdate () 
	{
		if (jump) 
			isJumping (true);
		if (jumpCancel) 
			isJumping (false);
		
		if (doubleTap) // if player double tapped, run
			Run ();
		if (!doubleTap) // if not, walk
			Walk ();

		if (isCrouching && Input.GetButton ("Jump") && isPlatformed && rb.velocity.y == 0f) 
			Physics2D.IgnoreLayerCollision (8, 9);
		if (!isCrouching || !Input.GetButton("Jump"))
			Physics2D.IgnoreLayerCollision (8, 9, false);
		if (rb.velocity.y < -15f && isPlatformed) 
			Physics2D.IgnoreLayerCollision(8,9,false);
	}
}