using EventHorizon.Editor.ProblemSolver;
using EventHorizon.Tests.Utilities;
using Moq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EventHorizon.Editor.Tests
{
	public class SceneWrapperTests
	{
		[Test]
		public void SceneWrapper_GetRootGameObjects_Works_CurrentScene()
		{
			var scene = SceneManager.GetActiveScene();
			var sceneWrapper = new SceneWrapper(SceneManager.GetActiveScene());
			CollectionAssert.AreEqual(scene.GetRootGameObjects(), sceneWrapper.GetRootGameObjects());
		}

		[Test]
		public void SceneWrapper_GetRootGameObjects_Works_EmptyNewScene()
		{
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			var scene = SceneManager.GetActiveScene();
			var sceneWrapper = new SceneWrapper(SceneManager.GetActiveScene());

			CollectionAssert.IsEmpty(scene.GetRootGameObjects());
			CollectionAssert.AreEqual(scene.GetRootGameObjects(), sceneWrapper.GetRootGameObjects());
		}
	}

	public class GenericTrackableIDProblemSolverTests
	{
		private Mock<ISceneWrapper> mockScene;
		private GenericSceneProblemFinder solver;
		private TrackableManager trackableManager;

		[SetUp]
		public void Setup()
		{
			mockScene = new Mock<ISceneWrapper>();
			trackableManager = new TrackableManager();
			solver = new GenericSceneProblemFinder();
		}

		[Test]
		public void ShouldFindNoProblems_WhenNoTrackablesPresent()
		{
			mockScene.Setup(x => x.GetRootGameObjects()).Returns(new GameObject[] { });

			var problems = solver.DiscoverProblemsInScene(mockScene.Object, trackableManager);

			Assert.IsEmpty(problems);
		}

		[Test]
		public void ShouldFindProblem_WhenTrackableIDIsInvalid()
		{
			var invalidTrackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID());
			mockScene.Setup(x => x.GetRootGameObjects()).Returns(new[] { invalidTrackable.gameObject });

			var problems = solver.DiscoverProblemsInScene(mockScene.Object, trackableManager);

			Assert.IsNotEmpty(problems);
			Assert.IsInstanceOf<InvalidTrackableIDProblem>(problems[0]);

			TrackableTestUtils.DestroyTrackableGameObject(invalidTrackable);
		}

		[Test]
		public void ShouldFindProblem_WhenIsInvalid()
		{
			var validTrackable1 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var validTrackable2 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			mockScene.Setup(x => x.GetRootGameObjects())
				.Returns(new[] { validTrackable1.gameObject, validTrackable2.gameObject });

			var problems = solver.DiscoverProblemsInScene(mockScene.Object, trackableManager);

			Assert.IsNotEmpty(problems);
			Assert.IsInstanceOf<TwoTrackablesWithSameIDProblem>(problems[0]);

			TrackableTestUtils.DestroyTrackableGameObject(validTrackable1);
			TrackableTestUtils.DestroyTrackableGameObject(validTrackable2);
		}

		[Test]
		public void ShouldFindProblems_WhenMixed()
		{
			var invalidTrackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID());
			var validTrackable1 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var validTrackable2 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			mockScene.Setup(x => x.GetRootGameObjects())
				.Returns(new[] { invalidTrackable.gameObject, validTrackable1.gameObject, validTrackable2.gameObject });

			var problems = solver.DiscoverProblemsInScene(mockScene.Object, trackableManager);

			Assert.IsNotEmpty(problems);
			Assert.IsInstanceOf<InvalidTrackableIDProblem>(problems[0]);
			Assert.IsInstanceOf<TwoTrackablesWithSameIDProblem>(problems[1]);

			TrackableTestUtils.DestroyTrackableGameObject(invalidTrackable);
			TrackableTestUtils.DestroyTrackableGameObject(validTrackable1);
			TrackableTestUtils.DestroyTrackableGameObject(validTrackable2);
		}

		[Test]
		public void ShouldRegisterTrackable_WhenIDIsValid()
		{
			var validTrackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(3));
			mockScene.Setup(x => x.GetRootGameObjects()).Returns(new[] { validTrackable.gameObject });

			solver.DiscoverProblemsInScene(mockScene.Object, trackableManager);

			Assert.IsTrue(trackableManager.RegisteredTrackables.ContainsKey(validTrackable.Id));
			Assert.AreEqual(validTrackable, trackableManager.RegisteredTrackables[validTrackable.Id]);

			TrackableTestUtils.DestroyTrackableGameObject(validTrackable);
		}
	}
}