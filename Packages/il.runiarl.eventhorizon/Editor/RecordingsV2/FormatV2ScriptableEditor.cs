using System;
using System.Globalization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace EventHorizon.Editor.RecordingsV2
{
	[CustomEditor(typeof(FormatV2Scriptable))]
	public class FormatV2ScriptableEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var scriptable = (FormatV2Scriptable) target;
			var container = new VisualElement();

			var playButton = new Button { text = "Start Playback" };
			playButton.clicked += InspectionModeHook.TogglePlayModeAndSetupCleanup;
			container.Add(playButton);

			#region Metadata

			var metadataHeader = UIHelpers.CreateHeader("Metadata");
			container.Add(metadataHeader);

			var sceneNameField = new TextField
			{
				label = "Scene Name", isReadOnly = true, value = scriptable.metadataPacket.sceneName
			};
			sceneNameField.AddToClassList("unity-base-field__aligned");
			container.Add(sceneNameField);

			var fpsField = new TextField
			{
				label = "Frame Rate", isReadOnly = true, value = scriptable.metadataPacket.fps.ToString()
			};
			fpsField.AddToClassList("unity-base-field__aligned");
			container.Add(fpsField);

			var timestampField = new TextField
			{
				label = "Frame Rate",
				isReadOnly = true,
				value = DateTime.Parse(scriptable.metadataPacket.timestamp).ToString(CultureInfo.InvariantCulture)
			};
			timestampField.AddToClassList("unity-base-field__aligned");
			container.Add(timestampField);

			var framesLengthField = new IntegerField
			{
				label = "Number of Frames", value = scriptable.framePackets.Count, isReadOnly = true
			};
			framesLengthField.AddToClassList("unity-base-field__aligned");
			container.Add(framesLengthField);

			#endregion


			#region Raw Data

			var rawDataHeader = UIHelpers.CreateHeader("Raw Data");
			container.Add(rawDataHeader);

			var framePacketsField = new PropertyField(serializedObject.FindProperty("framePackets"));
			container.Add(framePacketsField);

			var genericDataPacketField = new PropertyField(serializedObject.FindProperty("genericDataPackets"));
			container.Add(genericDataPacketField);

			var transformPacketsField = new PropertyField(serializedObject.FindProperty("transformPackets"));
			container.Add(transformPacketsField);


			#endregion

			return container;
		}
	}
}