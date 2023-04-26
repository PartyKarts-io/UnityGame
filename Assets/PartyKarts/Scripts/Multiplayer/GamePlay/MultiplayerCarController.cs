using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The main script for synchronizing cars.
/// </summary>
[RequireComponent (typeof(CarController))]
public class MultiplayerCarController :MonoBehaviourPunCallbacks, IPunObservable {

	TextMeshPro NickNameText;		//Nickname in world
	ICarControl UserControl;		//UserControl, or AIControl for debug.

	public string NickName { get { return PhotonView.Owner != null? PhotonView.Owner.NickName: string.Empty; } }
	public CarController Car { get; private set; }
	public PhotonView PhotonView { get; private set; }

	public bool IsMine { get { return PhotonView.IsMine; } }

	Rigidbody RB { get { return Car.RB; } }

	float SqrDistanceFastLerp;
	float SqrDistanceTeleport;

	void Start () 
	{
		//For load the prefab in the select car menu or debug mode.
		if (!WorldLoading.HasLoadingParams || !GameController.InGameScene)
		{
			Destroy (PhotonView);
			Destroy (this);
			return;
		}

		PhotonView = GetComponent<PhotonView> ();

		if (PhotonView == null)
		{
			Debug.LogError ("GameObject without PhotonView");
			Destroy (this);
			return;
		}

		//Destroy multiplayer components in a singleplayer game.
		if (!WorldLoading.IsMultiplayer || !WorldLoading.HasLoadingParams)
		{
			Destroy (PhotonView);
			Destroy (this);
			return;
		}

		Car = GetComponent<CarController> ();

		if (IsMine)
		{
			//For debug with AI
			if (B.MultiplayerSettings.EnableAiForDebug)
			{
				var userControl = gameObject.AddComponent<UserControl> ();
				userControl.enabled = false;
				UserControl = gameObject.AddComponent<DriftAIControl> ();
			}
			else
			{
				UserControl = gameObject.AddComponent<UserControl> ();
			}

			gameObject.AddComponent<AudioListener> ();
		}

		if (GameController.Instance == null)
		{
			Debug.LogError ("GameController not found");
			return;
		}

		if (PhotonView.Owner != null &&
			PhotonView.Owner.CustomProperties != null && 
			PhotonView.Owner.CustomProperties.ContainsKey (C.CarName) && 
			PhotonView.Owner.CustomProperties.ContainsKey (C.CarColorIndex))
		{
			var carPreset = WorldLoading.AvailableCars.Find (c => c.CarCaption == (string)PhotonView.Owner.CustomProperties[C.CarName]);
			var color = carPreset.AvailibleColors[(int)PhotonView.Owner.CustomProperties[C.CarColorIndex]];
			Car.SetColor (color);
		}

		//Adding the current car to GameController.
		GameController.Instance.AddMultiplayerCar (this);

		//Start for regimes methods invoke.
		StartDriftRegime ();
		StartRaceRegime ();

		//Calculate sqr fields.
		SqrDistanceFastLerp = B.MultiplayerSettings.DistanceFastSync * B.MultiplayerSettings.DistanceFastSync;
		SqrDistanceTeleport = B.MultiplayerSettings.DistanceTeleport * B.MultiplayerSettings.DistanceTeleport;

		//Init nickname in world
		NickNameText = Instantiate (B.MultiplayerSettings.NickNameInWorld, transform);
		NickNameText.text = PhotonView.Owner.NickName;
		NickNameText.transform.SetLocalY (B.MultiplayerSettings.NickNameY);
		if (PhotonNetwork.CurrentRoom.Players.ContainsValue (PhotonView.Owner)) {
			int colorIndex = PhotonNetwork.CurrentRoom.Players.FirstOrDefault (p => p.Value == PhotonView.Owner).Key - 1;
			colorIndex = MathExtentions.LoopClamp (colorIndex, 0, B.MultiplayerSettings.NickNameColors.Count);
			NickNameText.color = B.MultiplayerSettings.NickNameColors[colorIndex];
		}
		NickNameText.SetActive (false);
	}

#region DriftRegime

	CarStatisticsDriftRegime StatisticsDrift;

	/// <summary>
	/// We Find GameController. And subscribe to the event of changing TotalScore, to set TotalScore for all players.
	/// </summary>
	void StartDriftRegime ()
	{
		var driftRaceEntity = GameController.RaceEntity as DriftRaceEntity;
		if (driftRaceEntity != null)
		{
			StatisticsDrift = driftRaceEntity.DriftStatistics.FirstOrDefault (s => s.Car == Car);
			StatisticsDrift.TotalScoreChanged += UpdateTotalScore;
		}

	}

	/// <summary>
	/// Set TotalScore for all players
	/// </summary>
	void UpdateTotalScore () {
		if (IsMine && StatisticsDrift != null)
		{
			PhotonView.RPC ("UpdateTotalScore", RpcTarget.Others, StatisticsDrift.TotalScore);
		}
	}

	[PunRPC]
	void UpdateTotalScore (float totalScore)
	{
		if (StatisticsDrift != null)
		{
			StatisticsDrift.UpdateScore (totalScore);
		}
	}

	#endregion //DriftRegime

#region RaceRegime

	CarStatistic StatisticsRace;

	void StartRaceRegime ()
	{
		var raceEntity = GameController.RaceEntity as RaceEntity;
		if (raceEntity != null)
		{
			StatisticsRace = raceEntity.CarsStatistics.FirstOrDefault (s => s.Car == Car);
			StatisticsRace.PositioningCar.OnFinishRaceAction += OnFinishRace;
		}

	}

	void OnFinishRace ()
	{
		if (IsMine && StatisticsRace != null)
		{
			PhotonView.RPC ("RPCOnFinishRace", RpcTarget.Others, StatisticsRace.TotalRaceTime);
		}
	}

	[PunRPC]
	void RPCOnFinishRace (float time)
	{
		StatisticsRace.SetRaceTime (time);
	}

	#endregion //RaceRegime

	/// <summary>
	/// Synchronization of RigidBody, and control commands.
	/// </summary>

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (Car == null) return;

		if (stream.IsWriting)
		{
			stream.SendNext (Car.RB.position);
			stream.SendNext (Car.RB.rotation);
			stream.SendNext (Car.RB.velocity);
			stream.SendNext (Car.RB.angularVelocity.y);

			stream.SendNext (UserControl.Horizontal);
			stream.SendNext (UserControl.Vertical);
			stream.SendNext (UserControl.Brake);
		}
		else
		{
			var pos = (Vector3)stream.ReceiveNext ();
			var rot = (Quaternion)stream.ReceiveNext ();
			var velocity = (Vector3)stream.ReceiveNext ();
			var angularVelocity = (float)stream.ReceiveNext ();
			var horizontal = (float)stream.ReceiveNext ();
			var vertical = (float)stream.ReceiveNext ();
			var brake = (bool)stream.ReceiveNext ();

			//Lag compensation
			var lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
			pos += velocity * lag;
			rot *= Quaternion.AngleAxis (angularVelocity * lag, Vector3.up);

			SyncRigidbody (pos, rot, velocity, angularVelocity);
			Car.UpdateControls (horizontal, vertical, brake);
		}
	}

	/// <summary>
	/// Synchronization RigidBody.
	/// </summary>
	/// <param name="pos">World position</param>
	/// <param name="rot">World rotation</param>
	/// <param name="velocity">Velocity of RigidBody</param>
	/// <param name="angularVelocity">AngularVelocity of RigidBody</param>
	public void SyncRigidbody (Vector3 pos, Quaternion rot, Vector3 velocity, float angularVelocity)
	{
		var sqrMagnitude = (pos - RB.position).sqrMagnitude;

		//The closer the car is to the synchronization point, the smoother it moves.
		if (sqrMagnitude < SqrDistanceFastLerp)
		{
			RB.MovePosition (Vector3.Lerp (RB.position, pos, B.MultiplayerSettings.SlowPosSyncLerp));
			RB.MoveRotation (Quaternion.Lerp (RB.rotation, rot, B.MultiplayerSettings.SlowRotSyncLerp));
		}
		else if (sqrMagnitude < SqrDistanceTeleport)
		{
			RB.MovePosition (Vector3.Lerp (RB.position, pos, B.MultiplayerSettings.FastPosSyncLerp));
			RB.MoveRotation (Quaternion.Lerp (RB.rotation, rot, B.MultiplayerSettings.FastRotSyncLerp));
		}
		else
		{
			RB.MovePosition (pos);
			RB.MoveRotation (rot);
		}

		RB.velocity = velocity;
		RB.angularVelocity = new Vector3 (RB.angularVelocity.x, angularVelocity, RB.angularVelocity.z);
	}

	/// <summary>
	/// Only nickname text in world logic.
	/// </summary>
	void Update ()
	{
		if (NickNameText == null) return;

		if (Input.GetKeyDown (B.MultiplayerSettings.ShowNickNameCode))
		{
			NickNameText.SetActive (!NickNameText.gameObject.activeSelf);
		}

		if (NickNameText.gameObject.activeInHierarchy)
		{
			NickNameText.transform.rotation = Camera.main.transform.rotation;
		}
	}
}
