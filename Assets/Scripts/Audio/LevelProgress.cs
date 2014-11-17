using UnityEngine;
using System.Collections;
using LibPDBinding;

public class LevelProgress: MonoBehaviour {
	
	public GlimpseCamera glimpseCam;
	
	float[] glimpseDistance = new float[2];
	int glimpsingIn = 0;
	int countFrames = 0;
	
	void AttachScript(){
		GameObject[] triggers = new GameObject[5];

		for (int i = 0; i < triggers.Length; i++) 
		{
			triggers [i] = GameObject.Find (string.Format ("LevelProgress/Trigger_{0}", i + 1));
			triggers[i].AddComponent("AreaTriggers");
		}
	}

	void GlimpseDistance(){
		glimpseDistance[0] = glimpseCam.glimpseDistance;
		
		if(glimpseDistance[1] - glimpseDistance[0] > 0.05 || glimpseDistance[0] - glimpseDistance[1] > 0.05)
		{
			PDPlayer.SendValue ("glimpse_distance", glimpseCam.glimpseDistance);
		}
		
		glimpseDistance[1] = glimpseDistance[0];
	}

	void GlimpsingIn(){

		if (glimpseCam.glimpsingIn == true && glimpseCam.interpolationOn == true && glimpsingIn != 1) {
			glimpsingIn = 1;
			PDPlayer.SendValue("glimpse_glimpsingIn", glimpsingIn);
		}
		if(glimpseCam.interpolationOn == false && glimpsingIn != 0){
			glimpsingIn = 0;
			PDPlayer.SendValue("glimpse_glimpsingIn", glimpsingIn);
			}
		}
	
	// Use this for initialization
	void Start () {
		AttachScript ();

	}
	
	// Update is called once per frame
	void Update () {


		countFrames += 1;

		GlimpsingIn ();

		if (countFrames % 15 == 0) {
			GlimpseDistance ();
		}
	}
}
