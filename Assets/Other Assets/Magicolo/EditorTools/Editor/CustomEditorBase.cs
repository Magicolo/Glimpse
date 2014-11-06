using System.Collections;
using Magicolo.GeneralTools;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Magicolo.EditorTools {
	public class CustomEditorBase : Editor {
	
		public enum ButtonAlign {
			None,
			Left,
			Right,
			Center
		}
		
		public delegate void AddCallback(SerializedProperty arrayProperty);
		public delegate void DeleteCallback(SerializedProperty arrayProperty, int indexToRemove);
		public delegate void DropCallback<T>(T droppedObject);
		public delegate void ReorderCallback(SerializedProperty arrayProperty, int sourceIndex, int targetIndex);
		
		public Event currentEvent;
		public bool deleteBreak;

		public virtual void OnEnable() {
		}
		
		public virtual void OnDisable() {
		}
		
		public virtual void Begin(bool space = true) {
			currentEvent = Event.current;
		
			if (space) {
				EditorGUILayout.Space();
			}
			
			deleteBreak = false;
			Undo.RecordObject(target, string.Format("{0} ({1}) modified.", target.name, target.GetType()));
			EditorGUI.BeginChangeCheck();
			serializedObject.Update();
		}
	
		public virtual void End(bool space = true) {
			if (space) {
				EditorGUILayout.Space();
			}
			
			serializedObject.ApplyModifiedProperties();
			if (EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(target);
			}
		}
			
		#region Buttons
		public bool Button(GUIContent label, GUIStyle style, ButtonAlign align, bool disableOnPlay, params GUILayoutOption[] options) {
			if (style == null) {
				style = new GUIStyle("MiniToolbarButton");
				style.clipping = TextClipping.Overflow;
				style.contentOffset = new Vector2(2, 0);
			}
			
			EditorGUI.BeginDisabledGroup(Application.isPlaying && disableOnPlay);
			EditorGUILayout.BeginVertical();
			GUILayout.Space(1);
			EditorGUILayout.BeginHorizontal();
			
			if (align == ButtonAlign.Right || align == ButtonAlign.Center) {
				EditorGUILayout.Space();
			}
			
			bool pressed = GUILayout.Button(label, style, options);
			
			if (align == ButtonAlign.Left || align == ButtonAlign.Center) {
				EditorGUILayout.Space();
			}
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			EditorGUI.EndDisabledGroup();
			
			return pressed;
		}
		
		public bool Button(GUIContent label, ButtonAlign align, bool disableOnPlay, params GUILayoutOption[] options) {
			return Button(label, null, align, disableOnPlay, options);
		}
		
		public bool Button(GUIContent label, GUIStyle style, bool disableOnPlay, params GUILayoutOption[] options) {
			return Button(label, style, ButtonAlign.None, disableOnPlay, options);
		}
		
		public bool Button(GUIContent label, bool disableOnPlay, params GUILayoutOption[] options) {
			return Button(label, null, ButtonAlign.None, disableOnPlay, options);
		}
		
		public bool Button(GUIContent label, params GUILayoutOption[] options) {
			return Button(label, null, ButtonAlign.None, false, options);
		}

		public bool LargeButton(GUIContent label, bool disableOnPlay, params GUILayoutOption[] options) {
			bool pressed = false;
			
			GUIStyle style = new GUIStyle("toolbarButton");
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 10;
			style.clipping = TextClipping.Overflow;
			style.contentOffset = new Vector2(2, 0);
			
			pressed = Button(label, style, disableOnPlay, options);
			GUILayout.Space(2);
			
			return pressed;
		}
		
		public bool LargeAddButton(SerializedProperty property, GUIContent label, AddCallback addCallback, params GUILayoutOption[] options) {
			bool pressed = false;
			if (LargeButton(label, true, options)) {
				AddToArray(property, addCallback);
				pressed = true;
			}
			return pressed;
		}
	
		public bool LargeAddButton(SerializedProperty property, GUIContent label, params GUILayoutOption[] options) {
			return LargeAddButton(property, label, null, options);
		}

		public bool AddButton() {
			GUIStyle style = new GUIStyle("toolbarbutton");
			style.clipping = TextClipping.Overflow;
			style.contentOffset = new Vector2(0, -1);
			style.fontSize = 10;
			
			bool pressed = Button("+".ToGUIContent(), style, ButtonAlign.Right, true, GUILayout.Width(16));
			
			return pressed;
		}
		
		public bool AddButton(SerializedProperty property, AddCallback addCallback = null) {
			bool pressed = false;
			if (AddButton()) {
				AddToArray(property, addCallback);
				pressed = true;
			}
			return pressed;
		}
		
		public bool DeleteButton() {
			GUIStyle style = new GUIStyle("MiniToolbarButtonLeft");
			style.clipping = TextClipping.Overflow;
			
			bool pressed = Button("−".ToGUIContent(), style, ButtonAlign.Right, true, GUILayout.Width(16));
			
			return pressed;
		}
		
		public bool DeleteButton(SerializedProperty property, int indexToRemove, DeleteCallback deleteCallback = null) {
			bool pressed = false;
			if (DeleteButton()) {
				DeleteFromArray(property, indexToRemove, deleteCallback);
				pressed = true;
			}
			return pressed;
		}
		#endregion
		
		#region Foldouts
		public void Foldout(SerializedProperty property, GUIContent label, GUIStyle style) {
			style = style ?? EditorStyles.foldout;
			
			EditorGUILayout.BeginHorizontal();
			property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label, style);
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();
		}
		
		public void Foldout(SerializedProperty property, GUIContent label) {
			Foldout(property, label, null);
		}
		
		public void Foldout(IShowable showable, GUIContent label, GUIStyle style) {
			style = style ?? EditorStyles.foldout;
			
			EditorGUILayout.BeginHorizontal();
			showable.Showing = EditorGUILayout.Foldout(showable.Showing, label, style);
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();
		}
		
		public void Foldout(IShowable showable, GUIContent label) {
			Foldout(showable, label, null);
		}
		
		public void DropFoldout<T>(SerializedProperty property, GUIContent label, GUIStyle style, bool disableOnPlay, DropCallback<T> dropCallback) where T : Object {
			Rect dropArea = EditorGUILayout.BeginHorizontal();
			Foldout(property, label, style);
			EditorGUILayout.EndHorizontal();
			
			DropArea<T>(dropArea, disableOnPlay, dropCallback);
		}
		
		public void DropFoldout<T>(SerializedProperty property, GUIContent label, bool disableOnPlay, DropCallback<T> dropCallback) where T : Object {
			DropFoldout<T>(property, label, null, disableOnPlay, dropCallback);
		}
		
		public void DropFoldout<T>(SerializedProperty property, GUIContent label, DropCallback<T> dropCallback) where T : Object {
			DropFoldout<T>(property, label, null, false, dropCallback);
		}
		
		public void DropFoldout<T>(IShowable showable, GUIContent label, GUIStyle style, bool disableOnPlay, DropCallback<T> dropCallback) where T : Object {
			Rect dropArea = EditorGUILayout.BeginHorizontal();
			Foldout(showable, label, style);
			EditorGUILayout.EndHorizontal();
			
			DropArea<T>(dropArea, disableOnPlay, dropCallback);
		}
		
		public void DropFoldout<T>(IShowable showable, GUIContent label, bool disableOnPlay, DropCallback<T> dropCallback) where T : Object {
			DropFoldout<T>(showable, label, null, disableOnPlay, dropCallback);
		}
		
		public void DropFoldout<T>(IShowable showable, GUIContent label, DropCallback<T> dropCallback) where T : Object {
			DropFoldout<T>(showable, label, null, false, dropCallback);
		}
		
		public bool AddFoldOut(SerializedProperty property, IShowable showable, GUIContent label, GUIStyle style, int overrideArraySize, AddCallback addCallback = null) {
			label.text += string.Format(" ({0})", GetArraySize(property, overrideArraySize));
			
			EditorGUILayout.BeginHorizontal();
		
			if (showable.Showing && GetArraySize(property, overrideArraySize) == 0) {
				showable.Showing = false;
			}
		
			Foldout(showable, label, style);
		
			bool pressed = false;
			if (showable.Showing && GetArraySize(property, overrideArraySize) == 0 && !Application.isPlaying) {
				AddToArray(property, addCallback);
				pressed = true;
			}
		
			if (AddButton(property, addCallback)) {
				pressed = true;
			}
		
			EditorGUILayout.EndHorizontal();
			return pressed;
		}
	
		public bool AddFoldOut(SerializedProperty property, IShowable showable, GUIContent label, int overrideArraySize, AddCallback addCallback = null) {
			return AddFoldOut(property, showable, label, null, overrideArraySize, addCallback);
		}
	
		public bool AddFoldOut(SerializedProperty property, IShowable showable, GUIContent label, GUIStyle style, AddCallback addCallback = null) {
			return AddFoldOut(property, showable, label, style, -1, addCallback);
		}
	
		public bool AddFoldOut(SerializedProperty property, IShowable showable, GUIContent label, AddCallback addCallback = null) {
			return AddFoldOut(property, showable, label, null, -1, addCallback);
		}
		
		public bool AddFoldOut(SerializedProperty property, GUIContent label, GUIStyle style, int overrideArraySize, AddCallback addCallback = null) {
			label.text += string.Format(" ({0})", GetArraySize(property, overrideArraySize));
		
			EditorGUILayout.BeginHorizontal();
		
			if (property.isExpanded && GetArraySize(property, overrideArraySize) == 0) {
				property.isExpanded = false;
			}
			
			Foldout(property, label, style);
		
			bool pressed = false;
			if (property.isExpanded && GetArraySize(property, overrideArraySize) == 0 && !Application.isPlaying) {
				AddToArray(property, addCallback);
				pressed = true;
			}
		
			if (AddButton(property, addCallback)) {
				pressed = true;
			}
		
			EditorGUILayout.EndHorizontal();
			return pressed;
		}
	
		public bool AddFoldOut(SerializedProperty property, GUIContent label, int overrideArraySize, AddCallback addCallback = null) {
			return AddFoldOut(property, label, null, overrideArraySize, addCallback);
		}
	
		public bool AddFoldOut(SerializedProperty property, GUIContent label, GUIStyle style, AddCallback addCallback = null) {
			return AddFoldOut(property, label, style, -1, addCallback);
		}
	
		public bool AddFoldOut(SerializedProperty property, GUIContent label, AddCallback addCallback = null) {
			return AddFoldOut(property, label, null, -1, addCallback);
		}

		public bool AddFoldOut<T>(SerializedProperty property, IShowable showable, GUIContent label, GUIStyle style, int overrideArraySize, DropCallback<T> dropCallback, AddCallback addCallback = null) where T : Object {
			Rect dropArea = EditorGUILayout.BeginHorizontal();
			bool pressed = AddFoldOut(property, showable, label, style, overrideArraySize, addCallback);
			EditorGUILayout.EndHorizontal();
			
			DropArea<T>(dropArea, true, dropCallback);

			return pressed;
		}
		
		public bool AddFoldOut<T>(SerializedProperty property, IShowable showable, GUIContent label, int overrideArraySize, DropCallback<T> dropCallback, AddCallback addCallback = null) where T : Object {
			return AddFoldOut<T>(property, showable, label, null, overrideArraySize, dropCallback, addCallback);
		}
		
		public bool AddFoldOut<T>(SerializedProperty property, IShowable showable, GUIContent label, GUIStyle style, DropCallback<T> dropCallback, AddCallback addCallback = null) where T : Object {
			return AddFoldOut<T>(property, showable, label, style, -1, dropCallback, addCallback);
		}
		
		public bool AddFoldOut<T>(SerializedProperty arrayProperty, IShowable showable, GUIContent label, DropCallback<T> dropCallback, AddCallback addCallback = null) where T : Object {
			return AddFoldOut<T>(arrayProperty, showable, label, null, -1, dropCallback, addCallback);
		}
		
		public bool AddFoldOut<T>(SerializedProperty arrayProperty, GUIContent label, GUIStyle style, int overrideArraySize, DropCallback<T> dropCallback, AddCallback addCallback = null) where T : Object {
			Rect dropArea = EditorGUILayout.BeginVertical();
			bool pressed = AddFoldOut(arrayProperty, label, style, overrideArraySize, addCallback);
			EditorGUILayout.EndVertical();
			
			DropArea<T>(dropArea, true, dropCallback);

			return pressed;
		}
				
		public bool AddFoldOut<T>(SerializedProperty property, GUIContent label, int overrideArraySize, DropCallback<T> dropCallback, AddCallback addCallback = null) where T : Object {
			return AddFoldOut<T>(property, label, null, overrideArraySize, dropCallback, addCallback);
		}
		
		public bool AddFoldOut<T>(SerializedProperty arrayProperty, GUIContent label, GUIStyle style, DropCallback<T> dropCallback, AddCallback addCallback = null) where T : Object {
			return AddFoldOut<T>(arrayProperty, label, style, -1, dropCallback, addCallback);
		}
		
		public bool AddFoldOut<T>(SerializedProperty arrayProperty, GUIContent label, DropCallback<T> dropCallback, AddCallback addCallback = null) where T : Object {
			return AddFoldOut<T>(arrayProperty, label, null, -1, dropCallback, addCallback);
		}
		
		public bool DeleteFoldOut(SerializedProperty arrayProperty, int index, GUIContent label, GUIStyle style, ReorderCallback reorderCallback, DeleteCallback deleteCallback = null) {
			EditorGUILayout.BeginHorizontal();
		
			SerializedProperty elementProperty = arrayProperty.GetArrayElementAtIndex(index);
			Foldout(elementProperty, label, style);
			bool pressed = DeleteButton(arrayProperty, index, deleteCallback);
		
			EditorGUILayout.EndHorizontal();
			
			if (!pressed) {
				Reorderable(arrayProperty, index, reorderCallback);
			}
		
			return pressed;
		}
		
		public bool DeleteFoldOut(SerializedProperty arrayProperty, int index, GUIContent label, GUIStyle style, DeleteCallback deleteCallback = null) {
			return DeleteFoldOut(arrayProperty, index, label, style, null, deleteCallback);
		}
	
		public bool DeleteFoldOut(SerializedProperty arrayProperty, int index, GUIContent label, ReorderCallback reorderCallback, DeleteCallback deleteCallback = null) {
			return DeleteFoldOut(arrayProperty, index, label, null, reorderCallback, deleteCallback);
		}
	
		public bool DeleteFoldOut(SerializedProperty arrayProperty, int index, GUIContent label, DeleteCallback deleteCallback = null) {
			return DeleteFoldOut(arrayProperty, index, label, null, null, deleteCallback);
		}
	
		public bool DeleteFoldOut<T>(SerializedProperty arrayProperty, int index, GUIContent label, GUIStyle style, DropCallback<T> dropCallback, ReorderCallback reorderCallback, DeleteCallback deleteCallback = null) where T : Object {
			Rect dropArea = EditorGUILayout.BeginHorizontal();
			bool pressed = DeleteFoldOut(arrayProperty, index, label, style, reorderCallback, deleteCallback);
			EditorGUILayout.EndHorizontal();
			
			DropArea<T>(dropArea, true, dropCallback);
			
			return pressed;
		}
		
		public bool DeleteFoldOut<T>(SerializedProperty arrayProperty, int index, GUIContent label, GUIStyle style, DropCallback<T> dropCallback, DeleteCallback deleteCallback = null) where T : Object {
			return DeleteFoldOut<T>(arrayProperty, index, label, style, dropCallback, null, deleteCallback);
		}
			
		public bool DeleteFoldOut<T>(SerializedProperty arrayProperty, int index, GUIContent label, DropCallback<T> dropCallback, ReorderCallback reorderCallback, DeleteCallback deleteCallback = null) where T : Object {
			return DeleteFoldOut<T>(arrayProperty, index, label, null, dropCallback, reorderCallback, deleteCallback);
		}
			
		public bool DeleteFoldOut<T>(SerializedProperty arrayProperty, int index, GUIContent label, DropCallback<T> dropCallback, DeleteCallback deleteCallback = null) where T : Object {
			return DeleteFoldOut<T>(arrayProperty, index, label, null, dropCallback, null, deleteCallback);
		}
		#endregion

		#region Miscellaneous
		public void PropertyObjectField<T>(SerializedProperty property, GUIContent label, bool disableOnPlay, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object {
			EditorGUI.BeginDisabledGroup(Application.isPlaying && disableOnPlay);
			EditorGUILayout.BeginHorizontal();
			
			EditorGUI.BeginChangeCheck();
			property.objectReferenceValue = EditorGUILayout.ObjectField(label, property.objectReferenceValue, typeof(T), allowSceneObjects, options);
			if (EditorGUI.EndChangeCheck()) {
				property.serializedObject.ApplyModifiedProperties();
			}
			
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
		}
	
		public void PropertyObjectField<T>(SerializedProperty property, GUIContent label, bool disableOnPlay, params GUILayoutOption[] options) where T : Object {
			PropertyObjectField<T>(property, label, disableOnPlay, true, options);
		}
	
		public void PropertyObjectField<T>(SerializedProperty property, GUIContent label, params GUILayoutOption[] options) where T : Object {
			PropertyObjectField<T>(property, label, false, true, options);
		}
	
		public void MinMaxSlider(SerializedProperty minProperty, SerializedProperty maxProperty, float min, float max, bool disableOnPlay) {
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			float minValue = 0;
			if (minProperty.propertyType == SerializedPropertyType.Integer) {
				minValue = (int)minProperty.GetValue();
			}
			else if (minProperty.propertyType == SerializedPropertyType.Float) {
				minValue = (float)minProperty.GetValue();
			}
			
			float maxValue = 0;
			if (maxProperty.propertyType == SerializedPropertyType.Integer) {
				maxValue = (int)maxProperty.GetValue();
			}
			else if (maxProperty.propertyType == SerializedPropertyType.Float) {
				maxValue = (float)maxProperty.GetValue();
			}
			
			EditorGUI.BeginDisabledGroup(Application.isPlaying && disableOnPlay);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.Width(indent * 15));
			
			minValue = EditorGUILayout.FloatField(minValue, GUILayout.MaxWidth((Screen.width - EditorGUI.indentLevel * 15) * 0.125F));
			EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, min, max);
			maxValue = EditorGUILayout.FloatField(maxValue, GUILayout.MaxWidth((Screen.width - EditorGUI.indentLevel * 15) * 0.125F));
			
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
			
			minValue = Mathf.Clamp(minValue, min, maxValue);
			maxValue = Mathf.Clamp(maxValue, minValue, max);
			
			if (minProperty.propertyType == SerializedPropertyType.Integer) {
				minProperty.SetValue((int)minValue);
			}
			else if (minProperty.propertyType == SerializedPropertyType.Float) {
				minProperty.SetValue(minValue);
			}
			
			if (maxProperty.propertyType == SerializedPropertyType.Integer) {
				maxProperty.SetValue((int)maxValue);
			}
			else if (maxProperty.propertyType == SerializedPropertyType.Float) {
				maxProperty.SetValue(maxValue);
			}
			
			EditorGUI.indentLevel = indent;
		}
		
		public void MinMaxSlider(SerializedProperty minProperty, SerializedProperty maxProperty, float min, float max) {
			MinMaxSlider(minProperty, maxProperty, min, max, false);
		}
		
		public string Popup(GUIContent label, string currentOption, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options){
			style = style ?? new GUIStyle("popup");
			int currentIndex = System.Array.IndexOf(displayedOptions, currentOption);
			return displayedOptions[Mathf.Clamp(EditorGUILayout.Popup(label, currentIndex, displayedOptions.ToGUIContents(), style, options), 0, Mathf.Max(displayedOptions.Length - 1, 0))];
		}
				
		public string Popup(GUIContent label, string currentOption, string[] displayedOptions, params GUILayoutOption[] options){
			return Popup(label, currentOption, displayedOptions, null, options);
		}
		
		public void DropArea<T>(Rect dropArea, bool disableOnPlay, DropCallback<T> dropCallback) where T : Object {
			if (Application.isPlaying && disableOnPlay) {
				return;
			}
			
			if (dropArea.Contains(currentEvent.mousePosition)) {
				if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0) {
					GameObject gameObject = DragAndDrop.objectReferences[0] as GameObject;
					T dropTarget = DragAndDrop.objectReferences[0] as T ?? gameObject == null ? default(T) : gameObject.GetComponent(typeof(T)) as T;
					
					if (dropTarget != null) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}
				}
				
				if (currentEvent.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag();
						
					foreach (Object droppedElement in DragAndDrop.objectReferences) {
						GameObject gameObject = droppedElement as GameObject;
						T dropTarget = DragAndDrop.objectReferences[0] as T ?? gameObject == null ? default(T) : gameObject.GetComponent(typeof(T)) as T;
						if (dropTarget != null) {
							dropCallback(dropTarget);
						}
					}
					currentEvent.Use();
				}
			}
		}
		
		public void DropArea<T>(Rect dropArea, DropCallback<T> dropCallback) where T : Object {
			DropArea<T>(dropArea, false, dropCallback);
		}

		public void Reorderable(SerializedProperty arrayProperty, int index, Rect dragArea, ReorderCallback reorderCallback = null) {
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
						ReorderArray(arrayProperty, selectedIndex, targetIndex, reorderCallback);
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
		
		public void Reorderable(SerializedProperty arrayProperty, int index, ReorderCallback reorderCallback = null) {
			Reorderable(arrayProperty, index, EditorGUI.IndentedRect(GUILayoutUtility.GetLastRect()), reorderCallback);
		}

		public void Separator(bool reserveVerticalSpace = true) {
			if (reserveVerticalSpace) {
				GUILayout.Space(4);
				EditorGUILayout.LabelField(GUIContent.none, new GUIStyle("RL DragHandle"), GUILayout.Height(4));
				GUILayout.Space(4);
			}
			else {
				Rect position = EditorGUILayout.BeginVertical();
				position.y += 7;
				EditorGUI.LabelField(position, GUIContent.none, new GUIStyle("RL DragHandle"));
				EditorGUILayout.EndVertical();
			}
		}
		#endregion
		
		#region Utility
		public void BeginBox(GUIStyle style) {
			Rect rect = EditorGUILayout.BeginVertical();
			rect.width -= EditorGUI.indentLevel * 15 - 1;
			rect.height += 1;
			rect.x += EditorGUI.indentLevel * 15 + 1;
			
			GUI.Box(rect, "", style);
		}
	
		public void BeginBox() {
			BeginBox(new GUIStyle("box"));
		}
	
		public void EndBox() {
			EditorGUILayout.EndVertical();
		}

		public void AddToArray(SerializedProperty arrayProperty, AddCallback addCallback = null) {
			if (addCallback == null) {
				arrayProperty.arraySize += 1;
				arrayProperty.serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(arrayProperty.serializedObject.targetObject);
			}
			else {
				addCallback(arrayProperty);
			}
		}
	
		public void DeleteFromArray(SerializedProperty arrayProperty, int indexToRemove, DeleteCallback deleteCallback = null) {
			if (deleteCallback == null) {
				arrayProperty.DeleteArrayElementAtIndex(indexToRemove);
				arrayProperty.serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(arrayProperty.serializedObject.targetObject);
			}
			else {
				deleteCallback(arrayProperty, indexToRemove);
			}
			deleteBreak = true;
		}

		public void ReorderArray(SerializedProperty arrayProperty, int soureceIndex, int targetIndex, ReorderCallback reorderCallback = null) {
			if (reorderCallback == null) {
				arrayProperty.MoveArrayElement(soureceIndex, targetIndex);
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(arrayProperty.serializedObject.targetObject);
			}
			else {
				reorderCallback(arrayProperty, soureceIndex, targetIndex);
			}
		}
		
		public int GetArraySize(SerializedProperty property, int overrideArraySize) {
			int arraySize = property.arraySize;
			if (overrideArraySize >= 0) {
				arraySize = overrideArraySize;
			}
			return arraySize;
		}
		#endregion
	}
}