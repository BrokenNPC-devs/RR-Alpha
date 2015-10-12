using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	//public float walkSpeed = 5f;      //tweakable
	public float maxSpeed = 10f;       //tweakable
	public LayerMask whatIsGround;
	public Transform groundCheck;
	public float jumpShortSpeed = 10f;   // tweakable
	public float jumpSpeed = 55f;  		// tweakable

	bool facingRight = true;
	bool grounded = false;
	float groundRadius = 0.2f;
	bool jump = false;
	bool jumpCancel = false;

	void Start () {
	
	}

	void Update () {
		
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround); // ground

		//***********************************Jump Mechanic****************************************************//
		if (Input.GetButtonDown("Jump") && grounded) { // Player presses button
			jump = true;
		}
		if (Input.GetButtonUp("Jump") && !grounded)     // Player stops pressing the button
			jumpCancel = true;
		//***********************************Jump Mechanic****************************************************//
		
		//***********************************Platform Mechanic****************************************************//
		if (Input.GetButton ("Jump")) { // Change to No Platform Collision layer
			gameObject.layer = 10;
		} else if (!Input.GetButton ("Jump")) { // Change to Player layer
			gameObject.layer = 8;
		} 
		if (Input.GetButton("Vertical")) {
			gameObject.layer = 10;
		}
		//***********************************Platform Mechanic****************************************************//
	
	}


	void FixedUpdate () {
	
		//***********************************Horizontal Movement****************************************************//
		float move = Input.GetAxisRaw ("Horizontal");
		GetComponent<Rigidbody2D> ().velocity = new Vector2 (move * maxSpeed, GetComponent<Rigidbody2D> ().velocity.y);

		if (move > 0 && !facingRight) {
			Flip ();
		}
		if (move < 0 && facingRight) {
			Flip ();
		}
		//***********************************Horizontal Movement****************************************************//
	


		//***********************************Jump Mechanic****************************************************//
		if (jump)
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpSpeed);
			jump = false;
		}
		if (jumpCancel)
		{
			if (GetComponent<Rigidbody2D>().velocity.y > jumpShortSpeed)
			{ 
				GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpShortSpeed);
			}
			jumpCancel = false;
		}
		if (grounded == true) 
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
