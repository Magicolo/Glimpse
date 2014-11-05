using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Magicolo.GeneralTools;

public static class HelperFunctions {

	public static float MidiToFrequency(float note) {
		return Mathf.Pow(2, (note - 69) / 12) * 440;
	}
		
	public static float Hypotenuse(float a) {
		return Hypotenuse(a, a);
	}
	
	public static float Hypotenuse(float a, float b) {
		return Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
	}
	
	public static string GetFolderPath(string folderName) {
		string folderPath = "";
		
		#if UNITY_EDITOR
		foreach (string path in UnityEditor.AssetDatabase.GetAllAssetPaths()) {
			if (path.EndsWith(folderName)) {
				folderPath = path;
				break;
			}
		}
		#endif
		
		return folderPath;
	}
	
	public static T GetAssetInFolder<T>(string assetFileName, string folderName) where T : Object {
		T asset = default(T);
		
		#if UNITY_EDITOR
		asset = UnityEditor.AssetDatabase.LoadAssetAtPath(GetFolderPath(folderName) + Path.AltDirectorySeparatorChar + assetFileName, typeof(T)) as T;
		#endif
		
		return asset;
	}

	public static string GetAssetPath(string assetName) {
		string assetPath = "";
		
		#if UNITY_EDITOR
		foreach (string path in UnityEditor.AssetDatabase.GetAllAssetPaths()) {
			if (path.EndsWith(assetName)){
				assetPath = path;
				break;
			}
		}
		#endif
		
		return assetPath;
	}
	
	public static string ColorChannelsToVectorAxis(string channels) {
		channels = channels.Replace('R', 'X');
		channels = channels.Replace('G', 'Y');
		channels = channels.Replace('B', 'Z');
		channels = channels.Replace('A', 'W');
		return channels;
	}
	
	public static string VectorAxisToColorChannels(string channels) {
		channels = channels.Replace('X', 'R');
		channels = channels.Replace('Y', 'G');
		channels = channels.Replace('Z', 'B');
		channels = channels.Replace('W', 'A');
		return channels;
	}

	public static T WeightedRandom<T>(Dictionary<T, float> objectsAndWeights) {
		T[] objectList = new T[objectsAndWeights.Keys.Count];
		float[] weightList = new float[objectsAndWeights.Values.Count];
		
		objectsAndWeights.GetOrderedKeysValues(out objectList, out weightList);
		return WeightedRandom<T>(objectList, weightList);
	}

	public static T WeightedRandom<T>(IList<T> objectList, IList<float> weightList) {
		float[] weights = new float[weightList.Count];
		float weightSum = 0;
		float randomValue = 0;
		
		for (int i = 0; i < weights.Length; i++) {
			weightSum += weightList[i];
			weights[i] = weightSum;
		}
		
		randomValue = Random.Range(0, weightSum);
		
		for (int i = 0; i < weights.Length; i++) {
			if (randomValue < weights[i]) {
				return objectList[i];
			}
		}
		return default(T);
	}

	public static float ProportionalRandomRange(float minValue, float maxValue) {
		return Mathf.Pow(2, Random.Range(Mathf.Log(minValue, 2), Mathf.Log(maxValue, 2)));
	}
}
