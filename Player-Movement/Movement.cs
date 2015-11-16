using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	
	public LayerMask ground;
	public LayerMask platform;
	public Transform groundCheck;
	public Transform platformCheck;
	
	bool facingRight = true;
	bool jump = false;
	bool jumpCancel = false;
	bool doubleTap = false;
	bool isGrounded = false;
	bool isPlatformed = false;
	
	float dropTime = 0f;
	float dropDiff = 0.15f;     //tweakable
	float tapTime = 0f; 
	float tapDiff = 0.2f;       //tweakable
	float walkSpeed = 10f;      //tweakable
	float runSpeed = 20f;       //tweakable
	float jumpSpeed = 55f;     //tweakable
	float fixJumpSpeed = 55f; //tweakable
	float groundRadius = 0.75f; //tweakable
	float platformRadius = 0.75f; //tweatkable
	
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
		if (Input.GetAxisRaw ("Vertical") < 0f)
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
		if (Input.GetAxisRaw ("Vertical") < 0f)
			move = 0;
		rb.velocity = new Vector2 (move * runSpeed, rb.velocity.y);
		if (!Input.GetButton ("Horizontal")) //if not pressing horizontal movement button, stop running
			doubleTap = false;
		if(!isGrounded && !isPlatformed && (rb.velocity.x == (move * runSpeed))) //if your velocity in the x direction in the air is equal to the running velocity
		{ 
			if(Input.GetButtonDown("Horizontal"))  //if you press the horizontal movement button
			{ 
				doubleTap = false; // cancel the double tap
				Walk (); //change to walking speed
			}
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

		if (Input.GetAxis ("Vertical") < 0 && Input.GetButtonDown ("Jump") && isPlatformed) 
		{
			jump = false;
			Physics2D.IgnoreLayerCollision (8, 9); //ignore collision between player and platform 
			dropTime = Time.time; //start a timer
		}
		if (Time.time - dropTime > 0f && Time.time - dropTime >= dropDiff) //if x amount of time has passed, reset the "ignore ollision" 
		{ 
			Physics2D.IgnoreLayerCollision (8, 9, false);
			dropTime = 0f;
		}

		if (Input.GetButtonDown ("Jump") && isGrounded)  // Player starts pressing the jump button
			jump = true;
		else if (Input.GetButtonDown ("Jump") && isPlatformed && Input.GetAxis("Vertical") == 0f)
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

	}
	
	void FixedUpdate () 
	{
		if (jump) 
		{
			if (Input.GetAxisRaw ("Vertical") < 0f)
				jumpSpeed = 0f;
			else
				jumpSpeed = fixJumpSpeed;
			rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
			jump = false;
		}
		
		if (jumpCancel) 
		{
			if (rb.velocity.y > 0) 
				rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);      // Change upward velocity by a factor of 1/x, x is tweakable
			jumpCancel = false;
		}


		if (doubleTap) // if player double tapped, run
			Run ();
		if (!doubleTap) // if not, walk
			Walk ();
	}
}