using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To play sounds in the UI.
/// </summary>
public class SoundControllerInUI :MonoBehaviour
{

	[SerializeField] AudioSource SourceRef;

	HashSet<AudioSource> SourcePool = new HashSet<AudioSource>();

	private void Awake ()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad (this);
		}
		else
		{
			Destroy (gameObject);
		}
	}

	void Play (AudioClip clip)
	{
		AudioSource source = null;

		foreach (var s in SourcePool)
		{
			if (s.isPlaying)
			{
				source = s;
				break;
			}
		}

		if (source == null)
		{
			source = GameObject.Instantiate (SourceRef, this.transform);
			source.priority = 0 + (8 * SourcePool.Count);
			SourcePool.Add (source);
		}

		source.clip = clip;
		source.Play ();
	}


	#region Static fields

	public static SoundControllerInUI Instance { get; private set; }

	/// <summary>
	/// Play audioClip.
	/// Creates an instance of SoundControllerInUI, if it is not already created.
	/// </summary>
	public static void PlayAudioClip (AudioClip clip)
	{
		if (clip == null)
			return;
		CheckController ();
		Instance.Play (clip);
	}

	static void CheckController ()
	{
		if (Instance == null)
		{
			GameObject.Instantiate (B.ResourcesSettings.SoundControllerInUI);
		}
	}

	#endregion //Static fields
}
