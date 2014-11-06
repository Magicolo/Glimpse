using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SamplerInstrument : MultipleAudioItem {
		
		public SamplerInstrumentSettings settings;
		public Sampler sampler;
		
		Dictionary<int, SingleAudioItem> noteDict;

		public SamplerInstrument(string name, int id, SamplerItemManager itemManager, Sampler sampler)
			: base(name, id, itemManager, sampler) {
			this.sampler = sampler;
		}
		
		public void Initialize(int id, SamplerItemManager itemManager) {
			Id = id;
			this.itemManager = itemManager;
			audioItems = audioItems ?? new List<AudioItem>();
			actions = actions ?? new List<AudioAction>();
			noteDict = new Dictionary<int, SingleAudioItem>();
			
			settings.SetReferences();
		}
		
		public override void Update() {
			base.Update();
			
			State = audioItems.Count == 0 ? AudioStates.Stopped : AudioStates.Playing;
		}
		
		public override bool RemoveStoppedAudioItems() {
			base.RemoveStoppedAudioItems();
			
			foreach (int note in new List<int>(noteDict.Keys)) {
				if (noteDict.ContainsKey(note) && noteDict[note] != null && noteDict[note].State == AudioStates.Stopped) {
					noteDict.Remove(note);
				}
			}
			return false;
		}
		
		public virtual void AddAudioItem(SingleAudioItem audioItem, int note, float velocity) {
			base.AddAudioItem(audioItem);
			
			Stop(note);
			noteDict[note] = audioItem;
			
			if (settings.generateMode == SamplerInstrumentSettings.GenerateModes.GenerateAtRuntime && settings.destroyIdle && !GetLayer(note, velocity).original) {
				sampler.coroutineHolder.RemoveCoroutines(Name + note + velocity);
				sampler.coroutineHolder.AddCoroutine(Name + note + velocity, DestroyAfterIdleThreshold(audioItem, settings.idleThreshold));
			}
			
			if (settings.velocitySettings.affectsVolume) {
				audioItem.audioSource.volume *= settings.velocitySettings.curve.Evaluate(velocity / 128);
			}
			
			if (!itemManager.IsActive(this)){
				itemManager.Activate(this);
			}
		}
		
		public virtual void Stop(int note) {
			if (noteDict.ContainsKey(note)) {
				noteDict[note].Stop();
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
