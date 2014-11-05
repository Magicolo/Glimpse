using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class MultipleAudioItem : AudioItem {
		
		public List<AudioItem> audioItems = new List<AudioItem>();
		
		public MultipleAudioItem(string name, int id, AudioItemManager itemManager, Magicolo.AudioTools.Player player)
			: base(name, id, itemManager, player) {
		}
		
		public override void Update() {
			base.Update();
			
			UpdateAudioItems();
			
			if (RemoveStoppedAudioItems() && State != AudioStates.Stopped) {
				Stop();
			}
		}

		public virtual void UpdateAudioItems() {
			foreach (AudioItem audioItem in audioItems) {
				audioItem.Update();
			}
		}
		
		protected override void UpdateVolume() {
			foreach (AudioItem audioItem in audioItems) {
				audioItem.SetVolume(Volume);
			}
		}
		
		protected override void UpdatePitch() {
			foreach (AudioItem audioItem in audioItems) {
				audioItem.SetPitch(Pitch);
			}
		}
		
		public virtual bool RemoveStoppedAudioItems() {
			bool allStopped = true;
			
			foreach (AudioItem audioItem in audioItems.ToArray()) {
				if (audioItem != null) {
					if (audioItem.GetState() == AudioStates.Stopped) {
						audioItems.Remove(audioItem);
					}
					else {
						allStopped = false;
					}
				}
			}
			return allStopped;
		}
		
		public virtual bool TryAddDelayedAction(AudioAction.ActionTypes actionType, params AudioOption[] audioOptions) {
			AudioOption delayOption = audioOptions.PopOptionOfType(AudioOption.OptionTypes.Delay, out audioOptions);
			AudioOption syncOption = audioOptions.PopOptionOfType(AudioOption.OptionTypes.SyncMode, out audioOptions);
			float delay = delayOption == null ? 0 : delayOption.GetValue<float>();
			SyncMode syncMode = syncOption == null ? SyncMode.None : syncOption.GetValue<SyncMode>();
			
			if (delay <= 0 && syncMode == SyncMode.None) {
				return false;
			}
			
			if (syncMode == SyncMode.None) {
				actions.Add(new AudioDelayedAction(delay, actionType, this, audioOptions));
			}
			else {
				actions.Add(new AudioSyncedDelayedAction(delay, syncMode, player.metronome, actionType, this, audioOptions));
			}
			
			return true;
		}
		
		public virtual void AddAudioItem(AudioItem audioItem) {
			audioItems.Add(audioItem);
			UpdateVolume();
			UpdatePitch();
		}
		
		public override void Play(params AudioOption[] audioOptions) {
			if (audioOptions.Length == 0 || !TryAddDelayedAction(AudioAction.ActionTypes.Play, audioOptions)) {
				base.Play();
				
				foreach (AudioItem audioItem in audioItems) {
					audioItem.Play(audioOptions);
				}
			}
		}

		public override void Pause(params AudioOption[] audioOptions) {
			if (audioOptions.Length == 0 || !TryAddDelayedAction(AudioAction.ActionTypes.Pause, audioOptions)) {
				base.Pause();
				
				foreach (AudioItem audioItem in audioItems) {
					audioItem.Pause(audioOptions);
				}
			}
		}

		public override void Stop(params AudioOption[] audioOptions) {
			if (audioOptions.Length == 0 || !TryAddDelayedAction(AudioAction.ActionTypes.Stop, audioOptions)) {
				base.Stop();
				
				foreach (AudioItem audioItem in audioItems) {
					audioItem.Stop(audioOptions);
				}
			}
		}

		public override void StopImmediate() {
			base.StopImmediate();
			
			foreach (AudioItem audioItem in audioItems) {
				audioItem.StopImmediate();
			}
		}
		
		public override void SetVolume(float targetVolume, float time) {
			player.coroutineHolder.RemoveCoroutines("RampVolume" + Name + Id);
			player.coroutineHolder.AddCoroutine("RampVolume" + Name + Id, RampVolume(Volume, targetVolume, time));
		}
		
		public override void SetVolume(float targetVolume) {
			player.coroutineHolder.RemoveCoroutines("RampVolume" + Name + Id);
			Volume = targetVolume;
			UpdateVolume();
		}

		public override void SetPitch(float targetPitch, float time, float quantizeStep) {
			player.coroutineHolder.RemoveCoroutines("RampPitch" + Name + Id);
			player.coroutineHolder.AddCoroutine("RampPitch" + Name + Id, RampPitch(Pitch, targetPitch, time, quantizeStep));
		}
		
		public override void SetPitch(float targetPitch, float time) {
			player.coroutineHolder.RemoveCoroutines("RampPitch" + Name + Id);
			player.coroutineHolder.AddCoroutine("RampPitch" + Name + Id, RampPitch(Pitch, targetPitch, time, 0));
		}
		
		public override void SetPitch(float targetPitch) {
			player.coroutineHolder.RemoveCoroutines("RampPitch" + Name + Id);
			Pitch = targetPitch;
			UpdatePitch();
		}
	}
}
