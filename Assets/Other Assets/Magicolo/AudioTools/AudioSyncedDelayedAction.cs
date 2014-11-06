using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioSyncedDelayedAction : AudioAction {

		public int delay;
		
		public AudioSyncedDelayedAction(float delay, SyncMode syncMode, Metronome metronome, ActionTypes type, AudioItem audioItem, params AudioOption[] audioOptions)
			: base(metronome, type, audioItem, audioOptions) {
			
			this.delay = metronome.ConvertToBeats(delay, syncMode);
		}
		
		public override void Update() {
			if (delay <= 0) {
				ApplyAction();
			}
		}
		
		public override void BeatEvent(int currentBeat) {
			delay -= 1;
			Update();
		}
	}
}
