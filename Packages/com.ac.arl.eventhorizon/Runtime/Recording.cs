using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EventHorizon
{
	public class Recording : IDisposable
	{
		private readonly string outputPath;
		private readonly RecordingMetadata metadata;
		private readonly List<Task<(int, string)>> tasks;
		private MemoryStream memoryStream;
		private TextWriter streamWriter;

		public Recording(string filename, RecordingMetadata metadata)
		{
			tasks = new();

			this.outputPath = filename;
			this.metadata = metadata;

			memoryStream = new MemoryStream();
			streamWriter = new StreamWriter(memoryStream);
		}

		public void WriteHeader() => Task.Run(WriteHeaderAsync).Wait();

		public async void WriteHeaderAsync()
		{
			var rd = new RecordingData { version = RecordingFormatVersion.V1, metadata = metadata, frames = default };
			await streamWriter.WriteAsync(JsonUtility.ToJson(rd).Replace("\"frames\":[]}", "\"frames\":["));
		}

		public void WriteFrame(RecordingFrameData frameData) =>
			tasks.Add(Task.Factory.StartNew(() => (frameData.frame, JsonUtility.ToJson(frameData))));

		public void WrapStream() => Task.Run(WrapStreamAsync).Wait();

		public async void WrapStreamAsync()
		{
			await Task.WhenAll(tasks);

			var sortedLines = tasks.Select(x => x.Result).OrderBy(x => x.Item1).Select(x => x.Item2);
			var isFirst = true;
			foreach (var line in sortedLines)
			{
				if (!isFirst)
				{
					await streamWriter.WriteAsync(",");
				}

				await streamWriter.WriteAsync(line);
				isFirst = false;
			}

			await streamWriter.WriteAsync("]}");
			await streamWriter.FlushAsync();

			// compress using brotli
			await using var fileStream = File.Create(outputPath);
			await using var brotliStream =
				new BrotliStream(fileStream, System.IO.Compression.CompressionLevel.Optimal, false);
			await brotliStream.WriteAsync(memoryStream.ToArray());
			
			// dispose of stream
			Dispose();
		}

		public RecordingFrameData SerializeFrame(in IReadOnlyDictionary<TrackableID, Trackable> trackables,
			int frameNumber)
		{
			RecordingFrameData frameData = new RecordingFrameData
			{
				frame = frameNumber,
				timeCode = metadata.fps.GetFrameDuration() * frameNumber,
				trackers = new RecordingTrackerData[trackables.Count]
			};

			// safety: 0 <= i <= trackables.Count
			var i = 0;
			foreach (var (trackableID, trackable) in trackables)
			{
				var trackableTransform = trackable.gameObject.transform;
				frameData.trackers[i].id = trackableID;
				frameData.trackers[i].transform.position = trackableTransform.position;
				frameData.trackers[i].transform.rotation = trackableTransform.rotation;
				frameData.trackers[i].transform.scale = trackableTransform.localScale;
				i++;
			}

			return frameData;
		}

		public void Dispose()
		{
			streamWriter?.Dispose();
			streamWriter = null;

			memoryStream?.Dispose();
			memoryStream = null;
		}

		~Recording() => Dispose();
	}

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


		private static RecordingData Load(Stream compressedStream) =>
			LoadAsync(compressedStream).ConfigureAwait(false).GetAwaiter().GetResult();

		private static async Task<RecordingData> LoadAsync(Stream compressedStream)
		{
			using var uncompressedStream = new MemoryStream();
			await using var brotliStream = new BrotliStream(compressedStream, CompressionMode.Decompress, false);
			await brotliStream.CopyToAsync(uncompressedStream);

			var json = Encoding.ASCII.GetString(uncompressedStream.ToArray());
			return JsonUtility.FromJson<RecordingData>(json);
		}
	}
}