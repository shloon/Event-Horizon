using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace EventHorizon.Editor
{
	[CustomEditor(typeof(RecordingImporter))]
	public class RecordingImporterEditor : ScriptedImporterEditor
	{
		[ExcludeFromCodeCoverage] // unity internal function, cannot really test this
		public override void OnInspectorGUI() =>
			// This removes the "Script" field from the recording importer
			ApplyRevertGUI();
	}
}