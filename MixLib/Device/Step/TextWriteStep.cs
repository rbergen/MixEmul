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
		private int mRecordWordCount;
		private const string statusDescription = "Writing textual data";

		public TextWriteStep(int recordWordCount)
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

		public static void WriteBytes(Stream stream, int bytesPerRecord, List<IMixByteCollection> bytes)
		{
			StreamWriter writer = new StreamWriter(stream, Encoding.ASCII);

			foreach (IMixByteCollection collection in bytes)
			{
				writer.WriteLine(createStringFromBytes(collection.ToArray(), bytesPerRecord));
			}

			writer.Flush();
		}

		private static string createStringFromBytes(MixByte[] bytes, int maxByteCount)
		{
			int charsToWriteCount = Math.Min(maxByteCount, bytes.Length);
			char[] charsToWrite = new char[charsToWriteCount];
			int index = 0;

			while (index < charsToWriteCount)
			{
				charsToWrite[index] = (char)bytes[index];
				index++;
			}

			return new String(charsToWrite).TrimEnd(' ');
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
						StreamWriter writer = new StreamWriter(base.StreamStatus.Stream, Encoding.ASCII);
						writer.WriteLine(createStringFromBytes(mWriteBytes, mRecordWordCount * FullWord.ByteCount));
						writer.Flush();
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
