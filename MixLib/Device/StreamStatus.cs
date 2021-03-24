using System;
using MixLib.Events;
using MixLib.Misc;

namespace MixLib.Device
{
	public class StreamStatus
	{
		private string _fileName;
		private long _position;
		private System.IO.Stream _stream;

		public bool PositionSet { get; private set; }

		public event ReportingEventHandler ReportingEvent;

		public StreamStatus() 
			=> Reset();

		private void OnReportingEvent(ReportingEventArgs args) 
			=> ReportingEvent?.Invoke(this, args);

		public void CloseStream()
		{
			_stream?.Close();
			_stream = null;
		}

		public void Reset()
		{
			CloseStream();
			_position = 0L;
			PositionSet = false;
		}

		public void UpdatePosition()
		{
			if (_stream == null)
				return;

			try
			{
				Position = _stream.Position;
				PositionSet = true;
			}
			catch (Exception exception)
			{
				OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while getting position in file " + FileName + ":" + exception.Message));
			}
		}

		public string FileName
		{
			get => _fileName;
			set
			{
				if (_stream != null)
					throw new InvalidOperationException("can't change filename on open stream. Open stream must first be closed.");

				_fileName = value;
			}
		}

		public long Position
		{
			get => _position;
			set
			{
				_position = value;
				PositionSet = true;

				if (_stream == null)
					return;

				try
				{
					_stream.Position = _position;
				}
				catch (Exception exception)
				{
					OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while opening file " + FileName + ":" + exception.Message));
				}
			}
		}

		public System.IO.Stream Stream
		{
			get => _stream;
			set
			{
				if (_stream != null)
					throw new InvalidOperationException("can't replace open stream. Open stream must first be closed.");

				_stream = value ?? throw new ArgumentNullException(nameof(value), "stream may not be set to null");

				if (PositionSet)
					_stream.Position = Position;

				UpdatePosition();
			}
		}
	}
}
