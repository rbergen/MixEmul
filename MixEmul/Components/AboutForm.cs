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

        private void mFullVersionLabel_Click(object sender, EventArgs e) => Clipboard.SetText(mVersionString);

        private void mFullVersionLabel_DoubleClick(object sender, EventArgs e) => Clipboard.SetText(mVersionString.Replace('.', '_'));

        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				components?.Dispose();
			}

			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            ComponentResourceManager resources = new ComponentResourceManager(typeof(AboutForm));
            mLogoBox = new System.Windows.Forms.PictureBox();
            mCopyrightLabel = new Label();
            mProductAndVersionLabel = new Label();
            mCloseButton = new Button();
            mFullVersionLabel = new Label();
			((System.ComponentModel.ISupportInitialize)(mLogoBox)).BeginInit();
            SuspendLayout();
            // 
            // mLogoBox
            // 
            mLogoBox.BorderStyle = BorderStyle.FixedSingle;
            mLogoBox.Image = global::MixGui.Properties.Resources.AboutFormLogoBoxImage;
            mLogoBox.Location = new System.Drawing.Point(68, 104);
            mLogoBox.Name = "mLogoBox";
            mLogoBox.Size = new System.Drawing.Size(135, 135);
            mLogoBox.TabIndex = 0;
            mLogoBox.TabStop = false;
            // 
            // mCopyrightLabel
            // 
            mCopyrightLabel.Location = new System.Drawing.Point(0, 72);
            mCopyrightLabel.Name = "mCopyrightLabel";
            mCopyrightLabel.Size = new System.Drawing.Size(264, 16);
            mCopyrightLabel.TabIndex = 2;
            mCopyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mProductAndVersionLabel
            // 
            mProductAndVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            mProductAndVersionLabel.Location = new System.Drawing.Point(0, 8);
            mProductAndVersionLabel.Name = "mProductAndVersionLabel";
            mProductAndVersionLabel.Size = new System.Drawing.Size(264, 32);
            mProductAndVersionLabel.TabIndex = 0;
            mProductAndVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mCloseButton
            // 
            mCloseButton.DialogResult = DialogResult.OK;
            mCloseButton.Location = new System.Drawing.Point(99, 256);
            mCloseButton.Name = "mCloseButton";
            mCloseButton.Size = new System.Drawing.Size(74, 23);
            mCloseButton.TabIndex = 3;
            mCloseButton.Text = "&Close";
            // 
            // mFullVersionLabel
            // 
            mFullVersionLabel.Location = new System.Drawing.Point(0, 48);
            mFullVersionLabel.Name = "mFullVersionLabel";
            mFullVersionLabel.Size = new System.Drawing.Size(264, 16);
            mFullVersionLabel.TabIndex = 1;
            mFullVersionLabel.Text = "Full version number: ";
            mFullVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            mFullVersionLabel.Click += new EventHandler(mFullVersionLabel_Click);
            mFullVersionLabel.DoubleClick += new EventHandler(mFullVersionLabel_DoubleClick);
            // 
            // AboutForm
            // 
            ClientSize = new System.Drawing.Size(264, 285);
            Controls.Add(mFullVersionLabel);
            Controls.Add(mCloseButton);
            Controls.Add(mCopyrightLabel);
            Controls.Add(mProductAndVersionLabel);
            Controls.Add(mLogoBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "About";
            KeyPress += new KeyPressEventHandler(this_KeyPress);
			((System.ComponentModel.ISupportInitialize)(mLogoBox)).EndInit();
            ResumeLayout(false);

		}

		private void this_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Escape)
			{
				base.DialogResult = DialogResult.Cancel;
				base.Hide();
			}
		}
	}
}
