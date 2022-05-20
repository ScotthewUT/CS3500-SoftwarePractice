// AUTHOR:  Scott Crowley (u118178)
// VERSION: 27 September 2019

using System;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// Class representation of a cell object as used in spreadsheets.
    /// Cells have a name, contents, and a calculated value.
    /// </summary>
    public class Cell
    {
        private readonly string _name;
        private Object _contents;
        private Object _value;

        
        /// <summary>
        /// Default Cell constructor which requires a name.
        /// </summary>
        /// <param name="name">Name of the cell.</param>
        public Cell(string name)
        {
            _name = name;
        }


        /// <summary>
        /// Overloaded constructor for assigning text contents at creation.
        /// </summary>
        /// <param name="name">Name of the cell.</param>
        /// <param name="text">The cell's contents.</param>
        public Cell(string name, string text)
        {
            _name = name;
            _contents = text;
        }


        /// <summary>
        /// Overloaded constructor for assigning numeric contents at creation.
        /// </summary>
        /// <param name="name">Name of the cell.</param>
        /// <param name="number">The cell's contents.</param>
        public Cell(string name, double number)
        {
            _name = name;
            _contents = number;
        }


        /// <summary>
        /// Overloaded constructor for assigning a formula as contents at creation.
        /// </summary>
        /// <param name="name">Name of the cell.</param>
        /// <param name="formula">The cell's contents.</param>
        public Cell(string name, Formula formula)
        {
            _name = name;
            _contents = formula;
        }


        /// <summary>
        /// Gets the name of this cell.
        /// </summary>
        public string GetName()
        {
            return _name;
        }


        /// <summary>
        /// Sets the contents of this cell to a string.
        /// </summary>
        public void SetContents(string text)
        {
            _contents = text;
        }


        /// <summary>
        /// Sets the contents of this cell to a double.
        /// </summary>
        public void SetContents(double number)
        {
            _contents = number;
        }


        /// <summary>
        /// Sets the contents of this cell to a Formula.
        /// </summary>
        public void SetContents(Formula formula)
        {
            _contents = formula;
        }


        /// <summary>
        /// Gets the contents of this cell.
        /// </summary>
        /// <returns>The contents of this cell (string, double, or Formula).</returns>
        public Object GetContents()
        {
            return _contents;
        }


        /// <summary>
        /// Sets the value of this cell to a string.
        /// </summary>
        /// <param name="text"></param>
        public void SetValue(string text)
        {
            _value = text;
        }


        /// <summary>
        /// Sets the value of this cell to a double.
        /// </summary>
        /// <param name="number"></param>
        public void SetValue(double number)
        {
            _value = number;
        }


        /// <summary>
        /// Sets the value of this cell to a FormulaError when Evaluate failed.
        /// </summary>
        /// <param name="error"></param>
        public void SetValueError(FormulaError error)
        {
            _value = error;
        }


        /// <summary>
        /// Gets the value of this cell.
        /// </summary>
        /// <returns>The value of the cell (double, empty, or FormulaError).</returns>
        public Object GetValue()
        {
            return _value;
        }
    }

}
