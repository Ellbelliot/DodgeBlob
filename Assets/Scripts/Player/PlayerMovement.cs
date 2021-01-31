using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	// Assignables
	[Header("Camera")]
	[SerializeField] private GameObject cam;

	private Rigidbody rb;
	private SphereCollider col;

	// Movement
	[Header("Movement")]
	[SerializeField] private float moveSpeed = 700;
	[SerializeField] private float maxSpeed = 10;
	[SerializeField] private float walkSpeed = 6;
	[SerializeField] private float sprintSpeed = 10;
	private float multiplier;

	[SerializeField] private float counterMovement = 0.75f;
	[SerializeField] private float airCounterMovement = 0.25f;
	[SerializeField] private float groundCounterMovement = 0.75f;
	private float threshold = 0.01f;

	private Vector3 forward;
	private Vector3 right;

	// Jumping
	[Header("Jumping")]
	[SerializeField] private float jumpForce;
	[SerializeField] private float jumpCooldown = 0.25f;

	[SerializeField] private LayerMask groundLayers;

	private bool readyToJump = true;
	private bool jumping = false;

	private bool grounded = true;
	private bool cancellingWallJump;

	// Wall Jumping
	[SerializeField] private float minWallAngle = 45;

	private bool onWall;
	private bool wallJumped = false;
	private Vector3 normalVector = Vector3.up;

	// Animations
	[SerializeField] private Animator anim;

	// Input
	private InputMaster controls;

	private Vector2 moveInput;

	private void Awake() => controls = new InputMaster();

	private void Start()
	{
		// Runs proper functions when buttons are pressed
		controls.Player.Movement.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
		controls.Player.Movement.canceled += ctx => ResetMovement();

		controls.Player.Sprint.performed += ctx => Sprint();
		controls.Player.Sprint.canceled += ctx => Walk();

		controls.Player.Jump.performed += ctx => StartJump();
		controls.Player.Jump.canceled += ctx => StopJump();

		// Assigns components in script
		rb = GetComponent<Rigidbody>();
		col = GetComponent<SphereCollider>();
	}

	// Properly sets up the controls script
	private void OnEnable() => controls.Enable();

	private void OnDisable() => controls.Disable();

	// Stores the input in a value
	private void SetMovement(Vector2 inputVector)
	{
		moveInput = inputVector;
	}

	private void ResetMovement()
	{
		moveInput = Vector2.zero;
	}

	private void AirAnimation()
	{
		if (Mathf.Abs(rb.velocity.y) < 2 && !anim.GetBool("WallJumping"))
		{
			ResetAnimation();
			anim.SetBool("Peaking", true);
		}
		else if (rb.velocity.y < 0)
		{
			ResetAnimation();
			anim.SetBool("Falling", true);
		}
	}

	private void ResetAnimation()
	{
		CancelInvoke(nameof(ResetAnimation));
		anim.SetBool("Peaking", false);
		anim.SetBool("Jumping", false);
		anim.SetBool("Falling", false);
		anim.SetBool("Land", false);
		anim.SetBool("Move", false);
		anim.SetBool("WallJumping", false);
	}

	// Changes max speed depending on if you're sprinting
	private void Sprint()
	{
		maxSpeed = sprintSpeed;
		anim.SetFloat("MoveMultiplier", 2);
	}

	private void Walk()
	{
		maxSpeed = walkSpeed;
		anim.SetFloat("MoveMultiplier", 1);
	}

	// Stores jump input
	private void StartJump() => jumping = true;

	private void StopJump() => jumping = false;

	private void FixedUpdate()
	{
		// Checks if we're on the ground
		grounded = Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z), col.radius * 0.35f, groundLayers);
		HandleAnimation();
		Move();
		Jump();
	}

	private void HandleAnimation()
	{
		if (grounded && !jumping)
		{
			wallJumped = false;
			if (anim.GetBool("Falling"))
			{
				ResetAnimation();
				anim.SetBool("Land", true);
				Invoke(nameof(ResetAnimation), 0.5f);
			}
			else if (moveInput != Vector2.zero)
			{
				ResetAnimation();
				anim.SetBool("Move", true);
			}
			else
			{
				ResetAnimation();
			}
		}
		else
		{
			AirAnimation();
		}
	}

	private void Move()
	{
		// Properly sets up orientation variables
		forward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
		right = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;

		// Normalized direction in which we wanna move this frame
		Vector3 movementDirection = (right * moveInput.x + forward * moveInput.y).normalized;

		// Don't go too fast!
		if (movementDirection.x > 0 && rb.velocity.x > maxSpeed) movementDirection.x = 0;
		if (movementDirection.x < 0 && rb.velocity.x < -maxSpeed) movementDirection.x = 0;
		if (movementDirection.z > 0 && rb.velocity.z > maxSpeed) movementDirection.z = 0;
		if (movementDirection.z < 0 && rb.velocity.z < -maxSpeed) movementDirection.z = 0;

		// Slows down movement in mid-air
		if (!grounded) multiplier = .3f;
		else multiplier = 1;

		// Actually moves the player
		Vector3 movement = movementDirection * moveSpeed * multiplier * Time.deltaTime;
		rb.AddForce(movement);

		//Limit diagonal running.
		if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
		{
			Vector3 correctedVel = rb.velocity.normalized * maxSpeed;
			rb.velocity = new Vector3(correctedVel.x, rb.velocity.y, correctedVel.z);
		}

		StopSliding();
	}

	private void Jump()
	{
		// if we're on the ground, or on a wall
		if (((grounded && readyToJump) || (onWall && readyToJump)) && jumping)
		{
			readyToJump = false;
			jumping = true;

			// Adds jump forces
			rb.AddForce(Vector2.up * jumpForce * 1.25f);
			rb.AddForce(normalVector * jumpForce * 0.75f);

			// So that you dont jump too high when going down
			Vector3 vel = rb.velocity;
			if (vel.y < 0.5f)
				rb.velocity = new Vector3(vel.x, 0, vel.z);
			else if (vel.y > 0)
				rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

			if (onWall)
			{
				wallJumped = true;
				ResetAnimation();
				anim.SetBool("WallJumping", true);
			}
			else
			{
				ResetAnimation();
				anim.SetBool("Jumping", true);
			}

			// Adds a little delay between jumps
			Invoke(nameof(ResetJump), jumpCooldown);
		}
	}

	private void ResetJump() => readyToJump = true;

	private void ResetWallJump() => wallJumped = false;

	private void StopSliding()
	{
		if (wallJumped) return;

		Vector2 relativeVel = FindVelRelativeToLook();

		//Counter movement
		if (!grounded || jumping)
		{
			counterMovement = airCounterMovement;
		}
		else
		{
			counterMovement = groundCounterMovement;

			if (Mathf.Abs(relativeVel.x) > threshold && Mathf.Abs(moveInput.x) < 0.05f)
			{
				rb.AddForce(right * Time.deltaTime * -relativeVel.x * counterMovement);
			}
			if (Mathf.Abs(relativeVel.y) > threshold && Mathf.Abs(moveInput.y) < 0.05f)
			{
				rb.AddForce(forward * Time.deltaTime * -relativeVel.y * counterMovement);
			}
		}

		if ((relativeVel.x < -threshold && moveInput.x > 0) || (relativeVel.x > threshold && moveInput.x < 0))
		{
			rb.AddForce(right * Time.deltaTime * -relativeVel.x * counterMovement);
		}
		if ((relativeVel.y < -threshold && moveInput.y > 0) || (relativeVel.y > threshold && moveInput.y < 0))
		{
			rb.AddForce(forward * Time.deltaTime * -relativeVel.y * counterMovement);
		}
	}

	// Math stuff. Basically finds out how fast we're moving right, relative to where we're looking, 
	// and how fast we are moving forward, relative to where we're looking.
	public Vector2 FindVelRelativeToLook()
	{
		Vector3 flatForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;

		float rightVel = Vector3.Dot(rb.velocity, cam.transform.right);
		float forwardVel = Vector3.Dot(rb.velocity, flatForward);

		return new Vector2(rightVel, forwardVel);
	}

	private bool IsValidWall(Vector3 collisionPoint)
	{
		// We don't wanna be able to wall jump on ramps!
		float angle = Vector3.Angle(Vector3.up, collisionPoint);
		return angle > minWallAngle;
	}

	private void OnCollisionStay(Collision other)
	{
		// Make sure we are only checking the ground
		if (other.gameObject.CompareTag("Wall") && !grounded)
		{
			// Iterate through every collision in a physics update
			for (int i = 0; i < other.contactCount; i++)
			{
				Vector3 normal = other.contacts[i].normal;
				// This is a wall!
				if (IsValidWall(normal))
				{
					onWall = true;
					cancellingWallJump = false;
					normalVector = normal;
					CancelInvoke(nameof(StopWalled));
				}
			}

			// Invoke ground/wall cancel, since we can't check normals with CollisionExit
			float delay = 3f;
			if (!cancellingWallJump)
			{
				cancellingWallJump = true;
				Invoke(nameof(StopWalled), Time.deltaTime * delay);
			}
		}
	}

	private void StopWalled()
	{
		// We're no longer on a wall.
		onWall = false;
		normalVector = Vector3.up;
	}
}
