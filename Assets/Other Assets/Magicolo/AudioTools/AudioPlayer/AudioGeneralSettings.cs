using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioGeneralSettings {

		[SerializeField, PropertyField(typeof(RangeAttribute), 0F, 1F)]
		float masterVolume = 1;
		public float MasterVolume {
			get {
				masterVolume = AudioListener.volume;
				return masterVolume;
			}
			set {
				masterVolume = value;
				if (PdPlayer == null) {
					AudioListener.volume = masterVolume;
				}
				else {
					PdPlayer.communicator.SendValue("UMasterVolume", masterVolume);
				}
			}
		}
		
		[Range(1, 64)] public int maxVoices = 32;
		public bool persistent = true;
		
		public AudioContainer[] containers;
		
		AudioPlayer audioPlayer;
		public AudioPlayer AudioPlayer {
			get {
				if (audioPlayer == null){
					Debug.LogError("No AudioPlayer found in the scene.");
				}
				return audioPlayer;
			}
		}

		PDPlayer pdPlayer;
		public PDPlayer PdPlayer {
			get {
				if (pdPlayer == null){
					Debug.LogError("No PdPlayer found in the scene.");
				}
				return pdPlayer;
			}
		}

		Sampler sampler;
		public Sampler Sampler {
			get {
				if (sampler == null){
					Debug.LogError("No Sampler found in the scene.");
				}
				return sampler;
			}
		}
		
		public void Initialize(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			pdPlayer = pdPlayer ?? Object.FindObjectOfType<PDPlayer>();
			sampler = sampler ?? Object.FindObjectOfType<Sampler>();
		}
	
		public virtual void SetMasterVolume(float targetVolume, float time) {
			AudioPlayer.coroutineHolder.RemoveCoroutines("FadeMasterVolume");
			AudioPlayer.coroutineHolder.AddCoroutine("FadeMasterVolume", FadeMasterVolume(MasterVolume, targetVolume, time));
		}
		
		public virtual void SetMasterVolume(float targetVolume) {
			AudioPlayer.coroutineHolder.RemoveCoroutines("FadeMasterVolume");
			MasterVolume = targetVolume;
		}

		public virtual IEnumerator FadeMasterVolume(float startVolume, float targetVolume, float time) {
			float counter = 0;
			
			while (counter < time) {
				MasterVolume = (counter / time) * (targetVolume - startVolume) + startVolume;
				counter += Time.deltaTime;
				yield return new WaitForSeconds(0);
			}
			
			MasterVolume = targetVolume;
		}
	}
}
