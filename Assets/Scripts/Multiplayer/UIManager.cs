using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	public GameObject startMenu;
	public TMP_InputField usernameField;

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

	public void ConnectToServer()
	{
		startMenu.SetActive(false);
		usernameField.interactable = false;
		SceneManager.LoadScene("SampleScene");
		Client.instance.ConnectToServer();
	}
}
