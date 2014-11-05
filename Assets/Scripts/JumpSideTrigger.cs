using UnityEngine;
using System.Collections;

public class JumpSideTrigger : MonoBehaviour {

	public JumpHelpSide playerJumpHelper;
	// public Transform target;

	public void OnTriggerEnter (Collider player) {
		if (player.CompareTag ("Player")) { 
			playerJumpHelper.targetTransform = transform;
			playerJumpHelper.helperOn = true;
		}
	}
}