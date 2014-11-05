using UnityEngine;
using System.Collections;

public class GlimpseController : MonoBehaviour {

	public Transform projectorLocation;
	public bool transformComplete = false;
	public GameObject glimpseObjectMesh;
	public GameObject glimpseObjectCollider;
	public bool interpolationOn = false;
	public ParticleSystem glimpseFX;
	public bool inVolume;
	public Transform playerCamLocation;
	public Transform glimpseCam;
	public Projector glimpseProjector;
	public Camera effectsCam;
	public Color distantColor;
	public Color nearColor;

	public float glimpseRange = 1.0f;
	public float glimpseAngle = 35.0f;
	public float snapDistance = 0.02f;
	public float snapAngle = 0.02f;
	public float glowSpeed = 5.0f;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		//if ((Vector3.Distance (playerCamLocation.position, glimpseCam.position) < glimpseRange) && (Quaternion.Angle (playerCamLocation.rotation, glimpseCam.rotation) < glimpseAngle)) {
		//	effectsCam.backgroundColor = Color.Lerp (effectsCam.backgroundColor, nearColor, glowSpeed * TimeFabric.DeltaTime(false));
		//} else {
		//	effectsCam.backgroundColor = Color.Lerp (effectsCam.backgroundColor, distantColor, glowSpeed * TimeFabric.DeltaTime(false));
		//}
	}
}
