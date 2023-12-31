using NUnit.Framework;

namespace EventHorizon.Tests
{
	public class TrackableIDWrapperTests
	{
		[Test]
		public void TrackableIDWrapper_Value_ShouldBeSetCorrectly()
		{
			var id = new TrackableID(1234);
			var wrapper = new TrackableIDWrapper(id);

			Assert.AreEqual(id, wrapper.value);
		}

		[Test]
		public void TrackableIDWrapper_DefaultConstructor_ShouldSetDefaultValue()
		{
			var defaultId = new TrackableID();
			var wrapper = new TrackableIDWrapper();

			Assert.AreEqual(defaultId, wrapper.value);
		}
	}
}