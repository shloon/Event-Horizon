using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
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
		private readonly StreamWriter streamWriter;

		public RecordingWriter(Stream outputStream, RecordingMetadata metadata)
		{
			tasks = new();

			this.outputStream = new BrotliStream(outputStream, System.IO.Compression.CompressionLevel.Optimal, leaveOpen: true);
			this.metadata = metadata;

			streamWriter = new StreamWriter(this.outputStream);
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
		}

		public void Flush() => outputStream.Flush();

		public void Close()
		{
			Flush();
			Dispose();
		}

		public void Dispose() => streamWriter?.Dispose();
		~RecordingWriter() => Close();
	}
}