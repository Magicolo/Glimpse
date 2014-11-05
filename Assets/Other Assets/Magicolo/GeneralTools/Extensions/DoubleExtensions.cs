using UnityEngine;
using System.Collections;

public static class DoubleExtensions {

	public static float Pow(this double d, double power) {
		return Mathf.Pow((float)d, (float)power);
	}
	
	public static float Pow(this double d) {
		return d.Pow(2);
	}
	
	public static double Round(this double d, double step) {
		return step <= 0 ? d : (double)(Mathf.Round((float)(d * (1D / step))) / (1D / step));
	}
	
	public static double Round(this double d) {
		return d.Round(1);
	}
}
