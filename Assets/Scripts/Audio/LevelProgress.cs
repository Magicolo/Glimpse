using UnityEngine;
using System.Collections;
using LibPDBinding;

public class LevelProgress: MonoBehaviour {
	
	public GlimpseCamera glimpseCam;


	float[] glimpseDistance = new float[2];
	int countFrames = 0;
	
	void OnGUI() {
		
	} 
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		countFrames += 1;

		if (countFrames % 15 == 0) {
			glimpseDistance[0] = glimpseCam.glimpseDistance;

			if(glimpseDistance[1] - glimpseDistance[0] > 0.05 || glimpseDistance[0] - glimpseDistance[1] > 0.05){
				PDPlayer.SendValue ("glimpse_distance", glimpseCam.glimpseDistance);
			}

			glimpseDistance[1] = glimpseDistance[0];

		}
	}
}
