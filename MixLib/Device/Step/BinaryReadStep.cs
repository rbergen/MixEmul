using System;
using System.IO;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device.Step
{
    public class BinaryReadStep : StreamStep
	{
        int mRecordWordCount;
        const string statusDescription = "Reading binary data";

        public BinaryReadStep(int recordWordCount)
		{
			mRecordWordCount = recordWordCount;
		}

        public override string StatusDescription => statusDescription;

        public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus, mRecordWordCount);

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
				{
					currentWord[j] = readBytes[byteIndex++];
				}

				readWords[i] = currentWord;
			}

			return readWords;
		}

        new class Instance : StreamStep.Instance
        {
            MixByte[] mReadBytes;
            readonly int mRecordWordCount;

            public Instance(StreamStatus streamStatus, int recordWordCount) : base(streamStatus)
            {
                mRecordWordCount = recordWordCount;
            }

            public override object OutputForNextStep => mReadBytes;

            public override bool Tick()
            {
                if (StreamStatus.Stream == null) return true;

                try
                {
                    mReadBytes = ReadBytes(StreamStatus.Stream, mRecordWordCount);
                    StreamStatus.UpdatePosition();
                }
                catch (Exception exception)
                {
                    OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while reading file " + StreamStatus.FileName + ": " + exception.Message));
                    mReadBytes = new MixByte[0];
                }

                return true;
            }
        }
    }
}
