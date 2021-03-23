namespace MixLib.Misc
{

	public class ValidationError
	{
		public const int Unspecified = int.MinValue;

		public bool BoundsSpecified { get; private set; }
		public string Message { get; private set; }
		public int ValidLowerBound { get; private set; }
		public int ValidUpperBound { get; private set; }

		public ValidationError(string message)
		{
			Message = message;
			ValidLowerBound = Unspecified;
			ValidUpperBound = Unspecified;
			BoundsSpecified = false;
		}

		public ValidationError(int validLowerBound, int validUpperBound)
		{
			Message = null;
			ValidLowerBound = validLowerBound;
			ValidUpperBound = validUpperBound;
			BoundsSpecified = true;
		}

		public ValidationError(string message, int validLowerBound, int validUpperBound)
		{
			Message = message;
			ValidLowerBound = validLowerBound;
			ValidUpperBound = validUpperBound;
			BoundsSpecified = true;
		}

		public virtual string CompiledMessage
		{
			get
			{
				if (Message != null)
				{
					if (BoundsSpecified)
					{
						return string.Concat(Message, " (must be between ", ValidLowerBound, " and ", ValidUpperBound, ")");
					}

					return Message;
				}


				if (BoundsSpecified)
				{
					return string.Concat("value must be between ", ValidLowerBound, " and ", ValidUpperBound);
				}

				return "";
			}
		}
	}
}
