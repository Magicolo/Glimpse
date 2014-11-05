using UnityEngine;
using System.Collections;

public static class AudioClipExtensions {

	public static AudioClip Add(this AudioClip audioClip, AudioClip otherAudioClip) {
		int length = audioClip.samples >= otherAudioClip.samples ? audioClip.samples : otherAudioClip.samples;
		AudioClip clipSum = AudioClip.Create(string.Format("{0} + {1}", audioClip.name, otherAudioClip.name), length, audioClip.channels, audioClip.frequency, true, false);
		
		float[] dataSum;
		float[] otherData;
		
		if (audioClip.samples >= otherAudioClip.samples){
			dataSum = new float[audioClip.samples];
			audioClip.GetData(dataSum, 0);
			otherData = new float[otherAudioClip.samples];
			otherAudioClip.GetData(otherData, 0);
		}
		else {
			dataSum = new float[otherAudioClip.samples];
			otherAudioClip.GetData(dataSum, 0);
			otherData = new float[audioClip.samples];
			audioClip.GetData(otherData, 0);
		}
		
		for (int i = 0; i < otherData.Length; i++) {
			dataSum[i] += otherData[i];
		}
		
		clipSum.SetData(dataSum, 0);
		
		return clipSum;
	}
}
