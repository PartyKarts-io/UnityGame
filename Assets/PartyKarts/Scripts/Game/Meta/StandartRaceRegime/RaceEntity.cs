using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// For standard racing regime.
/// </summary>
public class RaceEntity :BaseRaceEntity
{
	public CarStatistic PlayerStatistics { get; private set; }
	public List<CarStatistic> CarsStatistics { get; private set; }

	public RaceEntity (GameController controller): base(controller)
	{
		CarsStatistics = new List<CarStatistic> ();
		foreach (var car in AllCars)
		{
			CarStatistic statistic;

			if (car == PlayerCar)
			{
				statistic = new CarStatistic (car, WorldLoading.PlayerName);
				PlayerStatistics = statistic;
			}
			else
			{
				statistic = new CarStatistic (car, GetNameForBot ());
			}

			Controller.FixedUpdateAction += statistic.FixedUpdate;

			CarsStatistics.Add (statistic);
		}
		Controller.FixedUpdateAction += CheckRatingOfPlayers;
	}

	/// <summary>
	/// For create CarStatisticsDriftRegime and invoke UpdatePlayersList.
	/// </summary>
	public override void AddMultiplayerCar (MultiplayerCarController multiplayerController)
	{
		base.AddMultiplayerCar (multiplayerController);
		var statistic = new CarStatistic (multiplayerController.Car, multiplayerController.NickName, multiplayerController.IsMine);
		CarsStatistics.Add (statistic);
		if (multiplayerController.IsMine)
		{
			PlayerStatistics = statistic;
		}

		CheckRatingOfPlayers ();
		MultiplayerCarAdded.SafeInvoke ();
	}

	/// <summary>
	/// Sorting cars by distance. If the order changes, the leaderboard is updated.
	/// </summary>
	public override void CheckRatingOfPlayers ()
	{
		bool ratingChanged = false;
		for (int i = 1; i < CarsStatistics.Count; i++)
		{
			if (CarsStatistics[i].PositioningCar.ProgressDistance > CarsStatistics[i - 1].PositioningCar.ProgressDistance)
			{
				ratingChanged = true;
				break;
			}
		}

		if (ratingChanged)
		{
			CarsStatistics = CarsStatistics.OrderBy (s => -s.PositioningCar.ProgressDistance).ToList ();
			Controller.RatingOfPlayersChanged.SafeInvoke ();
		}
	}
}
