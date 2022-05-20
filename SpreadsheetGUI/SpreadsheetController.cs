// AUTHOR:   Scott Crowley (u1178178)
// VERSION:  6 October 2019

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SpreadsheetUtilities;

namespace SS
{
    public class SpreadsheetController
    {
        private int _col;                       // Currently selected column.
        private int _row;                       // Currently selected row.
        private char[] _colNames;               // Maps an x-position to a letter A-Z.
        private int[] _rowNames;                // Maps a y-position to a number 1-99.
        private AbstractSpreadsheet _sheet;     // Models the spreadsheet's data and calculations.
        private string _filename;               // Current spreadsheet filename.
        private Stack<string> _undoStack;       // Supports reverting changes.
        private string _clipboard;              // Supports copy & paste.

        /// <summary>
        /// Default constructor for controller class.
        /// </summary>
        public SpreadsheetController()
        {
            // Initialize the spreadsheet model that backs the GUI.
            _sheet = new Spreadsheet(str => true, str => str.ToUpper(), "ps6");
            // Map zero-based cell coordinates to cell names.
            BuildCellNames();
            // Set default filename.
            _filename = "untitled.sprd";
            // Initialize the stack for undos.
            _undoStack = new Stack<string>();
            // Clipboard begins empty.
            _clipboard = null;
        }


        /// <summary>
        /// Overloaded constructor for opening existing spreadsheet files.
        /// </summary>
        public SpreadsheetController(string filename)
        {
            _sheet = new Spreadsheet(filename, str => true, str => str.ToUpper(), "ps6");
            BuildCellNames();
            _undoStack = new Stack<string>();
            _filename = filename;
            _clipboard = null;
        }


        /// <summary>
        /// Sets the contents of the clipboard.
        /// </summary>
        public void EditCopyContents(string contents)
        {
            _clipboard = contents;
        }


        /// <summary>
        /// Gets the contents of the clipboard.
        /// </summary>
        public string EditPasteContents()
        {
            return _clipboard;
        }


        /// <summary>
        /// Converts the return of AbstractSpreadsheet.GetCellContents to an appropriate string.
        /// </summary>
        /// <param name="name">Cell name (e.g. "C3").</param>
        /// <returns>The string form of either a string, double, or Formula.</returns>
        public string GetCellContents(string name)
        {
            Object contents = _sheet.GetCellContents(name);
            if (contents is string)
                return (string)contents;
            if (contents is double)
                return ((double)contents).ToString();
            return "=" + ((Formula)contents).ToString();
        }

        /// <summary>
        /// Converts a zero-based cell coordinate to its name. For example: (0, 0) becomes "A1".
        /// </summary>
        /// <returns>The name of the cell at the provided position.</returns>
        public string GetCellName(int col, int row)
        {
            return _colNames[col] + _rowNames[row].ToString();
        }

        /// <summary>
        /// Converts the return of AbstractSpreadsheet.GetCellValue to an appropriate string.
        /// </summary>
        /// <param name="name">Cell name (e.g. "C3").</param>
        /// <returns>The string form of either a string, double, or FormulaError.</returns>
        public string GetCellValue(string name)
        {
            Object value = _sheet.GetCellValue(name);
            if (value is string)
                return (string)value;
            if (value is double)
                return ((double)value).ToString();
            FormulaError error = (FormulaError)value;
            return error.Reason;
        }


        /// <summary>
        /// Gets the names of all non-empty cells in the spreadsheet.
        /// Used when drawing a new spreadsheet GUI from a saved file.
        /// </summary>
        /// <returns>A list of names of all non-empty cells.</returns>
        public List<string> GetCellsToDraw()
        {
            return new List<string>(_sheet.GetNamesOfAllNonemptyCells());
        }


        /// <summary>
        /// Returns the zero-based column position of the selected cell (e.g. returnds 0 when "A7" is selected).
        /// </summary>
        public int GetColumnPosition()
        {
            return _col;
        }


        /// <summary>
        /// Returns the zero-based row position of the selected cell (e.g. returns 0 when "F1" is selected).
        /// </summary>
        public int GetRowPosition()
        {
            return _row;
        }


        /// <summary>
        /// Saves the spreadsheet to the most recent file name.
        /// </summary>
        public void SaveSpreadsheet()
        {
            if (_filename.Equals("untitled.sprd"))
                SaveSpreadsheetAsNewFile();
            else
                _sheet.Save(_filename);
        }


        /// <summary>
        /// Saves the spreadsheet to user-provided file name.
        /// </summary>
        public void SaveSpreadsheetAsNewFile()
        {
            SaveFileDialog savePrompt = new SaveFileDialog();
            savePrompt.DefaultExt = "sprd";
            savePrompt.Filter = "Spreadsheet files|*.sprd|All files|*.*";
            savePrompt.Title = "Save As";
            savePrompt.CheckPathExists = true;
            savePrompt.CheckPathExists = true;

            if (savePrompt.ShowDialog() == DialogResult.OK)
            {
                string enteredName = savePrompt.FileName;

                if (savePrompt.FilterIndex == 1)
                {
                    if (enteredName.Length < 6 || !enteredName.Substring(enteredName.Length - 5).ToLower().Equals(".sprd"))
                        _filename = enteredName + ".sprd";
                    else
                        _filename = enteredName;
                }
                else
                    _filename = enteredName;

                _sheet.Save(_filename);
            }
        }

        /// <summary>
        /// Sets the zero-based coordinates of the selected cell.
        /// </summary>
        /// <param name="col">X-position of selected cell (e.g. 0 is Column A).</param>
        /// <param name="row">Y-position of selected cell (e.g. 0 is Row 1).</param>
        public void SetCellSelection(int col, int row)
        {
            _col = col;
            _row = row;
        }


        /// <summary>
        /// Checks if the spreadsheet has been changed since it was opened or saved.
        /// </summary>
        /// <returns>True, if there are unsaved changes; false otherwise.</returns>
        public bool SpreadsheetChanged()
        {
            return _sheet.Changed;
        }


        /// <summary>
        /// Updates the contents of the selected cell if able. If an invalid forumla or circular dependency is encountered
        /// the contents remains unchanged and an error is generated.
        /// </summary>
        /// <param name="newContents">The contents to update the selected cell with.</param>
        /// <param name="cellList">A list of cells that depend directly or indirectly on the selected cell.</param>
        /// <param name="error">An error message describing the caught exception.</param>
        /// <returns></returns>
        public bool TryUpdateContents(string newContents, out List<string> cellList, out string error)
        {
            string name = GetCellName(_col, _row);
            string prevContents = GetCellContents(name);

            if (newContents.Equals(prevContents))
            {
                cellList = null;
                error = null;
                return false;
            }

            try
            {
                cellList = new List<string>(_sheet.SetContentsOfCell(name, newContents));
                _undoStack.Push(prevContents);
                _undoStack.Push(name);
                error = null;
                return true;
            }
            catch (FormulaFormatException)
            {
                cellList = null;
                error = "ERROR:  \"" + newContents + "\" is not a valid formula!";
                return false;
            }
            catch (CircularException)
            {
                cellList = null;
                error = "ERROR:  Ciruclar dependency introduced!";
                return false;
            }
        }


        /// <summary>
        /// Reverts the most recent change to a cell's contents.
        /// </summary>
        /// <param name="cellList">The dependents, direct and indirect, of the cell whose contents was reverted.</param>
        /// <returns>True, if a cell's contents was reverted by this method; otherwise, false.</returns>
        public bool UndoCellChange(out List<string> cellList)
        {
            if (_undoStack.Count != 0)
            {
                string cellName = _undoStack.Pop();
                string contents = _undoStack.Pop();
                cellList = new List<string>(_sheet.SetContentsOfCell(cellName, contents));
                return true;
            }
            cellList = null;
            return false;
        }


        /// <summary>
        /// The private member arrays, _colNames & _rowNames, are assigned A-Z and 1-99, respectively.
        /// </summary>
        private void BuildCellNames()
        {
            _col = 0;
            _row = 0;
            _colNames = new char[26];
            _rowNames = new int[99];
            for (int idx = 0; idx < 26; idx++)
                _colNames[idx] = (char)(idx + 'A');
            for (int idx = 0; idx < 99; idx++)
                _rowNames[idx] = idx + 1;
        }



    }
}
