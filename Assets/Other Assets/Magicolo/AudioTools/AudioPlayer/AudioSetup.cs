using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	public class AudioSetup : MonoBehaviour {

		public AudioSource Source {
			get {
				return audioInfo.Source;
			}
		}

		public AudioClip Clip {
			get {
				return audioInfo.Clip;
			}
		}
		
		public bool original;
		public AudioInfo audioInfo;
		public AudioPlayer audioPlayer;
		
		public void Update() {
			if (!Application.isPlaying) {
				if (Source == null || Clip == null) {
					gameObject.Remove();
					return;
				}
				
				this.SetUniqueName(name, "", audioPlayer.hierarchyManager.audioSetups);
			}
		}

		public void FreezeTransform() {
			transform.hideFlags = HideFlags.HideInInspector;
			transform.Reset();
		}
	}
}
