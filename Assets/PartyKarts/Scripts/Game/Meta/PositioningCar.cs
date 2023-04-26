using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PositioningCar for calculate wrong direction.
/// </summary>
public class PositioningCar :MonoBehaviour
{
	public Action OnStartAction;                                //OnStartAction Invoked when crossing the finish line for the first time. TODO Add delay on start.
	public Action<int> OnFinishLapAction;                       //OnFinishLapAction Invoked every lap at the finish.
	public Action OnFinishRaceAction;                           //OnFinishLapAction Invoked at the finish last lap.
	public Action OnForceFinishRaceAction;                            //OnForceRaceAction Invoked for force finish.

	public bool IsFinished { get; private set; }
	public int CurrentLap { get; private set; }
	public bool IsWrongDirection { get; private set; }

	public WaypointCircuit Circuit { get { return PositioningSystem.PositioningAndAiPath; } }
	public bool LastPointIsCorrect { get { return ProgressDistance >= LastCorrectProgressDistance; } }
	public WaypointCircuit.RoutePoint LastCorrectPosition { get { return Circuit.GetRoutePoint (LastCorrectProgressDistance); } }
	public WaypointCircuit.RoutePoint ProgressPoint { get; private set; }
	public float ProgressDistance { get; private set; }
	public float LastCorrectProgressDistance { get; private set; }
	public float LapLength { get { return Circuit.Length; } }

	//Multiplayer properties.
	public bool IsLocalCar { get { return !WorldLoading.IsMultiplayer || MultiplayerCar == null || MultiplayerCar.IsMine; } }
	MultiplayerCarController MultiplayerCar;

	private CarController CarController;
	float DistanceToProgressPoint;

	void Awake ()
	{
		//For load the prefab in the select car menu.
		if (!GameController.InGameScene)
		{
			this.enabled = false;
			return;
		}

		IsWrongDirection = false;
		CurrentLap = 1;
		CarController = GetComponent<CarController> ();
	}

	void Start ()
	{
		ProgressPoint = Circuit.GetRoutePoint (0);
		MultiplayerCar = GetComponent<MultiplayerCarController> ();
		GameController.Instance.OnEndGameAction += ForceFinish;
	}

	void FixedUpdate ()
	{
		if (!GameController.InPause)
		{
			UpdateProgress ();
		}
	}

	/// <summary>
	/// The script from the "Standard Assets" is taken and converted.
	/// </summary>
	void UpdateProgress ()
	{
		
		Vector3 progressDelta = ProgressPoint.position - transform.position;
		float dotProgressDelta = Vector3.Dot (progressDelta, ProgressPoint.direction);

		if (dotProgressDelta < 0)
		{
			//Forward move direction logic
			while (dotProgressDelta < 0)
			{
				ProgressDistance += Mathf.Max (0.5f, CarController.CurrentSpeed * Time.fixedDeltaTime);
				ProgressPoint = Circuit.GetRoutePoint (ProgressDistance);
				progressDelta = ProgressPoint.position - transform.position;
				dotProgressDelta = Vector3.Dot (progressDelta, ProgressPoint.direction);
			}

			DistanceToProgressPoint = (ProgressPoint.position - transform.position).magnitude;

			if (ProgressDistance > LastCorrectProgressDistance)
			{
				LastCorrectProgressDistance = ProgressDistance;
			}

			IsWrongDirection = false;
			if (ProgressDistance > LapLength * CurrentLap)
			{
				CrossedFinishLine ();
			}
		}
		else if (ProgressDistance > 0 && ((DistanceToProgressPoint + 10) * (DistanceToProgressPoint + 10)) < progressDelta.sqrMagnitude)
		{
			//Wrog move direction logic
			dotProgressDelta = Vector3.Dot (progressDelta, -ProgressPoint.direction);

			if (dotProgressDelta < 0f)
			{
				ProgressDistance -= progressDelta.magnitude * 0.5f;
				ProgressPoint = Circuit.GetRoutePoint (ProgressDistance);
				IsWrongDirection = true;

				DistanceToProgressPoint = (ProgressPoint.position - transform.position).magnitude;
			}
		}
	}

	//Finish logic
	private void CrossedFinishLine ()
	{
		if (IsFinished) { return; }

		OnFinishLapAction.SafeInvoke (CurrentLap);

		if (CurrentLap + 1 > PositioningSystem.LapsCount)
		{
			FinishRace ();
		}
		else
		{
			CurrentLap++;
		}

		CurrentLap = Mathf.Clamp (CurrentLap, 1, PositioningSystem.LapsCount);

	}

	public void ForceFinish ()
	{
		if (!IsFinished && IsLocalCar)
		{
			OnForceFinishRaceAction.SafeInvoke ();
			FinishRace ();
		}
		else
		{
			IsFinished = true;
		}
	}

	void FinishRace ()
	{
		if (!IsLocalCar || IsFinished) 
		{ 
			return; 
		}

		IsFinished = true;
		OnFinishRaceAction.SafeInvoke ();
		if (CarController == GameController.PlayerCar)
		{
			if (WorldLoading.IsMultiplayer)
			{
				GameController.SendFinishEvent ();
			}
			else
			{
				GameController.Instance.OnEndGameAction.SafeInvoke ();
			}
		}
	}

	private void OnDrawGizmosSelected ()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (Circuit.GetRoutePosition (ProgressDistance), 1);
		}
	}
}
