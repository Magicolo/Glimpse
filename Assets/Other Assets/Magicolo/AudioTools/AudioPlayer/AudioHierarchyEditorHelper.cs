﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioHierarchyEditorHelper : Magicolo.EditorTools.EditorHelper {
		
		public IEnumerator routine;
		public AudioSource previewAudioSource;
		public AudioPlayer audioPlayer;
		
		public void Initialize(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			Update();
		}
		
		public override void OnHierarchyWindowItemGUI(int instanceid, Rect selectionrect) {
			#if UNITY_EDITOR
			Texture folderIcon = UnityEditor.EditorGUIUtility.IconContent("Project").image;
			Texture playAudioIcon = UnityEditor.EditorGUIUtility.ObjectContent(null, typeof(AudioSource)).image;
			GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceid) as GameObject;
			
			if (gameObject == null || playAudioIcon == null)
				return;
			
			float width = selectionrect.width;
			selectionrect.width = 30;
			selectionrect.height = 16;
			selectionrect.x = width - 3 + gameObject.GetHierarchyDepth() * 14;
			selectionrect.height += 1;
			GUIStyle style = new GUIStyle("MiniToolbarButtonLeft");
			style.fixedHeight += 1;
			style.contentOffset = new Vector2(-4, 0);
			style.clipping = TextClipping.Overflow;
			
			if (audioPlayer.hierarchyManager.folderStructure.Contains(gameObject)) {
				if (GUI.Button(selectionrect, folderIcon, style)){
					List<Object> selectedGameObjects = new List<Object>(UnityEditor.Selection.objects);
					if (selection.Contains(gameObject)) {
						selectedGameObjects.Remove(gameObject);
					}
					selectedGameObjects.AddRange(gameObject.GetChildrenRecursive());
					foreach (GameObject folder in audioPlayer.hierarchyManager.folderStructure) {
						if (selectedGameObjects.Contains(folder)){
							selectedGameObjects.Remove(folder);
						}
					}
					UnityEditor.Selection.objects = selectedGameObjects.ToArray();
				}
			}
			else {
				AudioSetup audioSetup = gameObject.GetComponent<AudioSetup>();
				if (audioSetup != null && audioSetup.audioInfo != null){
					AudioInfo audioInfo = audioSetup.audioInfo;
					
					if (GUI.Button(selectionrect, playAudioIcon, style)){
						UnityEditor.Selection.activeObject = gameObject;
						
						if (previewAudioSource != null){
							previewAudioSource.gameObject.Remove();
						}
						
						previewAudioSource = audioInfo.Source.PlayOnListener();
						if (previewAudioSource != null){
							previewAudioSource.Copy(audioInfo.Source);
							previewAudioSource.volume += Random.Range(-audioInfo.randomVolume, audioInfo.randomVolume);
							previewAudioSource.pitch += Random.Range(-audioInfo.randomPitch, audioInfo.randomPitch);
							routine = DestroyAfterPlaying(previewAudioSource);
						}
					}
					else if (Event.current.isMouse && Event.current.type == EventType.mouseDown){
						if (previewAudioSource != null){
							previewAudioSource.gameObject.Remove();
						}
						routine = null;
					}
				}
			}
			#endif
		}
		
		public override void OnPlaymodeStateChanged() {
			if (previewAudioSource != null) {
				previewAudioSource.gameObject.Remove();
			}
			routine = null;
		}
		
		public override void OnProjectWindowChanged() {
			if (!Application.isPlaying) {
				audioPlayer.hierarchyManager.UpdateHierarchy();
			}
		}
		
//		public override void OnSelectionChanged() {
//			base.OnSelectionChanged();
//			
//			if (selection.Length == 1 && selection[0] is GameObject && ((GameObject)selection[0]).GetComponent<AudioPlayer>() != null) {
//				audioPlayer.hierarchyManager.UpdateHierarchy();
//			}
//		}
		
		public override void OnUpdate() {
			if (routine != null && !routine.MoveNext()) {
				routine = null;
			}
		}
		
		IEnumerator DestroyAfterPlaying(AudioSource audioSource) {
			while (audioSource.isPlaying) {
				yield return null;
			}
			audioSource.gameObject.Remove();
		}
	}
}