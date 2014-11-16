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
		
		public string pName;
		
		public void Update() {
			if (!Application.isPlaying) {
				if (Source == null || Clip == null) {
					gameObject.Remove();
					return;
				}
				
				if (pName != name) {
					this.SetUniqueName(name, "", audioPlayer.hierarchyManager.audioSetups);
					pName = name;
				}
			}
		}

		public void FreezeTransform() {
			transform.hideFlags = HideFlags.HideInInspector;
			transform.Reset();
		}
	}
}
