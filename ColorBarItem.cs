// ColorBarItem.cs

using System;
using System.Windows.Forms;

namespace PokeFontDS
{
    #region ColorBarItem Class

    public class ColorBarItem : UserControl
    {
        #region Data Members

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #endregion

        #region Ctor Dtor

        public ColorBarItem()
        {
            InitializeComponent();
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
            // ColorBarItem
            // 
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Name = "ColorBarItem";
            Size = new System.Drawing.Size(2, 2);
            ResumeLayout(false);

        }

        #endregion

        #region Override Methods

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
    }

    #endregion
}
