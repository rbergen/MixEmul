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
        readonly int mRecordWordCount;
        const string statusDescription = "Writing textual data";

        public TextWriteStep(int recordWordCount)
		{
			mRecordWordCount = recordWordCount;
		}

        public override string StatusDescription => statusDescription;

        public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus, mRecordWordCount);

		public static void WriteBytes(Stream stream, int bytesPerRecord, List<IMixByteCollection> bytes)
		{
			var writer = new StreamWriter(stream, Encoding.ASCII);

			foreach (IMixByteCollection collection in bytes)
			{
				writer.WriteLine(createStringFromBytes(collection.ToArray(), bytesPerRecord));
			}

			writer.Flush();
		}

        static string createStringFromBytes(MixByte[] bytes, int maxByteCount)
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

        new class Instance : StreamStep.Instance
        {
            readonly int mRecordWordCount;
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
                    var writer = new StreamWriter(StreamStatus.Stream, Encoding.ASCII);
                    writer.WriteLine(createStringFromBytes(mWriteBytes, mRecordWordCount * FullWord.ByteCount));
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
                set
                {
                    mWriteBytes = (MixByte[])value;
                }
            }
        }
    }
}
