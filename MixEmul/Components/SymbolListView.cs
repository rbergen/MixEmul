using System;
using System.Windows.Forms;
using MixAssembler.Symbol;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public partial class SymbolListView : UserControl
	{
		private SymbolCollection mSymbols;
		private int mMemoryMinIndex = 0;
		private int mMemoryMaxIndex = 0;

		private const int nameFieldIndex = 0;
		private const int valueFieldIndex = 1;

		public event AddressSelectedHandler AddressSelected;

		public SymbolListView()
		{
			mSymbols = null;

			InitializeComponent();

			mSymbolValueTextBox.MinValue = ValueSymbol.MinValue;
			mSymbolValueTextBox.MaxValue = ValueSymbol.MaxValue;
			mSymbolNameTextBox.MaxLength = ValueSymbol.MaxNameLength;

			mSymbolNameTextBox.TextChanged += new EventHandler(mSymbolNameTextBox_TextChanged);
			mSymbolValueTextBox.TextChanged += new EventHandler(mSymbolValueTextBox_TextChanged);
			mListView.SelectedIndexChanged += new EventHandler(mListView_SelectedIndexChanged);
		}

		void mSymbolValueTextBox_TextChanged(object sender, EventArgs e)
		{
			setEnabledStates();
		}

		void mSymbolNameTextBox_TextChanged(object sender, EventArgs e)
		{
			setEnabledStates();
		}

		private void setEnabledStates()
		{
			setEnabledStates(true);
		}

		private void setEnabledStates(bool updateSelectedItem)
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
				if (mSymbols == value)
				{
					return;
				}

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
						showSymbol(symbol);
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

			showSymbol(symbol);
		}

		private void showSymbol(SymbolBase symbol)
		{
			ValueSymbol valueSymbol = symbol as ValueSymbol;

			if (valueSymbol != null)
			{
				string valueText = valueSymbol.Magnitude.ToString();
				if (valueSymbol.Sign.IsNegative())
				{
					valueText = '-' + valueText;
				}

				ListViewItem viewItem = new ListViewItem(new string[] { valueSymbol.Name, valueText });
				viewItem.Name = valueSymbol.Name;
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

				if (selectedItems.Count == 0)
				{
					return long.MinValue;
				}

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


		private void mListView_DoubleClick(object sender, EventArgs e)
		{
			valueSelected();
		}

		public int MemoryMinIndex
		{
			get
			{
				return mMemoryMinIndex;
			}
			set
			{
				mMemoryMinIndex = value;
			}
		}

		public int MemoryMaxIndex
		{
			get
			{
				return mMemoryMaxIndex;
			}
			set
			{
				mMemoryMaxIndex = value;
			}
		}

		private void valueSelected()
		{
			long selectedValue = SelectedValue;
			if (selectedValue >= mMemoryMinIndex && selectedValue <= mMemoryMaxIndex)
			{
				OnAddressSelected(new AddressSelectedEventArgs((int)selectedValue));
			}
		}

		protected virtual void OnAddressSelected(AddressSelectedEventArgs args)
		{
			if (AddressSelected != null)
			{
				AddressSelected(this, args);
			}
		}

		private void mListView_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				e.Handled = true;
				valueSelected();
			}
		}

		private void mSetButton_Click(object sender, EventArgs e)
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
				if (valueSymbol == null)
				{
					return;
				}

				valueSymbol.SetValue(symbolSign, symbolMagnitude);
				string valueText = symbolMagnitude.ToString();
				if (symbolSign.IsNegative())
				{
					valueText = '-' + valueText;
				}

				mListView.Items[symbolName].SubItems[valueFieldIndex].Text = valueText;
			}
			else
			{
				valueSymbol = ValueSymbol.ParseDefinition(symbolName) as ValueSymbol;

				if (valueSymbol == null)
				{
					return;
				}

				valueSymbol.SetValue(symbolSign, symbolMagnitude);

				mSymbols.Add(valueSymbol);
				showSymbol(valueSymbol);
			}

			setEnabledStates();
		}

		private void mUnsetButton_Click(object sender, EventArgs e)
		{
			if (mSymbols == null)
			{
				return;
			}

			string symbolName = mSymbolNameTextBox.Text;
			mSymbols.Remove(symbolName);
			mListView.Items.RemoveByKey(symbolName);

			setEnabledStates();
		}

		private void mListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection selectedItems = mListView.SelectedItems;

			if (selectedItems.Count == 0)
			{
				return;
			}

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
			setEnabledStates(false);
		}
	}
}
