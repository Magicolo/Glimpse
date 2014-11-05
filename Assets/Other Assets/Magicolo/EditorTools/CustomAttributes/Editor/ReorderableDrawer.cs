using UnityEngine;
using UnityEditor;

namespace Magicolo.EditorTools {
	[CustomPropertyDrawer(typeof(ReorderableAttribute))]
	public class ReorderableDrawer : CustomPropertyDrawerBase {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			drawPrefixLabel = false;
		
			position = Begin(position, property, label);
		
			if (!fieldInfo.FieldType.IsArray) {
				Debug.LogError("Field must be an array or list.");
				return;
			}
			if (property.propertyType == SerializedPropertyType.Generic){
				Debug.LogError("Array elements must be of type UnityEngine.Object.");
				return;
			}
			if (arrayProperty == null) {
				return;
			}
			
			EditorGUI.PropertyField(position, property, label, true);
			position.height = 16;
			Reorderable(arrayProperty, index, EditorGUI.IndentedRect(position));
			
			End(property);
		}
		
		public void Reorderable(SerializedProperty arrayProperty, int index, Rect dragArea) {
			string arrayIdentifier = arrayProperty.name + arrayProperty.arraySize + arrayProperty.depth + arrayProperty.propertyPath + arrayProperty.propertyType;
			SerializedProperty selectedArray = DragAndDrop.GetGenericData(arrayIdentifier) as SerializedProperty;
			int selectedIndex = DragAndDrop.GetGenericData("Selected Index") == null ? -1 : (int)DragAndDrop.GetGenericData("Selected Index");
			int targetIndex = DragAndDrop.GetGenericData("Target Index") == null ? -1 : (int)DragAndDrop.GetGenericData("Target Index");
			GUIStyle selectedStyle = new GUIStyle("TL SelectionButton PreDropGlow");
			GUIStyle targetStyle = new GUIStyle("ColorPickerBox");
			
			switch (currentEvent.type) {
				case EventType.MouseDown:
					if (dragArea.Contains(currentEvent.mousePosition)) {
						DragAndDrop.PrepareStartDrag();
						DragAndDrop.SetGenericData(arrayIdentifier, arrayProperty);
						DragAndDrop.SetGenericData("Selected Index", index);
						DragAndDrop.objectReferences = new Object[1];
						currentEvent.Use();
					}
					break;
				case EventType.MouseDrag:
					if (selectedArray != null && selectedIndex == index) {
						DragAndDrop.StartDrag(string.Format("Dragging array element {0} at index {1}.", arrayProperty.name, index));
						currentEvent.Use();
					}
					break;
				case EventType.MouseUp:
					if (selectedArray != null && selectedIndex == index) {
						DragAndDrop.PrepareStartDrag();
						currentEvent.Use();
					}
					break;
				case EventType.DragUpdated:
					if (selectedArray != null) {
						if (selectedIndex == index) {
							GUI.Label(dragArea, GUIContent.none, selectedStyle);
						}
						else if (selectedIndex != -1 && dragArea.Contains(currentEvent.mousePosition)) {
							DragAndDrop.visualMode = DragAndDropVisualMode.Move;
							DragAndDrop.SetGenericData("Target Index", index);
							GUI.Label(dragArea, GUIContent.none, targetStyle);
							currentEvent.Use();
						}
					}
					break;
				case EventType.DragPerform:
					if (selectedArray != null && selectedIndex != -1 && dragArea.Contains(currentEvent.mousePosition)) {
						DragAndDrop.AcceptDrag();
						ReorderArray(arrayProperty, selectedIndex, targetIndex);
						currentEvent.Use();
					}
					break;
				case EventType.DragExited:
					if (selectedArray != null && selectedIndex == index) {
						DragAndDrop.PrepareStartDrag();
						currentEvent.Use();
					}
					break;
				case EventType.Repaint:
					if (selectedArray != null) {
						if (selectedIndex == index) {
							GUI.Label(dragArea, GUIContent.none, selectedStyle);
						}
						else if (selectedIndex != -1 && dragArea.Contains(currentEvent.mousePosition)) {
							DragAndDrop.visualMode = DragAndDropVisualMode.Move;
							DragAndDrop.SetGenericData("Target Index", index);
							GUI.Label(dragArea, GUIContent.none, targetStyle);
						}
					}
					break;
			}
		}
		
		public void ReorderArray(SerializedProperty arrayProperty, int soureceIndex, int targetIndex) {
			arrayProperty.MoveArrayElement(soureceIndex, targetIndex);
			arrayProperty.serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(arrayProperty.serializedObject.targetObject);
		}
	}
}
