using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EventHorizon.Editor
{
	[CustomEditor(typeof(RecordingDataScriptable))]
	public class RecordingDataScriptableEditor : UnityEditor.Editor
	{
		public VisualElement CreateHeader(string headerText)
		{
			var decoratorContainer = new VisualElement();
			decoratorContainer.AddToClassList("unity-decorator-drawers-container");

			var metadataLabel = new Label(headerText);
			metadataLabel.AddToClassList("unity-header-drawer__label");
			decoratorContainer.Add(metadataLabel);

			return decoratorContainer;
		}

		public override VisualElement CreateInspectorGUI()
		{
			RecordingDataScriptable scriptable = (RecordingDataScriptable) target;
			var container = new VisualElement();

			var playButton = new Button();
			playButton.text = "Start Playback";
			playButton.clicked += () => { EventHorizonInspectionStateToggler.TogglePlayModeAndSetupCleanup(); };
			container.Add(playButton);

			var metadataHeader = CreateHeader("Metadata");
			container.Add(metadataHeader);

			var sceneNameField = new TextField
			{
				label = "Scene Name",
				isReadOnly = true,
				value = scriptable.data.metadata.sceneName
			};
			sceneNameField.AddToClassList("unity-base-field__aligned");
			container.Add(sceneNameField);

			var fpsProperty = serializedObject.FindProperty("data.metadata.fps");
			var fpsField = new TextField
			{
				label = "Frame Rate",
				isReadOnly = true,
				value = scriptable.data.metadata.fps.ToString()
			};
			fpsField.AddToClassList("unity-base-field__aligned");
			container.Add(fpsField);

			var framesLengthField = new IntegerField
			{
				label = "Number of Frames",
				value = scriptable.data.frames.Length,
				isReadOnly = true
			};
			framesLengthField.AddToClassList("unity-base-field__aligned");
			container.Add(framesLengthField);

			var rawDataHeader = CreateHeader("Raw Data");
			container.Add(rawDataHeader);

			var frameField = new PropertyField(serializedObject.FindProperty("data.frames"));
			container.Add(frameField);

			return container;
		}
	}
}