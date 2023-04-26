using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to return to the room after the end of the race.
/// </summary>
public class CheckRoomInMainMenu : MonoBehaviour {

	[SerializeField] LobbyManager LobbyManager;

	private void Start ()
	{
		LobbyManager.CheckCurrentConnection ();
	}
}
