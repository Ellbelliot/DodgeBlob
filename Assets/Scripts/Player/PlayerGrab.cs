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

	private GameObject selectedDodgeball;

	private GameObject dodgeball1;
	private GameObject dodgeball2;
	private GameObject dodgeball3;

	private bool grabbing = false;
	private bool holding = false;
	private bool charging = false;

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
		controls.Player.Shoot.canceled += ctx => Throw();

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

	private void Throw()
	{
		if (!holding) return;

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

		selectedDodgeball = null;
		ChangeDodgeball(null);
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
		if (selectedDodgeball != null) { selectedDodgeball.SetActive(false); }
		selectedDodgeball = obj;

		if (obj != null) { selectedDodgeball.SetActive(true); }

		if (obj != null) { holding = true; }
		else { holding = false; }

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

	private void ChangeDodgeball(GameObject newDodgeball)
	{
		if (slotAnim.GetInteger("Selected") == 3)
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
		if (slotAnim.GetInteger("Selected") == 2)
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
		if (slotAnim.GetInteger("Selected") == 1)
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
		if (grabbing)
		{
			FindGrab();
		}
		if (charging)
		{
			ChargeThrowPower();
		}
	}

	void FindGrab()
	{
		if (Physics.CheckSphere(grab.transform.position, grabRange, grabMask))
		{
			Collider[] colliders = Physics.OverlapSphere(grab.transform.position, grabRange, grabMask);

			colliders[0].transform.SetParent(grab.transform);
			colliders[0].transform.localPosition = Vector3.zero;
			colliders[0].GetComponent<Dodgeball>().activated = false;
			colliders[0].GetComponent<Dodgeball>().immunity = false;
			colliders[0].enabled = false;
			colliders[0].gameObject.GetComponent<Rigidbody>().isKinematic = true;
			colliders[0].gameObject.GetComponent<Rigidbody>().detectCollisions = true;

			ChangeDodgeball(colliders[0].gameObject);
			grabAnim.SetBool("Reaching", false);

			holding = true;
			grabbing = false;
		}
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
