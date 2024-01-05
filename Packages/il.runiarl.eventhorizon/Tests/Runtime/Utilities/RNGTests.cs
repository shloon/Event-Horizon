using NUnit.Framework;

namespace EventHorizon.Tests
{
	[Parallelizable]
	public class PcgRngTests
	{
		[Test]
		public void Constructor_DoesNotThrow() => Assert.DoesNotThrow(() => _ = new PcgRng());

		[Test]
		public void Constructor_WithDefaultSeed_ShouldInitialize()
		{
			var rng = new PcgRng();
			Assert.DoesNotThrow(() => rng.Next());
		}

		[Test]
		public void Constructor_WithSpecificSeed_ShouldInitialize()
		{
			const int seed = 12345;
			var rng = new PcgRng(seed);
			Assert.DoesNotThrow(() => rng.Next());
		}

		[Test]
		public void Next_WhenCalledMultipleTimes_ShouldReturnDifferentNumbers()
		{
			var rng = new PcgRng();
			var firstResult = rng.Next();
			var secondResult = rng.Next();
			var thirdResult = rng.Next();

			CollectionAssert.AllItemsAreUnique(new[] { firstResult, secondResult, thirdResult });
		}

		[Test]
		public void Next_WithSameSeed_ShouldReturnSameSequence()
		{
			const int seed = 67890;
			var rng1 = new PcgRng(seed);
			var rng2 = new PcgRng(seed);

			// two calls to next
			Assert.AreEqual(rng1.Next(), rng2.Next());
			Assert.AreEqual(rng1.Next(), rng2.Next());
		}
	}
}