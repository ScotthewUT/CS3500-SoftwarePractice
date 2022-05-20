// AUTHOR:   Scott Crowley (u1178178)
// VERSION:  6 October 2019

namespace SS
{
    partial class SpreadsheetGUI
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
            this.topMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileDropMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editDropMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutEditMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyEditMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteEditMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoEditMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatDropMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeTextFormatMenutItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallSizeTextFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mediumSizeTextFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeSizeTextFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.styleTextFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sansStyleTextFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serifStyleTextFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monoStyleTextFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpDropMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userGuideHelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutHelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectCellStatusBar = new System.Windows.Forms.ToolStrip();
            this.selectCellNameLabel = new System.Windows.Forms.ToolStripLabel();
            this.selectCellNameDisplay = new System.Windows.Forms.ToolStripTextBox();
            this.selectCellContentsLabel = new System.Windows.Forms.ToolStripLabel();
            this.selectCellContentsTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.selectCellValueLabel = new System.Windows.Forms.ToolStripLabel();
            this.selectCellValueDisplay = new System.Windows.Forms.ToolStripTextBox();
            this.spreadsheetPanel = new SS.SpreadsheetPanel();
            this.UserGuide = new System.Windows.Forms.HelpProvider();
            this.topMenuStrip.SuspendLayout();
            this.selectCellStatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // topMenuStrip
            // 
            this.topMenuStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            this.topMenuStrip.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.topMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileDropMenuItem,
            this.editDropMenuItem,
            this.formatDropMenuItem,
            this.helpDropMenuItem});
            this.topMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.topMenuStrip.Name = "topMenuStrip";
            this.topMenuStrip.Size = new System.Drawing.Size(809, 32);
            this.topMenuStrip.TabIndex = 0;
            this.topMenuStrip.Text = "Top Menu";
            // 
            // fileDropMenuItem
            // 
            this.fileDropMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileMenuItem,
            this.openFileMenuItem,
            this.closeFileMenuItem,
            this.saveFileMenuItem,
            this.saveAsFileMenuItem});
            this.fileDropMenuItem.Name = "fileDropMenuItem";
            this.fileDropMenuItem.Size = new System.Drawing.Size(51, 28);
            this.fileDropMenuItem.Text = "&File";
            // 
            // newFileMenuItem
            // 
            this.newFileMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.newFileMenuItem.Name = "newFileMenuItem";
            this.newFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newFileMenuItem.Size = new System.Drawing.Size(244, 26);
            this.newFileMenuItem.Text = "&New";
            this.newFileMenuItem.Click += new System.EventHandler(this.newFileMenuItem_Click);
            // 
            // openFileMenuItem
            // 
            this.openFileMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.openFileMenuItem.Name = "openFileMenuItem";
            this.openFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openFileMenuItem.Size = new System.Drawing.Size(244, 26);
            this.openFileMenuItem.Text = "&Open...";
            this.openFileMenuItem.Click += new System.EventHandler(this.openFileMenuItem_Click);
            // 
            // closeFileMenuItem
            // 
            this.closeFileMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.closeFileMenuItem.Name = "closeFileMenuItem";
            this.closeFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.End)));
            this.closeFileMenuItem.Size = new System.Drawing.Size(244, 26);
            this.closeFileMenuItem.Text = "&Close";
            this.closeFileMenuItem.Click += new System.EventHandler(this.closeFileMenuItem_Click);
            // 
            // saveFileMenuItem
            // 
            this.saveFileMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.saveFileMenuItem.Name = "saveFileMenuItem";
            this.saveFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveFileMenuItem.Size = new System.Drawing.Size(244, 26);
            this.saveFileMenuItem.Text = "&Save";
            this.saveFileMenuItem.Click += new System.EventHandler(this.saveFileMenuItem_Click);
            // 
            // saveAsFileMenuItem
            // 
            this.saveAsFileMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.saveAsFileMenuItem.Name = "saveAsFileMenuItem";
            this.saveAsFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsFileMenuItem.Size = new System.Drawing.Size(244, 26);
            this.saveAsFileMenuItem.Text = "Save &As...";
            this.saveAsFileMenuItem.Click += new System.EventHandler(this.saveAsFileMenuItem_Click);
            // 
            // editDropMenuItem
            // 
            this.editDropMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutEditMenuItem,
            this.copyEditMenuItem,
            this.pasteEditMenuItem,
            this.undoEditMenuItem});
            this.editDropMenuItem.Name = "editDropMenuItem";
            this.editDropMenuItem.Size = new System.Drawing.Size(55, 28);
            this.editDropMenuItem.Text = "&Edit";
            // 
            // cutEditMenuItem
            // 
            this.cutEditMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.cutEditMenuItem.Name = "cutEditMenuItem";
            this.cutEditMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutEditMenuItem.Size = new System.Drawing.Size(175, 26);
            this.cutEditMenuItem.Text = "&Cut";
            this.cutEditMenuItem.Click += new System.EventHandler(this.cutEditMenuItem_Click);
            // 
            // copyEditMenuItem
            // 
            this.copyEditMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.copyEditMenuItem.Name = "copyEditMenuItem";
            this.copyEditMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyEditMenuItem.Size = new System.Drawing.Size(175, 26);
            this.copyEditMenuItem.Text = "&Copy";
            this.copyEditMenuItem.Click += new System.EventHandler(this.copyEditMenuItem_Click);
            // 
            // pasteEditMenuItem
            // 
            this.pasteEditMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.pasteEditMenuItem.Name = "pasteEditMenuItem";
            this.pasteEditMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteEditMenuItem.Size = new System.Drawing.Size(175, 26);
            this.pasteEditMenuItem.Text = "&Paste";
            this.pasteEditMenuItem.Click += new System.EventHandler(this.pasteEditMenuItem_Click);
            // 
            // undoEditMenuItem
            // 
            this.undoEditMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.undoEditMenuItem.Name = "undoEditMenuItem";
            this.undoEditMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoEditMenuItem.Size = new System.Drawing.Size(175, 26);
            this.undoEditMenuItem.Text = "&Undo";
            this.undoEditMenuItem.Click += new System.EventHandler(this.undoEditMenuItem_Click);
            // 
            // formatDropMenuItem
            // 
            this.formatDropMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textFormatMenuItem});
            this.formatDropMenuItem.Name = "formatDropMenuItem";
            this.formatDropMenuItem.Size = new System.Drawing.Size(82, 28);
            this.formatDropMenuItem.Text = "F&ormat";
            // 
            // textFormatMenuItem
            // 
            this.textFormatMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sizeTextFormatMenutItem,
            this.styleTextFormatMenuItem});
            this.textFormatMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.textFormatMenuItem.Name = "textFormatMenuItem";
            this.textFormatMenuItem.Size = new System.Drawing.Size(110, 26);
            this.textFormatMenuItem.Text = "&Text";
            // 
            // sizeTextFormatMenutItem
            // 
            this.sizeTextFormatMenutItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smallSizeTextFormatMenuItem,
            this.mediumSizeTextFormatMenuItem,
            this.largeSizeTextFormatMenuItem});
            this.sizeTextFormatMenutItem.Name = "sizeTextFormatMenutItem";
            this.sizeTextFormatMenutItem.Size = new System.Drawing.Size(115, 26);
            this.sizeTextFormatMenutItem.Text = "&Size";
            // 
            // smallSizeTextFormatMenuItem
            // 
            this.smallSizeTextFormatMenuItem.Font = new System.Drawing.Font("Calibri", 10F);
            this.smallSizeTextFormatMenuItem.Name = "smallSizeTextFormatMenuItem";
            this.smallSizeTextFormatMenuItem.Size = new System.Drawing.Size(144, 34);
            this.smallSizeTextFormatMenuItem.Text = "&Small";
            this.smallSizeTextFormatMenuItem.Click += new System.EventHandler(this.smallSizeTextFormatMenuItem_Click);
            // 
            // mediumSizeTextFormatMenuItem
            // 
            this.mediumSizeTextFormatMenuItem.Font = new System.Drawing.Font("Calibri", 14F);
            this.mediumSizeTextFormatMenuItem.Name = "mediumSizeTextFormatMenuItem";
            this.mediumSizeTextFormatMenuItem.Size = new System.Drawing.Size(144, 34);
            this.mediumSizeTextFormatMenuItem.Text = "&Medium";
            this.mediumSizeTextFormatMenuItem.Click += new System.EventHandler(this.mediumSizeTextFormatMenuItem_Click);
            // 
            // largeSizeTextFormatMenuItem
            // 
            this.largeSizeTextFormatMenuItem.Font = new System.Drawing.Font("Calibri", 18F);
            this.largeSizeTextFormatMenuItem.Name = "largeSizeTextFormatMenuItem";
            this.largeSizeTextFormatMenuItem.Size = new System.Drawing.Size(144, 34);
            this.largeSizeTextFormatMenuItem.Text = "&Large";
            this.largeSizeTextFormatMenuItem.Click += new System.EventHandler(this.largeSizeTextFormatMenuItem_Click);
            // 
            // styleTextFormatMenuItem
            // 
            this.styleTextFormatMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sansStyleTextFormatMenuItem,
            this.serifStyleTextFormatMenuItem,
            this.monoStyleTextFormatMenuItem});
            this.styleTextFormatMenuItem.Name = "styleTextFormatMenuItem";
            this.styleTextFormatMenuItem.Size = new System.Drawing.Size(115, 26);
            this.styleTextFormatMenuItem.Text = "St&yle";
            // 
            // sansStyleTextFormatMenuItem
            // 
            this.sansStyleTextFormatMenuItem.Name = "sansStyleTextFormatMenuItem";
            this.sansStyleTextFormatMenuItem.Size = new System.Drawing.Size(180, 26);
            this.sansStyleTextFormatMenuItem.Text = "&Sans Serif";
            this.sansStyleTextFormatMenuItem.Click += new System.EventHandler(this.sansStyleTextFormatMenuItem_Click);
            // 
            // serifStyleTextFormatMenuItem
            // 
            this.serifStyleTextFormatMenuItem.Font = new System.Drawing.Font("Cambria", 13F);
            this.serifStyleTextFormatMenuItem.Name = "serifStyleTextFormatMenuItem";
            this.serifStyleTextFormatMenuItem.Size = new System.Drawing.Size(180, 26);
            this.serifStyleTextFormatMenuItem.Text = "S&erif";
            this.serifStyleTextFormatMenuItem.Click += new System.EventHandler(this.serifStyleTextFormatMenuItem_Click);
            // 
            // monoStyleTextFormatMenuItem
            // 
            this.monoStyleTextFormatMenuItem.Font = new System.Drawing.Font("Consolas", 13F);
            this.monoStyleTextFormatMenuItem.Name = "monoStyleTextFormatMenuItem";
            this.monoStyleTextFormatMenuItem.Size = new System.Drawing.Size(180, 26);
            this.monoStyleTextFormatMenuItem.Text = "&Monospaced";
            this.monoStyleTextFormatMenuItem.Click += new System.EventHandler(this.monoStyleTextFormatMenuItem_Click);
            // 
            // helpDropMenuItem
            // 
            this.helpDropMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.userGuideHelpMenuItem,
            this.aboutHelpMenuItem});
            this.helpDropMenuItem.Name = "helpDropMenuItem";
            this.helpDropMenuItem.Size = new System.Drawing.Size(60, 28);
            this.helpDropMenuItem.Text = "&Help";
            // 
            // userGuideHelpMenuItem
            // 
            this.userGuideHelpMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.userGuideHelpMenuItem.Name = "userGuideHelpMenuItem";
            this.userGuideHelpMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.userGuideHelpMenuItem.Size = new System.Drawing.Size(187, 26);
            this.userGuideHelpMenuItem.Text = "User &Guide";
            this.userGuideHelpMenuItem.Click += new System.EventHandler(this.userGuideHelpMenuItem_Click);
            // 
            // aboutHelpMenuItem
            // 
            this.aboutHelpMenuItem.Font = new System.Drawing.Font("Calibri", 13F);
            this.aboutHelpMenuItem.Name = "aboutHelpMenuItem";
            this.aboutHelpMenuItem.Size = new System.Drawing.Size(187, 26);
            this.aboutHelpMenuItem.Text = "&About";
            this.aboutHelpMenuItem.Click += new System.EventHandler(this.aboutHelpMenuItem_Click);
            // 
            // selectCellStatusBar
            // 
            this.selectCellStatusBar.AutoSize = false;
            this.selectCellStatusBar.BackColor = System.Drawing.SystemColors.ControlDark;
            this.selectCellStatusBar.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectCellStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectCellNameLabel,
            this.selectCellNameDisplay,
            this.selectCellContentsLabel,
            this.selectCellContentsTextBox,
            this.selectCellValueLabel,
            this.selectCellValueDisplay});
            this.selectCellStatusBar.Location = new System.Drawing.Point(0, 32);
            this.selectCellStatusBar.Name = "selectCellStatusBar";
            this.selectCellStatusBar.Size = new System.Drawing.Size(809, 32);
            this.selectCellStatusBar.TabIndex = 1;
            this.selectCellStatusBar.Text = "Cell Status Bar";
            // 
            // selectCellNameLabel
            // 
            this.selectCellNameLabel.AutoSize = false;
            this.selectCellNameLabel.Font = new System.Drawing.Font("Calibri", 12F);
            this.selectCellNameLabel.Margin = new System.Windows.Forms.Padding(8, 1, 0, 2);
            this.selectCellNameLabel.Name = "selectCellNameLabel";
            this.selectCellNameLabel.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.selectCellNameLabel.Size = new System.Drawing.Size(36, 28);
            this.selectCellNameLabel.Text = "Cell:";
            // 
            // selectCellNameDisplay
            // 
            this.selectCellNameDisplay.AutoSize = false;
            this.selectCellNameDisplay.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.selectCellNameDisplay.Enabled = false;
            this.selectCellNameDisplay.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.selectCellNameDisplay.Name = "selectCellNameDisplay";
            this.selectCellNameDisplay.Size = new System.Drawing.Size(48, 28);
            this.selectCellNameDisplay.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // selectCellContentsLabel
            // 
            this.selectCellContentsLabel.AutoSize = false;
            this.selectCellContentsLabel.Font = new System.Drawing.Font("Calibri", 12F);
            this.selectCellContentsLabel.Margin = new System.Windows.Forms.Padding(16, 1, 0, 2);
            this.selectCellContentsLabel.Name = "selectCellContentsLabel";
            this.selectCellContentsLabel.Size = new System.Drawing.Size(72, 28);
            this.selectCellContentsLabel.Text = "Contents:";
            // 
            // selectCellContentsTextBox
            // 
            this.selectCellContentsTextBox.AutoSize = false;
            this.selectCellContentsTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.selectCellContentsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selectCellContentsTextBox.Font = new System.Drawing.Font("Calibri", 14F);
            this.selectCellContentsTextBox.Name = "selectCellContentsTextBox";
            this.selectCellContentsTextBox.Size = new System.Drawing.Size(140, 30);
            this.selectCellContentsTextBox.Leave += new System.EventHandler(this.selectCellContentsTextBox_Leave);
            this.selectCellContentsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.selectCellContentsTextBox_KeyDown);
            // 
            // selectCellValueLabel
            // 
            this.selectCellValueLabel.AutoSize = false;
            this.selectCellValueLabel.Font = new System.Drawing.Font("Calibri", 12F);
            this.selectCellValueLabel.Margin = new System.Windows.Forms.Padding(16, 1, 0, 2);
            this.selectCellValueLabel.Name = "selectCellValueLabel";
            this.selectCellValueLabel.Size = new System.Drawing.Size(52, 28);
            this.selectCellValueLabel.Text = "Value:";
            // 
            // selectCellValueDisplay
            // 
            this.selectCellValueDisplay.AutoSize = false;
            this.selectCellValueDisplay.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.selectCellValueDisplay.Enabled = false;
            this.selectCellValueDisplay.Font = new System.Drawing.Font("Calibri", 14F);
            this.selectCellValueDisplay.Name = "selectCellValueDisplay";
            this.selectCellValueDisplay.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.selectCellValueDisplay.Size = new System.Drawing.Size(140, 28);
            // 
            // spreadsheetPanel
            // 
            this.spreadsheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetPanel.Font = new System.Drawing.Font("Calibri", 14F);
            this.spreadsheetPanel.Location = new System.Drawing.Point(0, 69);
            this.spreadsheetPanel.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.spreadsheetPanel.Name = "spreadsheetPanel";
            this.spreadsheetPanel.Size = new System.Drawing.Size(809, 407);
            this.spreadsheetPanel.TabIndex = 2;
            this.spreadsheetPanel.TabStop = false;
            // 
            // UserGuide
            // 
            this.UserGuide.HelpNamespace = "C:\\Users\\Scotthew\\Box\\VS Projects\\source\\repos\\cs3500\\PS6\\Resources\\user_guide.ht" +
    "ml";
            // 
            // SpreadsheetGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 471);
            this.Controls.Add(this.spreadsheetPanel);
            this.Controls.Add(this.selectCellStatusBar);
            this.Controls.Add(this.topMenuStrip);
            this.MainMenuStrip = this.topMenuStrip;
            this.Name = "SpreadsheetGUI";
            this.Text = "CS3500 Spreadsheet Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpreadsheetGUI_FormClosing);
            this.topMenuStrip.ResumeLayout(false);
            this.topMenuStrip.PerformLayout();
            this.selectCellStatusBar.ResumeLayout(false);
            this.selectCellStatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip topMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileDropMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editDropMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutEditMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyEditMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteEditMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatDropMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textFormatMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sizeTextFormatMenutItem;
        private System.Windows.Forms.ToolStripMenuItem styleTextFormatMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpDropMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userGuideHelpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutHelpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallSizeTextFormatMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mediumSizeTextFormatMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeSizeTextFormatMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sansStyleTextFormatMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serifStyleTextFormatMenuItem;
        private System.Windows.Forms.ToolStripMenuItem monoStyleTextFormatMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoEditMenuItem;
        private System.Windows.Forms.ToolStrip selectCellStatusBar;
        private System.Windows.Forms.ToolStripLabel selectCellNameLabel;
        private System.Windows.Forms.ToolStripTextBox selectCellNameDisplay;
        private System.Windows.Forms.ToolStripLabel selectCellContentsLabel;
        private System.Windows.Forms.ToolStripTextBox selectCellContentsTextBox;
        private System.Windows.Forms.ToolStripLabel selectCellValueLabel;
        private System.Windows.Forms.ToolStripTextBox selectCellValueDisplay;
        private SpreadsheetPanel spreadsheetPanel;
        private System.Windows.Forms.HelpProvider UserGuide;
    }
}

