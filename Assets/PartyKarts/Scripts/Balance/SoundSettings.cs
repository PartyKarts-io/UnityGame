using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameBalance
{

	/// <summary>
	/// Sound clips and effects settings.
	/// </summary>

	[CreateAssetMenu (fileName = "SoundSettings", menuName = "GameBalance/Settings/SoundSettings")]
	public class SoundSettings :ScriptableObject
	{

		[Header("Global settings")]
		[SerializeField] AudioMixerSnapshot m_StandartSnapshot;     //AudioMixerSnapshots for mute sounds;
		[SerializeField] AudioMixerSnapshot m_PauseSnapshot;
		[SerializeField] AudioMixerSnapshot m_MuteSnapshot;

		[SerializeField] AudioClip m_AsphaltSlip;       //The sound of slip on the asphalt.
		[SerializeField] AudioClip m_GroundSlip;        //The sound of sliding on the ground.

		[Header("Collisions")]
		[SerializeField] float m_CollisionSoundMultiplier = 40;

		[SerializeField, HideInInspector] List<Layer> m_Layers = new List<Layer>();                 //For custom editor.
		[SerializeField, HideInInspector] List<AudioClip> m_AudioClips = new List<AudioClip>();     //For custom editor.

		public AudioMixerSnapshot StandartSnapshot { get { return m_StandartSnapshot; } }
		public AudioMixerSnapshot PauseSnapshot { get { return m_PauseSnapshot; } }
		public AudioMixerSnapshot MuteSnapshot { get { return m_MuteSnapshot; } }

		public AudioClip AsphaltSlip { get { return m_AsphaltSlip; } }
		public AudioClip GroundSlip { get { return m_GroundSlip; } }
		public float CollisionSoundMultiplier { get { return m_CollisionSoundMultiplier; } }

		static Dictionary<int, AudioClip> CollisionsSounds;                                         //Collision Sounds Dictionary (Created during the first collision).
		public AudioClip GetAudioClipCollision (int layer)
		{
			AudioClip clipValue;
			if (CollisionsSounds == null)
			{
				CollisionsSounds = new Dictionary<int, AudioClip> ();
				for (int i = 0; i < m_AudioClips.Count; i++)
				{
					if (CollisionsSounds.TryGetValue (m_Layers[i], out clipValue))
					{
						Debug.LogErrorFormat ("Doble layer: {0}", m_Layers[i]);
					}
					else
					{
						CollisionsSounds.Add (m_Layers[i], m_AudioClips[i]);
					}
				}
			}
			if (CollisionsSounds.TryGetValue (layer, out clipValue))
			{
				return clipValue;
			}
			else
			{
				return null;
			}
		}
	}
}
