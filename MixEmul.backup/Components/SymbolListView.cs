using MixAssembler.Symbol;
using MixGui.Events;
using MixLib.Type;
using System;
using System.Windows.Forms;

namespace MixGui.Components
{
	public partial class SymbolListView : UserControl
	{
		SymbolCollection mSymbols;

		const int nameFieldIndex = 0;
		const int valueFieldIndex = 1;

		public int MemoryMinIndex { get; set; } = 0;
		public int MemoryMaxIndex { get; set; } = 0;

		public event AddressSelectedHandler AddressSelected;

		public SymbolListView()
		{
			mSymbols = null;

			InitializeComponent();

			mSymbolValueTextBox.MinValue = ValueSymbol.MinValue;
			mSymbolValueTextBox.MaxValue = ValueSymbol.MaxValue;
			mSymbolNameTextBox.MaxLength = ValueSymbol.MaxNameLength;

			mSymbolNameTextBox.TextChanged += MSymbolNameTextBox_TextChanged;
			mSymbolValueTextBox.TextChanged += MSymbolValueTextBox_TextChanged;
			mListView.SelectedIndexChanged += MListView_SelectedIndexChanged;
		}

		void MSymbolValueTextBox_TextChanged(object sender, EventArgs e) => SetEnabledStates();

		void MSymbolNameTextBox_TextChanged(object sender, EventArgs e) => SetEnabledStates();

		void SetEnabledStates() => SetEnabledStates(true);

		void MListView_DoubleClick(object sender, EventArgs e) => ValueSelected();

		protected virtual void OnAddressSelected(AddressSelectedEventArgs args) => AddressSelected?.Invoke(this, args);

		void SetEnabledStates(bool updateSelectedItem)
		{
			string symbolName = mSymbolNameTextBox.Text;
			mSetButton.Enabled = ValueSymbol.IsValueSymbolName(symbolName) && mSymbolValueTextBox.TextLength > 0;
			mUnsetButton.Enabled = mSymbols != null && mSymbols.Contains(symbolName);

			if (updateSelectedItem)
			{
				if (mUnsetButton.Enabled)
				{
					ListViewItem symbolItem = mListView.Items[symbolName];
					symbolItem.Selected = true;
					symbolItem.EnsureVisible();
				}
				else
				{
					foreach (ListViewItem item in mListView.Items)
					{
						item.Selected = false;
					}
				}
			}
		}

		public SymbolCollection Symbols
		{
			get
			{
				return mSymbols;
			}

			set
			{
				if (mSymbols == value) return;

				mSymbols = value;

				mListView.BeginUpdate();
				mListView.Items.Clear();
				mSymbolNameTextBox.Text = "";
				mSymbolValueTextBox.Text = "";

				if (mSymbols != null)
				{
					SuspendLayout();

					foreach (SymbolBase symbol in mSymbols)
					{
						ShowSymbol(symbol);
					}

					ResumeLayout();
				}

				mListView.EndUpdate();
			}
		}

		public void AddSymbol(SymbolBase symbol)
		{
			if (mSymbols == null)
			{
				mSymbols = new SymbolCollection();
			}

			mSymbols.Add(symbol);

			ShowSymbol(symbol);
		}

		void ShowSymbol(SymbolBase symbol)
		{
			if (symbol is ValueSymbol valueSymbol)
			{
				var valueText = valueSymbol.Magnitude.ToString();
				if (valueSymbol.Sign.IsNegative())
				{
					valueText = '-' + valueText;
				}

				var viewItem = new ListViewItem(new string[] { valueSymbol.Name, valueText })
				{
					Name = valueSymbol.Name
				};
				mListView.Items.Add(viewItem);
			}
		}

		public void Reset()
		{
			mSymbols = null;

			SuspendLayout();

			mListView.Items.Clear();

			ResumeLayout();
		}

		public long SelectedValue
		{
			get
			{
				ListView.SelectedListViewItemCollection selectedItems = mListView.SelectedItems;

				if (selectedItems.Count == 0) return long.MinValue;

				long value = long.MinValue;
				try
				{
					value = long.Parse(selectedItems[0].SubItems[valueFieldIndex].Text);
				}
				catch (FormatException)
				{
				}

				return value;
			}
		}

		void ValueSelected()
		{
			long selectedValue = SelectedValue;
			if (selectedValue >= MemoryMinIndex && selectedValue <= MemoryMaxIndex)
			{
				OnAddressSelected(new AddressSelectedEventArgs((int)selectedValue));
			}
		}

		void MListView_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				e.Handled = true;
				ValueSelected();
			}
		}

		void MSetButton_Click(object sender, EventArgs e)
		{
			if (mSymbols == null)
			{
				mSymbols = new SymbolCollection();
			}

			string symbolName = mSymbolNameTextBox.Text;
			long symbolMagnitude = mSymbolValueTextBox.Magnitude;
			Word.Signs symbolSign = mSymbolValueTextBox.Sign;
			SymbolBase symbol = mSymbols[symbolName];
			ValueSymbol valueSymbol = null;

			if (symbol != null)
			{
				valueSymbol = symbol as ValueSymbol;
				if (valueSymbol == null) return;

				valueSymbol.SetValue(symbolSign, symbolMagnitude);
				var valueText = symbolMagnitude.ToString();
				if (symbolSign.IsNegative())
				{
					valueText = '-' + valueText;
				}

				mListView.Items[symbolName].SubItems[valueFieldIndex].Text = valueText;
			}
			else
			{
				valueSymbol = ValueSymbol.ParseDefinition(symbolName) as ValueSymbol;

				if (valueSymbol == null) return;

				valueSymbol.SetValue(symbolSign, symbolMagnitude);

				mSymbols.Add(valueSymbol);
				ShowSymbol(valueSymbol);
			}

			SetEnabledStates();
		}

		void MUnsetButton_Click(object sender, EventArgs e)
		{
			if (mSymbols == null) return;

			string symbolName = mSymbolNameTextBox.Text;
			mSymbols.Remove(symbolName);
			mListView.Items.RemoveByKey(symbolName);

			SetEnabledStates();
		}

		void MListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection selectedItems = mListView.SelectedItems;

			if (selectedItems.Count == 0) return;

			ListViewItem selectedItem = selectedItems[0];
			mSymbolNameTextBox.Text = selectedItem.SubItems[nameFieldIndex].Text;
			long magnitude = 0;
			Word.Signs sign = Word.Signs.Positive;
			try
			{
				string valueText = selectedItem.SubItems[valueFieldIndex].Text;
				if (valueText[0] == '-')
				{
					sign = Word.Signs.Negative;
					valueText = valueText.Substring(1);
				}
				magnitude = long.Parse(valueText);
			}
			catch (FormatException)
			{
			}

			mSymbolValueTextBox.Magnitude = magnitude;
			mSymbolValueTextBox.Sign = sign;
			SetEnabledStates(false);
		}
	}
}
