namespace MixLib.Type
{

	public abstract class SymbolBase : IValue
	{
		public string Name { get; private set; }

		protected SymbolBase(string name) 
			=> Name = name;

		public virtual long MemoryWordValue 
			=> 0L;

		public virtual long MemoryWordMagnitude 
			=> 0L;

		public virtual Word.Signs MemoryWordSign 
			=> Word.Signs.Positive;

		public virtual bool IsMultiValuedSymbol 
			=> false;

		public abstract long GetValue(int currentAddress);

		public abstract bool IsValueDefined(int currentAddress);

		public abstract void SetValue(long value);

		public abstract void SetValue(Word.Signs sign, long magnitude);

		public abstract bool IsSymbolDefined { get; }

		#region IValue Members

		public abstract long GetMagnitude(int currentAddress);
		public abstract Word.Signs GetSign(int currentAddress);

		#endregion
	}
}
