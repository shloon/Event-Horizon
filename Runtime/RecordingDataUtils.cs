using System.Collections.Generic;
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
			if (compressedStream.Length == 0)
				throw new InvalidDataException("Stream cannot be of length 0");

			using var uncompressedStream = new MemoryStream();
			compressedStream.Position = 0;
			BrotliUtils.Decompress(compressedStream, uncompressedStream);

			var json = Encoding.ASCII.GetString(uncompressedStream.ToArray());
			return JsonUtility.FromJson<RecordingData>(json);
		}
	}

	public partial struct RecordingFrameData
	{
		public static RecordingFrameData FromCurrentFrame(in IReadOnlyDictionary<TrackableID, Trackable> trackables, in RecordingMetadata metadata, int frameNumber)
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
	}
}