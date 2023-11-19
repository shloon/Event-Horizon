using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace EventHorizon
{
	[Serializable]
	public struct TrackableID : IEquatable<TrackableID>, IComparable<TrackableID>
	{
		[SerializeField] private uint @internal;

		public TrackableID(uint id = 0) => @internal = id;
		public static TrackableID Unassigned = new TrackableID();

		public uint Internal => @internal;
		public bool IsValid => this.@internal != 0;

		public override string ToString() => $"TrackableId({@internal.ToString()})";
		public bool Equals(TrackableID other) => @internal == other.@internal;
		public override bool Equals(object obj) => obj is TrackableID other && Equals(other);
		public override int GetHashCode() => (int) @internal;

		public int CompareTo(TrackableID other) => @internal.CompareTo(other.@internal);
		public static bool operator ==(TrackableID one, TrackableID other) => one.@internal == other.@internal;
		public static bool operator !=(TrackableID one, TrackableID other) => !(one == other);
	}
}