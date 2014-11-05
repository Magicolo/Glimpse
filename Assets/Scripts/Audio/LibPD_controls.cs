using UnityEngine;
using System.Collections;
using LibPDBinding;

public class LibPD_controls: MonoBehaviour {
	
	public Windspeed wind;
	public float windSpeed;
	
	void OnGUI() {
		
	} 
	
	// Use this for initialization
	void Start () {
		PDPlayer.OpenPatch("Glimpse_MASTER");

		PDPlayer.Play("Wind");
		PDPlayer.Play("Music");
		PDPlayer.Play ("Crickets");

		//controle crickets et wind
		PDPlayer.Play ("AMB");

	}
	
	// Update is called once per frame
	/*void Update () {
		windSpeed = wind.getWindSpeed();
	}*/
}
