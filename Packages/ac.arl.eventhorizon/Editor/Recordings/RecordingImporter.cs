using UnityEngine;
using UnityEditor.AssetImporters;

namespace EventHorizon.Editor
{
	[ScriptedImporter(1, "evh")]
	public class RecordingImporter : ScriptedImporter
	{
		public override void OnImportAsset(AssetImportContext ctx)
		{
			var data = RecordingDataUtilities.Load(ctx.assetPath);
			var dataScriptable = ScriptableObject.CreateInstance<RecordingDataScriptable>();
			dataScriptable.data = data;
			ctx.AddObjectToAsset("data", dataScriptable);
			ctx.SetMainObject(dataScriptable);
		}
	}
}