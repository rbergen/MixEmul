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
			this.listView = new System.Windows.Forms.ListView();
			this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.valueColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.symbolNameTextBox = new System.Windows.Forms.TextBox();
			this.setButton = new System.Windows.Forms.Button();
			this.unsetButton = new System.Windows.Forms.Button();
			this.symbolValueTextBox = new MixGui.Components.LongValueTextBox();
			this.SuspendLayout();
			// 
			// mListView
			// 
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.valueColumnHeader});
			this.listView.FullRowSelect = true;
			this.listView.GridLines = true;
			this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView.HideSelection = false;
			this.listView.Location = new System.Drawing.Point(0, 23);
			this.listView.Margin = new System.Windows.Forms.Padding(4);
			this.listView.MultiSelect = false;
			this.listView.Name = "mListView";
			this.listView.ShowGroups = false;
			this.listView.Size = new System.Drawing.Size(254, 181);
			this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listView.TabIndex = 4;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
			this.listView.DoubleClick += new System.EventHandler(this.ListView_DoubleClick);
			this.listView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ListView_KeyPress);
			// 
			// mNameColumnHeader
			// 
			this.nameColumnHeader.Text = "Name";
			this.nameColumnHeader.Width = 114;
			// 
			// mValueColumnHeader
			// 
			this.valueColumnHeader.Text = "Value";
			this.valueColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.valueColumnHeader.Width = 83;
			// 
			// mSymbolNameTextBox
			// 
			this.symbolNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.symbolNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.symbolNameTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.symbolNameTextBox.Location = new System.Drawing.Point(0, 0);
			this.symbolNameTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.symbolNameTextBox.Name = "mSymbolNameTextBox";
			this.symbolNameTextBox.Size = new System.Drawing.Size(128, 23);
			this.symbolNameTextBox.TabIndex = 0;
			// 
			// mSetButton
			// 
			this.setButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.setButton.Enabled = false;
			this.setButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.setButton.Location = new System.Drawing.Point(163, 0);
			this.setButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.setButton.Name = "mSetButton";
			this.setButton.Size = new System.Drawing.Size(36, 24);
			this.setButton.TabIndex = 2;
			this.setButton.Text = "&Set";
			this.setButton.UseVisualStyleBackColor = true;
			this.setButton.Click += new System.EventHandler(this.SetButton_Click);
			// 
			// mUnsetButton
			// 
			this.unsetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.unsetButton.Enabled = false;
			this.unsetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.unsetButton.Location = new System.Drawing.Point(198, 0);
			this.unsetButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.unsetButton.Name = "mUnsetButton";
			this.unsetButton.Size = new System.Drawing.Size(56, 24);
			this.unsetButton.TabIndex = 3;
			this.unsetButton.Text = "U&nset";
			this.unsetButton.UseVisualStyleBackColor = true;
			this.unsetButton.Click += new System.EventHandler(this.UnsetButton_Click);
			// 
			// mSymbolValueTextBox
			// 
			this.symbolValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.symbolValueTextBox.BackColor = System.Drawing.Color.White;
			this.symbolValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.symbolValueTextBox.ClearZero = true;
			this.symbolValueTextBox.ForeColor = System.Drawing.Color.Black;
			this.symbolValueTextBox.Location = new System.Drawing.Point(127, 0);
			this.symbolValueTextBox.LongValue = ((long)(0));
			this.symbolValueTextBox.Magnitude = ((long)(0));
			this.symbolValueTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.symbolValueTextBox.MaxLength = 20;
			this.symbolValueTextBox.MaxValue = ((long)(9223372036854775807));
			this.symbolValueTextBox.MinValue = ((long)(-9223372036854775808));
			this.symbolValueTextBox.Name = "mSymbolValueTextBox";
			this.symbolValueTextBox.Sign = MixLib.Type.Word.Signs.Positive;
			this.symbolValueTextBox.Size = new System.Drawing.Size(37, 23);
			this.symbolValueTextBox.SupportNegativeZero = true;
			this.symbolValueTextBox.TabIndex = 1;
			this.symbolValueTextBox.Text = "0";
			// 
			// SymbolListView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.unsetButton);
			this.Controls.Add(this.setButton);
			this.Controls.Add(this.symbolValueTextBox);
			this.Controls.Add(this.symbolNameTextBox);
			this.Controls.Add(this.listView);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "SymbolListView";
			this.Size = new System.Drawing.Size(255, 204);
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListView listView;
    private System.Windows.Forms.ColumnHeader nameColumnHeader;
    private System.Windows.Forms.ColumnHeader valueColumnHeader;
    private System.Windows.Forms.TextBox symbolNameTextBox;
    private LongValueTextBox symbolValueTextBox;
    private System.Windows.Forms.Button setButton;
    private System.Windows.Forms.Button unsetButton;
  }
}
