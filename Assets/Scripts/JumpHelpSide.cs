using UnityEngine;
using System.Collections;

public class JumpHelpSide : MonoBehaviour {
	
	public Transform targetTransform; // Transform of object being steered towards
	public float helpSideStrength = 300.0f; // Initial strength of force applied
	public float cutoffLimit = 50.0f; // Force cuts off completely when it drops below this level
	public float forceCurve = 4.0f; // Speed at which force reduces over time

	public bool helperOn = false;
	
	private float currentSideStrength;
	
	void FixedUpdate () {

		if (!helperOn) {
			currentSideStrength = helpSideStrength;
		} else {
			
			Vector3 targetPoint = targetTransform.position;			
			Vector3 relativeDirection = transform.InverseTransformDirection(targetPoint - transform.position);			
			Vector3 sideAdjust = new Vector3 (relativeDirection.x, 0, 0); // Direction and strength multiplier for applying force

			rigidbody.AddRelativeForce (sideAdjust * currentSideStrength * Time.deltaTime, ForceMode.Force); // Apply side steering force to player
			currentSideStrength = Mathf.Lerp (currentSideStrength, 0, Time.deltaTime * forceCurve); // Reduce force over time
			if (currentSideStrength < cutoffLimit) helperOn = false;
		}
	}
}