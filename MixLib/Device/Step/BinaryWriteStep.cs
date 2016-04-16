using System;
using System.IO;
using MixLib.Device;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device.Step
{
	public class BinaryWriteStep : StreamStep
	{
		private int mRecordWordCount;
		private const string statusDescription = "Writing binary data";

		public BinaryWriteStep(int recordWordCount)
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

		public static void WriteWords(Stream stream, int wordCount, IFullWord[] writeWords)
		{
			MixByte[] writeBytes = new MixByte[writeWords.Length * (FullWord.ByteCount + 1)];

			int byteIndex = 0;
			foreach (IFullWord currentWord in writeWords)
			{
				writeBytes[byteIndex++] = currentWord.Sign == Word.Signs.Negative ? (byte)'-' : (byte)'+';

				foreach (MixByte currentByte in currentWord)
				{
					writeBytes[byteIndex++] = currentByte;
				}
			}

			WriteBytes(stream, wordCount, writeBytes);
		}

		public static void WriteBytes(Stream stream, int wordCount, MixByte[] writeBytes)
		{
			byte[] buffer = new byte[wordCount * (FullWord.ByteCount + 1)];
			int bytesToWriteCount = Math.Min(buffer.Length, writeBytes.Length);

			for (int index = 0; index < bytesToWriteCount; index++)
			{
				buffer[index] = (byte)writeBytes[index];
			}

			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();
		}

		private new class Instance : StreamStep.Instance
		{
			private int mRecordWordCount;
			private MixByte[] mWriteBytes;

			public Instance(StreamStatus streamStatus, int recordWordCount)
				: base(streamStatus)
			{
				mRecordWordCount = recordWordCount;
			}

			public override bool Tick()
			{
				if (base.StreamStatus.Stream != null && mWriteBytes != null)
				{
					try
					{
						WriteBytes(base.StreamStatus.Stream, mRecordWordCount, mWriteBytes);
						base.StreamStatus.UpdatePosition();
					}
					catch (Exception exception)
					{
						OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while writing file " + base.StreamStatus.FileName + ": " + exception.Message));
					}
				}

				return true;
			}

			public override object InputFromPreviousStep
			{
				set
				{
					mWriteBytes = (MixByte[])value;
				}
			}
		}
	}
}
