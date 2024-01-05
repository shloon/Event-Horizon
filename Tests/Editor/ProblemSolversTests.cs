using EventHorizon.Editor.ProblemSolver;
using EventHorizon.Tests.Utilities;
using Moq;
using NUnit.Framework;

// Additional using statements for your project

namespace EventHorizon.Editor.Tests
{
	[Parallelizable]
	public class InvalidTrackableIDProblemTests
	{
		private Mock<ITrackableManager> mockTrackableManager;
		private TestTrackable trackable;

		[SetUp]
		public void Setup()
		{
			trackable = new TestTrackable(name: "TestObject");
			mockTrackableManager = new Mock<ITrackableManager>();
		}

		[Test]
		public void InvalidTrackableIDProblem_Description_ShouldBeCorrect()
		{
			var problem = new InvalidTrackableIDProblem
			{
				trackable = trackable,
				trackableManager = mockTrackableManager.Object
			};

			var description = problem.Description;

			Assert.AreEqual("TestObject", trackable.Name);
			Assert.AreEqual("GameObject \"TestObject\" has no valid ID assigned to it", description);
		}

		[Test]
		public void InvalidTrackableIDProblem_Fix_ShouldGenerateNewId()
		{
			var newId = new TrackableID(3);
			mockTrackableManager.Setup(m => m.GenerateId()).Returns(newId);
			mockTrackableManager.Setup(m => m.Register(trackable)).Verifiable();
			var problem = new InvalidTrackableIDProblem
			{
				trackable = trackable,
				trackableManager = mockTrackableManager.Object
			};

			problem.Fix();

			mockTrackableManager.Verify(m => m.GenerateId(), Times.Once);
			mockTrackableManager.Verify(m => m.Register(trackable), Times.Once);
			Assert.AreEqual(newId, trackable.Id);
		}
	}

	[Parallelizable]
	public class TwoTrackablesWithSameIDProblemTests
	{
		private TestTrackable mockOtherTrackableComponent;
		private TestTrackable mockTrackableComponent;
		private Mock<ITrackableManager> mockTrackableManager;

		[SetUp]
		public void Setup()
		{
			mockTrackableComponent = new TestTrackable(new TrackableID(1), "TestObject");
			mockOtherTrackableComponent = new TestTrackable(new TrackableID(1), "OtherObject");
			mockTrackableManager = new Mock<ITrackableManager>();
		}

		[Test]
		public void TrackableIDInUseProblem_Description_ShouldBeCorrect()
		{
			// Arrange
			var problem = new TwoTrackablesWithSameIDProblem
			{
				trackable = mockTrackableComponent,
				otherTrackable = mockOtherTrackableComponent,
				trackableManager = mockTrackableManager.Object
			};

			// Act
			var description = problem.Description;

			// Assert
			Assert.AreEqual("GameObject \"TestObject\"'s ID is already assigned to \"OtherObject\"", description);
		}

		[Test]
		public void TrackableIDInUseProblem_Fix_ShouldGenerateNewId()
		{
			// Arrange
			var newId = new TrackableID(3); // Assuming TrackableId is a struct or class
			mockTrackableManager.Setup(m => m.GenerateId()).Returns(newId);
			mockTrackableManager.Setup(m => m.Register(mockTrackableComponent)).Verifiable();
			var problem = new TwoTrackablesWithSameIDProblem
			{
				trackable = mockTrackableComponent,
				otherTrackable = mockOtherTrackableComponent,
				trackableManager = mockTrackableManager.Object
			};

			// Act
			problem.Fix();

			// Assert
			mockTrackableManager.Verify(m => m.GenerateId(), Times.Once);
			mockTrackableManager.Verify(m => m.Register(mockTrackableComponent), Times.Once);
			Assert.AreEqual(newId, mockTrackableComponent.Id);
		}
	}
}