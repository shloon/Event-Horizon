using EventHorizon.Tests.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EventHorizon.Editor.Tests
{
	public class TrackableManagerComponentTests
	{
		private const string filename = "Assets/TempScript.cs";
		private GameObject testObject;

		[SetUp]
		public void SetUp()
		{
			if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				throw new InvalidOperationException("You must save your scene before continuing");
			}

			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			testObject = new GameObject("TestObject");
		}

		[UnityTearDown]
		public IEnumerator Teardown()
		{
			// clean up file
			if (!File.Exists(filename))
			{
				yield break;
			}

			File.Delete(filename);
			yield return new RecompileScripts();

			// Clean up after each test
			if (testObject != null)
			{
				Object.DestroyImmediate(testObject);
				testObject = null;
			}
		}

		[UnityTest]
		public IEnumerator Manager_Exists_ReinitializesAfterDomainReload()
		{
			var manager = testObject.AddComponent<TrackableManagerComponent>();
			yield return null;

			File.WriteAllText(filename, "namespace EventHorizon.Test.ReloadTest { public class TestClass {} }");
			yield return new RecompileScripts();

			Assert.IsNotNull(TrackableManagerComponent.Instance);
			Assert.AreEqual(manager, TrackableManagerComponent.Instance);
			Assert.IsNotNull(TrackableManagerComponent.Instance.RegisteredTrackables);
		}

		[UnityTest]
		public IEnumerator Manager_NotExists_StillDoesNotExistAfterReload()
		{
			File.WriteAllText(filename, "namespace EventHorizon.Test.ReloadTest { public class TestClass {} }");
			yield return new RecompileScripts();

			Assert.Throws<NullReferenceException>(() => _ = TrackableManagerComponent.Instance);
		}

		[Test]
		public void Manager_RegistersElementImmediately()
		{
			var manager = testObject.AddComponent<TrackableManagerComponent>();

			var key1 = new TrackableID(1);
			var key2 = new TrackableID(2);
			var trackableComponent1 = TrackableTestUtils.CreateTrackableGameObject(manager, key1);
			var trackableComponent2 = TrackableTestUtils.CreateTrackableGameObject(manager, key2);

			Assert.IsTrue(manager.RegisteredTrackables.ContainsKey(key1));
			Assert.AreSame(trackableComponent1, manager.RegisteredTrackables[key1]);
			Assert.IsTrue(manager.RegisteredTrackables.ContainsKey(key2));
			Assert.AreSame(trackableComponent2, manager.RegisteredTrackables[key2]);
		}

		[UnityTest]
		public IEnumerator Manager_RegistersElementAfterDomainReload()
		{
			var manager = testObject.AddComponent<TrackableManagerComponent>();
			var key1 = new TrackableID(1);
			var key2 = new TrackableID(2);
			var trackableComponent1 = TrackableTestUtils.CreateTrackableGameObject(manager, key1);
			var trackableComponent2 = TrackableTestUtils.CreateTrackableGameObject(manager, key2);

			File.WriteAllText(filename, "namespace EventHorizon.Test.ReloadTest { public class TestClass {} }");
			yield return new RecompileScripts();

			Assert.IsTrue(manager.RegisteredTrackables.ContainsKey(key1));
			Assert.AreSame(trackableComponent1, manager.RegisteredTrackables[key1]);
			Assert.IsTrue(manager.RegisteredTrackables.ContainsKey(key2));
			Assert.AreSame(trackableComponent2, manager.RegisteredTrackables[key2]);

			TrackableTestUtils.DestroyTrackableGameObject(trackableComponent1);
			TrackableTestUtils.DestroyTrackableGameObject(trackableComponent2);
		}
	}
}