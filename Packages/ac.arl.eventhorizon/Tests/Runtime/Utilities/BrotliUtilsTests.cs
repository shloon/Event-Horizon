using NUnit.Framework;
using System;
using System.IO;

namespace EventHorizon.Tests
{
	[Parallelizable]
	public class BrotliUtilsTests
	{
		private MemoryStream _inputStream;
		private MemoryStream _outputStream;

		[SetUp]
		public void Setup()
		{
			_inputStream = new MemoryStream();
			_outputStream = new MemoryStream();
		}

		[TearDown]
		public void Teardown()
		{
			_inputStream.Dispose();
			_outputStream.Dispose();
		}

		private void WriteTestDataToStream(Stream stream, string testData)
		{
			var writer = new StreamWriter(stream);
			writer.Write(testData);
			writer.Flush();
			stream.Position = 0;
		}

		private string ReadDataFromStream(Stream stream)
		{
			stream.Position = 0;
			var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}
		
		[Test]
		public void Compress_ShouldCorrectlyCompressData()
		{
			var testData = "This is a test string for compression";
			WriteTestDataToStream(_inputStream, testData);

			BrotliUtils.Compress(_inputStream, _outputStream);

			Assert.AreNotEqual(_inputStream.Length, _outputStream.Length);
			Assert.Greater(_inputStream.Length, _outputStream.Length);
		}

		
		[Test]
		public void Decompress_ShouldCorrectlyDecompressData()
		{
			var testData = "This is a test string for decompression";
			WriteTestDataToStream(_inputStream, testData);

			// First compress
			BrotliUtils.Compress(_inputStream, _outputStream);

			var decompressedStream = new MemoryStream();
			_outputStream.Position = 0;

			// Then decompress
			BrotliUtils.Decompress(_outputStream, decompressedStream);

			var decompressedData = ReadDataFromStream(decompressedStream);
			Assert.AreEqual(testData, decompressedData);
		}

		
		[Test]
		public void CompressAndDecompress_ShouldReturnOriginalData()
		{
			var testData = "Round trip test data";
			WriteTestDataToStream(_inputStream, testData);

			// Compress
			BrotliUtils.Compress(_inputStream, _outputStream);

			var decompressedStream = new MemoryStream();
			_outputStream.Position = 0;

			// Decompress
			BrotliUtils.Decompress(_outputStream, decompressedStream);

			var result = ReadDataFromStream(decompressedStream);
			Assert.AreEqual(testData, result);
		}

		[Test]
		public void Compress_WithNullStreams_ShouldThrowArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => BrotliUtils.Compress(null, _outputStream));
			Assert.Throws<ArgumentNullException>(() => BrotliUtils.Compress(_inputStream, null));
			Assert.Throws<ArgumentNullException>(() => BrotliUtils.Compress(null, null));
		}

		[Test]
		public void Decompress_WithNullStreams_ShouldThrowArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => BrotliUtils.Decompress(null, _outputStream));
			Assert.Throws<ArgumentNullException>(() => BrotliUtils.Decompress(_inputStream, null));
			Assert.Throws<ArgumentNullException>(() => BrotliUtils.Decompress(null, null));
		}
	}
}