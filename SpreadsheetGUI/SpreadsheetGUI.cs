// AUTHOR:   Scott Crowley (u1178178)
// VERSION:  6 October 2019

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SS
{
    public partial class SpreadsheetGUI : Form
    {
        // Controller connects GUI to spreadsheet model.
        private SpreadsheetController _control;


        /// <summary>
        /// Default constructor for spreadsheet GUI (view & control).
        /// </summary>
        public SpreadsheetGUI()
        {
            // Initialize the controller.
            _control = new SpreadsheetController();
            // Auto-generated method for Designer.
            InitializeComponent();
            // Initial cell selection is "A1".
            spreadsheetPanel.SetSelection(0, 0);
            // Update the selectedCellStatusBar with "A1".
            DisplayCellStatusBar(0, 0);
            // Register to cell selection changes so the status bar can be updated.
            spreadsheetPanel.SelectionChanged += NewCellSelection;
        }


        /// <summary>
        /// Overloaded constructor to open spreadsheets from XML files.
        /// The default extension is ".sprd", but this is not strictly enforced.
        /// </summary>
        /// <param name="filename">The spreadsheet's file name.</param>
        public SpreadsheetGUI(string filename)
        {
            _control = new SpreadsheetController(filename);
            InitializeComponent();
            // Update the SpreadsheetPanel with the values of all non-empty cells.
            UpdateValuesDisplayed(_control.GetCellsToDraw());
            spreadsheetPanel.SetSelection(0, 0);
            DisplayCellStatusBar(0, 0);
            spreadsheetPanel.SelectionChanged += NewCellSelection;
        }


        /// <summary>
        /// Updates the text displayed in the selectCellStatusBar.
        /// </summary>
        /// <param name="col">Zero-based x-position of selected cell.</param>
        /// <param name="row">Zero-based y-position of selected cell.</param>
        private void DisplayCellStatusBar(int col, int row)
        {
            string cellName = _control.GetCellName(col, row);
            selectCellNameDisplay.Text = cellName;
            selectCellContentsTextBox.Text = _control.GetCellContents(cellName);
            string cellValue;
            spreadsheetPanel.GetValue(col, row, out cellValue);
            selectCellValueDisplay.Text = cellValue;
        }


        /// <summary>
        /// Listens for a cell selection change (i.e. this is subscribed to spreadsheetPanel.SelectionChanged). Reports the
        /// selection to the controller and sends the coordinates to DisplayCellStatusBar so it can update.
        /// </summary>
        /// <param name="ssp">The SpreadsheetPanel this is subscribed to.</param>
        private void NewCellSelection(SpreadsheetPanel ssp)
        {
            spreadsheetPanel.Focus();
            int col, row;
            ssp.GetSelection(out col, out row);
            _control.SetCellSelection(col, row);
            DisplayCellStatusBar(col, row);
            selectCellContentsTextBox.Focus();
        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="cell_list"></param>
        private void UpdateValuesDisplayed(List<string> cell_list)
        {
            foreach (string cell in cell_list)
            {
                int col = cell[0] - 'A';
                int row = int.Parse(cell.Substring(1)) - 1;
                string value = _control.GetCellValue(cell);
                spreadsheetPanel.SetValue(col, row, value);
            }
        }


        /// <summary>
        /// Runs a new spreadsheet on the same thread as the others whenever File>New is selected.
        /// </summary>
        private void newFileMenuItem_Click(object sender, EventArgs e)
        {   
            SpreadsheetAppContext.getAppContext().RunSpreadsheet(new SpreadsheetGUI());
        }


        private void openFileMenuItem_Click(object sender, EventArgs e)
        {
            OpenSpreadsheet();
        }


        /// <summary>
        /// TODO
        /// </summary>
        private void OpenSpreadsheet()
        {
            if (_control.SpreadsheetChanged())
            {
                DialogResult response = MessageBox.Show("Save changes to spreadsheet before opening another?\n\n"
                                                      + "Your changes will be lost if you don’t save them.",
                                                        "Save Spreadsheet?", MessageBoxButtons.YesNoCancel);
                if (response == DialogResult.Yes)
                    _control.SaveSpreadsheet();
                else if (response == DialogResult.Cancel)
                    return;
            }

            OpenFileDialog openPrompt = new OpenFileDialog();
            openPrompt.DefaultExt = "sprd";
            openPrompt.Filter = "Spreadsheet files|*.sprd|All files|*.*";
            openPrompt.Title = "Open";
            openPrompt.CheckFileExists = true;
            openPrompt.CheckPathExists = true;

            if (openPrompt.ShowDialog() == DialogResult.OK)
            {
                this.Hide();
                SpreadsheetAppContext.getAppContext().RunSpreadsheet(new SpreadsheetGUI(openPrompt.FileName));
                this.Close();
            }
        }


        private void closeFileMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Saves the spreadsheet to the most recent file name when File>Save is selected.
        /// </summary>
        private void saveFileMenuItem_Click(object sender, EventArgs e)
        {
            _control.SaveSpreadsheet();
        }


        /// <summary>
        /// Prompts user to save file to provided file name when File>Save As is selected.
        /// </summary>
        private void saveAsFileMenuItem_Click(object sender, EventArgs e)
        {
            _control.SaveSpreadsheetAsNewFile();
        }


        /// <summary>
        /// Cuts the contents from the selected cell and puts it in the clipboard whenever Edit>Cut is selected.
        /// </summary>
        private void cutEditMenuItem_Click(object sender, EventArgs e)
        {
            _control.EditCopyContents(selectCellContentsTextBox.Text);
            selectCellContentsTextBox.Text = "";
            spreadsheetPanel.Focus();
            selectCellContentsTextBox.Focus();
        }


        /// <summary>
        /// Copies the contents from the selected cell and puts it in the clipboard whenever Edit>Copy is selected.
        /// </summary>
        private void copyEditMenuItem_Click(object sender, EventArgs e)
        {
            spreadsheetPanel.Focus();
            _control.EditCopyContents(String.Copy(selectCellContentsTextBox.Text));
        }


        /// <summary>
        /// Replaces the contents of the selected cell with whatever is in the clipboard when Edit>Paste is selected.
        /// </summary>
        private void pasteEditMenuItem_Click(object sender, EventArgs e)
        {
            selectCellContentsTextBox.Text = _control.EditPasteContents();
            spreadsheetPanel.Focus();
            selectCellContentsTextBox.Focus();
        }


        /// <summary>
        /// Supports the ability to reverse previous changes to cell contents when Edit>Undo is selected.
        /// </summary>
        private void undoEditMenuItem_Click(object sender, EventArgs e)
        {
            List<string> undoCells;
            if (_control.UndoCellChange(out undoCells))
                UpdateValuesDisplayed(undoCells);
        }


        /// <summary>
        /// Sets the spreadsheet's font size to 10pt when the menu option, Format>Text>Size>Small, is clicked.
        /// </summary>
        private void smallSizeTextFormatMenuItem_Click(object sender, EventArgs e)
        {
            spreadsheetPanel.Font = new Font(spreadsheetPanel.Font.FontFamily, 10);
        }


        /// <summary>
        /// Sets the spreadsheet's font size to 14pt when the menu option, Format>Text>Size>Medium, is clicked.
        /// </summary>
        private void mediumSizeTextFormatMenuItem_Click(object sender, EventArgs e)
        {
            spreadsheetPanel.Font = new Font(spreadsheetPanel.Font.FontFamily, 14);
        }


        /// <summary>
        /// Sets the spreadsheet's font size to 18pt when the menu option, Format>Text>Size>Large, is clicked.
        /// </summary>
        private void largeSizeTextFormatMenuItem_Click(object sender, EventArgs e)
        {
            spreadsheetPanel.Font = new Font(spreadsheetPanel.Font.FontFamily, 18);
        }


        /// <summary>
        /// Sets the spreadsheet's font style to Calibri when the menu option, Format>Text>Style>Sans Serif, is clicked.
        /// </summary>
        private void sansStyleTextFormatMenuItem_Click(object sender, EventArgs e)
        {
            spreadsheetPanel.Font = new Font("Calibri", spreadsheetPanel.Font.Size);
        }


        /// <summary>
        /// Sets the spreadsheet's font style to Cambria when the menu option, Format>Text>Style>Serif, is clicked.
        /// </summary>
        private void serifStyleTextFormatMenuItem_Click(object sender, EventArgs e)
        {
            spreadsheetPanel.Font = new Font("Cambria", spreadsheetPanel.Font.Size);
        }


        /// <summary>
        /// Sets the spreadsheet's font style to Consolas when the menu option, Format>Text>Style>Monospaced, is clicked.
        /// </summary>
        private void monoStyleTextFormatMenuItem_Click(object sender, EventArgs e)
        {
            spreadsheetPanel.Font = new Font("Consolas", spreadsheetPanel.Font.Size);
        }

        private void userGuideHelpMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(spreadsheetPanel, "https://72fpc4rk8t9j.htmlsave.com");
        }


        /// <summary>
        /// Displays information about the program when the menu option, Help>About, is clicked.
        /// </summary>
        private void aboutHelpMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PS6 - SpreadsheetGUI\nUpdated 6 October 2019\nBuilt for CS 3500 by Scott Crowley");
        }


        /// <summary>
        /// Listens for KeyDown events in the cell contents text box. Does nothing unless one of the following keys was
        /// pressed: Enter, Up, Down, Left, Right, or Tab. In which case the helper method, CheckContentsAndMoveSelection,
        /// is called.
        /// </summary>
        private void selectCellContentsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Tab:
                    e.SuppressKeyPress = true;  // Prevents Windows' error sound.
                    CheckContentsAndMoveSelection(e.KeyCode);
                    break;
            }
            e.Handled = true;
        }


        /// <summary>
        /// When entering text into the Contents text box, Enter & Tab will attempt to update the cell's contents then try
        /// to advance the selected cell down or right, respectively. The arrow keys may also be used to move the selection
        /// to neighboring cells, and, if a move is possible, the contents is also updated.
        /// </summary>
        /// <param name="key">A Keys Enum object representing Enter, Up, Down, Left, Right, or Tab.</param>
        private void CheckContentsAndMoveSelection(Keys key)
        {
            int col, row;
            spreadsheetPanel.GetSelection(out col, out row);

            switch (key)
            {
                case Keys.Enter:
                    CellContentsCheck();
                    if (row != 98)
                    {
                        row++;
                        spreadsheetPanel.SetSelection(col, row);
                        _control.SetCellSelection(col, row);
                        DisplayCellStatusBar(col, row);
                    }
                    break;


                case Keys.Up:
                    if (row != 0)
                    {
                        CellContentsCheck();
                        row--;
                        spreadsheetPanel.SetSelection(col, row);
                        _control.SetCellSelection(col, row);
                        DisplayCellStatusBar(col, row);
                    }
                    break;


                case Keys.Down:
                    if (row != 98)
                    {
                        CellContentsCheck();
                        row++;
                        spreadsheetPanel.SetSelection(col, row);
                        _control.SetCellSelection(col, row);
                        DisplayCellStatusBar(col, row);
                    }
                    break;


                case Keys.Left:
                    if (col != 0)
                    {
                        CellContentsCheck();
                        col--;
                        spreadsheetPanel.SetSelection(col, row);
                        _control.SetCellSelection(col, row);
                        DisplayCellStatusBar(col, row);
                    }
                    break;


                case Keys.Right:
                    if (col != 25)
                    {
                        CellContentsCheck();
                        col++;
                        spreadsheetPanel.SetSelection(col, row);
                        _control.SetCellSelection(col, row);
                        DisplayCellStatusBar(col, row);
                    }
                    break;


                case Keys.Tab:
                    CellContentsCheck();
                    if (col != 25)
                    {
                        col++;
                        spreadsheetPanel.SetSelection(col, row);
                        _control.SetCellSelection(col, row);
                        DisplayCellStatusBar(col, row);
                    }
                    else if (row != 98)
                    {
                        col = 0;
                        row++;
                        spreadsheetPanel.SetSelection(col, row);
                        _control.SetCellSelection(col, row);
                        DisplayCellStatusBar(col, row);
                    }
                    break;
            }
        }


        /// <summary>
        /// When focus leaves the contents text box, the entry is checked for valid update.
        /// </summary>
        private void selectCellContentsTextBox_Leave(object sender, EventArgs e)
        {
            CellContentsCheck();
        }

        
        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        private bool CellContentsCheck()
        {
            List<string> outdatedCells;
            string error;
            if (_control.TryUpdateContents(selectCellContentsTextBox.Text, out outdatedCells, out error))
            {
                UpdateValuesDisplayed(outdatedCells);
                return true;
            }
            selectCellContentsTextBox.Text = _control.GetCellContents(selectCellNameDisplay.Text);
            if (error != null)
                MessageBox.Show(error);
            return false;
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void SpreadsheetGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_control.SpreadsheetChanged())
            {
                DialogResult response = MessageBox.Show("Save changes to spreadsheet before closing?\n\n"
                                                      + "Your changes will be lost if you don’t save them.",
                                                        "Save Spreadsheet?", MessageBoxButtons.YesNoCancel);
                if (response == DialogResult.Yes)
                    _control.SaveSpreadsheet();
                else if (response == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }
    }

}
