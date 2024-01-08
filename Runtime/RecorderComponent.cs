using EventHorizon.FormatV2;
using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EventHorizon
{
	[AddComponentMenu("Event Horizon/Recorder")]
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-100)]
	public class RecorderComponent : MonoBehaviour
	{
		public string outputFileName = "Assets/Recordings/recording-{scene}-{timestamp}.evh";
		public FrameRate frameRate = new(60);
		private ulong elapsedFrames;

		private double elapsedTime;
		private Stream fileStream;
		private ITrackableManager manager;

		private FormatWriter writer;

		private void Start()
		{
			manager = TrackableManagerComponent.Instance;

			// create subfolder
			var parentDirectory = Path.GetDirectoryName(outputFileName);
			if (!Directory.Exists(parentDirectory))
			{
				Directory.CreateDirectory(parentDirectory);
			}

			// create filestream
			var sceneName = SceneManager.GetActiveScene().name;
			var timestamp = DateTime.Now;
			fileStream = File.Create(ResolveOutputFileName(outputFileName, sceneName, timestamp));

			// initialize writer
			writer = new FormatWriter(fileStream);
			writer.WritePacket(PacketUtils.GenerateMetadataPacket(sceneName, frameRate, timestamp));
		}

		private void Update()
		{
			elapsedTime += Time.deltaTime;
			var frameDuration = frameRate.GetFrameDuration();

			while (elapsedTime >= frameDuration)
			{
				var framePacket = new FramePacket
				{
					frame = elapsedFrames, elapsedTime = frameDuration * elapsedFrames
				};
				writer.WritePacket(framePacket);
				GetTrackablePackets(elapsedFrames);

				elapsedFrames++;
				elapsedTime -= frameDuration;
			}
		}

		private void OnApplicationQuit()
		{
#if UNITY_EDITOR
			// TODO: maybe include actual progress
			EditorUtility.DisplayProgressBar("Event Horizon", "Writing the recording to disk...", 0.1f);
#endif

			writer?.Close();
			fileStream?.Close();

#if UNITY_EDITOR
			EditorUtility.ClearProgressBar();
#endif
		}

		public string ResolveOutputFileName(string fileName, string sceneName, DateTime timestamp) => fileName
			.Replace("{scene}", sceneName).Replace("{timestamp}",
				timestamp.ToString("yyyy-mm-dd--HH-MM-ss", CultureInfo.InvariantCulture));

		public void WriteCustomPacket<T>(in T packet) where T : IPacket => writer.WritePacket(packet);

		private void GetTrackablePackets(ulong frame)
		{
			foreach (var trackable in manager.RegisteredTrackables.Values)
			{
				switch (trackable)
				{
					case IPacketGenerator<TransformPacket> trackable1:
						writer.WritePacket(trackable1.GetPacketForFrame(frame));
						break;
					case IPacketGenerator<ActivationPacket> trackable2:
						writer.WritePacket(trackable2.GetPacketForFrame(frame));
						break;
					case IPacketGenerator<GenericDataPacket> trackable3:
						writer.WritePacket(trackable3.GetPacketForFrame(frame));
						break;
				}
			}
		}
	}
}