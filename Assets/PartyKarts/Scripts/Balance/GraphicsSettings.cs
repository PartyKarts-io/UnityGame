using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance
{

	/// <summary>
	/// Graphics and visual settings.
	/// </summary>

	[CreateAssetMenu (fileName = "GraphicsSettings", menuName = "GameBalance/Settings/GraphicsSettings")]
	public class GraphicsSettings :ScriptableObject
	{
		[SerializeField] int m_TargetFPSStandalone = 60;
		[SerializeField] int m_TargetFPSMobile = 30;

		public int TargetFPS { get { return Application.isMobilePlatform ? m_TargetFPSMobile : m_TargetFPSStandalone; } }
	}

}
