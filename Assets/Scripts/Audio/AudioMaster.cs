using UnityEngine;
using System.Collections;

public class AudioMaster : MonoBehaviour {

	public enum FootstepActions {
		Jump,
		Land,
		Run,
		Walk,
	}
	
	public enum FootstepSurfaces {
		Dirt,
		Grass,
		Stone,
	}
	
	public PlayerAudio playerAudio;
	
	public FootstepActions footstepAction;
	FootstepActions FootstepAction {
		get {
			return footstepAction;
		}
	}
	
	public FootstepSurfaces footstepSurface;
	FootstepSurfaces FootstepSurface {
		get {
			footstepSurface = TextureReader.GetMainTexture();
			return footstepSurface;
		}
	}
	
	static AudioMaster instance;
	static AudioMaster Instance {
		get {
			instance = instance ?? FindObjectOfType<AudioMaster>();
			return instance;
		}
	}
	
	static AudioItem currentFootstepSound;
	
	public void Awake() {
		PDPlayer.OpenPatch("Glimpse_MASTER");
		
		PDPlayer.Play("Wind");
		PDPlayer.Play("Music");
		PDPlayer.Play("Crickets");
		PDPlayer.Play("AMB");
	}
	
	public static AudioItem Play(string soundName, GameObject source = null) {
		return null;
//		return AudioPlayer.Play(soundName, source);
	}
	
	public static AudioItem PlayContainer(string containerName, GameObject source = null) {
		return null;
//		return AudioPlayer.PlayContainer(containerName, source);
	}
	
	public static AudioItem PlayPlayerFootstep(FootstepActions footstepAction) {
		return null;
//		Instance.footstepAction = footstepAction;
//		
//		if (currentFootstepSound != null) {
//			currentFootstepSound.Stop();
//		}
//		currentFootstepSound = AudioPlayer.PlayContainer("Player_Footstep", Instance.playerAudio.gameObject);
//		return currentFootstepSound;
	}
	
	public static AudioItem PlayDollUnseenSound(GameObject doll) {
		return null;
//		return AudioPlayer.PlayContainer("Doll_Voice_Whisper", doll);
	}
}
