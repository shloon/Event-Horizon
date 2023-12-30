using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace EventHorizon.Tests
{
	public class RecordingDataUtilsTest
	{
		byte[] GenerateFile()
		{
			using var stream = new MemoryStream();
			RecordingWriter r = new RecordingWriter(stream, new RecordingMetadata { sceneName = "test", fps = new FrameRate(24000, 1001) });
			r.WriteHeader();
			r.WrapStream();
			r.Close();

			return stream.ToArray();
		}

		void AssertsOnData(RecordingData recordingData)
		{
			Assert.AreEqual(RecordingFormatVersion.Current, recordingData.version);
			Assert.AreEqual("test", recordingData.metadata.sceneName);
			Assert.AreEqual(new FrameRate(24000, 1001), recordingData.metadata.fps);
			Assert.IsEmpty(recordingData.frames);
		}

		[Test]
		public void Load_FromFilePath_WithValidData_ShouldReturnCorrectData()
		{
			var path = Path.GetTempFileName();
			File.WriteAllBytes(path, GenerateFile());

			Assert.DoesNotThrow(() => RecordingDataUtilities.Load(path));

			RecordingData recordingData = RecordingDataUtilities.Load(path);
			Assert.IsNotNull(recordingData);
			AssertsOnData(recordingData);

			File.Delete(path);
		}

		[Test]
		public void Load_FromFilePath_WithInvalidPath_ShouldThrowFileNotFoundException()
		{
			var path = "this_does_not_exist.evh";
			Assert.Throws<FileNotFoundException>(() => RecordingDataUtilities.Load(path));

			var path2 = "this/does/not/exist.evh";
			Assert.Throws<DirectoryNotFoundException>(() => RecordingDataUtilities.Load(path2));
		}

		[Test]
		public void Load_FromFilePath_WithCorruptData_ShouldThrowInvalidOperationException()
		{
			var path = Path.GetTempFileName();
			File.WriteAllText(path, "invalid data desu");
			Assert.Throws<InvalidOperationException>(() => RecordingDataUtilities.Load(path));

			File.Delete(path);
		}

		[Test]
		public void Load_FromByteArray_WithValidData_ShouldReturnCorrectData()
		{
			var data = GenerateFile();

			Assert.DoesNotThrow(() => RecordingDataUtilities.Load(data));
			RecordingData recordingData = RecordingDataUtilities.Load(data);

			Assert.IsNotNull(recordingData);
			AssertsOnData(recordingData);
		}

		[Test]
		public void Load_FromByteArray_WithEmptyArray_ShouldReturnNull()
		{
			var data = new byte[] { };
			Assert.Throws<InvalidDataException>(() => RecordingDataUtilities.Load(data));
		}

		[Test]
		public void Load_FromByteArray_WithCorruptData_ShouldThrowInvalidOperationException()
		{
			var data = Encoding.ASCII.GetBytes("invalid data desu");
			Assert.Throws<InvalidOperationException>(() => RecordingDataUtilities.Load(data));
		}

		[Test]
		public void Load_FromStream_WithValidData_ShouldReturnCorrectData()
		{
			using var stream = new MemoryStream();
			stream.Write(GenerateFile());
			stream.Position = 0; // seeking back to 0 required so we could read the written data from the stream

			Assert.DoesNotThrow(() => RecordingDataUtilities.Load(stream));
			stream.Position = 0; // ditto

			RecordingData recordingData = RecordingDataUtilities.Load(stream);
			Assert.IsNotNull(recordingData);
			AssertsOnData(recordingData);
		}

		[Test]
		public void Load_FromStream_WithEmptyStream_ShouldThrowInvalidDataException()
		{
			using var stream = new MemoryStream();
			Assert.Throws<InvalidDataException>(() => RecordingDataUtilities.Load(stream));
		}

		[Test]
		public void Load_FromStream_WithCorruptData_ShouldThrowInvalidOperationException()
		{
			using var stream = new MemoryStream();
			stream.Write(Encoding.ASCII.GetBytes("invalid data desu"));
			stream.Position = 0;

			Assert.Throws<InvalidOperationException>(() => RecordingDataUtilities.Load(stream));
		}
	}
}