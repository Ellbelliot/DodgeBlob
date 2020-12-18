using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// Input
	private InputMaster controls;

	private Vector2 moveInput;
	private bool isSprinting;
	private bool isJumping;

	private void Awake() => controls = new InputMaster();

	private void Start()
	{
		// Runs proper functions when buttons are pressed
		controls.Player.Movement.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
		controls.Player.Movement.canceled += ctx => SetMovement(Vector2.zero);

		controls.Player.Sprint.performed += ctx => SetSprint(true);
		controls.Player.Sprint.canceled += ctx => SetSprint(false);

		controls.Player.Jump.performed += ctx => SetJump(true);
		controls.Player.Jump.canceled += ctx => SetJump(false);
	}

	private void SetMovement(Vector2 _moveInput) => moveInput = _moveInput;

	private void SetSprint(bool _isSprinting) => isSprinting = _isSprinting;

	private void SetJump(bool _isJumping) => isJumping = _isJumping;

	// Properly sets up the controls script
	private void OnEnable() => controls.Enable();
	private void OnDisable() => controls.Disable();

	private void FixedUpdate()
	{
		SendInputToServer();
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

		ClientSend.PlayerMovement(_floatInputs, _boolInputs);
	}
}
