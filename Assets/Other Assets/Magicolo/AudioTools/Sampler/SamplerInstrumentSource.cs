using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SamplerInstrumentSource : INamable {
		
		[SerializeField]
		string name;
		public string Name {
			get {
				name = string.Format("{0}{1} ({2})", noteNames[note % 12], Mathf.Floor(note / 12), note);
				return name;
			}
			set {
				name = value;
			}
		}

		public SamplerInstrumentLayer[] layers = new SamplerInstrumentLayer[16];
		public int note;
		public int dropCounter;
		
		string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

		public SamplerInstrumentSource(int note) {
			this.note = note;
		}

		public void SetReferences(int layer, AudioClip referenceClip, int referenceClipIndex, SamplerInstrumentSettings settings) {
			layers[layer].SetReferences(referenceClip, note - referenceClipIndex, settings);
		}

		public SamplerInstrumentLayer GetLayer(int layer) {
			return layers[layer];
		}

		public void Reset() {
			layers = new SamplerInstrumentLayer[16];
		}
		
		public void ResetDropCounter() {
			dropCounter = 0;
		}
	}
}
