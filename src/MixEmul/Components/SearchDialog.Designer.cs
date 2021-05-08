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
			this.findLabel = new System.Windows.Forms.Label();
			this.valueCheckBox = new System.Windows.Forms.CheckBox();
			this.charsCheckBox = new System.Windows.Forms.CheckBox();
			this.instructionCheckBox = new System.Windows.Forms.CheckBox();
			this.matchWholeWordCheckBox = new System.Windows.Forms.CheckBox();
			this.wrapSearchCheckBox = new System.Windows.Forms.CheckBox();
			this.findButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.fieldsGroupBox = new System.Windows.Forms.GroupBox();
			this.clipboardLabel = new System.Windows.Forms.Label();
			this.clipboardButtons = new MixGui.Components.MixCharClipboardButtonControl();
			this.searchTextBox = new MixGui.Components.MixByteCollectionCharTextBox();
			this.fieldsGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// mFindLabel
			// 
			this.findLabel.AutoSize = true;
			this.findLabel.Location = new System.Drawing.Point(13, 9);
			this.findLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.findLabel.Name = "mFindLabel";
			this.findLabel.Size = new System.Drawing.Size(56, 15);
			this.findLabel.TabIndex = 0;
			this.findLabel.Text = "Find &text:";
			// 
			// mValueCheckBox
			// 
			this.valueCheckBox.AutoSize = true;
			this.valueCheckBox.Checked = true;
			this.valueCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.valueCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.valueCheckBox.Location = new System.Drawing.Point(7, 22);
			this.valueCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this.valueCheckBox.Name = "mValueCheckBox";
			this.valueCheckBox.Size = new System.Drawing.Size(100, 19);
			this.valueCheckBox.TabIndex = 0;
			this.valueCheckBox.Text = "&Numeric value";
			this.valueCheckBox.UseVisualStyleBackColor = true;
			this.valueCheckBox.CheckedChanged += new System.EventHandler(this.FieldCheckedChanged);
			// 
			// mCharsCheckBox
			// 
			this.charsCheckBox.AutoSize = true;
			this.charsCheckBox.Checked = true;
			this.charsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.charsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.charsCheckBox.Location = new System.Drawing.Point(7, 49);
			this.charsCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this.charsCheckBox.Name = "mCharsCheckBox";
			this.charsCheckBox.Size = new System.Drawing.Size(79, 19);
			this.charsCheckBox.TabIndex = 1;
			this.charsCheckBox.Text = "&Characters";
			this.charsCheckBox.UseVisualStyleBackColor = true;
			this.charsCheckBox.CheckedChanged += new System.EventHandler(this.FieldCheckedChanged);
			// 
			// mInstructionCheckBox
			// 
			this.instructionCheckBox.AutoSize = true;
			this.instructionCheckBox.Checked = true;
			this.instructionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.instructionCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.instructionCheckBox.Location = new System.Drawing.Point(7, 75);
			this.instructionCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this.instructionCheckBox.Name = "mInstructionCheckBox";
			this.instructionCheckBox.Size = new System.Drawing.Size(80, 19);
			this.instructionCheckBox.TabIndex = 2;
			this.instructionCheckBox.Text = "&Instruction";
			this.instructionCheckBox.UseVisualStyleBackColor = true;
			this.instructionCheckBox.CheckedChanged += new System.EventHandler(this.FieldCheckedChanged);
			// 
			// mMatchWholeWordCheckBox
			// 
			this.matchWholeWordCheckBox.AutoSize = true;
			this.matchWholeWordCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.matchWholeWordCheckBox.Location = new System.Drawing.Point(14, 171);
			this.matchWholeWordCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this.matchWholeWordCheckBox.Name = "mMatchWholeWordCheckBox";
			this.matchWholeWordCheckBox.Size = new System.Drawing.Size(122, 19);
			this.matchWholeWordCheckBox.TabIndex = 7;
			this.matchWholeWordCheckBox.Text = "&Match whole word";
			this.matchWholeWordCheckBox.UseVisualStyleBackColor = true;
			// 
			// mWrapSearchCheckBox
			// 
			this.wrapSearchCheckBox.AutoSize = true;
			this.wrapSearchCheckBox.Checked = true;
			this.wrapSearchCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.wrapSearchCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.wrapSearchCheckBox.Location = new System.Drawing.Point(14, 197);
			this.wrapSearchCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this.wrapSearchCheckBox.Name = "mWrapSearchCheckBox";
			this.wrapSearchCheckBox.Size = new System.Drawing.Size(88, 19);
			this.wrapSearchCheckBox.TabIndex = 8;
			this.wrapSearchCheckBox.Text = "&Wrap search";
			this.wrapSearchCheckBox.UseVisualStyleBackColor = true;
			// 
			// mFindButton
			// 
			this.findButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.findButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.findButton.Enabled = false;
			this.findButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.findButton.Location = new System.Drawing.Point(249, 29);
			this.findButton.Margin = new System.Windows.Forms.Padding(4);
			this.findButton.Name = "mFindButton";
			this.findButton.Size = new System.Drawing.Size(75, 26);
			this.findButton.TabIndex = 4;
			this.findButton.Text = "&Find";
			this.findButton.UseVisualStyleBackColor = true;
			this.findButton.Click += new System.EventHandler(this.FindButton_Click);
			// 
			// mCancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cancelButton.Location = new System.Drawing.Point(249, 62);
			this.cancelButton.Margin = new System.Windows.Forms.Padding(4);
			this.cancelButton.Name = "mCancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 26);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// mInFieldsGroupBox
			// 
			this.fieldsGroupBox.Controls.Add(this.valueCheckBox);
			this.fieldsGroupBox.Controls.Add(this.charsCheckBox);
			this.fieldsGroupBox.Controls.Add(this.instructionCheckBox);
			this.fieldsGroupBox.Location = new System.Drawing.Point(14, 60);
			this.fieldsGroupBox.Margin = new System.Windows.Forms.Padding(4);
			this.fieldsGroupBox.Name = "mInFieldsGroupBox";
			this.fieldsGroupBox.Padding = new System.Windows.Forms.Padding(4);
			this.fieldsGroupBox.Size = new System.Drawing.Size(199, 104);
			this.fieldsGroupBox.TabIndex = 6;
			this.fieldsGroupBox.TabStop = false;
			this.fieldsGroupBox.Text = "In";
			// 
			// label1
			// 
			this.clipboardLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.clipboardLabel.AutoSize = true;
			this.clipboardLabel.Location = new System.Drawing.Point(170, 9);
			this.clipboardLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.clipboardLabel.Name = "label1";
			this.clipboardLabel.Size = new System.Drawing.Size(62, 15);
			this.clipboardLabel.TabIndex = 2;
			this.clipboardLabel.Text = "Clipboard:";
			// 
			// mixCharClipboardButtonControl1
			// 
			this.clipboardButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.clipboardButtons.Location = new System.Drawing.Point(170, 28);
			this.clipboardButtons.Margin = new System.Windows.Forms.Padding(4);
			this.clipboardButtons.Name = "mixCharClipboardButtonControl1";
			this.clipboardButtons.Size = new System.Drawing.Size(71, 24);
			this.clipboardButtons.TabIndex = 3;
			// 
			// mSearchTextBox
			// 
			this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.searchTextBox.BackColor = System.Drawing.Color.White;
			this.searchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.searchTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.searchTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.searchTextBox.ForeColor = System.Drawing.Color.Blue;
			this.searchTextBox.Location = new System.Drawing.Point(14, 28);
			this.searchTextBox.Margin = new System.Windows.Forms.Padding(4);
			fullWord1.LongValue = ((long)(0));
			fullWord1.Magnitude = new MixLib.Type.MixByte[] {
        mixByte1,
        mixByte2,
        mixByte3,
        mixByte4,
        mixByte5};
			fullWord1.MagnitudeLongValue = ((long)(0));
			fullWord1.Sign = MixLib.Type.Word.Signs.Positive;
			this.searchTextBox.MixByteCollectionValue = fullWord1;
			this.searchTextBox.Name = "mSearchTextBox";
			this.searchTextBox.Size = new System.Drawing.Size(148, 21);
			this.searchTextBox.TabIndex = 1;
			this.searchTextBox.UseEditMode = false;
			this.searchTextBox.ValueChanged += new MixGui.Events.MixByteCollectionEditorValueChangedEventHandler(this.SearchTextBox_ValueChanged);
			// 
			// SearchDialog
			// 
			this.AcceptButton = this.findButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(332, 227);
			this.Controls.Add(this.clipboardButtons);
			this.Controls.Add(this.fieldsGroupBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.findButton);
			this.Controls.Add(this.wrapSearchCheckBox);
			this.Controls.Add(this.matchWholeWordCheckBox);
			this.Controls.Add(this.searchTextBox);
			this.Controls.Add(this.clipboardLabel);
			this.Controls.Add(this.findLabel);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximumSize = new System.Drawing.Size(930, 266);
			this.MinimumSize = new System.Drawing.Size(347, 266);
			this.Name = "SearchDialog";
			this.Text = "Search Parameters";
			this.VisibleChanged += new System.EventHandler(this.SearchDialog_VisibleChanged);
			this.fieldsGroupBox.ResumeLayout(false);
			this.fieldsGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label findLabel;
		private MixByteCollectionCharTextBox searchTextBox;
		private System.Windows.Forms.CheckBox valueCheckBox;
		private System.Windows.Forms.CheckBox charsCheckBox;
		private System.Windows.Forms.CheckBox instructionCheckBox;
		private System.Windows.Forms.CheckBox matchWholeWordCheckBox;
		private System.Windows.Forms.CheckBox wrapSearchCheckBox;
		private System.Windows.Forms.Button findButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.GroupBox fieldsGroupBox;
		private MixCharClipboardButtonControl clipboardButtons;
		private System.Windows.Forms.Label clipboardLabel;
	}
}
