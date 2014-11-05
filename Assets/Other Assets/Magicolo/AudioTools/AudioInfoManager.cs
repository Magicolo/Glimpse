using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioInfoManager {

		public AudioPlayer audioPlayer;
		
		Dictionary<string, AudioInfo> audioInfoDict;

		public AudioInfoManager(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			BuildAudioInfoDict();
		}
		
		public void BuildAudioInfoDict() {
			audioInfoDict = new Dictionary<string, AudioInfo>();
			
			foreach (AudioOptions options in Object.FindObjectsOfType<AudioOptions>()) {
				audioInfoDict[options.name] = options.audioInfo;
			}
		}
		
		public AudioInfo GetAudioInfo(string audioInfoName) {
			AudioInfo audioInfo = null;
			try {
				audioInfo = new AudioInfo(audioInfoDict[audioInfoName]);
			}
			catch {
				Debug.LogError(string.Format("Sound named {0} was not found.", audioInfoName));
			}
			return audioInfo;
		}
	}
}
