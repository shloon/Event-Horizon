using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace EventHorizon.FileFormat
{
	public class FormatReader : IDisposable
	{
		private readonly Stream inputStream;
		private readonly StreamReader streamReader;

		private bool disposed;

		public FormatReader(Stream inStream, bool compressed = true, bool leaveOpen = false)
		{
			inputStream = compressed
				? new BrotliStream(inStream, CompressionMode.Decompress, leaveOpen)
				: inStream;
			streamReader = new StreamReader(inputStream, Encoding.ASCII, true, 1024, leaveOpen);
			IsCompressed = compressed;
			LeaveOpen = leaveOpen;
		}

		public bool IsCompressed { get; private set; }

		public bool LeaveOpen { get; private set; }

		public bool IsEndOfFile => streamReader.Peek() == -1;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public T ReadPacket<T>() where T : IPacket
		{
			var header = streamReader.ReadLine();
			var packet = streamReader.ReadLine();

			var packetHeader = JsonUtility.FromJson<PacketHeader>(header);
			var packetContents = JsonUtility.FromJson<T>(packet);

			return packetContents;
		}

		public IPacket ReadPacket()
		{
			var header = streamReader.ReadLine();
			var packet = streamReader.ReadLine();

			return PacketUtils.DeserializePacket(header, packet);
		}

		private void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}

			if (disposing)
			{
				lock (streamReader)
				{
					streamReader?.Dispose();
				}

				if (inputStream is BrotliStream)
				{
					inputStream?.Dispose();
				}
			}

			disposed = true;
		}

		public void Close() => Dispose();
		~FormatReader() => Dispose();
	}
}