using System;

namespace EventHorizon
{
	/// <summary>
	/// Reference wrapper for TrackableID
	/// </summary>
	[Serializable]
	public class TrackableIDWrapper
	{
		public TrackableID value;
		public TrackableIDWrapper(TrackableID id = new TrackableID()) => this.value = id;
	}
}