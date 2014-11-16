using UnityEngine;
using System.Collections;

public class WindController : MonoBehaviour {

	private Component windZone;

	public GlimpseCamera glimpseCamera;

	public float baseMain = 0;
	public float baseTurbulence = 0;
	public float basePulseMagnitude = 0;
	public float basePulseFrequency = 0;

	public float highMain = 0;
	public float highTurbulence = 0;
	public float highPulseMagnitude = 0;
	public float highPulseFrequency = 0;

	public float interpolationSpeed = 1;

	void Start () {
		windZone = this.GetComponent ("WindZone");
	}
	
	// Update is called once per frame
	void Update () { 
		float glimpseDistance = glimpseCamera.glimpseDistance;

		float targetMain = Mathf.Lerp (baseMain, highMain, glimpseDistance);
		float targetTurbulence = Mathf.Lerp (baseTurbulence, highTurbulence, glimpseDistance);
		float targetPulseMagnitude = Mathf.Lerp (basePulseMagnitude, highPulseMagnitude, glimpseDistance);
		float targetPulseFrequency = Mathf.Lerp (basePulseFrequency, highPulseFrequency, glimpseDistance);


		windZone.SetValueToMember ("windTurbulence",
			Mathf.Lerp
		        ((float)windZone.GetValueFromMember ("windTurbulence"),
		 		targetTurbulence,
			    interpolationSpeed * TimeFabric.DeltaTime(false) ) );

		windZone.SetValueToMember ("windMain",
			Mathf.Lerp
		        ((float)windZone.GetValueFromMember ("windMain"),
		 		targetMain,
		 		interpolationSpeed * TimeFabric.DeltaTime(false) ) );

		windZone.SetValueToMember ("windPulseMagnitude",
			Mathf.Lerp
		        ((float)windZone.GetValueFromMember ("windPulseMagnitude"),
		 		targetPulseMagnitude,
		 		interpolationSpeed * TimeFabric.DeltaTime(false) ) );

		windZone.SetValueToMember ("windPulseFrequency",
			Mathf.Lerp
		        ((float)windZone.GetValueFromMember ("windPulseFrequency"),
		 		targetPulseFrequency,
		 		interpolationSpeed * TimeFabric.DeltaTime(false) ) );


	}
}
