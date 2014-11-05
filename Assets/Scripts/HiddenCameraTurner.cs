using UnityEngine;
using System.Collections;

public class HiddenCameraTurner : MonoBehaviour {
	
	public Camera viewCamera;
	public Transform playerLocation;
	public UnseenActionsTrigger unseenActionsTrigger;
	private bool isCurrentlySeen = true;
	
	// Update is called once per frame
	void Update () {
		Vector3 cameraPosition = viewCamera.WorldToViewportPoint (transform.position);
		if (!(cameraPosition.x > -0.5 && cameraPosition.x < 1.5 && cameraPosition.z > -5)) {
			transform.LookAt (playerLocation.position);

			if (isCurrentlySeen) {
				unseenActionsTrigger.PlayUnseenSound ();
				isCurrentlySeen = false;
			}

		}
		else {
			isCurrentlySeen = true;
		}
	}
}



