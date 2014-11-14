using System;
using System.Collections;
using System.Reflection;
using Magicolo.EditorTools;
using Magicolo.GeneralTools;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Magicolo.AudioTools {
	[CustomEditor(typeof(AudioPlayer))]
	public class AudioPlayerEditor : CustomEditorBase {
		
		AudioPlayer audioPlayer;
		PDPlayer pdPlayer;
		Sampler sampler;
		AudioGeneralSettings generalSettings;
		SerializedProperty generalSettingsProperty;
		
		SerializedProperty containersProperty;
		AudioContainer currentContainer;
		SerializedProperty currentContainerProperty;
		SerializedProperty subContainersProperty;
		AudioSubContainer currentSubContainer;
		int currentSubContainerIndex;
		SerializedProperty currentSubContainerProperty;
		AudioOption currentAudioOption;
		SerializedProperty currentAudioOptionProperty;
		AudioClip currentClip;
		
		Texture pdIcon;
		Texture samplerIcon;
		
		public override void OnEnable() {
			base.OnEnable();
			
			((AudioPlayer)target).SetExecutionOrder(-11);
		}
		
		public override void OnInspectorGUI() {
			audioPlayer = (AudioPlayer)target;
			pdPlayer = FindObjectOfType<PDPlayer>();
			sampler = FindObjectOfType<Sampler>();
			generalSettings = audioPlayer.generalSettings;
			generalSettingsProperty = serializedObject.FindProperty("generalSettings");
			
			Begin();
			
			ShowGeneralSettings();
			Separator();
			ShowContainers();
			ShowButtons();
			Separator();
			
			End();
		}

		void ShowGeneralSettings() {
			EditorGUILayout.PropertyField(generalSettingsProperty.FindPropertyRelative("masterVolume"), new GUIContent("Master Volume", "Controls the volume of all audio sources."));
			EditorGUILayout.PropertyField(generalSettingsProperty.FindPropertyRelative("maxVoices"), new GUIContent("Max Voices", "Limits the amount of simultaneous playing audio sources to this value, by removing the oldest audio sources. Audio sources with the 'DoNotKill' option will be ignored."));
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			EditorGUILayout.PropertyField(generalSettingsProperty.FindPropertyRelative("persistent"), new GUIContent("Persistent", "If true, the AudioPlayer will not be destroyed when loading a scene. Note that this will prevent any other AudioPlayer from other scenes to be loaded."));
			EditorGUI.EndDisabledGroup();
		}
		
		void ShowContainers() {
			containersProperty = generalSettingsProperty.FindPropertyRelative("containers");
			
			if (AddFoldOut(containersProperty, "Containers".ToGUIContent())) {
				generalSettings.containers[generalSettings.containers.Length - 1] = new AudioContainer("");
				generalSettings.containers.Last().SetUniqueName("default", generalSettings.containers);
			}
			
			if (containersProperty.isExpanded) {
				EditorGUI.indentLevel += 1;
				
				for (int i = 0; i < generalSettings.containers.Length; i++) {
					currentContainer = generalSettings.containers[i];
					currentContainerProperty = containersProperty.GetArrayElementAtIndex(i);
					
					BeginBox();
					
					GUIStyle style = new GUIStyle("foldout");
					style.fontStyle = FontStyle.Bold;
					if (DeleteFoldOut(containersProperty, i, currentContainer.Name.ToGUIContent(), style)) {
						break;
					}
						
					ShowContainer();
					
					EndBox();
				}
				EditorGUI.indentLevel -= 1;
			}
		}

		void ShowContainer() {
			subContainersProperty = currentContainerProperty.FindPropertyRelative("subContainers");
			
			if (currentContainerProperty.isExpanded) {
				EditorGUI.indentLevel += 1;
				
				EditorGUI.BeginChangeCheck();
				string containerName = EditorGUILayout.TextField(currentContainer.Name);
				if (EditorGUI.EndChangeCheck()) {
					currentContainer.SetUniqueName(containerName, currentContainer.Name, "default", generalSettings.containers);
				}
				currentContainer.type = (AudioContainer.Types)EditorGUILayout.EnumPopup(currentContainer.type);
				
				BeginBox();

				if (currentContainer.type == AudioContainer.Types.SwitchContainer) {
					ShowEnums(currentContainerProperty);
				}
				
				ShowSources();
				
				if (subContainersProperty.isExpanded) {
					EditorGUI.indentLevel += 1;
						
					ShowSubContainers(currentContainer.childrenIds);
					
					EditorGUI.indentLevel -= 1;
				}
				
				EndBox();
				
				Separator();
				
				EditorGUI.indentLevel -= 1;
			}
		}

		void ShowSources() {
			if (AddFoldOut<AudioSetup>(subContainersProperty, "Sources".ToGUIContent(), currentContainer.childrenIds.Count, OnContainerSourceDropped)) {
				if (currentContainer.childrenIds.Count > 0) {
					currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioSubContainer(currentContainer, 0, currentContainer.GetSubContainerWithID(currentContainer.childrenIds.Last()));
				}
				else {
					currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioSubContainer(currentContainer, 0);
				}
				serializedObject.Update();
			}
		}
		
		void OnContainerSourceDropped(AudioSetup droppedObject) {
			AddToArray(subContainersProperty);
			currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioSubContainer(currentContainer, 0, droppedObject);
			serializedObject.Update();
		}
		
		void ShowEnums(SerializedProperty property) {
			SerializedProperty stateHolderProperty = property.FindPropertyRelative("stateHolder");
			SerializedProperty statePathProperty = property.FindPropertyRelative("statePath");
			
			Rect rect = EditorGUILayout.BeginHorizontal();
			
			PropertyObjectField<UnityEngine.Object>(stateHolderProperty, "State Holder".ToGUIContent(), GUILayout.MaxWidth(stateHolderProperty.objectReferenceValue is GameObject ? Screen.width / 1.6F : Screen.width));
			
			UnityEngine.Object stateHolder = stateHolderProperty.objectReferenceValue;
			
			if (stateHolder is GameObject) {
				List<string> componentNames = new List<string>{ "-Components-" };
				Component[] components = ((GameObject)stateHolder).GetComponents<Component>();
				foreach (Component component in components) {
					componentNames.Add(component.GetType().Name);
				}
				
				float width = Mathf.Min(92 + EditorGUI.indentLevel * 16, EditorGUIUtility.currentViewWidth / 5 + EditorGUI.indentLevel * 16);
				int index = EditorGUI.Popup(new Rect(rect.x + rect.width - width, rect.y, width, rect.height), 0, componentNames.ToArray());
				if (index > 0) {
					stateHolderProperty.objectReferenceValue = components[index - 1];
				}
			}
			
			EditorGUILayout.EndHorizontal();
			
			if (stateHolder != null) {
				string[] enumNames = stateHolder.GetFieldsPropertiesNames(ObjectExtensions.AllPublicFlags, typeof(Enum));
				
				if (enumNames.Length > 0) {
					int index = Mathf.Max(Array.IndexOf(enumNames, statePathProperty.stringValue), 0);
					index = EditorGUILayout.Popup("State Field", index, enumNames);
					statePathProperty.stringValue = enumNames[Mathf.Clamp(index, 0, Mathf.Max(enumNames.Length - 1, 0))];
					return;
				}
			}
			EditorGUILayout.Popup("State Field", 0, new string[0]);
		}
		
		void ShowSubContainers(List<int> ids) {
			for (int i = 0; i < ids.Count; i++) {
				currentSubContainer = currentContainer.GetSubContainerWithID(ids[i]);
				currentSubContainerIndex = currentContainer.subContainers.IndexOf(currentSubContainer);
				currentSubContainerProperty = subContainersProperty.GetArrayElementAtIndex(currentSubContainerIndex);
				
				if (DeleteFoldOut<AudioSetup>(subContainersProperty, currentSubContainerIndex, currentSubContainer.Name.ToGUIContent(), GetContainerStyle(currentSubContainer.type), OnSubContainerDropped, OnSubContainerReorder)) {
					currentSubContainer.Remove(currentContainer);
					break;
				}
				
				ShowSubContainer();
			}
		}
		
		void OnSubContainerDropped(AudioSetup droppedObject) {
			if (currentSubContainer.type == AudioSubContainer.Types.AudioSource) {
				currentSubContainer.AudioSetup = droppedObject;
				serializedObject.Update();
			}
			else {
				OnSubContainerSourceDropped(droppedObject);
			}
		}
		
		void OnSubContainerReorder(SerializedProperty arrayProperty, int sourceIndex, int targetIndex) {
			AudioSubContainer sourceSubContainer = currentContainer.subContainers[sourceIndex];
			AudioSubContainer targetSubContainer = currentContainer.subContainers[targetIndex];
			List<int> sourceParentIds = sourceSubContainer.parentId == 0 ? currentContainer.childrenIds : currentContainer.GetSubContainerWithID(sourceSubContainer.parentId).childrenIds;
			List<int> targetParentIds = targetSubContainer.parentId == 0 ? currentContainer.childrenIds : currentContainer.GetSubContainerWithID(targetSubContainer.parentId).childrenIds;
			
			if (sourceParentIds == targetParentIds) {
				int sourceSubContainerIndex = sourceParentIds.IndexOf(sourceSubContainer.id);
				int targetSubContainerIndex = targetParentIds.IndexOf(targetSubContainer.id);
				sourceParentIds.Move(sourceSubContainerIndex, targetSubContainerIndex);
			}
			else {
				int targetSubContainerIndex = targetParentIds.IndexOf(targetSubContainer.id);
				targetParentIds.Insert(targetSubContainerIndex, sourceSubContainer.id);
				sourceParentIds.Remove(sourceSubContainer.id);
				sourceSubContainer.parentId = targetSubContainer.parentId;
			}
			serializedObject.Update();
		}

		void ShowSubContainer() {
			AdjustContainerName();
			
			if (currentSubContainerProperty.isExpanded) {
				EditorGUI.indentLevel += 1;
				
				ShowGeneralContainerSettings();
				
				if (currentSubContainer.IsSource) {
					switch (currentSubContainer.type) {
						case AudioSubContainer.Types.AudioSource:
							ShowAudioSource();
							break;
						case AudioSubContainer.Types.Sampler:
							ShowSampler();
							break;
					}
				}
				else {
					BeginBox();
					
					switch (currentSubContainer.type) {
						case AudioSubContainer.Types.MixContainer:
							ShowMixContainer();
							break;
						case AudioSubContainer.Types.RandomContainer:
							ShowRandomContainer();
							break;
						case AudioSubContainer.Types.SwitchContainer:
							ShowSwitchContainer();
							break;
					}
					
					EndBox();
			
					Separator();
				}
				
				
				EditorGUI.indentLevel -= 1;
			}
		}

		void ShowAudioSource() {
			currentSubContainer.AudioSetup = EditorGUILayout.ObjectField("Source".ToGUIContent(), currentSubContainer.AudioSetup, typeof(AudioSetup), true) as AudioSetup;
			ContextMenu(new []{ "Clear".ToGUIContent() }, new GenericMenu.MenuFunction2[]{ OnAudioSourceCleared }, new object[]{ currentSubContainer });
			
			currentClip = currentSubContainer.AudioSetup == null ? null : currentSubContainer.AudioSetup.audioInfo.Clip;
			
			if (currentSubContainer.AudioSetup != null && currentSubContainer.AudioSetup.audioInfo != null && currentSubContainer.AudioSetup.audioInfo.Source != null && currentClip != null) {
				ShowAudioOptions();
			}
		}

		void OnAudioSourceCleared(object data){
			AudioSubContainer subContainer = data as AudioSubContainer;
			subContainer.AudioSetup = null;
			subContainer.audioInfo = null;
			subContainer.audioSetupName = "";
			serializedObject.Update();
		}
		
		void ShowSampler() {
			if (sampler == null) {
				EditorGUILayout.HelpBox("No sampler was found in the scene.", MessageType.Warning);
				return;
			}
			
			if (sampler.editorHelper.instruments.Length <= 0) {
				EditorGUILayout.HelpBox("There needs to be at least one instrument in the sampler.", MessageType.Warning);
				return;
			}
			
			currentSubContainer.instrumentName = Popup("Instrument".ToGUIContent(), currentSubContainer.instrumentName, sampler.editorHelper.GetInstrumentNames());
			SamplerInstrument instrument = sampler.editorHelper.GetInstrument(currentSubContainer.instrumentName);
			
			if (instrument != null) {
				currentSubContainer.note = EditorGUILayout.IntSlider("Note".ToGUIContent(), currentSubContainer.note, instrument.settings.minNote, instrument.settings.maxNote);
				currentSubContainer.velocity = EditorGUILayout.Slider("Velocity".ToGUIContent(), currentSubContainer.velocity, 0, 127);
				
				ShowAudioOptions();
			}
		}

		void ShowMixContainer() {
			ShowSubSources();
			
			if (currentSubContainer.Showing && currentContainer.childrenIds.Count > 0) {
				EditorGUI.indentLevel += 1;
					
				ShowSubContainers(currentSubContainer.childrenIds);
				
				EditorGUI.indentLevel -= 1;
			}
		}

		void ShowRandomContainer() {
			ShowSubSources();
			
			if (currentSubContainer.Showing && currentContainer.childrenIds.Count > 0) {
				EditorGUI.indentLevel += 1;
					
				ShowSubContainers(currentSubContainer.childrenIds);
				
				EditorGUI.indentLevel -= 1;
			}
		}
		
		void ShowSwitchContainer() {
			ShowEnums(currentSubContainerProperty);
			
			ShowSubSources();
			
			if (currentSubContainer.Showing && currentContainer.childrenIds.Count > 0) {
				EditorGUI.indentLevel += 1;
					
				ShowSubContainers(currentSubContainer.childrenIds);
				
				EditorGUI.indentLevel -= 1;
			}
		}
		
		void OnSubContainerSourceDropped(AudioSetup droppedObject) {
			AddToArray(subContainersProperty);
			currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioSubContainer(currentContainer, currentSubContainer.id, droppedObject);
			serializedObject.Update();
		}
		
		void ShowGeneralContainerSettings() {
			currentSubContainer.type = (AudioSubContainer.Types)EditorGUILayout.EnumPopup(currentSubContainer.type);
			
			if (GetParentContainerType(currentSubContainer, currentContainer) == AudioSubContainer.Types.RandomContainer) {
				EditorGUILayout.PropertyField(currentSubContainerProperty.FindPropertyRelative("weight"));
			}
			else if (GetParentContainerType(currentSubContainer, currentContainer) == AudioSubContainer.Types.SwitchContainer) {
				UnityEngine.Object stateHolder = currentSubContainer.parentId == 0 ? currentContainer.stateHolder : currentContainer.GetSubContainerWithID(currentSubContainer.parentId).stateHolder;
				string statePath = currentSubContainer.parentId == 0 ? currentContainer.statePath : currentContainer.GetSubContainerWithID(currentSubContainer.parentId).statePath;
				
				if (stateHolder != null && !string.IsNullOrEmpty(statePath)) {
					FieldInfo enumField = stateHolder.GetType().GetField(statePath, ObjectExtensions.AllPublicFlags);
					PropertyInfo enumProperty = stateHolder.GetType().GetProperty(statePath, ObjectExtensions.AllPublicFlags);
					Type enumType = enumField == null ? enumProperty == null ? null : enumProperty.PropertyType : enumField.FieldType;
					
					if (enumType != null) {
						Enum defaultState = (Enum)Enum.Parse(enumType, Enum.GetNames(enumType)[0]);
						Enum selectedState = string.IsNullOrEmpty(currentSubContainer.stateName) ? null : Enum.GetNames(enumType).Contains(currentSubContainer.stateName) ? (Enum)Enum.Parse(enumType, currentSubContainer.stateName) : null;
						
						if (selectedState == null) {
							currentSubContainer.stateName = EditorGUILayout.EnumPopup("State", defaultState).ToString();
						}
						else {
							currentSubContainer.stateName = EditorGUILayout.EnumPopup("State", selectedState).ToString();
						}
						return;
					}
				}
				
				EditorGUILayout.Popup("State", 0, new string[0]);
			}
		}
	
		void ShowAudioOptions() {
			SerializedProperty audioOptionsProperty = currentSubContainerProperty.FindPropertyRelative("audioOptions");
			
			if (AddFoldOut(audioOptionsProperty, "Audio Options".ToGUIContent())) {
				currentSubContainer.audioOptions[currentSubContainer.audioOptions.Length - 1].SetDefaultValue();
			}
			
			if (currentSubContainer.audioOptions != null && audioOptionsProperty.isExpanded) {
				EditorGUI.indentLevel += 1;
				
				for (int i = 0; i < currentSubContainer.audioOptions.Length; i++) {
					currentAudioOption = currentSubContainer.audioOptions[i];
					currentAudioOptionProperty = audioOptionsProperty.GetArrayElementAtIndex(i);
					
					if (DeleteFoldOut(audioOptionsProperty, i, string.Format("{0} | {1}", currentAudioOption.type, currentAudioOption.GetValue()).ToGUIContent())) {
						break;
					}
				
					ShowAudioOption();
				}
				
				EditorGUI.indentLevel -= 1;
			}
		}
		
		void ShowAudioOption() {
			if (currentAudioOptionProperty.isExpanded) {
				EditorGUI.indentLevel += 1;
				
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(currentAudioOptionProperty.FindPropertyRelative("type"));
				if (EditorGUI.EndChangeCheck()) {
					serializedObject.ApplyModifiedProperties();
					currentAudioOption.SetDefaultValue();
				}
				
				// Float fields
				if (currentAudioOption.type == AudioOption.OptionTypes.FadeIn) {
					currentAudioOption.SetValue(Mathf.Max(EditorGUILayout.FloatField("Value".ToGUIContent(), currentAudioOption.GetValue<float>()), 0));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.FadeOut) {
					currentAudioOption.SetValue(Mathf.Max(EditorGUILayout.FloatField("Value".ToGUIContent(), currentAudioOption.GetValue<float>()), 0));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.Delay) {
					currentAudioOption.SetValue(Mathf.Max(EditorGUILayout.FloatField("Value".ToGUIContent(), currentAudioOption.GetValue<float>()), 0));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.MinDistance) {
					currentAudioOption.SetValue(Mathf.Max(EditorGUILayout.FloatField("Value".ToGUIContent(), currentAudioOption.GetValue<float>()), 0));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.MaxDistance) {
					currentAudioOption.SetValue(Mathf.Max(EditorGUILayout.FloatField("Value".ToGUIContent(), currentAudioOption.GetValue<float>()), 0));
				}
				// Slider fields
				else if (currentAudioOption.type == AudioOption.OptionTypes.Volume || currentAudioOption.type == AudioOption.OptionTypes.RandomVolume || currentAudioOption.type == AudioOption.OptionTypes.PanLevel) {
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), 0, 1));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.Pitch) {
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), -3, 3));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.RandomPitch) {
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), 0, 6));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.DopplerLevel) {
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), 0, 5));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.Spread) {
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), 0, 360));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.Pan2D) {
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), -1, 1));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.Priority) {
					currentAudioOption.SetValue(EditorGUILayout.IntSlider("Value".ToGUIContent(), currentAudioOption.GetValue<int>(), 0, 255));
				}
				// Animation curve fields
				else if (currentAudioOption.type == AudioOption.OptionTypes.FadeInCurve) {
					ShowFadeInCurve();
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.FadeOutCurve) {
					ShowFadeOutCurve();
				}
				// Bool fields
				else if (AudioOption.BoolTypes.Contains(currentAudioOption.type)) {
					currentAudioOption.SetValue(EditorGUILayout.Toggle("Value".ToGUIContent(), currentAudioOption.GetValue<bool>()));
				}
				// Audio rolloff mode fields
				else if (AudioOption.RolloffModeTypes.Contains(currentAudioOption.type)) {
					currentAudioOption.SetValue((AudioRolloffMode)EditorGUILayout.EnumPopup("Value".ToGUIContent(), currentAudioOption.GetValue<AudioRolloffMode>()));
				}
				// Sync mode fields
				else if (AudioOption.SyncModeTypes.Contains(currentAudioOption.type)) {
					currentAudioOption.SetValue((SyncMode)EditorGUILayout.EnumPopup("Value".ToGUIContent(), currentAudioOption.GetValue<SyncMode>()));
				}
				// Audio clip fields
				else if (AudioOption.ClipTypes.Contains(currentAudioOption.type)) {
					currentAudioOption.SetValue((AudioClip)EditorGUILayout.ObjectField("Value".ToGUIContent(), currentAudioOption.GetValue<AudioClip>(), typeof(AudioClip), true));
				}
				
				EditorGUI.indentLevel -= 1;
			}
		}
		
		void ShowFadeInCurve() {
			currentAudioOption.SetValue(EditorGUILayout.CurveField("Value".ToGUIContent(), currentAudioOption.GetValue<AnimationCurve>(), Color.cyan, new Rect(0, 0, 1, 1)));
			AnimationCurve curve = currentAudioOption.GetValue<AnimationCurve>();
			if (curve.keys.Length < 2) {
				curve.keys = new []{ new Keyframe(0, 0), new Keyframe(1, 1) };
			}
			curve.MoveKey(0, new Keyframe(0, 0));
			curve.MoveKey(curve.keys.Length - 1, new Keyframe(1, 1));
		}
		
		void ShowFadeOutCurve() {
			currentAudioOption.SetValue(EditorGUILayout.CurveField("Value".ToGUIContent(), currentAudioOption.GetValue<AnimationCurve>(), Color.cyan, new Rect(0, 0, 1, 1)));
			AnimationCurve curve = currentAudioOption.GetValue<AnimationCurve>();
			if (curve.keys.Length < 2) {
				curve.keys = new []{ new Keyframe(0, 1), new Keyframe(1, 0) };
			}
			curve.MoveKey(0, new Keyframe(0, 1));
			curve.MoveKey(curve.keys.Length - 1, new Keyframe(1, 0));
		}
		
		void ShowSubSources() {
			if (AddFoldOut<AudioSetup>(subContainersProperty, currentSubContainer, "Sources".ToGUIContent(), currentSubContainer.childrenIds.Count, OnSubContainerSourceDropped)) {
				if (currentSubContainer.childrenIds.Count > 0) {
					currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioSubContainer(currentContainer, currentSubContainer.id, currentContainer.GetSubContainerWithID(currentSubContainer.childrenIds.Last()));
				}
				else {
					currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioSubContainer(currentContainer, currentSubContainer.id);
				}
				serializedObject.Update();
			}
		}
		
		void ShowButtons() {
			if (pdPlayer != null && sampler != null) {
				return;
			}
			
			Separator();
			
			if (pdPlayer == null) {
				pdIcon = pdIcon ?? HelperFunctions.LoadAssetInFolder<Texture>("pd.png", "PDPlayer");
				if (LargeButton(new GUIContent(" Add Pure Data Player", pdIcon), true)) {
					audioPlayer.AddComponent<PDPlayer>();
				}
			}
			
			if (sampler == null) {
				samplerIcon = samplerIcon ?? HelperFunctions.LoadAssetInFolder<Texture>("sampler.png", "Sampler");
				if (LargeButton(new GUIContent(" Add Sampler", samplerIcon), true)) {
					audioPlayer.AddComponent<Sampler>();
				}
			}
		}

		void AdjustContainerName() {
			switch (currentSubContainer.type) {
				case AudioSubContainer.Types.AudioSource:
					if (currentSubContainer.AudioSetup == null) {
						AdjustName("Audio Source: null", currentSubContainer, currentContainer);
					}
					else {
						AdjustName("Audio Source: " + currentSubContainer.AudioSetup.name, currentSubContainer, currentContainer);
					}
					break;
				case AudioSubContainer.Types.Sampler:
					if (sampler == null || sampler.editorHelper.instruments.Length == 0 || string.IsNullOrEmpty(currentSubContainer.instrumentName)) {
						AdjustName("Sampler: null", currentSubContainer, currentContainer);
					}
					else {
						AdjustName(string.Format("Sampler: {0} ({1} : {2})", currentSubContainer.instrumentName, currentSubContainer.note, currentSubContainer.velocity), currentSubContainer, currentContainer);
					}
					break;
				case AudioSubContainer.Types.MixContainer:
					AdjustName("Mix Container", currentSubContainer, currentContainer);
					break;
				case AudioSubContainer.Types.RandomContainer:
					AdjustName("Random Container", currentSubContainer, currentContainer);
					break;
				case AudioSubContainer.Types.SwitchContainer:
					AdjustName("Switch Container", currentSubContainer, currentContainer);
					break;
			}
		}
		
		void AdjustName(string prefix, AudioSubContainer subContainer, AudioContainer container) {
			subContainer.Name = prefix;
			
			if (subContainer.IsContainer) {
				subContainer.Name += string.Format(" ({0})", subContainer.childrenIds.Count);
			}
			
			if (GetParentContainerType(subContainer, container) == AudioSubContainer.Types.RandomContainer) {
				subContainer.Name += " | Weight: " + subContainer.weight;
			}
			else if (GetParentContainerType(subContainer, container) == AudioSubContainer.Types.SwitchContainer) {
				subContainer.Name += " | State: " + subContainer.stateName;
			}
		}
		
		GUIStyle GetContainerStyle(AudioSubContainer.Types type) {
			GUIStyle style = new GUIStyle("foldout");
			style.fontStyle = FontStyle.Bold;
			Color textColor = style.normal.textColor;
			
			switch (type) {
				case AudioSubContainer.Types.AudioSource:
					textColor = new Color(1, 0.5F, 0.3F, 10);
					break;
				case AudioSubContainer.Types.Sampler:
					textColor = new Color(1, 0, 1, 10);
					break;
				case AudioSubContainer.Types.MixContainer:
					textColor = new Color(0, 1, 1, 10);
					break;
				case AudioSubContainer.Types.RandomContainer:
					textColor = new Color(1, 1, 0, 10);
					break;
				case AudioSubContainer.Types.SwitchContainer:
					textColor = new Color(0.5F, 1, 0.3F, 10);
					break;
			}
			
			style.normal.textColor = textColor * 0.7F;
			style.onNormal.textColor = textColor * 0.7F;
			style.focused.textColor = textColor * 0.85F;
			style.onFocused.textColor = textColor * 0.85F;
			style.active.textColor = textColor * 0.85F;
			style.onActive.textColor = textColor * 0.85F;
			
			return style;
		}
		
		AudioSubContainer.Types GetParentContainerType(AudioSubContainer subContainer, AudioContainer container) {
			AudioSubContainer.Types type = AudioSubContainer.Types.AudioSource;
			
			if (subContainer.parentId != 0) {
				type = container.GetSubContainerWithID(subContainer.parentId).type;
			}
			else {
				type = ContainerToSubContainerType(container.type);
			}
			
			return type;
		}
		
		AudioSubContainer.Types ContainerToSubContainerType(AudioContainer.Types type) {
			return (AudioSubContainer.Types)Enum.Parse(typeof(AudioSubContainer.Types), type.ToString());
		}
	}
}