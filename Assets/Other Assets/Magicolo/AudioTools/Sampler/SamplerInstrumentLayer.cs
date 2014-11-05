using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SamplerInstrumentLayer : INamable {
		
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
		AudioSetup audioSetup;
		public AudioSetup AudioSetup {
			get {
				return audioSetup;
			}
			set {
				audioSetup = value;
				clip = audioSetup == null ? null : audioSetup.Clip;
			}
		}
		public AudioClip referenceClip;
		public int pitchDifference;
		public AudioClip clip;
		public bool original;
		
		// For some reason, Unity serializes this field if it doesn't have this attribute even if the field is private.
		[System.NonSerialized] SamplerInstrumentSettings settings;
		
		public AudioClip GetClip() {
			if (clip == null && referenceClip != null) {
				int frequency = (int)Mathf.Max(referenceClip.frequency * Mathf.Pow(2, (float)pitchDifference / 12), 1);
				clip = AudioClip.Create(string.Format("{0} {1}{2}", referenceClip.name, pitchDifference >= 0 ? "+" : "", pitchDifference), referenceClip.samples, referenceClip.channels, frequency, settings.is3D, false);
				float[] data = new float[referenceClip.samples * referenceClip.channels];
				referenceClip.GetData(data, 0);
				clip.SetData(data, 0);
			}
			return clip;
		}

		public void SetReferences(AudioClip referenceClip, int pitchDifference, SamplerInstrumentSettings settings) {
			this.referenceClip = referenceClip;
			this.pitchDifference = pitchDifference;
			this.settings = settings;
			
			if (clip != null) {
				original = true;
			}
			
			Name = referenceClip.name;
			if (settings.generateMode == SamplerInstrumentSettings.GenerateModes.PreGenerateAll) {
				GetClip();
			}
		}
	}
}
