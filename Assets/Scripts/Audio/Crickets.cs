using UnityEngine;
using System.Collections;
using LibPDBinding;

[System.Serializable]
public class Crickets {

	[SerializeField, PropertyField]
	float crickets1_mul = 0.3F;
	public float Crickets1_mul {
		get {
			return crickets1_mul;
		}
		set {
			crickets1_mul = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float crickets2_mul = 0.1F;
	public float Crickets2_mul {
		get {
			return crickets2_mul;
		}
		set {
			crickets2_mul = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float crickets3_mul;
	public float Crickets3_mul {
		get {
			return crickets3_mul;
		}
		set {
			crickets3_mul = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float crickets4_mul;
	public float Crickets4_mul {
		get {
			return crickets4_mul;
		}
		set {
			crickets4_mul = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float cricketsAll_mul = 1.0F;
	public float CricketsAll_mul {
		get {
			return cricketsAll_mul;
		}
		set {
			cricketsAll_mul = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float crickets_reverb_dryWet = 0.7F;
	public float Crickets_reverb_dryWet {
		get {
			return crickets_reverb_dryWet;
		}
		set {
			crickets_reverb_dryWet = value;
			Update();
		}
	}
	
	public void Update() {
		if (Application.isPlaying) {
			PDPlayer.SendValue("crickets1_mul", Crickets1_mul);
			PDPlayer.SendValue("crickets2_mul", Crickets2_mul);
			PDPlayer.SendValue("crickets3_mul", Crickets3_mul);
			PDPlayer.SendValue("crickets4_mul", Crickets4_mul);
			PDPlayer.SendValue("cricketsAll_mul", CricketsAll_mul);
			PDPlayer.SendValue("crickets_reverb_dry-wet", Crickets_reverb_dryWet);
		}
	}
}
