using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base CarStatistic class, calculate basic states. TODO Add new regimes.
/// </summary>
public class CarStatistic
{
	public string PlayerName { get; private set; }
	public bool IsWrongDirection { get { return PositioningCar.IsWrongDirection; } }
	public int CurrentLap { get { return Mathf.Clamp (PositioningCar.CurrentLap, 0, int.MaxValue); } }
	public int LapsCount { get { return PositioningSystem.LapsCount; } }
	public bool IsFinished { get { return PositioningCar.IsFinished; } }

	public float BestLapTime { get; private set; }
	public float CurrentLapTime { get; private set; }
	public float TotalRaceTime { get; private set; }

	public CarController Car { get; private set; }
	public PositioningCar PositioningCar { get; private set; }

	public bool IsLocalCar { get; private set; }

	protected bool RaceIsStarted { get { return GameController.RaceIsStarted; } }

	float StartRaceTime;
	float StartCurrentLapTime;
	bool StopUpdateTime;

	public CarStatistic (CarController car, string playerName, bool isLocalCar = true)
	{
		PlayerName = playerName;
		Car = car;
		PositioningCar = Car.GetComponent<PositioningCar> ();
		Car.CollisionAction += CollisionCar;
		PositioningCar.OnFinishLapAction += OnFinishLap;
		PositioningCar.OnFinishRaceAction += OnFinishRace;
		PositioningCar.OnForceFinishRaceAction += OnForceFinish;

		GameController.Instance.FixedUpdateAction += FixedUpdate;
		GameController.Instance.OnStartRaceAction += OnStartRace;

		IsLocalCar = isLocalCar;
	}

	protected virtual void CollisionCar (CarController car, Collision collision) { }

	public virtual void FixedUpdate ()
	{
		if (!RaceIsStarted || IsFinished || StopUpdateTime) { return; }
		CurrentLapTime = Time.time - StartCurrentLapTime;
		if (PositioningCar.IsLocalCar)
		{
			TotalRaceTime = Time.time - StartRaceTime;
		}
	}

	/// <summary>
	/// To sync race time in multiplayer.
	/// </summary>
	public void SetRaceTime (float raceTime)
	{
		TotalRaceTime = raceTime;
	}

	protected virtual void OnStartRace ()
	{
		StartRaceTime = Time.time;
		StartCurrentLapTime = Time.time;
	}

	void OnFinishLap (int finishedLap)
	{
		if (finishedLap == 0)
		{ return; }

		if (CurrentLapTime < BestLapTime || Mathf.Approximately (BestLapTime, 0))
		{
			BestLapTime = CurrentLapTime;
		}

		StartCurrentLapTime = Time.time;
	}

	protected virtual void OnFinishRace () {}

	protected virtual void OnForceFinish ()
	{
		float currentDistance = PositioningCar.ProgressDistance;
		float fullDistance = PositioningSystem.PositioningAndAiPath.Length * PositioningSystem.LapsCount;

		var totalTimeMultiplier = (1 - currentDistance / fullDistance);

		TotalRaceTime += TotalRaceTime * totalTimeMultiplier;
	}

	void OnDestroy ()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.OnStartRaceAction -= OnStartRace;
		}
	}
}
