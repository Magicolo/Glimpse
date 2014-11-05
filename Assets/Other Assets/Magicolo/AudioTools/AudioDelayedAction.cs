using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioDelayedAction : AudioAction {

		public float delay;
		
		public AudioDelayedAction(float delay, ActionTypes type, AudioItem audioItem, params AudioOption[] audioOptions)
			: base(type, audioItem, audioOptions) {
			this.delay = delay;
		}
		
		public override void Update() {
			if (delay <= 0) {
				ApplyAction();
			}
			else {
				delay -= Time.deltaTime;
			}
		}
	}
}
