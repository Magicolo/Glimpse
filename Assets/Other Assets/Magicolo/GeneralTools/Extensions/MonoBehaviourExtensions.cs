using UnityEngine;
using System.Collections;

public static class MonoBehaviourExtensions {

	public static void SetExecutionOrder(this MonoBehaviour behaviour, int order) {
		#if UNITY_EDITOR
		foreach (UnityEditor.MonoScript s in UnityEditor.MonoImporter.GetAllRuntimeMonoScripts()) {
			if (s.name == behaviour.GetType().Name){
				if (UnityEditor.MonoImporter.GetExecutionOrder(s) != order){
					UnityEditor.MonoImporter.SetExecutionOrder(s, order);
				}
			}
		}
		#endif
	}
	
	public static void SetTransformHasChanged(this MonoBehaviour behaviour, Transform transform, bool hasChanged) {
		behaviour.StartCoroutine(SetHasChanged(transform, hasChanged));
	}
	
	static IEnumerator SetHasChanged(Transform transform, bool hasChanged) {
		yield return new WaitForEndOfFrame();
		transform.hasChanged = hasChanged;
	}

	public static void InvokeMethod(this MonoBehaviour behaviour, string methodName, float delay, params object[] arguments) {
		behaviour.StartCoroutine(InvokeDelayed(behaviour, methodName, delay, arguments));
	}
	
	static IEnumerator InvokeDelayed(MonoBehaviour behaviour, string methodName, float delay, params object[] arguments) {
		yield return new WaitForSeconds(delay);
		behaviour.InvokeMethod(methodName, arguments);
	}
	
	public static void InvokeMethodRepeating(this MonoBehaviour behaviour, string methodName, float delay, float repeatRate, params object[] arguments) {
		behaviour.StartCoroutine(InvokeDelayedRepeating(behaviour, methodName, delay, repeatRate, arguments));
	}
	
	static IEnumerator InvokeDelayedRepeating(MonoBehaviour behaviour, string methodName, float delay, float repeatRate, params object[] arguments) {
		yield return new WaitForSeconds(delay);
		
		while (behaviour != null && behaviour.enabled) {
			behaviour.InvokeMethod(methodName, arguments);
			yield return new WaitForSeconds(repeatRate);
		}
	}
	
}
