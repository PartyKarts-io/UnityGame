using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceRatingOfPlayersUI :MonoBehaviour
{

	[SerializeField] RaceRatingPlayerUI PlayerUIRef;
	[SerializeField] Vector2 FirstPanelPosition;
	[SerializeField] Vector2 OffsetToNextPosition;
	[SerializeField] float MovePanelSpeed = 400;
	[SerializeField] int MaxPanels = 6;

	RaceEntity RaceEntity { get { return GameController.RaceEntity as RaceEntity; } }
	GameController GameController { get { return GameController.Instance; } }
	Dictionary<CarStatistic, RaceRatingPlayerUI> PanelCarStatisticsDict = new Dictionary<CarStatistic, RaceRatingPlayerUI>();
	List<CarStatistic> AllCars { get { return RaceEntity.CarsStatistics; } }

	void Start ()
	{
		if (AllCars.Count <= 1 && !WorldLoading.IsMultiplayer)
		{
			gameObject.SetActive (false);
			return;
		}

		PlayerUIRef.SetActive (false);
		GameController.RatingOfPlayersChanged += UpdateRating;
		RaceEntity.MultiplayerCarAdded += UpdatePlayersList;

		UpdatePlayersList ();
	}

	void UpdatePlayersList ()
	{
		foreach (var carStat in AllCars)
		{
			if (!PanelCarStatisticsDict.ContainsKey (carStat))
			{
				var newPanel = Instantiate (PlayerUIRef, PlayerUIRef.transform.parent);
				newPanel.SetActive (true);
				PanelCarStatisticsDict.Add (carStat, newPanel);
			}
		}
		UpdateRating (true);
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
			RaceRatingPlayerUI panel;
			var car = AllCars[i];

			if (!PanelCarStatisticsDict.TryGetValue (car, out panel))
			{
				Debug.LogErrorFormat ("Panel for player({0}) not found", car.PlayerName);
				continue;
			}

			var targetPos = FirstPanelPosition + OffsetToNextPosition * i;
			string fullName = car.PlayerName;
			string truncatedName = fullName.Substring(0, 4) + "..." + fullName.Substring(fullName.Length - 4);

            panel.UpdateData (truncatedName, i + 1);
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

	IEnumerator MoveToPosition (RaceRatingPlayerUI panel, Vector2 targetPos)
	{
		while (!Mathf.Approximately (panel.Rect.anchoredPosition.x, targetPos.x))
		{
			yield return null;
			panel.Rect.anchoredPosition = Vector2.MoveTowards (panel.Rect.anchoredPosition, targetPos, Time.deltaTime * MovePanelSpeed);
		}
	}
}
