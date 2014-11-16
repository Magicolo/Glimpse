using UnityEngine;
using System.Collections;

public class GlimpseCamera : MonoBehaviour {
	public Transform target;
	public float distance = 1.5f;
	public float cameraTension = 1.0f;
	public PlayerMove playerMoveScript;
	
	public float xRotationSpeed = 240.0f;
	public float yRotationSpeed = 123.0f;
	
	public int yAngleMinLimit = 0;
	public int yAngleMaxLimit = 90;
	
	public float minDistance = 0.6f;
	public float maxDistance = 2.0f;
	
	private float x = 0.0f;
	private float y = 0.0f;

	public float interpolateSpeed = 2.0f;
	
	public Transform returnLocation;
	//public bool returnToPlayerOn = true;
	public bool interpolationOn = false;
	public bool completed = true;
	public bool glimpsingIn = true;
	public Transform playerCameraLocation;
	public float returnSnapDistance = 0.02f;
	public float returnSnapAngle = 0.02f;
	public float glimpseDistance = 0;
	
	public GlimpseController[] glimpses;
	public Vignetting vignettingEffect;
	public float vignetteAmount;
	public float vignetteSpeed;
	public float vignetteCutoff;

	public string[] avoidClippingTags;	
	
	public void Start () {
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
	
	public void Update () {

		if (!completed && glimpsingIn)
			vignettingEffect.intensity = Mathf.Lerp (vignettingEffect.intensity, vignetteAmount, vignetteSpeed * TimeFabric.DeltaTime(false));
		else
			vignettingEffect.intensity = Mathf.Lerp (vignettingEffect.intensity, 0, vignetteSpeed * TimeFabric.DeltaTime(false));

		if (vignettingEffect.intensity < vignetteCutoff)
						vignettingEffect.intensity = 0;

			// Go through the list of glimpses to check if any are in range and the player is firing glimpse button		
			// 	

		foreach (GlimpseController glimpse in glimpses) {
						if (Input.GetButton ("Glimpse") && !glimpse.interpolationOn) {
								returnLocation.position = playerCameraLocation.position;
								returnLocation.rotation = playerCameraLocation.rotation;
								checkGlimpse (glimpse);
						}
				
						if (glimpse.interpolationOn) {
								interpolationOn = true;
								completed = false;
								playerMoveScript.controlsFrozen = true;
								interpolateGlimpse (glimpse);	
						}
			}

			float a = Input.GetAxis ("LeftStickHorizontal");
			float b = Input.GetAxis ("LeftStickVertical"); 

			if (completed && !(a == 0 && b == 0)) interpolationOn = false;

			if (target && (interpolationOn == false)) {
			// Set movement based on right stick if the camera is not interpolating to a glimpse	
				x += Input.GetAxis ("RightStickHorizontal") * xRotationSpeed * TimeFabric.DeltaTime(false);
				y -= Input.GetAxis ("RightStickVertical") * yRotationSpeed * TimeFabric.DeltaTime(false);

				y = ClampAngle (y, yAngleMinLimit, yAngleMaxLimit);

				
				
			//rotate
			Quaternion rotation = Quaternion.Euler (y, x, 0.0f);
			Vector3 position = rotation * new Vector3 (0.0f, 0.0f, -distance) + target.position;


			//where should the camera be next frame?
			Vector3 nextFramePosition = Vector3.Lerp (transform.position, position, cameraTension * TimeFabric.DeltaTime(false));
			Vector3 direction = nextFramePosition - target.position;
			//raycast to this position
			RaycastHit hit;
			if(Physics.Raycast (target.position, direction, out hit, direction.magnitude + 0.3f) && avoidClippingTags.Length > 0)
			{
				transform.position = nextFramePosition;
				foreach(string tag in avoidClippingTags)
					if(hit.transform.tag == tag)
						transform.position = hit.point - direction.normalized * 0.3f;
						transform.rotation = Quaternion.Lerp (transform.rotation, rotation, cameraTension * TimeFabric.DeltaTime(false));
			}
			else
			{
				//otherwise, move cam to intended position

				transform.position = nextFramePosition;
				transform.rotation = Quaternion.Lerp (transform.rotation, rotation, cameraTension * TimeFabric.DeltaTime(false));
			}
			}
		}
	
	//clamp camera angle
	public static float ClampAngle (float angle, float min, float max) {
		if (angle < -360.0f)
			angle += 360.0f;
		if (angle > 360.0f)
			angle -= 360.0f;
		return Mathf.Clamp (angle, min, max);
	}

	void checkGlimpse (GlimpseController glimpse) {
		// Check if glimpse is within distance and angle range to be activated, and start interpolation if that's the case
		
		float glimpseDist = Vector3.Distance(glimpse.projectorLocation.position, playerCameraLocation.position);
		float glimpseAngle = Quaternion.Angle (glimpse.projectorLocation.rotation, playerCameraLocation.rotation);
		
		
		
		if (glimpseDist < glimpse.glimpseRange && glimpseAngle < glimpse.glimpseAngle && !glimpse.interpolationOn && !glimpse.transformComplete) {
			glimpse.interpolationOn = true;
		}
	}
	
	void interpolateGlimpse (GlimpseController glimpse) {

		// Move the camera towards the precise glimpse position
		if (glimpsingIn) {
			transform.position = Vector3.Lerp (transform.position, glimpse.projectorLocation.position, interpolateSpeed * TimeFabric.DeltaTime(false));
			transform.rotation = Quaternion.Slerp (transform.rotation, glimpse.projectorLocation.rotation, interpolateSpeed * TimeFabric.DeltaTime(false));
		}
		else {
			transform.position = Vector3.Lerp (transform.position, returnLocation.position, interpolateSpeed * TimeFabric.DeltaTime(false));
			transform.rotation = Quaternion.Slerp (transform.rotation, returnLocation.rotation, interpolateSpeed * TimeFabric.DeltaTime(false));
		}

		// When the glimpse gets to the right location the kick in the glimpse realisation effects
		if ((Vector3.Distance (glimpse.projectorLocation.position, transform.position) < glimpse.snapDistance) && (Quaternion.Angle (glimpse.projectorLocation.rotation, transform.rotation) < glimpse.snapAngle)) {

			SetLayerRecursively(glimpse.glimpseObjectMesh, 9);
			// interpolationOn = false;
			glimpse.transformComplete = true;
			glimpse.glimpseProjector.enabled = false;
			glimpsingIn = false;

			//turn on collision
			foreach (Collider col in glimpse.glimpseObjectCollider.GetComponentsInChildren<Collider>(true))
			{
				col.enabled = true;
			}

			// Turn on gravity at this point
			Rigidbody glimpseRigidbody = glimpse.glimpseObjectCollider.GetComponent<Rigidbody>();
			if (glimpseRigidbody) {
				glimpseRigidbody.useGravity = true;
			}

		}
		if (!glimpsingIn && (Vector3.Distance (returnLocation.position, transform.position) < glimpse.snapDistance) && (Quaternion.Angle (returnLocation.rotation, transform.rotation) < glimpse.snapAngle)) {
			completed = true;
			glimpsingIn = true;
			glimpse.interpolationOn = false;
			playerMoveScript.controlsFrozen = false;
		}
	}
	

	void SetLayerRecursively(GameObject go, int layerNumber)
	{
		// Used to make sure objects that have parts in children get fully moved into the correct layer, and not just the parent
		foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
		{
			trans.gameObject.layer = layerNumber;
		}
	}
	

}