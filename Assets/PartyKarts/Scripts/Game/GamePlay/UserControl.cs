using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For user multiplatform control.
/// </summary>
[RequireComponent (typeof (CarController))]
public class UserControl :MonoBehaviour, ICarControl
{
	public float Horizontal { get; private set; }
	public float Vertical { get; private set; }
	public bool Brake { get; private set; }

CarController ControlledCar;

	public static IUserControl CurrentUIControl { get; set; }

	private void Awake ()
	{
		ControlledCar = GetComponent<CarController> ();
	}

	private void Start ()
	{
		//Add AiControl after finish race.
		ControlledCar.PositioningCar.OnFinishRaceAction += () =>
		{
			gameObject.AddComponent<DriftAIControl> ();
			enabled = false;
		};
	}

	void Update ()
	{
		Horizontal = 0;
		Vertical = 0;
		Brake = false;

		if (!GameController.InPause)
		{
			if (CurrentUIControl != null && CurrentUIControl.ControlInUse)
			{
				//Mobile control.
				Horizontal = CurrentUIControl.GetHorizontalAxis;
				Vertical = CurrentUIControl.GetVerticalAxis;
			}
			else
			{
				//Standart input control (Keyboard or gamepad).
				Horizontal = Input.GetAxis ("Horizontal");
				Vertical = Input.GetAxis ("Vertical");
				Brake = Input.GetButton ("Jump");
			}

			if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown (KeyCode.Joystick1Button3))
			{
				ControlledCar.ResetPosition ();
			}
		}

		//Apply control for controlled car.
		ControlledCar.UpdateControls (Horizontal, Vertical, Brake);
	}

	void OnDestroy ()
	{
		CurrentUIControl = null;
	}
}

/// <summary>
/// Interface for different types of control.
/// </summary>
public interface IUserControl
{
	float GetHorizontalAxis { get; }
	float GetVerticalAxis { get; }
	bool ControlInUse { get; }
}
