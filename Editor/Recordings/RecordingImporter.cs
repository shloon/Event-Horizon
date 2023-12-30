using UnityEditor.AssetImporters;
using UnityEngine;

namespace EventHorizon.Editor
{
	public interface IAssetImportContext
	{
		void AddObjectToAsset(string identifier, Object objectToAdd);
		void SetMainObject(Object mainObject);
	}

	internal class AssetImportContextWrapper : IAssetImportContext
	{
		readonly AssetImportContext ctx;
		public AssetImportContextWrapper(AssetImportContext context) => ctx = context;
		public void AddObjectToAsset(string identifier, Object objectToAdd) => ctx.AddObjectToAsset(identifier, objectToAdd);
		public void SetMainObject(Object mainObject) => ctx.SetMainObject(mainObject);
	}

	[ScriptedImporter(1, "evh")]
	public class RecordingImporter : ScriptedImporter
	{
		public void ProcessImport(IAssetImportContext ctx, string assetPath)
		{
			var data = RecordingDataUtilities.Load(assetPath);
			var dataScriptable = ScriptableObject.CreateInstance<RecordingDataScriptable>();
			dataScriptable.data = data;
			ctx.AddObjectToAsset("data", dataScriptable);
			ctx.SetMainObject(dataScriptable);
		}

		public override void OnImportAsset(AssetImportContext ctx)
		{
			var wrapperContext = new AssetImportContextWrapper(ctx);
			ProcessImport(wrapperContext, ctx.assetPath);
		}
	}
}