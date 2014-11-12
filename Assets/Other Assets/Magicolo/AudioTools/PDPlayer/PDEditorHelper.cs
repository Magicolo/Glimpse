using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDEditorHelper : Magicolo.EditorTools.EditorHelper {

		public string initializePatchPath;
		public PDEditorModule defaultModule;
		public List<PDEditorModule> modules = new List<PDEditorModule>();
		public Texture icon;
		public PDPlayer pdPlayer;
		
		public void Initialize(PDPlayer pdPlayer) {
			this.pdPlayer = pdPlayer;
			Update();
		}

		public override void Update() {
			base.Update();
			
			initializePatchPath = HelperFunctions.GetAssetPath("initialize~.pd").Replace("Assets" + Path.AltDirectorySeparatorChar + "StreamingAssets" + Path.AltDirectorySeparatorChar, "");
		}
		
		public virtual void DrawGizmos() {
			#if UNITY_EDITOR
			List<PDEditorModule> moduleList = new List<PDEditorModule>(modules);
			moduleList.Add(defaultModule);
			foreach (PDEditorModule module in moduleList) {
				if (module == null || module.Source == null){
					continue;
				}
				
				Gizmos.DrawIcon(module.Source.transform.position, "pd.png", true);
				if (!UnityEditor.Selection.gameObjects.Contains(pdPlayer.gameObject) && !UnityEditor.Selection.gameObjects.Contains(module.Source) || !module.spatializerShowing) {
					return;
				}
				Gizmos.color = new Color(0.25F, 0.5F, 0.75F, 1);
				Gizmos.DrawWireSphere(module.Source.transform.position, module.MinDistance);
				Gizmos.color = new Color(0.25F, 0.75F, 0.5F, 0.35F);
				Gizmos.DrawWireSphere(module.Source.transform.position, module.MaxDistance);
			}
			#endif
		}
		
		public override void OnHierarchyWindowItemGUI(int instanceId, Rect selectionrect) {
			base.OnHierarchyWindowItemGUI(instanceId, selectionrect);
			
			#if UNITY_EDITOR
			icon = icon ?? HelperFunctions.LoadAssetInFolder<Texture>("pd.png", "Magicolo/AudioTools/PDPlayer");
			GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			
			if (gameObject == null || icon == null) return;
			
			float width = selectionrect.width;
			selectionrect.width = 16;
			selectionrect.height = 16;
			
			
			PDPlayer player = gameObject.GetComponent<PDPlayer>();
			if (player != null) {
				selectionrect.x = width - 22 + gameObject.GetHierarchyDepth() * 14;
				GUI.DrawTexture(selectionrect, icon);
			}
			#endif
		}
	}
}
