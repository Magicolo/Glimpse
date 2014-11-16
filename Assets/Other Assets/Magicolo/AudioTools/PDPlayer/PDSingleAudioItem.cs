using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDSingleAudioItem : Magicolo.AudioTools.SingleAudioItem {
		
		public PDGainManager pdGainManager;
		public PDItemManager pdItemManager;
		public PDPlayer pdPlayer;
		
		public PDSingleAudioItem(string name, int id, AudioSource audioSource, AudioInfo audioInfo, CoroutineHolder coroutineHolder, PDGainManager pdGainManager, PDItemManager pdItemManager, PDPlayer pdPlayer)
			: base(name, id, audioSource, audioInfo, coroutineHolder, pdGainManager, pdItemManager, pdPlayer) {
			
			this.pdPlayer = pdPlayer;
			this.pdGainManager = pdGainManager;
			this.pdItemManager = pdItemManager;
		}

		protected override void UpdateVolume() {
			base.UpdateVolume();
			
			pdPlayer.communicator.SendValue(string.Format("UVoice{0}Volume", pdGainManager.voice), Volume);
		}
	}
}
