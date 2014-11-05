using UnityEngine;
using System.Collections;
using LibPDBinding;

public class Wind : MonoBehaviour {

	//wind parameters
	public float wind_BG_freqBase = 100.0F;
	public float wind_BG_freqAmbitus = 200.0F;
	public float wind_BG_Q = 10.0F;
	public float wind_BG_hpfFreq = 100.0F;
	public float wind_BG_noiseAdd = 0.05F;
	public float wind_BG_bp1Freq = 200.0F;
	public float wind_BG_bp1Q = 1.0F;
	public float wind_BG_bp1Mul = 0.0F;
	public float wind_BG_bp2Freq = 400.0F;
	public float wind_BG_bp2Q = 10.0F;
	public float wind_BG_bp2Mul = 1.0F;
	public float wind_BG_bp3Freq = 1600.0F;
	public float wind_BG_bp3Q = 5.0F;
	public float wind_BG_bp3Mul = 1.0F;
	public float wind_BG_mul = 1.0F;
	public float wind_WH_freqBase = 100.0F;
	public float wind_WH_freqAmbitus = 500.0F;
	public float wind_WH_Q = 500.0F;
	public float wind_WH_add = 0.0F;
	public float wind_WH_mul = 4.0F;
	public float wind_WH_octRatio = 2.0F;
	public float wind_WH_octDamp = 0.5F;
	public float wind_WH_randVal = 10000.0F;
	public float wind_leaves_mul = 0.15F;

	//wind DSP
	public float wind_reverb_dryWet = 0.8F;

	void WindParameters() {
		LibPD.SendFloat("wind_reverb_dry-wet", wind_reverb_dryWet);

		LibPD.SendFloat("wind_BG_freqBase", wind_BG_freqBase);
		LibPD.SendFloat("wind_BG_freqAmbitus", wind_BG_freqAmbitus);
		LibPD.SendFloat("wind_BG_Q", wind_BG_Q);
		LibPD.SendFloat("wind_BG_hpfFreq", wind_BG_hpfFreq);
		LibPD.SendFloat("wind_BG_noiseAdd", wind_BG_noiseAdd);
		LibPD.SendFloat("wind_BG_bp1Freq", wind_BG_bp1Freq);
		LibPD.SendFloat("wind_BG_bp1Q", wind_BG_bp1Q);
		LibPD.SendFloat("wind_BG_bp1Mul", wind_BG_bp1Mul);
		LibPD.SendFloat("wind_BG_bp2Freq", wind_BG_bp2Freq);
		LibPD.SendFloat("wind_BG_bp2Q", wind_BG_bp2Q);
		LibPD.SendFloat("wind_BG_bp2Mul", wind_BG_bp2Mul);
		LibPD.SendFloat("wind_BG_bp3Freq", wind_BG_bp3Freq);
		LibPD.SendFloat("wind_BG_bp3Q", wind_BG_bp3Q);
		LibPD.SendFloat("wind_BG_bp3Mul", wind_BG_bp3Mul);
		LibPD.SendFloat("wind_BG_mul", wind_BG_mul);
		LibPD.SendFloat("wind_WH_freqBase", wind_WH_freqBase);
		LibPD.SendFloat("wind_WH_freqAmbitus", wind_WH_freqAmbitus);
		LibPD.SendFloat("wind_WH_Q", wind_WH_Q);
		LibPD.SendFloat("wind_WH_add", wind_WH_add);
		LibPD.SendFloat("wind_WH_mul", wind_WH_mul);
		LibPD.SendFloat("wind_WH_octRatio", wind_WH_octRatio);
		LibPD.SendFloat("wind_WH_octDamp", wind_WH_octDamp);
		LibPD.SendFloat("wind_WH_randVal", wind_WH_randVal);
		LibPD.SendFloat("wind_leaves_mul", wind_leaves_mul);
		LibPD.SendFloat("Wind_Play", 1);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		WindParameters();
	}

}
