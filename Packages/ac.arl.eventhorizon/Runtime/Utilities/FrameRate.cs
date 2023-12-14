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

		public readonly double GetAsDouble() => (numerator / (double) denominator);
		public readonly double GetFrameDuration() => (denominator / (double) numerator);

		public bool Equals(FrameRate other) => numerator == other.numerator && denominator == other.denominator;
		public override bool Equals(object obj) => obj is FrameRate other && Equals(other);
		public override int GetHashCode() => HashCode.Combine(numerator, denominator);
		public override string ToString() => $"{numerator}/{denominator} FPS";

		public static bool operator ==(FrameRate left, FrameRate right) => left.Equals(right);
		public static bool operator !=(FrameRate left, FrameRate right) => !left.Equals(right);

		public FrameRate Simplify()
		{
			var (a, b) = (numerator, denominator);
			while (b > 0)
				(a, b) = (b, a % b);
			var gcd = a;

			return new FrameRate { denominator = denominator / gcd, numerator = numerator / gcd };
		}

		public static bool TryParse(string input, out FrameRate output)
		{
			input = input?.Trim();
			if (string.IsNullOrEmpty(input))
			{
				output.numerator = -1;
				output.denominator = -1;
				return false;
			}

			var indexOfSlash = input.IndexOf('/');

			if (indexOfSlash != -1)
			{
				// if a fraction, try parsing it as a fraction
				var parts = input.Split('/', 2);
				var numStr = parts[0];
				var denStr = parts[1];
				if (uint.TryParse(denStr, out var den) && uint.TryParse(numStr, out var num) && num > 0 && den > 0)
				{
					output.numerator = (int) num;
					output.denominator = (int) den;
					output = output.Simplify();

					return true;
				}
			}
			else
			{
				// if not a fraction, try parsing as an int or approximate a double
				if (int.TryParse(input, out var intValue) && intValue > 0)
				{
					output.numerator = intValue;
					output.denominator = 1;
					return true;
				}

				if (decimal.TryParse(input, out var decimalValue) && decimalValue > 0)
				{
					var parts = input.Split('.');
					var integerPart = parts[0];
					var fractionalPart = parts[1];

					output.numerator = int.Parse(integerPart + fractionalPart);
					output.denominator = (int) Math.Pow(10, fractionalPart.Length);
					output = output.Simplify();

					return true;
				}
			}

			output.numerator = -1;
			output.denominator = -1;
			return false;
		}
	}
}