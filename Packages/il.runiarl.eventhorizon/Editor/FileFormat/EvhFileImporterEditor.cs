using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace EventHorizon.Editor.RecordingsV2
{
	[CustomEditor(typeof(EvhFileImporter))]
	public class EvhFileImporterEditor : ScriptedImporterEditor
	{
		[ExcludeFromCodeCoverage] // unity internal function, cannot really test this
		public override void OnInspectorGUI() =>
			// This removes the "Script" field from the recording importer
			ApplyRevertGUI();
	}
}