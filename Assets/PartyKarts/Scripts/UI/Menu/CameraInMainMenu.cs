using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This camera only for main menu. for change blur parameters and set position.
/// </summary>
public class CameraInMainMenu :Singleton<CameraInMainMenu>
{

	[SerializeField] UnityStandardAssets.ImageEffects.Blur CameraBlur;

	[SerializeField] Transform PositionInSelectCarMenu;
	[SerializeField] float ChangePositionSpeed = 10;
	[SerializeField] float ChangeRotationLerpSpeed = 5;
	[SerializeField] float ChangeBlurSpeed = 0.5f;
	[SerializeField] float RotateSensitivityInMenu = 5;

	Coroutine ChangePositionCoroutine;
	Vector3 DefaultPosition;
	Vector3 DefaultRotation;
	bool InCarSelectMenu;
	float CurrentSpeedRotate;

	int DefaultBlurIterations;
	float DefaultBlurSpread;
	bool ClickInEmptyPlace;
	Vector3 PrevMousePos;

	protected override void AwakeSingleton ()
	{
		DefaultPosition = transform.position;
		DefaultRotation = transform.eulerAngles;

		if (Application.isMobilePlatform)
		{
			CameraBlur.enabled = false;
		}
		else
		{
			DefaultBlurIterations = CameraBlur.iterations;
			DefaultBlurSpread = CameraBlur.blurSpread;
		}
	}

	public void SetCarSelectMenu (bool value)
	{
		InCarSelectMenu = value;
		StopAllCoroutines ();
		if (InCarSelectMenu)
		{
			ChangePositionCoroutine = StartCoroutine (ChangePosition (PositionInSelectCarMenu.position, PositionInSelectCarMenu.eulerAngles));
		}
		else
		{
			ChangePositionCoroutine = StartCoroutine (ChangePosition (DefaultPosition, DefaultRotation));
		}

		if (!Application.isMobilePlatform)
		{
			StartCoroutine (SetActiveBlur (!InCarSelectMenu));
		}
	}

	IEnumerator ChangePosition (Vector3 newPos, Vector3 newRot)
	{
		while (transform.position != newPos)
		{
			transform.position = Vector3.MoveTowards (transform.position, newPos, Time.deltaTime * ChangePositionSpeed);
			transform.eulerAngles = Vector3.Lerp (transform.eulerAngles, newRot, Time.deltaTime * ChangeRotationLerpSpeed);
			yield return null;
		}
		ChangePositionCoroutine = null;
	}

	IEnumerator SetActiveBlur (bool active)
	{
		if (active)
		{
			CameraBlur.enabled = true;
		}

		float normolizeTime = active ? 0 : 1;
		float targetNormalize = 1 - normolizeTime;

		while (!Mathf.Approximately (normolizeTime, targetNormalize))
		{
			normolizeTime = Mathf.MoveTowards (normolizeTime, targetNormalize, Time.deltaTime * ChangeBlurSpeed);
			CameraBlur.iterations = Mathf.RoundToInt (DefaultBlurIterations * normolizeTime);
			CameraBlur.blurSpread = DefaultBlurSpread * normolizeTime;
			yield return null;
		}

		if (!active)
		{
			CameraBlur.enabled = false;
		}
	}

	private void Update ()
	{
		if (InCarSelectMenu && ChangePositionCoroutine == null)
		{
			UpdateRotate ();
			transform.rotation *= Quaternion.AngleAxis (CurrentSpeedRotate * Time.deltaTime, Vector3.up);
		}
	}

	void UpdateRotate ()
	{
		if ((!Application.isMobilePlatform || Input.touchCount == 1) && Input.GetMouseButtonDown (0))
		{
			ClickInEmptyPlace = true;
			PrevMousePos = Input.mousePosition;
		}
		else if (Input.touchCount == 0 && !Input.GetMouseButton (0))
		{
			ClickInEmptyPlace = false;
		}

		var mouseDelta = Input.mousePosition - PrevMousePos;
		PrevMousePos = Input.mousePosition;
		CurrentSpeedRotate = Mathf.MoveTowards (CurrentSpeedRotate, ClickInEmptyPlace ? mouseDelta.x * 5 : 0, Time.deltaTime * RotateSensitivityInMenu * (ClickInEmptyPlace ? 5 : 1));
	}

	private void OnDisable ()
	{
		StopAllCoroutines ();
		ChangePositionCoroutine = null;
	}

}
