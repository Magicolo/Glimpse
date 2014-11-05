using UnityEngine;
using System.Collections;

public class SuperJumpActivator : MonoBehaviour {

	public Vector3 jumpVelocityBoost = new Vector3 (0f, 50f, 0f);
	public PlayerMove playerJumper;

	// Use this for initialization
	public void OnTriggerEnter (Collider player) {
		playerJumper.Jump (jumpVelocityBoost);
	}
}
