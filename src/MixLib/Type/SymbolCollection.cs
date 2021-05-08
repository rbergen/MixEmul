using System;
using System.Collections;
using System.Collections.Generic;

namespace MixLib.Type
{
	public sealed class SymbolCollection : IEnumerable<SymbolBase>, IEnumerable
	{
		private readonly SortedList<string, SymbolBase> list = new();

		public int Count 
			=> this.list.Count;

		public SymbolBase this[string name] 
			=> this.list.ContainsKey(name) ? this.list[name] : null;

		public bool Contains(SymbolBase value) 
			=> Contains(value.Name);

		public bool Contains(string value) 
			=> this.list.ContainsKey(value);

		private IEnumerator<SymbolBase> GetEnumerator() 
			=> this.list.Values.GetEnumerator();

		public void Remove(SymbolBase value) 
			=> Remove(value.Name);

		public void Remove(string name) 
			=> this.list.Remove(name);

		IEnumerator<SymbolBase> IEnumerable<SymbolBase>.GetEnumerator() 
			=> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() 
			=> GetEnumerator();

		public void Add(SymbolBase value)
		{
			if (Contains(value))
				throw new ArgumentException("symbol already exists", nameof(value));

			this.list.Add(value.Name, value);
		}
	}
}
