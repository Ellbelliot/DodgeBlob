using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	[SerializeField] private Transform head;

	[SerializeField] private float sensitivity = 10;

	private float horizontalRotation = 0;
	private float verticalRotation = 0;

	private Vector2 previousInput;

	private InputMaster controls;

	private void Awake() => controls = new InputMaster();

	private void Start()
	{
		// Using the new input system to get mouse input!
		controls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
		controls.Player.Look.canceled += ctx => ResetInput();
	}

	private void OnEnable() => controls.Enable();

	private void OnDisable() => controls.Disable();

	private void Look(Vector2 mouseVector) => previousInput = mouseVector;

	private void ResetInput() => previousInput = Vector2.zero;

	private void LookAround()
	{
		// Using fixed delta time so that if the game time slows for some reason you can still look around at the same speed
		Vector2 lookVector = previousInput * sensitivity * Time.fixedDeltaTime;

		// Find current look rotation
		Vector3 currentRotation = transform.localRotation.eulerAngles;
		horizontalRotation = currentRotation.y + lookVector.x;

		// Clamp vertical rotation
		verticalRotation -= lookVector.y;
		verticalRotation = Mathf.Clamp(verticalRotation, -89.99f, 89.99f);

		// Actually rotates the camera
		transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
	}

	private void Update()
	{
		transform.position = head.position;
		LookAround();

		// No cursor, because that would be annoying
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}
