using UnityEngine;
using UnityEngine.UI;

public class PlayerGrab : MonoBehaviour
{
	// Assignables
	[Header("Assignables")]
	[SerializeField] private GameObject cam;
	[SerializeField] private GameObject grab;
	[SerializeField] private Slider chargeMeter;
	[SerializeField] private Animator grabAnim;
	[SerializeField] private Animator slotAnim;

	[SerializeField] private float grabRange;
	[SerializeField] private LayerMask grabMask;

	[SerializeField] private GameObject dodgeball1Silhouette;
	[SerializeField] private GameObject dodgeball2Silhouette;
	[SerializeField] private GameObject dodgeball3Silhouette;

	[SerializeField] private Sprite dodgeballSprite;
	[SerializeField] private Sprite silhouetteSprite;

	[SerializeField] private Color silhouetteColor;

	// Throw values
	[Header("Throw Values")]
	[SerializeField] private float maxThrowSpeed;
	[SerializeField] private float minThrowSpeed;
	[SerializeField] private float throwSpeed;

	[SerializeField] private LayerMask checkableLayers;

	private Rigidbody body;

	[SerializeField] private GameObject selectedDodgeball;

	[SerializeField] private GameObject dodgeball1;
	[SerializeField] private GameObject dodgeball2;
	[SerializeField] private GameObject dodgeball3;

	[SerializeField] private bool grabbing = false;
	[SerializeField] private bool holding = false;
	[SerializeField] private bool charging = false;

	private InputMaster controls;

	private void Awake() => controls = new InputMaster();

	private void Start()
	{
		body = GetComponent<Rigidbody>();
		throwSpeed = minThrowSpeed;
		chargeMeter.gameObject.SetActive(false);

		SetupControls();
	}

	private void OnEnable() => controls.Enable();

	private void OnDisable() => controls.Disable();

	void SetupControls()
	{
		controls.Player.Grab.performed += ctx => Grab();
		controls.Player.Grab.canceled += ctx => ResetGrab();

		controls.Player.Shoot.performed += ctx => Charge();

		controls.Player.Slot1.performed += ctx => SwitchSlot(1);
		controls.Player.Slot2.performed += ctx => SwitchSlot(2);
		controls.Player.Slot3.performed += ctx => SwitchSlot(3);

		controls.Player.SlotScroll.performed += ctx => SlotScroll(ctx.ReadValue<float>());
	}

	private void Grab()
	{
		if (holding) return;

		grabAnim.SetBool("Reaching", true);
		grabbing = true;
	}

	private void ResetGrab()
	{
		if (holding) return;

		grabAnim.SetBool("Reaching", false);
		grabbing = false;
	}

	private void Charge()
	{
		if (!holding) return;

		grabAnim.SetBool("Charging", true);
		chargeMeter.gameObject.SetActive(true);
		charging = true;
	}

	public void Throw(int _slot)
	{
		if (!holding) return;

		SwitchSlot(_slot);
		grabAnim.SetBool("Charging", false);
		charging = false;
		grabbing = false;
		holding = false;

		selectedDodgeball.GetComponent<Collider>().enabled = true;
		selectedDodgeball.GetComponent<Dodgeball>().activated = true;
		selectedDodgeball.GetComponent<Dodgeball>().immunity = true;
		selectedDodgeball.GetComponent<Rigidbody>().isKinematic = false;
		selectedDodgeball.GetComponent<Rigidbody>().detectCollisions = true;
		selectedDodgeball.GetComponent<Rigidbody>().velocity = (cam.transform.forward + Vector3.up / (throwSpeed / 3)).normalized * throwSpeed;
		selectedDodgeball.transform.SetParent(null);
		selectedDodgeball.transform.position = transform.position + cam.transform.forward / 3 - cam.transform.up / 15;

		if (Physics.CheckSphere(transform.position + cam.transform.forward / 2, 0.2f, checkableLayers))
		{
			selectedDodgeball.GetComponent<Rigidbody>().velocity *= 1 / 2;
		}

		ChangeDodgeball(null, _slot);
		throwSpeed = minThrowSpeed;
		chargeMeter.value = 0;
		chargeMeter.gameObject.SetActive(false);
	}

	private void SwitchSlot(int slotNum)
	{
		slotAnim.SetInteger("Selected", slotNum);

		if (slotNum == 3)
		{
			if (dodgeball3 != null) { ChangeSlot(dodgeball3); }
			else { ChangeSlot(null); }
		}
		if (slotNum == 2)
		{
			if (dodgeball2 != null) { ChangeSlot(dodgeball2); }
			else { ChangeSlot(null); }
		}
		if (slotNum == 1)
		{
			if (dodgeball1 != null) { ChangeSlot(dodgeball1); }
			else { ChangeSlot(null); }
		}
	}

	private void ChangeSlot(GameObject obj)
	{
		selectedDodgeball = obj;

		if (obj != null) 
		{ 
			holding = true;
			grabAnim.SetBool("Reaching", false);
		}
		else 
		{ 
			holding = false; 
		}

		grabbing = false;
		charging = false;

		grabAnim.SetBool("Charging", false);
		throwSpeed = minThrowSpeed;
		chargeMeter.value = 0;
		chargeMeter.gameObject.SetActive(false);
	}

	private void SlotScroll(float value)
	{
		int sign = -Mathf.RoundToInt(Mathf.Sign(value));
		if (slotAnim.GetInteger("Selected") == 2 || (slotAnim.GetInteger("Selected") == 3 && sign != 1) || (slotAnim.GetInteger("Selected") == 1 && sign != -1))
		{
			SwitchSlot(slotAnim.GetInteger("Selected") + sign);
		}
		else
		{
			SwitchSlot(slotAnim.GetInteger("Selected") - 2 * sign);
		}
	}

	private void ChangeDodgeball(GameObject newDodgeball, int slot)
	{
		if (slot != slotAnim.GetInteger("Selected"))
		{
			SwitchSlot(slot);
		}
		if (slot == 3)
		{
			dodgeball3 = newDodgeball;
			if (newDodgeball != null)
			{
				dodgeball3Silhouette.GetComponent<Image>().sprite = dodgeballSprite;
				dodgeball3Silhouette.GetComponent<Image>().color = Color.white;
				newDodgeball.GetComponent<Dodgeball>().player = gameObject;
			}
			else
			{
				dodgeball3Silhouette.GetComponent<Image>().sprite = silhouetteSprite;
				dodgeball3Silhouette.GetComponent<Image>().color = silhouetteColor;
			}
		}
		if (slot == 2)
		{
			dodgeball2 = newDodgeball;
			if (newDodgeball != null)
			{
				dodgeball2Silhouette.GetComponent<Image>().sprite = dodgeballSprite;
				dodgeball2Silhouette.GetComponent<Image>().color = Color.white;
				newDodgeball.GetComponent<Dodgeball>().player = gameObject;
			}
			else
			{
				dodgeball2Silhouette.GetComponent<Image>().sprite = silhouetteSprite;
				dodgeball2Silhouette.GetComponent<Image>().color = silhouetteColor;
			}
		}
		if (slot == 1)
		{
			dodgeball1 = newDodgeball;
			if (newDodgeball != null)
			{
				dodgeball1Silhouette.GetComponent<Image>().sprite = dodgeballSprite;
				dodgeball1Silhouette.GetComponent<Image>().color = Color.white;
				newDodgeball.GetComponent<Dodgeball>().player = gameObject;
			}
			else
			{
				dodgeball1Silhouette.GetComponent<Image>().sprite = silhouetteSprite;
				dodgeball1Silhouette.GetComponent<Image>().color = silhouetteColor;
			}
		}
		selectedDodgeball = newDodgeball;
	}

	private void Update()
	{
		if (charging)
		{
			ChargeThrowPower();
		}
		if (selectedDodgeball)
		{
			selectedDodgeball.transform.localPosition = Vector3.zero;
		}
	}

	public void DropDodgeballs()
	{
		if (dodgeball1 != null)
		{
			dodgeball1.SetActive(true);
			dodgeball1.transform.parent = null;
			dodgeball1.GetComponent<Collider>().enabled = true;
			dodgeball1.GetComponent<Dodgeball>().activated = true;
			dodgeball1.GetComponent<Dodgeball>().immunity = true;
			dodgeball1.GetComponent<Rigidbody>().isKinematic = false;
			dodgeball1.GetComponent<Rigidbody>().detectCollisions = true;
			dodgeball1 = null;
			dodgeball1Silhouette.GetComponent<Image>().sprite = silhouetteSprite;
			dodgeball1Silhouette.GetComponent<Image>().color = silhouetteColor;
		}
		if (dodgeball2 != null)
		{
			dodgeball2.SetActive(true);
			dodgeball2.transform.parent = null;
			dodgeball2.GetComponent<Collider>().enabled = true;
			dodgeball2.GetComponent<Dodgeball>().activated = true;
			dodgeball2.GetComponent<Dodgeball>().immunity = true;
			dodgeball2.GetComponent<Rigidbody>().isKinematic = false;
			dodgeball2.GetComponent<Rigidbody>().detectCollisions = true;
			dodgeball2 = null;
			dodgeball2Silhouette.GetComponent<Image>().sprite = silhouetteSprite;
			dodgeball2Silhouette.GetComponent<Image>().color = silhouetteColor;
		}
		if (dodgeball3 != null)
		{
			dodgeball3.SetActive(true);
			dodgeball3.transform.parent = null;
			dodgeball3.GetComponent<Collider>().enabled = true;
			dodgeball3.GetComponent<Dodgeball>().activated = true;
			dodgeball3.GetComponent<Dodgeball>().immunity = true;
			dodgeball3.GetComponent<Rigidbody>().isKinematic = false;
			dodgeball3.GetComponent<Rigidbody>().detectCollisions = true;
			dodgeball3 = null;
			dodgeball3Silhouette.GetComponent<Image>().sprite = silhouetteSprite;
			dodgeball3Silhouette.GetComponent<Image>().color = silhouetteColor;
		}
		if (selectedDodgeball != null)
		{
			selectedDodgeball = null;
		}

		slotAnim.SetInteger("Selected", 0);
	}

	public void GrabDodgeball(GameObject dodgeball, int slot)
	{
		dodgeball.transform.SetParent(grab.transform);
		dodgeball.transform.localPosition = Vector3.zero;
		dodgeball.GetComponent<Dodgeball>().activated = false;
		dodgeball.GetComponent<Dodgeball>().immunity = false;
		dodgeball.GetComponent<Dodgeball>().player = gameObject;
		dodgeball.GetComponent<Collider>().enabled = false;
		dodgeball.GetComponent<Rigidbody>().isKinematic = true;
		dodgeball.GetComponent<Rigidbody>().detectCollisions = true;

		ChangeDodgeball(dodgeball, slot);
		grabAnim.SetBool("Reaching", false);

		holding = true;
		grabbing = false;
	}

	void ChargeThrowPower()
	{
		if (throwSpeed < maxThrowSpeed)
		{
			throwSpeed += Time.deltaTime * (maxThrowSpeed - minThrowSpeed);
			chargeMeter.value += Time.deltaTime;
		}
		else
		{
			throwSpeed = maxThrowSpeed;
			chargeMeter.value = 1;
		}
	}
}
