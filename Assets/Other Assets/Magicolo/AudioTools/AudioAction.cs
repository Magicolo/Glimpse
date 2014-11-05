using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public abstract class AudioAction {

		public enum ActionTypes {
			Play,
			Pause,
			Stop
		}
		
		public bool isApplied;
		
		public ActionTypes type;
		public AudioItem audioItem;
		public AudioOption[] audioOptions;
		
		protected AudioAction(AudioAction.ActionTypes type, AudioItem audioItem, params AudioOption[] audioOptions) {
			this.type = type;
			this.audioItem = audioItem;
			this.audioOptions = audioOptions;
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
		}
	}
}
