using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	public GameObject startMenu;
	public TMP_InputField usernameField;
	public TMP_InputField ipField;
	public TMP_Dropdown skinDropdown;
	public Image previewImage;
	public Sprite[] previewImages;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Debug.Log("Instance already exists, destroying this....");
			Destroy(this);
		}
	}

	public void ChangePreview()
	{
		previewImage.sprite = previewImages[skinDropdown.value];
	}

	public void ConnectToServer()
	{
		if (usernameField.text != "" && ipField.text != "")
		{
			startMenu.SetActive(false);
			usernameField.interactable = false;
			Client.instance.ip = ipField.text;
			SceneManager.LoadScene("SampleScene");
		}
	}
}
