using System;
using MixLib.Events;
using MixLib.Misc;

namespace MixLib.Device
{
	public class StreamStatus
	{
		private string fileName;
		private long position;
		private System.IO.Stream stream;

		public bool PositionSet { get; private set; }

		public event ReportingEventHandler ReportingEvent;

		public StreamStatus() 
			=> Reset();

		private void OnReportingEvent(ReportingEventArgs args) 
			=> ReportingEvent?.Invoke(this, args);

		public void CloseStream()
		{
			this.stream?.Close();
			this.stream = null;
		}

		public void Reset()
		{
			CloseStream();
			this.position = 0L;
			PositionSet = false;
		}

		public void UpdatePosition()
		{
			if (this.stream == null)
				return;

			try
			{
				Position = this.stream.Position;
				PositionSet = true;
			}
			catch (Exception exception)
			{
				OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while getting position in file " + FileName + ":" + exception.Message));
			}
		}

		public string FileName
		{
			get => this.fileName;
			set
			{
				if (this.stream != null)
					throw new InvalidOperationException("can't change filename on open stream. Open stream must first be closed.");

				this.fileName = value;
			}
		}

		public long Position
		{
			get => this.position;
			set
			{
				this.position = value;
				PositionSet = true;

				if (this.stream == null)
					return;

				try
				{
					this.stream.Position = this.position;
				}
				catch (Exception exception)
				{
					OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while opening file " + FileName + ":" + exception.Message));
				}
			}
		}

		public System.IO.Stream Stream
		{
			get => this.stream;
			set
			{
				if (this.stream != null)
					throw new InvalidOperationException("can't replace open stream. Open stream must first be closed.");

				this.stream = value ?? throw new ArgumentNullException(nameof(value), "stream may not be set to null");

				if (PositionSet)
					this.stream.Position = Position;

				UpdatePosition();
			}
		}
	}
}
