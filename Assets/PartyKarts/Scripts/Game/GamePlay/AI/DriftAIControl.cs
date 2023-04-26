using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// AI for drift regime.
/// </summary>
public class DriftAIControl :AIControlBase
{
	GameBalance.RegimeSettings.AiConfig AiConfig;
	float MaxSpeed { get { return AiConfig.MaxSpeed; } }
	float MinSpeed { get { return AiConfig.MinSpeed; } }
	private float AccelSensitivity { get { return AiConfig.AccelSensitivity; } }
	private float BrakeSensitivity { get { return AiConfig.BrakeSensitivity; } }
	private float ReverceWaitTime { get { return AiConfig.ReverceWaitTime; } }
	private float ReverceTime { get { return AiConfig.ReverceTime; } }
	private float BetweenReverceTimeForReset { get { return AiConfig.BetweenReverceTimeForReset; } }

	float LookAheadForTargetOffset1 { get { return AiConfig.OffsetToFirstTargetPoint; } }
	float LookAheadForTargetFactor1 { get { return AiConfig.SpeedFactorToFirstTargetPoint; } }
	float LookAheadForTargetOffset2 { get { return AiConfig.OffsetToSecondTargetPoint; } } 
	float LookAheadForTargetFactor2 { get { return AiConfig.SpeedFactorToSecondTargetPoint; } }
	float LookAngleSppedFactor { get { return AiConfig.LookAngleSppedFactor; } }
	float SetSteerAngleSensitivity { get { return AiConfig.SetSteerAngleSensitivity; } }

	private CarController Car;
	private bool Reverse;
	private float ReverseTimer = 0;
	private float PrevSpeed = 0;
	private float LastReverceTime;

	public float TargetDist { get; private set; }
	public Vector3 TargetPoint { get; private set; }
	public WaypointCircuit.RoutePoint TargetPoint1 { get; private set; }
	public WaypointCircuit.RoutePoint TargetPoint2 { get; private set; }

	private PositioningCar PositioningCar { get { return Car.PositioningCar; } }
	private Rigidbody RB { get { return Car.RB; } }
	private WaypointCircuit Circuit { get { return PositioningSystem.PositioningAndAiPath; } }

	private void Start ()
	{
		Car = GetComponent<CarController> ();
		AiConfig = WorldLoading.RegimeSettings.GetAiConfig;
	}

	private void FixedUpdate ()
	{
		if (!GameController.RaceIsStarted)
			return;
		if (Reverse)
		{
			ReverseMove ();
		}
		else
		{
			ForwardMove ();
		}
	}

	/// <summary>
	/// All behavior of bots is defined in this method.
	/// The target point is the point between two points calculated from the parameters from AIConfig.
	/// The acceleration is calculated from the angle of the car to the target point.
	/// </summary>
	private void ForwardMove ()
	{
		var angleFactor = (Car.VelocityAngle.Abs () / 55).Clamp ();
		TargetPoint1 = Circuit.GetRoutePoint (PositioningCar.ProgressDistance + LookAheadForTargetOffset1 + (LookAheadForTargetFactor1 * Car.CurrentSpeed));
		TargetPoint2 = Circuit.GetRoutePoint (PositioningCar.ProgressDistance + LookAheadForTargetOffset2 + (LookAheadForTargetFactor2 * Car.CurrentSpeed));
		TargetPoint = (TargetPoint1.position + TargetPoint2.position) * 0.5f;

		var angleToTargetPoint = Vector3.SignedAngle (Vector3.forward,
													transform.InverseTransformPoint (TargetPoint),
													Vector3.up).AbsClamp (0, LookAngleSppedFactor);

		float desiredSpeed = 0;

		if (HasLimit)
		{
			desiredSpeed = SpeedLimit;
		}
		else
		{
			desiredSpeed = (1 - (angleToTargetPoint / LookAngleSppedFactor)).AbsClamp ();
			desiredSpeed = desiredSpeed * (MaxSpeed - MinSpeed) + MinSpeed;
			desiredSpeed = desiredSpeed.Clamp (MinSpeed, MaxSpeed);
		}

		// Acceleration and brake logic
		float accelBrakeSensitivity = (desiredSpeed < Car.SpeedInHour) ? BrakeSensitivity : AccelSensitivity;

		if (NeedBrake)
		{
			Vertical = ((desiredSpeed - Car.SpeedInHour) * accelBrakeSensitivity).Clamp (-1, 1);
		} 
		else
		{
			Vertical = ((desiredSpeed - Car.SpeedInHour) * accelBrakeSensitivity).Clamp (0, 1);
		}

		//Steer angle logic
		Vector3 localTarget = transform.InverseTransformPoint (TargetPoint);
		float targetAngle = Mathf.Atan2 (localTarget.x, localTarget.z) * Mathf.Rad2Deg;
		Horizontal = Mathf.MoveTowards (Horizontal, (targetAngle / Car.GetCarConfig.MaxSteerAngle).Clamp (-1, 1), Time.deltaTime * SetSteerAngleSensitivity);

		//Apply controls to a controlled car
		Car.UpdateControls (Horizontal, Vertical, false);

		//Reverse logic
		var deltaSpeed = Mathf.Abs (Car.SpeedInHour - PrevSpeed);
		if (Vertical > 0.1f && deltaSpeed < 1 && Car.SpeedInHour < 10)
		{
			if (ReverseTimer < ReverceWaitTime)
			{
				ReverseTimer += Time.fixedDeltaTime;
			}
			else if (Time.time - LastReverceTime <= BetweenReverceTimeForReset)
			{
				Horizontal = 0;
				Vertical = 0;
				Car.ResetPosition ();
				ReverseTimer = 0;
			}
			else
			{
				Horizontal = -Horizontal;
				Vertical = -Vertical;
				ReverseTimer = 0;
				Reverse = true;
			}
		}
		else
		{
			ReverseTimer = 0;
		}
	}

	private void ReverseMove ()
	{
		var deltaSpeed = Mathf.Abs (Car.SpeedInHour - PrevSpeed);
		if (ReverseTimer < ReverceTime)
		{
			ReverseTimer += Time.fixedDeltaTime;
			Car.UpdateControls (Horizontal, Vertical, false);
		}
		else
		{
			LastReverceTime = Time.time;
			ReverseTimer = 0;
			Reverse = false;
		}
	}

	private void OnDrawGizmosSelected ()
	{
		if (Application.isPlaying && this.enabled)
		{

			Gizmos.color = Color.green;
			Gizmos.DrawLine (transform.position, TargetPoint);
			Gizmos.DrawWireSphere (TargetPoint, 0.5f);

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (TargetPoint1.position, 0.5f);
			Gizmos.DrawWireSphere (TargetPoint2.position, 0.5f);
		}
	}
}

