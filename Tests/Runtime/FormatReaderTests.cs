using EventHorizon.FileFormat;
using NUnit.Framework;
using System.IO;

namespace EventHorizon.Tests
{
	public class FormatReaderTests
	{
		private MemoryStream memoryStream;
		private FormatReader reader;

		[SetUp]
		public void SetUp() => memoryStream = new MemoryStream();

		[TearDown]
		public void TearDown()
		{
			reader?.Dispose();
			memoryStream?.Dispose();
		}
		
		[Test]
		public void FormatReader_EnsureDefaults()
		{
			reader = new FormatReader(memoryStream);
			Assert.IsTrue(reader.IsCompressed);
			Assert.IsFalse(reader.LeaveOpen);
		}
		
		[Test]
		public void Reader_DoubleDispose_HandleGracefully()
		{
			reader = new FormatReader(memoryStream);
			Assert.DoesNotThrow(()=>reader.Dispose());
			Assert.DoesNotThrow(()=>reader.Dispose());
		}
				
		[Test]
		public void Reader_Close_Disposes()
		{
			reader = new FormatReader(memoryStream);
			Assert.DoesNotThrow(()=>reader.Close());
		}
		
		[Test]
		public void Reader_DisposesOnDeconstruct()
		{
			{
				_ = new FormatReader(memoryStream);
			}
			
			reader = new FormatReader(memoryStream);
		}

		[Test]
		public void FileReader_ReadPacketsInOrder_Dynamic()
		{
			var writer = new FormatWriter(memoryStream, leaveOpen: true);
			for (var i = 0; i < 100; ++i)
			{
				writer.WritePacket(new GenericDataPacket { data = i.ToString() });
			}

			writer.Close();
			memoryStream.Position = 0;

			reader = new FormatReader(memoryStream);
			for (var i = 0; i < 100; ++i)
			{
				var packet = (GenericDataPacket) reader.ReadPacket();
				Assert.AreEqual(i, int.Parse(packet.data));
			}
			Assert.IsTrue(reader.IsEndOfFile);
		}
		
		[Test]
		public void FileReader_ReadPacketsInOrder_Typed()
		{
			var writer = new FormatWriter(memoryStream, leaveOpen: true);
			for (var i = 0; i < 100; ++i)
			{
				writer.WritePacket(new GenericDataPacket { data = i.ToString() });
			}

			writer.Close();
			memoryStream.Position = 0;

			reader = new FormatReader(memoryStream);
			for (var i = 0; i < 100; ++i)
			{
				var packet = reader.ReadPacket<GenericDataPacket>();
				Assert.AreEqual(i, int.Parse(packet.data));
			}
			Assert.IsTrue(reader.IsEndOfFile);
		}
	}
}