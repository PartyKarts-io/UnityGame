using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Window class to control the game windows.
/// </summary>
public abstract class Window :MonoBehaviour
{

	public Action OnEnableAction;
	public Action OnDisableAction;
	public Action CustomBackAction;

	private void OnEnable ()
	{
		OnEnableAction.SafeInvoke ();
	}

	private void OnDisable ()
	{
		OnDisableAction.SafeInvoke ();
	}

	protected virtual void Awake () { }
	public abstract void Open ();
	public abstract void Close ();

}
