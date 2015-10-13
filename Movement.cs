using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	//public float walkSpeed = 5f;      //tweakable
	public float maxSpeed = 10f;       //tweakable
	public LayerMask whatIsGround;
	public Transform groundCheck;
	public float jumpShortSpeed = 20f;   //tweakable
	public float jumpSpeed = 55f;		//tweakable

	bool facingRight = true;
	bool grounded = false;
	float groundRadius = 0.2f;
	bool jump = false;
	bool jumpCancel = false;
	Rigidbody2D rb;

	void Start () {
	
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround); // ground

		//***********************************Jump Mechanic****************************************************//
		if (Input.GetButtonDown("Jump") && grounded)   // Player starts pressing the button
			jump = true;
		if (Input.GetButtonUp("Jump") && !grounded)     // Player stops pressing the button
			jumpCancel = true;
		//***********************************Jump Mechanic****************************************************//

		//***********************************Platform Mechanic****************************************************//
		if (grounded && Input.GetButtonDown("Jump")) { // Change to No Platform Collision layer
			gameObject.layer = 10;
		} else if (!Input.GetButton ("Jump")) { // Change to Player layer
			gameObject.layer = 8;
		} 

		if (grounded && Input.GetAxis("Vertical") < 0) {
			gameObject.layer = 10;
			Physics2D.gravity = new Vector2(0f, -300f); //change gravity to x2 normal gravity
		}
		//***********************************Platform Mechanic****************************************************//
	
	}


	void FixedUpdate () {
	
		//***********************************Horizontal Movement****************************************************//
		float move = Input.GetAxisRaw ("Horizontal");
		rb.velocity = new Vector2 (move * maxSpeed, rb.velocity.y);

		if (move > 0 && !facingRight) {
			Flip ();
		}

		if (move < 0 && facingRight) {
			Flip ();
		}
		//***********************************Horizontal Movement****************************************************//
	


		//***********************************Jump Mechanic****************************************************//
		if (jump) {
			rb.gravityScale = 0.3f;
			rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
			jump = false;
		}

		if (jumpCancel) {
			if (rb.velocity.y > jumpShortSpeed) { 
				rb.gravityScale = 0.3f;
				rb.velocity = new Vector2(rb.velocity.x, jumpShortSpeed);
			}
			jumpCancel = false;
		}
		rb.gravityScale = 1f;
		Physics2D.gravity = new Vector2 (0, -150); //tweakable
		//***********************************Jump Mechanic****************************************************//
	
	}

	void Flip() {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
