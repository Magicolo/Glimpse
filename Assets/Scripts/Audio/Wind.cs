using UnityEngine;
using System.Collections;
using LibPDBinding;

[System.Serializable]
public class Wind {

	public WindParameters windParameters;
	
	public void Update() {
		windParameters.Update();
	}
}
