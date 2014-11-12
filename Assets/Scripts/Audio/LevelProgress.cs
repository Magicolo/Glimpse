using UnityEngine;
using System.Collections;
using LibPDBinding;

public class LevelProgress: MonoBehaviour {
	
	public GlimpseCamera glimpseCam;

	public float glimpseDistance;
	
	void OnGUI() {
		
	} 
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		glimpseDistance = glimpseCam.glimpseDistance;

		if (glimpseCam.glimpseDistance > 0.05) {
			PDPlayer.SendValue ("glimpse_distance", glimpseCam.glimpseDistance);
				}
	}
}
