using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EventHorizon
{
	[AddComponentMenu("Event Horizon/Recorder")]
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-100)]
	public class RecorderComponent : MonoBehaviour
	{
		public string outputFileName = "Assets/Recordings/recording.evh";
		public FrameRate frameRate = new(60);

		private float elapsedTime;
		private Stream fileStream;
		private int frames;
		private ITrackableManager manager;

		private RecordingMetadata recordingMetadata;
		private RecordingWriter recordingWriter;

		private void Start()
		{
			recordingMetadata = new RecordingMetadata
			{
				sceneName = SceneManager.GetActiveScene().name,
				fps = frameRate
			};

			manager = TrackableManagerComponent.Instance;

			// create subfolder
			var parentDirectory = Path.GetDirectoryName(outputFileName);
			if (!Directory.Exists(parentDirectory))
			{
				Directory.CreateDirectory(parentDirectory);
			}

			// create filestream
			fileStream = File.Create(outputFileName);

			// initialize writer
			recordingWriter = new RecordingWriter(fileStream, recordingMetadata);
			recordingWriter.WriteHeader();
		}

		private void Update()
		{
			elapsedTime += Time.deltaTime;
			if (!(elapsedTime >= frameRate.GetFrameDuration()))
			{
				return;
			}

			recordingWriter.WriteFrame(
				RecordingFrameData.FromCurrentFrame(manager.RegisteredTrackables, recordingMetadata, frames));
			frames++;
			elapsedTime = 0;
		}

		private void OnApplicationQuit()
		{
			recordingWriter.WrapStream();
			recordingWriter.Close();
			fileStream.Close();
		}
	}
}