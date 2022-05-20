using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace FormulaTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod(), Timeout(5000)]
        public void TestSingleNumber()
        {
            Formula five = new Formula("5");
            Assert.AreEqual(5.0, five.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestSingleVariable()
        {
            Formula thirteen = new Formula("X5");
            Assert.AreEqual(13.0, thirteen.Evaluate(s => 13));
        }

        [TestMethod(), Timeout(5000)]
        public void TestAddition()
        {
            Formula eight = new Formula("5+3");
            Assert.AreEqual(8.0, eight.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestSubtraction()
        {
            Formula eight = new Formula("18-10");
            Assert.AreEqual(8.0, eight.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestMultiplication()
        {
            Formula eight = new Formula("2*4");
            Assert.AreEqual(8.0, eight.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivision()
        {
            Formula eight = new Formula("16/2");
            Assert.AreEqual(8.0, eight.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestArithmeticWithVariable()
        {
            Formula six = new Formula("2+X1");
            Assert.AreEqual(6.0, six.Evaluate(s => 4));
        }

        [TestMethod(), Timeout(5000)]
        public void TestUnknownVariable()
        {
            Formula unknown = new Formula("2+X1");
            Assert.IsInstanceOfType(unknown.Evaluate(s => { throw new ArgumentException("Unknown variable."); }), typeof(FormulaError));
        }

        [TestMethod(), Timeout(5000)]
        public void TestLeftToRight()
        {
            Formula fifteen = new Formula("2*6+3");
            Assert.AreEqual(15.0, fifteen.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestOrderOperations()
        {
            Formula twenty = new Formula("2+6*3");
            Assert.AreEqual(20.0, twenty.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestParenthesesTimes()
        {
            Formula twentyfour = new Formula("(2+6)*3");
            Assert.AreEqual(24.0, twentyfour.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestTimesParentheses()
        {
            Formula sixteen = new Formula("2*(3+5)");
            Assert.AreEqual(16.0, sixteen.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestPlusParentheses()
        {
            Formula ten = new Formula("2+(3+5)");
            Assert.AreEqual(10.0, ten.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestPlusComplex()
        {
            Formula fifty = new Formula("2+(3+5*9)");
            Assert.AreEqual(50.0, fifty.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestOperatorAfterParens()
        {
            Formula one = new Formula("(1*1)-2/2");
            Assert.AreEqual(0.0, one.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexTimesParentheses()
        {
            Formula twentysix = new Formula("2+3*(3+5)");
            Assert.AreEqual(26.0, twentysix.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexAndParentheses()
        {
            Formula onenintyfour = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(194.0, onenintyfour.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivideByZero()
        {
            Formula undefined = new Formula("5/0");
            Assert.IsInstanceOfType(undefined.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivideByZeroDouble()
        {
            Formula undefined = new Formula("5.0 / 0.0");
            Assert.IsInstanceOfType(undefined.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula plus = new Formula("+");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula extra = new Formula("2+5+");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraParentheses()
        {
            Formula imbalanced = new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidVariable()
        {
            Formula bad_var = new Formula("C#");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestPlusInvalidVariable()
        {
            Formula bad_var = new Formula("5+C#");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestParensNoOperator()
        {
            Formula implied = new Formula("5+7+(5)8");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula empty = new Formula("");
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexMultiVar()
        {
            Formula five_and_change = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.14285714285714, (double)five_and_change.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexNestedParensRight()
        {
            Formula six = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6.0, six.Evaluate(s => 1));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexNestedParensLeft()
        {
            Formula twelve = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12.0, twelve.Evaluate(s => 2));
        }

        [TestMethod(), Timeout(5000)]
        public void TestRepeatedVar()
        {
            Formula zero = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0.0, zero.Evaluate(s => 3));
        }


        [TestMethod(), Timeout(1000)]
        public void TestDivideByZeroInParatheses()
        {
            Formula undefined = new Formula("5 / (Z0)");
            Assert.IsInstanceOfType(undefined.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod(), Timeout(1000)]
        public void TestGetVariablesJustOne()
        {
            Formula one_var = new Formula("1 + C4 / 2");
            IEnumerable<string> variables = one_var.GetVariables();
            List<string> var_list = new List<string>();
            foreach (string var in variables)
            {
                var_list.Add(var);
            }
            Assert.AreEqual(1, var_list.Count);
            Assert.AreEqual("C4", var_list[0]);
        }

        [TestMethod(), Timeout(1000)]
        public void TestGetVariablesWithNone()
        {
            Formula no_var = new Formula("1 + 4 / 2");
            IEnumerable<string> variables = no_var.GetVariables();
            List<string> var_list = new List<string>();
            foreach (string var in variables)
            {
                var_list.Add(var);
            }
            Assert.AreEqual(0, var_list.Count);
        }

        [TestMethod(), Timeout(1000)]
        public void TestGetVariablesWithSeveral()
        {
            Formula six_var = new Formula("1 + C4 / 2 + (x9 - x8) * D4 * D1 - 3 * (C4 / x1)");
            IEnumerable<string> variables = six_var.GetVariables();
            List<string> var_list = new List<string>();
            foreach (string var in variables)
            {
                var_list.Add(var);
            }
            Assert.AreEqual(6, var_list.Count);
            Assert.IsTrue(var_list.Contains("C4"));
            Assert.IsTrue(var_list.Contains("x9"));
            Assert.IsTrue(var_list.Contains("x8"));
            Assert.IsTrue(var_list.Contains("D4"));
            Assert.IsTrue(var_list.Contains("D1"));
            Assert.IsTrue(var_list.Contains("x1"));
        }

        [TestMethod(), Timeout(1000)]
        public void TestToString()
        {
            Formula form = new Formula("1 + 4 / 2 - 3");
            Assert.AreEqual("1+4/2-3", form.ToString());
        }

        [TestMethod(), Timeout(1000)]
        public void HashedFormulasAreEqual()
        {
            Formula form1 = new Formula("1 + 4 / 2 - 3");
            Formula form2 = new Formula("1+4/2-3");
            Assert.AreEqual(form1.GetHashCode(), form2.GetHashCode());
        }

    }

}
