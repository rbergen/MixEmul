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
			this.fromAddressLabel = new System.Windows.Forms.Label();
			this.fromAddressUpDown = new System.Windows.Forms.NumericUpDown();
			this.toAddressLabel = new System.Windows.Forms.Label();
			this.toAddressUpDown = new System.Windows.Forms.NumericUpDown();
			this.programCounterLabel = new System.Windows.Forms.Label();
			this.programCounterUpDown = new System.Windows.Forms.NumericUpDown();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.fromAddressUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.toAddressUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.programCounterUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// mFromAddressLabel
			// 
			this.fromAddressLabel.AutoSize = true;
			this.fromAddressLabel.Location = new System.Drawing.Point(16, 16);
			this.fromAddressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.fromAddressLabel.Name = "mFromAddressLabel";
			this.fromAddressLabel.Size = new System.Drawing.Size(153, 17);
			this.fromAddressLabel.TabIndex = 0;
			this.fromAddressLabel.Text = "First exported address:";
			// 
			// mFromAddressUpDown
			// 
			this.fromAddressUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.fromAddressUpDown.Location = new System.Drawing.Point(181, 14);
			this.fromAddressUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.fromAddressUpDown.Name = "mFromAddressUpDown";
			this.fromAddressUpDown.Size = new System.Drawing.Size(79, 22);
			this.fromAddressUpDown.TabIndex = 1;
			this.fromAddressUpDown.ValueChanged += new System.EventHandler(this.FromAddressUpDown_ValueChanged);
			// 
			// mToAddressLabel
			// 
			this.toAddressLabel.AutoSize = true;
			this.toAddressLabel.Location = new System.Drawing.Point(16, 48);
			this.toAddressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.toAddressLabel.Name = "mToAddressLabel";
			this.toAddressLabel.Size = new System.Drawing.Size(153, 17);
			this.toAddressLabel.TabIndex = 2;
			this.toAddressLabel.Text = "Last exported address:";
			// 
			// mToAddressUpDown
			// 
			this.toAddressUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.toAddressUpDown.Location = new System.Drawing.Point(181, 46);
			this.toAddressUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.toAddressUpDown.Name = "mToAddressUpDown";
			this.toAddressUpDown.Size = new System.Drawing.Size(79, 22);
			this.toAddressUpDown.TabIndex = 3;
			this.toAddressUpDown.ValueChanged += new System.EventHandler(this.ToAddressUpDown_ValueChanged);
			// 
			// mProgramCounterLabel
			// 
			this.programCounterLabel.AutoSize = true;
			this.programCounterLabel.Location = new System.Drawing.Point(16, 80);
			this.programCounterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.programCounterLabel.Name = "mProgramCounterLabel";
			this.programCounterLabel.Size = new System.Drawing.Size(158, 17);
			this.programCounterLabel.TabIndex = 4;
			this.programCounterLabel.Text = "Set program counter to:";
			// 
			// mProgramCounterUpDown
			// 
			this.programCounterUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.programCounterUpDown.Location = new System.Drawing.Point(181, 78);
			this.programCounterUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.programCounterUpDown.Name = "mProgramCounterUpDown";
			this.programCounterUpDown.Size = new System.Drawing.Size(79, 22);
			this.programCounterUpDown.TabIndex = 5;
			// 
			// mOkButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.okButton.Location = new System.Drawing.Point(53, 113);
			this.okButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.okButton.Name = "mOkButton";
			this.okButton.Size = new System.Drawing.Size(100, 28);
			this.okButton.TabIndex = 6;
			this.okButton.Text = "&OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// mCancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cancelButton.Location = new System.Drawing.Point(161, 113);
			this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cancelButton.Name = "mCancelButton";
			this.cancelButton.Size = new System.Drawing.Size(100, 28);
			this.cancelButton.TabIndex = 7;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// MemoryExportDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(275, 145);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.programCounterUpDown);
			this.Controls.Add(this.toAddressUpDown);
			this.Controls.Add(this.fromAddressUpDown);
			this.Controls.Add(this.programCounterLabel);
			this.Controls.Add(this.toAddressLabel);
			this.Controls.Add(this.fromAddressLabel);
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
			((System.ComponentModel.ISupportInitialize)(this.fromAddressUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.toAddressUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.programCounterUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label fromAddressLabel;
		private System.Windows.Forms.NumericUpDown fromAddressUpDown;
		private System.Windows.Forms.Label toAddressLabel;
		private System.Windows.Forms.NumericUpDown toAddressUpDown;
		private System.Windows.Forms.Label programCounterLabel;
		private System.Windows.Forms.NumericUpDown programCounterUpDown;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
	}
}
