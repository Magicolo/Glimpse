using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SamplerInstrumentSettings {

		public enum GenerateModes {
			GenerateAtRuntime,
			PreGenerateAll
		}
		
		public GenerateModes generateMode = GenerateModes.GenerateAtRuntime;
		public bool destroyIdle = true;
		[Min] public float idleThreshold = 3;
		public bool is3D = true;
		public SamplerVelocitySettings velocitySettings = new SamplerVelocitySettings();
		public SamplerInstrumentSource[] sources = new SamplerInstrumentSource[128];
		
		public int minNote = 48;
		public int maxNote = 72;
		
		public SamplerInstrumentSettings() {
			for (int i = 0; i < 128; i++) {
				sources[i] = new SamplerInstrumentSource(i);
			}
		}

		public void SetReferences() {
			for (int layer = 0; layer < velocitySettings.layers; layer++) {
				AudioClip referenceClip = null;
				int referenceClipIndex = 0;
				int emptyClipsCount = 0;
				
				for (int note = minNote; note <= maxNote; note++) {
					if (referenceClip == null) {
						if (sources[note].layers[layer].clip == null) {
							emptyClipsCount += 1;
						}
						else {
							referenceClip = sources[note].layers[layer].clip;
							referenceClipIndex = note;
							
							for (int i = 0; i <= emptyClipsCount; i++) {
								sources[referenceClipIndex - i].SetReferences(layer, referenceClip, referenceClipIndex, this);
							}
						}
					}
					else {
						if (sources[note].layers[layer].clip == null) {
							if (note == maxNote) {
								for (int i = 0; i <= maxNote - referenceClipIndex; i++) {
									sources[referenceClipIndex + i].SetReferences(layer, referenceClip, referenceClipIndex, this);
								}
							}
						}
						else {
							float clipDelta = (note - referenceClipIndex) / 2;
							for (int i = 0; i <= clipDelta; i++) {
								sources[referenceClipIndex + i].SetReferences(layer, referenceClip, referenceClipIndex, this);
							}
							
							referenceClip = sources[note].layers[layer].clip;
							referenceClipIndex = note;
							
							for (int i = 0; i <= clipDelta; i++) {
								sources[referenceClipIndex - i].SetReferences(layer, referenceClip, referenceClipIndex, this);
							}
						}
					}
				}
			}
		}

		public SamplerInstrumentLayer GetLayer(int note, float velocity) {
			return sources[note].GetLayer(Mathf.Clamp((int)Mathf.Floor(velocitySettings.curve.Evaluate(velocity / 128) * velocitySettings.layers), 0, velocitySettings.layers));
		}

		public void Reset() {
			foreach (SamplerInstrumentSource source in sources) {
				source.Reset();
			}
		}
	}
}
