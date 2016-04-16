using System;
using System.Collections.Generic;
using MixLib.Type;

namespace MixAssembler.Symbol
{
  /// <summary>
  /// This class represents both local symbols and the references to them. The latter are implemented using the inner class 
  /// named reference.
  /// </summary>
  public class LocalSymbol : SymbolBase
  {
    private List<long> mAddresses;

    public LocalSymbol(int index)
      : base(index.ToString())
    {
      if (index < 0 || index > 9)
      {
        throw new ArgumentException("index must be between 0 and 9");
      }

      mAddresses = new List<long>();
    }

    public override long GetValue(int currentAddress)
    {
      throw new NotImplementedException();
    }

    private static bool isBackwardReferenceChar(char c)
    {
      return c == 'B';
    }

    private static bool isDefinitionChar(char c)
    {
      return c == 'H';
    }

    private static bool isForwardReferenceChar(char c)
    {
      return c == 'F';
    }

    private static bool isLocalSymbol(string text)
    {
      if (text.Length == 2 && char.IsNumber(text[0]))
      {
        return isLocalSymbolChar(text[1]);
      }
      return false;
    }

    private static bool isLocalSymbolChar(char c)
    {
      return "BHF".IndexOf(c) >= 0;
    }

    public override bool IsValueDefined(int currentAddress)
    {
      return false;
    }

    public static SymbolBase ParseDefinition(string text, int sectionCharIndex, ParsingStatus status)
    {
      if (!isLocalSymbol(text))
      {
        return null;
      }

      if (!isDefinitionChar(text[1]))
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
      if (!isLocalSymbol(text))
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

      if (isDefinitionChar(text[1]))
      {
        status.ReportParsingError(sectionCharIndex + 1, 1, "local symbol definition found where reference was expected");
        return symbol;
      }

      return new reference((LocalSymbol)symbol, isForwardReferenceChar(text[1]) ? reference.Directions.Forwards : reference.Directions.Backwards);
    }

    public override void SetValue(long address)
    {
      mAddresses.Add(address);
    }

    public ICollection<long> Addresses
    {
      get
      {
        mAddresses.Sort();
        return mAddresses;
      }
    }

    public override bool IsMultiValuedSymbol
    {
      get
      {
        return true;
      }
    }

    public override bool IsSymbolDefined
    {
      get
      {
        return true;
      }
    }

    private class reference : IValue
    {
      private Directions mDirection;
      private LocalSymbol mReferee;

      public reference(LocalSymbol referee, Directions direction)
      {
        mReferee = referee;
        mDirection = direction;
      }

      /// <summary>
      /// Try to locate the symbol (i.e. address) that this reference refers to. 
      /// </summary>
      /// <param name="currentAddress"></param>
      /// <returns></returns>
      public long GetValue(int currentAddress)
      {
        long previousAddress = -1L;
        long followingAddress = -1L;
        foreach (long refereeAddress in mReferee.Addresses)
        {
          previousAddress = followingAddress;
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

      public bool IsValueDefined(int currentAddress)
      {
        return GetValue(currentAddress) != -1L;
      }

      public enum Directions
      {
        Backwards,
        Forwards
      }

      #region IValue Members


      public long GetMagnitude(int currentAddress)
      {
        return GetValue(currentAddress);
      }

      public Word.Signs GetSign(int currentAddress)
      {
        return Word.Signs.Positive;
      }

      #endregion
    }

    public override void SetValue(Word.Signs sign, long magnitude)
    {
      mAddresses.Add(magnitude);
    }

    public override long GetMagnitude(int currentAddress)
    {
      throw new NotImplementedException();
    }

    public override Word.Signs GetSign(int currentAddress)
    {
      throw new NotImplementedException();
    }
  }
}
