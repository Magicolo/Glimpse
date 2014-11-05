using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SamplerVelocitySettings {

		public bool affectsVolume = true;
		public AnimationCurve curve = new AnimationCurve(new [] { new Keyframe(0, 0), new Keyframe(1, 1) });
		[Range(1, 16)] public int layers = 1;
	}
}
