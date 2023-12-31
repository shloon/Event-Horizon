using UnityEditor;
using UnityEngine;

namespace EventHorizon.Editor
{
	[CustomPropertyDrawer(typeof(TrackableID))]
	public class TrackableIdPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			if (property.serializedObject.isEditingMultipleObjects)
			{
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.TextField(position, char.ConvertFromUtf32(0x00002015));
				EditorGUI.EndDisabledGroup();
			}
			else
			{
				// Calculate space for button
				var buttonRect = new Rect(position.x + position.width - 50, position.y, 50, position.height);
				position.width -= 55; // Adjust for button width

				// Display the GUID as a string
				var propertyValue = (TrackableID) property.boxedValue;
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.LabelField(position,
					propertyValue == TrackableID.Unassigned ? "Unassigned" : propertyValue.Internal.ToString(),
					EditorStyles.helpBox);
				EditorGUI.EndDisabledGroup();

				// Button to regenerate GUID
				if (GUI.Button(buttonRect, char.ConvertFromUtf32(0x000021BA)))
				{
					var trackableManager = Object.FindObjectOfType<TrackableManagerComponent>();
					if (trackableManager is null)
					{
						Debug.LogWarning(
							"Event Horizon: No TrackableManager was found in the current scene. Bailing out...");
					}
					else
					{
						var oldID = (TrackableID) property.boxedValue;
						var newID = trackableManager.GenerateId();

						var guidProp = property.serializedObject.FindProperty(property.propertyPath);
						property.boxedValue = newID;
						property.serializedObject.ApplyModifiedProperties();

						if (property.serializedObject.targetObject is ITrackable trackable)
						{
							if (trackableManager.RegisteredTrackables.ContainsKey(oldID))
							{
								// if the old ID was registered, it was claimed by this trackable, so we change it
								trackableManager.ChangeTrackableID(oldID, newID);
							}
							else
							{
								// if the old ID wasn't registered, this is a duplicate of an existing object,
								// so we now have to register it
								trackableManager.Register(trackable);
							}
						}
					}
				}
			}

			EditorGUI.EndProperty();
		}
	}
}