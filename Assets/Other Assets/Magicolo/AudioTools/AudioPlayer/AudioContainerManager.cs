using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioContainerManager {

		public AudioPlayer audioPlayer;
		
		Dictionary<string, AudioContainer> containerDict;
		
		public AudioContainerManager(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			BuildContainerDict();
		}
		
		public void BuildContainerDict() {
			containerDict = new Dictionary<string, AudioContainer>();
			
			foreach (AudioContainer container in audioPlayer.generalSettings.containers) {
				containerDict[container.Name] = container;
			}
		}
		
		public AudioContainer GetContainer(string containerName){
			AudioContainer container = null;
			try {
				container = containerDict[containerName];
			}
			catch {
				Debug.LogError(string.Format("Container named {0} was not found.", containerName));
			}
			return container;
			
		}
	}
}
