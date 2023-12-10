using System;
using System.Windows.Forms;
using MixAssembler.Symbol;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public partial class SymbolListView : UserControl
	{
		private SymbolCollection symbols;

		private const int NameFieldIndex = 0;
		private const int ValueFieldIndex = 1;

		public int MemoryMinIndex { get; set; } = 0;
		public int MemoryMaxIndex { get; set; } = 0;

		public event AddressSelectedHandler AddressSelected;

		public SymbolListView()
		{
			this.symbols = null;

			InitializeComponent();

			this.symbolValueTextBox.MinValue = ValueSymbol.MinValue;
			this.symbolValueTextBox.MaxValue = ValueSymbol.MaxValue;
			this.symbolNameTextBox.MaxLength = ValueSymbol.MaxNameLength;

			this.symbolNameTextBox.TextChanged += SymbolNameTextBox_TextChanged;
			this.symbolValueTextBox.TextChanged += SymbolValueTextBox_TextChanged;
			this.listView.SelectedIndexChanged += ListView_SelectedIndexChanged;
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
			string symbolName = this.symbolNameTextBox.Text;
			this.setButton.Enabled = ValueSymbol.IsValueSymbolName(symbolName) && this.symbolValueTextBox.TextLength > 0;
			this.unsetButton.Enabled = this.symbols != null && this.symbols.Contains(symbolName);

			if (!updateSelectedItem)
				return;

			if (this.unsetButton.Enabled)
			{
				ListViewItem symbolItem = this.listView.Items[symbolName];
				symbolItem.Selected = true;
				symbolItem.EnsureVisible();
			}
			else
			{
				foreach (ListViewItem item in this.listView.Items)
					item.Selected = false;
			}
		}

		public SymbolCollection Symbols
		{
			get => this.symbols;

			set
			{
				if (this.symbols == value)
					return;

				this.symbols = value;

				this.listView.BeginUpdate();
				this.listView.Items.Clear();
				this.symbolNameTextBox.Text = string.Empty;
				this.symbolValueTextBox.Text = string.Empty;

				if (this.symbols != null)
				{
					SuspendLayout();

					foreach (SymbolBase symbol in this.symbols)
						ShowSymbol(symbol);

					ResumeLayout();
				}

				this.listView.EndUpdate();
			}
		}

		public void AddSymbol(SymbolBase symbol)
		{
			this.symbols ??= [];
			this.symbols.Add(symbol);

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

				this.listView.Items.Add(viewItem);
			}
		}

		public void Reset()
		{
			this.symbols = null;

			SuspendLayout();

			this.listView.Items.Clear();

			ResumeLayout();
		}

		public long SelectedValue
		{
			get
			{
				ListView.SelectedListViewItemCollection selectedItems = this.listView.SelectedItems;

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
			this.symbols ??= [];

			string symbolName = this.symbolNameTextBox.Text;
			long symbolMagnitude = this.symbolValueTextBox.Magnitude;
			Word.Signs symbolSign = this.symbolValueTextBox.Sign;
			SymbolBase symbol = this.symbols[symbolName];

			if (symbol != null)
			{
				if (symbol is not ValueSymbol valueSymbol)
					return;

				valueSymbol.SetValue(symbolSign, symbolMagnitude);
				var valueText = symbolMagnitude.ToString();

				if (symbolSign.IsNegative())
					valueText = '-' + valueText;

				this.listView.Items[symbolName].SubItems[ValueFieldIndex].Text = valueText;
			}
			else
			{
				if (ValueSymbol.ParseDefinition(symbolName) is not ValueSymbol valueSymbol)
					return;

				valueSymbol.SetValue(symbolSign, symbolMagnitude);

				this.symbols.Add(valueSymbol);
				ShowSymbol(valueSymbol);
			}

			SetEnabledStates();
		}

		private void UnsetButton_Click(object sender, EventArgs e)
		{
			if (this.symbols == null)
				return;

			string symbolName = this.symbolNameTextBox.Text;
			this.symbols.Remove(symbolName);
			this.listView.Items.RemoveByKey(symbolName);

			SetEnabledStates();
		}

		private void ListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection selectedItems = this.listView.SelectedItems;

			if (selectedItems.Count == 0)
				return;

			ListViewItem selectedItem = selectedItems[0];
			this.symbolNameTextBox.Text = selectedItem.SubItems[NameFieldIndex].Text;
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

			this.symbolValueTextBox.Magnitude = magnitude;
			this.symbolValueTextBox.Sign = sign;
			SetEnabledStates(false);
		}
	}
}
