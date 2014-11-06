using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioDelayedAction : AudioAction {

		public float targetTime;
		
		public AudioDelayedAction(float delay, Metronome metronome, ActionTypes type, AudioItem audioItem, params AudioOption[] audioOptions)
			: base(metronome, type, audioItem, audioOptions) {
			
			this.targetTime = Time.time + delay;
		}
		
		public override void Update() {
			if (targetTime <= Time.time) {
				ApplyAction();
			}
		}

		public override void TickEvent() {
			Update();
		}
	}
}
