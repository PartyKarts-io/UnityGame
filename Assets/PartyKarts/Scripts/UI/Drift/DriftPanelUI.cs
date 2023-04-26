using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
/// Only visual on UI logic.
/// Drift points information.
/// </summary>
public class DriftPanelUI :MonoBehaviour
{
	[SerializeField] int UpdateFrameCount = 3;
	[SerializeField] TextMeshProUGUI TotalRaceTimeText;
	[SerializeField] TextMeshProUGUI LapText;
	[SerializeField] TextMeshProUGUI TotalScoreText;
	[SerializeField] TextMeshProUGUI BestScoreText;
	[SerializeField] TextMeshProUGUI CurrentScoreText;
	[SerializeField] TextMeshProUGUI MultiplierScoreText;
	[SerializeField] GameObject WrongDirectionObject;
	[SerializeField] Image DriftTimeImage;
	[SerializeField] Image MultiplierTimeImage;
	[SerializeField] GameObject InGameStatistics;
	[SerializeField] DriftEndGameStatisticsUI EndGameStatistics;

	int CurrentFrame;

	DriftRaceEntity DriftRaceEntity;
	GameController GameController { get { return GameController.Instance; } }
	CarStatisticsDriftRegime PlayerStatistics { get { return DriftRaceEntity.PlayerDriftStatistics; } }

	private void Start ()
	{
		DriftRaceEntity = GameController.RaceEntity as DriftRaceEntity;
		if (DriftRaceEntity == null)
		{
			Debug.LogError ("[DriftPanelUI] RaceEntity is not DriftRaceEntity");
			enabled = false;
		}

		EndGameStatistics.Init ();
		InGameStatistics.SetActive (true);
		GameController.OnEndGameAction += OnEndGame;
	}

	private void Update ()
	{
		if (PlayerStatistics == null)
		{
			return;
		}

		WrongDirectionObject.SetActive (PlayerStatistics.IsWrongDirection);
		DriftTimeImage.fillAmount = PlayerStatistics.DriftTimeProcent;
		if (PlayerStatistics.CurrentMultiplier == B.GameSettings.DriftRegimeSettings.MaxMultiplier)
		{
			MultiplierTimeImage.fillAmount = 1;
		}
		else
		{
			MultiplierTimeImage.fillAmount = PlayerStatistics.MultiplierProcent;
		}

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
		LapText.text = string.Format ("{0}/{1}", PlayerStatistics.CurrentLap, PlayerStatistics.LapsCount);
		TotalScoreText.text = PlayerStatistics.TotalScore.ToString ("########0");
		BestScoreText.text = PlayerStatistics.BestScore.ToString ("########0");
		CurrentScoreText.text = PlayerStatistics.CurrentScore.ToString ("########0");
		MultiplierScoreText.SetActive (!Mathf.Approximately (0, PlayerStatistics.CurrentScore));
		MultiplierScoreText.text = PlayerStatistics.CurrentMultiplier.ToString ("x#");
		TotalRaceTimeText.text = PlayerStatistics.TotalRaceTime.ToStringTime ();
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
