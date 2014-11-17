using UnityEngine;
using System.Collections;

public static class AudioSourceExtensions {

	public static AudioSource PlayOnListener(this AudioSource audioSource) {
		if (audioSource == null || audioSource.clip == null) {
			return null;
		}
		
		AudioListener listener = Object.FindObjectOfType<AudioListener>();
		if (listener == null) {
			Debug.LogError("No listener was found in the scene.");
			return null;
		}
		
		GameObject gameObject = new GameObject(audioSource.clip.name);
		gameObject.hideFlags = HideFlags.HideInHierarchy;
		AudioSource source = gameObject.AddCopiedComponent<AudioSource>(audioSource);
		gameObject.transform.parent = listener.transform;
		gameObject.transform.Reset();
		source.Play();
		
		return source;
	}
	
	public static void Copy(this AudioSource audioSource, AudioSource otherAudioSource) {
		audioSource.enabled = otherAudioSource.enabled;
		audioSource.clip = otherAudioSource.clip;
		audioSource.mute = otherAudioSource.mute;
		audioSource.bypassEffects = otherAudioSource.bypassEffects;
		audioSource.bypassListenerEffects = otherAudioSource.bypassListenerEffects;
		audioSource.bypassReverbZones = otherAudioSource.bypassReverbZones;
		audioSource.playOnAwake = otherAudioSource.playOnAwake;
		audioSource.loop = otherAudioSource.loop;
		audioSource.priority = otherAudioSource.priority;
		audioSource.volume = otherAudioSource.volume;
		audioSource.pitch = otherAudioSource.pitch;
		audioSource.dopplerLevel = otherAudioSource.dopplerLevel;
		audioSource.rolloffMode = otherAudioSource.rolloffMode;
		audioSource.minDistance = otherAudioSource.minDistance;
		audioSource.panLevel = otherAudioSource.panLevel;
		audioSource.spread = otherAudioSource.spread;
		audioSource.maxDistance = otherAudioSource.maxDistance;
		audioSource.pan = otherAudioSource.pan;
		audioSource.time = otherAudioSource.time;
		audioSource.timeSamples = otherAudioSource.timeSamples;
		audioSource.velocityUpdateMode = otherAudioSource.velocityUpdateMode;
		audioSource.ignoreListenerPause = otherAudioSource.ignoreListenerPause;
		audioSource.ignoreListenerVolume = otherAudioSource.ignoreListenerVolume;
	}
}
