using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MixLib.Device;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device.Step
{
	public class TextReadStep : StreamStep
	{
        int mRecordWordCount;
        const string statusDescription = "Reading textual data";

        public TextReadStep(int recordWordCount)
		{
			mRecordWordCount = recordWordCount;
		}

        public override string StatusDescription => statusDescription;

        public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus, mRecordWordCount);

		public static List<IMixByteCollection> ReadBytes(Stream stream, int bytesPerRecord)
		{
			var reader = new StreamReader(stream, Encoding.ASCII);

			string readText;
			var readBytes = new List<IMixByteCollection>();

			while ((readText = reader.ReadLine()) != null)
			{
				readBytes.Add(new MixByteCollection(processReadText(readText, bytesPerRecord)));
			}

			return readBytes;
		}

        static MixByte[] processReadText(string readText, int resultByteCount)
        {
            MixByte[] readBytes = new MixByte[resultByteCount];
            int readByteCount = (readText == null) ? 0 : Math.Min(readText.Length, resultByteCount);
            int index = 0;

            for (; index < readByteCount; index++)
            {
                readBytes[index] = readText[index];
            }

            for (; index < readBytes.Length; index++)
            {
                readBytes[index] = 0;
            }

            return readBytes;
        }

        new class Instance : StreamStep.Instance
        {
            MixByte[] mReadBytes;
            readonly int mRecordWordCount;

            public Instance(StreamStatus streamStatus, int recordWordCount)
                : base(streamStatus)
            {
                mRecordWordCount = recordWordCount;
            }

            public override object OutputForNextStep => mReadBytes;

            public override bool Tick()
            {
                if (StreamStatus.Stream != null)
                {
                    string readText = null;
                    try
                    {
                        readText = new StreamReader(StreamStatus.Stream, Encoding.ASCII).ReadLine();
                        if (readText != null)
                        {
                            StreamStatus.Position = Math.Min(StreamStatus.Stream.Length, StreamStatus.Position + readText.Length + Environment.NewLine.Length);
                        }
                    }
                    catch (Exception exception)
                    {
                        OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while reading file " + StreamStatus.FileName + ": " + exception.Message));
                    }

                    mReadBytes = processReadText(readText, mRecordWordCount * FullWord.ByteCount);
                }
                return true;
            }
        }
    }
}
