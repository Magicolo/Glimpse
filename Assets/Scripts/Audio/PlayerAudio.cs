using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {

	public void PlayPlayerFootstep(AudioMaster.FootstepActions action) {
		 AudioMaster.PlayPlayerFootstep(action);;
	}
}
