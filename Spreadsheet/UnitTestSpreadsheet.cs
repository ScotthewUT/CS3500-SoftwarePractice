// AUTHOR:  Scott Crowley (u118178)
// VERSION: 2 October 2019

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    /// <summary>
    /// This class represents a collection of unit tests for the Spreadsheet class.
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        // Testing Changed property of Spreadsheets.
        [TestMethod(), Timeout(1000)]
        public void ChangedPropertyIsFalseForNewSpreadsheetTest()
        {
            Spreadsheet ss_A = new Spreadsheet();
            Spreadsheet ss_B = new Spreadsheet(str => true, str => str, "1.0");

            Assert.IsFalse(ss_A.Changed);
            Assert.IsFalse(ss_B.Changed);
        }

        [TestMethod(), Timeout(1000)]
        public void ChangedPropertyIsTrueAfterCellAdditionTest()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("P51", "Mustang");

            Assert.IsTrue(ss.Changed);
        }

        [TestMethod(), Timeout(1000)]
        public void ChangedPropertyIsTrueAfterCellUpdateTest()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("P51", "Mustang");
            ss.SetContentsOfCell("P51", "NAA P-51 Mustang");

            Assert.IsTrue(ss.Changed);
        }

        [TestMethod(), Timeout(1000)]
        public void ChangedPropertyIsFalseAfterSavingTest()
        {
            Spreadsheet ss = new Spreadsheet(str => true, str => str.ToUpper(), "1st");
            ss.SetContentsOfCell("P51", "NAA P-51 Mustang");
            ss.SetContentsOfCell("B10", "Martin B-10 Bomber");

            Assert.IsTrue(ss.Changed);

            ss.Save("ChangedPropertyTest.xml");

            Assert.IsFalse(ss.Changed);
        }

        [TestMethod(), Timeout(1000)]
        public void ChangedPropertyIsFalseAfterCircularDependTest()
        {
            Spreadsheet ss = new Spreadsheet();
            try
            {
                ss.SetContentsOfCell("A0", "= A1 / A0");

            }
            catch (CircularException)
            {
                Assert.IsFalse(ss.Changed);
            }
        }

        /*****************************************************************************************/

        // Testing single cell additions with the three types.
        [TestMethod(), Timeout(1000)]
        public void SimpleAddNumberTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            IList<string> cell_list = ss.SetContentsOfCell("D01", ".1e+9");

            Assert.AreEqual(1, cell_list.Count);
            Assert.AreEqual("D01", cell_list[0]);
            Assert.AreEqual(.1e+9, ss.GetCellContents("D01"));
        }

        [TestMethod(), Timeout(1000)]
        public void SimpleAddTextTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            IList<string> cell_list = ss.SetContentsOfCell("T01", "Visuals Studioses");

            Assert.AreEqual(1, cell_list.Count);
            Assert.AreEqual("T01", cell_list[0]);
            Assert.AreEqual("Visuals Studioses", ss.GetCellContents("T01"));
        }

        [TestMethod(), Timeout(1000)]
        public void SimpleAddFormulaTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            IList<string> cell_list = ss.SetContentsOfCell("F01", "=P51 + B17");

            Assert.AreEqual(1, cell_list.Count);
            Assert.AreEqual("F01", cell_list[0]);
            Formula form = new Formula("P51 + B17");
            Assert.AreEqual(form, ss.GetCellContents("F01"));
        }

        /*****************************************************************************************/

        // Testing expected Exceptions are thrown by SetContentsOfCell for the 3 types,
        // using null, invalid, and empty names.
        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullNameInSetContentsOfCellTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string name = null;
            ss.SetContentsOfCell(name, "contents");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameInSetContentsOfCellTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string invalid = "#badname!";
            ss.SetContentsOfCell(invalid, "contents");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void EmptyNameInSetContentsOfCellTestA()
        {
            Spreadsheet ss = new Spreadsheet();
            string empty = "";
            ss.SetContentsOfCell(empty, "contents");
        }


        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullStringInSetContentsOfCellTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string contents = null;
            ss.SetContentsOfCell("T01", contents);
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NullFormulaInSetSetContentsOfCellTest()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("F01", "= A1 + A2 ?");
        }

        /*****************************************************************************************/

        // Testing a new cell with no contents (created with empty string).
        [TestMethod(), Timeout(1000)]
        public void SetContentsOfCellOfNewCellWithEmptyString()
        {
            Spreadsheet ss = new Spreadsheet();
            IList<string> cell_list = ss.SetContentsOfCell("A1", "");
            Assert.AreEqual(1, cell_list.Count);
            Assert.AreEqual("A1", cell_list[0]);
        }

        // Testing the getter method matches the setter.
        [TestMethod(), Timeout(1000)]
        public void GetCellContentsAfterSettingTest()
        {
            Spreadsheet ss = new Spreadsheet();
            double number = 6.02214076e+23;
            string text = "Avagadro's Number";
            Formula form = new Formula("kB1 * NA2");

            ss.SetContentsOfCell("A1", "6.02214076e+23");
            ss.SetContentsOfCell("B2", "Avagadro's Number");
            ss.SetContentsOfCell("C3", "= kB1 * NA2");

            Assert.AreEqual(number, ss.GetCellContents("A1"));
            Assert.AreEqual(text, ss.GetCellContents("B2"));
            Assert.AreEqual(form, ss.GetCellContents("C3"));
        }

        // Testing that cell contents can be rewritten.
        [TestMethod(), Timeout(1000)]
        public void GetCellContentsAfterRewriteTest()
        {
            Spreadsheet ss = new Spreadsheet();
            double number = 6.02214076e+23;
            string text = "Avagadro's Number";
            Formula form = new Formula("kB1 * NA2");

            ss.SetContentsOfCell("A1", "6.02214076e+23");
            ss.SetContentsOfCell("B2", "Avagadro's Number");
            ss.SetContentsOfCell("C3", "= kB1 * NA2");

            ss.SetContentsOfCell("A1", "Avagadro's Number");
            ss.SetContentsOfCell("B2", "= kB1* NA2");
            ss.SetContentsOfCell("C3", "6.02214076e+23");

            Assert.AreEqual(text, ss.GetCellContents("A1"));
            Assert.AreEqual(form, ss.GetCellContents("B2"));
            Assert.AreEqual(number, ss.GetCellContents("C3"));
        }

        // Getting the contents of an unassigned cell should return an empty string.
        [TestMethod(), Timeout(1000)]
        public void GetCellContentsEmptyCellTest()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "contents");

            Assert.AreEqual("", ss.GetCellContents("a1"));
        }

        /*****************************************************************************************/

        // Testing expected Exceptions from GetCellContents using null, invalid, & empty names.
        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullNameInGetCellContentsTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string invalid = null;
            ss.GetCellContents(invalid);
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameInGetCellContentsTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string invalid = "Shall_Not_Pass!";
            ss.GetCellContents(invalid);
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void EmptyNameInGetCellContentsTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string empty = "";
            ss.GetCellContents(empty);
        }

        /*****************************************************************************************/

        // Additional invalid name tests for code coverage.
        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameTooShortTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string invalid = "A";
            ss.GetCellContents(invalid);
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCharacterBetweenLettersTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string invalid = "QR$T123";
            ss.GetCellValue(invalid);
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCharacterBetweenNumbersTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string invalid = "Abcde12#45";
            ss.SetContentsOfCell(invalid, "");
        }

        /*****************************************************************************************/

        // Testing GetCellValue method.
        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValueWithNullNameTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string name = null;

            ss.GetCellValue(name);
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValueWithInvalidNameTest()
        {
            Spreadsheet ss = new Spreadsheet(str => false, str => str, "9/29/19");
            string name = "z0";

            ss.GetCellValue(name);
        }

        [TestMethod(), Timeout(1000)]
        public void SimpleGetCellValueTest()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("N0", "0.3");
            ss.SetContentsOfCell("S0", "test");
            ss.SetContentsOfCell("F0", "=(0.1 + 0.2) / N0");

            Assert.AreEqual(0.3, (double)ss.GetCellValue("N0"));
            Assert.AreEqual("test", (string)ss.GetCellValue("S0"));
            Assert.AreEqual(1.0, (double)ss.GetCellValue("F0"), 1e-6);
        }

        [TestMethod(), Timeout(1000)]
        public void GetCellValueWithFormulaErrorTest()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("E0", "= A1 + A2");

            Assert.IsTrue(ss.GetCellValue("E0") is FormulaError);
        }

        [TestMethod(), Timeout(1000)]
        public void GetCellValueOfEmptyCellTest()
        {
            Spreadsheet ss = new Spreadsheet();

            Assert.AreEqual("", (string)ss.GetCellValue("Z0"));
        }

        /*****************************************************************************************/

        // Basic test of GetNamesOfAllNonemptyCells method.
        [TestMethod(), Timeout(1000)]
        public void GetNonEmptyCellsTest()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("E0", "2.7182818284");
            ss.SetContentsOfCell("E1", "Euler's Number");
            ss.SetContentsOfCell("E2", "=2 + 1/2 + 1/6 + 1/24 + 1/120");

            IList<string> cell_list = new List<string>(ss.GetNamesOfAllNonemptyCells());

            Assert.AreEqual(3, cell_list.Count);
            Assert.IsTrue(cell_list.Contains("E0"));
            Assert.IsTrue(cell_list.Contains("E1"));
            Assert.IsTrue(cell_list.Contains("E2"));
            Assert.IsFalse(cell_list.Contains("e0"));
        }

        // GetNamesOfAllNonemptyCells should return an empty list when called on an empty spreadsheet.
        [TestMethod(), Timeout(1000)]
        public void GetNonEmptyCellsWithOnlyEmptyCellsTest()
        {
            Spreadsheet ss = new Spreadsheet();
            IList<string> cell_list = new List<string>(ss.GetNamesOfAllNonemptyCells());

            Assert.AreEqual(0, cell_list.Count);
        }

        // "Deleting" cell contents should mark them as empty.
        [TestMethod(), Timeout(1000)]
        public void GetNonEmptyCellsAfterRemovalTest()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("G0", "1.6180339887498948482");
            ss.SetContentsOfCell("G1", "Golden Ratio");
            ss.SetContentsOfCell("G2", "=(1 + 2.2360679775) / 2");

            IList<string> cell_list = new List<string>(ss.GetNamesOfAllNonemptyCells());

            Assert.AreEqual(3, cell_list.Count);
            Assert.IsTrue(cell_list.Contains("G0"));
            Assert.IsTrue(cell_list.Contains("G1"));
            Assert.IsTrue(cell_list.Contains("G2"));

            ss.SetContentsOfCell("G2", "");

            cell_list = new List<string>(ss.GetNamesOfAllNonemptyCells());

            Assert.AreEqual(2, cell_list.Count);
            Assert.IsTrue(cell_list.Contains("G0"));
            Assert.IsTrue(cell_list.Contains("G1"));
            Assert.IsFalse(cell_list.Contains("G2"));

            ss.SetContentsOfCell("G1", "");

            cell_list = new List<string>(ss.GetNamesOfAllNonemptyCells());

            Assert.AreEqual(1, cell_list.Count);
            Assert.IsTrue(cell_list.Contains("G0"));
            Assert.IsFalse(cell_list.Contains("G1"));

            ss.SetContentsOfCell("G0", "");

            cell_list = new List<string>(ss.GetNamesOfAllNonemptyCells());

            Assert.AreEqual(0, cell_list.Count);
            Assert.IsFalse(cell_list.Contains("G0"));
        }

        /*****************************************************************************************/

        // Testing expected Exception when circular dependency is introduced and that no changes
        // are made to the state of the spreadsheet when CircularException is thrown.
        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(CircularException))]
        public void NewCellWithCircularDependencyTest()
        {
            Spreadsheet ss = new Spreadsheet();
            Formula form = new Formula("X1 + 1.0");
            ss.SetContentsOfCell("X1", "=X1 + 1.0");
        }

        [TestMethod(), Timeout(1000)]
        public void NewCellWithCircularDependencyNoChangesTest()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("F1", "= F2 * 4.2");
            ss.SetContentsOfCell("F2", "= F3 - 5.3");
            ss.SetContentsOfCell("F3", "= F0 / F4");

            List<string> before = new List<string>(ss.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(3, before.Count);
            Assert.IsTrue(before.Contains("F1"));
            Assert.IsTrue(before.Contains("F2"));
            Assert.IsTrue(before.Contains("F3"));

            try
            {
                ss.SetContentsOfCell("F0", "=(F1 + F2 + F3 + F4) / F0");
            }
            catch (CircularException)
            {
                List<string> after = new List<string>(ss.GetNamesOfAllNonemptyCells());
                Assert.AreEqual(3, after.Count);
                Assert.IsFalse(after.Contains("F0"));
                Assert.IsTrue(after.Contains("F1"));
                Assert.IsTrue(after.Contains("F2"));
                Assert.IsTrue(after.Contains("F3"));
            }
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(CircularException))]
        public void UpdateCellWithCircularDependencyTest()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("L0", ".48151623e+42");
            ss.SetContentsOfCell("L0", "=L0");
        }

        [TestMethod(), Timeout(1000)]
        public void UpdateCellWithCircularDependencyNoChangesTest()
        {
            Spreadsheet ss = new Spreadsheet();
            Formula F0 = new Formula("(1.414213562373 + 1.73205080757) / F4");

            ss.SetContentsOfCell("F0", "=(1.414213562373 + 1.73205080757) / F4");
            ss.SetContentsOfCell("F1", "=F2 * 4");
            ss.SetContentsOfCell("F2", "=F3 - 5");
            ss.SetContentsOfCell("F3", "= F0 / F4 ");

            List<string> before = new List<string>(ss.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(4, before.Count);
            Assert.IsTrue(before.Contains("F0"));
            Assert.IsTrue(before.Contains("F1"));
            Assert.IsTrue(before.Contains("F2"));
            Assert.IsTrue(before.Contains("F3"));

            try
            {
                ss.SetContentsOfCell("F0", "=F3");
            }
            catch (CircularException)
            {
                List<string> after = new List<string>(ss.GetNamesOfAllNonemptyCells());
                Assert.AreEqual(4, after.Count);
                Assert.IsTrue(after.Contains("F0"));
                Assert.IsTrue(after.Contains("F1"));
                Assert.IsTrue(after.Contains("F2"));
                Assert.IsTrue(after.Contains("F3"));

                Formula F0_after = (Formula)ss.GetCellContents("F0");
                Assert.IsTrue(F0 == F0_after);
            }
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(CircularException))]
        public void UpdateStringWithCircularDependencyTest()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("S1", "S2 - 1");
            ss.SetContentsOfCell("S2", "=S1 + 1");

            ss.SetContentsOfCell("S1", "=S2 - 1");
        }

        /*****************************************************************************************/

        // Tests for the Spreadsheet.Save method.
        [TestMethod(), Timeout(1000)]
        public void SaveSpreadsheetToXMLTest()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("E0", "");
            ss.SetContentsOfCell("E1", "2.7182818284");
            ss.SetContentsOfCell("E2", "Euler's Number");
            ss.SetContentsOfCell("E3", "= 2 + 1/2 + 1/6 + 1/24 + 1/120");

            ss.Save("SaveSpreadsheetTest01.xml");

            Assert.IsTrue(File.Exists("SaveSpreadsheetTest01.xml"));
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveSpreadsheetWithBadFilenameTest()
        {
            Spreadsheet ss = new Spreadsheet(str => true, str => str, "v1.0");
            ss.Save("/bad/path/no_file.txt");
        }

        /*****************************************************************************************/

        // Tests for the Spreadsheet.GetSavedVersion method.
        [TestMethod(), Timeout(1000)]
        public void GetVersionFromFileTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("GetVersionTest01.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.0");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "= B2 + C3");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet();
            string version = ss.GetSavedVersion("GetVersionTest01.xml");
            Assert.AreEqual("v1.0", version);
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetVersionFromBadFilenameTest()
        {
            Spreadsheet ss = new Spreadsheet();
            string version = ss.GetSavedVersion("/bad/path/no_file.txt");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetVersionFromFileWithNoVersionTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("GetVersionTest02.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "2B");
                writer.WriteElementString("contents", "2B || !2B");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet();
            string version = ss.GetSavedVersion("GetVersionTest02.xml");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetVersionFromMalformedXMLTestA()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("GetVersionTest03.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("CELLS");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "steaksauce");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B2");
                writer.WriteElementString("contents", "bomber");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "C3");
                writer.WriteElementString("contents", "PO");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet();
            ss.GetSavedVersion("GetVersionTest03.xml");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetVersionFromMalformedXMLTestB()
        {
            using (StreamWriter writer = File.CreateText("GetVersionTest04.xml"))
            {
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                writer.WriteLine("<root>");
                writer.WriteLine("</root>");
            }

            Spreadsheet ss = new Spreadsheet();
            ss.GetSavedVersion("GetVersionTest04.xml");
        }

        [TestMethod(), Timeout(1000)]
        public void GetVersionFromXMLWithCommentBeforeRoot()
        {
            using (StreamWriter writer = File.CreateText("GetVersionTest05.xml"))
            {
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                writer.WriteLine("<!--Comment node: Should this be allowed or not?-->");
                writer.WriteLine("<spreadsheet version=\"commented\">");
                writer.WriteLine("</spreadsheet>");
            }

            Spreadsheet ss = new Spreadsheet();
            string version = ss.GetSavedVersion("GetVersionTest05.xml");
            Assert.AreEqual("commented", version);
        }

        /*****************************************************************************************/

        // Testing constructor that reads from XML file.
        [TestMethod(), Timeout(1000)]
        public void ReadSimpleXMLSpreadsheetTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest01.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.0");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "C4");
                writer.WriteElementString("contents", "KABOOM!");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet("ReadSpreadsheetTest01.xml", str => true, str => str, "v1.0");

            List<string> cell_list = new List<string>(ss.GetNamesOfAllNonemptyCells());

            Assert.AreEqual(1, cell_list.Count);
            Assert.IsTrue(cell_list.Contains("C4"));
            Assert.AreEqual("KABOOM!", ss.GetCellContents("C4"));
            Assert.IsFalse(ss.Changed);
        }

        [TestMethod(), Timeout(1000)]
        public void ReadXMLSpreadsheetContentsBeforeNameTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest02.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v2.0");
                writer.WriteStartElement("cell");
                writer.WriteElementString("contents", "steak sauce");
                writer.WriteElementString("name", "A1");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest02.xml", str => true, str => str, "v2.0");

            List<string> cell_list = new List<string>(ss.GetNamesOfAllNonemptyCells());

            Assert.AreEqual(1, cell_list.Count);
            Assert.IsTrue(cell_list.Contains("A1"));
            Assert.AreEqual("steak sauce", ss.GetCellContents("A1"));
            Assert.IsFalse(ss.Changed);
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetVersionMismatchTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest03.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "1.0");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "R2");
                writer.WriteElementString("contents", "D2");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest03.xml", str => true, str => str, "2.0");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetCellWithoutContentsTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest04.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "Z0");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest04.xml", str => true, str => str, "default");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetNoSpreadsheetElementTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest05.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A0");
                writer.WriteElementString("contents", "= A1 - A2");
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest05.xml", str => true, str => str, "default");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetNameBeforeCellTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest06.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");
                writer.WriteElementString("name", "A0");
                writer.WriteStartElement("cell");
                writer.WriteElementString("contents", "= A1 - A2");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest06.xml", str => true, str => str, "");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetContentsBeforeCellTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest07.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");
                writer.WriteElementString("contents", "= A1 - A2");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A0");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest07.xml", str => true, str => str, "");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetInvalidElementTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest08.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "beta");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B52");
                writer.WriteElementString("contents", "Boeing B-52 Stratofortress");
                writer.WriteElementString("year", "1952");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest08.xml", str => true, str => str, "beta");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetInvalidCellNameTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest09.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "alpha");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B-1");
                writer.WriteElementString("contents", "Rockwell B-1 Lancer");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest09.xml", str => true, str => str, "alpha");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetInvalidFormulaTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest10.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "charlie");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B21");
                writer.WriteElementString("contents", "= Northrop Grumman B-21 Raider");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest10.xml", str => true, str => str, "charlie");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetWithCircularDependencyTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest11.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "delta");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "AA88");
                writer.WriteElementString("contents", "= BB33 / 1.1 + II55");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "BB33");
                writer.WriteElementString("contents", "3.3");
                writer.WriteEndElement();


                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "II55");
                writer.WriteElementString("contents", "= UU22 - AA88");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest11.xml", str => true, str => str, "delta");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetWithTwoSpreadsheetsTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest12.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "foxtrot");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "CS3500");
                writer.WriteElementString("contents", "Software Practice");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "ECE3530");
                writer.WriteElementString("contents", "Engineering Probability & Statistics");
                writer.WriteEndElement();

                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "foxtrot");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "ECE2240");
                writer.WriteElementString("contents", "Introduction to Electric Circuits");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest12.xml", str => true, str => str, "foxtrot");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetWithNewCellBeforeContentsTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest13.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "golf");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "CS3500");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "ECE3530");
                writer.WriteElementString("contents", "Engineering Probability & Statistics");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest13.xml", str => true, str => str, "golf");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetWithNewCellBeforeNameTest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create("ReadSpreadsheetTest14.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "hotel");

                writer.WriteStartElement("cell");
                writer.WriteElementString("contents", "Software Practice");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "ECE3530");
                writer.WriteElementString("contents", "Engineering Probability & Statistics");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Spreadsheet ss = new Spreadsheet("ReadSpreadsheetTest14.xml", str => true, str => str, "hotel");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadXMLSpreadsheetInvalidFilepathTest()
        {
            Spreadsheet ss = new Spreadsheet("/nonsense/no_file_here.xml", str => true, str => str, "echo");
        }


        /*****************************************************************************************/

        // Testing Constructors with null arguments.
        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void FourArgumentConstructWithNullFilepath()
        {
            string filepath = null;
            AbstractSpreadsheet ss = new Spreadsheet(filepath, str => true, str => str, "v1.0");
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FourArgumentConstructWithNullVersion()
        {
            string version = null;
            AbstractSpreadsheet ss = new Spreadsheet("doesnotexist.xml", str => true, str => str, version);
        }

        [TestMethod(), Timeout(1000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThreeArgumentConstructWithNullVersion()
        {
            string version = null;
            AbstractSpreadsheet ss = new Spreadsheet(str => true, str => str, version);
        }

        /*****************************************************************************************/

        // Stress tests.
        [TestMethod(), Timeout(5000)]
        public void StressTestWithoutReadWriteA()
        {
            Spreadsheet ss = new Spreadsheet(str => { return (str.Length == 4); }, str => str.ToUpper(), "stress");
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    string cell_name = GetRandomCellName();
                    string contents = GetRandomContents();
                    ss.SetContentsOfCell(cell_name, contents);
                }
            }
            catch (CircularException)
            {
                Console.WriteLine("CircularException enountered in StressTestWithoutReadWrite.");
            }

        }
        [TestMethod(), Timeout(5000)]
        public void StressTestWithoutReadWriteB()
        {
            StressTestWithoutReadWriteA();
        }
        [TestMethod(), Timeout(5000)]
        public void StressTestWithoutReadWriteC()
        {
            StressTestWithoutReadWriteA();
        }

        [TestMethod(), Timeout(5000)]
        public void StressTestBuildAndSaveA()
        {
            Spreadsheet ss = new Spreadsheet(str => { return (str.Length == 4); }, str => str.ToUpper(), "stressWA");
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    string cell_name = GetRandomCellName();
                    string contents = GetRandomContents();
                    ss.SetContentsOfCell(cell_name, contents);
                }
            }
            catch (CircularException)
            {
                Console.WriteLine("CircularException enountered in StressTestBuildAndSaveA.");
            }
            finally
            {
                ss.Save("WriteSpreadsheetStressTestA.xml");
                Assert.IsTrue(File.Exists("WriteSpreadsheetStressTestA.xml"));
            }
        }
        [TestMethod(), Timeout(5000)]
        public void StressTestBuildAndSaveB()
        {
            Spreadsheet ss = new Spreadsheet(str => { return (str.Length == 4); }, str => str.ToUpper(), "stressWB");
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    string cell_name = GetRandomCellName();
                    string contents = GetRandomContents();
                    ss.SetContentsOfCell(cell_name, contents);
                }
            }
            catch (CircularException)
            {
                Console.WriteLine("CircularException enountered in StressTestBuildAndSaveB.");
            }
            finally
            {
                ss.Save("WriteSpreadsheetStressTestB.xml");
                Assert.IsTrue(File.Exists("WriteSpreadsheetStressTestB.xml"));
            }
        }
        [TestMethod(), Timeout(5000)]
        public void StressTestBuildAndSaveC()
        {
            Spreadsheet ss = new Spreadsheet(str => { return (str.Length == 4); }, str => str.ToUpper(), "stressWC");
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    string cell_name = GetRandomCellName();
                    string contents = GetRandomContents();
                    ss.SetContentsOfCell(cell_name, contents);
                }
            }
            catch (CircularException)
            {
                Console.WriteLine("CircularException enountered in StressTestBuildAndSaveC.");
            }
            finally
            {
                ss.Save("WriteSpreadsheetStressTestC.xml");
                Assert.IsTrue(File.Exists("WriteSpreadsheetStressTestC.xml"));
            }
        }

        [TestMethod(), Timeout(5000)]
        public void StressTestReadInFileA()
        {
            string filename = "ReadSpreadsheetStressTestA.xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "stressRA");

                for (int i = 0; i < 1000; i++)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", GetRandomCellName());
                    writer.WriteElementString("contents", GetRandomContents());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            try
            {
                Spreadsheet ss = new Spreadsheet(filename, str => { return (str.Length == 4); }, str => str.ToUpper(), "stressRA");
            }
            catch (CircularException)
            {
                Console.WriteLine("CircularException enountered in StressTestReadInFileA.");
            }
        }
        [TestMethod(), Timeout(5000)]
        public void StressTestReadInFileB()
        {
            string filename = "ReadSpreadsheetStressTestB.xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "stressRB");

                for (int i = 0; i < 1000; i++)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", GetRandomCellName());
                    writer.WriteElementString("contents", GetRandomContents());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            try
            {
                Spreadsheet ss = new Spreadsheet(filename, str => { return (str.Length == 4); }, str => str.ToUpper(), "stressRB");
            }
            catch (CircularException)
            {
                Console.WriteLine("CircularException enountered in StressTestReadInFileB.");
            }
        }
        [TestMethod(), Timeout(5000)]
        public void StressTestReadInFileC()
        {
            string filename = "ReadSpreadsheetStressTestC.xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "stressRC");

                for (int i = 0; i < 1000; i++)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", GetRandomCellName());
                    writer.WriteElementString("contents", GetRandomContents());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            try
            {
                Spreadsheet ss = new Spreadsheet(filename, str => { return (str.Length == 4); }, str => str.ToUpper(), "stressRC");
            }
            catch (CircularException)
            {
                Console.WriteLine("CircularException enountered in StressTestReadInFileC.");
            }
        }

        /*****************************************************************************************/

        // PROVIDED PS5 GRADING TESTS

        // Verifies cells and their values, which must alternate.
        public void VV(AbstractSpreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }


        // For setting a spreadsheet cell.
        public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod, Timeout(5000)]
        public void IsValidTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void IsValidTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("A1", "x");
        }

        [TestMethod, Timeout(5000)]
        public void IsValidTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsValidTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        // Tests Normalize
        [TestMethod, Timeout(5000)]
        public void NormalizeTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("", s.GetCellContents("b1"));
        }

        [TestMethod, Timeout(5000)]
        public void NormalizeTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod, Timeout(5000)]
        public void NormalizeTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("A1", "6");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod, Timeout(5000)]
        public void NormalizeTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod, Timeout(5000)]
        public void EmptySheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            VV(ss, "A1", "");
        }


        [TestMethod, Timeout(5000)]
        public void OneString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        public void OneString(AbstractSpreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VV(ss, "B1", "hello");
        }


        [TestMethod, Timeout(5000)]
        public void OneNumber()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        public void OneNumber(AbstractSpreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VV(ss, "C1", 17.5);
        }


        [TestMethod, Timeout(5000)]
        public void OneFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        public void OneFormula(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod, Timeout(5000)]
        public void Changed()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }


        [TestMethod, Timeout(5000)]
        public void DivisionByZero1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        public void DivisionByZero1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod, Timeout(5000)]
        public void DivisionByZero2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        public void DivisionByZero2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod, Timeout(5000)]
        public void EmptyArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        public void EmptyArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(5000)]
        public void StringArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        public void StringArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(5000)]
        public void ErrorArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        public void ErrorArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(5000)]
        public void NumberFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        public void NumberFormula1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VV(ss, "C1", 8.3);
        }


        [TestMethod, Timeout(5000)]
        public void NumberFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        public void NumberFormula2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VV(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod, Timeout(5000)]
        public void RepeatSimpleTests()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod, Timeout(5000)]
        public void Formulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod, Timeout(5000)]
        public void Formulasa()
        {
            Formulas();
        }

        [TestMethod, Timeout(5000)]
        public void Formulasb()
        {
            Formulas();
        }


        // Are multiple spreadsheets supported?
        [TestMethod, Timeout(5000)]
        public void Multiple()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            Set(s1, "X1", "hello");
            Set(s2, "X1", "goodbye");
            VV(s1, "X1", "hello");
            VV(s2, "X1", "goodbye");
        }

        [TestMethod, Timeout(5000)]
        public void Multiplea()
        {
            Multiple();
        }

        [TestMethod, Timeout(5000)]
        public void Multipleb()
        {
            Multiple();
        }

        [TestMethod, Timeout(5000)]
        public void Multiplec()
        {
            Multiple();
        }

        // Reading/writing spreadsheets
        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save(Path.GetFullPath("/missing/save.txt"));
        }

        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(Path.GetFullPath("/missing/save.txt"), s => true, s => s, "");
        }

        [TestMethod, Timeout(5000)]
        public void SaveTest3()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            Set(s1, "A1", "hello");
            s1.Save("save1.txt");
            s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual("hello", s1.GetCellContents("A1"));
        }

        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest4()
        {
            using (StreamWriter writer = new StreamWriter("save2.txt"))
            {
                writer.WriteLine("This");
                writer.WriteLine("is");
                writer.WriteLine("a");
                writer.WriteLine("test!");
            }
            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
        }

        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest5()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("save3.txt");
            ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
        }

        [TestMethod, Timeout(5000)]
        public void SaveTest6()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
            ss.Save("save4.txt");
            Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
        }

        [TestMethod, Timeout(5000)]
        public void SaveTest7()
        {
            using (XmlWriter writer = XmlWriter.Create("save5.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "5.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A3");
                writer.WriteElementString("contents", "4.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A4");
                writer.WriteElementString("contents", "= A2 + A3");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
            VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
        }

        [TestMethod, Timeout(5000)]
        public void SaveTest8()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "hello");
            Set(ss, "A2", "5.0");
            Set(ss, "A3", "4.0");
            Set(ss, "A4", "= A2 + A3");
            ss.Save("save6.txt");
            using (XmlReader reader = XmlReader.Create("save6.txt"))
            {
                int spreadsheetCount = 0;
                int cellCount = 0;
                bool A1 = false;
                bool A2 = false;
                bool A3 = false;
                bool A4 = false;
                string name = null;
                string contents = null;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Assert.AreEqual("default", reader["version"]);
                                spreadsheetCount++;
                                break;

                            case "cell":
                                cellCount++;
                                break;

                            case "name":
                                reader.Read();
                                name = reader.Value;
                                break;

                            case "contents":
                                reader.Read();
                                contents = reader.Value;
                                break;
                        }
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "cell":
                                if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                else Assert.Fail();
                                break;
                        }
                    }
                }
                Assert.AreEqual(1, spreadsheetCount);
                Assert.AreEqual(4, cellCount);
                Assert.IsTrue(A1);
                Assert.IsTrue(A2);
                Assert.IsTrue(A3);
                Assert.IsTrue(A4);
            }
        }


        // Fun with formulas
        [TestMethod, Timeout(5000)]
        public void Formula1()
        {
            Formula1(new Spreadsheet());
        }
        public void Formula1(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= b1 + b2");
            Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
            Set(ss, "a3", "5.0");
            Set(ss, "b1", "2.0");
            Set(ss, "b2", "3.0");
            VV(ss, "a1", 10.0, "a2", 5.0);
            Set(ss, "b2", "4.0");
            VV(ss, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod, Timeout(5000)]
        public void Formula2()
        {
            Formula2(new Spreadsheet());
        }
        public void Formula2(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= a3");
            Set(ss, "a3", "6.0");
            VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Set(ss, "a3", "5.0");
            VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod, Timeout(5000)]
        public void Formula3()
        {
            Formula3(new Spreadsheet());
        }
        public void Formula3(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a3 + a5");
            Set(ss, "a2", "= a5 + a4");
            Set(ss, "a3", "= a5");
            Set(ss, "a4", "= a5");
            Set(ss, "a5", "9.0");
            VV(ss, "a1", 18.0);
            VV(ss, "a2", 18.0);
            Set(ss, "a5", "8.0");
            VV(ss, "a1", 16.0);
            VV(ss, "a2", 16.0);
        }

        [TestMethod, Timeout(5000)]
        public void Formula4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula1(ss);
            Formula2(ss);
            Formula3(ss);
        }

        [TestMethod, Timeout(5000)]
        public void Formula4a()
        {
            Formula4();
        }


        [TestMethod, Timeout(5000)]
        public void MediumSheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
        }

        public void MediumSheet(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "1.0");
            Set(ss, "A2", "2.0");
            Set(ss, "A3", "3.0");
            Set(ss, "A4", "4.0");
            Set(ss, "B1", "= A1 + A2");
            Set(ss, "B2", "= A3 * A4");
            Set(ss, "C1", "= B1 + B2");
            VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Set(ss, "A1", "2.0");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Set(ss, "B1", "= A1 / A2");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod, Timeout(5000)]
        public void MediumSheeta()
        {
            MediumSheet();
        }


        [TestMethod, Timeout(5000)]
        public void MediumSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
            ss.Save("save7.txt");
            ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod, Timeout(5000)]
        public void MediumSavea()
        {
            MediumSave();
        }


        // A long chained formula. Solutions that re-evaluate 
        // cells on every request, rather than after a cell changes,
        // will timeout on this test.
        // This test is repeated to increase its scoring weight
        [TestMethod, Timeout(7000)]
        public void LongFormulaTest()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(7000)]
        public void LongFormulaTest2()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(7000)]
        public void LongFormulaTest3()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(7000)]
        public void LongFormulaTest4()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(7000)]
        public void LongFormulaTest5()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        public void LongFormulaHelper(out object result)
        {
            try
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("sum1", "= a1 + a2");
                int i;
                int depth = 100;
                for (i = 1; i <= depth * 2; i += 2)
                {
                    s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                    s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                }
                s.SetContentsOfCell("a" + i, "1");
                s.SetContentsOfCell("a" + (i + 1), "1");
                Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + i, "0");
                Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + (i + 1), "0");
                Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
                result = "ok";
            }
            catch (Exception e)
            {
                result = e;
            }
        }

        /*****************************************************************************************/

        /// <summary>
        /// Generates a random cell name in the form: "jk42".
        /// </summary>
        /// <returns>A string 4 char long with 2 letters followed by 2 numbers.</returns>
        private String GetRandomCellName()
        {
            Random rand = new Random();
            char[] char_arr = new char[4];
            char_arr[0] = (char)rand.Next(97, 123);
            char_arr[1] = (char)rand.Next(97, 123);
            char_arr[2] = (char)rand.Next(48, 58);
            char_arr[3] = (char)rand.Next(48, 58);
            return new string(char_arr);
        }

        /// <summary>
        /// Generates random cell contents.
        /// </summary>
        /// <returns>A string representing a double, string, or Formula.</returns>
        private string GetRandomContents()
        {
            Random rand = new Random();

            switch (rand.Next(6))
            {
                case 0:
                    return "";

                case 1:
                    return "Text";

                case 2:
                    return "6.02214e23";

                case 3:
                    return "2.71828182";

                case 4:
                    return "=" + GetRandomCellName();

                case 5:
                default:
                    return GetRandomFormula(rand);
            }
        }

        /// <summary>
        /// Generates a random Formula.
        /// </summary>
        /// <returns>A string representing a Formula prepended with '='.</returns>
        private string GetRandomFormula(Random rand)
        {
            StringBuilder form = new StringBuilder("= ");
            string append = "";

            for(int i = 0; i < 5; i++)
            {
                if(i % 2 == 0)
                {
                    switch (rand.Next(4))
                    {
                        case 0:
                            append = ".999";
                            break;

                        case 1:
                            append = (rand.Next(99) + 1).ToString();
                            break;

                        case 2:
                            append = GetRandomCellName();
                            break;

                        case 3:
                            append = "(" + GetRandomCellName() + " + " + GetRandomCellName() + ")";
                            break;
                    }
                }
                else
                {
                    switch (rand.Next(4))
                    {
                        case 0:
                            append = " + ";
                            break;

                        case 1:
                            append = " - ";
                            break;

                        case 2:
                            append = " * ";
                            break;

                        case 3:
                            append = " / ";
                            break;
                    }
                }
                form.Append(append);
            }
            return form.ToString();
        }
    }
}
