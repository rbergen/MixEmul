using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MixLib.Type;

namespace MixLib
{
	public class Memory(int minIndex, int maxIndex) : IMemory, IEnumerable
	{
		public static readonly FieldSpec DefaultFieldSpec = new(0, 5);

		private readonly SortedDictionary<int, MemoryFullWord> words = [];
		private readonly Lock syncLock = new();

		public int MinWordIndex { get; set; } = minIndex;
		public int MaxWordIndex { get; set; } = maxIndex;

		public int WordCount
			=> MaxWordIndex - MinWordIndex + 1;

		public void ResetProfilingCounts()
		{
			lock (syncLock)
			{
				foreach (MemoryFullWord word in this.words.Values)
					word.ResetProfilingCounts();
			}
		}

		public long MaxProfilingTickCount
		{
			get
			{
				lock (syncLock)
				{
					return this.words.Count == 0 ? 0 : this.words.Values.Select(w => w.ProfilingTickCount).Max();
				}
			}
		}

		public long MaxProfilingExecutionCount
		{
			get
			{
				lock (syncLock)
				{
					return this.words.Count == 0 ? 0 : this.words.Values.Select(w => w.ProfilingExecutionCount).Max();
				}
			}
		}

		public SearchResult FindMatch(SearchParameters options)
		{
			int startIndex = options.SearchFromWordIndex ?? MinWordIndex;
			int index = startIndex;
			bool virtualSearchPerformed = false;
			bool searchWrapped = false;
			SearchResult result;
			MemoryFullWord word;

			while (!searchWrapped || index < startIndex)
			{
				lock (syncLock)
				{
					this.words.TryGetValue(index, out word);
				}

				if (word != null)
				{
					result = word.FindMatch(options);

					if (result != null)
					{
						result.WordIndex = index;
						return result;
					}
				}
				else if (!virtualSearchPerformed)
				{
					result = VirtualMemoryFullWord.FindDefaultMatch(options, index == startIndex);

					if (result != null)
					{
						result.WordIndex = index;
						return result;
					}

					virtualSearchPerformed |= index != startIndex;
				}

				if (!virtualSearchPerformed)
				{
					index++;
					continue;
				}

				KeyValuePair<int, MemoryFullWord>? pair;

				lock (syncLock)
				{
					pair = this.words.Cast<KeyValuePair<int, MemoryFullWord>?>().FirstOrDefault(kvp => kvp.Value.Key > index);
				}

				if (pair != null)
				{
					index = pair.Value.Key;
					continue;
				}

				if (options.WrapSearch && !searchWrapped)
				{
					index = MinWordIndex;
					searchWrapped = true;
					continue;
				}

				return null;
			}

			return null;
		}

		public IEnumerator GetEnumerator()
		{
			lock (syncLock)
			{
				return this.words.Values.GetEnumerator();
			}
		}

		public void ClearSourceLines()
		{
			lock (syncLock)
			{
				foreach (MemoryFullWord word in this.words.Values)
					word.SourceLine = null;
			}
		}

		public void Reset()
		{
			lock (syncLock)
			{
				this.words.Clear();
			}
		}

		public IMemoryFullWord this[int index]
		{
			get
			{
				if (index < MinWordIndex || index > MaxWordIndex)
					throw new ArgumentOutOfRangeException(nameof(index), $"value must be between MinWordIndex ({MinWordIndex}) and MaxWordIndex ({MaxWordIndex}), inclusive");

				MemoryFullWord word;

				lock (syncLock)
				{
					this.words.TryGetValue(index, out word);
				}

				return (IMemoryFullWord)word ?? new VirtualMemoryFullWord(this, index);
			}
			set
			{
				if (index < MinWordIndex || index > MaxWordIndex)
					throw new IndexOutOfRangeException($"index must be between MinWordIndex ({MinWordIndex}) and MaxWordIndex ({MaxWordIndex}), inclusive");

				var word = GetRealWord(index);

				for (int byteIndex = 0; byteIndex < value.ByteCount; byteIndex++)
					word[byteIndex] = value[byteIndex];

				word.Sign = value.Sign;
			}
		}

		public MemoryFullWord GetRealWord(int index)
		{
			MemoryFullWord word;

			lock (syncLock)
			{
				if (!this.words.TryGetValue(index, out word))
				{
					word = new MemoryFullWord(index, 0);
					this.words[index] = word;
				}
			}

			return word;
		}

		public bool HasContents(int index)
		{
			lock (syncLock)
			{
				return this.words.ContainsKey(index) && !this.words[index].IsEmpty;
			}
		}

		public int? LastAddressWithContentsBefore(int index)
		{
			if (index <= MinWordIndex)
				return null;

			var collection = this.words.TakeWhile(kvp => kvp.Key < index).Reverse().SkipWhile(kvp => kvp.Value.IsEmpty);

			lock (syncLock)
			{
				return collection.Any() ? collection.First().Key : null;
			}
		}

		public int? FirstAddressWithContentsAfter(int index)
		{
			if (index >= MaxWordIndex)
				return null;

			var collection = this.words.SkipWhile(kvp => kvp.Key <= index || kvp.Value.IsEmpty);

			lock (syncLock)
			{
				return collection.Any() ? collection.First().Key : null;
			}
		}

		public void ResetRealWord(int index)
		{
			lock (syncLock)
			{
				this.words.Remove(index);
			}
		}

		public void ClearRealWordSourceLine(int index)
		{
			lock (syncLock)
			{
				if (this.words.TryGetValue(index, out var value))
					value.SourceLine = null;
			}
		}
	}
}
