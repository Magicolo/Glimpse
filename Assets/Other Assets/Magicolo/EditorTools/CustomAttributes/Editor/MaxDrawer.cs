using UnityEngine;
using UnityEditor;

namespace Magicolo.EditorTools {
	[CustomPropertyDrawer(typeof(MaxAttribute))]
	public class MaxDrawer : CustomPropertyDrawerBase {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			drawPrefixLabel = false;
			position = Begin(position, property, label);
			float max = ((MaxAttribute)attribute).max;
		
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, property, label, true);
			if (EditorGUI.EndChangeCheck()) {
				property.Max(max);
			}
			End(property);
		}
	}
}