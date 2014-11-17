using UnityEngine;
using System.Collections;
using LibPDBinding;

[System.Serializable]
public class LibPDControls {
	
	public void Start() {
		PDPlayer.OpenPatch("Glimpse_MASTER");

		PDPlayer.Play("Wind");
		PDPlayer.Play("Music");
		PDPlayer.Play("Crickets");
		PDPlayer.Play ("Glimpses");

		//controle crickets et wind
		PDPlayer.Play("AMB");
	}
}
