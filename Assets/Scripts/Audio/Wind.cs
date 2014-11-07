using UnityEngine;
using System.Collections;
using LibPDBinding;

[System.Serializable]
public class Wind {

	public WindParameters windParameters;
	public WindSpeed windSpeed;
	
	public void Update() {
		windParameters.Update();
		windSpeed.Update();
	}
}
