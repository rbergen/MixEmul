using System;
using System.Collections.Generic;
using MixLib.Type;

namespace MixAssembler.Value
{
	/// <summary>
	/// This class represents a MIX expression value
	/// </summary>
	public static class ExpressionValue
	{
        const int fullWordBitCount = FullWord.ByteCount * MixByte.BitCount;
        const long fullWordModulusMask = 1L << fullWordBitCount;

        // Holds the mappings of binary operation identifiers to the methods (delegates) that perform the actual action
        static SortedList<string, operationDelegate> mBinaryOperations = new SortedList<string, operationDelegate>(new OperationComparator());

        static ExpressionValue()
		{
			mBinaryOperations["+"] = DoAdd;
			mBinaryOperations["-"] = DoSubstract;
			mBinaryOperations["*"] = DoMultiply;
			mBinaryOperations["/"] = DoDivide;
			mBinaryOperations["//"] = DoFractionDivide;
			mBinaryOperations[":"] = DoCalculateField;
		}

        static IValue DoAdd(IValue left, IValue right, int currentAddress) =>
            new NumberValue((left.GetValue(currentAddress) + right.GetValue(currentAddress)) % fullWordModulusMask);

        static IValue DoCalculateField(IValue left, IValue right, int currentAddress) =>
            new NumberValue(((left.GetValue(currentAddress) * 8L) + right.GetValue(currentAddress)) % fullWordModulusMask);

        static IValue DoDivide(IValue left, IValue right, int currentAddress) =>
            new NumberValue((left.GetValue(currentAddress) / right.GetValue(currentAddress)) % fullWordModulusMask);

        static IValue DoMultiply(IValue left, IValue right, int currentAddress) =>
            new NumberValue((left.GetValue(currentAddress) * right.GetValue(currentAddress)) % fullWordModulusMask);

        static IValue DoSubstract(IValue left, IValue right, int currentAddress) =>
            new NumberValue((left.GetValue(currentAddress) - right.GetValue(currentAddress)) % fullWordModulusMask);

        static IValue DoFractionDivide(IValue left, IValue right, int currentAddress)
        {
            var divider = new decimal(left.GetValue(currentAddress));
            divider *= fullWordModulusMask;
            divider /= right.GetValue(currentAddress);
            return new NumberValue((long)decimal.Remainder(decimal.Truncate(divider), 1 << Math.Min(fullWordBitCount, 64)));
        }

        public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			if (text.Length != 0)
			{
				// check if this expression is an atomic expression
				var value = AtomicExpressionValue.ParseValue(text, sectionCharIndex, status);
				if (value != null) return value;

				// if the expression is not an atomic one, it must be longer than 1 character
				if (text.Length == 1) return null;

				// check if this expression is an atomic expression preceded by a numeric sign
				if (text[0] == '+' || text[0] == '-')
				{
					value = AtomicExpressionValue.ParseValue(text.Substring(1), sectionCharIndex + 1, status);
					if (value != null)
					{
						return text[0] == '-' ? new NumberValue(value.GetSign(status.LocationCounter).Invert(), value.GetMagnitude(status.LocationCounter)) : value;
					}
				}

				// check if this expression contains one or more binary operations
				if (text.Length >= 3)
				{
					int operatorPosition = -1;
					string operatorText = null;
					operationDelegate operatorDelegate = null;

					// find the last operator included the expression, if any
					foreach (KeyValuePair<string, operationDelegate> pair in mBinaryOperations)
					{
						string currentOperator = pair.Key;

						var operatorIndex = text.LastIndexOf(currentOperator, text.Length - 2, text.Length - 2, StringComparison.Ordinal);
						// this find only counts if it is closer to the end of the expression than the previous find
						if (operatorIndex > (operatorPosition + 1))
						{
							operatorPosition = operatorIndex;
							operatorText = currentOperator;
							operatorDelegate = pair.Value;
						}
					}

					if (operatorPosition == -1) return null;

					int rightTermStartIndex = operatorPosition + operatorText.Length;
					// the left term can itself be an expression, so parse it as one (recursively)
					var left = ParseValue(text.Substring(0, operatorPosition), sectionCharIndex, status);
					// the right term must be an atomic expression
					var right = AtomicExpressionValue.ParseValue(text.Substring(rightTermStartIndex), sectionCharIndex + rightTermStartIndex, status);

					if (left != null && right != null)
					{
						// both terms were successfully parsed. Perform the operation and return the result
						return operatorDelegate(left, right, status.LocationCounter);
					}
				}
			}
			return null;
		}

        // compare operators. Longer operators end up higher than shorter ones
        class OperationComparator : IComparer<string>
        {
            public int Compare(string left, string right)
            {
                var lengthComparison = left.Length.CompareTo(right.Length);

                if (lengthComparison == 0) return string.Compare(left, right, StringComparison.Ordinal);

                return -lengthComparison;
            }
        }

        delegate IValue operationDelegate(IValue left, IValue right, int currentAddress);
    }
}
