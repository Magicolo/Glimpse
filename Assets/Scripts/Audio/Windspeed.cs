using UnityEngine;
using System.Collections;
using LibPDBinding;

public class Windspeed : MonoBehaviour {

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
}
	
	// Use this for initialization
	void Start () {
	
	}
		void Update () {
			WindSpeed ();
		}
		
		public float getWindSpeed () {
			return windSpeed;
		}
}