using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioInfo : INamable, ICloneable {

		[SerializeField]
		string name;
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		[SerializeField]
		AudioSource source;
		public AudioSource Source {
			get {
				source = source ?? AudioSetup.GetComponent<AudioSource>();
				return source;
			}
			set {
				source = value;
			}
		}

		public AudioClip Clip {
			get {
				return Resources.Load<AudioClip>(clipPath);
			}
		}
		
		public string clipPath;
		
		[SerializeField]
		AudioSetup audioSetup;
		public AudioSetup AudioSetup {
			get {
				if (audioSetup == null && !string.IsNullOrEmpty(name)) {
					GameObject defaultOptionsGameObject = GameObject.Find(Name);
					if (defaultOptionsGameObject != null) {
						audioSetup = defaultOptionsGameObject.GetComponent<AudioSetup>();
					}
				}
				return audioSetup;
			}
			set {
				audioSetup = value;
				Name = audioSetup.name;
			}
		}

		AudioOption[] defaultAudioOptions;
		
		public float fadeIn;
		public AnimationCurve fadeInCurve = new AnimationCurve(new []{ new Keyframe(0, 0), new Keyframe(1, 1) });
		public float fadeOut = 0.1F;
		public AnimationCurve fadeOutCurve = new AnimationCurve(new []{ new Keyframe(0, 1), new Keyframe(1, 0) });
		[Range(0, 1)] public float randomVolume;
		[Range(0, 6)] public float randomPitch;
		[Min] public float delay;
		public SyncMode syncMode;
		public bool doNotKill;
		
		public AudioPlayer audioPlayer;

		public AudioInfo(AudioSource source, string clipPath, AudioSetup audioSetup, float fadeIn, AnimationCurve fadeInCurve, float fadeOut, AnimationCurve fadeOutCurve, float randomVolume, float randomPitch, bool doNotKill, AudioPlayer audioPlayer) {
			this.name = audioSetup.name;
			this.source = source;
			this.clipPath = clipPath;
			this.audioSetup = audioSetup;
			this.fadeIn = fadeIn;
			this.fadeInCurve = fadeInCurve;
			this.fadeOut = fadeOut;
			this.fadeOutCurve = fadeOutCurve;
			this.randomVolume = randomVolume;
			this.randomPitch = randomPitch;
			this.doNotKill = doNotKill;
			this.audioPlayer = audioPlayer;
		}
		
		public AudioInfo(AudioSource source, AudioSetup audioSetup, AudioPlayer audioPlayer) {
			this.name = audioSetup.name;
			this.source = source;
			this.audioSetup = audioSetup;
			this.audioPlayer = audioPlayer;
		}
		
		public void ConvertToAudioOptions() {
			defaultAudioOptions = new [] {
				AudioOption.Mute(Source.mute),
				AudioOption.BypassEffects(Source.bypassEffects),
				AudioOption.BypassListenerEffects(Source.bypassListenerEffects),
				AudioOption.BypassReverbZones(Source.bypassReverbZones),
				AudioOption.Loop(Source.loop),
				AudioOption.Priority(Source.priority),
				AudioOption.Volume(Source.volume),
				AudioOption.Pitch(Source.pitch),
				AudioOption.DopplerLevel(Source.dopplerLevel),
				AudioOption.RolloffMode(Source.rolloffMode),
				AudioOption.MinDistance(Source.minDistance),
				AudioOption.PanLevel(Source.panLevel),
				AudioOption.Spread(Source.spread),
				AudioOption.MaxDistance(Source.maxDistance),
				AudioOption.Pan2D(Source.pan),
				AudioOption.FadeIn(fadeIn),
				AudioOption.FadeInCurve(fadeInCurve),
				AudioOption.FadeOut(fadeOut),
				AudioOption.FadeOutCurve(fadeOutCurve),
				AudioOption.RandomVolume(randomVolume),
				AudioOption.RandomPitch(randomPitch),
				AudioOption.Delay(delay),
				AudioOption.SyncMode(syncMode),
				AudioOption.DoNotKill(doNotKill)
			};
		}
		
		public void ApplyDefaultOptions(AudioSource audioSource) {
			ConvertToAudioOptions();
			ApplyAudioOptions(audioSource, defaultAudioOptions);
		}
		
		public void ApplyAudioOptions(AudioSource audioSource, params AudioOption[] options) {
			foreach (AudioOption option in options) {
				ApplyAudioOption(option, audioSource);
			}
		}
		
		public void ApplyAudioOption(AudioOption option, AudioSource audioSource) {
			switch (option.type) {
				case AudioOption.OptionTypes.FadeIn:
					fadeIn = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.FadeInCurve:
					fadeInCurve = option.GetValue<AnimationCurve>();
					break;
				case AudioOption.OptionTypes.FadeOut:
					fadeOut = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.FadeOutCurve:
					fadeOutCurve = option.GetValue<AnimationCurve>();
					break;
				case AudioOption.OptionTypes.RandomVolume:
					randomVolume = option.GetValue<float>();
					audioSource.volume += UnityEngine.Random.Range(-randomVolume, randomVolume);
					break;
				case AudioOption.OptionTypes.RandomPitch:
					randomPitch = option.GetValue<float>();
					audioSource.pitch += UnityEngine.Random.Range(-randomPitch, randomPitch);
					break;
				case AudioOption.OptionTypes.Delay:
					delay = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.SyncMode:
					syncMode = option.GetValue<SyncMode>();
					break;
				case AudioOption.OptionTypes.DoNotKill:
					doNotKill = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.Clip:
					audioSource.clip = option.GetValue<AudioClip>();
					break;
				case AudioOption.OptionTypes.Mute:
					audioSource.mute = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.BypassEffects:
					audioSource.bypassEffects = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.BypassListenerEffects:
					audioSource.bypassListenerEffects = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.BypassReverbZones:
					audioSource.bypassReverbZones = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.Loop:
					audioSource.loop = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.Priority:
					audioSource.priority = option.GetValue<int>();
					break;
				case AudioOption.OptionTypes.Volume:
					audioSource.volume = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.Pitch:
					audioSource.pitch = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.DopplerLevel:
					audioSource.dopplerLevel = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.RolloffMode:
					audioSource.rolloffMode = option.GetValue<AudioRolloffMode>();
					break;
				case AudioOption.OptionTypes.MinDistance:
					audioSource.minDistance = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.PanLevel:
					audioSource.panLevel = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.Spread:
					audioSource.spread = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.MaxDistance:
					audioSource.maxDistance = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.Pan2D:
					audioSource.pan = option.GetValue<float>();
					break;
			}
		}

		public object Clone() {
			return new AudioInfo(source, clipPath, audioSetup, fadeIn, fadeInCurve, fadeOut, fadeOutCurve, randomVolume, randomPitch, doNotKill, audioPlayer);
		}
	}
}
