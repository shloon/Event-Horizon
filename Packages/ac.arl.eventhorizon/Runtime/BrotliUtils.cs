using System;
using System.IO;
using System.IO.Compression;

namespace EventHorizon
{
	public class BrotliUtils
	{
		public static void Compress(Stream source, Stream destination)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (destination == null) throw new ArgumentNullException(nameof(destination));

			source.Position = 0; // move head back
			using BrotliStream compressor = new BrotliStream(destination, CompressionLevel.Optimal, leaveOpen: true);
			source.CopyTo(compressor);
		}

		public static void Decompress(Stream source, Stream destination)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (destination == null) throw new ArgumentNullException(nameof(destination));

			source.Position = 0; // move head back
			using BrotliStream decompressor = new BrotliStream(source, CompressionMode.Decompress, leaveOpen: true);
			decompressor.CopyTo(destination);
		}
	}
}