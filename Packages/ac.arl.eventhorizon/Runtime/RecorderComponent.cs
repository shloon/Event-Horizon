using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace EventHorizon
{
	[AddComponentMenu("Event Horizon/Recorder")]
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-100)]
	public class RecorderComponent : MonoBehaviour
	{
		public FrameRate frameRate = new FrameRate(60, 1);

		private ITrackableManager manager;
		private System.IO.Stream fileStream;
		private RecordingWriter recordingWriter;

		private float elapsedTime = 0;
		private int frames = 0;

		private void Start()
		{
			var recordingMetadata = new RecordingMetadata
			{
				sceneName = SceneManager.GetActiveScene().name,
				fps = frameRate
			};
			manager = TrackableManagerComponent.Instance;
			fileStream = System.IO.File.Create("Assets/Recordings/recording.evh");
			recordingWriter = new RecordingWriter(fileStream, recordingMetadata);
			recordingWriter.WriteHeader();
		}

		private void Update()
		{
			elapsedTime += Time.deltaTime;
			if (!(elapsedTime >= frameRate.GetFrameDuration())) return;

			recordingWriter.WriteFrame(recordingWriter.SerializeFrame(manager.RegisteredTrackables, frames));
			frames++;
			elapsedTime = 0;
		}

		private void OnApplicationQuit()
		{
			recordingWriter.WrapStream();
			fileStream.Close();
		}
	}
}