using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioHierarchyManager {

		public string audioClipsPath;
		
		public AudioSetup[] audioSetups;
		public AudioClip[] currentAudioClips;
		public AudioClip[] audioClips;
		public List<GameObject> folderStructure = new List<GameObject>();
		
		public AudioPlayer audioPlayer;

		public void Initialize(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			
			if (Application.isPlaying) {
				audioPlayer.SetChildrenActive(false);
				FreezeTransforms();
			}
			
			UpdateHierarchy();
		}
		
		public void UpdateHierarchy() {
			if (audioPlayer == null) {
				return;
			}
			
			if (Application.isPlaying){
				return;
			}
			
			UpdateAudioSetups();
			SetCurrentAudioClips();
			CreateHierarchy();
			RemoveEmptyFolders();
			audioPlayer.gameObject.SortChildrenRecursive();
			FreezeTransforms();
			CleanUp();
		}

		public void UpdateAudioSetups() {
			audioSetups = audioPlayer.GetComponentsInChildren<AudioSetup>();
			
			foreach (AudioSetup audioSetup in audioSetups) {
				audioSetup.Update();
			}
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
					AudioSetup audioSetup = child.GetOrAddComponent<AudioSetup>();
					AudioSource audioSource = audioSetup.GetOrAddComponent<AudioSource>();
					audioSource.playOnAwake = false;
					
//					audioSource.clip = audioClip;
					
					audioSetup.audioInfo = new AudioInfo(audioSource, audioSetup, audioPlayer);
					audioSetup.audioInfo.clipPath = HelperFunctions.GetResourcesPath(audioClip);
					audioSetup.audioPlayer = audioPlayer;
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
			currentAudioClips = new AudioClip[audioSetups.Length];
			
			for (int i = 0; i < audioSetups.Length; i++) {
				if (audioSetups[i] != null) {
					currentAudioClips[i] = audioSetups[i].Clip;
				}
			}
		}
		
		public void FreezeTransforms() {
			audioPlayer.transform.hideFlags = HideFlags.HideInInspector;
			audioPlayer.transform.Reset();
			
			if (audioSetups != null) {
				foreach (AudioSetup audioSetup in audioSetups) {
					if (audioSetup != null) {
						audioSetup.FreezeTransform();
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

		void CleanUp() {
			foreach (AudioClip audioClip in currentAudioClips) {
				Resources.UnloadAsset(audioClip);
			}
			foreach (AudioClip audioClip in audioClips) {
				Resources.UnloadAsset(audioClip);
			}
			
			audioSetups = new AudioSetup[0];
			currentAudioClips = new AudioClip[0];
			audioClips = new AudioClip[0];
		}
	}
}
