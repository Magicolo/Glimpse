using UnityEngine;
using System.Collections;

[System.Serializable]
public class WaterFalls {

	public GameObject[] waterFalls;
	
	public void Start() {
		foreach (GameObject waterFall in waterFalls) {
			AudioPlayer.Play("AMB_Pond_WaterfallLoop", waterFall);
			AudioPlayer.Play("AMB_Pond_WaterfallLoop_02", waterFall);
		}
	}
}
