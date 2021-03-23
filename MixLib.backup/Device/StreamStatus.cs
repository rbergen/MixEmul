using MixLib.Events;
using MixLib.Misc;
using System;

namespace MixLib.Device
{
	public class StreamStatus
	{
		string mFileName;
		long mPosition;
		System.IO.Stream mStream;

		public bool PositionSet { get; private set; }

		public event ReportingEventHandler ReportingEvent;

		public StreamStatus()
		{
			Reset();
		}

		void OnReportingEvent(ReportingEventArgs args) => ReportingEvent?.Invoke(this, args);

		public void CloseStream()
		{
			mStream?.Close();
			mStream = null;
		}

		public void Reset()
		{
			CloseStream();
			mPosition = 0L;
			PositionSet = false;
		}

		public void UpdatePosition()
		{
			if (mStream == null) return;

			try
			{
				Position = mStream.Position;
				PositionSet = true;
			}
			catch (Exception exception)
			{
				OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while getting position in file " + FileName + ":" + exception.Message));
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
				if (mStream == null) return;

				try
				{
					mStream.Position = mPosition;
				}
				catch (Exception exception)
				{
					OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while opening file " + FileName + ":" + exception.Message));
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
				if (mStream != null)
				{
					throw new InvalidOperationException("can't replace open stream. Open stream must first be closed.");
				}

				mStream = value ?? throw new ArgumentNullException(nameof(value), "stream may not be set to null");

				if (PositionSet)
				{
					mStream.Position = Position;
				}

				UpdatePosition();
			}
		}
	}
}
