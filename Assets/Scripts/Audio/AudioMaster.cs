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
		Water
	}
	
	public PlayerAudio playerAudio;
	
	[SerializeField, PropertyField]
	FootstepActions footstepAction;
	public FootstepActions FootstepAction {
		get {
			return footstepAction;
		}
	}
	
	[SerializeField, PropertyField]
	FootstepSurfaces footstepSurface;
	public FootstepSurfaces FootstepSurface {
		get {
			footstepSurface = TextureReader.GetFootstepSurface();
			return footstepSurface;
		}
	}
	
	public LibPDControls libpdControls;
	public Wind wind;
	public Crickets crickets;
	
	static AudioMaster instance;
	static AudioMaster Instance {
		get {
			instance = instance ?? FindObjectOfType<AudioMaster>();
			return instance;
		}
	}
	
	public void Awake() {
		libpdControls.Start();

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
		
		return AudioPlayer.PlayContainer("Player_Footstep", Instance.playerAudio.transform.position);;
	}
	
	public static AudioItem PlayDollUnseenSound(GameObject doll) {
		return AudioPlayer.PlayContainer("Doll_Voice_Whisper", doll);
	}
}
