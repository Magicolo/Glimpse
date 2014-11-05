#if UNITY_EDITOR
using System;
using System.Collections;
using Magicolo.EditorTools;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Magicolo.AudioTools{
	[CustomEditor(typeof(AudioPlayer))]
	public class AudioPlayerEditor : CustomEditorBase {
		
		AudioPlayer audioPlayer;
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
		
		public override void OnInspectorGUI(){
			audioPlayer = (AudioPlayer) target;
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
			EditorGUILayout.PropertyField(generalSettingsProperty.FindPropertyRelative("maxVoices"), new GUIContent("Max Voices", "Limits the amount of simultaneous playing audio sources to this value, by removing the oldest audio sources. Audio sources with the 'Do Not Kill' option will be ignored."));
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			EditorGUILayout.PropertyField(generalSettingsProperty.FindPropertyRelative("persistent"), new GUIContent("Persistent", "If true, the AudioPlayer will not be destroyed when loading a scene. Note that this will prevent any other AudioPlayer from other scenes to be loaded."));
			EditorGUI.EndDisabledGroup();
		}
		
		void ShowContainers(){
			containersProperty = generalSettingsProperty.FindPropertyRelative("containers");
			
			if (AddElementFoldOut(containersProperty, "Containers".ToGUIContent())){
				generalSettings.containers[generalSettings.containers.Length - 1] = new AudioContainer("");
				generalSettings.containers.Last().SetUniqueName("default", generalSettings.containers);
			}
			
			if (containersProperty.isExpanded){
				EditorGUI.indentLevel += 1;
				
				for (int i = 0; i < generalSettings.containers.Length; i++) {
					currentContainer = generalSettings.containers[i];
					currentContainerProperty = containersProperty.GetArrayElementAtIndex(i);
					
					BeginBox();
					
					GUIStyle style = new GUIStyle("foldout");
					style.fontStyle = FontStyle.Bold;
					if (DeleteElementFoldOutWithArrows(containersProperty, i, currentContainer.Name.ToGUIContent(), style)){
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
			
			if (currentContainerProperty.isExpanded){
				EditorGUI.indentLevel += 1;
				
				EditorGUI.BeginChangeCheck();
				string containerName = EditorGUILayout.TextField(currentContainer.Name);
				if (EditorGUI.EndChangeCheck()){
					currentContainer.SetUniqueName(containerName, currentContainer.Name, "default", generalSettings.containers);
				}
				currentContainer.type = (AudioContainer.Types)EditorGUILayout.EnumPopup(currentContainer.type);
				
				BeginBox();
				
				if (AddElementFoldOut(subContainersProperty, "Sources".ToGUIContent(), currentContainer.childrenIds.Count)){
					currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioSubContainer(currentContainer, 0);
				}
				
				if (subContainersProperty.isExpanded){
					EditorGUI.indentLevel += 1;
						
					ShowSubContainers(currentContainer.childrenIds);
					
					EditorGUI.indentLevel -= 1;
				}
				
				EndBox();
				
				Separator();
				
				EditorGUI.indentLevel -= 1;
			}
		}

		void ShowSubContainers(List<int> ids){
			foreach (int id in ids) {
				currentSubContainer = currentContainer.GetSubContainerWithID(id);
				currentSubContainerIndex = currentContainer.subContainers.IndexOf(currentSubContainer);
				currentSubContainerProperty = subContainersProperty.GetArrayElementAtIndex(currentSubContainerIndex);
				
				if (DeleteElementFoldOut(subContainersProperty, currentSubContainerIndex, currentSubContainer.Name.ToGUIContent())){
					currentSubContainer.Remove(currentContainer);
					break;
				}
				ShowSubContainer();
			}
		}
		
		void ShowSubContainer() {
			AdjustContainerName();
			
			if (currentSubContainerProperty.isExpanded){
				EditorGUI.indentLevel += 1;
				
				ShowGeneralContainerSettings();
				
				switch (currentSubContainer.type) {
					case AudioSubContainer.Types.AudioSource:
						ShowAudioSource();
						break;
					case AudioSubContainer.Types.MixContainer:
						ShowMixContainer();
						break;
					case AudioSubContainer.Types.RandomContainer:
						ShowRandomContainer();
						break;
				}
				
				EditorGUI.indentLevel -= 1;
			}
		}	

		void ShowAudioSource() {
			PropertyObjectField<AudioOptions>(currentSubContainerProperty.FindPropertyRelative("audioOptions"), "Source".ToGUIContent());
			if (currentSubContainer.audioOptions != null && currentSubContainer.audioOptions.audioInfo != null && currentSubContainer.audioOptions.audioInfo.Source != null && currentSubContainer.audioOptions.audioInfo.Clip != null){
				ShowAudioOptions();
			}
		}

		void ShowMixContainer() {
			BeginBox();
			
			if (AddElementFoldOut(subContainersProperty, currentSubContainer, "Sources".ToGUIContent(), currentSubContainer.childrenIds.Count)){
				currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioSubContainer(currentContainer, currentSubContainer.id);
			}
			
			if (currentSubContainer.Showing && currentContainer.childrenIds.Count > 0){
				EditorGUI.indentLevel += 1;
					
				ShowSubContainers(currentSubContainer.childrenIds);
				
				EditorGUI.indentLevel -= 1;
			}
			
			EndBox();
			
			Separator();
		}	

		void ShowRandomContainer() {
			BeginBox();
			
			if (AddElementFoldOut(subContainersProperty, currentSubContainer, "Sources".ToGUIContent(), currentSubContainer.childrenIds.Count)){
				currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioSubContainer(currentContainer, currentSubContainer.id);
			}
			
			if (currentSubContainer.Showing && currentContainer.childrenIds.Count > 0){
				EditorGUI.indentLevel += 1;
					
				ShowSubContainers(currentSubContainer.childrenIds);
				
				EditorGUI.indentLevel -= 1;
			}
			
			EndBox();
		}
		
		void ShowGeneralContainerSettings(){
			currentSubContainer.type = (AudioSubContainer.Types)EditorGUILayout.EnumPopup(currentSubContainer.type);
			
			if (GetParentContainerType(currentSubContainer, currentContainer) == AudioSubContainer.Types.RandomContainer){
				EditorGUILayout.PropertyField(currentSubContainerProperty.FindPropertyRelative("weight"));
			}
		}
	
		void ShowAudioOptions(){
			SerializedProperty audioOptionsProperty = currentSubContainerProperty.FindPropertyRelative("options");
			
			if (AddElementFoldOut(audioOptionsProperty, "Audio Options".ToGUIContent())){
				currentSubContainer.options[currentSubContainer.options.Length - 1].SetDefaultValue();
			}
			
			if (currentSubContainer.options != null && audioOptionsProperty.isExpanded){
				EditorGUI.indentLevel += 1;
				
				for (int i = 0; i < currentSubContainer.options.Length; i++) {
					currentAudioOption = currentSubContainer.options[i];
					currentAudioOptionProperty = audioOptionsProperty.GetArrayElementAtIndex(i);
					
					if (DeleteElementFoldOutWithArrows(audioOptionsProperty, i, string.Format("{0} | {1}", currentAudioOption.type, currentAudioOption.GetValue()).ToGUIContent())){
						break;
					}
				
					ShowAudioOption();
				}
				
				EditorGUI.indentLevel -= 1;
			}
		}
		
		void ShowAudioOption(){
			if (currentAudioOptionProperty.isExpanded){
				EditorGUI.indentLevel += 1;
				
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(currentAudioOptionProperty.FindPropertyRelative("type"));
				if (EditorGUI.EndChangeCheck()){
					serializedObject.ApplyModifiedProperties();
					currentAudioOption.SetDefaultValue();
				}
				
				// Float fields
				if (currentAudioOption.type == AudioOption.OptionTypes.FadeIn){
					currentAudioOption.SetValue(Mathf.Clamp(EditorGUILayout.FloatField("Value".ToGUIContent(), currentAudioOption.GetValue<float>()), 0, currentSubContainer.audioOptions.audioInfo.Clip.length - currentSubContainer.audioOptions.audioInfo.fadeOut));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.FadeOut){
					currentAudioOption.SetValue(Mathf.Clamp(EditorGUILayout.FloatField("Value".ToGUIContent(), currentAudioOption.GetValue<float>()), 0, currentSubContainer.audioOptions.audioInfo.Clip.length - currentSubContainer.audioOptions.audioInfo.fadeIn));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.Delay){
					currentAudioOption.SetValue(Mathf.Max(EditorGUILayout.FloatField("Value".ToGUIContent(), currentAudioOption.GetValue<float>()), 0));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.MinDistance){
					currentAudioOption.SetValue(Mathf.Clamp(EditorGUILayout.FloatField("Value".ToGUIContent(), currentAudioOption.GetValue<float>()), 0, currentSubContainer.audioOptions.audioInfo.Source.maxDistance));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.MaxDistance){
					currentAudioOption.SetValue(Mathf.Max(EditorGUILayout.FloatField("Value".ToGUIContent(), currentAudioOption.GetValue<float>()), currentSubContainer.audioOptions.audioInfo.Source.minDistance + currentSubContainer.audioOptions.audioInfo.Source.minDistance / 10));
				}
				// Slider fields
				else if (currentAudioOption.type == AudioOption.OptionTypes.Volume || currentAudioOption.type == AudioOption.OptionTypes.RandomVolume || currentAudioOption.type == AudioOption.OptionTypes.PanLevel){
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), 0, 1));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.Pitch){
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), -3, 3));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.RandomPitch){
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), 0, 6));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.DopplerLevel){
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), 0, 5));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.Spread){
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), 0, 360));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.Pan2D){
					currentAudioOption.SetValue(EditorGUILayout.Slider("Value".ToGUIContent(), currentAudioOption.GetValue<float>(), -1, 1));
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.Priority){
					currentAudioOption.SetValue(EditorGUILayout.IntSlider("Value".ToGUIContent(), currentAudioOption.GetValue<int>(), 0, 255));
				}
				// Animation curve fields
				else if (currentAudioOption.type == AudioOption.OptionTypes.FadeInCurve){
					ShowFadeInCurve();
				}
				else if (currentAudioOption.type == AudioOption.OptionTypes.FadeOutCurve){
					ShowFadeOutCurve();
				}
				// Bool fields
				else if (AudioOption.BoolTypes.Contains(currentAudioOption.type)){
					currentAudioOption.SetValue(EditorGUILayout.Toggle("Value".ToGUIContent(), currentAudioOption.GetValue<bool>()));
				}
				// Audio rolloff mode fields
				else if (AudioOption.RolloffModeTypes.Contains(currentAudioOption.type)){
					currentAudioOption.SetValue((AudioRolloffMode)EditorGUILayout.EnumPopup("Value".ToGUIContent(), currentAudioOption.GetValue<AudioRolloffMode>()));
				}
				// Sync mode fields
				else if (AudioOption.SyncModeTypes.Contains(currentAudioOption.type)){
					currentAudioOption.SetValue((SyncMode)EditorGUILayout.EnumPopup("Value".ToGUIContent(), currentAudioOption.GetValue<SyncMode>()));
				}
				// Audio clip fields
				else if (AudioOption.ClipTypes.Contains(currentAudioOption.type)){
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
		
		void ShowButtons() {
			PDPlayer pdPlayer = FindObjectOfType<PDPlayer>();
			
			if (pdPlayer != null) {
				return;
			}
			
			Separator();
			
			if (pdPlayer == null) {
				if (LargeButton("Add Pure Data Player".ToGUIContent())) {
					audioPlayer.AddComponent<PDPlayer>();
				}
			}
		}

		void AdjustContainerName() {
			switch (currentSubContainer.type) {
				case AudioSubContainer.Types.AudioSource:
					if (currentSubContainer.audioOptions == null) {
						AdjustName("Audio Source: null", currentSubContainer, currentContainer);
					}
					else {
						AdjustName("Audio Source: " + currentSubContainer.audioOptions.name, currentSubContainer, currentContainer);
					}
					break;
				case AudioSubContainer.Types.MixContainer:
					AdjustName("Mix Container", currentSubContainer, currentContainer);
					break;
				case AudioSubContainer.Types.RandomContainer:
					AdjustName("Random Container", currentSubContainer, currentContainer);
					break;
			}
		}		
		
		void AdjustName(string prefix, AudioSubContainer subContainer, AudioContainer container){
			subContainer.Name = prefix;
			
			if (subContainer.type == AudioSubContainer.Types.MixContainer || subContainer.type == AudioSubContainer.Types.RandomContainer){
				subContainer.Name += string.Format(" ({0})", subContainer.childrenIds.Count);
			}
			
			if (GetParentContainerType(subContainer, container) == AudioSubContainer.Types.RandomContainer){
				subContainer.Name += " | Weight: " + subContainer.weight;
			}
		}
	
		AudioSubContainer.Types GetParentContainerType(AudioSubContainer subContainer, AudioContainer container){
			AudioSubContainer.Types type = AudioSubContainer.Types.AudioSource;
			
			if (subContainer.parentId != 0){
				type = container.GetSubContainerWithID(subContainer.parentId).type;
			}
			else if (container.type == AudioContainer.Types.MixContainer){
				type = AudioSubContainer.Types.MixContainer;
			}
			else if (container.type == AudioContainer.Types.RandomContainer){
				type = AudioSubContainer.Types.RandomContainer;
			}
			
			return type;
		}
	}
}
#endif