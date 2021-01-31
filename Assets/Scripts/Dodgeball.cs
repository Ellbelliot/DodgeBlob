using UnityEngine;

public class Dodgeball : MonoBehaviour
{
	public GameObject player;
	public int id;

	[SerializeField] private Material normalMat;
	[SerializeField] private Material activatedMat;

	public bool activated = false;
	public bool immunity = false;

	private void Awake()
	{
		GameManager.dodgeballs.Add(id, this);
	}

	private void Update()
	{
		if (activated) { GetComponent<MeshRenderer>().material = activatedMat; }
		else { GetComponent<MeshRenderer>().material = normalMat; }

		if (immunity && !IsInvoking(nameof(disableImmunity)))
		{
			Invoke(nameof(disableImmunity), 0.1f);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Ground") && activated && !immunity)
		{
			activated = false;
			player = null;
		}
	}

	private void OnCollisionStay(Collision other)
	{
		if (other.gameObject.CompareTag("Ground") && activated && !immunity)
		{
			activated = false;
			player = null;
		}
	}

	public void disableImmunity()
	{
		immunity = false;
	}
}
