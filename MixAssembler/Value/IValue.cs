namespace MixAssembler.Value
{
  using System;
  using MixLib.Type;

  public interface IValue
  {
    long GetValue(int currentAddress);
    bool IsValueDefined(int currentAddress);
    long GetMagnitude(int currentAddress);
    Word.Signs GetSign(int currentAddress);
  }
}
