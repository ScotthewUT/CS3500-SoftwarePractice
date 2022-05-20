// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens

// (Scott Crowley u1178178)
// Version 1.4 (10/4/19)

// Change log:
//  (Version 1.3) Implemented methods defined in skeleton.
//  (Version 1.4) Fixed bug in GetVariables where it checked for
//                char between 'a'-'Z' instead of 'a'-'z'.



using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        // The Formula is backed by a read-only list of strings.
        private readonly ReadOnlyCollection<string> _expression;


        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }


        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            // Empty expressions aren't valid.
            if (formula.Length == 0)
                throw new FormulaFormatException("ERROR: Formulas need at least one token.");

            // Split the expression into a list of tokens.
            IEnumerable<string> tokens = GetTokens(formula);

            // Call helper method to check for a valid expression which returns a normalized formula in an array.
            _expression = Array.AsReadOnly<string>(ParseAndValidateTokens(tokens, normalize, isValid));
        }


        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            // Create two stacks. One for doubles and one for operators.
            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();

            // Iterate through the tokens applying the provided algorithm to each.
            foreach (string token in _expression)
            {
                // Evaluate the token by cases.
                switch (token)
                {
                    case "(":
                        // If the token is a '(', put it on the operator stack.
                        operators.Push(token);
                        break;

                    case ")":
                        // If the token is a ')' and there is also a '+' or '-' on the operator stack, pot the operator
                        // stack once and value stack twice, compute the operation, then push the result to the value stack.
                        AddOrSubtract(values, operators);
                        // Next, the top of the operator stack should be a '('. Pop it.
                        operators.Pop();
                        // Finally, if there is a '*' or '/' on the operator stack, pop the operator stack once and the
                        // value stack twice, compute the operation, then push the result to the value stack.
                        if (operators.Count > 0)
                        {
                            if (operators.Peek().Equals("*") || operators.Peek().Equals("/"))
                            {
                                double b = values.Pop();
                                double a = values.Pop();
                                if (operators.Pop().Equals("*"))
                                    values.Push(a * b);
                                else
                                {
                                    if (b == 0.0)
                                        return new FormulaError("#DIV/0!");
                                    values.Push(a / b);
                                }
                            }
                        }
                        break;

                    case "*":
                    case "/":
                        // If the token is a '*' or '/', put it on the operator stack.
                        operators.Push(token);
                        break;

                    case "+":
                    case "-":
                        // If the token is a '+' or '-' and there is also a '+' or '-' on the operator stack, pop the
                        // operator stack once and value stack twice, compute the operation, then push the result to
                        // the value stack.
                        if (operators.Count > 0)
                            AddOrSubtract(values, operators);
                        // Finally, push the token to the operator stack.
                        operators.Push(token);
                        break;

                    default:
                        // The remaining cases consist of values (either doubles or variables).
                        // If the token begins with a letter or underscore, lookup the variable's value.
                        double y;
                        if (('A' <= token[0] && token[0] <= 'Z') || ('a' <= token[0] && token[0] <= 'z') || token[0] == '_')
                        {
                            try
                            {
                                y = lookup(token);
                            }
                            catch (ArgumentException e)
                            {
                                return new FormulaError("#NOVALUE!");
                            }
                        }
                        // If the token begins with a 0-9, parse the double.
                        else
                            y = Double.Parse(token);
                        // If there is a '*' or '/' on the operator stack, pop the operator and value stack, compute the
                        // operation with the popped value and token, then push the result to the value stack.
                        if (operators.Count > 0)
                        {
                            if (operators.Peek().Equals("*") || operators.Peek().Equals("/"))
                            {
                                double x = values.Pop();
                                if (operators.Pop().Equals("*"))
                                    values.Push(x * y);
                                else
                                {
                                    if (y == 0.0)
                                        return new FormulaError("#DIV/0!");
                                    values.Push(x / y);
                                }
                                break;
                            }
                        }
                        values.Push(y);
                        break;
                }
            }
            // End of for-each loop; all tokens processed. There should either be a single value and no operators or
            // two values and a single '+' or '-' operator.
            if (operators.Count == 0)
                return values.Pop();
            AddOrSubtract(values, operators);
            return values.Pop();
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<string> variables = new HashSet<string>();
            foreach (string token in _expression)
            {
                if (('A' <= token[0] && token[0] <= 'Z') || ('a' <= token[0] && token[0] <= 'z') || token[0] == '_')
                    variables.Add(token);
            }
            return variables.AsEnumerable();
        }


        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            foreach (string token in _expression)
            {
                str.Append(token);
            }
            return str.ToString();
        }


        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Formula))
                return false;
            return this.ToString().Equals(obj.ToString());
        }


        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null))
            {
                if (ReferenceEquals(f2, null))
                    return true;
                return false;
            }
            if (ReferenceEquals(f2, null))
                return false;
            return f1.Equals(f2);
        }


        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if ((ReferenceEquals(f1, null) && !ReferenceEquals(f2, null)) || (!ReferenceEquals(f1, null) && ReferenceEquals(f2, null)))
                return true;
            return !(f1 == f2);
        }


        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }


        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }


        /// <summary>
        /// Helper method iterates over collection of tokens, applies a normalizer to any variables, checks for correct
        /// expression syntax, and returns an array version of the expression if it was found to be valid.
        /// </summary>
        /// <param name="_tokens">A collection of tokens to parse.</param>
        /// <param name="_normalize">A method that normalizes string variables.</param>
        /// <param name="_isValid">A method that checks if a variable is valid format.</param>
        /// <returns>An array of tokens which represent a valid formula.</returns>
        /// <exception cref="FormulaFormatException">Expression wasn't properly formated.</exception>
        private static string[] ParseAndValidateTokens(IEnumerable<string> _tokens, Func<string, string> _normalize, Func<string, bool> _isValid)
        {
            string[] formula_array = new string[_tokens.Count()];

            // Two toggles to track when to check for 'Parenthesis/Operator Following Rule' or 'Extra Following Rule.'
            bool lpara_op_following_rule = false;
            bool rpara_val_following_rule = false;
            // Left and right paratheses counters to check balance.
            int l_para = 0;
            int r_para = 0;
            // Index for _tokens and formula_array.
            int idx = 0;

            // Iterate through the tokens confirming format validity and copying to array.
            foreach (string token in _tokens)
            {
                // Was the previous token a '(' or an operator?
                if (lpara_op_following_rule)
                {
                    // Any token that immediately follows a '(' or operator, must be either a number, a variable, or '('.
                    if (token.Equals(")") || token.Equals("+") || token.Equals("-") || token.Equals("*") || token.Equals("/"))
                        throw new FormulaFormatException("ERROR: Invalid token, \"" + token + "\", at position " + (idx + 1) + ".");
                    lpara_op_following_rule = false;
                }
                // Was the previous token a ')' or a value?
                if (rpara_val_following_rule)
                {
                    // Any token that immediately follows a number, a variable, or ')', must be either an operator or ')'.
                    if (!(token.Equals(")") || token.Equals("+") || token.Equals("-") || token.Equals("*") || token.Equals("/")))
                        throw new FormulaFormatException("ERROR: Invalid token, \"" + token + "\", at position " + (idx + 1) + ".");
                    rpara_val_following_rule = false;
                }

                switch (token)
                {
                    case "(":
                        l_para++;
                        lpara_op_following_rule = true;
                        formula_array[idx] = "(";
                        break;

                    case ")":
                        r_para++;
                        // Check for 'Right Parathesis Rule.'
                        if (l_para < r_para)
                            throw new FormulaFormatException("ERROR: Unmatched closing parathesis at token position " + (idx + 1) + ".");
                        rpara_val_following_rule = true;
                        formula_array[idx] = ")";
                        break;

                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        lpara_op_following_rule = true;
                        formula_array[idx] = token;
                        break;

                    default:
                        // The remaining cases consist of 3 types of tokens: integers, variables, or invalids.

                        // If the token begins with 0-9, attempt to parse it to an double.
                        if (('0' <= token[0] && token[0] <= '9') || token[0] == '.')
                        {
                            double value;
                            if (!double.TryParse(token, out value))
                                throw new FormulaFormatException("ERROR: Invalid token, \"" + token + "\", at position " + (idx + 1) + ".");
                            rpara_val_following_rule = true;
                            formula_array[idx] = value.ToString();
                            break;
                        }

                        // Check if the token is a valid variable.
                        if (VariableIsValid(token))
                        {
                            // Normalize the variable as determined by the user.
                            string token_normalized = _normalize(token);
                            // Check that the normalizer didn't break variable's minimum format requirements.
                            if (!VariableIsValid(token_normalized))
                                throw new FormulaFormatException("ERROR: Invalid variable format. Token at position " + (idx + 1) + " was \"" + token
                                                                                             + "\" and normalized to \"" + token_normalized + "\".");
                            // Check if variable meets user's format requirements.
                            if (!_isValid(token_normalized))
                                throw new FormulaFormatException("ERROR: Invalid variable format, \"" + token + "\", at position " + (idx + 1) + ".");

                            rpara_val_following_rule = true;
                            formula_array[idx] = token_normalized;
                            break;
                        }
                        // Token isn't a parathesis, operator, double, or variable.
                        throw new FormulaFormatException("ERROR: Invalid token, \"" + token + "\", at position " + (idx + 1) + ".");
                }
                // Bottom of the for-each loop. Increment the index.
                idx++;
            }
            // Check for 'Balanced Paratheses Rule.'
            if (l_para != r_para)
                throw new FormulaFormatException("ERROR: Imbalanced paratheses. There are " + l_para + " '('s and " + r_para + " ')'s.");
            // Check for 'Starting Token Rule.'
            char first = formula_array[0][0];
            if (!(('0' <= first && first <= '9') || ('A' <= first && first <= 'Z') || ('a' <= first && first <= 'z') || first == '_' || first == '('))
                throw new FormulaFormatException("ERROR: Expression must begin with a number, variable, or '('.");
            // Check for 'Ending Token Rule.'
            char last = formula_array[formula_array.Length - 1][0];
            if (!(('0' <= last && last <= '9') || ('A' <= last && last <= 'Z') || ('a' <= last && last <= 'z') || last == '_' || last == ')'))
                throw new FormulaFormatException("ERROR: Expression must end with a number, variable, or ')'.");

            // Success! Valid expression confirmed. Return it in array form.
            return formula_array;
        }


        /// <summary>
        /// Checks if a potential variable string meets the minimum format requirements:
        /// any letter or underscore followed by any number of letters and/or digits and/or underscores
        /// </summary>
        /// <param name="variable">A string to check for format validity.</param>
        /// <returns>True, if the string is properly formated as a variable; otherwise, false.</returns>
        private static bool VariableIsValid(string variable)
        {
            // The first character in a valid variable is a letter or underscore.
            char first = variable[0];
            if (!(('A' <= first && first <= 'Z') || ('a' <= first && first <= 'z') || first == '_'))
                return false;
            // Each subsequent character is a letter, number, or underscore.
            for (int idx = 1; idx < variable.Length; idx++)
            {
                char next = variable[idx];
                if (('A' <= next && next <= 'Z') || ('a' <= next && next <= 'z') || ('0' <= next && next <= '9') || next == '_')
                    continue;
                return false;
            }
            return true;
        }


        /// <summary>
        /// Private helper method checks if the top of the operator stack is a '+' or '-'. If so, it pops the operator
        /// stack once and value stack twice, computes the operation, and pushes the result to the value stack.
        /// WARNING: This method assumes there is at least one item on the operator stack.
        /// </summary>
        /// <param name="vals">Stack of doubles representing values from an arithmetic expression.</param>
        /// <param name="ops">Stack of strings representing operators from an arithmetic expression.</param>
        private static void AddOrSubtract(Stack<double> vals, Stack<string> ops)
        {
            if (ops.Peek().Equals("+") || ops.Peek().Equals("-"))
            {
                double b = vals.Pop();
                double a = vals.Pop();
                if (ops.Pop().Equals("+"))
                    vals.Push(a + b);
                else
                    vals.Push(a - b);
            }
        }


        /// <summary>
        /// Private helper method processes double tokens before placing them on the value stack.
        /// </summary>
        /// <param name="vals">Stack of doubles representing values from an arithmetic expression.</param>
        /// <param name="ops">Stack of strings representing operators from an arithmetic expression.</param>
        /// <param name="b">Double token for the values stack.</param>
        private static void PushAndProcessDouble(Stack<double> vals, Stack<string> ops, double b)
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
                    double a = vals.Pop();
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


    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }


    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

