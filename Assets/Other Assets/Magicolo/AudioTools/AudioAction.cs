using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public abstract class AudioAction : ISyncable {

		public enum ActionTypes {
			Play,
			Pause,
			Stop
		}
		
		public bool isApplied;
		
		public Metronome metronome;
		public ActionTypes type;
		public List<AudioAction> actions;
		public AudioItem audioItem;
		public AudioOption[] audioOptions;

		protected AudioAction(Metronome metronome, AudioAction.ActionTypes type, List<AudioAction> actions, AudioItem audioItem, params AudioOption[] audioOptions) {
			this.metronome = metronome;
			this.type = type;
			this.actions = actions;
			this.audioItem = audioItem;
			this.audioOptions = audioOptions;
			
			metronome.Subscribe(this);
		}
		
		public abstract void Update();
		
		public virtual void ApplyAction() {
			if (isApplied) {
				return;
			}
			
			switch (type) {
				case ActionTypes.Play:
					audioItem.Play(audioOptions);
					break;
				case ActionTypes.Pause:
					audioItem.Pause(audioOptions);
					break;
				case ActionTypes.Stop:
					audioItem.Stop(audioOptions);
					break;
			}
			
			isApplied = true;
			actions.Remove(this);
			metronome.Unsubscribe(this);
		}
		
		public virtual void TickEvent() {
		}

		public virtual void BeatEvent(int currentBeat) {
		}

		public virtual void MeasureEvent(int currentMeasure) {
		}
	}
}
