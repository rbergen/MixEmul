using System;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public partial class SearchDialog : Form
	{
		private SearchParameters _searchParameters;

		public SearchDialog()
		{
			InitializeComponent();
			_searchTextBox.MixByteCollectionValue = new MixByteCollection(80);
		}

		private void FieldCheckedChanged(object sender, EventArgs e)
			=> SetFindButtonEnabledState();

		private void SearchTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args) 
			=> SetFindButtonEnabledState();

		public SearchParameters SearchParameters
		{
			get => _searchParameters;

			set
			{
				if (value != null)
				{
					_searchParameters = value;

					FillControls();
				}
			}
		}

		private void SetFindButtonEnabledState() 
			=> _findButton.Enabled = _searchTextBox.MixByteCollectionValue.ToString(true).Trim() != string.Empty && (_valueCheckBox.Checked || _charsCheckBox.Checked || _instructionCheckBox.Checked);

		private void FindButton_Click(object sender, EventArgs e)
		{
			if (_searchParameters == null)
				_searchParameters = new SearchParameters();

			_searchParameters.SearchText = _searchTextBox.MixByteCollectionValue.ToString(true).Trim();
			_searchParameters.SearchFields = FieldTypes.None;

			if (_valueCheckBox.Checked)
				_searchParameters.SearchFields |= FieldTypes.Value;

			if (_charsCheckBox.Checked)
				_searchParameters.SearchFields |= FieldTypes.Chars;

			if (_instructionCheckBox.Checked)
				_searchParameters.SearchFields |= FieldTypes.Instruction;

			_searchParameters.MatchWholeWord = _matchWholeWordCheckBox.Checked;
			_searchParameters.WrapSearch = _wrapSearchCheckBox.Checked;
		}

		private void SearchDialog_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
				FillControls();
		}

		private void FillControls()
		{
			if (_searchParameters != null)
			{
				_searchTextBox.MixByteCollectionValue.Load(_searchParameters.SearchText);
				_valueCheckBox.Checked = (_searchParameters.SearchFields & FieldTypes.Value) == FieldTypes.Value;
				_charsCheckBox.Checked = (_searchParameters.SearchFields & FieldTypes.Chars) == FieldTypes.Chars;
				_instructionCheckBox.Checked = (_searchParameters.SearchFields & FieldTypes.Instruction) == FieldTypes.Instruction;
				_matchWholeWordCheckBox.Checked = _searchParameters.MatchWholeWord;
				_wrapSearchCheckBox.Checked = _searchParameters.WrapSearch;
			}
			else
			{
				_searchTextBox.Text = "";
				_valueCheckBox.Checked = true;
				_charsCheckBox.Checked = true;
				_instructionCheckBox.Checked = true;
				_matchWholeWordCheckBox.Checked = false;
				_wrapSearchCheckBox.Checked = true;
			}
		}
	}
}
