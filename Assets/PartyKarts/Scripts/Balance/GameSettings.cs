using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance
{
	/// <summary>
	/// Gameplay settings. TODO Add new regimes.
	/// </summary>

	[CreateAssetMenu (fileName = "GameSettings", menuName = "GameBalance/Settings/GameSettings")]

	public class GameSettings :ScriptableObject
	{
		[SerializeField] string m_MainMenuSceneName = "MainMenuScene";
		[SerializeField] RegimeSettings m_DefaultRegimeSettings;
		[SerializeField] RegimeSettings m_RaceRegimeSettings;
		[SerializeField] DriftRegimeSettings m_DriftRegimeSettings;
		[SerializeField] List<TrackPreset> m_Tracks = new List<TrackPreset>();
		[SerializeField] List<string> m_BotNames = new List<string>(){ "Mason(b)", "Emma(b)", "Sofia(b)", "William(b)", "Natalie(b)", "Michael(b)", "Emily(b)", "Jacob(b)", };

		public string MainMenuSceneName { get { return m_MainMenuSceneName; } }
		public RegimeSettings DefaultRegimeSettings { get { return m_DefaultRegimeSettings; } }
		public RegimeSettings RaceRegimeSettings { get { return m_RaceRegimeSettings; } }
		public DriftRegimeSettings DriftRegimeSettings { get { return m_DriftRegimeSettings; } }
		public List<string> BotNames { get { return m_BotNames; } }
		public List<TrackPreset> Tracks
		{
			get
			{
				m_Tracks.RemoveAll (t => t == null);
				return m_Tracks;
			}
		}

		public void AddAvailableTrack (TrackPreset track)
		{
			m_Tracks.Add (track);
			m_Tracks.RemoveAll (t => t == null);

#if UNITY_EDITOR

			UnityEditor.EditorUtility.SetDirty (this);
			UnityEditor.AssetDatabase.SaveAssets ();

#endif
		}
	}

}
