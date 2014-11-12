using UnityEngine;
using System.Collections;
using LibPDBinding;

public class AreaTriggers : MonoBehaviour {

	GameObject trigger1;

	public int area = 0;
	int[] areas = new int[2];

	void OnTriggerEnter(Collider trigger1){
		Debug.Log ("caca enteredd");
		}

	// Use this for initialization
	void Start () {
		trigger1 = GameObject.Find("LevelProgress/Trigger1");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
