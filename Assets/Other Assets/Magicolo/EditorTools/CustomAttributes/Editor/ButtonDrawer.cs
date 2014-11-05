using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Magicolo.EditorTools {
	[CustomPropertyDrawer(typeof(ButtonAttribute))]
	public class ButtonDrawer : CustomPropertyDrawerBase {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			position = Begin(position, property, label);
		
			if (property.type == "UInt8") {
				string buttonLabel = string.IsNullOrEmpty(((ButtonAttribute)attribute).label) ? label.text : ((ButtonAttribute)attribute).label;
				string buttonPressedMethodName = ((ButtonAttribute)attribute).methodName;
				string buttonIndexVariableName = ((ButtonAttribute)attribute).indexVariableName;
				GUIStyle buttonStyle = ((ButtonAttribute)attribute).style;
			
				position = AttributeUtility.BeginIndentation(position);
			
				if (noFieldLabel) buttonLabel = "";
			
				bool pressed;
				if (buttonStyle != null) {
					pressed = GUI.Button(position, buttonLabel, buttonStyle);
				}
				else {
					pressed = GUI.Button(position, buttonLabel);
				}
			
				AttributeUtility.EndIndentation();
			
				if (pressed) {
					if (!string.IsNullOrEmpty(buttonIndexVariableName)) {
						property.serializedObject.FindProperty(buttonIndexVariableName).intValue = index;
					}
				
					if (!string.IsNullOrEmpty(buttonPressedMethodName)) {
						property.serializedObject.targetObject.InvokeMethod(buttonPressedMethodName);
					}
				
					EditorUtility.SetDirty(property.serializedObject.targetObject);
				}
				property.boolValue = pressed;
			}
			else {
				EditorGUI.LabelField(position, "Button variable must be of type boolean.");
			}
		
			End(property);
		}
	}
}
