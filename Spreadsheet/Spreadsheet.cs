// AUTHOR:  Scott Crowley (u118178)
// VERSION: 4 October 2019

using System;
using System.Collections.Generic;
using System.Xml;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> _cell_register;
        private DependencyGraph _dGraph;
        private bool _changed;


        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get => _changed; protected set => _changed = value; }


        /// <summary>
        /// Constructs an empty spreadsheet object.
        /// </summary>
        public Spreadsheet() : base(name => true, name => name , "default")
        {
            _changed = false;
            Version = "default";
            _cell_register = new Dictionary<string, Cell>();
            _dGraph = new DependencyGraph();
        }


        /// <summary>
        /// Constructs a spreadsheet object with user-specified methods to normalize and validate names and declare the
        /// spreadsheet's version.
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
                                                                                    : base(isValid, normalize, version)
        {
            if (version is null)
                throw new ArgumentNullException("ERROR: Version is null.");
            _changed = false;
            IsValid = isValid;
            Normalize = normalize;
            Version = version;
            _cell_register = new Dictionary<string, Cell>();
            _dGraph = new DependencyGraph();
        }


        /// <summary>
        /// Constructs a spreadsheet object with user-specified methods to normalize and validate names and declare the
        /// spreadsheet's version. Reads in a spreadsheet XML from the given filepath and copies the data.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(string filepath, Func<string, bool> isValid, Func<string, string> normalize, string version)
                                                                                    : base(isValid, normalize, version)
        {
            if (filepath is null)
                throw new SpreadsheetReadWriteException("ERROR: Filepath is null.");
            if (version is null)
                throw new ArgumentNullException("ERROR: Version is null.");

            IsValid = isValid;
            Normalize = normalize;
            Version = version;
            _cell_register = new Dictionary<string, Cell>();
            _dGraph = new DependencyGraph();

            ReadInXML(filepath);

            _changed = false;
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (name is null)
                throw new InvalidNameException();
            string name_norm = Normalize(name);
            if (!IsValidName(name_norm))
                throw new InvalidNameException();
            Cell cell;
            if (_cell_register.TryGetValue(name_norm, out cell))
                return cell.GetContents();
            return "";
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (name is null)
                throw new InvalidNameException();
            string name_norm = Normalize(name);
            if (!IsValidName(name_norm))
                throw new InvalidNameException();
            Cell cell;
            if (_cell_register.TryGetValue(name_norm, out cell))
                return cell.GetValue();
            return "";
        }


        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            string[] cell_name_list = new string[_cell_register.Count];
            int idx = 0;
            foreach (KeyValuePair<string, Cell> entry in _cell_register)
            {
                cell_name_list[idx] = entry.Value.GetName();
                idx++;
            }
            return cell_name_list;
        }


        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            string error = "ERROR: Unable to read from file, \"" + filename + "\".";
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    string version = null;
                    if (reader.IsStartElement() && reader.Name.Equals("spreadsheet"))
                    {
                        version = reader.GetAttribute("version");
                        if (version is null)
                        {
                            error = "ERROR: Unable to get version from file, \"" + filename + "\".";
                            throw new Exception();
                        }
                        return version;
                    }
                    error = "ERROR: Malformed XML file, \"" + filename + "\".";
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException(error);
            }
        }


        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    foreach (string cell in GetNamesOfAllNonemptyCells())
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cell);

                        Object contents = GetCellContents(cell);
                        if (contents is double)
                            writer.WriteElementString("contents", ((double)contents).ToString());
                        else if (contents is string)
                            writer.WriteElementString("contents", (string)contents);
                        else
                            writer.WriteElementString("contents", "=" + ((Formula)contents).ToString());

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    _changed = false;
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("ERROR: Unable to save to file, \"" + filename + "\".");
            }
        }


        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content is null)
                throw new ArgumentNullException();
            if (name is null)
                throw new InvalidNameException();
            string name_norm = Normalize(name);
            if (!IsValidName(name_norm))
                throw new InvalidNameException();

            IList<string> cells_to_evaluate;

            double number;
            if (Double.TryParse(content, out number))
                cells_to_evaluate = SetCellContents(name_norm, number);

            else if (content.Length == 0 || content[0] != '=')
                cells_to_evaluate = SetCellContents(name_norm, content);

            else
            {
                Formula form = new Formula(content.Remove(0, 1), Normalize, IsValidName);
                cells_to_evaluate = SetCellContents(name_norm, form);
            }
            EvaluateCells(cells_to_evaluate);
            _changed = true;
            return cells_to_evaluate;
        }


        /// <summary>
        /// The contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            Cell cell;
            if (!(_cell_register.TryGetValue(name, out cell)))
            {
                cell = new Cell(name, number);
                _cell_register.Add(name, cell);
            }
            else
            {
                TryRemoveDependees(cell);
                cell.SetContents(number);
            }
            IList<string> cell_list = new List<string>(GetCellsToRecalculate(name));
            return cell_list;
        }


        /// <summary>
        /// The contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            Cell cell;
            if (!(_cell_register.TryGetValue(name, out cell)))
            {
                cell = new Cell(name, text);
                _cell_register.Add(name, cell);
            }
            else
            {
                TryRemoveDependees(cell);
                cell.SetContents(text);
            }
            IList<string> cell_list = new List<string>(GetCellsToRecalculate(name));
            return cell_list;
        }


        /// <summary>
        /// If changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula. The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            Cell cell;
            if (!(_cell_register.TryGetValue(name, out cell)))
            {
                try
                {
                    cell = new Cell(name, formula);
                    _cell_register.Add(name, cell);

                    foreach (string var in formula.GetVariables())
                        _dGraph.AddDependency(var, name);

                    IList<string> cell_list = new List<string>(GetCellsToRecalculate(name));
                    return cell_list;
                }
                catch (CircularException)
                {
                    _cell_register.Remove(name);

                    foreach (string var in formula.GetVariables())
                        _dGraph.RemoveDependency(var, name);

                    throw new CircularException();
                }
            }
            Object replaced = CopyCellContents(cell);

            TryRemoveDependees(cell);

            foreach (string var in formula.GetVariables())
                _dGraph.AddDependency(var, name);

            cell.SetContents(formula);

            try
            {
                IList<string> cell_list = new List<string>(GetCellsToRecalculate(name));
                return cell_list;
            }
            catch (CircularException)
            {
                if (replaced is double)
                    cell.SetContents((double)replaced);
                else if (replaced is string)
                    cell.SetContents((string)replaced);
                else
                {
                    cell.SetContents((Formula)replaced);
                    foreach (string var in ((Formula)replaced).GetVariables())
                        _dGraph.AddDependency(var, name);
                }
                foreach (string var in formula.GetVariables())
                    _dGraph.RemoveDependency(var, name);

                throw new CircularException();
            }
        }


        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return _dGraph.GetDependents(name);
        }


        /// <summary>
        /// Private helper method checks if a potential cell or variable name meets the minimum format requirements:
        /// consists of one or more letters followed by one or more digits AND it satisfies _Validator.
        /// </summary>
        /// <param name="_name">The proposed cell or variable name.</param>
        /// <returns>True if argument meets format requirements; otherwise, false.</returns>
        private bool IsValidName(string _name)
        {
            int length = _name.Length;
            if (length < 2)
                return false;

            int idx = 0;
            while (idx < length - 1)
            {
                char next = _name[idx];
                if (('A' <= next && next <= 'Z') || ('a' <= next && next <= 'z'))
                {
                    idx++;
                    continue;
                }
                else if ('0' <= next && next <= '9')
                    break;
                return false;
            }
            while (idx < length)
            {
                char next = _name[idx];
                if ('0' <= next && next <= '9')
                {
                    idx++;
                    continue;
                }
                return false;
            }
            return IsValid(_name);
        }


        /// <summary>
        /// Private helper method provides a way to create a copy/clone of a Cell's contents.
        /// </summary>
        /// <param name="c">A Cell whose contents needs to be copied.</param>
        /// <returns>A copy of the double, string, or Formula in the argument's contents.</returns>
        private Object CopyCellContents(Cell c)
        {
            Object copy = c.GetContents();
            if (copy is double)
            {
                double number = (double) copy;
                return number;
            }
            if (copy is string)
            {
                string text = (string) copy;
                return text;
            }
            Formula formula = new Formula(((Formula)copy).ToString());
            return formula;
        }


        /// <summary>
        /// Private helper method removes a cell's dependees from the graph, if its contents is a formula.
        /// </summary>
        /// <param name="c">Cell that might have dependees to remove.</param>
        /// <returns>True if the cell's contents is a Formula; otherwise, false.</returns>
        private bool TryRemoveDependees(Cell c)
        {
            if (c.GetContents() is Formula)
            {
                string cell_name = c.GetName();
                foreach (string var in ((Formula)c.GetContents()).GetVariables())
                    _dGraph.RemoveDependency(var, cell_name);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Private helper method calls Evaluate on a list of Cells and sets their values appropriately.
        /// </summary>
        /// <param name="cells"></param>
        private void EvaluateCells(IList<string> cells)
        {
            foreach (string next in cells)
            {
                Cell cell;
                _cell_register.TryGetValue(next, out cell);
                Object contents = cell.GetContents();

                if (contents is double)
                {
                    cell.SetValue((double)contents);
                    continue;
                }

                if (contents is string)
                {
                    cell.SetValue((string)contents);
                    if ((string)contents == "")
                        _cell_register.Remove(next);
                    continue;
                }

                Object result = ((Formula)contents).Evaluate(var =>
                {   // Lookup method in lambda expression.
                    Cell c;
                    if (!_cell_register.TryGetValue(var, out c))
                        throw new ArgumentException("ERROR: Cell has no value.");
                    Object value = c.GetValue();
                    if (value is double)
                        return (double)value;
                    throw new ArgumentException("ERROR: Cells value was not a double.");
                });
                if (result is double)
                {
                    cell.SetValue((double)result);
                    continue;
                }
                cell.SetValueError((FormulaError)result);    
            }
        }

        
        /// <summary>
        /// Private helper method uses an XmlReader to parse an XML-formated spreadsheet into this Spreadsheet.
        /// </summary>
        /// <param name="filepath">Path and filename of spreadsheet.</param>
        private void ReadInXML(string filepath)
        {
            string error = "IO";
            try
            {
                using (XmlReader reader = XmlReader.Create(filepath))
                {
                    string cell_name = null;
                    string contents = null;
                    bool root_encountered = false;
                    bool within_cell_element = false;

                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    if (!root_encountered)
                                    {
                                        if (reader.GetAttribute("version").Equals(Version))
                                        {
                                            root_encountered = true;
                                            break;
                                        }
                                        error = "VER";
                                        throw new Exception();
                                    }
                                    error = "FORM";
                                    throw new Exception();

                                case "cell":
                                    if (contents == null && cell_name == null && root_encountered)
                                    {
                                        within_cell_element = true;
                                        break;
                                    }
                                    error = "FORM";
                                    throw new Exception();

                                case "name":
                                    if(within_cell_element && cell_name is null && reader.Read())
                                    {
                                        cell_name = reader.Value;
                                        if (contents != null)
                                        {
                                            SetContentsOfCell(cell_name, contents);
                                            cell_name = null;
                                            contents = null;
                                            within_cell_element = false;
                                        }
                                        break;
                                    }
                                    error = "FORM";
                                    throw new Exception();

                                case "contents":
                                    if (within_cell_element && contents is null && reader.Read())
                                    {
                                        contents = reader.Value;
                                        if (cell_name != null)
                                        {
                                            SetContentsOfCell(cell_name, contents);
                                            cell_name = null;
                                            contents = null;
                                            within_cell_element = false;
                                        }
                                        break;
                                    }
                                    error = "FORM";
                                    throw new Exception();

                                default:
                                    error = "FORM";
                                    throw new Exception();
                            }
                        }
                    }
                    if (!root_encountered || within_cell_element || cell_name != null || contents != null)
                    {
                        error = "FORM";
                        throw new Exception();
                    }
                }
            }
            catch (InvalidNameException)
            {
                throw new SpreadsheetReadWriteException("ERROR: Invalid name encountered in file, \"" + filepath + "\".");
            }
            catch (FormulaFormatException)
            {
                throw new SpreadsheetReadWriteException("ERROR: Invalid Formula encountered in file, \"" + filepath + "\".");
            }
            catch (CircularException)
            {
                throw new SpreadsheetReadWriteException("ERROR: Circular dependency encountered in file, \"" + filepath + "\".");
            }
            catch (Exception)
            {
                switch (error)
                {
                    case "FORM":
                        throw new SpreadsheetReadWriteException("ERROR: Malformed XML in file, \"" + filepath + "\".");

                    case "IO":
                        throw new SpreadsheetReadWriteException("ERROR: Unable to read from file, \"" + filepath + "\".");

                    case "VER":
                        throw new SpreadsheetReadWriteException("ERROR: Version argument does not match version in file, \"" + filepath + "\".");
                }
            }
        }

    }
}
