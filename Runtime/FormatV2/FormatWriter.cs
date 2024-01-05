using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace EventHorizon.FormatV2
{
	public class FormatWriter : IDisposable
	{
		private readonly Stream outputStream;

		private readonly Task processingTask;
		private readonly StreamWriter streamWriter;
		private readonly ConcurrentQueue<Task<SerializedPacketData>> tasks;

		private bool disposed;
		private bool keepReading = true;

		public FormatWriter(Stream outputStream, bool compress = true, bool leaveOpen = false)
		{
			IsCompressed = compress;
			LeaveOpen = leaveOpen;

			this.outputStream =
				compress ? new BrotliStream(outputStream, CompressionLevel.Optimal, leaveOpen) : outputStream;
			streamWriter = new StreamWriter(this.outputStream, Encoding.ASCII, 1024, leaveOpen);
			tasks = new ConcurrentQueue<Task<SerializedPacketData>>();
			processingTask = Task.Run(ProcessTasksAsync);
		}

		public bool IsCompressed { get; private set; }
		public bool LeaveOpen { get; private set; }

		#region Task Management

		public void WritePacket<T>(T packet) where T : IPacket =>
			tasks.Enqueue(Task.Run(() => PacketUtils.SerializePacket(packet)));

		private async Task ProcessTasksAsync()
		{
			while (keepReading || !tasks.IsEmpty) // Use a flag to signal when to stop processing
			{
				while (tasks.TryDequeue(out var task))
				{
					await task;
					await streamWriter.WriteLineAsync(task.Result.header);
					await streamWriter.WriteLineAsync(task.Result.contents);
				}

				await streamWriter.FlushAsync();
				await Task.Delay(10); // Prevents tight loop, adjust delay as needed.
			}
		}

		#endregion


		#region Dispose & Cleanup

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		private void Dispose(bool disposing)
		{
			lock (processingTask)
			{
				keepReading = false;
			}

			processingTask.Wait();

			if (disposed)
			{
				return;
			}

			if (disposing)
			{
				lock (streamWriter)
				{
					streamWriter?.Dispose();
				}

				if (outputStream is BrotliStream)
				{
					outputStream?.Dispose();
				}
			}

			disposed = true;
		}

		public void Close() => Dispose();

		~FormatWriter() => Dispose(false);

		#endregion
	}
}