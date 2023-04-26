using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance
{

	/// <summary>
	/// Mask and layers settings.
	/// </summary>

	[CreateAssetMenu (fileName = "LayerSettings", menuName = "GameBalance/Settings/LayerSettings")]
	public class LayerSettings :ScriptableObject
	{
		[SerializeField] LayerMask m_RoadMask;
		[SerializeField] LayerMask m_BrakingGroundMask;

		public LayerMask RoadMask { get { return m_RoadMask; } }
		public LayerMask BrakingGroundMask { get { return m_BrakingGroundMask; } }
	}

}
