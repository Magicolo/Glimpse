using System;
using System.Reflection;
using System.Runtime.Remoting;
using UnityEngine;
using UnityEditor;

namespace Magicolo.EditorTools {
	[CustomPropertyDrawer(typeof(PropertyFieldAttribute))]
	public class PropertyFieldDrawer : CustomPropertyDrawerBase {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			drawPrefixLabel = false;
			Type attributeType = ((PropertyFieldAttribute)attribute).attributeType;
			object[] arguments = ((PropertyFieldAttribute)attribute).arguments;
			PropertyDrawer drawerOverride = null;
		
			if (fieldInfo.FieldType.IsArray) {
				Debug.LogError(string.Format("{0} should not be applied to arrays or lists.", attribute.GetType().Name));
				return;
			}
		
			if (attributeType != null) {
				drawerOverride = GetPropertyDrawer(attributeType, arguments);
			}
		
			position = Begin(position, property, label);
		
			EditorGUI.BeginChangeCheck();
			
			if (drawerOverride == null) {
				EditorGUI.PropertyField(position, property, label, true);
			}
			else {
				drawerOverride.OnGUI(position, property, label);
			}
		
			if (EditorGUI.EndChangeCheck()) {
				string propertyPath = property.propertyPath.Replace("Array.data", "").Replace("[", "").Replace("]", "");
				string[] propertyPathSplit = propertyPath.Split('.');
				propertyPathSplit[propertyPathSplit.Length - 1] = propertyPathSplit.Last().Capitalized();
				propertyPath = propertyPathSplit.Concat(".");
				property.serializedObject.targetObject.SetValueToMemberAtPath(propertyPath, property.GetValue());
			}
		
			End(property);
		}
	}
}
