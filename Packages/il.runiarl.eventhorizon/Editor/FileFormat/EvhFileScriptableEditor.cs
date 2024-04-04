using EventHorizon.FileFormat;
using System;
using System.Globalization;
using UnityEditor;
using UnityEngine.UIElements;

namespace EventHorizon.Editor.RecordingsV2
{
	[CustomEditor(typeof(EvhFileScriptable))]
	public class EvhFileScriptableEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var scriptable = (EvhFileScriptable) target;
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

			var headsetNameField = new TextField
			{
				label = "Headset Name",
				value = string.IsNullOrEmpty(scriptable.vrMetadataPacket.headsetType)
					? "None"
					: scriptable.vrMetadataPacket.headsetType,
				isReadOnly = true
			};
			headsetNameField.AddToClassList("unity-base-field__aligned");
			container.Add(headsetNameField);

			var interactionProfileField = new TextField
			{
				label = "Interaction Profile",
				value = string.IsNullOrEmpty(scriptable.vrMetadataPacket.interactionProfile)
					? "None"
					: scriptable.vrMetadataPacket.headsetType,
				isReadOnly = true
			};
			interactionProfileField.AddToClassList("unity-base-field__aligned");
			container.Add(interactionProfileField);

			#endregion

			return container;
		}
	}
}