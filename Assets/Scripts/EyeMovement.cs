using UnityEngine;

public class EyeMovement : MonoBehaviour
{
	[SerializeField] Transform blob;
	[SerializeField] Transform cam;

	[SerializeField] Vector3 rotationOffset;
	[SerializeField] Vector3 positionOffset;

	private void Update()
	{
		transform.eulerAngles = cam.transform.eulerAngles + rotationOffset;

		Vector3 finalPositionOffset = positionOffset.x * cam.transform.right + positionOffset.y * cam.transform.up + positionOffset.z * cam.transform.forward;
		transform.position = blob.transform.position + finalPositionOffset;
	}
}
