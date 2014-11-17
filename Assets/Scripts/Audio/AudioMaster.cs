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
	public WaterFalls waterFalls;
	
	static AudioMaster instance;
	static AudioMaster Instance {
		get {
			instance = instance ?? FindObjectOfType<AudioMaster>();
			return instance;
		}
	}
	
	public void Start() {
		libpdControls.Start();
		waterFalls.Start();

		wind.Update();
		crickets.Update();
	}

	public static AudioItem Play(string soundName, GameObject source = null) {
		return AudioPlayer.Play(soundName, source);
	}
	
	public static AudioItem PlayContainer(string containerName, GameObject source = null) {
		return AudioPlayer.PlayContainer(containerName, source);
	}
	
	public static void PlayPlayerFootstep(FootstepActions footstepAction) {
		Instance.footstepAction = footstepAction;
		AudioPlayer.PlayContainer("Player_Footstep", Instance.playerAudio.gameObject);
	}

	public static void PlayDollUnseenSound(GameObject doll) {
		PDPlayer.PlayContainer("Dolls", "Doll_Voice_Whisper", doll);
	}

}
