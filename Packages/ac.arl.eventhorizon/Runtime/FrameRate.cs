using System;
using UnityEngine;

namespace EventHorizon
{
	[Serializable]
	public struct FrameRate : IEquatable<FrameRate>
	{
		[field: SerializeField] public int numerator;
		[field: SerializeField] public int denominator;

		public FrameRate(int numerator, int denominator = 1)
		{
			if (numerator <= 0 || denominator <= 0)
				throw new ArgumentException("Numerator and Denominator must be positive integers");

			this.numerator = numerator;
			this.denominator = denominator;
		}

		public readonly double GetAsDouble() => (numerator / (double)denominator);
		public readonly double GetFrameDuration() => (denominator / (double) numerator);

		public bool Equals(FrameRate other) => numerator == other.numerator && denominator == other.denominator;
		public override bool Equals(object obj) => obj is FrameRate other && Equals(other);
		public override int GetHashCode() => HashCode.Combine(numerator, denominator);
		public override string ToString() => $"{numerator}/{denominator} FPS";

		public static bool operator ==(FrameRate left, FrameRate right) => left.Equals(right);
		public static bool operator !=(FrameRate left, FrameRate right) => !left.Equals(right);
	}
}