using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace MixGui.Components
{
	public class AboutForm : Form
	{
		private Button _closeButton;
		private Label _copyrightLabel;
		private Label _fullVersionLabel;
		private PictureBox _logoBox;
		private Label _productAndVersionLabel;
		private readonly string _versionString;

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

			_productAndVersionLabel.Text = product;
			_versionString = version.ToString();
			_fullVersionLabel.Text += _versionString;
			_copyrightLabel.Text = copyright;
		}

		private void FullVersionLabel_Click(object sender, EventArgs e)
			=> Clipboard.SetText(_versionString);

		private void FullVersionLabel_DoubleClick(object sender, EventArgs e)
			=> Clipboard.SetText(_versionString.Replace('.', '_'));

		private void InitializeComponent()
		{
			var resources = new ComponentResourceManager(typeof(AboutForm));
			_logoBox = new PictureBox();
			_copyrightLabel = new Label();
			_productAndVersionLabel = new Label();
			_closeButton = new Button();
			_fullVersionLabel = new Label();
			((ISupportInitialize)(_logoBox)).BeginInit();
			SuspendLayout();
			// 
			// mLogoBox
			// 
			_logoBox.BorderStyle = BorderStyle.FixedSingle;
			_logoBox.Image = Properties.Resources.AboutFormLogoBoxImage;
			_logoBox.Location = new System.Drawing.Point(68, 104);
			_logoBox.Name = "mLogoBox";
			_logoBox.Size = new System.Drawing.Size(135, 135);
			_logoBox.TabIndex = 0;
			_logoBox.TabStop = false;
			// 
			// mCopyrightLabel
			// 
			_copyrightLabel.Location = new System.Drawing.Point(0, 72);
			_copyrightLabel.Name = "mCopyrightLabel";
			_copyrightLabel.Size = new System.Drawing.Size(264, 16);
			_copyrightLabel.TabIndex = 2;
			_copyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mProductAndVersionLabel
			// 
			_productAndVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			_productAndVersionLabel.Location = new System.Drawing.Point(0, 8);
			_productAndVersionLabel.Name = "mProductAndVersionLabel";
			_productAndVersionLabel.Size = new System.Drawing.Size(264, 32);
			_productAndVersionLabel.TabIndex = 0;
			_productAndVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mCloseButton
			// 
			_closeButton.DialogResult = DialogResult.OK;
			_closeButton.Location = new System.Drawing.Point(99, 256);
			_closeButton.Name = "mCloseButton";
			_closeButton.Size = new System.Drawing.Size(74, 23);
			_closeButton.TabIndex = 3;
			_closeButton.FlatStyle = FlatStyle.Flat;
			_closeButton.Text = "&Close";
			// 
			// mFullVersionLabel
			// 
			_fullVersionLabel.Location = new System.Drawing.Point(0, 48);
			_fullVersionLabel.Name = "mFullVersionLabel";
			_fullVersionLabel.Size = new System.Drawing.Size(264, 16);
			_fullVersionLabel.TabIndex = 1;
			_fullVersionLabel.Text = "Full version number: ";
			_fullVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			_fullVersionLabel.Click += FullVersionLabel_Click;
			_fullVersionLabel.DoubleClick += FullVersionLabel_DoubleClick;
			// 
			// AboutForm
			// 
			ClientSize = new System.Drawing.Size(264, 285);
			Controls.Add(_fullVersionLabel);
			Controls.Add(_closeButton);
			Controls.Add(_copyrightLabel);
			Controls.Add(_productAndVersionLabel);
			Controls.Add(_logoBox);
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
			((ISupportInitialize)(_logoBox)).EndInit();
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
