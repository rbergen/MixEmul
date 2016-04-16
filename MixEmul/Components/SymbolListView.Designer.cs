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
      if (disposing && (components != null))
      {
        components.Dispose();
      }
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
      this.mNameColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.mValueColumnHeader = new System.Windows.Forms.ColumnHeader();
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
      this.mListView.Location = new System.Drawing.Point(0, 20);
      this.mListView.MultiSelect = false;
      this.mListView.Name = "mListView";
      this.mListView.ShowGroups = false;
      this.mListView.Size = new System.Drawing.Size(218, 157);
      this.mListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.mListView.TabIndex = 4;
      this.mListView.UseCompatibleStateImageBehavior = false;
      this.mListView.View = System.Windows.Forms.View.Details;
      this.mListView.DoubleClick += new System.EventHandler(this.mListView_DoubleClick);
      this.mListView.SelectedIndexChanged += new System.EventHandler(this.mListView_SelectedIndexChanged);
      this.mListView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mListView_KeyPress);
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
      this.mSymbolNameTextBox.Name = "mSymbolNameTextBox";
      this.mSymbolNameTextBox.Size = new System.Drawing.Size(114, 20);
      this.mSymbolNameTextBox.TabIndex = 0;
      // 
      // mSetButton
      // 
      this.mSetButton.Enabled = false;
      this.mSetButton.Location = new System.Drawing.Point(145, 0);
      this.mSetButton.Margin = new System.Windows.Forms.Padding(2);
      this.mSetButton.Name = "mSetButton";
      this.mSetButton.Size = new System.Drawing.Size(31, 21);
      this.mSetButton.TabIndex = 2;
      this.mSetButton.Text = "&Set";
      this.mSetButton.UseVisualStyleBackColor = true;
      this.mSetButton.Click += new System.EventHandler(this.mSetButton_Click);
      // 
      // mUnsetButton
      // 
      this.mUnsetButton.Enabled = false;
      this.mUnsetButton.Location = new System.Drawing.Point(175, 0);
      this.mUnsetButton.Margin = new System.Windows.Forms.Padding(2);
      this.mUnsetButton.Name = "mUnsetButton";
      this.mUnsetButton.Size = new System.Drawing.Size(43, 21);
      this.mUnsetButton.TabIndex = 3;
      this.mUnsetButton.Text = "U&nset";
      this.mUnsetButton.UseVisualStyleBackColor = true;
      this.mUnsetButton.Click += new System.EventHandler(this.mUnsetButton_Click);
      // 
      // mSymbolValueTextBox
      // 
      this.mSymbolValueTextBox.BackColor = System.Drawing.Color.White;
      this.mSymbolValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.mSymbolValueTextBox.ForeColor = System.Drawing.Color.Black;
      this.mSymbolValueTextBox.Location = new System.Drawing.Point(113, 0);
      this.mSymbolValueTextBox.LongValue = ((long)(0));
      this.mSymbolValueTextBox.MaxLength = 20;
      this.mSymbolValueTextBox.MaxValue = ((long)(9223372036854775807));
      this.mSymbolValueTextBox.MinValue = ((long)(-9223372036854775808));
      this.mSymbolValueTextBox.Name = "mSymbolValueTextBox";
      this.mSymbolValueTextBox.Size = new System.Drawing.Size(32, 20);
      this.mSymbolValueTextBox.SupportNegativeZero = true;
      this.mSymbolValueTextBox.TabIndex = 1;
      this.mSymbolValueTextBox.Text = "0";
      // 
      // SymbolListView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.mUnsetButton);
      this.Controls.Add(this.mSetButton);
      this.Controls.Add(this.mSymbolValueTextBox);
      this.Controls.Add(this.mSymbolNameTextBox);
      this.Controls.Add(this.mListView);
      this.Name = "SymbolListView";
      this.Size = new System.Drawing.Size(218, 177);
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
