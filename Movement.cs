using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public LayerMask ground;
	public Transform groundCheck;

	bool facingRight = true;
	bool jump = false;
	bool jumpCancel = false;
	bool doubleTap = false;
	bool isGrounded = false;

	float dropTime = 0f;
	float dropDiff = 0.15f;     //tweakable
	float tapTime = 0f; 
	float tapDiff = 0.2f;       //tweakable
	float walkSpeed = 10f;      //tweakable
	float runSpeed = 20f;       //tweakable
	float jumpSpeed = 55f;     //tweakable
	float groundRadius = 0.75f; //tweakable

	Rigidbody2D rb;


	void Start () {
		
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update () {

		isGrounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, ground);

		gameObject.layer = 8;

		// Platform Mechanic
		if (Input.GetAxis ("Vertical") < 0 && Input.GetButtonDown ("Jump") && isGrounded) {
			jump = false;
			Physics2D.IgnoreLayerCollision (8, 9);
			dropTime = Time.time;
		}
		if (Time.time - dropTime > 0f && Time.time - dropTime >= dropDiff) {
			Physics2D.IgnoreLayerCollision (8, 9, false);
			dropTime = 0f;
		}
		
		
		// Jump Mechanic
		if (Input.GetButtonDown ("Jump") && isGrounded && Input.GetAxisRaw ("Vertical") == 0)  // Player starts pressing the button
			jump = true;
		if (Input.GetButtonUp ("Jump") && !isGrounded)   // Player stops pressing the button
			jumpCancel = true;
		
		
		// Double Tap Mechanic
		if (Input.GetButtonDown ("Horizontal") && isGrounded) {
			if(Time.time - tapTime > 0f && Time.time - tapTime < tapDiff)
				doubleTap = true;
			else if(Time.time - tapTime == 0f || Time.time - tapTime > tapDiff)
				doubleTap = false;
			tapTime = Time.time;
		}
		if (doubleTap)
			Run ();
		if (!doubleTap)
			Walk ();
		
		
	}
	
	void Run() {
		
		float move = Input.GetAxisRaw("Horizontal");
		if (move > 0 && !facingRight)
			Flip ();
		if (move < 0 && facingRight) 
			Flip ();
		rb.velocity = new Vector2 (move * runSpeed, rb.velocity.y);
		if (!Input.GetButton ("Horizontal"))
			doubleTap = false;
	}
	
	void Walk() {
		
		float move = Input.GetAxisRaw("Horizontal");
		if (move > 0 && !facingRight)
			Flip ();
		if (move < 0 && facingRight) 
			Flip ();
		rb.velocity = new Vector2 (move * walkSpeed, rb.velocity.y);
		
	}
	
	void FixedUpdate () {
		
		// Jump Mechanic
		if (jump) {
			rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
			jump = false;
		}
		
		if (jumpCancel) {
			if (rb.velocity.y > 0) { 
				rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);      // Change upward velocity by a factor of 1/x, x is tweakable
			}
			jumpCancel = false;
		}
		
		
	}
	
	void Flip() {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}