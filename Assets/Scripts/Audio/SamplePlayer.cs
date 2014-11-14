using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using LibPDBinding;

public class SamplePlayer : MonoBehaviour {

	readonly List<int> notesToPlay = new List<int>();
	readonly List<float> velocitiesToPlay = new List<float>();
	
	void Awake() {
		LibPD.Subscribe("music_piano_midi");
		LibPD.List += ReceiveList;
	}
	
	void Update() {
		for (int i = notesToPlay.Count; i > 0; i--) {
			Logger.Log("ReceiveList", notesToPlay[0], velocitiesToPlay[0]);
			if (velocitiesToPlay[0] > 0) {
				Logger.Log(Sampler.Play("Piano", notesToPlay.Pop(), velocitiesToPlay.Pop()));
			}
			else {
				Sampler.Play("Piano", notesToPlay.Pop(), velocitiesToPlay.Pop());
			}
		}
	}
	
	void OnApplicationQuit() {
		LibPD.List -= ReceiveList;
		LibPD.Unsubscribe("music_piano_midi");
	}
	
	void ReceiveList(string sendName, object[] values) {
		if (sendName == "music_piano_midi") {
			notesToPlay.Add((int)(float)values[0]);
			velocitiesToPlay.Add((float)values[1]);
		}
	}
}