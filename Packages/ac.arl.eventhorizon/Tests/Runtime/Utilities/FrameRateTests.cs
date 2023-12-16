using EventHorizon;
using NUnit.Framework;
using System;
using UnityEngine; // Assuming your FrameRate struct is in this namespace

namespace EventHorizon.Tests
{
	[Parallelizable]
	public class FrameRateTests
	{
		[Test]
		public void Constructor_WithValidInput_WithDefaultValues_ShouldSetProperties()
		{
			var frameRate = new FrameRate(30);
			Assert.AreEqual(30, frameRate.numerator);
			Assert.AreEqual(1, frameRate.denominator);
		}

		[Test]
		public void Constructor_WithValidInput_ShouldSetProperties()
		{
			var frameRate = new FrameRate(30, 1);
			Assert.AreEqual(30, frameRate.numerator);
			Assert.AreEqual(1, frameRate.denominator);
		}

		[Test]
		public void Constructor_WithNegativeNumerator_ShouldThrowArgumentException()
		{
			Assert.Throws<ArgumentException>(() => _ = new FrameRate(-30, 1));
		}

		[Test]
		public void Constructor_WithNegativeDenominator_ShouldThrowArgumentException()
		{
			Assert.Throws<ArgumentException>(() => _ = new FrameRate(30, -1));
		}

		[Test]
		public void Constructor_WithZeroNumerator_ShouldThrowArgumentException()
		{
			Assert.Throws<ArgumentException>(() => _ = new FrameRate(0, 1));
		}

		[Test]
		public void Constructor_WithZeroDenominator_ShouldThrowArgumentException()
		{
			Assert.Throws<ArgumentException>(() => _ = new FrameRate(30, 0));
		}

		[TestCase(24, 1, 24.0)]
		[TestCase(24000, 1001, 24000.0 / 1001.0)]
		[TestCase(25, 1, 25.0)]
		[TestCase(30000, 1001, 30000.0 / 1001.0)]
		[TestCase(30, 1, 30.0)]
		[TestCase(50, 1, 50.0)]
		[TestCase(60000, 1001, 60000.0 / 1001.0)]
		[TestCase(60, 1, 60.0)]
		public void GetAsDouble_ValidFrameRate_ShouldReturnCorrectValue(int numerator, int denominator,
			double expectedValue)
		{
			var frameRate = new FrameRate(numerator, denominator);
			Assert.AreEqual(expectedValue, frameRate.GetAsDouble(), message: $"Failed for frame rate {numerator}/{denominator}");
		}

		[TestCase(24, 1, 1.0 / 24.0)]
		[TestCase(24000, 1001, 1001.0 / 24000.0)]
		[TestCase(25, 1, 1.0 / 25.0)]
		[TestCase(30000, 1001, 1001.0 / 30000.0)]
		[TestCase(30, 1, 1.0 / 30.0)]
		[TestCase(50, 1, 1.0 / 50.0)]
		[TestCase(60000, 1001, 1001.0 / 60000.0)]
		[TestCase(60, 1, 1.0 / 60.0)]
		public void GetFrameDuration_ValidFrameRate_ShouldReturnCorrectDuration(int numerator, int denominator,
			double expectedValue)
		{
			var frameRate = new FrameRate(numerator, denominator);
			Assert.AreEqual(expectedValue, frameRate.GetFrameDuration(), message: $"Failed for frame rate {numerator}/{denominator}");
		}

		[Test]
		public void GetFrameDuration_ValidFrameRate_ShouldReturnCorrectDuration()
		{
			var frameRate = new FrameRate(30, 1);
			Assert.AreEqual(1.0 / 30.0, frameRate.GetFrameDuration());
		}

		[Test]
		public void ToString_ValidFrameRate_ShouldReturnCorrectString()
		{
			var frameRate = new FrameRate(24, 1);
			var frameRate2 = new FrameRate(24000, 1001);
			Assert.AreEqual("24 FPS", frameRate.ToString());
			Assert.AreEqual("24000/1001 FPS", frameRate2.ToString());
		}

		[Test]
		public void Equals_WithEqualFrameRates_ShouldReturnTrue()
		{
			var frameRate1 = new FrameRate(30, 1);
			var frameRate2 = new FrameRate(30, 1);
			Assert.IsTrue(frameRate1.Equals(frameRate2));
		}

		[Test]
		public void Equals_WithDifferentFrameRates_ShouldReturnFalse()
		{
			var frameRate1 = new FrameRate(30, 1);
			var frameRate2 = new FrameRate(24, 1);
			Assert.IsFalse(frameRate1.Equals(frameRate2));
		}

		[Test]
		public void EqualityOperator_WithEqualFrameRates_ShouldReturnTrue()
		{
			var frameRate1 = new FrameRate(30, 1);
			var frameRate2 = new FrameRate(30, 1);
			Assert.IsTrue(frameRate1 == frameRate2);
		}

		[Test]
		public void InequalityOperator_WithDifferentFrameRates_ShouldReturnTrue()
		{
			var frameRate1 = new FrameRate(30, 1);
			var frameRate2 = new FrameRate(24, 1);
			Assert.IsTrue(frameRate1 != frameRate2);
		}

		[Test]
		public void GetHashCode_WithEqualFrameRates_ShouldReturnSameValue()
		{
			var frameRate1 = new FrameRate(30, 1);
			var frameRate2 = new FrameRate(30, 1);
			Assert.AreEqual(frameRate1.GetHashCode(), frameRate2.GetHashCode());
		}

		[Test]
		public void SerializesAndDeserializesCorrectly()
		{
			FrameRate original = new FrameRate(30, 1);
			var json = JsonUtility.ToJson(original);
			var deserialized = JsonUtility.FromJson<FrameRate>(json);
			Debug.Log(json);

			// Assert
			Assert.AreEqual(original, deserialized);
		}

		[TestCase(100, 100, 1, 1)]
		[TestCase(1200, 1000, 6, 5)]
		[TestCase(100, 200, 1, 2)]
		[TestCase(50, 100, 1, 2)]
		[TestCase(75, 300, 1, 4)]
		[TestCase(360, 60, 6, 1)]
		[TestCase(500, 500, 1, 1)]
		[TestCase(144, 12, 12, 1)]
		public void Simplify_CorrectlySimplifiesFraction(int beforeNum, int beforeDen, int expectedNumerator, int expectedDenominator)
		{
			FrameRate original = new FrameRate() { numerator = beforeNum, denominator = beforeDen };
			FrameRate reduced = original.Simplify();
			Assert.AreEqual(expectedNumerator, reduced.numerator);
			Assert.AreEqual(expectedDenominator, reduced.denominator);
		}

		[TestCase("30", 30, 1)]
		[TestCase("29.97", 2997, 100)]
		[TestCase("12345.6789", 123456789, 10000)]
		[TestCase("1.0", 1, 1)]
		[TestCase("0.001", 1, 1000)]
		[TestCase("1000000", 1000000, 1)]
		[TestCase("0.0000001", 1, 10000000)]
		[TestCase(" 30 ", 30, 1)]
		[TestCase("30000/1001", 30000, 1001)]
		[TestCase("100/100", 1, 1)]
		[TestCase("1200/1000", 6, 5)]
		[TestCase("100/200", 1, 2)]
		[TestCase("50/100", 1, 2)]
		[TestCase("75/300", 1, 4)]
		[TestCase("360/60", 6, 1)]
		[TestCase("500/500", 1, 1)]
		[TestCase("144/12", 12, 1)]
		[TestCase("30 FPS", 30, 1)]
		[TestCase("29.97 FPS", 2997, 100)]
		[TestCase("30000/1001 FPS", 30000, 1001)]
		public void TryParse_WithProperInput_ReturnsValidFrameRate(string input, int expectedNumerator, int expectedDenominator)
		{
			var result = FrameRate.TryParse(input, out FrameRate frameRate);

			Assert.AreEqual(true, result);
			Assert.AreEqual(expectedNumerator, frameRate.numerator);
			Assert.AreEqual(expectedDenominator, frameRate.denominator);
		}

		[TestCase("0")]
		[TestCase("abc")]
		[TestCase("0/0")]
		[TestCase("-1/-3")]
		[TestCase("-1/3")]
		[TestCase("1/2/3/4")]
		[TestCase("")]
		[TestCase(null)]
		[TestCase("1/0")]
		[TestCase("/")]
		[TestCase("1./2")]
		[TestCase("1.2/3")]
		[TestCase("1/3.4")]
		[TestCase("1/2/3")]
		[TestCase("30fps")]
		[TestCase("one/three")]
		[TestCase("-30")]
		[TestCase("-29.97")]
		[TestCase("0/100")]
		public void TryParse_WithoutProperInput_ReturnsInvalidFrameRate(string input)
		{
			var result = FrameRate.TryParse(input, out FrameRate frameRate);

			Assert.AreEqual(false, result);
			Assert.AreEqual(-1, frameRate.numerator);
			Assert.AreEqual(-1, frameRate.denominator);
		}
	}
}