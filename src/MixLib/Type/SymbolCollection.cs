using System;
using System.Collections;
using System.Collections.Generic;

namespace MixLib.Type
{
	public sealed class SymbolCollection : IEnumerable<SymbolBase>, IEnumerable
	{
		private readonly SortedList<string, SymbolBase> _list = new();

		public int Count 
			=> _list.Count;

		public SymbolBase this[string name] 
			=> _list.ContainsKey(name) ? _list[name] : null;

		public bool Contains(SymbolBase value) 
			=> Contains(value.Name);

		public bool Contains(string value) 
			=> _list.ContainsKey(value);

		private IEnumerator<SymbolBase> GetEnumerator() 
			=> _list.Values.GetEnumerator();

		public void Remove(SymbolBase value) 
			=> Remove(value.Name);

		public void Remove(string name) 
			=> _list.Remove(name);

		IEnumerator<SymbolBase> IEnumerable<SymbolBase>.GetEnumerator() 
			=> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() 
			=> GetEnumerator();

		public void Add(SymbolBase value)
		{
			if (Contains(value))
				throw new ArgumentException("symbol already exists", nameof(value));

			_list.Add(value.Name, value);
		}
	}
}
