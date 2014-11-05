using UnityEngine;
using UnityEditor;

namespace Magicolo.EditorTools {
	[CustomPropertyDrawer(typeof(MinAttribute))]
	public class MinDrawer : CustomPropertyDrawerBase {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			drawPrefixLabel = false;
			position = Begin(position, property, label);
			float min = ((MinAttribute)attribute).min;
		
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, property, label, true);
			if (EditorGUI.EndChangeCheck()) {
				property.Min(min);
			}
			End(property);
		}
	}
}