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
		private int mRecordWordCount;
		private const string statusDescription = "Reading textual data";

		public TextReadStep(int recordWordCount)
		{
			mRecordWordCount = recordWordCount;
		}

        public override string StatusDescription => statusDescription;

        public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus, mRecordWordCount);

		public static List<IMixByteCollection> ReadBytes(Stream stream, int bytesPerRecord)
		{
			StreamReader reader = new StreamReader(stream, Encoding.ASCII);

			string readText;
			List<IMixByteCollection> readBytes = new List<IMixByteCollection>();

			while ((readText = reader.ReadLine()) != null)
			{
				readBytes.Add(new MixByteCollection(processReadText(readText, bytesPerRecord)));
			}

			return readBytes;
		}

		private static MixByte[] processReadText(string readText, int resultByteCount)
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

		private new class Instance : StreamStep.Instance
		{
			private MixByte[] mReadBytes;
			private int mRecordWordCount;

			public Instance(StreamStatus streamStatus, int recordWordCount)
				: base(streamStatus)
			{
				mRecordWordCount = recordWordCount;
			}

            public override object OutputForNextStep => mReadBytes;

            public override bool Tick()
			{
				if (base.StreamStatus.Stream != null)
				{
					string readText = null;
					try
					{
						readText = new StreamReader(base.StreamStatus.Stream, Encoding.ASCII).ReadLine();
						if (readText != null)
						{
							base.StreamStatus.Position = Math.Min(base.StreamStatus.Stream.Length, (base.StreamStatus.Position + readText.Length) + Environment.NewLine.Length);
						}
					}
					catch (Exception exception)
					{
						OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while reading file " + base.StreamStatus.FileName + ": " + exception.Message));
					}

					mReadBytes = processReadText(readText, mRecordWordCount * FullWord.ByteCount);
				}
				return true;
			}
		}
	}
}
