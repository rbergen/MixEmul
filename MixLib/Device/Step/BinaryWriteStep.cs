using System;
using System.IO;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device.Step
{
    public class BinaryWriteStep : StreamStep
	{
        int mRecordWordCount;
        const string statusDescription = "Writing binary data";

        public BinaryWriteStep(int recordWordCount)
		{
			mRecordWordCount = recordWordCount;
		}

        public override string StatusDescription => statusDescription;

        public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => 
            new Instance(streamStatus, mRecordWordCount);

		public static void WriteWords(Stream stream, int wordCount, IFullWord[] writeWords)
		{
			MixByte[] writeBytes = new MixByte[writeWords.Length * (FullWord.ByteCount + 1)];

			int byteIndex = 0;
			foreach (IFullWord currentWord in writeWords)
			{
				writeBytes[byteIndex++] = currentWord.Sign.ToChar();

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
				buffer[index] = writeBytes[index];
			}

			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();
		}

        new class Instance : StreamStep.Instance
        {
            int mRecordWordCount;
            MixByte[] mWriteBytes;

            public Instance(StreamStatus streamStatus, int recordWordCount) : base(streamStatus)
            {
                mRecordWordCount = recordWordCount;
            }

            public override bool Tick()
            {
                if (StreamStatus.Stream == null || mWriteBytes == null) return true;

                try
                {
                    WriteBytes(StreamStatus.Stream, mRecordWordCount, mWriteBytes);
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
                set
                {
                    mWriteBytes = (MixByte[])value;
                }
            }
        }
    }
}
