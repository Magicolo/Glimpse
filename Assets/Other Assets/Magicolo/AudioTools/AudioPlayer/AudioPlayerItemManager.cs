using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioPlayerItemManager : AudioItemManager {
		
		public AudioPlayer audioPlayer;
		
		public AudioPlayerItemManager(AudioPlayer audioPlayer)
			: base(audioPlayer.listener, audioPlayer.infoManager, audioPlayer) {
			
			this.audioPlayer = audioPlayer;
		}

		public virtual AudioItem Play(string soundName, object source, params AudioOption[] audioOptions) {
			AudioItem audioItem = GetSingleAudioItem(soundName, source);
			audioItem.Play(audioOptions);
			return audioItem;
		}

		public virtual AudioItem PlayContainer(string containerName, object source, params AudioOption[] audioOptions) {
			AudioItem audioItem = GetMultipleAudioItem(player.containerManager.GetContainer(containerName), source);
			audioItem.Play(audioOptions);
			return audioItem;
		}
	}
}
