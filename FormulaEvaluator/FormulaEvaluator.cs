using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// The Evaluator class provides a method for processing arithmetic expressions that use standard
    /// infix notation.
    /// </summary>
    public static class Evaluator
    {
        //Declare delegate for a variable lookup method.
        public delegate int Lookup(string var);

        /// <summary>
        /// The primary method for processing arithmetic expressions using standard infix notation.
        /// </summary>
        /// <param name="expression">A string representation of an arithmetic expression formated
        /// for standard infix notation.</param>
        /// <param name="variableEvaluator">A value lookup method that takes a string variable and
        /// returns an integer.</param>
        /// <returns>The integer value computed by the arithmetic expression.</returns>
        /// <exception cref="System.ArgumentException">Expression parameter is not properly formated
        /// or otherwise invalid</exception>
        public static int Evaluate(string expression, Lookup variableEvaluator)
        {
            //Split the expression into an array of substrings.
            string[] tokens = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            //Create two stacks. One for integers and one for operators.
            Stack<int> values = new Stack<int>();
            Stack<string> operators = new Stack<string>();

            //Iterate through the substrings applying the provided algorithm to each.
            foreach (string t in tokens)
            {   
                //Trim the leading and trailing whitespace from the substring.
                string token = t.Trim();

                //Evaluate the token by cases.
                switch (token)
                {
                    case "":
                        //If the token is empty, skip it.
                        break;
                    case "(":
                        //If the token is a '(', put it on the operator stack.
                        operators.Push(token);
                        break;

                    case ")":
                        //If the token is a ')' and there is also a '+' or '-' on the operator stack,
                        //pop the operator stack once and value stack twice, compute the operation,
                        //then push the result to the value stack.
                        if (operators.Count > 0)
                            AddOrSubtract(values, operators);
                        else
                            throw new System.ArgumentException("Error: Expression is invalid!");
                        //Next, the top of the operator stack should be a '('. Pop it.
                        if (operators.Count < 1 || !operators.Pop().Equals("("))
                            throw new System.ArgumentException("Error: Expression is invalid!");
                        //Finally, if there is a '*' or '/' on the operator stack, pop the operator
                        //stack once and the value stack twice, compute the operation, then push the
                        //result to the value stack.
                        if (operators.Count > 0)
                        {
                            if (operators.Peek().Equals("*") || operators.Peek().Equals("/"))
                            {
                                if (values.Count < 2)
                                    throw new System.ArgumentException("Error: Expression is invalid!");
                                int b = values.Pop();
                                int a = values.Pop();
                                if (operators.Pop().Equals("*"))
                                    values.Push(a * b);
                                else
                                {
                                    if (b == 0)
                                        throw new System.ArgumentException("Error: Expression is invalid!");
                                    values.Push(a / b);
                                }
                            }
                        }
                        break;

                    case "*":
                    case "/":
                        //If the token is a '*' or '/', put it on the operator stack.
                        operators.Push(token);
                        break;

                    case "+":
                    case "-":
                        //If the token is a '+' or '-' and there is also a '+' or '-' on the operator
                        //stack, pop the operator stack once and value stack twice, compute the
                        //operation, then push the result to the value stack.
                        if (operators.Count > 0)
                            AddOrSubtract(values, operators);
                        //Finally, push the token to the operator stack.
                        operators.Push(token);
                        break;

                    default:
                        //The remaining cases consist of 3 types of tokens: integers, variables, or invalids.
                        char first = token[0];
                        //If the token begins with 0-9, attempt to parse it to an int.
                        if ('0' <= first && first <= '9')
                        {
                            int b;
                            if (!int.TryParse(token, out b))
                                throw new System.ArgumentException("Error: Expression is invalid!");
                            //If a valid integer was found, call helper method to process it.
                            PushInteger(values, operators, b);
                            break;
                        }
                        //If the token begins with a letter, check that it is a valid variable format.
                        if ((64 < first && first < 91) || (96 < first && first < 123))
                        {
                            int length = token.Length;
                            //Variables of length 1 are invalid.
                            if (length < 2)
                                throw new System.ArgumentException("Error: Expression is invalid!");
                            char last = token[length - 1];
                            //Variables must end with a number.
                            if (!('0' <= last && last <= '9'))
                                throw new System.ArgumentException("Error: Expression is invalid!");
                            //Check the token left to right until a number or invalid character is found.
                            int idx = 1;
                            while (idx < length - 2)
                            {
                                char next = token[idx];
                                if ((64 < next && next < 91) || (96 < next && next < 123))
                                {
                                    idx++;
                                    continue;
                                }
                                else if ('0' <= next && next <= '9')
                                    break;
                                throw new System.ArgumentException("Error: Expression is invalid!");
                            }
                            //Continue to check the token until anything except a number is found.
                            while (idx < length - 2)
                            {
                                char next = token[idx];
                                if ('0' <= next && next <= '9')
                                {
                                    idx++;
                                    continue;
                                }
                                throw new System.ArgumentException("Error: Expression is invalid!");
                            }
                            //Valid variable format confirmed. Lookup its value.
                            int b = variableEvaluator(token);
                            //Process the variable's value same as the above integer method.
                            PushInteger(values, operators, b);
                            break;
                        }
                        //If the token wasn't a valid integer or variable, it must be invalid.
                        throw new System.ArgumentException("Error: Expression is invalid!");
                }
            }
            //All tokens processed. There should either be a single value and no operators or two values
            //and a single '+' or '-' operator.
            if (operators.Count == 0)
            {
                if (values.Count != 1)
                    throw new System.ArgumentException("Error: Expression is invalid!");
                return values.Pop();
            }
            else if (operators.Count == 1 && values.Count == 2 && AddOrSubtract(values, operators))
                return values.Pop();
            throw new System.ArgumentException("Error: Expression is invalid!");
        }


        /// <summary>
        /// Private helper method checks if the top of the operator stack is a '+' or '-'. If true,
        /// it pops the operator stack once and value stack twice, computes the operation, pushes
        /// the result to the value stack, then returns true. Otherwise, is simply returns false.
        /// WARNING: This method assumes there is at least 1 item on the operator stack.
        /// </summary>
        /// <param name="vals">Stack of ints representing values from an arithmetic expression.</param>
        /// <param name="ops">Stack of strings representing operators from an arithmetic expression.</param>
        /// <returns>True, if addition or subtraction was performed, otherwise false.</returns>
        private static bool AddOrSubtract(Stack<int> vals, Stack<string> ops)
        {
            if (ops.Peek().Equals("+") || ops.Peek().Equals("-"))
            {
                if (vals.Count < 2)
                    throw new System.ArgumentException("Error: Expression is invalid!");
                int b = vals.Pop();
                int a = vals.Pop();
                if (ops.Pop().Equals("+"))
                    vals.Push(a + b);
                else
                    vals.Push(a - b);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Private helper method processes integer tokens before placing them on the value stack.
        /// </summary>
        /// <param name="vals">Stack of ints representing values from an arithmetic expression.</param>
        /// <param name="ops">Stack of strings representing operators from an arithmetic expression.</param>
        /// <param name="b">Integer token for the values stack.</param>
        private static void PushInteger(Stack<int> vals, Stack<string> ops, int b)
        {
            //If there is a '*' or '/' on the operator stack, pop the operator and value stack,
            //compute the operation with the popped value and token, then push the result to the
            //value stack.
            if (ops.Count > 0)
            {
                if (ops.Peek().Equals("*") || ops.Peek().Equals("/"))
                {
                    if (vals.Count < 1)
                        throw new System.ArgumentException("Error: Expression is invalid!");
                    int a = vals.Pop();
                    if (ops.Pop().Equals("*"))
                        vals.Push(a * b);
                    else
                    {
                        if (b == 0)
                            throw new System.ArgumentException("Error: Expression is invalid!");
                        vals.Push(a / b);
                    }
                    return;
                }
            }
            vals.Push(b);
            return;
        }
    }
}
