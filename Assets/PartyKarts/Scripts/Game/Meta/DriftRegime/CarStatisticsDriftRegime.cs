using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBalance;

/// <summary>
/// Drift score points logic for one car.
/// </summary>
public class CarStatisticsDriftRegime :CarStatistic
{

	public float TotalScore { get; private set; }
	public float BestScore { get; private set; }
	public float CurrentScore { get; private set; }
	public int CurrentMultiplier { get; private set; }
	public bool InDrift { get; private set; }
	public float DriftTimeProcent { get; private set; }
	public float MultiplierProcent { get { return CurrentMultiplierScore / Settings.MinScoreForIncMultiplier; } }
	public bool NeedCalcelateDriftPoint { get { return !IsWrongDirection && PositioningCar.LastPointIsCorrect; } }

	DriftRegimeSettings Settings { get { return B.GameSettings.DriftRegimeSettings; } }
	float AbsCarVelocityAngle { get { return Mathf.Abs (Car.VelocityAngle); } }

	public System.Action TotalScoreChanged;

	int DriftDirection;
	float CurrentMultiplierScore;
	float Timer = 0;

	public CarStatisticsDriftRegime (CarController car, string playerName, bool isLocalCar = true) :
		base (car, playerName, isLocalCar)
	{
		car.ResetCarAction += ResetCurrentStatistics;
		CurrentMultiplier = 1;
		InDrift = false;
	}

	public override void FixedUpdate ()
	{
		if (!RaceIsStarted || IsFinished || GameController.RaceIsEnded) { return; }

		base.FixedUpdate ();

		if (!IsLocalCar || PositioningCar.LastCorrectProgressDistance == 0)
		{
			return;
		}

		if (InDrift)
		{
			InDriftUpdate ();
		}
		else if (NeedCalcelateDriftPoint && AbsCarVelocityAngle > Settings.MinAngle && Car.SpeedInHour > Settings.MinSpeed)
		{
			if (Timer > Settings.WaitDriftTime)
			{
				//Enter in drift.
				Timer = 0;
				InDrift = true;
				DriftDirection = Car.VelocityAngle < 0 ? -1 : 1;
			}
			else
			{
				//Wait enter in drift.
				Timer += Time.fixedDeltaTime;
				DriftTimeProcent = Timer / Settings.WaitDriftTime;
			}
		}
		else
		{
			Timer = 0;
			DriftTimeProcent = 0;
			CurrentMultiplierScore = 0;
		}
	}

	/// <summary>
	/// Drift score points update logic.
	/// </summary>
	void InDriftUpdate ()
	{

		if (!NeedCalcelateDriftPoint || AbsCarVelocityAngle < Settings.MinAngle || AbsCarVelocityAngle > Settings.MaxAngle || Car.SpeedInHour < Settings.MinSpeed)
		{
			//if car is not drift.
			if (Timer > Settings.WaitEndDriftTime)
			{
				//Exit from drift.
				TotalScore += CurrentScore;
				TotalScoreChanged.SafeInvoke ();
				if (CurrentScore > BestScore)
				{
					BestScore = CurrentScore;
				}
				ResetCurrentStatistics ();
			}
			else
			{
				//Wait exit time.
				Timer += Time.fixedDeltaTime;
				DriftTimeProcent = 1 - Timer / Settings.WaitEndDriftTime;
			}

			//Reset current multiplier score.
			CurrentMultiplierScore = 0;
			return;
		}

		Timer = 0;

		//Current drift score.
		DriftTimeProcent = 1;
		float driftDelta = (AbsCarVelocityAngle / Settings.MaxAngle) * (Car.CurrentSpeed * Time.fixedDeltaTime) * Settings.ScorePerMeter;
		CurrentScore += (driftDelta * CurrentMultiplier);

		//Multiplier.
		CurrentMultiplierScore += driftDelta;
		if (CurrentMultiplierScore > Settings.MinScoreForIncMultiplier)
		{
			CurrentMultiplier = Mathf.Clamp (CurrentMultiplier + 1, 1, Settings.MaxMultiplier);
			CurrentMultiplierScore = 0;
		}
		int currentDriftDirection = Car.VelocityAngle < 0 ? -1 : 1;
		if (DriftDirection != currentDriftDirection)
		{
			DriftDirection = currentDriftDirection;
			CurrentMultiplierScore = 0;
		}
	}

	/// <summary>
	/// Update score in multiplayer
	/// </summary>
	public void UpdateScore (float totalScore)
	{
		if (totalScore != TotalScore)
		{
			TotalScore = totalScore;
			TotalScoreChanged.SafeInvoke ();
		}
	}
	/// <summary>
	/// End game and save current drift score.
	/// </summary>
	protected override void OnFinishRace ()
	{
		//Exit from drift.
		if (InDrift)
		{
			TotalScore += CurrentScore;
			TotalScoreChanged.SafeInvoke ();
			if (CurrentScore > BestScore)
			{
				BestScore = CurrentScore;
			}
			ResetCurrentStatistics ();
		}
	}

	/// <summary>
	/// Force calculate the total score for bots.
	/// </summary>
	protected override void OnForceFinish ()
	{
		float currentDistance = PositioningCar.ProgressDistance;
		float fullDistance = PositioningSystem.PositioningAndAiPath.Length * PositioningSystem.LapsCount;

		var totalscoreMultiplier = (1 - currentDistance / fullDistance);

		TotalScore += TotalScore * totalscoreMultiplier;
		TotalScoreChanged.SafeInvoke ();
	}

	/// <summary>
	/// Reset drift if car collision with ResetDriftCollision tag
	/// </summary>
	protected override void CollisionCar (CarController car, Collision collision)
	{
		if (collision == null || collision.gameObject.tag == C.ResetDriftTag)
		{
			ResetCurrentStatistics ();
		}
	}

	/// <summary>
	/// Reset all temp properties.
	/// </summary>
	void ResetCurrentStatistics ()
	{
		Timer = 0;
		InDrift = false;
		CurrentMultiplier = 1;
		CurrentScore = 0;
		CurrentMultiplierScore = 0;
		DriftTimeProcent = 0;
	}
}
