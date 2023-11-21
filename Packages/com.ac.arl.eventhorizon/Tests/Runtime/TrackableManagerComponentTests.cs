using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EventHorizon.Tests
{
	public class TrackableManagerComponentTests
	{
		private GameObject testObject;

		[SetUp]
		public void Setup()
		{
			// Ensure no instance exists before each test
			var existingInstance = Object.FindObjectOfType<TrackableManagerComponent>();
			if (existingInstance != null)
			{
				Object.DestroyImmediate(existingInstance.gameObject);
			}

			// Create a new GameObject for testing
			testObject = new GameObject("TestObject");
		}

		[TearDown]
		public void Teardown()
		{
			// Clean up after each test
			if (testObject != null)
			{
				Object.DestroyImmediate(testObject);
				testObject = null;
			}
		}

		[Test]
		public void TestInitialInstanceCreation()
		{
			testObject.AddComponent<TrackableManagerComponent>();
			Assert.IsNotNull(TrackableManagerComponent.Instance);
		}

		[UnityTest]
		public IEnumerator TestSingletonUniqueness()
		{
			var testManager = testObject.AddComponent<TrackableManagerComponent>();
			yield return null;

			var anotherManagerObject = new GameObject("AnotherTestObject");
			var anotherManager = anotherManagerObject.AddComponent<TrackableManagerComponent>();

			LogAssert.Expect(LogType.Exception,
				"InvalidOperationException: Another instance of TrackableManager already exists.");
			// ideally, we would `return yield null` to call `Awake` automatically and somehow expect InvalidOperationException here,
			// but I think that's currently impossible
			Assert.Throws<InvalidOperationException>(() => { 
				anotherManager.Awake();
			});
			Object.DestroyImmediate(anotherManagerObject);
		}

		[UnityTest]
		public IEnumerator TestPersistenceAcrossScenes()
		{
			testObject.AddComponent<TrackableManagerComponent>();
			yield return null;
			
			yield return SceneManager.LoadSceneAsync("EmptyScene");
			
			Assert.IsNotNull(TrackableManagerComponent.Instance);
		}

		[Test]
		public void TestInstanceNullAfterDestruction()
		{
			testObject.AddComponent<TrackableManagerComponent>();
			Object.DestroyImmediate(testObject);

			LogAssert.Expect(LogType.Error, "An instance of TrackableManager is needed in the scene, but there is none.");
			Assert.IsNull(TrackableManagerComponent.Instance);
		}
	}
}