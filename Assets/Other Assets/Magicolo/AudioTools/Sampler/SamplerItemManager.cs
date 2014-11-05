using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SamplerItemManager : AudioItemManager {

		public Sampler sampler;
		
		Dictionary<string, SamplerInstrument> instrumentDict;
		
		public SamplerItemManager(Sampler sampler)
			: base(sampler.listener, sampler.infoManager, sampler) {
			
			this.sampler = sampler;
			BuildInstrumentDict();
		}

		public AudioItem Play(string instrumentName, int note, float velocity, GameObject source, params AudioOption[] audioOptions) {
			SamplerInstrument instrument = GetInstrument(instrumentName);
			
			if (velocity > 0) {
				SingleAudioItem audioItem = GetSingleAudioItem(instrument, note, velocity, source);
				instrument.AddAudioItem(audioItem, note, velocity);
				instrument.Play(audioOptions);
				return audioItem;
			}
			
			instrument.Stop(note);
			return instrument;
		}

		public virtual SamplerInstrument GetInstrument(string instrumentName) {
			return instrumentDict[instrumentName];
		}
		
		public virtual SingleAudioItem GetSingleAudioItem(SamplerInstrument instrument, int note, float velocity, GameObject source) {
			SamplerInstrumentLayer layer = instrument.GetLayer(note, velocity);
			AudioInfo audioInfo = infoManager.GetAudioInfo(layer.Name);
			AudioSource audioSource = GetAudioSource(audioInfo, source);
			audioSource.clip = layer.GetClip();
			CoroutineHolder coroutineHolder = audioSource.GetOrAddComponent<CoroutineHolder>();
			GainManager gainManager = audioSource.GetOrAddComponent<GainManager>();
			
			idCounter += 1;
			SingleAudioItem audioItem = new SingleAudioItem(audioInfo.Name, idCounter, audioSource, audioInfo, coroutineHolder, gainManager, this, sampler);
			
			gainManager.Initialize(source, audioItem, sampler);
			audioItem.Update();
			inactiveAudioItems.Add(audioItem);
			return audioItem;
		}
		
		public void BuildInstrumentDict() {
			instrumentDict = new Dictionary<string, SamplerInstrument>();
			
			foreach (SamplerInstrument instrument in sampler.editorHelper.instruments) {
				instrumentDict[instrument.Name] = instrument;
				idCounter += 1;
				instrument.Initialize(idCounter, this);
			}
		}
	}
}
