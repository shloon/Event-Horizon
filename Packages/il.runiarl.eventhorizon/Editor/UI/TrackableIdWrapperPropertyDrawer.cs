using UnityEditor;
using UnityEngine;

namespace EventHorizon
{
	[CustomPropertyDrawer(typeof(TrackableIDWrapper))]
	public class TrackableIdPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), new GUIContent());
			EditorGUI.EndProperty();
		}
	}
}