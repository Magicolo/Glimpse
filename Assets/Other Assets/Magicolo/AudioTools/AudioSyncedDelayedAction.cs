using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioSyncedDelayedAction : AudioAction, ITickable {

		int delay;
		
		public AudioSyncedDelayedAction(float delay, SyncMode syncMode, Metronome metronome, ActionTypes type, AudioItem audioItem, params AudioOption[] audioOptions)
			: base(type, audioItem, audioOptions) {
			
			this.delay = metronome.ConvertToBeats(delay, syncMode);
			metronome.Subscribe(this);
		}
		
		public override void Update() {
		}

		public void BeatEvent(int currentBeat) {
			if (delay <= 0) {
				ApplyAction();
			}
			else {
				delay -= 1;
			}
		}

		public void MeasureEvent(int currentMeasure) {
		}
	}
}
