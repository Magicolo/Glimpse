using UnityEngine;
using System.Collections;
using LibPDBinding;

public class Crickets : MonoBehaviour {

	public float crickets1_mul = 0.3F;
	public float crickets2_mul = 0.1F;
	public float crickets3_mul = 0.0F;
	public float crickets4_mul = 0.0F;
	public float cricketsAll_mul = 1.0F;
	public float crickets_reverb_dryWet = 0.7F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		LibPD.SendFloat("crickets1_mul", crickets1_mul);
		LibPD.SendFloat("crickets2_mul", crickets2_mul);
		LibPD.SendFloat("crickets3_mul", crickets3_mul);
		LibPD.SendFloat("crickets4_mul", crickets4_mul);
		LibPD.SendFloat("cricketsAll_mul", cricketsAll_mul);
		LibPD.SendFloat("crickets_reverb_dry-wet", crickets_reverb_dryWet);
	}
}
