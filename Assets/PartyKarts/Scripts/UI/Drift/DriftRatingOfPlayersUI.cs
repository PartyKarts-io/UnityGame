using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftRatingOfPlayersUI :MonoBehaviour
{

	[SerializeField] DriftRatingPlayerUI PlayerUIRef;
	[SerializeField] Vector2 FirstPanelPosition;
	[SerializeField] Vector2 OffsetToNextPosition;
	[SerializeField] float MovePanelSpeed = 400;
	[SerializeField] int MaxPanels = 6;

	DriftRaceEntity DriftRaceEntity;
	GameController GameController { get { return GameController.Instance; } }
	Dictionary<CarStatisticsDriftRegime, DriftRatingPlayerUI> PanelCarStatisticsDict = new Dictionary<CarStatisticsDriftRegime, DriftRatingPlayerUI>();
	List<CarStatisticsDriftRegime> AllCars { get { return DriftRaceEntity.DriftStatistics; } }

	void Start ()
	{
		DriftRaceEntity = GameController.RaceEntity as DriftRaceEntity;

		if (DriftRaceEntity == null)
		{
			Debug.LogError ("The RaceEntity is not DriftRaceEntity");
			Destroy (this);
			return;
		}

		if (AllCars.Count <= 1 && !WorldLoading.IsMultiplayer)
		{
			gameObject.SetActive (false);
			return;
		}

		GameController.RatingOfPlayersChanged += UpdateRating;
		DriftRaceEntity.MultiplayerCarAdded += UpdatePlayersList;

		UpdatePlayersList ();
		PlayerUIRef.SetActive (false);
	}

	void UpdatePlayersList ()
	{
		foreach (var carStat in AllCars) {
			if (!PanelCarStatisticsDict.ContainsKey (carStat))
			{
				var newPanel = Instantiate (PlayerUIRef, PlayerUIRef.transform.parent);
				newPanel.SetActive (true);
				PanelCarStatisticsDict.Add (carStat, newPanel);
			}
		}
		UpdateRating (forceSetPositions: true);
	}

	void UpdateRating ()
	{
		UpdateRating (forceSetPositions: false);
	}

	void UpdateRating (bool forceSetPositions = false)
	{
		StopAllCoroutines ();
		for (int i = 0; i < AllCars.Count; i++)
		{
			DriftRatingPlayerUI panel;
			var car = AllCars[i];

			if (!PanelCarStatisticsDict.TryGetValue (car, out panel))
			{
				Debug.LogErrorFormat ("Panel for player({0}) not found", car.PlayerName);
				continue;
			}

			var targetPos = FirstPanelPosition + OffsetToNextPosition * i;

			panel.UpdateData (string.Format ("{0}:{1}", i + 1, car.PlayerName), car.TotalScore.ToInt());
			panel.SetActive (i + 1 <= MaxPanels);

			if (!gameObject.activeInHierarchy || forceSetPositions)
			{
				panel.Rect.anchoredPosition = targetPos;
				continue;
			}
			else
			{
				StartCoroutine (MoveToPosition (panel, targetPos));
			}
		}
	}

	IEnumerator MoveToPosition (DriftRatingPlayerUI panel, Vector2 targetPos)
	{
		while (!Mathf.Approximately (panel.Rect.anchoredPosition.x, targetPos.x))
		{
			yield return null;
			panel.Rect.anchoredPosition = Vector2.MoveTowards (panel.Rect.anchoredPosition, targetPos, Time.deltaTime * MovePanelSpeed);
		}
	}
}
