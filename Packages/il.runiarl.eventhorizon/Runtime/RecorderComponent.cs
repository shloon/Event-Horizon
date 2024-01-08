using EventHorizon.FormatV2;
using System;
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
		public string outputFileName = "Assets/Recordings/recording.evh2";
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
			fileStream = File.Create(outputFileName);

			// initialize writer
			writer = new FormatWriter(fileStream);
			writer.WritePacket(PacketUtils.GenerateMetadataPacket(SceneManager.GetActiveScene().name, frameRate,
				DateTime.Now));
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
			var progress = Progress.Start("Finishing up recording", options: Progress.Options.Indefinite);
#endif

			writer?.Close();
			fileStream?.Close();

#if UNITY_EDITOR
			Progress.Remove(progress);
#endif
		}

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