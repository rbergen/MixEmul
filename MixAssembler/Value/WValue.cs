using MixLib.Type;

namespace MixAssembler.Value
{
	/// <summary>
	/// This class represents a MIX W-value. 
	/// </summary>
	public static class WValue
	{
		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			// split the text to parse in its W-value components
			var textParts = text.Split(new char[] { ',' });
			int currentIndex = 0;

			var register = new FullWordRegister();
			var word = new FullWord(0);

			// parse and apply each component to the word that contains the result
			foreach (string part in textParts)
			{
				// parse the address part...
				var braceIndex = part.IndexOf('(');
				var address = ExpressionValue.ParseValue((braceIndex == -1) ? part : part.Substring(0, braceIndex), sectionCharIndex + currentIndex, status);
				if (address == null)
					return null;

				// ... and check if it is valid
				var addressSign = address.GetSign(status.LocationCounter);
				var addressMagnitude = address.GetMagnitude(status.LocationCounter);
				if (addressMagnitude > register.MaxMagnitude)
				{
					status.ReportParsingError(sectionCharIndex + currentIndex, (braceIndex == -1) ? part.Length : braceIndex, "W-value field value invalid");
					return null;
				}

				register.MagnitudeLongValue = addressMagnitude;
				register.Sign = addressSign;
				int fieldValue = FullWord.ByteCount;

				// if a fieldspec part is present...
				if (braceIndex >= 0)
				{
					// ... parse its value...
					var field = FPartValue.ParseValue(part[braceIndex..], (sectionCharIndex + currentIndex) + braceIndex, status);
					
					if (field == null)
						return null;

					// ... and check if it is valid
					if (field.GetValue(status.LocationCounter) != FPartValue.Default)
						fieldValue = (int)field.GetValue(status.LocationCounter);
				}

				// use the fieldspec value to create and check an actual fieldspec
				var fieldSpec = new FieldSpec(fieldValue);
				
				if (!fieldSpec.IsValid)
				{
					status.ReportParsingError((sectionCharIndex + currentIndex) + braceIndex, part.Length - braceIndex, "field must be a fieldspec");
					return null;
				}

				// apply the component to the word that will contain the end result
				WordField.LoadFromRegister(fieldSpec, register).ApplyToFullWord(word);
				currentIndex += part.Length + 1;
			}

			return new NumberValue(word.Sign, word.MagnitudeLongValue);
		}
	}
}
