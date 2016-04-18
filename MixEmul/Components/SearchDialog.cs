using System;
using System.Windows.Forms;
using MixGui.Events;
using MixLib.Type;

namespace MixGui.Components
{
	public partial class SearchDialog : Form
	{
		private SearchParameters mSearchParameters = null;

		public SearchDialog()
		{
			InitializeComponent();
			mSearchTextBox.MixByteCollectionValue = new MixByteCollection(80);
		}

        private void fieldCheckedChanged(object sender, EventArgs e) => setFindButtonEnabledState();

        private void mSearchTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args) => setFindButtonEnabledState();

        public SearchParameters SearchParameters
		{
			get
			{
				return mSearchParameters;
			}

			set
			{
				if (value != null)
				{
					mSearchParameters = value;

					fillControls();
				}
			}
		}

		private void setFindButtonEnabledState()
		{
			mFindButton.Enabled = mSearchTextBox.MixByteCollectionValue.ToString(true).Trim() != string.Empty && (mValueCheckBox.Checked || mCharsCheckBox.Checked || mInstructionCheckBox.Checked);
		}

		private void mFindButton_Click(object sender, EventArgs e)
		{
			if (mSearchParameters == null)
			{
				mSearchParameters = new SearchParameters();
			}

			mSearchParameters.SearchText = mSearchTextBox.MixByteCollectionValue.ToString(true).Trim();
			mSearchParameters.SearchFields = FieldTypes.None;
			if (mValueCheckBox.Checked)
			{
				mSearchParameters.SearchFields |= FieldTypes.Value;
			}
			if (mCharsCheckBox.Checked)
			{
				mSearchParameters.SearchFields |= FieldTypes.Chars;
			}
			if (mInstructionCheckBox.Checked)
			{
				mSearchParameters.SearchFields |= FieldTypes.Instruction;
			}
			mSearchParameters.MatchWholeWord = mMatchWholeWordCheckBox.Checked;
			mSearchParameters.WrapSearch = mWrapSearchCheckBox.Checked;
		}

		private void SearchDialog_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				fillControls();
			}
		}

		private void fillControls()
		{
			if (mSearchParameters != null)
			{
				mSearchTextBox.MixByteCollectionValue.Load(mSearchParameters.SearchText);
				mValueCheckBox.Checked = (mSearchParameters.SearchFields & FieldTypes.Value) == FieldTypes.Value;
				mCharsCheckBox.Checked = (mSearchParameters.SearchFields & FieldTypes.Chars) == FieldTypes.Chars;
				mInstructionCheckBox.Checked = (mSearchParameters.SearchFields & FieldTypes.Instruction) == FieldTypes.Instruction;
				mMatchWholeWordCheckBox.Checked = mSearchParameters.MatchWholeWord;
				mWrapSearchCheckBox.Checked = mSearchParameters.WrapSearch;
			}
			else
			{
				mSearchTextBox.Text = "";
				mValueCheckBox.Checked = true;
				mCharsCheckBox.Checked = true;
				mInstructionCheckBox.Checked = true;
				mMatchWholeWordCheckBox.Checked = false;
				mWrapSearchCheckBox.Checked = true;
			}
		}
	}
}
