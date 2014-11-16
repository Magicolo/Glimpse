using UnityEngine;
using System.Collections;

public class SoundClips : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AudioPlayer.Play("AMB_Pond_WaterfallLoop", GameObject.Find ("Waterfall_Audio_A_01"));	
		AudioPlayer.Play("AMB_Pond_WaterfallLoop_02", GameObject.Find ("Waterfall_Audio_B_01"));
		AudioPlayer.Play("AMB_Pond_WaterfallLoop", GameObject.Find ("Waterfall_Audio_A_02"));	
		AudioPlayer.Play("AMB_Pond_WaterfallLoop_02", GameObject.Find ("Waterfall_Audio_B_02"));
		//AudioPlayer.Play("AMB_Pond_WavesLoop", GameObject.Find ("Waves"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
