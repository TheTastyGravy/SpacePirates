using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCameraForCanvas : MonoBehaviour
{
	public Canvas canvas;

	void Awake()
	{
		Invoke(nameof(Func), 0.1f);
	}

	private void Func()
	{
		canvas.worldCamera = Camera.main;
	}
}
