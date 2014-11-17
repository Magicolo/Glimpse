using UnityEngine;
using System.Collections;
using LibPDBinding;

public class AreaTriggers : MonoBehaviour {



	public int area = 0;
	int[] areas = new int[2]{0, 1}; // 0 old, 1 new
	int a = 1;

	void OnTriggerEnter(Collider other){
		if (collider.name == "Trigger_1") {
			if (areas [0] < areas [1]) {
					areas [1] = areas [0];
					areas [0] = 1;
			} else {
					areas [1] = areas [0];
					areas [0] = 0;
			}
			area = areas [0];
			PDPlayer.SendValue ("glimpse_area", area);
		}
		if (collider.name == "Trigger_2") {
			areas[0] = (a % 2) + 1;
			area = areas[0];
			PDPlayer.SendValue("glimpse_area", area);
			a += 1;
		}
		if (collider.name == "Trigger_3") {
			area = 3;
			PDPlayer.SendValue("glimpse_area", area);
		}
		if (collider.name == "Trigger_4") {
			area = 4;
			PDPlayer.SendValue("glimpse_area", area);
		}

		if (collider.name == "Trigger_5") {
			area = 5;
			PDPlayer.SendValue("glimpse_area", area);
		}
	}

	void OnTriggerExit(Collider other){
		if (collider.name == "Trigger_3") {
			area = areas[1];
				PDPlayer.SendValue("glimpse_area", area);
		}
		if (collider.name == "Trigger_5") {
			area = areas[1];
			PDPlayer.SendValue("glimpse_area", area);
		}
	}
}

