using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreateHelper
{
	[CreateAssetMenu (fileName = "CreateHelperSettings", menuName = "Utils/CreateHelperSettings")]
	public class CreateHelperSettings :ScriptableObject
	{
		[Header("Car settings")]
		[SerializeField] GameBalance.CarPreset m_CarPresetRef;
		[SerializeField] List<CarColorPreset> m_ColorPresets = new List<CarColorPreset>();
		[SerializeField] string m_CarPrefabSavePath = "Assets/ACC/Prefabs/Cars/";
		[SerializeField] string m_CarAssetSavePath = "Assets/ACC/Balance/Cars/";

		[Space(10), Header("TrackSettings")]
		[SerializeField] GameBalance.RegimeSettings m_DefaultRegime;
		[SerializeField] FXController m_FXControllerRef;
		[SerializeField] GameController m_GameControllerRef;
		[SerializeField] string m_TrackSceneSavePath = "Assets/ACC/Scenes/";
		[SerializeField] string m_TrackAssetSavePath = "Assets/ACC/Balance/Tracks/";
		[SerializeField] string m_GameControllerSavePath = "Assets/ACC/Prefabs/GameControllers/";

		public List<CarColorPreset> ColorPresets { get { return m_ColorPresets; } }
		public GameBalance.CarPreset CarPresetRef { get { return m_CarPresetRef; } }
		public string CarPrefabSavePath { get { return m_CarPrefabSavePath; } }
		public string CarAssetSavePath { get { return m_CarAssetSavePath; } }

		public GameBalance.RegimeSettings DefaultRegime { get { return m_DefaultRegime; } }
		public FXController FXControllerRef { get { return m_FXControllerRef; } }
		public GameController GameControllerRef { get { return m_GameControllerRef; } }
		public string TrackSceneSavePath { get { return m_TrackSceneSavePath; } }
		public string TrackAssetSavePath { get { return m_TrackAssetSavePath; } }
		public string GameControllerSavePath { get { return m_GameControllerSavePath; } }
	}
}
