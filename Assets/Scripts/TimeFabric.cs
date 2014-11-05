using UnityEngine;
using System.Collections;

public class TimeFabric {
	private static float timeScale = 1.0f;
	
	public static void SetTimeScale( float ts ) {
		Time.timeScale = ts;
		timeScale = ts;
	}
	
	public static float DeltaTime( bool affectBySlowTime ) {
		return Time.deltaTime / (affectBySlowTime ? 1f : timeScale);
	}
	
	public static float DeltaTime() {
		return Time.deltaTime;
	}
}
