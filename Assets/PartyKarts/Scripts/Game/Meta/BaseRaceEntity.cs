using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for position and other control in all regimes.
/// </summary>
public class BaseRaceEntity {

	protected GameController Controller;
	protected CarController PlayerCar { get { return GameController.PlayerCar; } }
	protected List<CarController> AllCars { get { return GameController.AllCars; } }

	public System.Action MultiplayerCarAdded;

	List<string> BotNames = new List<string>();

	public BaseRaceEntity (GameController controller)
	{
		Controller = controller;
	}

	public virtual void AddMultiplayerCar (MultiplayerCarController multiplayerController) { }

	public virtual void CheckRatingOfPlayers () { }

	protected string GetNameForBot ()
	{
		if (BotNames.Count == 0)
		{
			BotNames.AddRange (B.GameSettings.BotNames);
		}

		string name = BotNames.RandomChoice ();
		BotNames.Remove (name);

		return name;
	}
}
