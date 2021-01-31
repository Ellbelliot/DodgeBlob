using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	private Transform cam;

	private void LateUpdate()
	{
		try
		{
			cam = Camera.main.transform;
			if (cam)
			{
				transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
			}
		}
		catch
		{
			Debug.Log("The player hasn't spawn yet. That's like, super weird!");
		}
	}
}
