using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Window with animation and play sounds when open or close. 
/// </summary>
public class WindowWithShowHideAnimators :Window
{

	[SerializeField] float DellayFirstShow = 0.3f;
	[SerializeField] float DellayToDesableGO = 0.3f;
	[SerializeField] AudioClip ShowClip;
	[SerializeField] AudioClip HideClip;

	Coroutine ShowHideCoroutine;

	bool IsInitialized;

	Animator _Animator;
	Animator Animator
	{
		get
		{
			if (_Animator == null)
			{
				_Animator = GetComponent<Animator> ();
			}
			return _Animator;
		}
	}

	public override void Open ()
	{
		gameObject.SetActive (true);
		StopCoroutine ();
		ShowHideCoroutine = StartCoroutine (DoShowAnimation ());
	}

	IEnumerator DoShowAnimation ()
	{

		if (!IsInitialized)
		{
			yield return new WaitForSecondsRealtime (DellayFirstShow);
			IsInitialized = true;
		}

		if (Animator)
		{
			Animator.SetTrigger (C.ShowTrigger);
		}

		SoundControllerInUI.PlayAudioClip (ShowClip);

		ShowHideCoroutine = null;
	}

	public override void Close ()
	{
		StopCoroutine ();
		ShowHideCoroutine = StartCoroutine (DoHideAnimation ());
	}

	IEnumerator DoHideAnimation ()
	{
		SoundControllerInUI.PlayAudioClip (HideClip);

		if (Animator)
		{
			Animator.SetTrigger (C.HideTrigger);
		}

		yield return new WaitForSecondsRealtime (DellayToDesableGO);

		ShowHideCoroutine = null;
		gameObject.SetActive (false);

		yield break;
	}

	void StopCoroutine ()
	{
		if (ShowHideCoroutine != null)
		{
			StopCoroutine (ShowHideCoroutine);
			ShowHideCoroutine = null;
		}
	}
}
