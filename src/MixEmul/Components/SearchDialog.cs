using System;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public partial class SearchDialog : Form
	{
		private SearchParameters searchParameters;

		public SearchDialog()
		{
			InitializeComponent();
			this.searchTextBox.MixByteCollectionValue = new MixByteCollection(80);
		}

		private void FieldCheckedChanged(object sender, EventArgs e)
			=> SetFindButtonEnabledState();

		private void SearchTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args) 
			=> SetFindButtonEnabledState();

		public SearchParameters SearchParameters
		{
			get => this.searchParameters;

			set
			{
				if (value != null)
				{
					this.searchParameters = value;

					FillControls();
				}
			}
		}

		private void SetFindButtonEnabledState() 
			=> this.findButton.Enabled = this.searchTextBox.MixByteCollectionValue.ToString(true).Trim() != string.Empty && (this.valueCheckBox.Checked || this.charsCheckBox.Checked || this.instructionCheckBox.Checked);

		private void FindButton_Click(object sender, EventArgs e)
		{
			if (this.searchParameters == null)
				this.searchParameters = new SearchParameters();

			this.searchParameters.SearchText = this.searchTextBox.MixByteCollectionValue.ToString(true).Trim();
			this.searchParameters.SearchFields = FieldTypes.None;

			if (this.valueCheckBox.Checked)
				this.searchParameters.SearchFields |= FieldTypes.Value;

			if (this.charsCheckBox.Checked)
				this.searchParameters.SearchFields |= FieldTypes.Chars;

			if (this.instructionCheckBox.Checked)
				this.searchParameters.SearchFields |= FieldTypes.Instruction;

			this.searchParameters.MatchWholeWord = this.matchWholeWordCheckBox.Checked;
			this.searchParameters.WrapSearch = this.wrapSearchCheckBox.Checked;
		}

		private void SearchDialog_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
				FillControls();
		}

		private void FillControls()
		{
			if (this.searchParameters != null)
			{
				this.searchTextBox.MixByteCollectionValue.Load(this.searchParameters.SearchText);
				this.valueCheckBox.Checked = (this.searchParameters.SearchFields & FieldTypes.Value) == FieldTypes.Value;
				this.charsCheckBox.Checked = (this.searchParameters.SearchFields & FieldTypes.Chars) == FieldTypes.Chars;
				this.instructionCheckBox.Checked = (this.searchParameters.SearchFields & FieldTypes.Instruction) == FieldTypes.Instruction;
				this.matchWholeWordCheckBox.Checked = this.searchParameters.MatchWholeWord;
				this.wrapSearchCheckBox.Checked = this.searchParameters.WrapSearch;
			}
			else
			{
				this.searchTextBox.Text = string.Empty;
				this.valueCheckBox.Checked = true;
				this.charsCheckBox.Checked = true;
				this.instructionCheckBox.Checked = true;
				this.matchWholeWordCheckBox.Checked = false;
				this.wrapSearchCheckBox.Checked = true;
			}
		}
	}
}
