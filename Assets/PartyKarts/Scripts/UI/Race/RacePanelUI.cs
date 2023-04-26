using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameBalance;

/// <summary>
/// Only visual on UI logic.
/// Drift points information.
/// </summary>
public class RacePanelUI :MonoBehaviour
{
	[SerializeField] int UpdateFrameCount = 3;
	[SerializeField] TextMeshProUGUI TotalRaceTimeText;
	[SerializeField] TextMeshProUGUI CurrentLapTimeText;
	[SerializeField] TextMeshProUGUI BestLapTimeText;
	[SerializeField] TextMeshProUGUI LapText;
	[SerializeField] GameObject WrongDirectionObject;
	[SerializeField] GameObject InGameStatistics;
	[SerializeField] RaceEndGameStatisticsUI EndGameStatistics;

	int CurrentFrame;

	RaceEntity RaceEntity { get { return GameController.RaceEntity as RaceEntity; } }
	GameController GameController { get { return GameController.Instance; } }
	CarStatistic PlayerStatistics { get { return RaceEntity.PlayerStatistics; } }

	private void Start ()
	{
		EndGameStatistics.Init ();
		InGameStatistics.SetActive (true);
		GameController.OnEndGameAction += OnEndGame;
		WrongDirectionObject.SetActive (false);
	}

	private void Update ()
	{
		if (PlayerStatistics == null)
		{
			return;
		}

		WrongDirectionObject.SetActive (PlayerStatistics.IsWrongDirection);

		if (CurrentFrame >= UpdateFrameCount)
		{
			UpdateStatistics ();
			CurrentFrame = 0;
		}
		else
		{
			CurrentFrame++;
		}
	}

	void UpdateStatistics ()
	{
		TotalRaceTimeText.text = PlayerStatistics.TotalRaceTime.ToStringTime ();
		CurrentLapTimeText.text = PlayerStatistics.CurrentLapTime.ToStringTime ();
		BestLapTimeText.text = PlayerStatistics.BestLapTime.ToStringTime ();
		LapText.text = string.Format ("{0}/{1}", PlayerStatistics.CurrentLap, PlayerStatistics.LapsCount);
	}

	void OnEndGame ()
	{
		InGameStatistics.SetActive (false);
	}

	void OnDestroy ()
	{
		if (GameController.Instance != null)
		{
			GameController.OnEndGameAction -= OnEndGame;
		}
	}

}
