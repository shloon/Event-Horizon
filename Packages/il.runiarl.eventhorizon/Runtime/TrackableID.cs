using System;
using UnityEngine;

namespace EventHorizon
{
	[Serializable]
	public struct TrackableID : IEquatable<TrackableID>, IComparable<TrackableID>
	{
		public static readonly TrackableID Unassigned = new();

		// unity serialization requires this to be a non-readonly field
#pragma warning disable IDE0044
		[SerializeField] private uint @internal;
#pragma warning restore IDE0044

		public TrackableID(uint id = 0) => @internal = id;

		public uint Internal => @internal;
		public bool IsValid => @internal != 0;

		public int CompareTo(TrackableID other) => @internal.CompareTo(other.@internal);
		public bool Equals(TrackableID other) => @internal == other.@internal;

		public override string ToString() => $"TrackableId({@internal.ToString()})";
		public override bool Equals(object obj) => obj is TrackableID other && Equals(other);
		public override int GetHashCode() => (int) @internal;
		public static bool operator ==(TrackableID one, TrackableID other) => one.@internal == other.@internal;
		public static bool operator !=(TrackableID one, TrackableID other) => !(one == other);
	}
}