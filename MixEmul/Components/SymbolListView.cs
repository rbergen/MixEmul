using System;
using System.Windows.Forms;
using MixAssembler.Symbol;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public partial class SymbolListView : UserControl
	{
		private SymbolCollection _symbols;

		private const int NameFieldIndex = 0;
		private const int ValueFieldIndex = 1;

		public int MemoryMinIndex { get; set; } = 0;
		public int MemoryMaxIndex { get; set; } = 0;

		public event AddressSelectedHandler AddressSelected;

		public SymbolListView()
		{
			_symbols = null;

			InitializeComponent();

			_symbolValueTextBox.MinValue = ValueSymbol.MinValue;
			_symbolValueTextBox.MaxValue = ValueSymbol.MaxValue;
			_symbolNameTextBox.MaxLength = ValueSymbol.MaxNameLength;

			_symbolNameTextBox.TextChanged += SymbolNameTextBox_TextChanged;
			_symbolValueTextBox.TextChanged += SymbolValueTextBox_TextChanged;
			_listView.SelectedIndexChanged += ListView_SelectedIndexChanged;
		}

		private void SymbolValueTextBox_TextChanged(object sender, EventArgs e) 
			=> SetEnabledStates();

		private void SymbolNameTextBox_TextChanged(object sender, EventArgs e) 
			=> SetEnabledStates();

		private void SetEnabledStates() 
			=> SetEnabledStates(true);

		private void ListView_DoubleClick(object sender, EventArgs e) 
			=> ValueSelected();

		protected virtual void OnAddressSelected(AddressSelectedEventArgs args) 
			=> AddressSelected?.Invoke(this, args);

		private void SetEnabledStates(bool updateSelectedItem)
		{
			string symbolName = _symbolNameTextBox.Text;
			_setButton.Enabled = ValueSymbol.IsValueSymbolName(symbolName) && _symbolValueTextBox.TextLength > 0;
			_unsetButton.Enabled = _symbols != null && _symbols.Contains(symbolName);

			if (!updateSelectedItem)
				return;

			if (_unsetButton.Enabled)
			{
				ListViewItem symbolItem = _listView.Items[symbolName];
				symbolItem.Selected = true;
				symbolItem.EnsureVisible();
			}
			else
			{
				foreach (ListViewItem item in _listView.Items)
					item.Selected = false;
			}
		}

		public SymbolCollection Symbols
		{
			get => _symbols;

			set
			{
				if (_symbols == value)
					return;

				_symbols = value;

				_listView.BeginUpdate();
				_listView.Items.Clear();
				_symbolNameTextBox.Text = "";
				_symbolValueTextBox.Text = "";

				if (_symbols != null)
				{
					SuspendLayout();

					foreach (SymbolBase symbol in _symbols)
						ShowSymbol(symbol);

					ResumeLayout();
				}

				_listView.EndUpdate();
			}
		}

		public void AddSymbol(SymbolBase symbol)
		{
			if (_symbols == null)
				_symbols = new SymbolCollection();

			_symbols.Add(symbol);

			ShowSymbol(symbol);
		}

		private void ShowSymbol(SymbolBase symbol)
		{
			if (symbol is ValueSymbol valueSymbol)
			{
				var valueText = valueSymbol.Magnitude.ToString();
				if (valueSymbol.Sign.IsNegative())
					valueText = '-' + valueText;

				var viewItem = new ListViewItem(new string[] { valueSymbol.Name, valueText })
				{
					Name = valueSymbol.Name
				};

				_listView.Items.Add(viewItem);
			}
		}

		public void Reset()
		{
			_symbols = null;

			SuspendLayout();

			_listView.Items.Clear();

			ResumeLayout();
		}

		public long SelectedValue
		{
			get
			{
				ListView.SelectedListViewItemCollection selectedItems = _listView.SelectedItems;

				if (selectedItems.Count == 0)
					return long.MinValue;

				long value = long.MinValue;
				try
				{
					value = long.Parse(selectedItems[0].SubItems[ValueFieldIndex].Text);
				}
				catch (FormatException) { }

				return value;
			}
		}

		private void ValueSelected()
		{
			long selectedValue = SelectedValue;
			if (selectedValue >= MemoryMinIndex && selectedValue <= MemoryMaxIndex)
				OnAddressSelected(new AddressSelectedEventArgs((int)selectedValue));
		}

		private void ListView_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				e.Handled = true;
				ValueSelected();
			}
		}

		private void SetButton_Click(object sender, EventArgs e)
		{
			if (_symbols == null)
				_symbols = new SymbolCollection();

			string symbolName = _symbolNameTextBox.Text;
			long symbolMagnitude = _symbolValueTextBox.Magnitude;
			Word.Signs symbolSign = _symbolValueTextBox.Sign;
			SymbolBase symbol = _symbols[symbolName];

			if (symbol != null)
			{
				if (symbol is not ValueSymbol valueSymbol)
					return;

				valueSymbol.SetValue(symbolSign, symbolMagnitude);
				var valueText = symbolMagnitude.ToString();

				if (symbolSign.IsNegative())
					valueText = '-' + valueText;

				_listView.Items[symbolName].SubItems[ValueFieldIndex].Text = valueText;
			}
			else
			{
				if (ValueSymbol.ParseDefinition(symbolName) is not ValueSymbol valueSymbol)
					return;

				valueSymbol.SetValue(symbolSign, symbolMagnitude);

				_symbols.Add(valueSymbol);
				ShowSymbol(valueSymbol);
			}

			SetEnabledStates();
		}

		private void UnsetButton_Click(object sender, EventArgs e)
		{
			if (_symbols == null)
				return;

			string symbolName = _symbolNameTextBox.Text;
			_symbols.Remove(symbolName);
			_listView.Items.RemoveByKey(symbolName);

			SetEnabledStates();
		}

		private void ListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection selectedItems = _listView.SelectedItems;

			if (selectedItems.Count == 0)
				return;

			ListViewItem selectedItem = selectedItems[0];
			_symbolNameTextBox.Text = selectedItem.SubItems[NameFieldIndex].Text;
			long magnitude = 0;
			Word.Signs sign = Word.Signs.Positive;
			try
			{
				string valueText = selectedItem.SubItems[ValueFieldIndex].Text;

				if (valueText[0] == '-')
				{
					sign = Word.Signs.Negative;
					valueText = valueText[1..];
				}

				magnitude = long.Parse(valueText);
			}
			catch (FormatException) { }

			_symbolValueTextBox.Magnitude = magnitude;
			_symbolValueTextBox.Sign = sign;
			SetEnabledStates(false);
		}
	}
}
