using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main mobile control class.
/// Can choose the type of control and display on the PC.
/// </summary>
public class ControlUI :MonoBehaviour
{

	[SerializeField] ControlType ShowControlType;
	[SerializeField] bool ShowOnlyOnMobile = true;

	protected CarController ControlledCar { get { return GameController.PlayerCar; } }

	protected virtual void Awake ()
	{
		GameOptions.OnControlChanged += OnControlChanged;
		OnControlChanged (GameOptions.CurrentControl);
	}

	private void OnEnable ()
	{
		if (ShowOnlyOnMobile)
		{
			gameObject.SetActive (Application.isMobilePlatform);
		}
	}

	private void OnDestroy ()
	{
		GameOptions.OnControlChanged -= OnControlChanged;
	}

	void OnControlChanged (ControlType type)
	{
		gameObject.SetActive (ShowControlType == type);
		if (ShowControlType == type)
		{
			UserControl.CurrentUIControl = this as IUserControl;
		}
	}
}
