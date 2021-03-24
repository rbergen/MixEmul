using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MixLib.Type;

namespace MixLib
{
	public class Memory : IMemory, IEnumerable
	{
		public static readonly FieldSpec DefaultFieldSpec = new(0, 5);

		private readonly SortedDictionary<int, MemoryFullWord> _words;
		private readonly object _syncRoot;

		public int MinWordIndex { get; set; }
		public int MaxWordIndex { get; set; }

		public Memory(int minIndex, int maxIndex)
		{
			MinWordIndex = minIndex;
			MaxWordIndex = maxIndex;
			_words = new SortedDictionary<int, MemoryFullWord>();
			_syncRoot = ((ICollection)_words).SyncRoot;
		}

		public int WordCount 
			=> MaxWordIndex - MinWordIndex + 1;

		public void ResetProfilingCounts()
		{
			lock (_syncRoot)
			{
				foreach (MemoryFullWord word in _words.Values)
					word.ResetProfilingCounts();
			}
		}

		public long MaxProfilingTickCount
		{
			get
			{
				lock (_syncRoot)
				{
					return _words.Count == 0 ? 0 : _words.Values.Select(w => w.ProfilingTickCount).Max();
				}
			}
		}

		public long MaxProfilingExecutionCount
		{
			get
			{
				lock (_syncRoot)
				{
					return _words.Count == 0 ? 0 : _words.Values.Select(w => w.ProfilingExecutionCount).Max();
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
				lock (_syncRoot)
				{
					_words.TryGetValue(index, out word);
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

				lock (_syncRoot)
				{
					pair = _words.Cast<KeyValuePair<int, MemoryFullWord>?>().FirstOrDefault(kvp => kvp.Value.Key > index);
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
			lock (_syncRoot)
			{
				return _words.Values.GetEnumerator();
			}
		}

		public void ClearSourceLines()
		{
			lock (_syncRoot)
			{
				foreach (MemoryFullWord word in _words.Values)
					word.SourceLine = null;
			}
		}

		public void Reset()
		{
			lock (_syncRoot)
			{
				_words.Clear();
			}
		}

		public IMemoryFullWord this[int index]
		{
			get
			{
				if (index < MinWordIndex || index > MaxWordIndex)
					throw new ArgumentOutOfRangeException(nameof(index), string.Format("value must be between MinWordIndex ({0}) and MaxWordIndex ({1}), inclusive", MinWordIndex, MaxWordIndex));

				MemoryFullWord word;

				lock (_syncRoot)
				{
					_words.TryGetValue(index, out word);
				}

				return (IMemoryFullWord)word ?? new VirtualMemoryFullWord(this, index);
			}
			set
			{
				if (index < MinWordIndex || index > MaxWordIndex)
					throw new IndexOutOfRangeException(string.Format("index must be between MinWordIndex ({0}) and MaxWordIndex ({1}), inclusive", MinWordIndex, MaxWordIndex));

				var word = GetRealWord(index);

				for (int byteIndex = 0; byteIndex < value.ByteCount; byteIndex++)
					word[byteIndex] = value[byteIndex];

				word.Sign = value.Sign;
			}
		}

		public MemoryFullWord GetRealWord(int index)
		{
			MemoryFullWord word;

			lock (_syncRoot)
			{
				if (!_words.TryGetValue(index, out word))
				{
					word = new MemoryFullWord(index, 0);
					_words[index] = word;
				}
			}

			return word;
		}

		public bool HasContents(int index)
		{
			lock (_syncRoot)
			{
				return _words.ContainsKey(index) && !_words[index].IsEmpty;
			}
		}

		public int? LastAddressWithContentsBefore(int index)
		{
			if (index <= MinWordIndex)
				return null;

			var collection = _words.TakeWhile(kvp => kvp.Key < index).Reverse().SkipWhile(kvp => kvp.Value.IsEmpty);

			lock (_syncRoot)
			{
				return collection.Any() ? collection.First().Key : null;
			}
		}

		public int? FirstAddressWithContentsAfter(int index)
		{
			if (index >= MaxWordIndex)
				return null;

			var collection = _words.SkipWhile(kvp => kvp.Key <= index || kvp.Value.IsEmpty);

			lock (_syncRoot)
			{
				return collection.Any() ? collection.First().Key : null;
			}
		}

		public void ResetRealWord(int index)
		{
			lock (_syncRoot)
			{
				_words.Remove(index);
			}
		}

		public void ClearRealWordSourceLine(int index)
		{
			lock (_syncRoot)
			{
				if (_words.ContainsKey(index))
					_words[index].SourceLine = null;
			}
		}
	}
}
