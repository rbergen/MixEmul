using System;
using System.Collections;
using System.Collections.Generic;

namespace MixLib.Type
{
	public sealed class SymbolCollection : IEnumerable<SymbolBase>, IEnumerable
	{
        readonly SortedList<string, SymbolBase> mList = new SortedList<string, SymbolBase>();

        public int Count => mList.Count;

        public bool Contains(SymbolBase value) => Contains(value.Name);

        public bool Contains(string value) => mList.ContainsKey(value);

        IEnumerator<SymbolBase> getEnumerator() => mList.Values.GetEnumerator();

        public void Remove(SymbolBase value) => Remove(value.Name);

        public void Remove(string name) => mList.Remove(name);

        IEnumerator<SymbolBase> IEnumerable<SymbolBase>.GetEnumerator() => getEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => getEnumerator();

        public void Add(SymbolBase value)
		{
			if (Contains(value))
			{
				throw new ArgumentException("symbol already exists", nameof(value));
			}

			mList.Add(value.Name, value);
		}

		public SymbolBase this[string name]
		{
			get
			{
				if (!mList.ContainsKey(name))
				{
					return null;
				}

				return mList[name];
			}
		}
	}
}
