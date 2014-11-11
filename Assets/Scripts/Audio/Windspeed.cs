using UnityEngine;
using System.Collections;
using LibPDBinding;

[System.Serializable]
public class WindSpeed {

	//controls for wind speed

	public GlimpseCamera glimpseCam;

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

	/*[SerializeField, PropertyField]
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
	}*/

	
	public void Update() {
		if (Application.isPlaying) {
			PDPlayer.SendValue("wind_speed_max", Max);
			PDPlayer.SendValue("wind_speed_min", Min);
		}
	}
}