using EventHorizon.Editor.ProblemSolver;
using EventHorizon.Tests.Utilities;
using Moq;
using NUnit.Framework;

// Additional using statements for your project

namespace EventHorizon.Editor.Tests
{
	public class InvalidTrackableIDProblemTests
	{
		private TrackableComponent mockTrackableComponent;
		private Mock<ITrackableManager> mockTrackableManager;

		[SetUp]
		public void Setup()
		{
			mockTrackableComponent = TrackableTestUtils.CreateTrackableGameObject();
			mockTrackableManager = new Mock<ITrackableManager>();
		}

		[Test]
		public void InvalidTrackableIDProblem_Description_ShouldBeCorrect()
		{
			mockTrackableComponent.gameObject.name = "TestObject";
			var problem = new InvalidTrackableIDProblem
			{
				trackable = mockTrackableComponent, trackableManager = mockTrackableManager.Object
			};

			var description = problem.Description;

			Assert.AreEqual("GameObject \"TestObject\" has no valid ID assigned to it", description);
		}

		[Test]
		public void InvalidTrackableIDProblem_Fix_ShouldGenerateNewId()
		{
			var newId = new TrackableID(3);
			mockTrackableManager.Setup(m => m.GenerateId()).Returns(newId);
			mockTrackableManager.Setup(m => m.Register(mockTrackableComponent)).Verifiable();
			var problem = new InvalidTrackableIDProblem
			{
				trackable = mockTrackableComponent, trackableManager = mockTrackableManager.Object
			};

			problem.Fix();

			mockTrackableManager.Verify(m => m.GenerateId(), Times.Once);
			mockTrackableManager.Verify(m => m.Register(mockTrackableComponent), Times.Once);
			Assert.AreEqual(newId, mockTrackableComponent.Id);
		}
	}

	public class TwoTrackablesWithSameIDProblemTests
	{
		private TrackableComponent mockOtherTrackableComponent;
		private TrackableComponent mockTrackableComponent;
		private Mock<ITrackableManager> mockTrackableManager;

		[SetUp]
		public void Setup()
		{
			mockTrackableComponent = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			mockOtherTrackableComponent = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			mockTrackableManager = new Mock<ITrackableManager>();
		}

		[Test]
		public void TrackableIDInUseProblem_Description_ShouldBeCorrect()
		{
			// Arrange
			mockTrackableComponent.gameObject.name = "TestObject";
			mockOtherTrackableComponent.gameObject.name = "OtherObject";
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