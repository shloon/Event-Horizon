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
		public FrameRate frameRate = new FrameRate(60, 1);

		private RecordingMetadata recordingMetadata;
		private ITrackableManager manager;
		private System.IO.Stream fileStream;
		private RecordingWriter recordingWriter;

		private float elapsedTime = 0;
		private int frames = 0;

		private void Start()
		{
			recordingMetadata = new RecordingMetadata
			{
				sceneName = SceneManager.GetActiveScene().name,
				fps = frameRate
			};

			manager = TrackableManagerComponent.Instance;

			// create subfolder
			var parentDirectory = System.IO.Path.GetDirectoryName(outputFileName);
			if (!System.IO.Directory.Exists(parentDirectory))
				System.IO.Directory.CreateDirectory(parentDirectory);

			// create filestream
			fileStream = System.IO.File.Create(outputFileName);

			// initialize writer
			recordingWriter = new RecordingWriter(fileStream, recordingMetadata);
			recordingWriter.WriteHeader();
		}

		private void Update()
		{
			elapsedTime += Time.deltaTime;
			if (!(elapsedTime >= frameRate.GetFrameDuration())) return;

			recordingWriter.WriteFrame(RecordingFrameData.FromCurrentFrame(manager.RegisteredTrackables, recordingMetadata, frames));
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