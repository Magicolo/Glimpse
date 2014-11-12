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
			
		public virtual string[] GetInstrumentNames() {
			string[] names = new string[sampler.editorHelper.instruments.Length];
			
			for (int i = 0; i < names.Length; i++) {
				names[i] = sampler.editorHelper.instruments[i].Name;
			}
			return names;
		}
		
		public virtual SamplerInstrument GetInstrument(string instrumentName) {
			foreach (SamplerInstrument instrument in sampler.editorHelper.instruments) {
				if (instrument.Name == instrumentName) {
					return instrument;
				}
			}
			return null;
		}
		
		public override void OnHierarchyWindowItemGUI(int instanceId, Rect selectionrect) {
			base.OnHierarchyWindowItemGUI(instanceId, selectionrect);
			
			#if UNITY_EDITOR
			icon = icon ?? HelperFunctions.LoadAssetInFolder<Texture>("sampler.png", "Magicolo/AudioTools/Sampler");
			GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			
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
