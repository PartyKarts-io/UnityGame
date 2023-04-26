using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DriftEndGameStatisticsUI :MonoBehaviour
{

	[SerializeField] Animator EndGameStatisticHolder;
	[SerializeField] Button RestartGameButton;
	[SerializeField] Button ExitToMainMenuButton;
	[SerializeField] TextMeshProUGUI MoneyCaptionText;

	[SerializeField] TextMeshProUGUI EndTotalScoreText;
	[SerializeField] TextMeshProUGUI PrevTotalScoreText;

	[SerializeField] TextMeshProUGUI EndBestScoreText;
	[SerializeField] TextMeshProUGUI PrevBestScoreText;

	[SerializeField] TextMeshProUGUI EndTotalTimeText;
	[SerializeField] TextMeshProUGUI PrevRaceTimeText;

	[SerializeField] TextMeshProUGUI TotalDistanceText;
	[SerializeField] TextMeshProUGUI PrevTotalDistanceText;

	[SerializeField] Color ResultWorseColor;
	[SerializeField] Color ResultBetterColor;

	[SerializeField] List<DriftRatingPlayerUI> Players = new List<DriftRatingPlayerUI>();


	DriftRaceEntity DriftRaceEntity;
	GameController GameController { get { return GameController.Instance; } }
	CarStatisticsDriftRegime PlayerStatistics { get { return DriftRaceEntity.PlayerDriftStatistics; } }

	public void Init ()
	{
		DriftRaceEntity = GameController.RaceEntity as DriftRaceEntity;
		if (DriftRaceEntity == null)
		{
			Debug.LogError ("[DriftPanelUI] RaceEntity is not DriftRaceEntity");
			enabled = false;
		}

		if (WorldLoading.IsMultiplayer)
		{
			RestartGameButton.interactable = false;
		}
		else
		{
			RestartGameButton.onClick.AddListener (RestartGame);
		}
		
		ExitToMainMenuButton.onClick.AddListener (Exit);
		GameController.OnEndGameAction += OnEndGame;

		gameObject.SetActive (false);
	}

	IEnumerator ShowEndGameCoroutine ()
	{
		while (PlayerStatistics.InDrift || GameController.AllCars.Any(c => c != null && !c.PositioningCar.IsFinished))
		{
			yield return null;
		}

		Players.ForEach (p => p.SetActive (false));

		// Show rating of players.
		for (int i = 0; i < DriftRaceEntity.DriftStatistics.Count; i++)
		{
			if (i < Players.Count)
			{
				var carStat = DriftRaceEntity.DriftStatistics[i];
				Players[i].SetActive (true);
				var playerName = string.Format ("{0}: {1}", i + 1, carStat.PlayerName);
				Players[i].UpdateData (playerName, carStat.TotalScore.ToInt ());
			} 
		}

		int money = 0;

		if (WorldLoading.HasLoadingParams) {
			var playerPosition = DriftRaceEntity.DriftStatistics.IndexOf(PlayerStatistics);
			var positionsCount = DriftRaceEntity.DriftStatistics.Count;
			money = (((float)(positionsCount - playerPosition) / (float)positionsCount) * WorldLoading.LoadingTrack.MoneyForFirstPlace).ToInt ();

			money += (PlayerStatistics.TotalScore * B.GameSettings.DriftRegimeSettings.MoneyForDriftMultiplier).ToInt(); 

			money = Mathf.RoundToInt (money * 0.01f) * 100; //* 0.01f and * 100, Rounding money to hundreds

			var playerStatistics = DriftRaceEntity.PlayerDriftStatistics;

			//If the first place is taken, then mark the track as completed
			if (DriftRaceEntity.DriftStatistics.All(s => s == playerStatistics || s.TotalScore < playerStatistics.TotalScore))
			{
				PlayerProfile.SetTrackAsComplited (WorldLoading.LoadingTrack);
			}
		}

		//Inc money
		PlayerProfile.Money += money;

		var carCaption = WorldLoading.PlayerCar ? WorldLoading.PlayerCar.CarCaption : GameController.PlayerCar.gameObject.name;

		var prevTotalScore = PlayerProfile.TotalScore;
		var prevBestScore = PlayerProfile.BestScore;
		var prevRaceTime = PlayerProfile.RaceTime;
		var prevDistance = PlayerProfile.TotalDistance;

		var bestScoreDiff = Mathf.RoundToInt (PlayerStatistics.BestScore - prevBestScore);

		//Total score inc
		PrevTotalScoreText.text = string.Format ("{0}\n + {1}", prevTotalScore, PlayerStatistics.TotalScore.ToString("########0"));
		PlayerProfile.TotalScore += Mathf.RoundToInt (PlayerStatistics.TotalScore);
		PrevTotalScoreText.color = ResultBetterColor;

		//Get prev total score best result and save current if better.
		PrevBestScoreText.text = string.Format ("{0}\n{1}{2}",
													prevBestScore,
													bestScoreDiff > 0 ? "+" : "",
													bestScoreDiff
		);

		if (PlayerProfile.BestScore < PlayerStatistics.BestScore)
		{
			PlayerProfile.BestScore = Mathf.RoundToInt (PlayerStatistics.BestScore);
			PlayerProfile.BestScoreCar = carCaption;
			PrevBestScoreText.color = ResultBetterColor;
		}
		else
		{
			PrevBestScoreText.color = ResultWorseColor;
		}

		//Race time inc
		PrevRaceTimeText.text = string.Format ("{0}\n+{1}", prevRaceTime.ToStringTime (), PlayerStatistics.TotalRaceTime.ToStringTime ());

		PlayerProfile.RaceTime += Mathf.RoundToInt (PlayerStatistics.TotalRaceTime);
		PrevRaceTimeText.color = ResultBetterColor;

		//Get prev total score best result and save current if better.
		var distance = (int)PositioningSystem.PositioningAndAiPath.Length * WorldLoading.LapsCount;
		PrevTotalDistanceText.text = string.Format ("{0}\n+{1}", prevDistance.ToString (), distance.ToString (""));

		PlayerProfile.TotalDistance += distance;
		PrevTotalDistanceText.color = ResultBetterColor;

		//Show on screen current statistics
		MoneyCaptionText.text = string.Format("+${0}", money);
		EndTotalScoreText.text = PlayerStatistics.TotalScore.ToString ("########0");
		EndBestScoreText.text = PlayerStatistics.BestScore.ToString ("########0");
		EndTotalTimeText.text = PlayerStatistics.TotalRaceTime.ToStringTime ();
		TotalDistanceText.text = distance.ToString ();
	}

	void OnEndGame ()
	{
		gameObject.SetActive (true);
		ExitToMainMenuButton.Select ();
		EndGameStatisticHolder.SetTrigger (C.ShowTrigger);
		StartCoroutine (ShowEndGameCoroutine ());
	}

	void RestartGame ()
	{
		LoadingScreenUI.ReloadCurrentScene ();
	}

	void Exit ()
	{
		LoadingScreenUI.LoadScene (B.GameSettings.MainMenuSceneName);
	}

	void OnDestroy ()
	{
		if (GameController.Instance != null)
		{
			GameController.OnEndGameAction -= OnEndGame;
		}
	}
}
