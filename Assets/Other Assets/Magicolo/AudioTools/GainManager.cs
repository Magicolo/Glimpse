using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	public class GainManager : MonoBehaviour {

		[Min] public float volume = 1;
		public GameObject source;
		
		[HideInInspector] public SingleAudioItem audioItem;
		[HideInInspector] public Magicolo.AudioTools.Player player;
		
		Vector3 listenerRelativePosition;
	
		public virtual void Initialize(object source, SingleAudioItem audioItem, Magicolo.AudioTools.Player player) {
			this.source = source as GameObject;
			this.audioItem = audioItem;
			this.player = player;
			
			volume = audioItem.GetVolume();
		}

		public virtual void Activate() {
			enabled = true;
		}
		
		public virtual void Deactivate() {
			enabled = false;
		}
		
		public virtual void Update() {
			if (player.listener != null) {
				listenerRelativePosition = transform.position - player.listener.transform.position;
			}
			else {
				transform.position = player.listener.transform.position + listenerRelativePosition;
			}
			
			if (source != null) {
				transform.position = source.transform.position;
			}
		}
		
		public virtual void OnAudioFilterRead(float[] data, int channels) {
			for (int i = 0; i < data.Length; i++) {
				data[i] *= volume;
			}
		}
	}
}
