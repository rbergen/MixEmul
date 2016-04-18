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
			if (disposing)
			{
				components?.Dispose();
			}
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
			this.mFindLabel = new System.Windows.Forms.Label();
			this.mValueCheckBox = new System.Windows.Forms.CheckBox();
			this.mCharsCheckBox = new System.Windows.Forms.CheckBox();
			this.mInstructionCheckBox = new System.Windows.Forms.CheckBox();
			this.mMatchWholeWordCheckBox = new System.Windows.Forms.CheckBox();
			this.mWrapSearchCheckBox = new System.Windows.Forms.CheckBox();
			this.mFindButton = new System.Windows.Forms.Button();
			this.mCancelButton = new System.Windows.Forms.Button();
			this.mInFieldsGroupBox = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.mixCharClipboardButtonControl1 = new MixGui.Components.MixCharClipboardButtonControl();
			this.mSearchTextBox = new MixGui.Components.MixByteCollectionCharTextBox();
			this.mInFieldsGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// mFindLabel
			// 
			this.mFindLabel.AutoSize = true;
			this.mFindLabel.Location = new System.Drawing.Point(9, 10);
			this.mFindLabel.Name = "mFindLabel";
			this.mFindLabel.Size = new System.Drawing.Size(50, 13);
			this.mFindLabel.TabIndex = 0;
			this.mFindLabel.Text = "Find &text:";
			// 
			// mValueCheckBox
			// 
			this.mValueCheckBox.AutoSize = true;
			this.mValueCheckBox.Checked = true;
			this.mValueCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mValueCheckBox.Location = new System.Drawing.Point(6, 19);
			this.mValueCheckBox.Name = "mValueCheckBox";
			this.mValueCheckBox.Size = new System.Drawing.Size(94, 17);
			this.mValueCheckBox.TabIndex = 0;
			this.mValueCheckBox.Text = "&Numeric value";
			this.mValueCheckBox.UseVisualStyleBackColor = true;
			this.mValueCheckBox.CheckedChanged += new System.EventHandler(this.fieldCheckedChanged);
			// 
			// mCharsCheckBox
			// 
			this.mCharsCheckBox.AutoSize = true;
			this.mCharsCheckBox.Checked = true;
			this.mCharsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mCharsCheckBox.Location = new System.Drawing.Point(6, 42);
			this.mCharsCheckBox.Name = "mCharsCheckBox";
			this.mCharsCheckBox.Size = new System.Drawing.Size(77, 17);
			this.mCharsCheckBox.TabIndex = 1;
			this.mCharsCheckBox.Text = "&Characters";
			this.mCharsCheckBox.UseVisualStyleBackColor = true;
			this.mCharsCheckBox.CheckedChanged += new System.EventHandler(this.fieldCheckedChanged);
			// 
			// mInstructionCheckBox
			// 
			this.mInstructionCheckBox.AutoSize = true;
			this.mInstructionCheckBox.Checked = true;
			this.mInstructionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mInstructionCheckBox.Location = new System.Drawing.Point(6, 65);
			this.mInstructionCheckBox.Name = "mInstructionCheckBox";
			this.mInstructionCheckBox.Size = new System.Drawing.Size(75, 17);
			this.mInstructionCheckBox.TabIndex = 2;
			this.mInstructionCheckBox.Text = "&Instruction";
			this.mInstructionCheckBox.UseVisualStyleBackColor = true;
			this.mInstructionCheckBox.CheckedChanged += new System.EventHandler(this.fieldCheckedChanged);
			// 
			// mMatchWholeWordCheckBox
			// 
			this.mMatchWholeWordCheckBox.AutoSize = true;
			this.mMatchWholeWordCheckBox.Location = new System.Drawing.Point(12, 148);
			this.mMatchWholeWordCheckBox.Name = "mMatchWholeWordCheckBox";
			this.mMatchWholeWordCheckBox.Size = new System.Drawing.Size(113, 17);
			this.mMatchWholeWordCheckBox.TabIndex = 7;
			this.mMatchWholeWordCheckBox.Text = "&Match whole word";
			this.mMatchWholeWordCheckBox.UseVisualStyleBackColor = true;
			// 
			// mWrapSearchCheckBox
			// 
			this.mWrapSearchCheckBox.AutoSize = true;
			this.mWrapSearchCheckBox.Checked = true;
			this.mWrapSearchCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mWrapSearchCheckBox.Location = new System.Drawing.Point(12, 171);
			this.mWrapSearchCheckBox.Name = "mWrapSearchCheckBox";
			this.mWrapSearchCheckBox.Size = new System.Drawing.Size(87, 17);
			this.mWrapSearchCheckBox.TabIndex = 8;
			this.mWrapSearchCheckBox.Text = "&Wrap search";
			this.mWrapSearchCheckBox.UseVisualStyleBackColor = true;
			// 
			// mFindButton
			// 
			this.mFindButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mFindButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.mFindButton.Enabled = false;
			this.mFindButton.Location = new System.Drawing.Point(197, 24);
			this.mFindButton.Name = "mFindButton";
			this.mFindButton.Size = new System.Drawing.Size(75, 23);
			this.mFindButton.TabIndex = 4;
			this.mFindButton.Text = "&Find";
			this.mFindButton.UseVisualStyleBackColor = true;
			this.mFindButton.Click += new System.EventHandler(this.mFindButton_Click);
			// 
			// mCancelButton
			// 
			this.mCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.mCancelButton.Location = new System.Drawing.Point(197, 53);
			this.mCancelButton.Name = "mCancelButton";
			this.mCancelButton.Size = new System.Drawing.Size(75, 23);
			this.mCancelButton.TabIndex = 5;
			this.mCancelButton.Text = "&Cancel";
			this.mCancelButton.UseVisualStyleBackColor = true;
			// 
			// mInFieldsGroupBox
			// 
			this.mInFieldsGroupBox.Controls.Add(this.mValueCheckBox);
			this.mInFieldsGroupBox.Controls.Add(this.mCharsCheckBox);
			this.mInFieldsGroupBox.Controls.Add(this.mInstructionCheckBox);
			this.mInFieldsGroupBox.Location = new System.Drawing.Point(12, 52);
			this.mInFieldsGroupBox.Name = "mInFieldsGroupBox";
			this.mInFieldsGroupBox.Size = new System.Drawing.Size(170, 90);
			this.mInFieldsGroupBox.TabIndex = 6;
			this.mInFieldsGroupBox.TabStop = false;
			this.mInFieldsGroupBox.Text = "In";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(118, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Clipboard:";
			// 
			// mixCharClipboardButtonControl1
			// 
			this.mixCharClipboardButtonControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mixCharClipboardButtonControl1.Location = new System.Drawing.Point(121, 25);
			this.mixCharClipboardButtonControl1.Name = "mixCharClipboardButtonControl1";
			this.mixCharClipboardButtonControl1.Size = new System.Drawing.Size(61, 21);
			this.mixCharClipboardButtonControl1.TabIndex = 3;
			// 
			// mSearchTextBox
			// 
			this.mSearchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mSearchTextBox.BackColor = System.Drawing.Color.White;
			this.mSearchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mSearchTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.mSearchTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mSearchTextBox.ForeColor = System.Drawing.Color.Blue;
			this.mSearchTextBox.Location = new System.Drawing.Point(12, 25);
			fullWord1.LongValue = ((long)(0));
			fullWord1.Magnitude = new MixLib.Type.MixByte[] {
        mixByte1,
        mixByte2,
        mixByte3,
        mixByte4,
        mixByte5};
			fullWord1.MagnitudeLongValue = ((long)(0));
			fullWord1.Sign = MixLib.Type.Word.Signs.Positive;
			this.mSearchTextBox.MixByteCollectionValue = fullWord1;
			this.mSearchTextBox.Name = "mSearchTextBox";
			this.mSearchTextBox.Size = new System.Drawing.Size(103, 21);
			this.mSearchTextBox.TabIndex = 1;
			this.mSearchTextBox.UseEditMode = false;
			this.mSearchTextBox.ValueChanged += new MixGui.Events.MixByteCollectionEditorValueChangedEventHandler(this.mSearchTextBox_ValueChanged);
			// 
			// SearchDialog
			// 
			this.AcceptButton = this.mFindButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.mCancelButton;
			this.ClientSize = new System.Drawing.Size(284, 199);
			this.Controls.Add(this.mixCharClipboardButtonControl1);
			this.Controls.Add(this.mInFieldsGroupBox);
			this.Controls.Add(this.mCancelButton);
			this.Controls.Add(this.mFindButton);
			this.Controls.Add(this.mWrapSearchCheckBox);
			this.Controls.Add(this.mMatchWholeWordCheckBox);
			this.Controls.Add(this.mSearchTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.mFindLabel);
			this.MaximumSize = new System.Drawing.Size(800, 237);
			this.MinimumSize = new System.Drawing.Size(300, 237);
			this.Name = "SearchDialog";
			this.Text = "Search Parameters";
			this.VisibleChanged += new System.EventHandler(this.SearchDialog_VisibleChanged);
			this.mInFieldsGroupBox.ResumeLayout(false);
			this.mInFieldsGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label mFindLabel;
		private MixByteCollectionCharTextBox mSearchTextBox;
		private System.Windows.Forms.CheckBox mValueCheckBox;
		private System.Windows.Forms.CheckBox mCharsCheckBox;
		private System.Windows.Forms.CheckBox mInstructionCheckBox;
		private System.Windows.Forms.CheckBox mMatchWholeWordCheckBox;
		private System.Windows.Forms.CheckBox mWrapSearchCheckBox;
		private System.Windows.Forms.Button mFindButton;
		private System.Windows.Forms.Button mCancelButton;
		private System.Windows.Forms.GroupBox mInFieldsGroupBox;
		private MixCharClipboardButtonControl mixCharClipboardButtonControl1;
		private System.Windows.Forms.Label label1;
	}
}