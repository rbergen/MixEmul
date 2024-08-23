using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device.Step
{
	public class TextReadStep(int recordWordCount) : StreamStep
	{
		private const string MyStatusDescription = "Reading textual data";

		public override string StatusDescription
			=> MyStatusDescription;

		public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus)
			=> new Instance(streamStatus, recordWordCount);

		public static List<IMixByteCollection> ReadBytes(Stream stream, int bytesPerRecord)
		{
			var reader = new StreamReader(stream, Encoding.ASCII);

			string readText;
			var readBytes = new List<IMixByteCollection>();

			while ((readText = reader.ReadLine()) != null)
				readBytes.Add(new MixByteCollection(ProcessReadText(readText, bytesPerRecord)));

			return readBytes;
		}

		private static MixByte[] ProcessReadText(string readText, int resultByteCount)
		{
			MixByte[] readBytes = new MixByte[resultByteCount];
			int readByteCount = (readText == null) ? 0 : Math.Min(readText.Length, resultByteCount);
			int index = 0;

			for (; index < readByteCount; index++)
				readBytes[index] = readText[index];

			for (; index < readBytes.Length; index++)
				readBytes[index] = 0;

			return readBytes;
		}

		private new class Instance(StreamStatus streamStatus, int recordWordCount) : StreamStep.Instance(streamStatus)
		{
			private MixByte[] readBytes;

			public override object OutputForNextStep
				=> this.readBytes;

			public override bool Tick()
			{
				if (StreamStatus.Stream == null)
					return true;

				string readText = null;
				try
				{
					readText = new StreamReader(StreamStatus.Stream, Encoding.ASCII).ReadLine();

					if (readText != null)
						StreamStatus.Position = Math.Min(StreamStatus.Stream.Length, StreamStatus.Position + readText.Length + Environment.NewLine.Length);
				}
				catch (Exception exception)
				{
					OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while reading file " + StreamStatus.FileName + ": " + exception.Message));
				}

				this.readBytes = ProcessReadText(readText, recordWordCount * FullWord.ByteCount);

				return true;
			}
		}
	}
}
