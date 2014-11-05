using UnityEngine;
using System.Collections;

public class UnseenActionsTrigger : MonoBehaviour {

	public void PlayUnseenSound() {
		AudioMaster.PlayDollUnseenSound(gameObject);
	}
}
