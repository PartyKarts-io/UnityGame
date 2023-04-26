using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CarPreset for select menu and load in world cars;
/// </summary>

namespace GameBalance
{
	[CreateAssetMenu (fileName = "Car", menuName = "GameBalance/Game/CarPreset")]
	public class CarPreset :LockedContent
	{

		[SerializeField] string m_CarCaption;
		[SerializeField] GameObject m_CarPrefabForSelectMenu;
		[SerializeField] CarController m_CarPrefab;
		[SerializeField] List<CarColorPreset> m_AvailibleColors = new List<CarColorPreset>();
		[SerializeField, TextArea(2, 5)] string m_Description;

		//To display the parameters in the car selection menu
		public float GetPower { get { return m_CarPrefab.GetCarConfig.MaxMotorTorque; } }
		public float GetControl { get { return m_CarPrefab.GetCarConfig.MaxSteerAngle; } }
		public float GetMass { get { return m_CarPrefab.RB.mass; } }

		public string CarCaption { get { return m_CarCaption; } }
		public CarController CarPrefab { get { return m_CarPrefab; } }
		public GameObject CarPrefabForSelectMenu { get { return m_CarPrefabForSelectMenu; } }
		public List<CarColorPreset> AvailibleColors { get { return m_AvailibleColors; } }
		public string Description { get { return m_Description; } }

		public CarColorPreset GetRandomColor ()
		{
			return AvailibleColors[UnityEngine.Random.Range (0, AvailibleColors.Count)];
		}

		public CarPreset (string carCaption, GameObject carPrefabForSelectMenu, CarController carPrefab, List<CarColorPreset> colors, string description,
			UnlockType unlockType, int price, TrackPreset completeTrackForUnlock)
		{
			m_CarCaption = carCaption;
			m_CarPrefabForSelectMenu = carPrefabForSelectMenu;
			m_CarPrefab = carPrefab;
			m_AvailibleColors = colors;
			m_Description = description;
			Unlock = unlockType;
			Price = price;
			CompleteTrackForUnlock = completeTrackForUnlock;
		}
	}
}
