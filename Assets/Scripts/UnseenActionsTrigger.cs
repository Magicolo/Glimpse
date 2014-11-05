using UnityEngine;
using System.Collections;

public class UnseenActionsTrigger : MonoBehaviour {

	public void PlayUnseenSound () {
		AudioPlayer.PlayContainer("Doll_Voice_Whisper", gameObject);
	}
}
