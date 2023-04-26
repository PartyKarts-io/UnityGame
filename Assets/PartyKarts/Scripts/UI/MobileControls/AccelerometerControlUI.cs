using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mobile input accelerometer.
/// </summary>
public class AccelerometerControlUI :ControlUI, IUserControl
{

	[SerializeField] CustomButton AccelerationButton;
	[SerializeField] CustomButton DecelerationButton;
	[SerializeField] GameObject AccelerometerNotSupportObject;
	[SerializeField] float DeadZone = 5f;
	[SerializeField] float MaxAngle = 45f;
	[SerializeField] float AccelerometerLerpSpeed = 500;

	bool AccelerationPressed { get { return AccelerationButton.ButtonIsPressed; } }
	bool DecelerationPressed { get { return DecelerationButton.ButtonIsPressed; } }
	public bool ControlInUse { get { return SystemInfo.supportsAccelerometer; } }
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
		AccelerometerNotSupportObject.SetActive (!SystemInfo.supportsAccelerometer);
	}

	private void Update ()
	{
		//The tilt of the phone sets the velocity vector to the desired angle.
		if (SystemInfo.supportsAccelerometer)
		{
			float axisX = Input.acceleration.x * 90;

			float targetAnge = 0;
			if (axisX > DeadZone || axisX < -DeadZone)
			{
				targetAnge = Mathf.Clamp ((axisX + (axisX > 0 ? -DeadZone : DeadZone)) / (MaxAngle), -1, 1) * 90;
			}

			if (ControlledCar.CarDirection >= 0 && ControlledCar.SpeedInHour > 20)
			{
				targetAnge += ControlledCar.VelocityAngle;
			}

			targetAnge = targetAnge.Clamp (-90, 90) / 90;

			GetHorizontalAxis = Mathf.Lerp (GetHorizontalAxis, targetAnge, Time.deltaTime * AccelerometerLerpSpeed);
		}
	}
}
