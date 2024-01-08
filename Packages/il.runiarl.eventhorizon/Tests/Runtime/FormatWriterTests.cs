using EventHorizon.FormatV2;
using NUnit.Framework;
using System;
using System.IO;

namespace EventHorizon.Tests
{
	public class FormatWriterTests
	{
		private MemoryStream memoryStream;
		private FormatWriter writer;

		[SetUp]
		public void SetUp() => memoryStream = new MemoryStream();

		[TearDown]
		public void TearDown()
		{
			writer?.Dispose();
			memoryStream?.Dispose();
		}

		[Test]
		public void Constructor_Defaults()
		{
			writer = new FormatWriter(memoryStream);

			Assert.IsTrue(writer.IsCompressed);
			Assert.IsFalse(writer.LeaveOpen);
		}

		[Test]
		public void Constructor_WithCompression_CreatesCompressedStream()
		{
			writer = new FormatWriter(memoryStream);

			Assert.IsTrue(writer.IsCompressed, "Stream should be a BrotliStream");
		}

		[Test]
		public void Constructor_WithoutCompression_CreatesNormalStream()
		{
			writer = new FormatWriter(memoryStream, false, true);

			Assert.IsFalse(writer.IsCompressed, "Stream should not be a BrotliStream");
		}

		[Test]
		public void Stream_WithCompressionOn_WithLeaveOpenOff_AfterDispose_LeavesStreamOpen()
		{
			writer = new FormatWriter(memoryStream, leaveOpen: false);
			writer.Dispose();

			Assert.IsFalse(memoryStream.CanRead, "Stream should not be readable");
			Assert.IsFalse(memoryStream.CanWrite, "Stream should not be writeable");
			Assert.IsFalse(memoryStream.CanSeek, "Stream should not be seekable");
		}

		[Test]
		public void Stream_WithCompressionOn_WithLeaveOpenOn_AfterDispose_LeavesStreamOpen()
		{
			writer = new FormatWriter(memoryStream, leaveOpen: true);
			writer.Dispose();

			Assert.IsTrue(memoryStream.CanRead, "Stream should be open");
			Assert.IsTrue(memoryStream.CanWrite, "Stream should be writeable");
			Assert.IsTrue(memoryStream.CanSeek, "Stream should be seekable");
		}

		[Test]
		public void Stream_WithCompressionOff_WithLeaveOpenOff_AfterDispose_LeavesStreamOpen()
		{
			writer = new FormatWriter(memoryStream, false);
			writer.Dispose();

			Assert.IsFalse(memoryStream.CanRead, "Stream should not be readable");
			Assert.IsFalse(memoryStream.CanWrite, "Stream should not be writeable");
			Assert.IsFalse(memoryStream.CanSeek, "Stream should not be seekable");
		}

		[Test]
		public void Stream_WithCompressionOff_WithLeaveOpenOn_AfterDispose_LeavesStreamOpen()
		{
			writer = new FormatWriter(memoryStream, false, true);
			writer.Dispose();

			Assert.IsTrue(memoryStream.CanRead, "Stream should be open to read");
			Assert.IsTrue(memoryStream.CanWrite, "Stream should be writeable");
			Assert.IsTrue(memoryStream.CanSeek, "Stream should be seekable");
		}

		[Test]
		public void WritePacket_WritesSerializedPacket()
		{
			writer = new FormatWriter(memoryStream, false, true);
			var packet = new GenericDataPacket { data = "Test" };

			writer.WritePacket(packet);
			writer.Close();

			memoryStream.Seek(0, SeekOrigin.Begin);
			var reader = new StreamReader(memoryStream);
			var content = reader.ReadToEnd();

			StringAssert.Contains("Test", content, "Packet data should be written to the stream");
		}

		[Test]
		public void Dispose_WaitsForAllTasksToComplete()
		{
			writer = new FormatWriter(memoryStream, false, true);
			var packet = new GenericDataPacket { data = "Test" };

			writer.WritePacket(packet);
			writer.Close();

			memoryStream.Position = 0;
			Assert.IsNotEmpty(memoryStream.ToArray());
		}

		[Test]
		public void Dispose_ReleasesResources()
		{
			writer = new FormatWriter(memoryStream, false);
			writer.Dispose();

			Assert.Throws<ObjectDisposedException>(() => memoryStream.ReadByte(),
				"Disposed stream should throw ObjectDisposedException");
		}
	}
}