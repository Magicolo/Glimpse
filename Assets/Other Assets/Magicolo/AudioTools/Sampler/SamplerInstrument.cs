using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SamplerInstrument : MultipleAudioItem {
		
		public SamplerInstrumentSettings settings;
		public Sampler sampler;
		public Dictionary<int, List<SingleAudioItem>> noteDict;

		public SamplerInstrument(string name, int id, SamplerItemManager itemManager, Sampler sampler)
			: base(name, id, itemManager, sampler) {
			this.sampler = sampler;
		}
		
		public void Initialize(int id, SamplerItemManager itemManager) {
			this.Id = id;
			this.itemManager = itemManager;
			audioItems = audioItems ?? new List<AudioItem>();
			actions = actions ?? new List<AudioAction>();
			noteDict = new Dictionary<int, List<SingleAudioItem>>();
			
			settings.SetReferences();
		}

		public virtual void AddAudioItem(SingleAudioItem audioItem, int note, float velocity) {
			audioItems = audioItems ?? new List<AudioItem>();
			audioItems.Add(audioItem);
			
			if (!noteDict.ContainsKey(note)){
				noteDict[note] = new List<SingleAudioItem>();
			}
			noteDict[note].Add(audioItem);
			
			if (settings.generateMode == SamplerInstrumentSettings.GenerateModes.GenerateAtRuntime && settings.destroyIdle && !GetLayer(note, velocity).original) {
				sampler.coroutineHolder.RemoveCoroutines(Name + note + velocity);
				sampler.coroutineHolder.AddCoroutine(Name + note + velocity, DestroyAfterIdleThreshold(audioItem, settings.idleThreshold));
			}
			
			if (settings.velocitySettings.affectsVolume) {
				audioItem.SetVolume(audioItem.GetVolume() * settings.velocitySettings.curve.Evaluate(velocity / 128));
			}
		}
		
		public virtual void Stop(int note) {
			if (noteDict.ContainsKey(note)){
				foreach (SingleAudioItem audioItem in noteDict[note]) {
					audioItem.Stop();
				}
				noteDict.Remove(note);
			}
		}
		
		public SamplerInstrumentLayer GetLayer(int note, float velocity) {
			return settings.GetLayer(note, velocity);
		}
		
		public virtual IEnumerator DestroyAfterIdleThreshold(SingleAudioItem audioItem, float idleThreshold) {
			AudioClip clipToRemove = audioItem.audioSource.clip;
			
			while (audioItem != null && audioItem.State != AudioStates.Stopped) {
				yield return new WaitForSeconds(0);
			}
			
			float counter = 0;
			while (counter < idleThreshold) {
				yield return new WaitForSeconds(0);
				counter += Time.deltaTime;
			}
			
			if (clipToRemove != null) {
				clipToRemove.Remove();
			}
		}
	}
}
