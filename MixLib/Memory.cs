using MixLib.Type;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MixLib
{
	public class Memory : IMemory, IEnumerable
	{
		public static readonly FieldSpec DefaultFieldSpec = new(0, 5);
		private readonly SortedDictionary<int, MemoryFullWord> mWords;
		private readonly object mSyncRoot;

		public int MinWordIndex { get; set; }
		public int MaxWordIndex { get; set; }

		public Memory(int minIndex, int maxIndex)
		{
			MinWordIndex = minIndex;
			MaxWordIndex = maxIndex;
			mWords = new SortedDictionary<int, MemoryFullWord>();
			mSyncRoot = ((ICollection)mWords).SyncRoot;
		}

		public int WordCount => MaxWordIndex - MinWordIndex + 1;

		public void ResetProfilingCounts()
		{
			lock (mSyncRoot)
			{
				foreach (MemoryFullWord word in mWords.Values)
				{
					word.ResetProfilingCounts();
				}
			}
		}

		public long MaxProfilingTickCount
		{
			get
			{
				lock (mSyncRoot)
				{
					return mWords.Count == 0 ? 0 : mWords.Values.Select(w => w.ProfilingTickCount).Max();
				}
			}
		}

		public long MaxProfilingExecutionCount
		{
			get
			{
				lock (mSyncRoot)
				{
					return mWords.Count == 0 ? 0 : mWords.Values.Select(w => w.ProfilingExecutionCount).Max();
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
				lock (mSyncRoot)
				{
					mWords.TryGetValue(index, out word);
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

				lock (mSyncRoot)
				{
					pair = mWords.Cast<KeyValuePair<int, MemoryFullWord>?>().FirstOrDefault(kvp => kvp.Value.Key > index);
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
			lock (mSyncRoot)
			{
				return mWords.Values.GetEnumerator();
			}
		}

		public void ClearSourceLines()
		{
			lock (mSyncRoot)
			{
				foreach (MemoryFullWord word in mWords.Values)
				{
					word.SourceLine = null;
				}
			}
		}

		public void Reset()
		{
			lock (mSyncRoot)
			{
				mWords.Clear();
			}
		}

		public IMemoryFullWord this[int index]
		{
			get
			{
				if (index < MinWordIndex || index > MaxWordIndex)
				{
					throw new ArgumentOutOfRangeException(nameof(index), string.Format("value must be between MinWordIndex ({0}) and MaxWordIndex ({1}), inclusive", MinWordIndex, MaxWordIndex));
				}

				MemoryFullWord word;

				lock (mSyncRoot)
				{
					mWords.TryGetValue(index, out word);
				}

				return (IMemoryFullWord)word ?? new VirtualMemoryFullWord(this, index);
			}
			set
			{
				if (index < MinWordIndex || index > MaxWordIndex)
				{
					throw new IndexOutOfRangeException(string.Format("index must be between MinWordIndex ({0}) and MaxWordIndex ({1}), inclusive", MinWordIndex, MaxWordIndex));
				}

				var word = GetRealWord(index);

				for (int byteIndex = 0; byteIndex < value.ByteCount; byteIndex++)
				{
					word[byteIndex] = value[byteIndex];
				}

				word.Sign = value.Sign;
			}
		}

		public MemoryFullWord GetRealWord(int index)
		{
			MemoryFullWord word;

			lock (mSyncRoot)
			{
				if (!mWords.TryGetValue(index, out word))
				{
					word = new MemoryFullWord(index, 0);
					mWords[index] = word;
				}
			}

			return word;
		}

		public bool HasContents(int index)
		{
			lock (mSyncRoot)
			{
				return mWords.ContainsKey(index) && !mWords[index].IsEmpty;
			}
		}

		public int? LastAddressWithContentsBefore(int index)
		{
			if (index <= MinWordIndex)
			{
				return null;
			}

			var collection = mWords.TakeWhile(kvp => kvp.Key < index).Reverse().SkipWhile(kvp => kvp.Value.IsEmpty);

			lock (mSyncRoot)
			{
				return collection.Any() ? collection.First().Key : null;
			}
		}

		public int? FirstAddressWithContentsAfter(int index)
		{
			if (index >= MaxWordIndex)
			{
				return null;
			}

			var collection = mWords.SkipWhile(kvp => kvp.Key <= index || kvp.Value.IsEmpty);

			lock (mSyncRoot)
			{
				return collection.Any() ? collection.First().Key : null;
			}
		}

		public void ResetRealWord(int index)
		{
			lock (mSyncRoot)
			{
				mWords.Remove(index);
			}
		}

		public void ClearRealWordSourceLine(int index)
		{
			lock (mSyncRoot)
			{
				if (mWords.ContainsKey(index))
				{
					mWords[index].SourceLine = null;
				}
			}
		}
	}
}
