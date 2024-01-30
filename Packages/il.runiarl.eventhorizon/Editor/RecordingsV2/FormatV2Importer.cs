using EventHorizon.FileFormat;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace EventHorizon.Editor.RecordingsV2
{
	[ScriptedImporter(1, "evh")]
	public class FormatV2Importer : ScriptedImporter
	{
		public void ProcessImport(IAssetImportContext ctx, string ctxAssetPath)
		{
			var dataScriptable = ScriptableObject.CreateInstance<EvhFileScriptable>();

			{
				// should not throw an exception since this is only called for existant files
				using var fs = File.Open(ctxAssetPath, FileMode.Open);
				using var reader = new FormatReader(fs);

				dataScriptable.metadataPacket = reader.ReadPacket<MetadataPacket>();
				while (!reader.IsEndOfFile)
				{
					switch (reader.ReadPacket())
					{
						case TransformPacket transformPacket:
							dataScriptable.transformPackets.Add(transformPacket);
							break;
						case ActivationPacket activationPacket:
							dataScriptable.activationPackets.Add(activationPacket);
							break;
						case GenericDataPacket genericDataPacket:
							dataScriptable.genericDataPackets.Add(genericDataPacket);
							break;
						case VRMetadataPacket vrMetadataPacket:
							dataScriptable.vrMetadataPacket = vrMetadataPacket;
							break;
						case FramePacket framePacket:
							dataScriptable.framePackets.Add(framePacket);
							break;
					}
				}
			}

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