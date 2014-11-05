using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Candlelight;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioGeneralSettings {

		[SerializeField, PropertyBackingField(typeof(AudioGeneralSettings), "MasterVolume", typeof(RangeAttribute), 0f, 1f)]
		public float masterVolume = 1;
		public float MasterVolume {
			get {
				return masterVolume;
			}
			set {
				masterVolume = value;
				if (Application.isPlaying) {
					if (audioPlayer != null) {
						audioPlayer.itemManager.SetMasterVolume(masterVolume);
					}
					if (pdPlayer != null) {
						pdPlayer.itemManager.SetMasterVolume(masterVolume);
					}
				}
			}
		}
		
		[Range(1, 64)] public int maxVoices = 32;
		public bool persistent = true;
		
		public AudioContainer[] containers;
		
		public AudioPlayer audioPlayer;
		public PDPlayer pdPlayer;
		public AudioHierarchyEditorHelper editorHelper;

		public void Initialize(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			editorHelper = editorHelper ?? new AudioHierarchyEditorHelper();
			editorHelper.Initialize(audioPlayer);
			pdPlayer = pdPlayer ?? Object.FindObjectOfType<PDPlayer>();
		}
		
		public void Update() {
			editorHelper.Update();
		}
	}
}
