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
				if (pdPlayer == null) {
					AudioListener.volume = masterVolume;
				}
				else {
					pdPlayer.communicator.SendValue("UMasterVolume", masterVolume);
				}
			}
		}
		
		[Range(1, 64)] public int maxVoices = 32;
		public bool persistent = true;
		
		public AudioContainer[] containers;
		
		public AudioPlayer audioPlayer;
		public PDPlayer pdPlayer;
		public Sampler sampler;

		public void Initialize(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			pdPlayer = pdPlayer ?? Object.FindObjectOfType<PDPlayer>();
			sampler = sampler ?? Object.FindObjectOfType<Sampler>();
		}
	}
}
