using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	// Animation
	[SerializeField] private Animator anim;
	[SerializeField] private Animator slotAnim;
	[SerializeField] private Animator grabAnim;

	// Input
	private InputMaster controls;

	private Vector2 moveInput;
	private bool isSprinting;
	private bool isJumping;

	private bool isGrabbing;
	private bool isShooting;

	private void Awake() => controls = new InputMaster();

	private void Start()
	{
		SetMovement(Vector2.zero);
		SetSprint(false);
		SetJump(false);

		// Runs proper functions when buttons are pressed
		controls.Player.Movement.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
		controls.Player.Movement.canceled += ctx => SetMovement(Vector2.zero);

		controls.Player.Sprint.performed += ctx => SetSprint(true);
		controls.Player.Sprint.canceled += ctx => SetSprint(false);

		controls.Player.Jump.performed += ctx => SetJump(true);
		controls.Player.Jump.canceled += ctx => SetJump(false);

		controls.Player.Grab.performed += ctx => SetGrab(true);
		controls.Player.Grab.canceled += ctx => SetGrab(false);

		controls.Player.Shoot.performed += ctx => SetShoot(true);
		controls.Player.Shoot.canceled += ctx => SetShoot(false);
	}

	private void SetMovement(Vector2 _moveInput) => moveInput = _moveInput;

	private void SetSprint(bool _isSprinting) => isSprinting = _isSprinting;

	private void SetJump(bool _isJumping) => isJumping = _isJumping;

	private void SetGrab(bool _isGrabbing) => isGrabbing = _isGrabbing;

	private void SetShoot(bool _isShooting) => isShooting = _isShooting;

	// Properly sets up the controls script
	private void OnEnable() => controls.Enable();
	private void OnDisable() => controls.Disable();

	private void FixedUpdate()
	{
		SendInputToServer();
		SendAnimationToServer();
	}

	private void OnDestroy()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		SceneManager.LoadScene("Multiplayer");
	}

	public void SendInputToServer()
	{
		float[] _floatInputs = new float[]
		{
			moveInput.x,
			moveInput.y
		};

		bool[] _boolInputs = new bool[]
		{
			isSprinting,
			isJumping
		};

		bool[] _grabInputs = new bool[]
		{
			isGrabbing,
			isShooting
		};

		ClientSend.PlayerMovement(_floatInputs, _boolInputs);
		ClientSend.PlayerGrab(_grabInputs, slotAnim.GetInteger("Selected"));
	}

	public void SendAnimationToServer()
	{
		bool[] _params = new bool[]
		{
			anim.GetBool("Peaking"),
			anim.GetBool("Jumping"),
			anim.GetBool("Falling"),
			anim.GetBool("Land"),
			anim.GetBool("Move"),
			anim.GetBool("WallJumping")
		};

		float moveMultiplier = anim.GetFloat("MoveMultiplier");

		ClientSend.PlayerAnimation(_params, moveMultiplier);
	}
}
