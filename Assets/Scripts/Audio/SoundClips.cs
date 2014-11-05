using UnityEngine;
using System.Collections;

public class SoundClips : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AudioPlayer.Play("AMB_Pond_WaterfallLoop", GameObject.Find ("Waterfall"));	
		AudioPlayer.Play("AMB_Pond_WavesLoop", GameObject.Find ("Waves"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
