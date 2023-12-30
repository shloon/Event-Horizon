using System.IO;

namespace EventHorizon.Tests.Utilities
{
	public class RecordingTestUtils
	{
		public static byte[] GenerateEmptyRecordingData()
		{
			using var stream = new MemoryStream();
			var r = new RecordingWriter(stream,
				new RecordingMetadata { sceneName = "test", fps = new FrameRate(24000, 1001) });
			r.WriteHeader();
			r.WrapStream();
			r.Close();

			return stream.ToArray();
		}

		public static string CreateEmptyRecordingFile()
		{
			var tempFilePath = Path.GetTempFileName();
			File.WriteAllBytes(tempFilePath, GenerateEmptyRecordingData());
			return tempFilePath;
		}
	}
}