using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormulaEvaluator;

namespace DemoFormulaEvaluator
{
    /// <summary>
    /// This program demonstrates the use of FormulaEvaluator for testing purposes.
    /// </summary>
    class Program
    {
        //Declare a dictionary to store varialbe values.
        private static Dictionary<string, int> variables;

        static void Main(string[] args)
        {
            //Initialize the variables dictionary.
            variables = new Dictionary<string, int>();
            //Call helper method to fill the dictionary.
            BuildVarTable();

            Console.WriteLine("Demonstration of FormulaEvaluator which processes arithmetic expressions using infix notation.\n");
            System.Threading.Thread.Sleep(1000);

            //This block demos valid expressions without variables.
            string example633 = "16 / 4 + 23 * (42 - 15) + 8";
            PrintExample(example633, 633);
            string exampleNeg5 = "(15 - 16) * 23 + 42 / 4 + 8";
            PrintExample(exampleNeg5, -5);
            string example17 = "((42 + 4) / 16) * 8 + 23 / 15";
            PrintExample(example17, 17);
            string example42 = "42";
            PrintExample(example42, 42);
            string exampleWithSpaces = "   1   +   1   ";
            PrintExample(exampleWithSpaces, 2);
            string examplePara = "((0))";
            PrintExample(examplePara, 0);
            string exampleLead0 = "0100 / 0010";
            PrintExample(exampleLead0, 10);
            string exampleEmptyPara = "2() + ()()3()";
            PrintExample(exampleEmptyPara, 5);

            //This block demos valid expressions with variables.
            PrintVarTable();
            PrintExample(" A01 + C03 ", 10);
            PrintExample("(Ten03 - C03) / A03 - B03", 1);
            PrintExample("C02/4 * (12-Ten01) * ( ( B03-4 ) )", 8);

            //This block demos invalid or improperly formated expressions.
            Console.WriteLine("An improperly formated expression will throw an ArgumentExpression.\n");
            string invalid = "+1";
            TryInvalid(invalid);
            invalid = "7 / 0";
            TryInvalid(invalid);
            invalid = "10(3 + 2)";
            TryInvalid(invalid);
            invalid = "";
            TryInvalid(invalid);
            invalid = "()";
            TryInvalid(invalid);
            invalid = "L33T";
            TryInvalid(invalid);

            //This block prompts user to input expressions to test.
            Console.Write("\nEnter your own infix expression to evaluate:   ");
            string input = Console.ReadLine();
            UserInput(input);

            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }

        /// <summary>
        /// Private helper method adds some variables to a dictionary for lookup.
        /// </summary>
        private static void BuildVarTable()
        {
            variables.Add("A01", 1); variables.Add("A02", 2); variables.Add("A03", 3);
            variables.Add("B01", 4); variables.Add("B02", 5); variables.Add("B03", 6);
            variables.Add("C01", 7); variables.Add("C02", 8); variables.Add("C03", 9);
            variables.Add("Ten01", 10); variables.Add("Ten02", 20); variables.Add("Ten03", 30);
        }

        /// <summary>
        /// Private helper method prints a table of the variable names and their values.
        /// </summary>
        private static void PrintVarTable()
        {
            Console.WriteLine("Previously stored variables for lookup:\n");
            Console.WriteLine("\t     |  A  |  B  |  C  | Ten |");
            Console.WriteLine("\t-----|-----|-----|-----|-----|");
            Console.WriteLine("\t  01 |  1  |  4  |  7  |  10 |");
            Console.WriteLine("\t-----|-----|-----|-----|-----|");
            Console.WriteLine("\t  02 |  2  |  5  |  8  |  20 |");
            Console.WriteLine("\t-----|-----|-----|-----|-----|");
            Console.WriteLine("\t  03 |  3  |  6  |  9  |  30 |");
            Console.WriteLine("\t-----|-----|-----|-----|-----|\n\n");
        }

        /// <summary>
        /// Wraps the dictionary getter method into a valid lookup type for Evaluate(string, Lookup).
        /// </summary>
        /// <param name="var">Name of the variable.</param>
        /// <returns>Value of the variable.</returns>
        private static int LookupVarVal(string var)
        {
            return variables[var];
        }

        /// <summary>
        /// Private helper method prints valid expressions, their expected result, and their evaluated result.
        /// </summary>
        /// <param name="expression">A valid infix expression.</param>
        /// <param name="expected">The expected result.</param>
        private static void PrintExample(string expression, int expected)
        {
            Console.WriteLine("Example:     \"" + expression + "\"  should return " + expected + ".");
            System.Threading.Thread.Sleep(500);
            Console.Write("Processing:   " + expression);
            System.Threading.Thread.Sleep(100);
            Console.Write(" .");
            System.Threading.Thread.Sleep(100);
            Console.Write(" .");
            System.Threading.Thread.Sleep(100);
            Console.Write(" .");
            System.Threading.Thread.Sleep(200);
            Console.Write("  = " + Evaluator.Evaluate(expression, LookupVarVal) + "\n\n");
            System.Threading.Thread.Sleep(500);
        }


        /// <summary>
        /// Private helper method prompts the user to enter expressions to evaluate.
        /// </summary>
        /// <param name="exp">An arithmetic expression.</param>
        private static void UserInput(string exp)
        {
            System.Threading.Thread.Sleep(100);
            Console.Write("Processing:   " + exp);
            System.Threading.Thread.Sleep(50);
            Console.Write(" .");
            System.Threading.Thread.Sleep(50);
            Console.Write(" .");
            System.Threading.Thread.Sleep(50);
            Console.Write(" .");
            System.Threading.Thread.Sleep(50);
            Console.Write("  = " + Evaluator.Evaluate(exp, LookupVarVal) + "\n\n");
            System.Threading.Thread.Sleep(200);

            Console.WriteLine("Would you like to try another expression? (Y/N)");
            string response = Console.ReadLine();
            if (response.Equals("y") || response.Equals("Y") || response.Equals("YES") || response.Equals("yes") || response.Equals("Yes"))
            {
                Console.Write("\nExpression:   ");
                string input = Console.ReadLine();
                UserInput(input);
            }
        }

        /// <summary>
        /// Private helper method demonstrates invalid expressions and catches their exceptions.
        /// </summary>
        /// <param name="exp">An invalid infix expression.</param>
        private static void TryInvalid(string exp)
        {
            System.Threading.Thread.Sleep(500);
            Console.WriteLine("Attempting to process:   \"" + exp + "\"");
            System.Threading.Thread.Sleep(100);
            Console.Write(" .");
            System.Threading.Thread.Sleep(100);
            Console.Write(" .");
            System.Threading.Thread.Sleep(100);
            Console.Write(" .\n");
            System.Threading.Thread.Sleep(200);
            try
            {
                Evaluator.Evaluate(exp, LookupVarVal);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("An ArgumentException was caught as expected.\n");
            }
        }










































































































































    }
}
