using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu (fileName = "Track", menuName = "GameBalance/Game/TrackPreset")]
	public class TrackPreset :LockedContent
	{
		[SerializeField] string m_TrackName;
		[SerializeField] Sprite m_TrackIcon;
		[SerializeField] string m_SceneName;
		[SerializeField] GameController m_GameController;
		[SerializeField] int m_LapsCount = 1;
		[SerializeField] int m_AIsCount = 3;
		[SerializeField] RegimeSettings m_RegimeSettings;
		[SerializeField] float m_MoneyForFirstPlace = 1000f;

		public string TrackName { get { return m_TrackName; } }
		public Sprite TrackIcon { get { return m_TrackIcon; } }
		public string SceneName { get { return m_SceneName; } }
		public GameController GameController { get { return m_GameController; } }
		public int LapsCount { get { return m_LapsCount; } }
		public int AIsCount { get { return m_AIsCount; } }
		public RegimeSettings RegimeSettings { get { return m_RegimeSettings; } }
		public float MoneyForFirstPlace { get { return m_MoneyForFirstPlace; } }

		public TrackPreset (string trackName, Sprite trackIcon, string sceneName,
			GameController gameController, int lapsCount, int aisCount, RegimeSettings regimeSettings,
			int money, UnlockType unlock, int price, TrackPreset completeTrackForUnlock)
		{
			m_TrackName = trackName;
			m_TrackIcon = trackIcon;
			m_SceneName = sceneName;
			m_GameController = gameController;
			m_LapsCount = lapsCount;
			m_AIsCount = aisCount;
			m_RegimeSettings = regimeSettings;
			m_MoneyForFirstPlace = money;
			Unlock = unlock;
			Price = price;
			CompleteTrackForUnlock = completeTrackForUnlock;
		}
	}
}
