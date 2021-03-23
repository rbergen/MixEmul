namespace MixAssembler.Symbol
{
  using MixAssembler.Value;
  using System;
  using MixLib.Type;

  public abstract class SymbolBase : IValue
  {
    private string mName;

    protected SymbolBase(string name)
    {
      mName = name;
    }

    public abstract long GetValue(int currentAddress);

    public abstract bool IsValueDefined(int currentAddress);

    public abstract void SetValue(long value);

    public abstract void SetValue(Word.Signs sign, long magnitude);

    public virtual bool IsMultiValuedSymbol
    {
      get
      {
        return false;
      }
    }

    public abstract bool IsSymbolDefined
    {
      get;
    }

    public virtual long MemoryWordValue
    {
      get
      {
        return 0L;
      }
    }

    public virtual long MemoryWordMagnitude
    {
      get
      {
        return 0L;
      }
    }

    public virtual Word.Signs MemoryWordSign
    {
      get
      {
        return Word.Signs.Positive;
      }
    }

    public string Name
    {
      get
      {
        return mName;
      }
    }

    #region IValue Members

    public abstract long GetMagnitude(int currentAddress);
    public abstract Word.Signs GetSign(int currentAddress);

    #endregion
  }
}
