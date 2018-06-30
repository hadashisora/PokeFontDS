using System;
using System.Windows.Forms;
using System.IO;
using NARCFileReadingDLL;
using System.Collections.Generic;
using System.Diagnostics;

namespace PokeFontDS
{
    public class MainDialog : Form
    {
        #region Global variables
        private ListBox NARC_View;
        private ListView Font_View;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ColorDialog m_clrdlgColor;
        private ColorBarItem m_cbiColor1;
        private ColorBarItem m_cbiColor0;
        private ColorBarItem m_cbiColor3;
        private ColorBarItem m_cbiColor2;
        private ColorBarItem m_cbiItems;
        private ColorBarItem m_cbiRight;
        private ColorBarItem m_cbiLeft;
        private FontItem m_fiItem;
        private System.Windows.Forms.NumericUpDown m_nudLetterID;
        private System.Windows.Forms.NumericUpDown m_nudWidth;
        private System.Windows.Forms.Label m_lblWidth;
        private System.Windows.Forms.Label m_lblLetterID;
        private OpenFileDialog m_ofdFile;
        private SaveFileDialog m_sfdFile;
        private Control[] m_arrctrlColors;
        private Control m_ctrlLeft;
        private Control m_ctrlPrevLeft;
        private Control m_ctrlRight;
        private IFontTable m_iftTable;
        private NARCFile m_narcFile;
        private MouseButtons m_mbCurr;
        private int m_nLetterID;
        private int m_nTableID = 0;
        private bool m_bAskSaveItem;
        private bool m_bAskSaveTable;
        private bool m_bAskSaveFile;
        private List<IFontTable> m_lstnFiles;
        private MenuStrip m_msMenu;
        private ToolStripMenuItem m_tsmiFile;
        private ToolStripMenuItem m_tsmiHelp;
        private ToolStripMenuItem m_tsmiLoadFile;
        private ToolStripMenuItem m_tsmiSaveFile;
        private ToolStripMenuItem m_tsmiSaveTable;
        private ToolStripMenuItem m_tsmiExit;
        private ToolStripMenuItem m_tsmiAbout;
        private string m_strNarcFileName;
        private AboutDialog m_aboutAbout;
        private bool m_bEditing;
        private int m_nCopyWidth;
        private ToolStripMenuItem m_tsmiSaveItem;
        private NumericUpDown m_nudSpaceWidth;
        private Label m_lblSpaceWidth;
        private ToolStripMenuItem m_tsmiEdit;
        private ToolStripMenuItem m_tsmiView;
        private ToolStripMenuItem m_tsmiShowGrid;
        private ToolStripMenuItem m_tsmiCopy;
        private ToolStripMenuItem m_tsmiPaste;
        private VALUE[,] m_arrvCopy;
        #endregion

        #region Events
        public MainDialog()
        {
            this.m_bEditing = false;
            this.InitializeComponent();
            this.m_ofdFile = new OpenFileDialog();
            this.m_ofdFile.Filter = "NDS Roms (*.nds)|*.nds";
            this.m_sfdFile = new SaveFileDialog();
            this.m_sfdFile.Filter = "NDS Roms (*.nds)|*.nds";
            this.m_ctrlLeft = this.m_cbiColor0;
            this.m_ctrlRight = this.m_cbiColor1;
            this.m_cbiLeft.BackColor = this.m_cbiColor1.BackColor;
            this.m_cbiRight.BackColor = this.m_cbiColor3.BackColor;
            this.m_arrctrlColors = new Control[] { this.m_cbiColor0, this.m_cbiColor1, this.m_cbiColor2, this.m_cbiColor3 };
            this.m_mbCurr = System.Windows.Forms.MouseButtons.None;
            this.m_lstnFiles = new List<IFontTable>();
            this.m_bAskSaveFile = false;

            foreach (Control ctrlCurr in m_fiItem.Pixels)
            {
                ctrlCurr.MouseDown += new MouseEventHandler(Panel_MouseDown);
                ctrlCurr.MouseUp += new MouseEventHandler(Panel_MouseUp);
                ctrlCurr.MouseMove += new MouseEventHandler(Panel_MouseMove);
                ctrlCurr.Visible = false;
            }

            this.m_aboutAbout = new AboutDialog();
            this.m_bEditing = true;
            this.m_arrvCopy = new VALUE[16, 16];
        }

        public MainDialog(string strFileName)
            : this()
        {
            this.LoadFile(strFileName);
        }

        private void NARC_View_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("NARC_View_SelectedIndexChanged()");
            m_bEditing = false;
            if (m_nTableID != NARC_View.SelectedIndex)
            {
                AskSaveTable();
                m_nTableID = NARC_View.SelectedIndex;
                RefreshTable();
            }
            m_bEditing = true;
        }

        private void m_tsmiLoadFile_Click(object sender, EventArgs e)
        {
            if (this.m_ofdFile.ShowDialog() == DialogResult.OK)
            {
                this.AskSaveFile();
                this.LoadFile(this.m_ofdFile.FileName);
            }
        }

        private void m_tsmiSaveFile_Click(object sender, EventArgs e)
        {
            this.AskSaveTable();
            this.SaveFile();
        }

        private void m_tsmiSaveTable_Click(object sender, EventArgs e)
        {
            this.SaveTable();
        }

        private void m_tsmiSaveItem_Click(object sender, EventArgs e)
        {
            this.SaveItem();
        }

        private void m_tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void m_tsmiCopy_Click(object sender, EventArgs e)
        {
            this.m_nCopyWidth = (int)this.m_nudWidth.Value;

            for (int nY = 0; nY < this.m_iftTable.Height; ++nY)
            {
                for (int nX = 0; nX < this.m_nCopyWidth; ++nX)
                {
                    for (int nIndex = 0; nIndex < 4; ++nIndex)
                    {
                        if (this.m_arrctrlColors[nIndex].BackColor == this.m_fiItem.Pixels[nY, nX].BackColor)
                        {
                            this.m_arrvCopy[nY, nX] = (VALUE)nIndex;
                            break;
                        }
                    }
                }
            }

            this.m_tsmiPaste.Visible = true;
        }

        private void m_tsmiPaste_Click(object sender, EventArgs e)
        {
            if (this.m_nudWidth.Value == this.m_nCopyWidth)
            {
                this.m_bAskSaveItem = true;
                this.m_tsmiSaveItem.Visible = true;
            }
            else
            {
                this.m_nudWidth.Value = this.m_nCopyWidth;
            }

            for (int nY = 0; nY < this.m_iftTable.Height; ++nY)
            {
                for (int nX = 0; nX < this.m_nCopyWidth; ++nX)
                {
                    this.m_fiItem.Pixels[nY, nX].BackColor = this.m_arrctrlColors[(int)this.m_arrvCopy[nY, nX]].BackColor;
                }
            }
        }

        private void m_tsmiShowGrid_Click(object sender, EventArgs e)
        {
            this.m_fiItem.ShowGrid(this.m_tsmiShowGrid.Checked = !this.m_tsmiShowGrid.Checked);
        }

        private void m_tsmiAbout_Click(object sender, EventArgs e)
        {
            this.m_aboutAbout.ShowDialog();
        }

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == this.m_mbCurr)
            {
                this.m_mbCurr = System.Windows.Forms.MouseButtons.None;
            }
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case (MouseButtons.Left):
                    {
                        if (((Control)sender).Enabled)
                        {
                            this.m_bEditing = true;
                            this.m_bAskSaveItem = true;
                            this.m_tsmiSaveItem.Visible = true;
                            ((Control)sender).BackColor = this.m_ctrlLeft.BackColor;
                        }

                        this.m_mbCurr = System.Windows.Forms.MouseButtons.Left;
                        break;
                    }
                case (MouseButtons.Right):
                    {
                        if (((Control)sender).Enabled)
                        {
                            this.m_bEditing = true;
                            this.m_bAskSaveItem = true;
                            this.m_tsmiSaveItem.Visible = true;
                            ((Control)sender).BackColor = this.m_ctrlRight.BackColor;
                        }

                        this.m_mbCurr = System.Windows.Forms.MouseButtons.Right;
                        break;
                    }
                case (MouseButtons.XButton1):
                    {
                        for (int nIndex = 0; nIndex < this.m_arrctrlColors.Length; ++nIndex)
                        {
                            if (this.m_arrctrlColors[nIndex] == m_ctrlLeft)
                            {
                                this.m_ctrlLeft = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length];
                                this.m_cbiLeft.BackColor = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length].BackColor;
                                break;
                            }
                        }

                        break;
                    }
                case (MouseButtons.XButton2):
                    {
                        for (int nIndex = 0; nIndex < this.m_arrctrlColors.Length; ++nIndex)
                        {
                            if (this.m_arrctrlColors[nIndex] == m_ctrlRight)
                            {
                                this.m_ctrlRight = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length];
                                this.m_cbiRight.BackColor = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length].BackColor;
                                break;
                            }
                        }

                        break;
                    }
            }
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            switch (this.m_mbCurr)
            {
                case (MouseButtons.Left):
                    {
                        this.m_bEditing = true;

                        if ((((e.Y + ((Control)sender).Top) / 15) >= 0) &&
                            (((e.Y + ((Control)sender).Top) / 15) < 16) &&
                            (((e.X + ((Control)sender).Left) / 15) >= 0) &&
                            (((e.X + ((Control)sender).Left) / 15) < 16))
                        {
                            this.m_fiItem.Pixels[(e.Y + ((Control)sender).Top) / 15, (e.X + ((Control)sender).Left) / 15].BackColor = m_ctrlLeft.BackColor;
                        }

                        break;
                    }
                case (MouseButtons.Right):
                    {
                        this.m_bEditing = true;

                        if ((((e.Y + ((Control)sender).Top) / 15) >= 0) &&
                            (((e.Y + ((Control)sender).Top) / 15) < 16) &&
                            (((e.X + ((Control)sender).Top) / 15) >= 0) &&
                            (((e.X + ((Control)sender).Top) / 15) < 16))
                        {
                            this.m_fiItem.Pixels[(e.Y + ((Control)sender).Top) / 15, (e.X + ((Control)sender).Left) / 15].BackColor = m_ctrlRight.BackColor;
                        }

                        break;
                    }
            }
        }

        private void ColorBarItem_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case (MouseButtons.Left):
                    {
                        this.m_ctrlPrevLeft = this.m_ctrlLeft;
                        this.m_ctrlLeft = (Control)sender;
                        this.m_cbiLeft.BackColor = ((Control)sender).BackColor;
                        break;
                    }
                case (MouseButtons.Right):
                    {
                        this.m_ctrlRight = (Control)sender;
                        this.m_cbiRight.BackColor = ((Control)sender).BackColor;
                        break;
                    }
                case (MouseButtons.Middle):
                    {
                        ChangeColor((Control)sender);
                        break;
                    }
                case (MouseButtons.XButton1):
                    {
                        for (int nIndex = 0; nIndex < this.m_arrctrlColors.Length; ++nIndex)
                        {
                            if (this.m_arrctrlColors[nIndex] == m_ctrlLeft)
                            {
                                this.m_ctrlLeft = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length];
                                this.m_cbiLeft.BackColor = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length].BackColor;
                                break;
                            }
                        }

                        break;
                    }
                case (MouseButtons.XButton2):
                    {
                        for (int nIndex = 0; nIndex < this.m_arrctrlColors.Length; ++nIndex)
                        {
                            if (this.m_arrctrlColors[nIndex] == m_ctrlRight)
                            {
                                this.m_ctrlRight = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length];
                                this.m_cbiRight.BackColor = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length].BackColor;
                                break;
                            }
                        }

                        break;
                    }
            }
        }

        private void ColorBarItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.m_ctrlLeft = this.m_ctrlPrevLeft;
                this.m_cbiLeft.BackColor = m_ctrlLeft.BackColor;
                this.ChangeColor((Control)sender);
            }
        }

        private void m_nudLetterID_ValueChanged(object sender, EventArgs e)
        {
            this.m_bEditing = false;
            this.AskSaveItem();
            this.m_nLetterID = (int)this.m_nudLetterID.Value;
            this.RefreshItem();
            this.m_bEditing = true;
        }

        private void m_nudWidth_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_bEditing)
            {
                this.m_bAskSaveItem = true;
                this.m_tsmiSaveItem.Visible = true;
            }

            this.RefreshWidth();
            this.m_bEditing = true;
        }

        private void m_nudSpaceWidth_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_bEditing)
            {
                this.m_bAskSaveItem = true;
                this.m_tsmiSaveItem.Visible = true;
            }

            if (this.m_iftTable.Items[this.m_nLetterID] is IContainsSpaceWidth)
            {
                ((IContainsSpaceWidth)this.m_iftTable.Items[this.m_nLetterID]).SpaceWidth = (byte)this.m_nudSpaceWidth.Value;
            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case (MouseButtons.XButton1):
                    {
                        for (int nIndex = 0; nIndex < this.m_arrctrlColors.Length; ++nIndex)
                        {
                            if (this.m_arrctrlColors[nIndex] == m_ctrlLeft)
                            {
                                this.m_ctrlLeft = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length];
                                this.m_cbiLeft.BackColor = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length].BackColor;
                                break;
                            }
                        }

                        break;
                    }
                case (MouseButtons.XButton2):
                    {
                        for (int nIndex = 0; nIndex < this.m_arrctrlColors.Length; ++nIndex)
                        {
                            if (this.m_arrctrlColors[nIndex] == m_ctrlRight)
                            {
                                this.m_ctrlRight = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length];
                                this.m_cbiRight.BackColor = this.m_arrctrlColors[(nIndex + 1) % this.m_arrctrlColors.Length].BackColor;
                                break;
                            }
                        }

                        break;
                    }
            }
        }
        #endregion

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDialog));
            this.m_clrdlgColor = new System.Windows.Forms.ColorDialog();
            this.m_nudLetterID = new System.Windows.Forms.NumericUpDown();
            this.m_nudWidth = new System.Windows.Forms.NumericUpDown();
            this.m_lblWidth = new System.Windows.Forms.Label();
            this.m_lblLetterID = new System.Windows.Forms.Label();
            this.m_msMenu = new System.Windows.Forms.MenuStrip();
            this.m_tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiLoadFile = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiSaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiSaveTable = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiSaveItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiView = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiShowGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.m_nudSpaceWidth = new System.Windows.Forms.NumericUpDown();
            this.m_lblSpaceWidth = new System.Windows.Forms.Label();
            this.Font_View = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NARC_View = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.m_fiItem = new PokeFontDS.FontItem();
            this.m_cbiLeft = new PokeFontDS.ColorBarItem();
            this.m_cbiRight = new PokeFontDS.ColorBarItem();
            this.m_cbiItems = new PokeFontDS.ColorBarItem();
            this.m_cbiColor2 = new PokeFontDS.ColorBarItem();
            this.m_cbiColor3 = new PokeFontDS.ColorBarItem();
            this.m_cbiColor0 = new PokeFontDS.ColorBarItem();
            this.m_cbiColor1 = new PokeFontDS.ColorBarItem();
            ((System.ComponentModel.ISupportInitialize)(this.m_nudLetterID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_nudWidth)).BeginInit();
            this.m_msMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_nudSpaceWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // m_nudLetterID
            // 
            this.m_nudLetterID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_nudLetterID.Enabled = false;
            this.m_nudLetterID.Location = new System.Drawing.Point(618, 80);
            this.m_nudLetterID.Name = "m_nudLetterID";
            this.m_nudLetterID.Size = new System.Drawing.Size(45, 20);
            this.m_nudLetterID.TabIndex = 15;
            this.m_nudLetterID.ValueChanged += new System.EventHandler(this.m_nudLetterID_ValueChanged);
            this.m_nudLetterID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_nudWidth
            // 
            this.m_nudWidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_nudWidth.Enabled = false;
            this.m_nudWidth.Location = new System.Drawing.Point(611, 104);
            this.m_nudWidth.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.m_nudWidth.Name = "m_nudWidth";
            this.m_nudWidth.Size = new System.Drawing.Size(33, 20);
            this.m_nudWidth.TabIndex = 9;
            this.m_nudWidth.ValueChanged += new System.EventHandler(this.m_nudWidth_ValueChanged);
            this.m_nudWidth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_lblWidth
            // 
            this.m_lblWidth.AutoSize = true;
            this.m_lblWidth.Location = new System.Drawing.Point(567, 106);
            this.m_lblWidth.Name = "m_lblWidth";
            this.m_lblWidth.Size = new System.Drawing.Size(38, 13);
            this.m_lblWidth.TabIndex = 8;
            this.m_lblWidth.Text = "Width:";
            this.m_lblWidth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_lblLetterID
            // 
            this.m_lblLetterID.AutoSize = true;
            this.m_lblLetterID.Location = new System.Drawing.Point(567, 84);
            this.m_lblLetterID.Name = "m_lblLetterID";
            this.m_lblLetterID.Size = new System.Drawing.Size(42, 13);
            this.m_lblLetterID.TabIndex = 14;
            this.m_lblLetterID.Text = "Char #:";
            this.m_lblLetterID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_msMenu
            // 
            this.m_msMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiFile,
            this.m_tsmiEdit,
            this.m_tsmiView,
            this.m_tsmiHelp});
            this.m_msMenu.Location = new System.Drawing.Point(0, 0);
            this.m_msMenu.Name = "m_msMenu";
            this.m_msMenu.Size = new System.Drawing.Size(691, 24);
            this.m_msMenu.TabIndex = 18;
            this.m_msMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiFile
            // 
            this.m_tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiLoadFile,
            this.m_tsmiSaveFile,
            this.m_tsmiSaveTable,
            this.m_tsmiSaveItem,
            this.m_tsmiExit});
            this.m_tsmiFile.Name = "m_tsmiFile";
            this.m_tsmiFile.Size = new System.Drawing.Size(37, 20);
            this.m_tsmiFile.Text = "&File";
            this.m_tsmiFile.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiLoadFile
            // 
            this.m_tsmiLoadFile.Name = "m_tsmiLoadFile";
            this.m_tsmiLoadFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.m_tsmiLoadFile.Size = new System.Drawing.Size(170, 22);
            this.m_tsmiLoadFile.Text = "&Load File...";
            this.m_tsmiLoadFile.Click += new System.EventHandler(this.m_tsmiLoadFile_Click);
            this.m_tsmiLoadFile.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiSaveFile
            // 
            this.m_tsmiSaveFile.Name = "m_tsmiSaveFile";
            this.m_tsmiSaveFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.m_tsmiSaveFile.Size = new System.Drawing.Size(170, 22);
            this.m_tsmiSaveFile.Text = "&Save File...";
            this.m_tsmiSaveFile.Visible = false;
            this.m_tsmiSaveFile.Click += new System.EventHandler(this.m_tsmiSaveFile_Click);
            this.m_tsmiSaveFile.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiSaveTable
            // 
            this.m_tsmiSaveTable.Name = "m_tsmiSaveTable";
            this.m_tsmiSaveTable.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.m_tsmiSaveTable.Size = new System.Drawing.Size(170, 22);
            this.m_tsmiSaveTable.Text = "Save &Table";
            this.m_tsmiSaveTable.Visible = false;
            this.m_tsmiSaveTable.Click += new System.EventHandler(this.m_tsmiSaveTable_Click);
            this.m_tsmiSaveTable.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiSaveItem
            // 
            this.m_tsmiSaveItem.Name = "m_tsmiSaveItem";
            this.m_tsmiSaveItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.m_tsmiSaveItem.Size = new System.Drawing.Size(170, 22);
            this.m_tsmiSaveItem.Text = "Save &Item";
            this.m_tsmiSaveItem.Visible = false;
            this.m_tsmiSaveItem.Click += new System.EventHandler(this.m_tsmiSaveItem_Click);
            this.m_tsmiSaveItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiExit
            // 
            this.m_tsmiExit.Name = "m_tsmiExit";
            this.m_tsmiExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.m_tsmiExit.Size = new System.Drawing.Size(170, 22);
            this.m_tsmiExit.Text = "&Exit";
            this.m_tsmiExit.Click += new System.EventHandler(this.m_tsmiExit_Click);
            this.m_tsmiExit.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiEdit
            // 
            this.m_tsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiCopy,
            this.m_tsmiPaste});
            this.m_tsmiEdit.Name = "m_tsmiEdit";
            this.m_tsmiEdit.Size = new System.Drawing.Size(39, 20);
            this.m_tsmiEdit.Text = "&Edit";
            this.m_tsmiEdit.Visible = false;
            this.m_tsmiEdit.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiCopy
            // 
            this.m_tsmiCopy.Name = "m_tsmiCopy";
            this.m_tsmiCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.m_tsmiCopy.Size = new System.Drawing.Size(144, 22);
            this.m_tsmiCopy.Text = "&Copy";
            this.m_tsmiCopy.Click += new System.EventHandler(this.m_tsmiCopy_Click);
            this.m_tsmiCopy.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiPaste
            // 
            this.m_tsmiPaste.Name = "m_tsmiPaste";
            this.m_tsmiPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.m_tsmiPaste.Size = new System.Drawing.Size(144, 22);
            this.m_tsmiPaste.Text = "&Paste";
            this.m_tsmiPaste.Visible = false;
            this.m_tsmiPaste.Click += new System.EventHandler(this.m_tsmiPaste_Click);
            this.m_tsmiPaste.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiView
            // 
            this.m_tsmiView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiShowGrid});
            this.m_tsmiView.Name = "m_tsmiView";
            this.m_tsmiView.Size = new System.Drawing.Size(44, 20);
            this.m_tsmiView.Text = "&View";
            this.m_tsmiView.Visible = false;
            this.m_tsmiView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiShowGrid
            // 
            this.m_tsmiShowGrid.Name = "m_tsmiShowGrid";
            this.m_tsmiShowGrid.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.m_tsmiShowGrid.Size = new System.Drawing.Size(170, 22);
            this.m_tsmiShowGrid.Text = "Show &Grid";
            this.m_tsmiShowGrid.Click += new System.EventHandler(this.m_tsmiShowGrid_Click);
            this.m_tsmiShowGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiHelp
            // 
            this.m_tsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiAbout});
            this.m_tsmiHelp.Name = "m_tsmiHelp";
            this.m_tsmiHelp.Size = new System.Drawing.Size(44, 20);
            this.m_tsmiHelp.Text = "&Help";
            this.m_tsmiHelp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_tsmiAbout
            // 
            this.m_tsmiAbout.Name = "m_tsmiAbout";
            this.m_tsmiAbout.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.m_tsmiAbout.Size = new System.Drawing.Size(126, 22);
            this.m_tsmiAbout.Text = "&About";
            this.m_tsmiAbout.Click += new System.EventHandler(this.m_tsmiAbout_Click);
            this.m_tsmiAbout.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_nudSpaceWidth
            // 
            this.m_nudSpaceWidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_nudSpaceWidth.Enabled = false;
            this.m_nudSpaceWidth.Location = new System.Drawing.Point(642, 126);
            this.m_nudSpaceWidth.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.m_nudSpaceWidth.Name = "m_nudSpaceWidth";
            this.m_nudSpaceWidth.Size = new System.Drawing.Size(40, 20);
            this.m_nudSpaceWidth.TabIndex = 13;
            this.m_nudSpaceWidth.ValueChanged += new System.EventHandler(this.m_nudSpaceWidth_ValueChanged);
            this.m_nudSpaceWidth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_lblSpaceWidth
            // 
            this.m_lblSpaceWidth.AutoSize = true;
            this.m_lblSpaceWidth.Location = new System.Drawing.Point(566, 130);
            this.m_lblSpaceWidth.Name = "m_lblSpaceWidth";
            this.m_lblSpaceWidth.Size = new System.Drawing.Size(72, 13);
            this.m_lblSpaceWidth.TabIndex = 12;
            this.m_lblSpaceWidth.Text = "Space Width:";
            this.m_lblSpaceWidth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // Font_View
            // 
            this.Font_View.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Font_View.Enabled = false;
            this.Font_View.HideSelection = false;
            this.Font_View.Location = new System.Drawing.Point(12, 47);
            this.Font_View.Name = "Font_View";
            this.Font_View.ShowGroups = false;
            this.Font_View.Size = new System.Drawing.Size(300, 359);
            this.Font_View.TabIndex = 23;
            this.Font_View.TileSize = new System.Drawing.Size(32, 32);
            this.Font_View.UseCompatibleStateImageBehavior = false;
            this.Font_View.View = System.Windows.Forms.View.Tile;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(316, 297);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "NARC View";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Font View";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(315, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Character View";
            // 
            // NARC_View
            // 
            this.NARC_View.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NARC_View.Enabled = false;
            this.NARC_View.FormattingEnabled = true;
            this.NARC_View.Location = new System.Drawing.Point(315, 313);
            this.NARC_View.Name = "NARC_View";
            this.NARC_View.Size = new System.Drawing.Size(363, 93);
            this.NARC_View.TabIndex = 28;
            this.NARC_View.SelectedIndexChanged += new System.EventHandler(this.NARC_View_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(34, 215);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(256, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Doesn\'t work yet. Use \"Char #\" to select a character";
            // 
            // m_fiItem
            // 
            this.m_fiItem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.m_fiItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_fiItem.Enabled = false;
            this.m_fiItem.Location = new System.Drawing.Point(315, 47);
            this.m_fiItem.Name = "m_fiItem";
            this.m_fiItem.Size = new System.Drawing.Size(245, 245);
            this.m_fiItem.TabIndex = 0;
            this.m_fiItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_cbiLeft
            // 
            this.m_cbiLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_cbiLeft.Enabled = false;
            this.m_cbiLeft.Location = new System.Drawing.Point(573, 51);
            this.m_cbiLeft.Name = "m_cbiLeft";
            this.m_cbiLeft.Size = new System.Drawing.Size(16, 14);
            this.m_cbiLeft.TabIndex = 2;
            this.m_cbiLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_cbiRight
            // 
            this.m_cbiRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_cbiRight.Enabled = false;
            this.m_cbiRight.Location = new System.Drawing.Point(579, 57);
            this.m_cbiRight.Name = "m_cbiRight";
            this.m_cbiRight.Size = new System.Drawing.Size(16, 14);
            this.m_cbiRight.TabIndex = 3;
            this.m_cbiRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_cbiItems
            // 
            this.m_cbiItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_cbiItems.Enabled = false;
            this.m_cbiItems.Location = new System.Drawing.Point(569, 47);
            this.m_cbiItems.Name = "m_cbiItems";
            this.m_cbiItems.Size = new System.Drawing.Size(32, 30);
            this.m_cbiItems.TabIndex = 1;
            this.m_cbiItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_cbiColor2
            // 
            this.m_cbiColor2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.m_cbiColor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_cbiColor2.Enabled = false;
            this.m_cbiColor2.Location = new System.Drawing.Point(604, 62);
            this.m_cbiColor2.Name = "m_cbiColor2";
            this.m_cbiColor2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.m_cbiColor2.Size = new System.Drawing.Size(15, 15);
            this.m_cbiColor2.TabIndex = 5;
            this.m_cbiColor2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorBarItem_MouseClick);
            this.m_cbiColor2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorBarItem_MouseDoubleClick);
            this.m_cbiColor2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_cbiColor3
            // 
            this.m_cbiColor3.BackColor = System.Drawing.Color.Black;
            this.m_cbiColor3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_cbiColor3.Enabled = false;
            this.m_cbiColor3.Location = new System.Drawing.Point(619, 62);
            this.m_cbiColor3.Name = "m_cbiColor3";
            this.m_cbiColor3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.m_cbiColor3.Size = new System.Drawing.Size(15, 15);
            this.m_cbiColor3.TabIndex = 7;
            this.m_cbiColor3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorBarItem_MouseClick);
            this.m_cbiColor3.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorBarItem_MouseDoubleClick);
            this.m_cbiColor3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_cbiColor0
            // 
            this.m_cbiColor0.BackColor = System.Drawing.Color.White;
            this.m_cbiColor0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_cbiColor0.Enabled = false;
            this.m_cbiColor0.Location = new System.Drawing.Point(604, 47);
            this.m_cbiColor0.Name = "m_cbiColor0";
            this.m_cbiColor0.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.m_cbiColor0.Size = new System.Drawing.Size(15, 15);
            this.m_cbiColor0.TabIndex = 4;
            this.m_cbiColor0.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorBarItem_MouseClick);
            this.m_cbiColor0.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorBarItem_MouseDoubleClick);
            this.m_cbiColor0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // m_cbiColor1
            // 
            this.m_cbiColor1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.m_cbiColor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_cbiColor1.Enabled = false;
            this.m_cbiColor1.Location = new System.Drawing.Point(619, 47);
            this.m_cbiColor1.Name = "m_cbiColor1";
            this.m_cbiColor1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.m_cbiColor1.Size = new System.Drawing.Size(15, 15);
            this.m_cbiColor1.TabIndex = 6;
            this.m_cbiColor1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorBarItem_MouseClick);
            this.m_cbiColor1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorBarItem_MouseDoubleClick);
            this.m_cbiColor1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            // 
            // MainDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 412);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NARC_View);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Font_View);
            this.Controls.Add(this.m_lblSpaceWidth);
            this.Controls.Add(this.m_nudSpaceWidth);
            this.Controls.Add(this.m_lblLetterID);
            this.Controls.Add(this.m_lblWidth);
            this.Controls.Add(this.m_nudWidth);
            this.Controls.Add(this.m_nudLetterID);
            this.Controls.Add(this.m_fiItem);
            this.Controls.Add(this.m_cbiLeft);
            this.Controls.Add(this.m_cbiRight);
            this.Controls.Add(this.m_cbiItems);
            this.Controls.Add(this.m_cbiColor2);
            this.Controls.Add(this.m_cbiColor3);
            this.Controls.Add(this.m_cbiColor0);
            this.Controls.Add(this.m_cbiColor1);
            this.Controls.Add(this.m_msMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.m_msMenu;
            this.MaximizeBox = false;
            this.Name = "MainDialog";
            this.Text = "PokeFontDS";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.m_nudLetterID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_nudWidth)).EndInit();
            this.m_msMenu.ResumeLayout(false);
            this.m_msMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_nudSpaceWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #region Other functions
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainDialog main = null;

            if (args.Length > 1)
            {
                Process.GetCurrentProcess().Kill();
            }
            else if (args.Length == 1)
            {
                main = new MainDialog(args[0]);
            }
            else
            {
                main = new MainDialog();
            }

            Application.Run(main);
        }

        protected override void Dispose(bool disposing)
        {
            AskSaveFile();

            if (Directory.Exists(Application.StartupPath + "\\temp"))
            {
                Directory.Delete(Application.StartupPath + "\\temp", true);
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void LoadFile(string strFileName)
        {
            m_bEditing = false;

            if (Directory.Exists(Application.StartupPath + "\\temp"))
            {
                Directory.Delete(Application.StartupPath + "\\temp", true);
            }
            Directory.CreateDirectory(Application.StartupPath + "\\temp");

            if (!strFileName.Substring(1).StartsWith(":\\"))
            {
                strFileName = Directory.GetCurrentDirectory() + "\\" + strFileName;
            }

            Process ndstool = new Process();
            ndstool.StartInfo.FileName = Application.StartupPath + "\\ndstool.exe";
            ndstool.StartInfo.Arguments = "-x \"" + strFileName + "\" -9 arm9.bin -7 arm7.bin -y9 overarm9.bin -y7 overarm7.bin -d root -y overlay -t banner.bin -h header.bin";
            ndstool.StartInfo.WorkingDirectory = Application.StartupPath + "\\temp";
            ndstool.StartInfo.CreateNoWindow = true;
            ndstool.Start();
            while (!ndstool.HasExited)
            {
                Refresh();
            }

            BinaryReader brrHeader = new BinaryReader(File.OpenRead(Application.StartupPath + @"\temp\Header.bin"));
            string strName = new string(brrHeader.ReadChars(12));
            brrHeader.Close();
            while (strName.EndsWith("\0") || strName.EndsWith(" "))
            {
                strName = strName.Remove(strName.Length - 1);
            }
            Type type = Type.GetType("PokeFontDS." + strName.Replace(' ', '_') + "FontTableNARCLoader");
            if (type == null)
            {
                throw new FormatException();
            }
            FontTableNARCLoaderBase ftnlbLoader = (FontTableNARCLoaderBase)Activator.CreateInstance(type);
            m_strNarcFileName = Application.StartupPath + @"\temp\root\" + ftnlbLoader.FileName;
            m_narcFile = ftnlbLoader.NARCFile;
            m_lstnFiles.Clear();

            int temp = 0;
            foreach (FIMGFrame.FileImageEntryBase fiebCurr in m_narcFile.Files)
            {
                if (fiebCurr is IFontTable)
                {
                    m_lstnFiles.Add((IFontTable)fiebCurr);
                    NARC_View.Items.Add("font_" + temp);
                    temp++;
                }
            }

            m_bAskSaveTable = false;
            m_bAskSaveItem = false;
            m_tsmiSaveTable.Visible = false;
            m_tsmiSaveItem.Visible = false;
            m_bAskSaveFile = false;
            NARC_View.SelectedIndex = 0;
            RefreshTable();
            m_bAskSaveFile = false;

            m_tsmiSaveFile.Visible = true;
            m_tsmiEdit.Visible = true;
            m_tsmiView.Visible = true;
            m_bEditing = true;
        }

        private void ChangeColor(Control ctrlControl)
        {
            bool bDone = false;

            while (!bDone)
            {
                m_clrdlgColor.Color = ctrlControl.BackColor;
                switch (m_clrdlgColor.ShowDialog())
                {
                    case (DialogResult.Cancel):
                        {
                            bDone = true;
                            break;
                        }
                    case (DialogResult.OK):
                        {
                            bDone = true;

                            foreach (Control ctrlCurr in m_arrctrlColors)
                            {
                                if ((ctrlControl != ctrlCurr) && (m_clrdlgColor.Color == ctrlCurr.BackColor))
                                {
                                    MessageBox.Show("This color is already exist!\nPlease choose another color.", "Color Already Exist", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                    bDone = false;
                                    break;
                                }
                            }

                            if (bDone)
                            {
                                foreach (Control ctrlCurr in m_fiItem.Pixels)
                                {
                                    if (ctrlCurr.BackColor == ctrlControl.BackColor)
                                    {
                                        ctrlCurr.BackColor = m_clrdlgColor.Color;
                                    }
                                }

                                ctrlControl.BackColor = m_clrdlgColor.Color;

                                if (ctrlControl == m_ctrlLeft)
                                {
                                    m_cbiLeft.BackColor = ctrlControl.BackColor;
                                }
                                else if (ctrlControl == m_ctrlRight)
                                {
                                    m_cbiRight.BackColor = ctrlControl.BackColor;
                                }
                            }

                            break;
                        }
                }
            }
        }

        private void RefreshWidth()
        {
            for (int nY = 0; nY < this.m_iftTable.Height; ++nY)
            {
                for (int nX = 0; nX < (int)this.m_nudWidth.Value; ++nX)
                {
                    if (!this.m_fiItem.Pixels[nY, nX].Visible || !this.m_bAskSaveItem)
                    {
                        this.m_fiItem.Pixels[nY, nX].BackColor = this.m_arrctrlColors[(int)this.m_iftTable.Items[this.m_nLetterID].Item[nY, nX]].BackColor;
                        this.m_fiItem.Pixels[nY, nX].Visible = true;
                        this.m_fiItem.Pixels[nY, nX].Enabled = true;
                    }
                }

                for (int nX = (int)this.m_nudWidth.Value; nX < 16; ++nX)
                {
                    this.m_fiItem.Pixels[nY, nX].Enabled = false;
                    this.m_fiItem.Pixels[nY, nX].Visible = false;
                }
            }

            for (int nY = this.m_iftTable.Height; nY < 16; ++nY)
            {
                for (int nX = 0; nX < 16; ++nX)
                {
                    this.m_fiItem.Pixels[nY, nX].Enabled = false;
                    this.m_fiItem.Pixels[nY, nX].Visible = false;
                }
            }
        }

        private void RefreshItem()
        {
            if (this.m_iftTable.Items[this.m_nLetterID] is IContainsSpaceWidth)
            {
                this.m_nudSpaceWidth.Value = ((IContainsSpaceWidth)this.m_iftTable.Items[this.m_nLetterID]).SpaceWidth;
            }

            if (this.m_nudWidth.Value == this.m_iftTable.Items[this.m_nLetterID].Width)
            {
                this.RefreshWidth();
            }
            else
            {
                this.m_nudWidth.Value = this.m_iftTable.Items[this.m_nLetterID].Width;
            }
        }

        private void RefreshTable()
        {
            ByteArrayStream stream = new ByteArrayStream();
            this.m_lstnFiles[this.m_nTableID].WriteTo(new BinaryWriter(stream));
            stream.Position = 0;
            this.m_iftTable = (IFontTable)Activator.CreateInstance(this.m_lstnFiles[this.m_nTableID].GetType(), new BinaryReader(stream));
            this.m_nudWidth.Maximum = this.m_iftTable.MaxWidth;
            this.m_nudLetterID.Maximum = this.m_iftTable.Items.Length - 1;

            if (stream.Position != stream.Length)
            {
                throw new FormatException();
            }

            stream.Close();
            this.m_bAskSaveItem = false;

            if (this.m_nudLetterID.Value == 0)
            {
                this.RefreshItem();
            }
            else
            {
                this.m_nudLetterID.Value = 0;
            }

            foreach (Control ctrlCurr in this.Controls)
            {
                ctrlCurr.Enabled = true;
            }

            if (!(this.m_iftTable.Items[0] is IContainsSpaceWidth))
            {
                this.m_nudSpaceWidth.Enabled = false;
                this.m_nudSpaceWidth.Value = 0;
            }

            this.m_tsmiSaveTable.Visible = false;
            this.m_bAskSaveTable = false;
            this.m_tsmiSaveItem.Visible = false;
        }

        private void AskSaveItem()
        {
            if (this.m_bAskSaveItem)
            {
                if (MessageBox.Show("The current item has been changed.\nDo you want to save the changes?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.SaveItem();
                }
                else
                {
                    this.m_bAskSaveItem = false;
                    this.m_tsmiSaveItem.Visible = false;
                }
            }
        }

        private void SaveItem()
        {
            if (this.m_bAskSaveItem)
            {
                VALUE[,] values = new VALUE[this.m_iftTable.MaxHeight, this.m_iftTable.MaxWidth];

                for (int nY = 0; nY < this.m_iftTable.Height; ++nY)
                {
                    for (int nX = 0; nX < this.m_nudWidth.Value; ++nX)
                    {
                        for (int nIndex = 0; nIndex < this.m_arrctrlColors.Length; ++nIndex)
                        {
                            if (this.m_arrctrlColors[nIndex].BackColor == this.m_fiItem.Pixels[nY, nX].BackColor)
                            {
                                values[nY, nX] = (VALUE)nIndex;
                                break;
                            }
                        }
                    }
                }

                this.m_iftTable.Items[this.m_nLetterID].Item = values;
                this.m_iftTable.Items[this.m_nLetterID].Width = (byte)this.m_nudWidth.Value;
                this.m_bAskSaveTable = true;
                this.m_bAskSaveItem = false;
                this.m_tsmiSaveTable.Visible = true;
                this.m_tsmiSaveItem.Visible = false;
            }
        }

        private void AskSaveTable()
        {
            this.AskSaveItem();

            if (this.m_bAskSaveTable)
            {
                if (MessageBox.Show("The current table has been changed.\nDo you want to save the changes?", "Table", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.SaveTable();
                }

                this.m_bAskSaveTable = false;
                this.m_bAskSaveItem = false;
                this.m_tsmiSaveTable.Visible = false;
                this.m_tsmiSaveItem.Visible = false;
            }
        }

        private void SaveTable()
        {
            this.AskSaveItem();
            ByteArrayStream stream = new ByteArrayStream();
            this.m_iftTable.WriteTo(new BinaryWriter(stream));
            stream.Seek(0, SeekOrigin.Begin);
            this.m_lstnFiles[this.m_nTableID] = (IFontTable)Activator.CreateInstance(this.m_lstnFiles[this.m_nTableID].GetType(), new BinaryReader(stream));
            int nActIndex = -1;
            int nIndex;

            for (nIndex = 0; nActIndex != this.m_nTableID; ++nIndex)
            {
                if (this.m_narcFile.Files[nIndex] is IFontTable)
                {
                    nActIndex++;
                }
            }

            this.m_narcFile.Files[nIndex - 1] = (FIMGFrame.FileImageEntryBase)this.m_lstnFiles[this.m_nTableID];
            stream.Close();
            this.m_bAskSaveTable = false;
            this.m_bAskSaveItem = false;
            this.m_tsmiSaveTable.Visible = false;
            this.m_tsmiSaveItem.Visible = false;
            this.m_bAskSaveFile = true;
        }

        private void AskSaveFile()
        {
            this.AskSaveTable();

            if (this.m_bAskSaveFile)
            {
                if (MessageBox.Show("The current file has been changed.\nDo you want to save the changes?", "File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.SaveFile();
                }
            }

            this.m_bAskSaveFile = false;
            this.m_bAskSaveTable = false;
            this.m_bAskSaveItem = false;
            this.m_tsmiSaveTable.Visible = false;
            this.m_tsmiSaveItem.Visible = false;
        }

        private void SaveFile()
        {
            if (m_sfdFile.ShowDialog() == DialogResult.OK)
            {
                FileStream file = File.Create(this.m_strNarcFileName);
                this.m_narcFile.WriteTo(new BinaryWriter(file));
                file.Close();
                Directory.CreateDirectory(Application.StartupPath + "\\temp");
                Process ndstool = new Process();
                ndstool.StartInfo.FileName = Application.StartupPath + "\\ndstool.exe";
                ndstool.StartInfo.Arguments = "-c temp.nds -9 arm9.bin -7 arm7.bin -y9 overarm9.bin -y7 overarm7.bin -d root -y overlay -t banner.bin -h header.bin";
                ndstool.StartInfo.WorkingDirectory = Application.StartupPath + "\\temp";
                ndstool.StartInfo.CreateNoWindow = true;
                ndstool.Start();

                while (!ndstool.HasExited) ;

                if (File.Exists(this.m_sfdFile.FileName))
                {
                    File.Delete(this.m_sfdFile.FileName);
                }

                File.Move(Application.StartupPath + "\\temp\\temp.nds", this.m_sfdFile.FileName);
                this.m_bAskSaveFile = false;
            }
        }
        #endregion
    }
}
