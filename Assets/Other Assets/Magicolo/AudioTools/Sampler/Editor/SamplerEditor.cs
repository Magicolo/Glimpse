using Magicolo.EditorTools;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[CustomEditor(typeof(Sampler))]
	public class SamplerEditor : CustomEditorBase {

		Sampler sampler;
		SamplerEditorHelper editorHelper;
		SerializedProperty editorHelperProperty;
		
		SerializedProperty instrumentsProperty;
		SamplerInstrument currentInstrument;
		SerializedProperty currentInstrumentProperty;
		SamplerInstrumentSettings currentInstrumentSettings;
		SerializedProperty currentInstrumentSettingsProperty;
		SamplerVelocitySettings currentVelocitySettings;
		SerializedProperty currentVelocitySettingsProperty;
		SerializedProperty currentSourcesProperty;
		SamplerInstrumentSource currentSource;
		int currentSourceIndex;
		SamplerInstrumentLayer currentLayer;
		
		public override void OnEnable() {
			base.OnEnable();
		
			((Sampler)target).SetExecutionOrder(-9);
		}
		
		public override void OnInspectorGUI() {
			sampler = (Sampler)target;
			editorHelper = sampler.editorHelper;
			editorHelperProperty = serializedObject.FindProperty("editorHelper");
		
			Begin();
		
			ShowInstruments();
		
			End();
		}
		
		public void ShowInstruments() {
			instrumentsProperty = editorHelperProperty.FindPropertyRelative("instruments");
		
			if (LargeAddButton(instrumentsProperty, "Add New Instrument".ToGUIContent())) {
				editorHelper.instruments[editorHelper.instruments.Length - 1] = new SamplerInstrument("", 0, sampler.itemManager, sampler);
				editorHelper.instruments[editorHelper.instruments.Length - 1].SetUniqueName("default", editorHelper.instruments);
				serializedObject.Update();
			}
		
			for (int i = 0; i < editorHelper.instruments.Length; i++) {
				currentInstrument = editorHelper.instruments[i];
				currentInstrumentProperty = instrumentsProperty.GetArrayElementAtIndex(i);
				currentInstrumentSettings = currentInstrument.settings;
				currentInstrumentSettingsProperty = currentInstrumentProperty.FindPropertyRelative("settings");
				currentVelocitySettings = currentInstrumentSettings.velocitySettings;
				currentVelocitySettingsProperty = currentInstrumentSettingsProperty.FindPropertyRelative("velocitySettings");
				currentSourcesProperty = currentInstrumentSettingsProperty.FindPropertyRelative("sources");
		
				BeginBox();
		
				if (DeleteFoldOut(instrumentsProperty, i, currentInstrument.Name.ToGUIContent(), GetStateColorStyle())) {
					break;
				}
		
				ShowInstrument();
		
				EndBox();
			}
		}
		
		public void ShowInstrument() {
			if (currentInstrumentProperty.isExpanded) {
				EditorGUI.indentLevel += 1;
		
				// Name
				EditorGUI.BeginDisabledGroup(Application.isPlaying);
				EditorGUI.BeginChangeCheck();
				string instrumentName = EditorGUILayout.TextField(currentInstrument.Name);
				if (EditorGUI.EndChangeCheck()) {
					currentInstrument.SetUniqueName(instrumentName, currentInstrument.Name, "default", editorHelper.instruments);
				}
		
				// 3D
				EditorGUILayout.PropertyField(currentInstrumentSettingsProperty.FindPropertyRelative("is3D"), "3D Clips".ToGUIContent());
		
				// Generate Settings
				currentInstrumentSettings.generateMode = (SamplerInstrumentSettings.GenerateModes)EditorGUILayout.EnumPopup("Generate Mode".ToGUIContent(), currentInstrumentSettings.generateMode);
				EditorGUI.EndDisabledGroup();
		
				if (currentInstrumentSettings.generateMode == SamplerInstrumentSettings.GenerateModes.GenerateAtRuntime) {
					EditorGUI.indentLevel += 1;
		
					EditorGUILayout.PropertyField(currentInstrumentSettingsProperty.FindPropertyRelative("destroyIdle"));
		
					if (currentInstrumentSettings.destroyIdle) {
						EditorGUILayout.PropertyField(currentInstrumentSettingsProperty.FindPropertyRelative("idleThreshold"));
					}
		
					EditorGUI.indentLevel -= 1;
				}
		
				// Velocity Settings
				BeginBox();
		
				Foldout(currentVelocitySettingsProperty, "Velocity".ToGUIContent());
		
				if (currentVelocitySettingsProperty.isExpanded) {
					EditorGUI.indentLevel += 1;
		
					EditorGUILayout.PropertyField(currentVelocitySettingsProperty.FindPropertyRelative("affectsVolume"));
					currentVelocitySettings.curve = EditorGUILayout.CurveField("Curve".ToGUIContent(), currentVelocitySettings.curve, Color.cyan, new Rect(0, 0, 1, 1));
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					EditorGUILayout.PropertyField(currentVelocitySettingsProperty.FindPropertyRelative("layers"));
					EditorGUI.EndDisabledGroup();
		
					EditorGUI.indentLevel -= 1;
				}
		
				EndBox();
		
				// Sources
				BeginBox();
		
				EditorGUILayout.BeginHorizontal();
		
				Foldout(currentSourcesProperty, "Sources".ToGUIContent());
		
				if (Button("Reset".ToGUIContent(), ButtonAlign.Right, true, GUILayout.MaxWidth(42))) {
					currentInstrumentSettings.Reset();
					return;
				}
		
				EditorGUILayout.EndHorizontal();
		
				ShowSources();
		
				EndBox();
				Separator();
		
				EditorGUI.indentLevel -= 1;
			}
		}
		
		void ShowSources() {
			if (currentSourcesProperty.isExpanded) {
				MinMaxSlider(currentInstrumentSettingsProperty.FindPropertyRelative("minNote"), currentInstrumentSettingsProperty.FindPropertyRelative("maxNote"), 0, 127, true);
		
				EditorGUI.indentLevel += 1;
		
				for (int i = currentInstrumentSettings.minNote; i < currentInstrumentSettings.maxNote + 1; i++) {
					currentSource = currentInstrumentSettings.sources[i];
					currentSourceIndex = i;
					currentSource.ResetDropCounter();
		
					ShowLayers();
				}
		
				EditorGUI.indentLevel -= 1;
			}
		}
		
		void ShowLayers() {
			EditorGUILayout.BeginHorizontal();
		
			EditorGUILayout.LabelField("", GUILayout.Width((EditorGUI.indentLevel - 1) * 15 + 3));
			GUIStyle style = new GUIStyle("MiniToolbarButton");
			style.clipping = TextClipping.Overflow;
			style.alignment = TextAnchor.MiddleLeft;
			style.contentOffset = new Vector2(-2, 0);
		
			Rect dropArea = EditorGUILayout.BeginHorizontal();
			if (Button(new GUIContent(currentSource.Name, EditorGUIUtility.ObjectContent(null, typeof(AudioSource)).image), style, false, GUILayout.Width(92)) && Application.isPlaying) {
				sampler.itemManager.Play(currentInstrument.Name, currentSourceIndex, 100, null);
			}
			EditorGUILayout.EndHorizontal();
		
			DropArea<AudioSetup>(dropArea, true, OnLayerDropped);
		
			for (int i = 0; i < currentVelocitySettings.layers; i++) {
				currentLayer = currentSource.layers[i];
		
				int index = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;
		
				EditorGUI.BeginDisabledGroup(Application.isPlaying);
				if (Application.isPlaying) {
					EditorGUILayout.ObjectField(GUIContent.none, currentLayer.clip, typeof(AudioClip), true, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(36), GUILayout.Height(16));
				}
				else {
					currentLayer.AudioSetup = EditorGUILayout.ObjectField(GUIContent.none, currentLayer.AudioSetup, typeof(AudioSetup), true, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(36), GUILayout.Height(16)) as AudioSetup;
				}
				EditorGUI.EndDisabledGroup();
		
				EditorGUI.indentLevel = index;
			}
			EditorGUILayout.EndHorizontal();
		}
		
		void OnLayerDropped(AudioSetup droppedObject) {
			if (currentInstrumentSettings.sources[currentSourceIndex].dropCounter >= currentVelocitySettings.layers) {
				currentSourceIndex = Mathf.Min(currentSourceIndex + 1, currentInstrumentSettings.maxNote - 1);
			}
		
			if (currentInstrumentSettings.sources[currentSourceIndex].dropCounter < currentVelocitySettings.layers) {
				currentInstrumentSettings.sources[currentSourceIndex].layers[currentInstrumentSettings.sources[currentSourceIndex].dropCounter].AudioSetup = droppedObject;
				currentInstrumentSettings.sources[currentSourceIndex].dropCounter += 1;
				serializedObject.Update();
			}
		}
	
		GUIStyle GetStateColorStyle() {
			GUIStyle style = new GUIStyle("foldout");
			style.fontStyle = FontStyle.Bold;
		
			if (Application.isPlaying) {
				Color textColor = style.normal.textColor;
				AudioStates state = currentInstrument.State;
			
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
}
