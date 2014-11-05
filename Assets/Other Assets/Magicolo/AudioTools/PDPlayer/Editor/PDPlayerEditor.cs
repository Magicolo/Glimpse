using Magicolo.AudioTools;
using Magicolo.EditorTools;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PDPlayer))]
public class PDPlayerEditor : CustomEditorBase {

	PDPlayer pdPlayer;
	PDEditorHelper editorHelper;
	SerializedProperty editorHelperProperty;
	
	SerializedProperty modulesProperty;
	PDEditorModule currentModule;
	SerializedProperty currentModuleProperty;
	PDEditorModule defaultModule;
	SerializedProperty defaultModuleProperty;
	
	public override void OnEnable() {
		base.OnEnable();
		
		((PDPlayer)target).SetExecutionOrder(-10);
	}
	
	public override void OnInspectorGUI() {
		pdPlayer = (PDPlayer)target;
		editorHelper = pdPlayer.editorHelper;
		editorHelperProperty = serializedObject.FindProperty("editorHelper");
		modulesProperty = editorHelperProperty.FindPropertyRelative("modules");
		defaultModule = editorHelper.defaultModule;
		defaultModuleProperty = editorHelperProperty.FindPropertyRelative("defaultModule");
		
		Begin();
		
		ShowGeneralSettings();
		Separator();
		ShowDefaultSettings();
		Separator();
		ShowModules();
		
		End();
	}
	
	void ShowGeneralSettings() {
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("patchesPath"), new GUIContent("Patches Path", "The path where the Pure Data patches are relative to Assets/StreamingAssets/."));
		EditorGUI.EndDisabledGroup();
	}

	void ShowDefaultSettings() {
		BeginBox();
		
		EditorGUILayout.LabelField(new GUIContent("Default Module Settings", "Default settings for new modules."), EditorStyles.boldLabel);
		
		EditorGUI.indentLevel += 1;
		
		ShowModule(defaultModule, defaultModuleProperty);
		
		EditorGUI.indentLevel -= 1;
		
		EndBox();
	}
	
	void ShowModules() {
		if (LargeAddButton(modulesProperty, new GUIContent("Add New Module"))) {
			editorHelper.modules[editorHelper.modules.Count - 1] = new PDEditorModule("", defaultModule, pdPlayer);
			editorHelper.modules[editorHelper.modules.Count - 1].SetUniqueName("default", editorHelper.modules);
		}
	
		for (int i = 0; i < editorHelper.modules.Count; i++) {
			currentModule = editorHelper.modules[i];
			currentModuleProperty = modulesProperty.GetArrayElementAtIndex(i);
			
			BeginBox();
			
			if (DeleteFoldOut(modulesProperty, i, new GUIContent(currentModule.Name, "Sets specific settings for a module."), GetStateColorStyle())) {
				break;
			}
			
			if (currentModuleProperty.isExpanded) {
				EditorGUI.indentLevel += 1;
				
				EditorGUI.BeginChangeCheck();
				EditorGUI.BeginDisabledGroup(Application.isPlaying);
				string moduleName = EditorGUILayout.TextField(currentModule.Name);
				EditorGUI.EndDisabledGroup();
				if (EditorGUI.EndChangeCheck()) {
					currentModule.SetUniqueName(moduleName, currentModule.Name, "default", editorHelper.modules);
				}
				
				ShowModule(currentModule, currentModuleProperty);
				Separator();
				
				EditorGUI.indentLevel -= 1;
			}
			
			EndBox();
		}
	}

	void ShowModule(PDEditorModule module, SerializedProperty moduleProperty) {
		EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("volume"), new GUIContent("Volume", "Sets the volume of this module."));
			
		BeginBox();
		module.spatializerShowing = EditorGUILayout.Foldout(module.spatializerShowing, new GUIContent("Spatializer", "These settings specify how the module should be spatialize via the [uspatialize~] object in Pure Data."));
		if (module.spatializerShowing) {
			EditorGUI.indentLevel += 1;
			
			PropertyObjectField<GameObject>(moduleProperty.FindPropertyRelative("source"), new GUIContent("Source", "The source around which the module will be spatialized.\nIf left empty, the module will not be spatialized or attenuated."));
			EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("volumeRolloff"), new GUIContent("Volume Rolloff", "The curve of the distance attenuation."));
			EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("minDistance"), new GUIContent("Min Distance", "Distance at which the module starts to be attenuated."));
			moduleProperty.FindPropertyRelative("minDistance").Clamp(0, moduleProperty.FindPropertyRelative("maxDistance").floatValue);
			EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("maxDistance"), new GUIContent("Max Distance", "Distance at which the module has been completely."));
			moduleProperty.FindPropertyRelative("maxDistance").Min(moduleProperty.FindPropertyRelative("minDistance").floatValue + moduleProperty.FindPropertyRelative("minDistance").floatValue / 10);
			EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("panLevel"), new GUIContent("Pan Level", "Controls how much panning is applied."));
				
			EditorGUI.indentLevel -= 1;
		}
		EndBox();
	}

	GUIStyle GetStateColorStyle() {
		GUIStyle style = new GUIStyle("foldout");
		style.fontStyle = FontStyle.Bold;
		
		if (Application.isPlaying) {
			Color textColor = style.normal.textColor;
			AudioStates state = currentModule.State;
			
			switch (state) {
				case AudioStates.FadingIn:
					textColor = new Color(1, 1, 0, 1);
					break;
				case AudioStates.FadingOut:
					textColor = new Color(1, 1, 0.5F, 1);
					break;
				case AudioStates.Paused:
					textColor = new Color(0, 0, 1, 1);
					break;
				case AudioStates.Playing:
					textColor = new Color(0, 1, 0, 1);
					break;
				case AudioStates.Waiting:
					textColor = new Color(1, 0, 0, 1);
					break;
				case AudioStates.Stopped:
					textColor = new Color(1, 0, 0, 1);
					break;
			}
			
			style.normal.textColor = textColor * 0.9F;
			style.onNormal.textColor = textColor * 0.9F;
			style.focused.textColor = textColor;
			style.onFocused.textColor = textColor;
			style.active.textColor = textColor;
			style.onActive.textColor = textColor;
		}
		
		return style;
	}
}