using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectForBuildStandalone : MonoBehaviour {

#if !UNITY_EDITOR

	void Awake () {
		if (!Application.isMobilePlatform)
		{
			gameObject.SetActive (false);
		}
	}

#endif
}
