using PG_Atributes;
using PG_Physics.Wheel;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance
{

	/// <summary>
	/// Base calss fo regimes.
	/// </summary>

	[CreateAssetMenu (fileName = "RegimeSettings", menuName = "GameBalance/Settings/RegimeSettings")]
	public class RegimeSettings :ScriptableObject
	{
		[SerializeField] string m_RegimeSceneName;
		[SerializeField] List<CarPreset> m_AvailableCars = new List<CarPreset>();
		[SerializeField] AiConfig m_AiConfog;

		[Header("Info")]
		[SerializeField] string m_RegimeCaption;
		[SerializeField] Sprite m_RegimeImage;

		public string RegimeSceneName { get { return m_RegimeSceneName; } }
		public AiConfig GetAiConfig { get { return m_AiConfog; } }
		public string RegimeCaption { get { return m_RegimeCaption; } }
		public Sprite RegimeImage { get { return m_RegimeImage; } }
		public List<CarPreset> AvailableCars
		{
			get
			{
				m_AvailableCars.RemoveAll (c => c == null);
				return m_AvailableCars;
			}
		}
        [SerializeField] CarDriftConfig m_CarDriftConfig;

		[SerializeField] PG_WheelColliderConfig m_FrontWheelsConfig;        //To change the friction of the front wheel.
		[SerializeField] PG_WheelColliderConfig m_RearWheelsConfig;         //To change the friction of the rear wheel.

        public CarDriftConfig CarDriftConfig { get { return m_CarDriftConfig; } }
		public PG_WheelColliderConfig FrontWheelsConfig { get { return m_FrontWheelsConfig; } }
		public PG_WheelColliderConfig RearWheelsConfig { get { return m_RearWheelsConfig; } }

		public void AddAvailableCar (CarPreset car)
		{
			m_AvailableCars.Add (car);
			m_AvailableCars.RemoveAll (c => c == null);

#if UNITY_EDITOR

			UnityEditor.EditorUtility.SetDirty (this);
			UnityEditor.AssetDatabase.SaveAssets ();

#endif
		}



		/// <summary>
		/// The config is stored in the regime settings, one config is used for all bots.
		/// </summary>
		[System.Serializable]
		public class AiConfig
		{
			public float MaxSpeed = 160;                                //Max speed for bots.
			public float MinSpeed = 30;                                 //Min speed for bots. Bots adhere to speed in a given range.
			public float AccelSensitivity = 1f;							//The sensitivity of the set acceleration.
			public float BrakeSensitivity = 1f;                         //The sensitivity of the set acceleration.
			public float ReverceWaitTime = 2;                           //If the car does not move directly at a specified time, then it starts to go back.
			public float ReverceTime = 2;                               //Reversing time.
			public float BetweenReverceTimeForReset = 6;				//To reset the position of the AI car.

			public float OffsetToFirstTargetPoint = 5;                  // Offset to the first point along the route.
			public float SpeedFactorToFirstTargetPoint = -0.7f;         // A multiplier adding distance to the first point along the route.
			public float OffsetToSecondTargetPoint = 11;                // Offset to the second point along the route.
			public float SpeedFactorToSecondTargetPoint = 0.6f;         // A multiplier adding distance to the second point along the route.

			public float LookAngleSppedFactor = 30f;					//The multiplier of the angle of rotation of the wheels, to calculate the speed (The smaller the angle to the point of repetition, the greater the speed).
			public float SetSteerAngleSensitivity = 5f;					//Steer angle sensitivity
		}

	}

}
