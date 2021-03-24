namespace MixGui.Components
{
	partial class SearchDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) components?.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			MixLib.Type.FullWord fullWord1 = new MixLib.Type.FullWord();
			MixLib.Type.MixByte mixByte1 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte2 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte3 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte4 = new MixLib.Type.MixByte();
			MixLib.Type.MixByte mixByte5 = new MixLib.Type.MixByte();
			this._findLabel = new System.Windows.Forms.Label();
			this._valueCheckBox = new System.Windows.Forms.CheckBox();
			this._charsCheckBox = new System.Windows.Forms.CheckBox();
			this._instructionCheckBox = new System.Windows.Forms.CheckBox();
			this._matchWholeWordCheckBox = new System.Windows.Forms.CheckBox();
			this._wrapSearchCheckBox = new System.Windows.Forms.CheckBox();
			this._findButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._fieldsGroupBox = new System.Windows.Forms.GroupBox();
			this._clipboardLabel = new System.Windows.Forms.Label();
			this._clipboardButtons = new MixGui.Components.MixCharClipboardButtonControl();
			this._searchTextBox = new MixGui.Components.MixByteCollectionCharTextBox();
			this._fieldsGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// mFindLabel
			// 
			this._findLabel.AutoSize = true;
			this._findLabel.Location = new System.Drawing.Point(13, 9);
			this._findLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._findLabel.Name = "mFindLabel";
			this._findLabel.Size = new System.Drawing.Size(56, 15);
			this._findLabel.TabIndex = 0;
			this._findLabel.Text = "Find &text:";
			// 
			// mValueCheckBox
			// 
			this._valueCheckBox.AutoSize = true;
			this._valueCheckBox.Checked = true;
			this._valueCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._valueCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._valueCheckBox.Location = new System.Drawing.Point(7, 22);
			this._valueCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this._valueCheckBox.Name = "mValueCheckBox";
			this._valueCheckBox.Size = new System.Drawing.Size(100, 19);
			this._valueCheckBox.TabIndex = 0;
			this._valueCheckBox.Text = "&Numeric value";
			this._valueCheckBox.UseVisualStyleBackColor = true;
			this._valueCheckBox.CheckedChanged += new System.EventHandler(this.FieldCheckedChanged);
			// 
			// mCharsCheckBox
			// 
			this._charsCheckBox.AutoSize = true;
			this._charsCheckBox.Checked = true;
			this._charsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._charsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._charsCheckBox.Location = new System.Drawing.Point(7, 49);
			this._charsCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this._charsCheckBox.Name = "mCharsCheckBox";
			this._charsCheckBox.Size = new System.Drawing.Size(79, 19);
			this._charsCheckBox.TabIndex = 1;
			this._charsCheckBox.Text = "&Characters";
			this._charsCheckBox.UseVisualStyleBackColor = true;
			this._charsCheckBox.CheckedChanged += new System.EventHandler(this.FieldCheckedChanged);
			// 
			// mInstructionCheckBox
			// 
			this._instructionCheckBox.AutoSize = true;
			this._instructionCheckBox.Checked = true;
			this._instructionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._instructionCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._instructionCheckBox.Location = new System.Drawing.Point(7, 75);
			this._instructionCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this._instructionCheckBox.Name = "mInstructionCheckBox";
			this._instructionCheckBox.Size = new System.Drawing.Size(80, 19);
			this._instructionCheckBox.TabIndex = 2;
			this._instructionCheckBox.Text = "&Instruction";
			this._instructionCheckBox.UseVisualStyleBackColor = true;
			this._instructionCheckBox.CheckedChanged += new System.EventHandler(this.FieldCheckedChanged);
			// 
			// mMatchWholeWordCheckBox
			// 
			this._matchWholeWordCheckBox.AutoSize = true;
			this._matchWholeWordCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._matchWholeWordCheckBox.Location = new System.Drawing.Point(14, 171);
			this._matchWholeWordCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this._matchWholeWordCheckBox.Name = "mMatchWholeWordCheckBox";
			this._matchWholeWordCheckBox.Size = new System.Drawing.Size(122, 19);
			this._matchWholeWordCheckBox.TabIndex = 7;
			this._matchWholeWordCheckBox.Text = "&Match whole word";
			this._matchWholeWordCheckBox.UseVisualStyleBackColor = true;
			// 
			// mWrapSearchCheckBox
			// 
			this._wrapSearchCheckBox.AutoSize = true;
			this._wrapSearchCheckBox.Checked = true;
			this._wrapSearchCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._wrapSearchCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._wrapSearchCheckBox.Location = new System.Drawing.Point(14, 197);
			this._wrapSearchCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this._wrapSearchCheckBox.Name = "mWrapSearchCheckBox";
			this._wrapSearchCheckBox.Size = new System.Drawing.Size(88, 19);
			this._wrapSearchCheckBox.TabIndex = 8;
			this._wrapSearchCheckBox.Text = "&Wrap search";
			this._wrapSearchCheckBox.UseVisualStyleBackColor = true;
			// 
			// mFindButton
			// 
			this._findButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._findButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._findButton.Enabled = false;
			this._findButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._findButton.Location = new System.Drawing.Point(249, 29);
			this._findButton.Margin = new System.Windows.Forms.Padding(4);
			this._findButton.Name = "mFindButton";
			this._findButton.Size = new System.Drawing.Size(75, 26);
			this._findButton.TabIndex = 4;
			this._findButton.Text = "&Find";
			this._findButton.UseVisualStyleBackColor = true;
			this._findButton.Click += new System.EventHandler(this.FindButton_Click);
			// 
			// mCancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._cancelButton.Location = new System.Drawing.Point(249, 62);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(4);
			this._cancelButton.Name = "mCancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 26);
			this._cancelButton.TabIndex = 5;
			this._cancelButton.Text = "&Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// mInFieldsGroupBox
			// 
			this._fieldsGroupBox.Controls.Add(this._valueCheckBox);
			this._fieldsGroupBox.Controls.Add(this._charsCheckBox);
			this._fieldsGroupBox.Controls.Add(this._instructionCheckBox);
			this._fieldsGroupBox.Location = new System.Drawing.Point(14, 60);
			this._fieldsGroupBox.Margin = new System.Windows.Forms.Padding(4);
			this._fieldsGroupBox.Name = "mInFieldsGroupBox";
			this._fieldsGroupBox.Padding = new System.Windows.Forms.Padding(4);
			this._fieldsGroupBox.Size = new System.Drawing.Size(199, 104);
			this._fieldsGroupBox.TabIndex = 6;
			this._fieldsGroupBox.TabStop = false;
			this._fieldsGroupBox.Text = "In";
			// 
			// label1
			// 
			this._clipboardLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._clipboardLabel.AutoSize = true;
			this._clipboardLabel.Location = new System.Drawing.Point(170, 9);
			this._clipboardLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._clipboardLabel.Name = "label1";
			this._clipboardLabel.Size = new System.Drawing.Size(62, 15);
			this._clipboardLabel.TabIndex = 2;
			this._clipboardLabel.Text = "Clipboard:";
			// 
			// mixCharClipboardButtonControl1
			// 
			this._clipboardButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._clipboardButtons.Location = new System.Drawing.Point(170, 28);
			this._clipboardButtons.Margin = new System.Windows.Forms.Padding(4);
			this._clipboardButtons.Name = "mixCharClipboardButtonControl1";
			this._clipboardButtons.Size = new System.Drawing.Size(71, 24);
			this._clipboardButtons.TabIndex = 3;
			// 
			// mSearchTextBox
			// 
			this._searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._searchTextBox.BackColor = System.Drawing.Color.White;
			this._searchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._searchTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this._searchTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this._searchTextBox.ForeColor = System.Drawing.Color.Blue;
			this._searchTextBox.Location = new System.Drawing.Point(14, 28);
			this._searchTextBox.Margin = new System.Windows.Forms.Padding(4);
			fullWord1.LongValue = ((long)(0));
			fullWord1.Magnitude = new MixLib.Type.MixByte[] {
        mixByte1,
        mixByte2,
        mixByte3,
        mixByte4,
        mixByte5};
			fullWord1.MagnitudeLongValue = ((long)(0));
			fullWord1.Sign = MixLib.Type.Word.Signs.Positive;
			this._searchTextBox.MixByteCollectionValue = fullWord1;
			this._searchTextBox.Name = "mSearchTextBox";
			this._searchTextBox.Size = new System.Drawing.Size(148, 21);
			this._searchTextBox.TabIndex = 1;
			this._searchTextBox.UseEditMode = false;
			this._searchTextBox.ValueChanged += new MixGui.Events.MixByteCollectionEditorValueChangedEventHandler(this.SearchTextBox_ValueChanged);
			// 
			// SearchDialog
			// 
			this.AcceptButton = this._findButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(332, 227);
			this.Controls.Add(this._clipboardButtons);
			this.Controls.Add(this._fieldsGroupBox);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._findButton);
			this.Controls.Add(this._wrapSearchCheckBox);
			this.Controls.Add(this._matchWholeWordCheckBox);
			this.Controls.Add(this._searchTextBox);
			this.Controls.Add(this._clipboardLabel);
			this.Controls.Add(this._findLabel);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximumSize = new System.Drawing.Size(930, 266);
			this.MinimumSize = new System.Drawing.Size(347, 266);
			this.Name = "SearchDialog";
			this.Text = "Search Parameters";
			this.VisibleChanged += new System.EventHandler(this.SearchDialog_VisibleChanged);
			this._fieldsGroupBox.ResumeLayout(false);
			this._fieldsGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _findLabel;
		private MixByteCollectionCharTextBox _searchTextBox;
		private System.Windows.Forms.CheckBox _valueCheckBox;
		private System.Windows.Forms.CheckBox _charsCheckBox;
		private System.Windows.Forms.CheckBox _instructionCheckBox;
		private System.Windows.Forms.CheckBox _matchWholeWordCheckBox;
		private System.Windows.Forms.CheckBox _wrapSearchCheckBox;
		private System.Windows.Forms.Button _findButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.GroupBox _fieldsGroupBox;
		private MixCharClipboardButtonControl _clipboardButtons;
		private System.Windows.Forms.Label _clipboardLabel;
	}
}
