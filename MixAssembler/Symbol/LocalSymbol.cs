using MixLib.Type;
using System;
using System.Collections.Generic;

namespace MixAssembler.Symbol
{
	/// <summary>
	/// This class represents both local symbols and the references to them. The latter are implemented using the inner class 
	/// named reference.
	/// </summary>
	public class LocalSymbol : SymbolBase
	{
		readonly List<long> mAddresses;

		public LocalSymbol(int index) : base(index.ToString())
		{
			if (index < 0 || index > 9)
			{
				throw new ArgumentException("index must be between 0 and 9");
			}

			mAddresses = new List<long>();
		}

		public override bool IsMultiValuedSymbol => true;

		public override bool IsSymbolDefined => true;

		public override void SetValue(long value)
		{
			mAddresses.Add(value);
		}

		static bool IsDefinitionChar(char c)
		{
			return c == 'H';
		}

		static bool IsForwardReferenceChar(char c)
		{
			return c == 'F';
		}

		static bool IsLocalSymbol(string text)
		{
			return (text.Length == 2 && char.IsNumber(text[0])) && IsLocalSymbolChar(text[1]);
		}

		static bool IsLocalSymbolChar(char c)
		{
			return "BHF".Contains(c);
		}

		public override bool IsValueDefined(int currentAddress)
		{
			return false;
		}

		public override void SetValue(Word.Signs sign, long magnitude)
		{
			mAddresses.Add(magnitude);
		}

		public override long GetValue(int currentAddress)
		{
			throw new NotImplementedException();
		}

		public static SymbolBase ParseDefinition(string text, int sectionCharIndex, ParsingStatus status)
		{
			if (!IsLocalSymbol(text))
			{
				return null;
			}

			if (!IsDefinitionChar(text[1]))
			{
				status.ReportParsingError(sectionCharIndex + 1, 1, "local symbol reference found where definition was expected");
			}

			char localSymbolChar = text[0];
			SymbolBase symbol = status.Symbols[localSymbolChar.ToString()];
			if (symbol == null)
			{
				return new LocalSymbol(localSymbolChar - '0');
			}

			if (!(symbol is LocalSymbol))
			{
				status.ReportParsingError(sectionCharIndex, 1, "non-local symbol named " + localSymbolChar + " already defined");
			}

			return symbol;
		}

		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			if (!IsLocalSymbol(text))
			{
				return null;
			}

			char localSymbolChar = text[0];
			SymbolBase symbol = status.Symbols[new string(localSymbolChar, 1)];

			if (symbol == null)
			{
				symbol = new LocalSymbol(localSymbolChar - '0');
				status.Symbols.Add(symbol);
			}

			if (!(symbol is LocalSymbol))
			{
				status.ReportParsingError(sectionCharIndex, 1, "non-local symbol named " + localSymbolChar + " already defined, unable to refer to it");
				return symbol;
			}

			if (IsDefinitionChar(text[1]))
			{
				status.ReportParsingError(sectionCharIndex + 1, 1, "local symbol definition found where reference was expected");
				return symbol;
			}

			return new Reference((LocalSymbol)symbol, IsForwardReferenceChar(text[1]) ? Reference.Directions.Forwards : Reference.Directions.Backwards);
		}

		public ICollection<long> Addresses
		{
			get
			{
				mAddresses.Sort();
				return mAddresses;
			}
		}

		public override long GetMagnitude(int currentAddress)
		{
			throw new NotImplementedException();
		}

		public override Word.Signs GetSign(int currentAddress)
		{
			throw new NotImplementedException();
		}

		class Reference : IValue
		{
			readonly Directions mDirection;
			readonly LocalSymbol mReferee;

			public Reference(LocalSymbol referee, Directions direction)
			{
				mReferee = referee;
				mDirection = direction;
			}

			public long GetMagnitude(int currentAddress)
			{
				return GetValue(currentAddress);
			}

			public Word.Signs GetSign(int currentAddress)
			{
				return Word.Signs.Positive;
			}

			public bool IsValueDefined(int currentAddress)
			{
				return GetValue(currentAddress) != -1L;
			}

			/// <summary>
			/// Try to locate the symbol (i.e. address) that this reference refers to. 
			/// </summary>
			/// <param name="currentAddress"></param>
			/// <returns></returns>
			public long GetValue(int currentAddress)
			{
				long followingAddress = -1L;
				foreach (long refereeAddress in mReferee.Addresses)
				{
					long previousAddress = followingAddress;
					followingAddress = refereeAddress;

					if (mDirection == Directions.Backwards && followingAddress >= currentAddress)
					{
						return previousAddress;
					}

					if (mDirection == Directions.Forwards && previousAddress <= currentAddress && followingAddress > currentAddress)
					{
						return followingAddress;
					}
				}

				if (mDirection == Directions.Backwards)
				{
					return followingAddress;
				}

				return -1L;
			}

			public enum Directions
			{
				Backwards,
				Forwards
			}
		}
	}
}
