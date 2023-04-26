using UnityEngine;

namespace GameBalance
{

	/// <summary>
	/// Main asset with links to all settings assets.
	/// </summary>

	[CreateAssetMenu (fileName = "Settings", menuName = "GameBalance/Settings/Settings")]

	public class Settings :ScriptableObject
	{

		[SerializeField] GameSettings m_GameSettings;
		[SerializeField] GraphicsSettings m_GraphicsSettings;
		[SerializeField] LayerSettings m_LayerSettings;
		[SerializeField] SoundSettings m_SoundSettings;
		[SerializeField] ResourcesSettings m_ResourcesSettings;
		[SerializeField] MultiplayerSettings m_MultiplayerSettings;

		public GameSettings GameSettings { get { return m_GameSettings; } }
		public GraphicsSettings GraphicsSettings { get { return m_GraphicsSettings; } }
		public LayerSettings LayerSettings { get { return m_LayerSettings; } }
		public SoundSettings SoundSettings { get { return m_SoundSettings; } }
		public ResourcesSettings ResourcesSettings { get { return m_ResourcesSettings; } }
		public MultiplayerSettings MultiplayerSettings { get { return m_MultiplayerSettings; } }

	}

}
