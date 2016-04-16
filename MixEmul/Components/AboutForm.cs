using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class AboutForm : Form
	{
		private Container components = null;
		private Button mCloseButton;
		private Label mCopyrightLabel;
		private Label mFullVersionLabel;
		private PictureBox mLogoBox;
		private Label mProductAndVersionLabel;
		private string mVersionString;

		public AboutForm()
		{
			InitializeComponent();

			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			object[] customAttributes = executingAssembly.GetCustomAttributes(false);

			string product = null;
			string copyright = null;

			foreach (object attribute in customAttributes)
			{
				if (copyright == null && attribute is AssemblyCopyrightAttribute)
				{
					copyright = ((AssemblyCopyrightAttribute)attribute).Copyright;
				}
				else if (product == null && attribute is AssemblyProductAttribute)
				{
					product = ((AssemblyProductAttribute)attribute).Product;
				}

				if (copyright != null && product != null)
				{
					break;
				}
			}

			Version version = executingAssembly.GetName().Version;
			product = product + " " + version.ToString(2);

			mProductAndVersionLabel.Text = product;
			mVersionString = version.ToString();
			mFullVersionLabel.Text = mFullVersionLabel.Text + mVersionString;
			mCopyrightLabel.Text = copyright;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
			this.mLogoBox = new System.Windows.Forms.PictureBox();
			this.mCopyrightLabel = new System.Windows.Forms.Label();
			this.mProductAndVersionLabel = new System.Windows.Forms.Label();
			this.mCloseButton = new System.Windows.Forms.Button();
			this.mFullVersionLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.mLogoBox)).BeginInit();
			this.SuspendLayout();
			// 
			// mLogoBox
			// 
			this.mLogoBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mLogoBox.Image = global::MixGui.Properties.Resources.AboutFormLogoBoxImage;
			this.mLogoBox.Location = new System.Drawing.Point(68, 104);
			this.mLogoBox.Name = "mLogoBox";
			this.mLogoBox.Size = new System.Drawing.Size(135, 135);
			this.mLogoBox.TabIndex = 0;
			this.mLogoBox.TabStop = false;
			// 
			// mCopyrightLabel
			// 
			this.mCopyrightLabel.Location = new System.Drawing.Point(0, 72);
			this.mCopyrightLabel.Name = "mCopyrightLabel";
			this.mCopyrightLabel.Size = new System.Drawing.Size(264, 16);
			this.mCopyrightLabel.TabIndex = 2;
			this.mCopyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mProductAndVersionLabel
			// 
			this.mProductAndVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mProductAndVersionLabel.Location = new System.Drawing.Point(0, 8);
			this.mProductAndVersionLabel.Name = "mProductAndVersionLabel";
			this.mProductAndVersionLabel.Size = new System.Drawing.Size(264, 32);
			this.mProductAndVersionLabel.TabIndex = 0;
			this.mProductAndVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mCloseButton
			// 
			this.mCloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.mCloseButton.Location = new System.Drawing.Point(99, 256);
			this.mCloseButton.Name = "mCloseButton";
			this.mCloseButton.Size = new System.Drawing.Size(74, 23);
			this.mCloseButton.TabIndex = 3;
			this.mCloseButton.Text = "&Close";
			// 
			// mFullVersionLabel
			// 
			this.mFullVersionLabel.Location = new System.Drawing.Point(0, 48);
			this.mFullVersionLabel.Name = "mFullVersionLabel";
			this.mFullVersionLabel.Size = new System.Drawing.Size(264, 16);
			this.mFullVersionLabel.TabIndex = 1;
			this.mFullVersionLabel.Text = "Full version number: ";
			this.mFullVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.mFullVersionLabel.Click += new System.EventHandler(this.mFullVersionLabel_Click);
			this.mFullVersionLabel.DoubleClick += new System.EventHandler(this.mFullVersionLabel_DoubleClick);
			// 
			// AboutForm
			// 
			this.ClientSize = new System.Drawing.Size(264, 285);
			this.Controls.Add(this.mFullVersionLabel);
			this.Controls.Add(this.mCloseButton);
			this.Controls.Add(this.mCopyrightLabel);
			this.Controls.Add(this.mProductAndVersionLabel);
			this.Controls.Add(this.mLogoBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About";
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.this_KeyPress);
			((System.ComponentModel.ISupportInitialize)(this.mLogoBox)).EndInit();
			this.ResumeLayout(false);

		}

		private void this_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Escape)
			{
				base.DialogResult = DialogResult.Cancel;
				base.Hide();
			}
		}

		private void mFullVersionLabel_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(mVersionString);
		}

		private void mFullVersionLabel_DoubleClick(object sender, EventArgs e)
		{
			Clipboard.SetText(mVersionString.Replace('.', '_'));
		}
	}
}
