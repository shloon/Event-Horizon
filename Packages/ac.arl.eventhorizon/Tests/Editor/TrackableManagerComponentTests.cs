using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EventHorizon.Editor.Tests
{
	public class TrackableManagerComponentTests
	{
		private GameObject testObject;
		private const string filename = "Assets/TempScript.cs";

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
				yield break;

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
			Assert.Throws<NullReferenceException>(() => _ = TrackableManagerComponent.Instance);

			yield return new RecompileScripts();

			Assert.Throws<NullReferenceException>(() => _ = TrackableManagerComponent.Instance);
		}
	}
}