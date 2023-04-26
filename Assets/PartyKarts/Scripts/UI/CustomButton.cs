using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// base Button, for added actions "onPointerDown" and "onPointerUp".
/// </summary>
public class CustomButton :Button
{
	[SerializeField] bool SelectedOnStart;
	[SerializeField] bool EnableSounds;
	[SerializeField] AudioClip OnClickSound;
	[SerializeField] AudioClip OnDownSound;
	[SerializeField] AudioClip OnUpSound;

	public event Action<PointerEventData> onPointerDown;
	public event Action<PointerEventData> onPointerUp;

	public bool ButtonIsPressed { get { return base.IsPressed (); } }

	protected override void Awake ()
	{
		if (Application.isMobilePlatform) {
			var newNavigation = navigation;
			newNavigation.mode = Navigation.Mode.None;
			navigation = newNavigation;
		}
		base.Awake ();
	}

	protected override void OnEnable ()
	{
		if (SelectedOnStart && navigation.mode != Navigation.Mode.None)
		{
			StartCoroutine (DoSelect ());
		}
		base.OnEnable ();
	}

	IEnumerator DoSelect ()
	{
		yield return null;
		Select ();
	}

	public override void OnPointerDown (PointerEventData eventData)
	{
		base.OnPointerDown (eventData);
		onPointerDown.SafeInvoke (eventData);

		if (EnableSounds)
		{
			SoundControllerInUI.PlayAudioClip (OnDownSound);
		}
	}

	public override void OnPointerUp (PointerEventData eventData)
	{
		base.OnPointerUp (eventData);
		onPointerUp.SafeInvoke (eventData);

		if (EnableSounds)
		{
			SoundControllerInUI.PlayAudioClip (OnUpSound);
		}
	}

	public override void OnPointerClick (PointerEventData eventData)
	{
		base.OnPointerClick (eventData);

		if (EnableSounds)
		{
			SoundControllerInUI.PlayAudioClip (OnClickSound);
		}
	}
}
