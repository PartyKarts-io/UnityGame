using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance
{

	/// <summary>
	/// Links to resources.
	/// </summary>

	[CreateAssetMenu (fileName = "ResourcesSettings", menuName = "GameBalance/Settings/ResourcesSettings")]
	public class ResourcesSettings :ScriptableObject
	{

		[SerializeField] SoundControllerInUI m_SoundControllerInUI;
		[SerializeField] LoadingScreenUI m_LoadingScreenUI;
		[SerializeField] MessageBox m_MessageBox;

		public SoundControllerInUI SoundControllerInUI { get { return m_SoundControllerInUI; } }
		public LoadingScreenUI LoadingScreenUI { get { return m_LoadingScreenUI; } }
		public MessageBox MessageBox { get { return m_MessageBox; } }

	}

}
