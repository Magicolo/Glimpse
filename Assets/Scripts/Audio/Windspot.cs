using UnityEngine;
using System.Collections;

public class Windspot : MonoBehaviour {

	public Transform charTransform;
	public float maxDistance = 100.0F;
	public float minDistance = 5.0F;

	public float distanceFromSpot () {
		Vector3 thisPosition = transform.position;
		Vector3 characterPosition = charTransform.position;

		float actualDistance = Vector3.Distance (thisPosition, characterPosition);
		float distanceValue;

		distanceValue = Mathf.InverseLerp (maxDistance, minDistance, actualDistance);
		Debug.Log (distanceValue);
		return distanceValue;
	}

	void Update () {
		distanceFromSpot();
	}
}
