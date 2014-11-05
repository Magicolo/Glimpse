using UnityEngine;
using System.Collections;

public class JumpBrakeTrigger : MonoBehaviour {
	
	public JumpHelpBrake playerJumpHelper;
//	public Transform target;
	
	public void OnTriggerEnter (Collider player) {
		if (player.CompareTag ("Player")) {
//				playerJumpHelper.targetTransform = target;
				playerJumpHelper.helperOn = true;
		}
	}
}
