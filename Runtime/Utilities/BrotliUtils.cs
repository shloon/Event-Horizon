using System;
using System.IO;
using System.IO.Compression;

namespace EventHorizon
{
	public class BrotliUtils
	{
		public static void Compress(Stream source, Stream destination)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			if (destination == null)
			{
				throw new ArgumentNullException(nameof(destination));
			}

			source.Position = 0; // move head back
			using var compressor = new BrotliStream(destination, CompressionLevel.Optimal, true);
			source.CopyTo(compressor);
		}

		public static void Decompress(Stream source, Stream destination)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			if (destination == null)
			{
				throw new ArgumentNullException(nameof(destination));
			}

			source.Position = 0; // move head back
			using var decompressor = new BrotliStream(source, CompressionMode.Decompress, true);
			decompressor.CopyTo(destination);
		}
	}
}