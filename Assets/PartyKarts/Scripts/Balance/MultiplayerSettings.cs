using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameBalance
{

	/// <summary>
	/// Mask and layers settings.
	/// </summary>

	[CreateAssetMenu (fileName = "MultiplayerSettings", menuName = "GameBalance/Settings/MultiplayerSettings")]
	public class MultiplayerSettings :ScriptableObject
	{
		[SerializeField] List<ServerName> m_Servers = new List<ServerName>();

		[Header("Ping info settings")]
		[SerializeField] float m_PingUpdateSettings = 1;
		[SerializeField] Sprite m_BadPingSprite;
		[SerializeField] Sprite m_MediumPingSprite;
		[SerializeField] Sprite m_GoodPingSprite;
		[SerializeField] Sprite m_VeryGoodPingSprite;
		[SerializeField] int m_MediumPing = 150;
		[SerializeField] int m_GoodPing = 100;
		[SerializeField] int m_VeryGoodPing = 50;

		[Space(10)]
		[Header("Available content for multiplayer")]
		[SerializeField] List<TrackPreset> m_AvailableTracksForMultiplayer = new List<TrackPreset>();
		[SerializeField] List<CarPreset> m_AvailableCarsForMultiplayer = new List<CarPreset>();

		[Space(10)]
		[SerializeField] byte m_MaxPlayersInRoom = 4;
		[SerializeField] float m_WaitOtherPlayersTime = 3;
		[SerializeField] int m_SecondsToEndGame = 20;

		[Space(10)]
		[SerializeField] float m_SlowPosSyncLerp = 0.1f;
		[SerializeField] float m_SlowRotSyncLerp = 0.1f;

		[Space(10)]
		[SerializeField] float m_FastPosSyncLerp = 0.3f;
		[SerializeField] float m_FastRotSyncLerp = 0.3f;

		[Space(10)]
		[SerializeField] float m_DistanceFastSync = 2;
		[SerializeField] float m_DistanceTeleport = 5;

		[Space(10)]
		[SerializeField] KeyCode m_ShowNickNameCode = KeyCode.Tab;
		[SerializeField] TextMeshPro m_NickNameInWorld;
		[SerializeField] float m_NickNameY = 2.3f;
		[SerializeField] List<Color> m_NickNameColors;

		[Space(10)]
		[Header("Debug settings")]
		[SerializeField] bool m_EnableAiForDebug = false;

		public List<ServerName> Servers { get { return m_Servers; } }
		public float PingUpdateSettings { get { return m_PingUpdateSettings; } }
		public Sprite BadPingSprite { get { return m_BadPingSprite; } }
		public Sprite MediumPingSprite { get { return m_MediumPingSprite; } }
		public Sprite GoodPingSprite { get { return m_GoodPingSprite; } }
		public Sprite VeryGoodPingSprite { get { return m_VeryGoodPingSprite; } }
		public int MediumPing { get { return m_MediumPing; } }
		public int GoodPing  { get { return m_GoodPing; } }
		public int VeryGoodPing { get { return m_VeryGoodPing; } }

		public List<TrackPreset> AvailableTracksForMultiplayer 
		{ 
			get 
			{
				m_AvailableTracksForMultiplayer.RemoveAll (t => t == null);
				return m_AvailableTracksForMultiplayer; 
			} 
		}
		public List<CarPreset> AvailableCarsForMultiplayer
		{
			get
			{
				m_AvailableCarsForMultiplayer.RemoveAll (c => c == null);
				return m_AvailableCarsForMultiplayer;
			}
		}

		public byte MaxPlayersInRoom { get { return m_MaxPlayersInRoom; } }
		public float WaitOtherPlayersTime { get { return m_WaitOtherPlayersTime; } }
		public int SecondsToEndGame { get { return m_SecondsToEndGame; } }

		public float SlowPosSyncLerp { get { return m_SlowPosSyncLerp; } }
		public float SlowRotSyncLerp { get { return m_SlowRotSyncLerp; } }
		public float FastPosSyncLerp { get { return m_FastPosSyncLerp; } }
		public float FastRotSyncLerp { get { return m_FastRotSyncLerp; } }
		public float DistanceFastSync { get { return m_DistanceFastSync; } }
		public float DistanceTeleport { get { return m_DistanceTeleport; } }
		public bool EnableAiForDebug { get { return m_EnableAiForDebug; } }
		public KeyCode ShowNickNameCode { get { return m_ShowNickNameCode; } }
		public TextMeshPro NickNameInWorld { get { return m_NickNameInWorld; } }
		public float NickNameY { get { return m_NickNameY; } }
		public List<Color> NickNameColors { get { return m_NickNameColors; } }

		public Dictionary<DisconnectCause, string> DisconnectCauseStrings = new Dictionary<DisconnectCause, string>
		{
			{ DisconnectCause.ServerTimeout, "Server timeout" },
			{ DisconnectCause.MaxCcuReached, "Max CCU has reached, try connecting later." },
			{ DisconnectCause.ClientTimeout, "Client timeout" },

			//If need, you can add your own types of disconnection.
		};

		public void ShowDisconnectCause (DisconnectCause cause, System.Action onCloseAction = null)
		{
			if (cause == DisconnectCause.DisconnectByClientLogic || cause == DisconnectCause.None)
			{
				onCloseAction.SafeInvoke ();
				return;
			}

			string str = string.Empty;
			if (!DisconnectCauseStrings.TryGetValue (cause, out str))
			{
				str = cause.ToString ();
			}

			MessageBox.Show (str, null, onCloseAction, "", "Close");
		}

		public void AddAvailableTrack (TrackPreset track)
		{
			m_AvailableTracksForMultiplayer.RemoveAll (t => t == null);
			m_AvailableTracksForMultiplayer.Add (track);

#if UNITY_EDITOR

			UnityEditor.EditorUtility.SetDirty (this);
			UnityEditor.AssetDatabase.SaveAssets ();

#endif
		}

		public void AddAvailableCar (CarPreset car)
		{
			m_AvailableCarsForMultiplayer.RemoveAll (c => c == null);
			m_AvailableCarsForMultiplayer.Add (car);

#if UNITY_EDITOR

			UnityEditor.EditorUtility.SetDirty (this);
			UnityEditor.AssetDatabase.SaveAssets ();

#endif
		}
	}

	[System.Serializable]
	public struct ServerName
	{
		[SerializeField] string m_ServerCaption;
		[SerializeField] string m_ServerToken;

		public string ServerCaption { get { return m_ServerCaption; } }
		public string ServerToken { get { return m_ServerToken; } }
	}

}
