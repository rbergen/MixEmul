using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device.Step
{
	public class TextWriteStep : StreamStep
	{
		private readonly int _recordWordCount;
		private const string MyStatusDescription = "Writing textual data";

		public TextWriteStep(int recordWordCount)
			=> _recordWordCount = recordWordCount;

		public override string StatusDescription
			=> MyStatusDescription;

		public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus)
			=> new Instance(streamStatus, _recordWordCount);

		public static void WriteBytes(Stream stream, int bytesPerRecord, List<IMixByteCollection> bytes)
		{
			var writer = new StreamWriter(stream, Encoding.ASCII);

			foreach (IMixByteCollection collection in bytes)
				writer.WriteLine(CreateStringFromBytes(collection.ToArray(), bytesPerRecord));

			writer.Flush();
		}

		private static string CreateStringFromBytes(MixByte[] bytes, int maxByteCount)
		{
			var charsToWriteCount = Math.Min(maxByteCount, bytes.Length);
			char[] charsToWrite = new char[charsToWriteCount];
			int index = 0;

			while (index < charsToWriteCount)
			{
				charsToWrite[index] = bytes[index];
				index++;
			}

			return new String(charsToWrite).TrimEnd(' ');
		}

		private new class Instance : StreamStep.Instance
		{
			private readonly int _recordWordCount;
			private MixByte[] _writeBytes;

			public Instance(StreamStatus streamStatus, int recordWordCount) : base(streamStatus)
				=> _recordWordCount = recordWordCount;

			public override bool Tick()
			{
				if (StreamStatus.Stream == null || _writeBytes == null)
					return true;

				try
				{
					var writer = new StreamWriter(StreamStatus.Stream, Encoding.ASCII);
					writer.WriteLine(CreateStringFromBytes(_writeBytes, _recordWordCount * FullWord.ByteCount));
					writer.Flush();
					StreamStatus.UpdatePosition();
				}
				catch (Exception exception)
				{
					OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while writing file " + StreamStatus.FileName + ": " + exception.Message));
				}

				return true;
			}

			public override object InputFromPreviousStep
			{
				set => _writeBytes = (MixByte[])value;
			}
		}
	}
}
