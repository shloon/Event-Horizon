using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace EventHorizon
{
	public class RecordingWriter : IDisposable
	{
		private readonly RecordingMetadata metadata;
		private readonly Stream outputStream;
		private readonly StreamWriter streamWriter;
		private readonly ConcurrentQueue<Task<string>> tasks;


		private bool disposed; // to track whether Dispose has been called

		private bool wrappedStream;

		public RecordingWriter(Stream outputStream, RecordingMetadata metadata)
		{
			tasks = new ConcurrentQueue<Task<string>>();

			this.outputStream = new BrotliStream(outputStream, CompressionLevel.Optimal, true);
			this.metadata = metadata;

			streamWriter = new StreamWriter(this.outputStream);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this); // Prevent finalizer from running.
		}

		public void WriteHeader()
		{
			var rd = new RecordingData { version = RecordingFormatVersion.V1, metadata = metadata, frames = default };
			streamWriter.Write(JsonUtility.ToJson(rd).Replace("\"frames\":[]}", "\"frames\":["));
		}

		public void WriteFrame(RecordingFrameData frameData) =>
			tasks.Enqueue(Task.Run(() => JsonUtility.ToJson(frameData)));

		public void WrapStream()
		{
			if (wrappedStream)
			{
				return;
			}

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
				{
					streamWriter.Write($",{line}");
				}
			}

			streamWriter.Write("]}");
			streamWriter.Flush();

			wrappedStream = true;
		}


		public void Close()
		{
			WrapStream();
			Dispose();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					// Dispose managed resources.
					streamWriter?.Dispose();
					outputStream?.Dispose();
				}

				// Here you can dispose any unmanaged resources if you have any.

				disposed = true;
			}
		}

		~RecordingWriter() => Dispose(false);
	}
}