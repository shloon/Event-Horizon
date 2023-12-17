using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;

namespace EventHorizon.Tests
{
	public class TransformControlTests
	{
		private PlayableGraph graph;
		private GameObject gameObject;

		[SetUp]
		public void Setup()
		{
			graph = PlayableGraph.Create();
			gameObject = new GameObject();
		}

		[TearDown]
		public void Teardown()
		{
			graph.Destroy();
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void CreatePlayable_ShouldCreatePlayableOfTypeTransformControl()
		{
			var transformControlAsset = new TransformControlAsset();
			var playable = transformControlAsset.CreatePlayable(graph, gameObject);

			// Checking the type of Playable
			Assert.IsTrue(playable.GetPlayableType() == typeof(TransformControl));
		}

		[Test]
		public void CreatePlayable_ShouldTransferDataAndMetadata()
		{
			var metadata = new RecordingMetadata { sceneName = "TestScene", fps = new FrameRate(30, 1) };
			var data = new TransformData[]
			{
				new TransformData { position = new Vector3(1, 2, 3), rotation = Quaternion.identity, scale = new Vector3(1, 1, 1) },
				new TransformData { position = new Vector3(4, 5, 6), rotation = Quaternion.Euler(45, 45, 45), scale = new Vector3(2, 2, 2) }
			};

			var transformControlAsset = new TransformControlAsset
			{
				data = data,
				metadata = metadata
			};
			var playable = (ScriptPlayable<TransformControl>) transformControlAsset.CreatePlayable(graph, gameObject);
			var behaviour = playable.GetBehaviour();

			CollectionAssert.AreEqual(data, behaviour.data);
			Assert.AreEqual(metadata, behaviour.metadata);
		}

		[Test]
		public void CreatePlayable_WithEmptyDataArray_ShouldCreatePlayable()
		{
			var metadata = new RecordingMetadata { sceneName = "EmptyTest", fps = new FrameRate(24, 1) };

			var transformControlAsset = new TransformControlAsset
			{
				data = new TransformData[0],
				metadata = metadata
			};
			var playable = transformControlAsset.CreatePlayable(graph, gameObject);

			// Checking the type of Playable
			Assert.IsTrue(playable.GetPlayableType() == typeof(TransformControl));
		}
	}
}