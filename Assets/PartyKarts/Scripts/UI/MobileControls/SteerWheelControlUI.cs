using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Mobile input SteerWheel.
/// </summary>
public class SteerWheelControlUI :ControlUI, IUserControl
{

	[SerializeField] CustomButton AccelerationButton;
	[SerializeField] CustomButton DecelerationButton;
	[SerializeField] CustomButton SteerWheelButton;
	[SerializeField] float DeadZone = 10f;
	[SerializeField] float MaxSteerWheelAngle = 270;
	[SerializeField] float SteerWheelToDefaultSpeed = 360;

	float CurrentSteerAngle;
	bool WheelIsPressed = false;
	Vector2 PrevTouchPos;

	bool AccelerationPressed { get { return AccelerationButton.ButtonIsPressed; } }
	bool DecelerationPressed { get { return DecelerationButton.ButtonIsPressed; } }
	public bool ControlInUse { get { return CurrentSteerAngle != 0 || AccelerationButton.ButtonIsPressed || DecelerationButton.ButtonIsPressed; } }
	public float GetHorizontalAxis { get; private set; }

	public float GetVerticalAxis
	{
		get
		{
			if (AccelerationPressed)
			{
				return 1;
			}
			else if (DecelerationPressed)
			{
				return -1;
			}
			return 0;
		}
	}

	protected override void Awake ()
	{
		base.Awake ();
		SteerWheelButton.onPointerDown += OnSteerDown;
		SteerWheelButton.onPointerUp += OnSteerUp;
	}

	private void Update ()
	{
		float targetAnge = 0;
		float carVelocityAngleNormolized = ControlledCar.VelocityAngle / 90;
		bool needGetCarVelocity = ControlledCar.CarDirection >= 0 && ControlledCar.SpeedInHour > 20;
		if (!WheelIsPressed)
		{
			targetAnge = (needGetCarVelocity ? carVelocityAngleNormolized : 0) * MaxSteerWheelAngle;
			CurrentSteerAngle = Mathf.MoveTowards (CurrentSteerAngle, targetAnge, Time.deltaTime * SteerWheelToDefaultSpeed);
		}
		else
		{
			Vector2 pressedPos = SteerWheelButton.transform.position;
			if (Application.isMobilePlatform)
			{
				var leftTouch = Input.GetTouch (0).position;
				for (int i = 1; i < Input.touchCount; i++)
				{
					if (leftTouch.x > Input.GetTouch (i).position.x)
					{
						leftTouch = Input.GetTouch (i).position;
					}
				}
				pressedPos -= leftTouch;
			}
			else
			{
				pressedPos -= (Vector2)Input.mousePosition;
			}
			float angleDelta = Vector2.SignedAngle (PrevTouchPos, pressedPos);
			PrevTouchPos = pressedPos;
			CurrentSteerAngle = Mathf.Clamp (CurrentSteerAngle + angleDelta, -MaxSteerWheelAngle, MaxSteerWheelAngle);
		}
		SteerWheelButton.transform.rotation = Quaternion.AngleAxis (CurrentSteerAngle, Vector3.forward);

		targetAnge = -CurrentSteerAngle / MaxSteerWheelAngle;

		if (needGetCarVelocity)
		{
			targetAnge += carVelocityAngleNormolized;
		}

		targetAnge = targetAnge.Clamp (-1, 1);

		GetHorizontalAxis = targetAnge;
	}

	private void OnSteerDown (PointerEventData eventData)
	{
		WheelIsPressed = true;
		PrevTouchPos = (Vector2)SteerWheelButton.transform.position - eventData.position;
	}

	private void OnSteerUp (PointerEventData eventData)
	{
		WheelIsPressed = false;
	}
}
