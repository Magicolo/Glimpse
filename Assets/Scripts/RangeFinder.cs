using UnityEngine;
using System.Collections;

public class RangeFinder : MonoBehaviour {

	public GameObject playerCamera;
	public GlimpseCamera glimpseCamera;
	public float rangeLimit = 50;

	void Update () {
	}

	void OnTriggerStay (Collider player) {
		Transform camTransform = playerCamera.transform;
		Transform glimpseTransform = transform;
		float posDistance = Vector3.Distance (camTransform.position, glimpseTransform.position);
		float rotAngle = Quaternion.Angle (camTransform.rotation, glimpseTransform.rotation);

		float rotAngleInterpolate = Mathf.InverseLerp (180, 0, rotAngle);
		float posDistanceInterpolate = Mathf.InverseLerp (rangeLimit, 0, posDistance);

		if (GetComponent <Projector> ().enabled)
						glimpseCamera.glimpseDistance = (rotAngleInterpolate + posDistanceInterpolate) / 2;
				else
						glimpseCamera.glimpseDistance = 0;
	}

	void OnTriggerExit (Collider player) {
		glimpseCamera.glimpseDistance = 0;
	}
}
