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
	
	public LibPDControls libpdControls;
	public PDMaster pdMaster;
	public Wind wind;
	public Crickets crickets;
	
	static AudioMaster instance;
	static AudioMaster Instance {
		get {
			instance = instance ?? FindObjectOfType<AudioMaster>();
			return instance;
		}
	}
	
	static AudioItem currentFootstepSound;
	
	public void Awake() {
		libpdControls.Start();
		
		pdMaster.Update();
		wind.Update();
		crickets.Update();
	}
	
	public static AudioItem Play(string soundName, GameObject source = null) {
		return AudioPlayer.Play(soundName, source);
	}
	
	public static AudioItem PlayContainer(string containerName, GameObject source = null) {
		return AudioPlayer.PlayContainer(containerName, source);
	}
	
	public static AudioItem PlayPlayerFootstep(FootstepActions footstepAction) {
		Instance.footstepAction = footstepAction;
		
		if (currentFootstepSound != null) {
			currentFootstepSound.Stop();
		}
		currentFootstepSound = AudioPlayer.PlayContainer("Player_Footstep", Instance.playerAudio.gameObject);
		return currentFootstepSound;
	}
	
	public static AudioItem PlayDollUnseenSound(GameObject doll) {
		return AudioPlayer.PlayContainer("Doll_Voice_Whisper", doll);
	}
}
