using System.IO;
using System.Text;
using UnityEngine;

namespace EventHorizon
{
	public static class RecordingDataUtilities
	{
		public static RecordingData Load(string filePath)
		{
			using var stream = File.OpenRead(filePath);
			return Load(stream);
		}

		public static RecordingData Load(byte[] data)
		{
			using MemoryStream stream = new MemoryStream(data);
			return Load(stream);
		}

		public static RecordingData Load(Stream compressedStream)
		{
			using var uncompressedStream = new MemoryStream();
			compressedStream.Position = 0;
			BrotliUtils.Decompress(compressedStream, uncompressedStream);

			var json = Encoding.ASCII.GetString(uncompressedStream.ToArray());
			return JsonUtility.FromJson<RecordingData>(json);
		}
	}
}