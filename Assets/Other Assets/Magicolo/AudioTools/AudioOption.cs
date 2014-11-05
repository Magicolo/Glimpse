using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;

/// <summary>
/// Lets you override the default settings of a sound set in the inspector.
/// </summary>
[System.Serializable]
public class AudioOption {

	public enum OptionTypes {
		FadeIn,
		FadeInCurve,
		FadeOut,
		FadeOutCurve,
		RandomVolume,
		RandomPitch,
		Delay,
		SyncMode,
		DoNotKill,
		Clip,
		Mute,
		BypassEffects,
		BypassListenerEffects,
		BypassReverbZones,
		Loop,
		Priority,
		Volume,
		Pitch,
		DopplerLevel,
		VolumeRolloff,
		MinDistance,
		PanLevel,
		Spread,
		MaxDistance,
		Pan2D
	}
	
	public OptionTypes type;
	
	[SerializeField]
	float floatValue;
	[SerializeField]
	int intValue;
	[SerializeField]
	bool boolValue;
	[SerializeField]
	AnimationCurve curveValue;
	[SerializeField]
	AudioRolloffMode rolloffModeValue;
	[SerializeField]
	SyncMode syncModeValue;
	[SerializeField]
	AudioClip clipValue;
	
	public static readonly OptionTypes[] FloatTypes = { OptionTypes.FadeIn, OptionTypes.FadeOut, OptionTypes.RandomVolume, OptionTypes.RandomPitch, OptionTypes.Delay, OptionTypes.Volume, OptionTypes.Pitch, OptionTypes.DopplerLevel, OptionTypes.MinDistance, OptionTypes.PanLevel, OptionTypes.Spread, OptionTypes.MaxDistance, OptionTypes.Pan2D };
	public static readonly OptionTypes[] IntTypes = { OptionTypes.Priority };
	public static readonly OptionTypes[] BoolTypes = { OptionTypes.DoNotKill, OptionTypes.Mute, OptionTypes.BypassEffects, OptionTypes.BypassListenerEffects, OptionTypes.BypassReverbZones, OptionTypes.Loop };
	public static readonly OptionTypes[] CurveTypes = { OptionTypes.FadeInCurve, OptionTypes.FadeOutCurve };
	public static readonly OptionTypes[] RolloffModeTypes = { OptionTypes.VolumeRolloff };
	public static readonly OptionTypes[] SyncModeTypes = { OptionTypes.SyncMode };
	public static readonly OptionTypes[] ClipTypes = { OptionTypes.Clip };
	
	AudioOption(OptionTypes type, object value) {
		this.type = type;
		SetDefaultValue();
		SetValue(value);
	}
	
	public static AudioOption FadeIn(float fadeIn) {
		return new AudioOption(OptionTypes.FadeIn, fadeIn);
	}
	
	public static AudioOption FadeInCurve(AnimationCurve fadeInCurve) {
		return new AudioOption(OptionTypes.FadeInCurve, fadeInCurve);
	}
	
	public static AudioOption FadeOut(float fadeOut) {
		return new AudioOption(OptionTypes.FadeOut, fadeOut);
	}
	
	public static AudioOption FadeOutCurve(AnimationCurve fadeOutCurve) {
		return new AudioOption(OptionTypes.FadeOutCurve, fadeOutCurve);
	}
	
	public static AudioOption RandomVolume(float randomVolume) {
		return new AudioOption(OptionTypes.RandomVolume, randomVolume);
	}
	
	public static AudioOption RandomPitch(float randomPitch) {
		return new AudioOption(OptionTypes.RandomPitch, randomPitch);
	}
	
	public static AudioOption Delay(float delay) {
		return new AudioOption(OptionTypes.Delay, delay);
	}
	
	public static AudioOption SyncMode(SyncMode syncMode) {
		return new AudioOption(OptionTypes.SyncMode, syncMode);
	}
	
	public static AudioOption DoNotKill(bool doNotKill) {
		return new AudioOption(OptionTypes.DoNotKill, doNotKill);
	}
	
	public static AudioOption Clip(AudioClip clip) {
		return new AudioOption(OptionTypes.Clip, clip);
	}
	
	public static AudioOption Mute(bool mute) {
		return new AudioOption(OptionTypes.Mute, mute);
	}
	
	public static AudioOption BypassEffects(bool bypassEffects) {
		return new AudioOption(OptionTypes.BypassEffects, bypassEffects);
	}
	
	public static AudioOption BypassListenerEffects(bool bypassListenerEffects) {
		return new AudioOption(OptionTypes.BypassListenerEffects, bypassListenerEffects);
	}
	
	public static AudioOption BypassReverbZones(bool bypassReverbZones) {
		return new AudioOption(OptionTypes.BypassReverbZones, bypassReverbZones);
	}
	
	public static AudioOption Loop(bool loop) {
		return new AudioOption(OptionTypes.Loop, loop);
	}
	
	public static AudioOption Priority(int priority) {
		return new AudioOption(OptionTypes.Priority, priority);
	}
	
	public static AudioOption Volume(float volume) {
		return new AudioOption(OptionTypes.Volume, volume);
	}
	
	public static AudioOption Pitch(float pitch) {
		return new AudioOption(OptionTypes.Pitch, pitch);
	}
	
	public static AudioOption DopplerLevel(float dopplerLevel) {
		return new AudioOption(OptionTypes.DopplerLevel, dopplerLevel);
	}
	
	public static AudioOption VolumeRolloff(AudioRolloffMode volumeRolloff) {
		return new AudioOption(OptionTypes.VolumeRolloff, volumeRolloff);
	}
	
	public static AudioOption MinDistance(float minDistance) {
		return new AudioOption(OptionTypes.MinDistance, minDistance);
	}
	
	public static AudioOption PanLevel(float panLevel) {
		return new AudioOption(OptionTypes.PanLevel, panLevel);
	}
	
	public static AudioOption Spread(float spread) {
		return new AudioOption(OptionTypes.Spread, spread);
	}
	
	public static AudioOption MaxDistance(float maxDistance) {
		return new AudioOption(OptionTypes.MaxDistance, maxDistance);
	}
	
	public static AudioOption Pan2D(float pan2D) {
		return new AudioOption(OptionTypes.Pan2D, pan2D);
	}
	
	public T GetValue<T>() {
		return (T)GetValue();
	}
	
	public object GetValue() {
		if (FloatTypes.Contains(type)) {
			return floatValue;
		}
		if (IntTypes.Contains(type)) {
			return intValue;
		}
		if (BoolTypes.Contains(type)) {
			return boolValue;
		}
		if (CurveTypes.Contains(type)) {
			return curveValue;
		}
		if (RolloffModeTypes.Contains(type)) {
			return rolloffModeValue;
		}
		if (SyncModeTypes.Contains(type)) {
			return syncModeValue;
		}
		if (ClipTypes.Contains(type)) {
			return clipValue;
		}
		return null;
	}

	public void SetValue(object value) {
		if (value is float) {
			floatValue = (float)value;
		}
		else if (value is int) {
			intValue = (int)value;
		}
		else if (value is bool) {
			boolValue = (bool)value;
		}
		else if (value is AnimationCurve) {
			curveValue = (AnimationCurve)value;
		}
		else if (value is AudioRolloffMode) {
			rolloffModeValue = (AudioRolloffMode)value;
		}
		else if (value is SyncMode) {
			syncModeValue = (SyncMode)value;
		}
		else if (value is AudioClip) {
			clipValue = (AudioClip)value;
		}
	}
	
	public void SetDefaultValue() {
		floatValue = 0;
		intValue = 0;
		boolValue = false;
		rolloffModeValue = AudioRolloffMode.Logarithmic;
		syncModeValue = 0;
		clipValue = null;
		
		if (CurveTypes.Contains(type) && type == OptionTypes.FadeInCurve) {
			curveValue = new AnimationCurve(new []{ new Keyframe(0, 0), new Keyframe(1, 1) });
		}
		else if (CurveTypes.Contains(type) && type == OptionTypes.FadeOutCurve) {
			curveValue = new AnimationCurve(new []{ new Keyframe(0, 1), new Keyframe(1, 0) });
		}
	}
}
