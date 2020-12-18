using UnityEngine;

public class Dodgeball : MonoBehaviour
{
	public GameObject player;

	[SerializeField] private Material normalMat;
	[SerializeField] private Material activatedMat;

	public bool activated = false;

	public bool immunity = false;

	private void Update()
	{
		if (activated) { GetComponent<MeshRenderer>().material = activatedMat; }
		else { GetComponent<MeshRenderer>().material = normalMat; }

		if (immunity && !IsInvoking(nameof(disableImmunity)))
		{
			Invoke(nameof(disableImmunity), 0.1f);
		}
	}

	private void OnCollisionStay(Collision other)
	{
		if (other.gameObject.tag == "Ground" && activated && !immunity)
		{
			activated = false;
			player = null;
		}
		else if (other.gameObject.tag == "Player" && other.gameObject != player && activated && !immunity)
		{
			float power = Mathf.RoundToInt(GetComponent<Rigidbody>().velocity.magnitude);
			Debug.Log(player.name + " hit " + other.gameObject.name + " for " + power + " damage.");
		}
	}

	public void disableImmunity()
	{
		immunity = false;
	}
}
