using System;
using System.IO;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device.Step
{
	public class BinaryWriteStep : StreamStep
	{
		private readonly int _recordWordCount;
		private const string MyStatusDescription = "Writing binary data";

		public BinaryWriteStep(int recordWordCount) 
			=> _recordWordCount = recordWordCount;

		public override string StatusDescription 
			=> MyStatusDescription;

		public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) 
			=> new Instance(streamStatus, _recordWordCount);

		public static void WriteWords(Stream stream, int wordCount, IFullWord[] writeWords)
		{
			MixByte[] writeBytes = new MixByte[writeWords.Length * (FullWord.ByteCount + 1)];

			int byteIndex = 0;
			foreach (IFullWord currentWord in writeWords)
			{
				writeBytes[byteIndex++] = currentWord.Sign.ToChar();

				foreach (MixByte currentByte in currentWord)
					writeBytes[byteIndex++] = currentByte;
			}

			WriteBytes(stream, wordCount, writeBytes);
		}

		public static void WriteBytes(Stream stream, int wordCount, MixByte[] writeBytes)
		{
			byte[] buffer = new byte[wordCount * (FullWord.ByteCount + 1)];
			var bytesToWriteCount = Math.Min(buffer.Length, writeBytes.Length);

			for (int index = 0; index < bytesToWriteCount; index++)
				buffer[index] = writeBytes[index];

			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();
		}

		private new class Instance : StreamStep.Instance
		{
			private readonly int _recordWordCount;
			private MixByte[] _writeBytes;

			public Instance(StreamStatus streamStatus, int recordWordCount) : base(streamStatus) => _recordWordCount = recordWordCount;

			public override bool Tick()
			{
				if (StreamStatus.Stream == null || _writeBytes == null)
					return true;

				try
				{
					WriteBytes(StreamStatus.Stream, _recordWordCount, _writeBytes);
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
