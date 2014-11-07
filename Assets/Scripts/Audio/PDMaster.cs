using UnityEngine;
using System.Collections;
using LibPDBinding;

[System.Serializable]
public class PDMaster {

	[SerializeField, PropertyField]
	float insectMaster = 1.0F;
	public float InsectMaster {
		get {
			return insectMaster;
		}
		set {
			insectMaster = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float critters = 1.0F;
	public float Critters {
		get {
			return critters;
		}
		set {
			critters = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float critters2 = 1.0F;
	public float Critters2 {
		get {
			return critters2;
		}
		set {
			critters2 = value;
			Update();
		}
	}

	[SerializeField, PropertyField]
	float chirper = 0.5F;
	public float Chirper {
		get {
			return chirper;
		}
		set {
			chirper = value;
			Update();
		}
	}
	
	public void Update() {
		if (Application.isPlaying) {
			PDPlayer.SendValue("critters", InsectMaster * Critters);
			PDPlayer.SendValue("critters2", InsectMaster * Critters2);
			PDPlayer.SendValue("chirper", InsectMaster * Chirper);
		}
	}
}
