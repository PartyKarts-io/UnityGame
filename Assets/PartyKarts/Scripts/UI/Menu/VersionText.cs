using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionText : MonoBehaviour {

	[SerializeField] TextMeshProUGUI m_VersionText;

	void Start ()
	{
		m_VersionText.text = string.Format("{0}: {1}", C.VersionPrefix, Application.version);
	}
}
