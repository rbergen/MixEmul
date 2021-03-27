namespace MixGui.Components
{
	partial class MemoryExportDialog
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
			this._fromAddressLabel = new System.Windows.Forms.Label();
			this._fromAddressUpDown = new System.Windows.Forms.NumericUpDown();
			this._toAddressLabel = new System.Windows.Forms.Label();
			this._toAddressUpDown = new System.Windows.Forms.NumericUpDown();
			this._programCounterLabel = new System.Windows.Forms.Label();
			this._programCounterUpDown = new System.Windows.Forms.NumericUpDown();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._fromAddressUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._toAddressUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._programCounterUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// mFromAddressLabel
			// 
			this._fromAddressLabel.AutoSize = true;
			this._fromAddressLabel.Location = new System.Drawing.Point(16, 16);
			this._fromAddressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._fromAddressLabel.Name = "mFromAddressLabel";
			this._fromAddressLabel.Size = new System.Drawing.Size(153, 17);
			this._fromAddressLabel.TabIndex = 0;
			this._fromAddressLabel.Text = "First exported address:";
			// 
			// mFromAddressUpDown
			// 
			this._fromAddressUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._fromAddressUpDown.Location = new System.Drawing.Point(181, 14);
			this._fromAddressUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._fromAddressUpDown.Name = "mFromAddressUpDown";
			this._fromAddressUpDown.Size = new System.Drawing.Size(79, 22);
			this._fromAddressUpDown.TabIndex = 1;
			this._fromAddressUpDown.ValueChanged += new System.EventHandler(this.FromAddressUpDown_ValueChanged);
			// 
			// mToAddressLabel
			// 
			this._toAddressLabel.AutoSize = true;
			this._toAddressLabel.Location = new System.Drawing.Point(16, 48);
			this._toAddressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._toAddressLabel.Name = "mToAddressLabel";
			this._toAddressLabel.Size = new System.Drawing.Size(153, 17);
			this._toAddressLabel.TabIndex = 2;
			this._toAddressLabel.Text = "Last exported address:";
			// 
			// mToAddressUpDown
			// 
			this._toAddressUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._toAddressUpDown.Location = new System.Drawing.Point(181, 46);
			this._toAddressUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._toAddressUpDown.Name = "mToAddressUpDown";
			this._toAddressUpDown.Size = new System.Drawing.Size(79, 22);
			this._toAddressUpDown.TabIndex = 3;
			this._toAddressUpDown.ValueChanged += new System.EventHandler(this.ToAddressUpDown_ValueChanged);
			// 
			// mProgramCounterLabel
			// 
			this._programCounterLabel.AutoSize = true;
			this._programCounterLabel.Location = new System.Drawing.Point(16, 80);
			this._programCounterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._programCounterLabel.Name = "mProgramCounterLabel";
			this._programCounterLabel.Size = new System.Drawing.Size(158, 17);
			this._programCounterLabel.TabIndex = 4;
			this._programCounterLabel.Text = "Set program counter to:";
			// 
			// mProgramCounterUpDown
			// 
			this._programCounterUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._programCounterUpDown.Location = new System.Drawing.Point(181, 78);
			this._programCounterUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._programCounterUpDown.Name = "mProgramCounterUpDown";
			this._programCounterUpDown.Size = new System.Drawing.Size(79, 22);
			this._programCounterUpDown.TabIndex = 5;
			// 
			// mOkButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._okButton.Location = new System.Drawing.Point(53, 113);
			this._okButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._okButton.Name = "mOkButton";
			this._okButton.Size = new System.Drawing.Size(100, 28);
			this._okButton.TabIndex = 6;
			this._okButton.Text = "&OK";
			this._okButton.UseVisualStyleBackColor = true;
			// 
			// mCancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._cancelButton.Location = new System.Drawing.Point(161, 113);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._cancelButton.Name = "mCancelButton";
			this._cancelButton.Size = new System.Drawing.Size(100, 28);
			this._cancelButton.TabIndex = 7;
			this._cancelButton.Text = "&Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// MemoryExportDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(275, 145);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._programCounterUpDown);
			this.Controls.Add(this._toAddressUpDown);
			this.Controls.Add(this._fromAddressUpDown);
			this.Controls.Add(this._programCounterLabel);
			this.Controls.Add(this._toAddressLabel);
			this.Controls.Add(this._fromAddressLabel);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(293, 192);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(293, 192);
			this.Name = "MemoryExportDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Memory export";
			((System.ComponentModel.ISupportInitialize)(this._fromAddressUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._toAddressUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._programCounterUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _fromAddressLabel;
		private System.Windows.Forms.NumericUpDown _fromAddressUpDown;
		private System.Windows.Forms.Label _toAddressLabel;
		private System.Windows.Forms.NumericUpDown _toAddressUpDown;
		private System.Windows.Forms.Label _programCounterLabel;
		private System.Windows.Forms.NumericUpDown _programCounterUpDown;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
	}
}
