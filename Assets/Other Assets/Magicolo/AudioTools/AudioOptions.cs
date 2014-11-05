using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	public class AudioOptions : MonoBehaviour {

		AudioSource source;
		public AudioSource Source {
			get {
				source = source ?? this.GetOrAddComponent<AudioSource>();
				return source;
			}
		}

		public bool original;
		public AudioInfo audioInfo;
		public AudioPlayer audioPlayer;
		
		public void Update() {
			if (!Application.isPlaying) {
				if (Source == null || Source.clip == null) {
					gameObject.Remove();
					return;
				}
				
				this.SetUniqueName(name, "", audioPlayer.hierarchyManager.audioOptions);
			}
		}

		public void FreezeTransform() {
			transform.hideFlags = HideFlags.HideInInspector;
			transform.Reset();
		}
	}
}
