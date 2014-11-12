using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.EditorTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioPlayerEditorHelper : EditorHelper {

		public Texture icon;
		public AudioPlayer audioPlayer;
		
		public void Initialize(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			Update();
		}

		public override void OnHierarchyWindowItemGUI(int instanceId, Rect selectionrect) {
			base.OnHierarchyWindowItemGUI(instanceId, selectionrect);
			
			#if UNITY_EDITOR
			icon = icon ?? HelperFunctions.LoadAssetInFolder<Texture>("audioplayer.png", "Magicolo/AudioTools/AudioPlayer");
			GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			
			if (gameObject == null || icon == null) return;
			
			float width = selectionrect.width;
			selectionrect.width = 16;
			selectionrect.height = 16;
			
			AudioPlayer player = gameObject.GetComponent<AudioPlayer>();
			if (player != null) {
				selectionrect.x = width - 4 + gameObject.GetHierarchyDepth() * 14;
				GUI.DrawTexture(selectionrect, icon);
			}
			#endif
		}
	}
}
