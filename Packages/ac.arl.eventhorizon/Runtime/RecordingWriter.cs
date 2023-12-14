using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace EventHorizon
{
	public class RecordingWriter : IDisposable
	{
		private readonly Stream outputStream;
		private readonly RecordingMetadata metadata;
		private readonly ConcurrentQueue<Task<string>> tasks;
		private MemoryStream memoryStream;
		private TextWriter streamWriter;

		public RecordingWriter(Stream outStream, RecordingMetadata metadata)
		{
			tasks = new();

			this.outputStream = outStream;
			this.metadata = metadata;

			memoryStream = new MemoryStream();
			streamWriter = TextWriter.Synchronized(new StreamWriter(memoryStream));
		}

		public void WriteHeader()
		{
			var rd = new RecordingData { version = RecordingFormatVersion.V1, metadata = metadata, frames = default };
			streamWriter.Write(JsonUtility.ToJson(rd).Replace("\"frames\":[]}", "\"frames\":["));
		}

		public void WriteFrame(RecordingFrameData frameData) => tasks.Enqueue(Task.Run(() => JsonUtility.ToJson(frameData)));

		public void WrapStream()
		{
			Task.WaitAll(tasks.ToArray<Task>());

			var sortedLines = tasks
				.Select(x => x.Result)
				.ToList();

			if (sortedLines.Any())
			{
				// First line doesn't need a comma
				streamWriter.Write(sortedLines.First());

				// Write remaining lines, each prefixed with a comma
				foreach (var line in sortedLines.Skip(1))
					streamWriter.Write($",{line}");
			}

			streamWriter.Write("]}");
			streamWriter.Flush();

			// compress using brotli
			BrotliUtils.Compress(memoryStream, outputStream);
			outputStream.Flush();

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

		~RecordingWriter() => Dispose();
	}
}