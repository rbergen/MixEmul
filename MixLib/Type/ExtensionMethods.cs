namespace MixLib.Type
{
    public static class ExtensionMethods
    {
        public static long ApplyTo(this Word.Signs sign, long magnitude) => sign == Word.Signs.Negative ? -magnitude : magnitude;

        public static long Apply(this long magnitude, Word.Signs sign) => sign.ApplyTo(magnitude);

        public static Word.Signs GetSign(this long value) => value < 0 ? Word.Signs.Negative : Word.Signs.Positive;

        public static long GetMagnitude(this long value) => value < 0 ? -value : value;

        public static int GetMagnitude(this int value) => value < 0 ? -value : value;

        public static Word.Signs GetSign(this decimal value) => value < 0 ? Word.Signs.Negative : Word.Signs.Positive;

        public static decimal GetMagnitude(this decimal value) => value < 0 ? -value : value;

        public static Word.Signs Invert(this Word.Signs sign) => sign == Word.Signs.Positive ? Word.Signs.Negative : Word.Signs.Positive;

        public static char ToChar(this Word.Signs sign) => sign == Word.Signs.Positive ? '+' : '-';

        public static bool IsPositive(this Word.Signs sign) => sign == Word.Signs.Positive;

        public static bool IsNegative(this Word.Signs sign) => sign == Word.Signs.Negative;

        public static Word.Signs ToSign(this char c) => c == '-' ? Word.Signs.Negative : Word.Signs.Positive;

        public static Word.Signs ToSign(this MixByte b) => ((char)b).ToSign();

        public static Registers.CompValues Next(this Registers.CompValues current)
        {
            switch (current)
            {
                case Registers.CompValues.Equal:
                    return Registers.CompValues.Greater;

                case Registers.CompValues.Greater:
                    return Registers.CompValues.Less;
            }

            return Registers.CompValues.Equal;
        }

        public static char ToChar(this Registers.CompValues compValue)
        {
            switch (compValue)
            {
                case Registers.CompValues.Less:
                    return '<';

                case Registers.CompValues.Equal:
                    return '=';

                case Registers.CompValues.Greater:
                    return '>';
            }

            return '?';
        }

        public static Registers.CompValues ToCompValue(this int comparisonValue)
        {
            if (comparisonValue < 0)
            {
                return Registers.CompValues.Less;
            }
            else if (comparisonValue > 0)
            {
                return Registers.CompValues.Greater;
            }

            return Registers.CompValues.Equal;
        }
    }
}
