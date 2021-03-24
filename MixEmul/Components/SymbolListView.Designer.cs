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
			this._listView = new System.Windows.Forms.ListView();
			this._nameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this._valueColumnHeader = new System.Windows.Forms.ColumnHeader();
			this._symbolNameTextBox = new System.Windows.Forms.TextBox();
			this._setButton = new System.Windows.Forms.Button();
			this._unsetButton = new System.Windows.Forms.Button();
			this._symbolValueTextBox = new MixGui.Components.LongValueTextBox();
			this.SuspendLayout();
			// 
			// mListView
			// 
			this._listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._listView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._nameColumnHeader,
            this._valueColumnHeader});
			this._listView.FullRowSelect = true;
			this._listView.GridLines = true;
			this._listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._listView.HideSelection = false;
			this._listView.Location = new System.Drawing.Point(0, 23);
			this._listView.Margin = new System.Windows.Forms.Padding(4);
			this._listView.MultiSelect = false;
			this._listView.Name = "mListView";
			this._listView.ShowGroups = false;
			this._listView.Size = new System.Drawing.Size(254, 181);
			this._listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this._listView.TabIndex = 4;
			this._listView.UseCompatibleStateImageBehavior = false;
			this._listView.View = System.Windows.Forms.View.Details;
			this._listView.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
			this._listView.DoubleClick += new System.EventHandler(this.ListView_DoubleClick);
			this._listView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ListView_KeyPress);
			// 
			// mNameColumnHeader
			// 
			this._nameColumnHeader.Text = "Name";
			this._nameColumnHeader.Width = 114;
			// 
			// mValueColumnHeader
			// 
			this._valueColumnHeader.Text = "Value";
			this._valueColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._valueColumnHeader.Width = 83;
			// 
			// mSymbolNameTextBox
			// 
			this._symbolNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._symbolNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._symbolNameTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this._symbolNameTextBox.Location = new System.Drawing.Point(0, 0);
			this._symbolNameTextBox.Margin = new System.Windows.Forms.Padding(4);
			this._symbolNameTextBox.Name = "mSymbolNameTextBox";
			this._symbolNameTextBox.Size = new System.Drawing.Size(128, 23);
			this._symbolNameTextBox.TabIndex = 0;
			// 
			// mSetButton
			// 
			this._setButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._setButton.Enabled = false;
			this._setButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._setButton.Location = new System.Drawing.Point(163, 0);
			this._setButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this._setButton.Name = "mSetButton";
			this._setButton.Size = new System.Drawing.Size(36, 24);
			this._setButton.TabIndex = 2;
			this._setButton.Text = "&Set";
			this._setButton.UseVisualStyleBackColor = true;
			this._setButton.Click += new System.EventHandler(this.SetButton_Click);
			// 
			// mUnsetButton
			// 
			this._unsetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._unsetButton.Enabled = false;
			this._unsetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._unsetButton.Location = new System.Drawing.Point(198, 0);
			this._unsetButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this._unsetButton.Name = "mUnsetButton";
			this._unsetButton.Size = new System.Drawing.Size(56, 24);
			this._unsetButton.TabIndex = 3;
			this._unsetButton.Text = "U&nset";
			this._unsetButton.UseVisualStyleBackColor = true;
			this._unsetButton.Click += new System.EventHandler(this.UnsetButton_Click);
			// 
			// mSymbolValueTextBox
			// 
			this._symbolValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._symbolValueTextBox.BackColor = System.Drawing.Color.White;
			this._symbolValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._symbolValueTextBox.ClearZero = true;
			this._symbolValueTextBox.ForeColor = System.Drawing.Color.Black;
			this._symbolValueTextBox.Location = new System.Drawing.Point(127, 0);
			this._symbolValueTextBox.LongValue = ((long)(0));
			this._symbolValueTextBox.Magnitude = ((long)(0));
			this._symbolValueTextBox.Margin = new System.Windows.Forms.Padding(4);
			this._symbolValueTextBox.MaxLength = 20;
			this._symbolValueTextBox.MaxValue = ((long)(9223372036854775807));
			this._symbolValueTextBox.MinValue = ((long)(-9223372036854775808));
			this._symbolValueTextBox.Name = "mSymbolValueTextBox";
			this._symbolValueTextBox.Sign = MixLib.Type.Word.Signs.Positive;
			this._symbolValueTextBox.Size = new System.Drawing.Size(37, 23);
			this._symbolValueTextBox.SupportNegativeZero = true;
			this._symbolValueTextBox.TabIndex = 1;
			this._symbolValueTextBox.Text = "0";
			// 
			// SymbolListView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._unsetButton);
			this.Controls.Add(this._setButton);
			this.Controls.Add(this._symbolValueTextBox);
			this.Controls.Add(this._symbolNameTextBox);
			this.Controls.Add(this._listView);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "SymbolListView";
			this.Size = new System.Drawing.Size(255, 204);
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListView _listView;
    private System.Windows.Forms.ColumnHeader _nameColumnHeader;
    private System.Windows.Forms.ColumnHeader _valueColumnHeader;
    private System.Windows.Forms.TextBox _symbolNameTextBox;
    private LongValueTextBox _symbolValueTextBox;
    private System.Windows.Forms.Button _setButton;
    private System.Windows.Forms.Button _unsetButton;
  }
}
