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
		private readonly AssetImportContext ctx;
		public AssetImportContextWrapper(AssetImportContext context) => ctx = context;

		public void AddObjectToAsset(string identifier, Object objectToAdd) =>
			ctx.AddObjectToAsset(identifier, objectToAdd);

		public void SetMainObject(Object mainObject) => ctx.SetMainObject(mainObject);
	}
}