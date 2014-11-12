using UnityEngine;
using System.Collections;
using LibPDBinding;

public class SamplePlayer : MonoBehaviour {

	void Awake(){
		LibPD.List += ReceiveList;
	}
	
	void OnApplicationQuit(){
		LibPD.List -= ReceiveList;
	}
	
	void ReceiveList(string sendName, object[] values) {
		if (sendName == "music_piano_midi"){
			Sampler.Play("Piano", (int)values[0], (float)values[1]);
		}
	}
}