using System;
using MixLib.Events;
using MixLib.Misc;

namespace MixLib.Device
{
	public class StreamStatus
	{
		private string mFileName;
		private long mPosition;
		private System.IO.Stream mStream;

        public bool PositionSet { get; private set; }

        public event ReportingEventHandler ReportingEvent;

		public StreamStatus()
		{
			Reset();
		}

		public void CloseStream()
		{
			if (mStream != null)
			{
				try
				{
					mStream.Close();
				}
				catch (Exception)
				{
				}
				mStream = null;
			}
		}

		private void onReportingEvent(ReportingEventArgs args)
		{
            ReportingEvent?.Invoke(this, args);
        }

		public void Reset()
		{
			CloseStream();
			mPosition = 0L;
			PositionSet = false;
		}

		public void UpdatePosition()
		{
			if (mStream != null)
			{
				try
				{
					Position = mStream.Position;
					PositionSet = true;
				}
				catch (Exception exception)
				{
					onReportingEvent(new ReportingEventArgs(Severity.Error, "exception while getting position in file " + FileName + ":" + exception.Message));
				}
			}
		}

		public string FileName
		{
			get
			{
				return mFileName;
			}
			set
			{
				if (mStream != null)
				{
					throw new InvalidOperationException("can't change filename on open stream. Open stream must first be closed.");
				}
				mFileName = value;
			}
		}

		public long Position
		{
			get
			{
				return mPosition;
			}
			set
			{
				mPosition = value;
				PositionSet = true;
				if (mStream != null)
				{
					try
					{
						mStream.Position = mPosition;
					}
					catch (Exception exception)
					{
						onReportingEvent(new ReportingEventArgs(Severity.Error, "exception while opening file " + FileName + ":" + exception.Message));
					}
				}
			}
		}

		public System.IO.Stream Stream
		{
			get
			{
				return mStream;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "stream may not be set to null");
				}

				if (mStream != null)
				{
					throw new InvalidOperationException("can't replace open stream. Open stream must first be closed.");
				}

				mStream = value;

				if (PositionSet)
				{
					mStream.Position = Position;
				}

				UpdatePosition();
			}
		}
	}
}
