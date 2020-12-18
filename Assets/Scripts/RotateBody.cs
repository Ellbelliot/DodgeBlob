using UnityEngine;

public class RotateBody : MonoBehaviour
{
	[SerializeField] private Transform cam;
	[SerializeField] private Rigidbody blob;
	[SerializeField] private Animator anim;
	[SerializeField] private float offset;

    void LateUpdate()
    {
		if (anim.GetBool("WallJumping"))
		{
			Vector3 normalizedVelocity = new Vector3(blob.velocity.x, 0, blob.velocity.z).normalized;
			transform.LookAt(transform.position - normalizedVelocity);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + offset, transform.eulerAngles.z);
		}
		else if (blob.velocity.magnitude > 0)
		{
			Vector3 normalizedVelocity = new Vector3(blob.velocity.x, 0, blob.velocity.z).normalized;
			transform.LookAt(transform.position + normalizedVelocity);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + offset, transform.eulerAngles.z);
		}
		else
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam.eulerAngles.y + offset, transform.eulerAngles.z);
		}
    }
}
