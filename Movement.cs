using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float walkSpeed = 10f;      //tweakable
	public float runSpeed = 50f;       //tweakable
	public float jumpSpeed = 55f;     //tweakable
	public LayerMask whatIsGround;
	public Transform groundCheck;

	bool facingRight = true;
	bool grounded = false;
	bool jump = false;
	bool jumpCancel = false;
	float groundRadius = 0.2f;
	float dropTime = 0f;
	float lastTime = -1.0f;
	Rigidbody2D rb;




	void Start () {
	
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update () {

		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround); // Find Ground
		gameObject.layer = 8;


		// Platform Mechanic
		if (Input.GetAxis ("Vertical") < 0 && Input.GetButtonDown ("Jump") && grounded) {
			jump = false;
			Physics2D.IgnoreLayerCollision (8, 9);
			dropTime = Time.time;
		}
		if (Time.time - dropTime > 0f && Time.time - dropTime >= 0.15f) {
			Physics2D.IgnoreLayerCollision (8, 9, false);
			dropTime = 0f;
		}


		// Jump Mechanic
		if (Input.GetButtonDown ("Jump") && grounded && Input.GetAxisRaw ("Vertical") == 0)  // Player starts pressing the button
			jump = true;
		if (Input.GetButtonUp ("Jump") && !grounded)   // Player stops pressing the button
			jumpCancel = true;


		// Double Tap Mechanic
		//Walk ();
		Run ();

	}

	void Run() {

		float move = Input.GetAxisRaw("Horizontal");
		if (move > 0 && !facingRight)
			Flip ();
		if (move < 0 && facingRight) 
			Flip ();
		rb.velocity = new Vector2 (move * runSpeed, rb.velocity.y);

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
