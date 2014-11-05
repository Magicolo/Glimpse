using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioHierarchyManager {

		public string audioClipsPath;
		public bool updateHierarchy = true;
		
		public AudioOptions[] audioOptions;
		public AudioClip[] currentAudioClips;
		public AudioClip[] audioClips;
		public List<GameObject> folderStructure = new List<GameObject>();
		
		public AudioPlayer audioPlayer;

		public void Initialize(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			
			if (Application.isPlaying) {
				audioPlayer.SetChildrenActive(false);
			}
		}
		
		public void Update() {
			FreezeTransforms();
		}

		public void UpdateHierarchyToggle() {
			if (updateHierarchy) {
				UpdateHierarchy();
			}
			updateHierarchy = !updateHierarchy;
		}
		
		public void UpdateAudioOptions() {
			audioOptions = audioPlayer.GetComponentsInChildren<AudioOptions>();
			
			foreach (AudioOptions options in audioOptions) {
				options.Update();
			}
		}
		
		public void UpdateHierarchy() {
			if (audioPlayer == null){
				return;
			}
			
			UpdateAudioOptions();
			SetCurrentAudioClips();
			CreateHierarchy();
			RemoveEmptyFolders();
			
			audioPlayer.gameObject.SortChildrenRecursive();
		}

		public void CreateHierarchy() {
			#if UNITY_EDITOR
			foreach (AudioClip audioClip in audioClips) {
				string audioClipPath = UnityEditor.AssetDatabase.GetAssetPath(audioClip).TrimStart(("Assets/Resources/" + audioClipsPath).ToCharArray());
				string audioClipDirectory = Path.GetDirectoryName(audioClipPath);
				GameObject parent = GetOrAddFolder(audioClipDirectory);
				GameObject child = audioPlayer.gameObject.FindChildRecursive(audioClip.name);
				if (child == null) {
					child = new GameObject(audioClip.name);
					AudioOptions options = child.GetOrAddComponent<AudioOptions>();
					AudioSource source = options.GetOrAddComponent<AudioSource>();
					
					source.playOnAwake = false;
					source.clip = audioClip;
					options.audioInfo = new AudioInfo(source, options, audioPlayer);
					options.audioPlayer = audioPlayer;
				}
				child.transform.parent = parent.transform;
				child.transform.Reset();
			}
			#endif
		}
		
		public GameObject GetOrAddFolder(string directory) {
			string[] folderNames = directory.Split(Path.AltDirectorySeparatorChar);
			GameObject parent = audioPlayer.gameObject;
			GameObject folder = audioPlayer.gameObject;
			
			foreach (string folderName in folderNames) {
				if (string.IsNullOrEmpty(folderName)) {
					continue;
				}
				
				folder = parent.FindChild(folderName);
				if (folder == null) {
					folder = new GameObject(folderName);
					folder.transform.parent = parent.transform;
					folderStructure.Add(folder);
				}
				parent = folder;
			}
			return parent;
		}

		public void RemoveEmptyFolders() {
			foreach (GameObject folder in folderStructure.ToArray()) {
				if (folder != null) {
					if (folder.transform.childCount == 0) {
						RemoveEmptyFolder(folder);
					}
				}
			}
		}
		
		public void RemoveEmptyFolder(GameObject folder) {
			Transform parent = folder.transform.parent;
			
			if (parent != null && parent.childCount == 1 && parent != audioPlayer.transform) {
				folderStructure.Remove(folder);
				RemoveEmptyFolder(folder.transform.parent.gameObject);
			}
			else {
				folderStructure.Remove(folder);
				folder.Remove();
			}
		}

		public void SetCurrentAudioClips() {
			audioClips = Resources.LoadAll<AudioClip>(audioClipsPath);
			currentAudioClips = new AudioClip[audioOptions.Length];
			
			for (int i = 0; i < audioOptions.Length; i++) {
				if (audioOptions[i] != null) {
					currentAudioClips[i] = audioOptions[i].Source.clip;
				}
			}
		}
		
		public void FreezeTransforms() {
			audioPlayer.transform.hideFlags = HideFlags.HideInInspector;
			audioPlayer.transform.Reset();
			
			if (audioOptions != null) {
				foreach (AudioOptions options in audioOptions) {
					if (options != null) {
						options.FreezeTransform();
					}
				}
			}
			
			foreach (GameObject gameObject in folderStructure) {
				if (gameObject != null) {
					gameObject.transform.hideFlags = HideFlags.HideInInspector;
					gameObject.transform.Reset();
				}
			}
		}
	}
}
