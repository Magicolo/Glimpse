﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class MultipleAudioItem : AudioItem {
		
		public List<AudioItem> audioItems = new List<AudioItem>();
		protected readonly string rampVolumeName;
		protected readonly string rampPitchName;
		
		public MultipleAudioItem(string name, int id, AudioItemManager itemManager, Magicolo.AudioTools.Player player)
			: base(name, id, itemManager, player) {
			rampVolumeName = "RampVolume" + name + id;
			rampPitchName = "RampPitch" + name + id;
		}
		
		public virtual void Update() {
			if (RemoveStoppedAudioItems() && State != AudioStates.Stopped && audioItems.Count == 0) {
				Stop();
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
			
			for (int i = 0; i < audioItems.Count; i++) {
				if (audioItems[i].State == AudioStates.Stopped) {
					audioItems.RemoveAt(i);
				}
				else {
					allStopped = false;
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
				actions.Add(new AudioDelayedAction(delay, player.metronome, actionType, actions, this, audioOptions));
			}
			else {
				actions.Add(new AudioSyncedDelayedAction(delay, syncMode, player.metronome, actionType, actions, this, audioOptions));
			}
			
			return true;
		}
		
		public virtual void AddAudioItem(AudioItem audioItem) {
			audioItems = audioItems ?? new List<AudioItem>();
			audioItems.Add(audioItem);
			UpdateVolume();
		}
		
		public override void Play(params AudioOption[] audioOptions) {
			if (audioOptions.Length == 0 || !TryAddDelayedAction(AudioAction.ActionTypes.Play, audioOptions)) {
				if (State == AudioStates.Waiting) {
					itemManager.Activate(this);
				}
				
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
				itemManager.Deactivate(this);
				
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
			player.coroutineHolder.RemoveCoroutines(rampVolumeName);
			player.coroutineHolder.AddCoroutine(rampVolumeName, RampVolume(Volume, targetVolume, time));
		}
		
		public override void SetVolume(float targetVolume) {
			player.coroutineHolder.RemoveCoroutines(rampVolumeName);
			Volume = targetVolume;
			UpdateVolume();
		}

		public override void SetPitch(float targetPitch, float time, float quantizeStep) {
			player.coroutineHolder.RemoveCoroutines(rampPitchName);
			player.coroutineHolder.AddCoroutine(rampPitchName, RampPitch(Pitch, targetPitch, time, quantizeStep));
		}
		
		public override void SetPitch(float targetPitch, float time) {
			player.coroutineHolder.RemoveCoroutines(rampPitchName);
			player.coroutineHolder.AddCoroutine(rampPitchName, RampPitch(Pitch, targetPitch, time, 0));
		}
		
		public override void SetPitch(float targetPitch) {
			player.coroutineHolder.RemoveCoroutines(rampPitchName);
			Pitch = targetPitch;
			UpdatePitch();
		}
		
		public override string ToString() {
			return string.Format("{0}({1}, {2}, {3}, {4})", GetType().Name, Name, Id, State, Logger.ObjectToString(audioItems));
		}

	}
}
