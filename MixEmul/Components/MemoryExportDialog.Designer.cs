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
			if (disposing && (components != null))
			{
				components.Dispose();
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
			this.mFromAddressLabel = new System.Windows.Forms.Label();
			this.mFromAddressUpDown = new System.Windows.Forms.NumericUpDown();
			this.mToAddressLabel = new System.Windows.Forms.Label();
			this.mToAddressUpDown = new System.Windows.Forms.NumericUpDown();
			this.mProgramCounterLabel = new System.Windows.Forms.Label();
			this.mProgramCounterUpDown = new System.Windows.Forms.NumericUpDown();
			this.mOkButton = new System.Windows.Forms.Button();
			this.mCancelButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.mFromAddressUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mToAddressUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mProgramCounterUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// mFromAddressLabel
			// 
			this.mFromAddressLabel.AutoSize = true;
			this.mFromAddressLabel.Location = new System.Drawing.Point(12, 13);
			this.mFromAddressLabel.Name = "mFromAddressLabel";
			this.mFromAddressLabel.Size = new System.Drawing.Size(113, 13);
			this.mFromAddressLabel.TabIndex = 0;
			this.mFromAddressLabel.Text = "First exported address:";
			// 
			// mFromAddressUpDown
			// 
			this.mFromAddressUpDown.Location = new System.Drawing.Point(136, 11);
			this.mFromAddressUpDown.Name = "mFromAddressUpDown";
			this.mFromAddressUpDown.Size = new System.Drawing.Size(59, 20);
			this.mFromAddressUpDown.TabIndex = 1;
			this.mFromAddressUpDown.ValueChanged += new System.EventHandler(this.mFromAddressUpDown_ValueChanged);
			// 
			// mToAddressLabel
			// 
			this.mToAddressLabel.AutoSize = true;
			this.mToAddressLabel.Location = new System.Drawing.Point(12, 39);
			this.mToAddressLabel.Name = "mToAddressLabel";
			this.mToAddressLabel.Size = new System.Drawing.Size(114, 13);
			this.mToAddressLabel.TabIndex = 2;
			this.mToAddressLabel.Text = "Last exported address:";
			// 
			// mToAddressUpDown
			// 
			this.mToAddressUpDown.Location = new System.Drawing.Point(136, 37);
			this.mToAddressUpDown.Name = "mToAddressUpDown";
			this.mToAddressUpDown.Size = new System.Drawing.Size(59, 20);
			this.mToAddressUpDown.TabIndex = 3;
			this.mToAddressUpDown.ValueChanged += new System.EventHandler(this.mToAddressUpDown_ValueChanged);
			// 
			// mProgramCounterLabel
			// 
			this.mProgramCounterLabel.AutoSize = true;
			this.mProgramCounterLabel.Location = new System.Drawing.Point(12, 65);
			this.mProgramCounterLabel.Name = "mProgramCounterLabel";
			this.mProgramCounterLabel.Size = new System.Drawing.Size(118, 13);
			this.mProgramCounterLabel.TabIndex = 4;
			this.mProgramCounterLabel.Text = "Set program counter to:";
			// 
			// mProgramCounterUpDown
			// 
			this.mProgramCounterUpDown.Location = new System.Drawing.Point(136, 63);
			this.mProgramCounterUpDown.Name = "mProgramCounterUpDown";
			this.mProgramCounterUpDown.Size = new System.Drawing.Size(59, 20);
			this.mProgramCounterUpDown.TabIndex = 5;
			// 
			// mOkButton
			// 
			this.mOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.mOkButton.Location = new System.Drawing.Point(40, 92);
			this.mOkButton.Name = "mOkButton";
			this.mOkButton.Size = new System.Drawing.Size(75, 23);
			this.mOkButton.TabIndex = 6;
			this.mOkButton.Text = "&OK";
			this.mOkButton.UseVisualStyleBackColor = true;
			// 
			// mCancelButton
			// 
			this.mCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.mCancelButton.Location = new System.Drawing.Point(121, 92);
			this.mCancelButton.Name = "mCancelButton";
			this.mCancelButton.Size = new System.Drawing.Size(75, 23);
			this.mCancelButton.TabIndex = 7;
			this.mCancelButton.Text = "&Cancel";
			this.mCancelButton.UseVisualStyleBackColor = true;
			// 
			// MemoryExportDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(208, 127);
			this.Controls.Add(this.mCancelButton);
			this.Controls.Add(this.mOkButton);
			this.Controls.Add(this.mProgramCounterUpDown);
			this.Controls.Add(this.mToAddressUpDown);
			this.Controls.Add(this.mFromAddressUpDown);
			this.Controls.Add(this.mProgramCounterLabel);
			this.Controls.Add(this.mToAddressLabel);
			this.Controls.Add(this.mFromAddressLabel);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(224, 165);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(224, 165);
			this.Name = "MemoryExportDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Memory export";
			((System.ComponentModel.ISupportInitialize)(this.mFromAddressUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mToAddressUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mProgramCounterUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label mFromAddressLabel;
		private System.Windows.Forms.NumericUpDown mFromAddressUpDown;
		private System.Windows.Forms.Label mToAddressLabel;
		private System.Windows.Forms.NumericUpDown mToAddressUpDown;
		private System.Windows.Forms.Label mProgramCounterLabel;
		private System.Windows.Forms.NumericUpDown mProgramCounterUpDown;
		private System.Windows.Forms.Button mOkButton;
		private System.Windows.Forms.Button mCancelButton;
	}
}