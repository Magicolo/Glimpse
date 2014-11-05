using UnityEngine;
using System.Collections;
using LibPDBinding;

public class PD_Master : MonoBehaviour {

	public Windspot windspot;
	/*
	//wind
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
	public float wind_BG_mul = 4.0F;
	public float wind_WH_freqBase = 100.0F;
	public float wind_WH_freqAmbitus = 500.0F;
	public float wind_WH_Q = 500.0F;
	public float wind_WH_add = 0.0F;
	public float wind_WH_mul = 4.0F;
	public float wind_WH_octRatio = 2.0F;
	public float wind_WH_octDamp = 0.5F;
	public float wind_WH_randVal = 10000.0F;
	public float wind_master_mul = 1.0F;

	//controls for wind speed
	public float max = 0.3F;
	public float min = 0.0F;
	public float freq = 0.005F;
	public float panFreq = 0.01F;
	public int octaves = 3;
	public float octRatio = 2.0F;
	public float octDamp = 0.5F;
	public float windSpeed = 0.0F;
	public float wind1 = 0.0F;
	public float wind2 = 0.0F;
	public float wind3 = 0.0F;
*/

	public float insectMaster = 1.0F;
	public float critters = 1.0F;
	public float critters2 = 1.0F;
	public float chirper = 0.5F;
	/*
	void Wind() {
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
		LibPD.SendFloat("wind_master_mul", wind_master_mul);
	}

	void WindSpeed() {

		float[] winds = new float[octaves];
		float[] freqs = new float[octaves];
		float[] damps = new float[octaves];
		float[] windPans = new float[4];

		//windspeed
		for (int i = 0; i < winds.Length; i++)
		{
			damps[i] = Mathf.Pow(octDamp, i);
			freqs[i] = (freq * (i + 1)) * octRatio;
			//winds[i] = windspot.distanceFromSpot() * 0.5F + (octDamp * ((min + ((max - min) * Mathf.PerlinNoise(Time.time * freqs[i], 0.0F)))));
			winds[i] = (octDamp * ((min + ((max - min) * Mathf.PerlinNoise(Time.time * freqs[i], 0.0F)))));
		}

		//Panning for whistling wind
		for (int i = 0; i < windPans.Length; i++)
		{
			windPans[i] = Mathf.PerlinNoise(Time.time * 1.0F, 0.0F);
		}

		wind1 = winds[0];
		wind2 = winds[1];
		wind3 = winds[2];

		windSpeed = winds[0] + winds[1] + winds[2];

		LibPD.SendFloat("windSpeed", windSpeed);
		LibPD.SendFloat("whistle_pan_01", windPans[0]);
		LibPD.SendFloat("whistle_pan_02", windPans[1]);
		LibPD.SendFloat("whistle_pan_03", windPans[2]);
		LibPD.SendFloat("whistle_pan_04", windPans[3]);
	}*/

	void Insects() {

		LibPD.SendFloat ("critters", insectMaster * critters);
		LibPD.SendFloat ("critters2", insectMaster * critters2);
		LibPD.SendFloat ("chirper", insectMaster * chirper);

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Wind();
		//WindSpeed ();
		Insects();
	
	}

	/*public float getWindSpeed () {
		return windSpeed;
	}*/
}
