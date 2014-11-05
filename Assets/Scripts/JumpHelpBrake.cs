using UnityEngine;
using System.Collections;

public class JumpHelpBrake : MonoBehaviour {
	
//	public Transform targetTransform;
	
	public float slowdownVelocityLimit = 25.0f;
	public float helpBrakeStrength = 8000.0f;
	public float forceCurve = 4.0f;
	public float cutoffLimit = 50.0f;

	public bool helperOn = false;

	private float currentBrakeStrength;
	
	void FixedUpdate () {
		
		if (!helperOn) {
			currentBrakeStrength = helpBrakeStrength;
		} else {


			
//			Vector3 targetPoint = targetTransform.position;		
			Vector3 relativeVelocity =  transform.InverseTransformDirection(rigidbody.velocity);
			relativeVelocity.x = 0;
			relativeVelocity.y = 0;

			if (relativeVelocity.z > slowdownVelocityLimit) {
				rigidbody.AddRelativeForce (relativeVelocity * currentBrakeStrength * Time.deltaTime * -1, ForceMode.Force);
			}
			currentBrakeStrength = Mathf.Lerp (currentBrakeStrength, 0, Time.deltaTime * forceCurve);
			if (currentBrakeStrength < cutoffLimit) helperOn = false;
		}
	}
}