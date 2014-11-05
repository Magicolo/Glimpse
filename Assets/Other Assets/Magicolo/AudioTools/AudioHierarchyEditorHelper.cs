using System.Collections;
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
			Texture icon = UnityEditor.EditorGUIUtility.ObjectContent(null, typeof(AudioSource)).image;
			GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceid) as GameObject;
			
			if (gameObject == null || icon == null)
				return;
			
			float width = selectionrect.width;
			selectionrect.width = 30;
			selectionrect.height = 16;
			
			AudioOptions audioOptions = gameObject.GetComponent<AudioOptions>();
			if (audioOptions == null || audioOptions.audioInfo == null){
				return;
			}
			
			AudioInfo audioInfo = audioOptions.audioInfo;
			selectionrect.x = width - 3 + gameObject.GetHierarchyDepth() * 14;
			selectionrect.height += 1;
			GUIStyle style = new GUIStyle("MiniToolbarButtonLeft");
			style.fixedHeight += 1;
			style.contentOffset = new Vector2(-4, 0);
			style.clipping = TextClipping.Overflow;
			
			if (GUI.Button(selectionrect, icon, style)){
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
			#endif
		}
		
		public override void OnPlaymodeStateChanged() {
			if (previewAudioSource != null) {
				previewAudioSource.gameObject.Remove();
			}
			routine = null;
		}
		
		public override void OnProjectWindowChanged() {
			audioPlayer.hierarchyManager.UpdateHierarchy();
			audioPlayer.hierarchyManager.updateHierarchy = true;
		}
		
		public override void OnHierarchyWindowChanged() {
//			audioPlayer.hierarchyManager.UpdateHierarchyToggle();
		}
		
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