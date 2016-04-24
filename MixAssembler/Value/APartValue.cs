using MixAssembler.Symbol;
using MixLib.Type;

namespace MixAssembler.Value
{
    /// <summary>
    /// This class represents the absolute address part of an instruction's address section 
    /// </summary>
    public static class APartValue
    {
        public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
        {
            if (text.Length == 0)
            {
                return new NumberValue(0L);
            }

            // it can be either a literal value...
            IValue value = ValueSymbol.ParseValue(text, sectionCharIndex, status);
            if (value != null)
            {
                return value;
            }

            // ... a literal constant...
            value = LiteralConstantSymbol.ParseValue(text, sectionCharIndex, status);
            if (value != null)
            {
                return value;
            }

            // ... or an expression
            return ExpressionValue.ParseValue(text, sectionCharIndex, status);
        }
    }
}
