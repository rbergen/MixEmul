using System;
using System.IO;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device.Step
{
	public class BinaryReadStep(int recordWordCount) : StreamStep
	{
		private readonly int recordWordCount = recordWordCount;

		private const string MyStatusDescription = "Reading binary data";

		public override string StatusDescription 
			=> MyStatusDescription;

		public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus, this.recordWordCount);

		public static MixByte[] ReadBytes(Stream stream, int wordCount)
		{
			var buffer = new byte[wordCount * (FullWord.ByteCount + 1)];

			int readByteCount = stream.Read(buffer, 0, buffer.Length);
			var readBytes = new MixByte[buffer.Length];

			for (int index = 0; index < readByteCount; index++)
				readBytes[index] = buffer[index];

			for (int index = readByteCount; index < readBytes.Length; index++)
				readBytes[index] = 0;

			return readBytes;
		}

		public static IFullWord[] ReadWords(Stream stream, int wordCount)
		{
			var readBytes = ReadBytes(stream, wordCount);

			IFullWord[] readWords = new IFullWord[wordCount];
			IFullWord currentWord;
			int byteIndex = 0;

			for (int i = 0; i < wordCount; i++)
			{
				currentWord = new FullWord
				{
					Sign = readBytes[byteIndex++].ToSign()
				};

				for (int j = 0; j < FullWord.ByteCount; j++)
					currentWord[j] = readBytes[byteIndex++];

				readWords[i] = currentWord;
			}

			return readWords;
		}

		private new class Instance(StreamStatus streamStatus, int recordWordCount) : StreamStep.Instance(streamStatus)
		{
			private MixByte[] mReadBytes;

			public override object OutputForNextStep 
				=> mReadBytes;

			public override bool Tick()
			{
				if (StreamStatus.Stream == null)
					return true;

				try
				{
					mReadBytes = ReadBytes(StreamStatus.Stream, recordWordCount);
					StreamStatus.UpdatePosition();
				}
				catch (Exception exception)
				{
					OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while reading file " + StreamStatus.FileName + ": " + exception.Message));
					mReadBytes = [];
				}

				return true;
			}
		}
	}
}
