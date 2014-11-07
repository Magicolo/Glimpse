using UnityEngine;
using System.Collections;
using LibPDBinding;

[System.Serializable]
public class WindSpeed {

	//controls for wind speed

	[SerializeField, PropertyField]
	float max = 0.3F;
	public float Max {
		get {
			return max;
		}
		set {
			max = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float min;
	public float Min {
		get {
			return min;
		}
		set {
			min = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float frequency = 0.005F;
	public float Frequency {
		get {
			return frequency;
		}
		set {
			frequency = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float panFrequency = 0.01F;
	public float PanFrequency {
		get {
			return panFrequency;
		}
		set {
			panFrequency = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	int octaves = 3;
	public int Octaves {
		get {
			return octaves;
		}
		set {
			octaves = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float octaveRatio = 2.0F;
	public float OctaveRatio {
		get {
			return octaveRatio;
		}
		set {
			octaveRatio = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float octaveDamping = 0.5F;
	public float OctaveDamping {
		get {
			return octaveDamping;
		}
		set {
			octaveDamping = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float speed;
	public float Speed {
		get {
			return speed;
		}
		set {
			speed = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float wind1;
	public float Wind1 {
		get {
			return wind1;
		}
		set {
			wind1 = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float wind2;
	public float Wind2 {
		get {
			return wind2;
		}
		set {
			wind2 = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float wind3;
	public float Wind3 {
		get {
			return wind3;
		}
		set {
			wind3 = value;
			Update();
		}
	}
	
	public void Update() {
		if (Application.isPlaying) {
			float[] winds = new float[octaves];
			float[] frequencies = new float[octaves];
			float[] dampings = new float[octaves];
			float[] windPans = new float[4];
	
			//windspeed
			for (int i = 0; i < winds.Length; i++) {
				dampings[i] = Mathf.Pow(OctaveDamping, i);
				frequencies[i] = (Frequency * (i + 1)) * OctaveRatio;
				//winds[i] = windspot.distanceFromSpot() * 0.5F + (octDamp * ((min + ((max - min) * Mathf.PerlinNoise(Time.time * freqs[i], 0.0F)))));
				winds[i] = (OctaveDamping * ((Min + ((Max - Min) * Mathf.PerlinNoise(Time.time * frequencies[i], 0.0F)))));
			}
	
			//Panning for whistling wind
			for (int i = 0; i < windPans.Length; i++) {
				windPans[i] = Mathf.PerlinNoise(Time.time * 1.0F, 0.0F);
			}
	
			wind1 = winds[0];
			wind2 = winds[1];
			wind3 = winds[2];
	
			speed = winds[0] + winds[1] + winds[2];
	
			PDPlayer.SendValue("windSpeed", Speed);
			PDPlayer.SendValue("whistle_pan_01", windPans[0]);
			PDPlayer.SendValue("whistle_pan_02", windPans[1]);
			PDPlayer.SendValue("whistle_pan_03", windPans[2]);
			PDPlayer.SendValue("whistle_pan_04", windPans[3]);
		}
	}
}