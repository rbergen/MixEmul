using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MixLib.Instruction;
using MixLib.Type;

namespace MixGui.Utils
{
	public static class CardDeckExporter
	{
		public static readonly string[] DefaultLoaderCards =
		{
			" O O6 A O4 2 O6 C O4   BK 2DO6   BI G O4 3D-H M BB B  U 3DEH A  F F CF 0  E B LU",
			" 3DIH M BB B EJ  CA. 2DEU B EH K BA B EU 5A-H M BB  C U 4AEH 5AEN    E  CLU  ABG",
			" 2DEH K BB Q B. E  9"
		};

		private static string[] loaderCards;

		private const int MaxWordsPerCard = 7;

		private static string GetTransLine(int programCounter) 
			=> "TRANS0" + GetAddressText(programCounter);

		private static char GetNegativeDigit(char digit) 
			=> MixByte.MixChars[MixByte.MixChars.IndexOf(digit) - 30];

		public static string[] LoaderCards
		{
			get => loaderCards ?? DefaultLoaderCards;
			set => loaderCards = value;
		}

		private static StreamWriter PrepareWriter(string filePath)
		{
			var writer = new StreamWriter(filePath, false, Encoding.ASCII);

			foreach (string loaderCard in LoaderCards.Where(card => card != null && card.TrimEnd() != string.Empty))
				writer.WriteLine(loaderCard);

			return writer;
		}


		public static void ExportFullWords(string filePath, IList<IFullWord> wordsToWrite, int firstWordLocation, int programCounter)
		{
			var words = new List<IFullWord>();

			var writer = PrepareWriter(filePath);

			foreach (IFullWord word in wordsToWrite)
			{
				words.Add(word);

				if (words.Count == MaxWordsPerCard)
				{
					writer.WriteLine(GetInformationLine(firstWordLocation, words));
					words.Clear();
					firstWordLocation += MaxWordsPerCard;
				}
			}

			if (words.Count > 0)
				writer.WriteLine(GetInformationLine(firstWordLocation, words));

			writer.WriteLine(GetTransLine(programCounter));
			writer.Close();
		}

		public static void ExportInstructions(string filePath, InstructionInstanceBase[] instances)
		{
			var words = new List<IFullWord>();
			int firstWordLocation = 0;
			int locationCounter = 0;

			var writer = PrepareWriter(filePath);

			foreach (InstructionInstanceBase instance in instances)
			{
				switch (instance)
				{
					case LoaderInstruction.Instance loaderInstance:
						switch (((LoaderInstruction)instance.Instruction).Operation)
						{
							case LoaderInstruction.Operations.SetLocationCounter:
								if (words.Count > 0)
									writer.WriteLine(GetInformationLine(firstWordLocation, words));

								words.Clear();
								firstWordLocation = locationCounter = (int)loaderInstance.Value.LongValue;

								break;

							case LoaderInstruction.Operations.SetMemoryWord:
								words.Add(loaderInstance.Value);
								locationCounter++;

								break;

							case LoaderInstruction.Operations.SetProgramCounter:
								if (words.Count > 0)
									writer.WriteLine(GetInformationLine(firstWordLocation, words));

								writer.WriteLine(GetTransLine((int)loaderInstance.Value.LongValue));
								writer.Close();
								return;
						}
						break;

					case MixInstruction.Instance mixInstance:
						words.Add(mixInstance.InstructionWord);
						locationCounter++;
						break;
				}

				if (words.Count == MaxWordsPerCard)
				{
					writer.WriteLine(GetInformationLine(firstWordLocation, words));
					words.Clear();
					firstWordLocation = locationCounter;
				}
			}
		}

		private static string GetAddressText(int address)
		{
			if (address < 0)
			{
				address = -address;
				var addressText = address.ToString("0000");
				return addressText.Substring(0, 3) + GetNegativeDigit(addressText[3]);
			}

			return address.ToString("0000");
		}

		private static string GetInformationLine(int firstWordLocation, List<IFullWord> words)
		{
			var lineBuilder = new StringBuilder("INFO ");

			lineBuilder.Append(words.Count);
			lineBuilder.Append(GetAddressText(firstWordLocation));

			string numberText;

			foreach (IFullWord word in words)
			{
				numberText = word.MagnitudeLongValue.ToString("0000000000");

				if (word.Sign.IsPositive())
					lineBuilder.Append(numberText);

				else
				{
					lineBuilder.Append(numberText.Substring(0, 9));
					lineBuilder.Append(GetNegativeDigit(numberText[9]));
				}
			}

			return lineBuilder.ToString();
		}
	}
}
