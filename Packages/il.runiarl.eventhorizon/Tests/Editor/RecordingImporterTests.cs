using EventHorizon.Tests.Utilities;
using Moq;
using NUnit.Framework;
using System.IO;
using UnityEngine;

namespace EventHorizon.Editor.Tests
{
	public class RecordingImporterTests
	{
		[Test]
		public void ProcessImport_ShouldAddAndSetMainObjectCorrectly()
		{
			// Arrange
			var testAssetPath = RecordingTestUtils.CreateEmptyRecordingFile();
			var mockContext = new Mock<IAssetImportContext>();
			var importer = new RecordingImporter();

			RecordingDataScriptable capturedDataScriptable = null;
			mockContext.Setup(x => x.AddObjectToAsset("data", It.IsAny<Object>()))
				.Callback<string, Object>((key, dataScriptable) =>
					capturedDataScriptable = (RecordingDataScriptable) dataScriptable);

			// Act
			importer.ProcessImport(mockContext.Object, testAssetPath);

			// Assert
			mockContext.Verify(x => x.AddObjectToAsset("data", It.IsAny<RecordingDataScriptable>()), Times.Once);
			mockContext.Verify(x => x.SetMainObject(It.IsAny<RecordingDataScriptable>()), Times.Once);
			Assert.IsNotNull(capturedDataScriptable);

			// Cleanup
			File.Delete(testAssetPath);
		}


		[Test]
		public void ProcessImport_ShouldHaveNonNullDataInScriptableObject()
		{
			// Arrange
			var testAssetPath = RecordingTestUtils.CreateEmptyRecordingFile();
			var mockContext = new Mock<IAssetImportContext>();
			var importer = new RecordingImporter();

			RecordingDataScriptable capturedDataScriptable = null;
			mockContext.Setup(x => x.AddObjectToAsset("data", It.IsAny<Object>()))
				.Callback<string, Object>((key, dataScriptable) =>
					capturedDataScriptable = (RecordingDataScriptable) dataScriptable);

			// Act
			importer.ProcessImport(mockContext.Object, testAssetPath);

			// Assert
			Assert.IsNotNull(capturedDataScriptable, "The RecordingDataScriptable should not be null.");
			Assert.IsNotNull(capturedDataScriptable.data,
				"The data property of RecordingDataScriptable should not be null.");

			// Cleanup
			File.Delete(testAssetPath);
		}
	}
}