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
			
			foreach (AudioSetup audioSetup in Object.FindObjectsOfType<AudioSetup>()) {
				audioInfoDict[audioSetup.name] = audioSetup.audioInfo;
			}
		}
		
		public AudioInfo GetAudioInfo(string audioInfoName) {
			AudioInfo audioInfo = null;
			try {
				audioInfo = audioInfoDict[audioInfoName].Clone() as AudioInfo;
			}
			catch {
				Debug.LogError(string.Format("Sound named {0} was not found.", audioInfoName));
			}
			return audioInfo;
		}
	}
}
