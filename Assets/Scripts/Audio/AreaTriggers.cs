using UnityEngine;
using System.Collections;
using LibPDBinding;

public class AreaTriggers : MonoBehaviour {

	GameObject trigger1;

	public int area = 0;
	int[] areas = new int[2]{0, 1}; // 0 old, 1 new

	void OnTriggerEnter(Collider trigger1){
		if (areas [0] < areas [1]){
			areas[1] = areas[0];
			areas[0] = 1;
		}
		else {
			areas[1] = areas[0];
			areas[0] = 0;
		}
		area = areas[0];
		PDPlayer.SendValue("glimpse_area", area);
	}

	// Use this for initialization
	void Start () {
		trigger1 = GameObject.Find("LevelProgress/Trigger1");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
