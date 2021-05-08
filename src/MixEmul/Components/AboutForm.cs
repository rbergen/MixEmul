using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class AboutForm : Form
	{
		private Button closeButton;
		private Label copyrightLabel;
		private Label fullVersionLabel;
		private PictureBox logoBox;
		private Label productAndVersionLabel;
		private readonly string versionString;

		public AboutForm()
		{
			InitializeComponent();

			var executingAssembly = Assembly.GetExecutingAssembly();
			var customAttributes = executingAssembly.GetCustomAttributes(false);

			string product = null;
			string copyright = null;

			foreach (object attribute in customAttributes)
			{
				if (copyright == null && attribute is AssemblyCopyrightAttribute copyrightAttribute)
					copyright = copyrightAttribute.Copyright;

				else if (product == null && attribute is AssemblyProductAttribute productAttribute)
					product = productAttribute.Product;

				if (copyright != null && product != null)
					break;
			}

			Version version = executingAssembly.GetName().Version;
			product += " " + version.ToString(2);

			this.productAndVersionLabel.Text = product;
			this.versionString = version.ToString();
			this.fullVersionLabel.Text += this.versionString;
			this.copyrightLabel.Text = copyright;
		}

		private void FullVersionLabel_Click(object sender, EventArgs e)
			=> Clipboard.SetText(this.versionString);

		private void FullVersionLabel_DoubleClick(object sender, EventArgs e)
			=> Clipboard.SetText(this.versionString);

		private void InitializeComponent()
		{
			var resources = new ComponentResourceManager(typeof(AboutForm));
			this.logoBox = new PictureBox();
			this.copyrightLabel = new Label();
			this.productAndVersionLabel = new Label();
			this.closeButton = new Button();
			this.fullVersionLabel = new Label();
			((ISupportInitialize)(this.logoBox)).BeginInit();
			SuspendLayout();
			// 
			// mLogoBox
			// 
			this.logoBox.BorderStyle = BorderStyle.FixedSingle;
			this.logoBox.Image = Properties.Resources.AboutFormLogoBoxImage;
			this.logoBox.Location = new System.Drawing.Point(68, 104);
			this.logoBox.Name = "mLogoBox";
			this.logoBox.Size = new System.Drawing.Size(135, 135);
			this.logoBox.TabIndex = 0;
			this.logoBox.TabStop = false;
			// 
			// mCopyrightLabel
			// 
			this.copyrightLabel.Location = new System.Drawing.Point(0, 72);
			this.copyrightLabel.Name = "mCopyrightLabel";
			this.copyrightLabel.Size = new System.Drawing.Size(264, 16);
			this.copyrightLabel.TabIndex = 2;
			this.copyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mProductAndVersionLabel
			// 
			this.productAndVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.productAndVersionLabel.Location = new System.Drawing.Point(0, 8);
			this.productAndVersionLabel.Name = "mProductAndVersionLabel";
			this.productAndVersionLabel.Size = new System.Drawing.Size(264, 32);
			this.productAndVersionLabel.TabIndex = 0;
			this.productAndVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mCloseButton
			// 
			this.closeButton.DialogResult = DialogResult.OK;
			this.closeButton.Location = new System.Drawing.Point(99, 256);
			this.closeButton.Name = "mCloseButton";
			this.closeButton.Size = new System.Drawing.Size(74, 23);
			this.closeButton.TabIndex = 3;
			this.closeButton.FlatStyle = FlatStyle.Flat;
			this.closeButton.Text = "&Close";
			// 
			// mFullVersionLabel
			// 
			this.fullVersionLabel.Location = new System.Drawing.Point(0, 48);
			this.fullVersionLabel.Name = "mFullVersionLabel";
			this.fullVersionLabel.Size = new System.Drawing.Size(264, 16);
			this.fullVersionLabel.TabIndex = 1;
			this.fullVersionLabel.Text = "Full version number: ";
			this.fullVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.fullVersionLabel.Click += FullVersionLabel_Click;
			this.fullVersionLabel.DoubleClick += FullVersionLabel_DoubleClick;
			// 
			// AboutForm
			// 
			ClientSize = new System.Drawing.Size(264, 285);
			Controls.Add(this.fullVersionLabel);
			Controls.Add(this.closeButton);
			Controls.Add(this.copyrightLabel);
			Controls.Add(this.productAndVersionLabel);
			Controls.Add(this.logoBox);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			KeyPreview = true;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "AboutForm";
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "About";
			KeyPress += This_KeyPress;
			((ISupportInitialize)(this.logoBox)).EndInit();
			ResumeLayout(false);

		}

		private void This_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Escape)
			{
				DialogResult = DialogResult.Cancel;
				Hide();
			}
		}
	}
}
