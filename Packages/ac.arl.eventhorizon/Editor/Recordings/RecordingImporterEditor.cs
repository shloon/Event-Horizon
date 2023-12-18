using UnityEditor;
using UnityEditor.AssetImporters;

namespace EventHorizon.Editor
{
	[CustomEditor(typeof(RecordingImporter))]
	public class RecordingImporterEditor : ScriptedImporterEditor
	{
		public override void OnInspectorGUI()
		{
			ApplyRevertGUI();
		}
	}
}