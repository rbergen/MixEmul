using System;
using System.Collections;
using System.Collections.Generic;

namespace MixLib.Type
{
	public sealed class SymbolCollection : IEnumerable<SymbolBase>, IEnumerable
	{
		readonly SortedList<string, SymbolBase> mList = new SortedList<string, SymbolBase>();

		public int Count => mList.Count;

		public SymbolBase this[string name] => mList.ContainsKey(name) ? mList[name] : null;

		public bool Contains(SymbolBase value)
		{
			return Contains(value.Name);
		}

		public bool Contains(string value)
		{
			return mList.ContainsKey(value);
		}

		IEnumerator<SymbolBase> GetEnumerator()
		{
			return mList.Values.GetEnumerator();
		}

		public void Remove(SymbolBase value)
		{
			Remove(value.Name);
		}

		public void Remove(string name)
		{
			mList.Remove(name);
		}

		IEnumerator<SymbolBase> IEnumerable<SymbolBase>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(SymbolBase value)
		{
			if (Contains(value))
			{
				throw new ArgumentException("symbol already exists", nameof(value));
			}

			mList.Add(value.Name, value);
		}
	}
}
