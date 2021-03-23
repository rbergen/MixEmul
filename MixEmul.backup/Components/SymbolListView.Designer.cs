namespace MixGui.Components
{
  partial class SymbolListView
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
			this.mListView = new System.Windows.Forms.ListView();
			this.mNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.mValueColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.mSymbolNameTextBox = new System.Windows.Forms.TextBox();
			this.mSetButton = new System.Windows.Forms.Button();
			this.mUnsetButton = new System.Windows.Forms.Button();
			this.mSymbolValueTextBox = new MixGui.Components.LongValueTextBox();
			this.SuspendLayout();
			// 
			// mListView
			// 
			this.mListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mNameColumnHeader,
            this.mValueColumnHeader});
			this.mListView.FullRowSelect = true;
			this.mListView.GridLines = true;
			this.mListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.mListView.HideSelection = false;
			this.mListView.Location = new System.Drawing.Point(0, 25);
			this.mListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.mListView.MultiSelect = false;
			this.mListView.Name = "mListView";
			this.mListView.ShowGroups = false;
			this.mListView.Size = new System.Drawing.Size(290, 193);
			this.mListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.mListView.TabIndex = 4;
			this.mListView.UseCompatibleStateImageBehavior = false;
			this.mListView.View = System.Windows.Forms.View.Details;
			this.mListView.SelectedIndexChanged += new System.EventHandler(this.MListView_SelectedIndexChanged);
			this.mListView.DoubleClick += new System.EventHandler(this.MListView_DoubleClick);
			this.mListView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MListView_KeyPress);
			// 
			// mNameColumnHeader
			// 
			this.mNameColumnHeader.Text = "Name";
			this.mNameColumnHeader.Width = 114;
			// 
			// mValueColumnHeader
			// 
			this.mValueColumnHeader.Text = "Value";
			this.mValueColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.mValueColumnHeader.Width = 83;
			// 
			// mSymbolNameTextBox
			// 
			this.mSymbolNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mSymbolNameTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.mSymbolNameTextBox.Location = new System.Drawing.Point(0, 0);
			this.mSymbolNameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.mSymbolNameTextBox.Name = "mSymbolNameTextBox";
			this.mSymbolNameTextBox.Size = new System.Drawing.Size(143, 22);
			this.mSymbolNameTextBox.TabIndex = 0;
			// 
			// mSetButton
			// 
			this.mSetButton.Enabled = false;
			this.mSetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mSetButton.Location = new System.Drawing.Point(186, 0);
			this.mSetButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.mSetButton.Name = "mSetButton";
			this.mSetButton.Size = new System.Drawing.Size(41, 26);
			this.mSetButton.TabIndex = 2;
			this.mSetButton.Text = "&Set";
			this.mSetButton.UseVisualStyleBackColor = true;
			this.mSetButton.Click += new System.EventHandler(this.MSetButton_Click);
			// 
			// mUnsetButton
			// 
			this.mUnsetButton.Enabled = false;
			this.mUnsetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mUnsetButton.Location = new System.Drawing.Point(226, 0);
			this.mUnsetButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.mUnsetButton.Name = "mUnsetButton";
			this.mUnsetButton.Size = new System.Drawing.Size(64, 26);
			this.mUnsetButton.TabIndex = 3;
			this.mUnsetButton.Text = "U&nset";
			this.mUnsetButton.UseVisualStyleBackColor = true;
			this.mUnsetButton.Click += new System.EventHandler(this.MUnsetButton_Click);
			// 
			// mSymbolValueTextBox
			// 
			this.mSymbolValueTextBox.BackColor = System.Drawing.Color.White;
			this.mSymbolValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mSymbolValueTextBox.ClearZero = true;
			this.mSymbolValueTextBox.ForeColor = System.Drawing.Color.Black;
			this.mSymbolValueTextBox.Location = new System.Drawing.Point(144, 0);
			this.mSymbolValueTextBox.LongValue = ((long)(0));
			this.mSymbolValueTextBox.Magnitude = ((long)(0));
			this.mSymbolValueTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.mSymbolValueTextBox.MaxLength = 20;
			this.mSymbolValueTextBox.MaxValue = ((long)(9223372036854775807));
			this.mSymbolValueTextBox.MinValue = ((long)(-9223372036854775808));
			this.mSymbolValueTextBox.Name = "mSymbolValueTextBox";
			this.mSymbolValueTextBox.Sign = MixLib.Type.Word.Signs.Positive;
			this.mSymbolValueTextBox.Size = new System.Drawing.Size(42, 22);
			this.mSymbolValueTextBox.SupportNegativeZero = true;
			this.mSymbolValueTextBox.TabIndex = 1;
			this.mSymbolValueTextBox.Text = "0";
			// 
			// SymbolListView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mUnsetButton);
			this.Controls.Add(this.mSetButton);
			this.Controls.Add(this.mSymbolValueTextBox);
			this.Controls.Add(this.mSymbolNameTextBox);
			this.Controls.Add(this.mListView);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "SymbolListView";
			this.Size = new System.Drawing.Size(291, 218);
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListView mListView;
    private System.Windows.Forms.ColumnHeader mNameColumnHeader;
    private System.Windows.Forms.ColumnHeader mValueColumnHeader;
    private System.Windows.Forms.TextBox mSymbolNameTextBox;
    private LongValueTextBox mSymbolValueTextBox;
    private System.Windows.Forms.Button mSetButton;
    private System.Windows.Forms.Button mUnsetButton;
  }
}
