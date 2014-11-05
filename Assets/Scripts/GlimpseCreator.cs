using UnityEngine;
using System.Collections;

public class GlimpseCreator : MonoBehaviour {

	public Camera glimpseCam;

	// Use this for initialization
	void Start () {
		glimpseCam.Render ();
	}
}