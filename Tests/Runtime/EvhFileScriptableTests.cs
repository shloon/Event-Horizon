using EventHorizon.FileFormat;
using NUnit.Framework;
using UnityEngine;

namespace EventHorizon.Tests
{
	public class EvhFileScriptableTests
	{
		[Test]
		public void EvhFileScriptable_EnsureDefaults()
		{
			var scriptable = ScriptableObject.CreateInstance<EvhFileScriptable>();
			
			// assert
			Assert.AreEqual(default(MetadataPacket), scriptable.metadataPacket);
			Assert.AreEqual(default(VRMetadataPacket), scriptable.vrMetadataPacket);
			
			Assert.IsNotNull(scriptable.framePackets);
			Assert.IsNotNull(scriptable.genericDataPackets);
			Assert.IsNotNull(scriptable.transformPackets);
			Assert.IsNotNull(scriptable.activationPackets);
			
			Assert.IsEmpty(scriptable.framePackets);
			Assert.IsEmpty(scriptable.genericDataPackets);
			Assert.IsEmpty(scriptable.transformPackets);
			Assert.IsEmpty(scriptable.activationPackets);
			
			// cleanup
			Object.Destroy(scriptable);
		}
	}
}