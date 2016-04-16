namespace MixLib.Type
{

	public abstract class SymbolBase : IValue
	{
        public string Name { get; private set; }

        protected SymbolBase(string name)
		{
			Name = name;
		}

		public abstract long GetValue(int currentAddress);

		public abstract bool IsValueDefined(int currentAddress);

		public abstract void SetValue(long value);

		public abstract void SetValue(Word.Signs sign, long magnitude);

		public virtual bool IsMultiValuedSymbol
		{
			get
			{
				return false;
			}
		}

		public abstract bool IsSymbolDefined
		{
			get;
		}

		public virtual long MemoryWordValue
		{
			get
			{
				return 0L;
			}
		}

		public virtual long MemoryWordMagnitude
		{
			get
			{
				return 0L;
			}
		}

		public virtual Word.Signs MemoryWordSign
		{
			get
			{
				return Word.Signs.Positive;
			}
		}

		#region IValue Members

		public abstract long GetMagnitude(int currentAddress);
		public abstract Word.Signs GetSign(int currentAddress);

		#endregion
	}
}
