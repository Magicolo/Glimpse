using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SamplerSingleAudioItem : SingleAudioItem {

		public Sampler sampler;
		
		public SamplerSingleAudioItem(string name, int id, AudioSource audioSource, AudioInfo audioInfo, CoroutineHolder coroutineHolder, GainManager gainManager, SamplerItemManager itemManager, Sampler sampler)
			: base(name, id, audioSource, audioInfo, coroutineHolder, gainManager, itemManager, sampler) {
			this.sampler = sampler;
		}
	}
}
