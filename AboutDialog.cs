// AboutDialog.cs

using System.Windows.Forms;

namespace PokeFontDS
{
    #region AboutDialog Class

    public class AboutDialog : Form
    {
        #region Data Members

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label m_lblAbout;
        private Button m_btnOK;

        #endregion

        #region Ctor Dtor

        public AboutDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void m_btnOK_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_lblAbout = new System.Windows.Forms.Label();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lblAbout
            // 
            this.m_lblAbout.AutoSize = true;
            this.m_lblAbout.Location = new System.Drawing.Point(12, 9);
            this.m_lblAbout.Name = "m_lblAbout";
            this.m_lblAbout.Size = new System.Drawing.Size(252, 52);
            this.m_lblAbout.TabIndex = 1;
            this.m_lblAbout.Text = "PokeFontDS version 3.0 beta (2.3.0.0)\r\nOriginal software by Evyatar Cohen (evco1)" +
    "\r\nSmall improvements by CHEMI6DER\r\nPokemon D/P/Pt/HG/SS/B/W/B2/W2 Font Editor\r\n";
            this.m_lblAbout.Click += new System.EventHandler(this.m_lblAbout_Click);
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnOK.Location = new System.Drawing.Point(104, 64);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(74, 23);
            this.m_btnOK.TabIndex = 2;
            this.m_btnOK.Text = "OK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            this.m_btnOK.Click += new System.EventHandler(this.m_btnOK_Click);
            // 
            // AboutDialog
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnOK;
            this.ClientSize = new System.Drawing.Size(277, 91);
            this.ControlBox = false;
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_lblAbout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AboutDialog";
            this.Text = "About PokeFontDS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Other Methods

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

        #endregion

        private void m_lblAbout_Click(object sender, System.EventArgs e)
        {

        }
    }

    #endregion
}
