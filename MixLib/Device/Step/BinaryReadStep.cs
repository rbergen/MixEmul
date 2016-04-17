using System;
using System.IO;
using MixLib.Device;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device.Step
{
	public class BinaryReadStep : StreamStep
	{
		private int mRecordWordCount;
		private const string statusDescription = "Reading binary data";

		public BinaryReadStep(int recordWordCount)
		{
			mRecordWordCount = recordWordCount;
		}

		public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus)
		{
			return new Instance(streamStatus, mRecordWordCount);
		}

		public override string StatusDescription
		{
			get
			{
				return statusDescription;
			}
		}

		public static MixByte[] ReadBytes(Stream stream, int wordCount)
		{
			byte[] buffer = new byte[wordCount * (FullWord.ByteCount + 1)];

			int readByteCount = 0;

			readByteCount = stream.Read(buffer, 0, buffer.Length);

			MixByte[] readBytes = new MixByte[buffer.Length];
			for (int index = 0; index < readByteCount; index++)
			{
				readBytes[index] = buffer[index];
			}

			for (int index = readByteCount; index < readBytes.Length; index++)
			{
				readBytes[index] = 0;
			}

			return readBytes;
		}

		public static IFullWord[] ReadWords(Stream stream, int wordCount)
		{
			MixByte[] readBytes = ReadBytes(stream, wordCount);

			IFullWord[] readWords = new IFullWord[wordCount];
			IFullWord currentWord;
			int byteIndex = 0;

			for (int i = 0; i < wordCount; i++)
			{
				currentWord = new FullWord();

				currentWord.Sign = readBytes[byteIndex++].ToSign();

				for (int j = 0; j < FullWord.ByteCount; j++)
				{
					currentWord[j] = readBytes[byteIndex++];
				}

				readWords[i] = currentWord;
			}

			return readWords;
		}

		private new class Instance : StreamStep.Instance
		{
			private MixByte[] mReadBytes;
			private int mRecordWordCount;

			public Instance(StreamStatus streamStatus, int recordWordCount)
				: base(streamStatus)
			{
				mRecordWordCount = recordWordCount;
			}

			public override bool Tick()
			{
				if (base.StreamStatus.Stream != null)
				{
					try
					{
						mReadBytes = ReadBytes(base.StreamStatus.Stream, mRecordWordCount);
						base.StreamStatus.UpdatePosition();
					}
					catch (Exception exception)
					{
						OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while reading file " + base.StreamStatus.FileName + ": " + exception.Message));
						mReadBytes = new MixByte[0];
					}
				}

				return true;
			}

			public override object OutputForNextStep
			{
				get
				{
					return mReadBytes;
				}
			}
		}
	}
}
