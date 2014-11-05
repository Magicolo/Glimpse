﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;
using System.Linq;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SingleAudioItem : AudioItem {

		public AudioOption[] containerAudioOptions = new AudioOption[0];
		
		public AudioSource audioSource;
		public AudioInfo audioInfo;
		public GameObject gameObject;
		public CoroutineHolder coroutineHolder;
		public GainManager gainManager;
		
		AudioStates pausedState;
		
		public SingleAudioItem(string name, int id, AudioSource audioSource, AudioInfo audioInfo, CoroutineHolder coroutineHolder, GainManager gainManager, AudioItemManager itemManager, Magicolo.AudioTools.Player player)
			: base(name, id, itemManager, player) {
			
			this.audioSource = audioSource;
			this.audioInfo = audioInfo;
			this.gameObject = audioSource.gameObject;
			this.coroutineHolder = coroutineHolder;
			this.gainManager = gainManager;
		}
		
		public override void Update() {
			base.Update();
			
			if (State == AudioStates.Playing && !audioSource.loop) {
				if ((audioSource.pitch > 0 && audioSource.time >= audioSource.clip.length - audioInfo.fadeOut) || (audioSource.pitch < 0 && audioSource.time <= audioInfo.fadeOut)) {
					Stop();
				}
			}
			gameObject.name = string.Format("{0} ({1})", audioInfo.Name, State);
		}
		
		protected override void UpdateVolume() {
			gainManager.volume = Volume * player.generalSettings.masterVolume;
		}
		
		protected override void UpdatePitch() {
			audioSource.pitch = Pitch;
		}

		protected virtual bool TryAddDelayedAction(AudioAction.ActionTypes actionType, params AudioOption[] audioOptions) {
			AudioOption delayOption = audioOptions.PopOptionOfType(AudioOption.OptionTypes.Delay, out audioOptions);
			AudioOption syncOption = audioOptions.PopOptionOfType(AudioOption.OptionTypes.SyncMode, out audioOptions);
			float delay = delayOption == null ? audioInfo.delay : delayOption.GetValue<float>();
			SyncMode syncMode = syncOption == null ? audioInfo.syncMode : syncOption.GetValue<SyncMode>();
			
			if (delay <= 0 && syncMode == SyncMode.None) {
				return false;
			}
			
			if (syncMode == SyncMode.None) {
				actions.Add(new AudioDelayedAction(delay, actionType, this, audioOptions));
			}
			else {
				actions.Add(new AudioSyncedDelayedAction(delay, syncMode, player.metronome, actionType, this, audioOptions));
			}
			
			audioInfo.delay = 0;
			audioInfo.syncMode = SyncMode.None;
			return true;
		}
		
		public override void Play(params AudioOption[] audioOptions) {
			List<AudioOption> audioOptionsSum = new List<AudioOption>(containerAudioOptions);
			audioOptionsSum.AddRange(audioOptions);
			
			if (!TryAddDelayedAction(AudioAction.ActionTypes.Play, audioOptionsSum.ToArray())) {
				audioInfo.ApplyAudioOptions(audioSource, containerAudioOptions);
				audioInfo.ApplyAudioOptions(audioSource, audioOptions);
				containerAudioOptions = new AudioOption[0];
				
				if (State == AudioStates.Waiting) {
					//HACK Trick to deal with reversed sounds.
					if (audioSource.pitch < 0) {
						audioSource.time = audioSource.clip.length - 0.00001f;
					}
					coroutineHolder.AddCoroutine("FadeIn", FadeIn(audioSource.volume, audioInfo.fadeIn, audioInfo.fadeInCurve));
				}
				else {
					if (State == AudioStates.Paused) {
						audioSource.Play();
						coroutineHolder.ResumeCoroutines("FadeIn");
						coroutineHolder.ResumeCoroutines("RampVolume");
						coroutineHolder.ResumeCoroutines("RampPitch");
						State = pausedState;
					}
				}
			}
		}

		public override void Pause(params AudioOption[] audioOptions) {
			if (!TryAddDelayedAction(AudioAction.ActionTypes.Pause, audioOptions)) {
				audioInfo.ApplyAudioOptions(audioSource, audioOptions);
			
				if (State == AudioStates.Playing || State == AudioStates.FadingIn) {
					audioSource.Pause();
					coroutineHolder.PauseCoroutines("FadeIn");
					coroutineHolder.PauseCoroutines("RampVolume");
					coroutineHolder.PauseCoroutines("RampPitch");
					pausedState = State;
					base.Pause();
				}
			}
		}

		public override void Stop(params AudioOption[] audioOptions) {
			if (!TryAddDelayedAction(AudioAction.ActionTypes.Stop, audioOptions)) {
				audioInfo.ApplyAudioOptions(audioSource, audioOptions);
			
				if (State != AudioStates.Stopped || State != AudioStates.FadingOut) {
					coroutineHolder.AddCoroutine("FadeOut", FadeOut(0, audioInfo.fadeOut, audioInfo.fadeOutCurve));
				}
			}
		}

		public override void StopImmediate() {
			if (State != AudioStates.Stopped) {
				base.Stop();
				audioSource.Stop();
				gainManager.Deactivate();
				itemManager.Deactivate(this);
				coroutineHolder.RemoveAllCoroutines();
			}
		}
		
		public override void SetVolume(float targetVolume, float time) {
			coroutineHolder.RemoveCoroutines("RampVolume");
			coroutineHolder.AddCoroutine("RampVolume", RampVolume(gainManager.volume, Mathf.Clamp(targetVolume, 0, 10), time));
		}

		public override void SetVolume(float targetVolume) {
			coroutineHolder.RemoveCoroutines("RampVolume");
			
			Volume = targetVolume;
			UpdateVolume();
		}

		public override void SetPitch(float targetPitch, float time, float quantizeStep) {
			coroutineHolder.RemoveCoroutines("RampPitch");
			coroutineHolder.AddCoroutine("RampPitch", RampPitch(audioSource.pitch, targetPitch, time, quantizeStep));
		}
		
		public override void SetPitch(float targetPitch, float time) {
			coroutineHolder.RemoveCoroutines("RampPitch");
			coroutineHolder.AddCoroutine("RampPitch", RampPitch(audioSource.pitch, targetPitch, time, 0));
		}
		
		public override void SetPitch(float targetPitch) {
			coroutineHolder.RemoveCoroutines("RampPitch");
			
			Pitch = targetPitch;
			UpdatePitch();
		}

		#region IEnumerators
		public virtual IEnumerator FadeIn(float targetVolume, float time, AnimationCurve curve) {
			State = AudioStates.FadingIn;
			audioSource.Play();
			itemManager.Activate(this);
			gainManager.Activate(); // The gainManager must be activated after the itemManager for the PDPlayer to work correctly.
			
			IEnumerator fade = FadeVolume(audioSource.volume, targetVolume, time, curve);
			while (fade.MoveNext()) {
				yield return fade.Current;
			}
			
			base.Play();
		}
		
		public virtual IEnumerator FadeOut(float targetVolume, float time, AnimationCurve curve) {
			State = AudioStates.FadingOut;
			coroutineHolder.RemoveCoroutines("FadeIn");
			
			IEnumerator fade = FadeVolume(audioSource.volume, targetVolume, time, curve);
			while (fade.MoveNext()) {
				yield return fade.Current;
			}
			
			base.Stop();
			audioSource.Stop();
			gainManager.Deactivate();
			itemManager.Deactivate(this);
			coroutineHolder.RemoveAllCoroutines();
			Update();
		}

		public virtual IEnumerator FadeVolume(float startVolume, float targetVolume, float time, AnimationCurve curve) {
			float counter = 0;
			
			while (counter < time) {
				float fadeVolume = curve.Evaluate(counter / time);
				audioSource.volume = fadeVolume * startVolume;
				counter += Time.deltaTime;
				yield return new WaitForSeconds(0);
			}
			audioSource.volume = targetVolume;
		}
		#endregion
	}
}
