using UnityEngine;
using System.Collections;

public class PlayAudio : MonoBehaviour {

	AudioItem currentSound;
	
	public void Awake() {
		PDPlayer.OpenPatch("Glimpse_MASTER");
		
		PDPlayer.Play("Wind");
		PDPlayer.Play("Music");
		PDPlayer.Play ("Crickets");
		PDPlayer.Play ("AMB");
	}
	
	public void PlayPlayerFootstep(string action) {
		if (currentSound != null) {
			currentSound.Stop();
		}
		currentSound = AudioPlayer.PlayContainer(string.Format("Player_Footstep_{0}_{1}", TextureReader.GetMainTextureName(), action), gameObject);
	}
}
