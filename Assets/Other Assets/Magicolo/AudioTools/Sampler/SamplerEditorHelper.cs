using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.EditorTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SamplerEditorHelper : EditorHelper {

		public SamplerInstrument[] instruments = new SamplerInstrument[0];
		public Texture icon;
		public Sampler sampler;
		
		public void Initialize(Sampler sampler) {
			this.sampler = sampler;
			Update();
		}

		public override void OnHierarchyWindowItemGUI(int instanceid, Rect selectionrect) {
			#if UNITY_EDITOR
			icon = icon ?? HelperFunctions.LoadAssetInFolder<Texture>("sampler.png", "Sampler");
			GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceid) as GameObject;
			
			if (gameObject == null || icon == null) return;
			
			float width = selectionrect.width;
			selectionrect.width = 16;
			selectionrect.height = 16;
			
			Sampler player = gameObject.GetComponent<Sampler>();
			if (player != null) {
				selectionrect.x = width - 40 + gameObject.GetHierarchyDepth() * 14;
				GUI.DrawTexture(selectionrect, icon);
			}
			#endif
		}
	}
}
