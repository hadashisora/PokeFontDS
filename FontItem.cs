// FontItem.cs

using System.Windows.Forms;
using System.Drawing;

namespace PokeFontDS
{
    #region FontItem Class

    public class FontItem : UserControl
    {
        #region Data Members

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Control[,] m_arrctrlPixels;

        #endregion

        #region Ctor Dtor

        public FontItem()
        {
            InitializeComponent();
            m_arrctrlPixels = new Control[16, 16];

            for (int nY = 0; nY < 16; ++nY)
            {
                for (int nX = 0; nX < 16; ++nX)
                {
                    m_arrctrlPixels[nY, nX] = new Panel();
                    m_arrctrlPixels[nY, nX].Size = new Size(15, 15);
                    m_arrctrlPixels[nY, nX].Location = new Point(15 * nX, 15 * nY);
                    Controls.Add(m_arrctrlPixels[nY, nX]);
                }
            }
        }

        #endregion

        #region Properties

        public Control[,] Pixels
        {
            get
            {
                return (m_arrctrlPixels);
            }
        }

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // FontItem
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Name = "FontItem";
            Size = new System.Drawing.Size(242, 242);
            ResumeLayout(false);

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

        public void ShowGrid(bool bShowGrid)
        {
            for (int nY = 0; nY < 16; ++nY)
            {
                for (int nX = 0; nX < 16; ++nX)
                {
                    ((Panel)m_arrctrlPixels[nY, nX]).BorderStyle = bShowGrid ? System.Windows.Forms.BorderStyle.FixedSingle : System.Windows.Forms.BorderStyle.None;
                }
            }
        }

        #endregion
    }

    #endregion
}
