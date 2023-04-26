using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// FX and sounds effects.
/// </summary>
public class FXController :Singleton<FXController>
{

	[Header("Particles settings")]
	[SerializeField] ParticleSystem AsphaltSmokeParticles;      //Asphalt smoke (Gray).
	[SerializeField] ParticleSystem GroundSmokeParticles;       //Ground smoke (Brown).
	[SerializeField] ParticleSystem SparkParticles;             //TODO add sparks.
	[SerializeField] LayerMask TrackMask;                       //Asphalt track mask.
	[SerializeField] LayerMask GroundMask;                      //Ground mask.

	[Header("Trail settings")]
	[SerializeField] TrailRenderer TrailRef;                    //Trail ref, The lifetime of the tracks is configured in it.
	[SerializeField] Transform TrailsHolder;                    //Parent for copy of TrailRef.

	[Header("Sound settings")]
	[SerializeField] AudioSource AudioSourceRef;                //AudioSource ref.
	[SerializeField] Transform SoundsHolder;                    //Parent for copy of AudioSourceRef.
	[SerializeField] int MaxAudioSources = 10;                  //Max pool AudioSources.
	[SerializeField] float MinTimeBetweenSounds = 1f;

	protected override void AwakeSingleton ()
	{
		//Hide ref objects.
		TrailRef.gameObject.SetActive (false);
		AudioSourceRef.gameObject.SetActive (false);
	}

	#region Particles

	public ParticleSystem GetAspahaltParticles { get { return AsphaltSmokeParticles; } }
	public ParticleSystem GetGroundParticles { get { return GroundSmokeParticles; } }

	/// <summary>
	/// Returns a particle system based on the surface layer.
	/// </summary>
	/// <param name="layer">Surface layer</param>
	/// <returns></returns>
	public ParticleSystem GetParticles (int layer)
	{
		if (GroundMask.LayerInMask (layer))
		{
			return GroundSmokeParticles;
		}

		return AsphaltSmokeParticles;
	}

	#endregion //Particles

	#region Trail

	Queue<TrailRenderer> FreeTrails = new Queue<TrailRenderer>();

	/// <summary>
	/// Get first free trail and set start position.
	/// </summary>
	public TrailRenderer GetTrail (Vector3 startPos)
	{
		TrailRenderer trail = null;
		if (FreeTrails.Count > 0)
		{
			trail = FreeTrails.Dequeue ();
		}
		else
		{
			trail = Instantiate (TrailRef, TrailsHolder);
		}

		trail.transform.position = startPos;
		trail.gameObject.SetActive (true);

		return trail;
	}

	/// <summary>
	/// Set trail as free and wait life time.
	/// </summary>
	public void SetFreeTrail (TrailRenderer trail)
	{
		StartCoroutine (WaitVisibleTrail (trail));
	}

	/// <summary>
	/// The trail is considered busy until it disappeared.
	/// </summary>
	private IEnumerator WaitVisibleTrail (TrailRenderer trail)
	{
		trail.transform.SetParent (TrailsHolder);
		yield return new WaitForSeconds (trail.time);
		trail.Clear ();
		trail.gameObject.SetActive (false);
		FreeTrails.Enqueue (trail);
	}

	#endregion //Trail

	#region Sounds

	HashSet<CarController> InPlayingCollissionCars = new HashSet<CarController>();
	Queue<AudioSource> FreeAudioSources = new Queue<AudioSource>();
	int SourceCount = 0;


	/// <summary>
	/// Play collision sound if has free audio source.
	/// </summary>
	public void PlayCollisionSound (CarController car, Collision collision)
	{

		if (!car.CarIsVisible || collision == null)
			return;

		var collisionLayer = collision.gameObject.layer;

		if (InPlayingCollissionCars.Contains (car))
			return;

		AudioSource source;
		if (FreeAudioSources.Count > 0)
		{
			source = FreeAudioSources.Dequeue ();
		}
		else if (SourceCount < MaxAudioSources)
		{
			source = Instantiate (AudioSourceRef, SoundsHolder);
			source.gameObject.SetActive (true);
			SourceCount++;
		}
		else
		{
			Debug.LogWarning ("No free SoundSources");
			return;
		}

		source.transform.position = collision.contacts[0].point;
		source.clip = B.SoundSettings.GetAudioClipCollision (collisionLayer);
		var volume = Mathf.Clamp01 (collision.relativeVelocity.magnitude / B.SoundSettings.CollisionSoundMultiplier);
		source.volume = volume;
		StartCoroutine (PlaySoundCoroutine (source));

		InPlayingCollissionCars.Add (car);
		StartCoroutine (RemoveLayerFromPlaying (car));
	}


	/// <summary>
	/// To prevent fast repetition of identical sounds.
	/// </summary>
	IEnumerator RemoveLayerFromPlaying (CarController car)
	{
		yield return new WaitForSeconds (MinTimeBetweenSounds);
		InPlayingCollissionCars.Remove (car);
	}

	/// <summary>
	/// Wait playing sound.
	/// </summary>
	IEnumerator PlaySoundCoroutine (AudioSource source)
	{
		if (source.clip != null)
		{
			source.Play ();
			yield return new WaitForSeconds (source.clip.length);
		}
		FreeAudioSources.Enqueue (source);
	}

	#endregion //Sounds

}
