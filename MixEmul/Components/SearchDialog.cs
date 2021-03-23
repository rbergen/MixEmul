using MixGui.Events;
using MixLib.Type;
using System;
using System.Windows.Forms;

namespace MixGui.Components
{
	public partial class SearchDialog : Form
	{
		SearchParameters mSearchParameters;

		public SearchDialog()
		{
			InitializeComponent();
			mSearchTextBox.MixByteCollectionValue = new MixByteCollection(80);
		}

		void FieldCheckedChanged(object sender, EventArgs e)
		{
			SetFindButtonEnabledState();
		}

		void MSearchTextBox_ValueChanged(IMixByteCollectionEditor sender, MixByteCollectionEditorValueChangedEventArgs args)
		{
			SetFindButtonEnabledState();
		}

		public SearchParameters SearchParameters
		{
			get => mSearchParameters;

			set
			{
				if (value != null)
				{
					mSearchParameters = value;

					FillControls();
				}
			}
		}

		void SetFindButtonEnabledState()
		{
			mFindButton.Enabled = mSearchTextBox.MixByteCollectionValue.ToString(true).Trim() != string.Empty && (mValueCheckBox.Checked || mCharsCheckBox.Checked || mInstructionCheckBox.Checked);
		}

		void MFindButton_Click(object sender, EventArgs e)
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

		void SearchDialog_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				FillControls();
			}
		}

		void FillControls()
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
