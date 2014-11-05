using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {

	public AudioItem PlayPlayerFootstep(AudioMaster.FootstepActions action) {
		return AudioMaster.PlayPlayerFootstep(action);;
	}
}
