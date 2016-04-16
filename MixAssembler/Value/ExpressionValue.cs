using System;
using System.Collections.Generic;
using MixLib.Type;

namespace MixAssembler.Value
{
	/// <summary>
	/// This class represents a MIX expression value
	/// </summary>
	public class ExpressionValue
	{
		private const int fullWordBitCount = FullWord.ByteCount * MixByte.BitCount;
		private const long fullWordModulusMask = 1L << fullWordBitCount;

		// Holds the mappings of binary operation identifiers to the methods (delegates) that perform the actual action
		private static SortedList<string, operationDelegate> mBinaryOperations = new SortedList<string, operationDelegate>(new operationComparator());

		static ExpressionValue()
		{
			mBinaryOperations["+"] = doAdd;
			mBinaryOperations["-"] = doSubstract;
			mBinaryOperations["*"] = doMultiply;
			mBinaryOperations["/"] = doDivide;
			mBinaryOperations["//"] = doFractionDivide;
			mBinaryOperations[":"] = doCalculateField;
		}

		private static IValue doAdd(IValue left, IValue right, int currentAddress)
		{
			return new NumberValue((left.GetValue(currentAddress) + right.GetValue(currentAddress)) % fullWordModulusMask);
		}

		private static IValue doCalculateField(IValue left, IValue right, int currentAddress)
		{
			return new NumberValue(((left.GetValue(currentAddress) * 8L) + right.GetValue(currentAddress)) % fullWordModulusMask);
		}

		private static IValue doDivide(IValue left, IValue right, int currentAddress)
		{
			return new NumberValue((left.GetValue(currentAddress) / right.GetValue(currentAddress)) % fullWordModulusMask);
		}

		private static IValue doFractionDivide(IValue left, IValue right, int currentAddress)
		{
			decimal divider = new decimal(left.GetValue(currentAddress));
			divider *= fullWordModulusMask;
			divider /= right.GetValue(currentAddress);
			return new NumberValue((long)decimal.Remainder(decimal.Truncate(divider), ((int)1) << Math.Min(fullWordBitCount, 64)));
		}

		private static IValue doMultiply(IValue left, IValue right, int currentAddress)
		{
			return new NumberValue((left.GetValue(currentAddress) * right.GetValue(currentAddress)) % fullWordModulusMask);
		}

		private static IValue doSubstract(IValue left, IValue right, int currentAddress)
		{
			return new NumberValue((left.GetValue(currentAddress) - right.GetValue(currentAddress)) % fullWordModulusMask);
		}

		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			if (text.Length != 0)
			{
				// check if this expression is an atomic expression
				IValue value = AtomicExpressionValue.ParseValue(text, sectionCharIndex, status);
				if (value != null)
				{
					return value;
				}

				// if the expression is not an atomic one, it must be longer than 1 character
				if (text.Length == 1)
				{
					return null;
				}

				// check if this expression is an atomic expression preceded by a numeric sign
				if (text[0] == '+' || text[0] == '-')
				{
					value = AtomicExpressionValue.ParseValue(text.Substring(1), sectionCharIndex + 1, status);
					if (value != null)
					{
						return text[0] == '-' ? new NumberValue(Word.ChangeSign(value.GetSign(status.LocationCounter)), value.GetMagnitude(status.LocationCounter)) : value;
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

						int operatorIndex = text.LastIndexOf(currentOperator, text.Length - 2, text.Length - 2);
						// this find only counts if it is closer to the end of the expression than the previous find
						if (operatorIndex > (operatorPosition + 1))
						{
							operatorPosition = operatorIndex;
							operatorText = currentOperator;
							operatorDelegate = pair.Value;
						}
					}

					if (operatorPosition == -1)
					{
						// no operator found
						return null;
					}

					int rightTermStartIndex = operatorPosition + operatorText.Length;
					// the left term can itself be an expression, so parse it as one (recursively)
					IValue left = ParseValue(text.Substring(0, operatorPosition), sectionCharIndex, status);
					// the right term must be an atomic expression
					IValue right = AtomicExpressionValue.ParseValue(text.Substring(rightTermStartIndex), sectionCharIndex + rightTermStartIndex, status);

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
		private class operationComparator : IComparer<string>
		{
			public int Compare(string left, string right)
			{
				int lengthComparison = left.Length.CompareTo(right.Length);

				if (lengthComparison == 0)
				{
					return left.CompareTo(right);
				}
				return -lengthComparison;
			}
		}

		private delegate IValue operationDelegate(IValue left, IValue right, int currentAddress);
	}
}
