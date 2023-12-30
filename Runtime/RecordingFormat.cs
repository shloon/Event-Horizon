using System;
using UnityEngine;

namespace EventHorizon
{
	// Internal representation of the format.
	// Serialized using unity's JsonUtility API
	// which we compress later via Brotli (see `Recording.WrapStream`)
	[Serializable]
	public class RecordingData
	{
		public RecordingFormatVersion version;
		public RecordingMetadata metadata;
		public RecordingFrameData[] frames;
	}

	public enum RecordingFormatVersion { V1, Current = V1 }

	[Serializable]
	public struct RecordingMetadata
	{
		public string sceneName;
		public FrameRate fps;
	}

	[Serializable]
	public partial struct RecordingFrameData
	{
		public int frame;
		public double timeCode;
		public RecordingTrackerData[] trackers;
	}

	[Serializable]
	public struct RecordingTrackerData
	{
		public TrackableID id;
		public TransformData transform;
	}

	[Serializable]
	public struct TransformData
	{
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;
	}
}