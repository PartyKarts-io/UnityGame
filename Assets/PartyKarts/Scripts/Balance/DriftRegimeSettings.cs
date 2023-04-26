using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance
{

	/// <summary>
	/// All helpers and meta of drift.
	/// </summary>

	[CreateAssetMenu (fileName = "DriftRegimeSettings", menuName = "GameBalance/Settings/DriftRegimeSettings")]
	public class DriftRegimeSettings :RegimeSettings
	{

		#region Drift settings

		[Header("Drift settings")]
		[SerializeField] float m_WaitDriftTime = 1f;                    //Waiting for the start of drift points.
		[SerializeField] float m_WaitEndDriftTime = 1f;					//Waiting until the end of the drift points.
		[SerializeField] float m_MinAngle = 10;							//The minimum angle at which drift pints begins.
		[SerializeField] float m_MaxAngle = 90f;                        //The angle at which the max accrual of points drift.
		[SerializeField] float m_MinSpeed = 20;							//The minimum speed at which drift pints begins.
		[SerializeField] float m_ScorePerMeter = 100f;					//Drift points per meter at max angle.
		[SerializeField] int m_MaxMultiplier = 5;                       //Max multiplier.
		[SerializeField] float m_MinScoreForIncMultiplier = 2000;       //One-way drift points to increase multiplier
		[SerializeField] float m_MoneyForDriftMultiplier = 0.004f;      //Additional money for drift score.

		public float WaitDriftTime { get { return m_WaitDriftTime; } }
		public float WaitEndDriftTime { get { return m_WaitEndDriftTime; } }
		public float MinAngle { get { return m_MinAngle; } }
		public float MaxAngle { get { return m_MaxAngle; } }
		public float MinSpeed { get { return m_MinSpeed; } }
		public float ScorePerMeter { get { return m_ScorePerMeter; } }
		public int MaxMultiplier { get { return m_MaxMultiplier; } }
		public float MinScoreForIncMultiplier { get { return m_MinScoreForIncMultiplier; } }
		public float MoneyForDriftMultiplier { get { return m_MoneyForDriftMultiplier; } }

		#endregion //Meta

	}

}
