﻿// Skeleton written by Joe Zachary for CS 3500, January 2015
// Revised by Joe Zachary, January 2016
// JLZ Repaired pair of mistakes, January 23, 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public struct Formula
    {
        private const string lpPattern = @"\(";
        private const string rpPattern = @"\)";
        private const string opPattern = @"^[\+\-*/]$";
        private const string varPattern = @"[a-zA-Z][0-9]+";
        private const string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
        private const string spacePattern = @"\s+";

        private List<string> _tokens;
        private Validator _validator;

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(string formula)
        {
            List<string> tokens = GetTokens(formula).ToList();
            _tokens = tokens;
            _validator = null;
        }

        /// <summary>
        /// Creates a formula with a Normalizer and Validator.
        /// The purpose of a Normalizer is to convert variables into a canonical form.  
        /// The purpose of a Validator is to impose extra restrictions on the validity of a variable,
        /// beyond the ones already built into the Formula definition.
        /// </summary>
        public Formula(string formula, Normalizer normalizer, Validator validator) : this()
        {
            List<string> tokens = GetTokens(formula, normalizer).ToList();
            _validator = validator;
            _tokens = tokens;
        }

        public void ValidateFormula()
        {
            if (_tokens.Count <= 0) throw new FormulaFormatException("Formula must contain at least one token.");
            ValidateTokens(_tokens, _validator);
            ValidateParentheses(_tokens);
            ValidateOrderOfOperations(_tokens);
        }
         
        /// <summary>
        /// Tests the order of operations in a list of tokens from a formula.
        /// </summary>
        /// <param name="tokens">List of strings (tokens) to check.</param>
        private static void ValidateOrderOfOperations(List<string> tokens)
        {
            //The first token of a formula must be a number, a variable, or an opening parenthesis.
            if (!Regex.IsMatch(tokens[0], $"({doublePattern}) | ({varPattern}) | ({lpPattern})",
                RegexOptions.IgnorePatternWhitespace))
            {
                throw new FormulaFormatException(
                    "The first token of a formula must be a number, a variable, or an opening parenthesis.");
            }
            //The last token of a formula must be a number, a variable, or a closing parenthesis.
            if (
                !Regex.IsMatch(tokens.Last(), $"({doublePattern}) | ({varPattern}) | ({rpPattern})",
                    RegexOptions.IgnorePatternWhitespace))
            {
                throw new FormulaFormatException(
                    "The last token of a formula must be a number, a variable, or a closing parenthesis.");
            }
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                // Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.
                if (Regex.IsMatch(tokens[i], $"({doublePattern}) | ({varPattern}) | ({rpPattern})",
                    RegexOptions.IgnorePatternWhitespace))
                {
                    if (
                        !Regex.IsMatch(tokens[i + 1], $"({opPattern}) | ({rpPattern})",
                            RegexOptions.IgnorePatternWhitespace))
                    {
                        throw new FormulaFormatException(
                            "Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.");
                    }
                }

                // Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.
                if (Regex.IsMatch(tokens[i], $"({lpPattern}) | ({opPattern})", RegexOptions.IgnorePatternWhitespace))
                {
                    if (
                        !Regex.IsMatch(tokens[i + 1], $"({doublePattern}) | ({varPattern}) | ({lpPattern})",
                            RegexOptions.IgnorePatternWhitespace))
                    {
                        throw new FormulaFormatException(
                            "Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.");
                    }
                }
            }
        }

        /// <summary>
        /// Validates all tokens in a formula that they match one of the patterns.
        /// Also validates against a given validator.
        /// </summary>
        private void ValidateTokens(List<string> tokens, Validator validator)
        {
            ValidateTokens(tokens);
            foreach (var token in tokens.Where(token => Regex.IsMatch(token, $"^{varPattern}$")))
            {
                if (validator != null && !validator.Invoke(token))
                {
                    throw new FormulaFormatException($"Validator caught an invalid token '{token}'.");
                }
            }
        }

        /// <summary>
        /// Validates all tokens in a formula that they match one of the patterns.
        /// </summary>
        private static void ValidateTokens(List<string> tokens)
        {
            string pattern = $"({lpPattern}) | ({rpPattern}) | ({opPattern}) | ({varPattern}) | ({doublePattern})";
            foreach (string token in tokens)
            {
                if (!Regex.IsMatch(token, pattern, RegexOptions.IgnorePatternWhitespace))
                {
                    throw new FormulaFormatException($"Invalid token in formula: \"{token}\"");
                }
            }
        }

        /// <summary>
        /// Checks that the number of opening parantheses is equal to the number of closing parantheses.
        /// Checks that the number of closing parentheses when read from left to right is never greater than
        /// the number of opening perentheses seen so far.
        /// </summary>
        /// <param name="tokens">List of strings (tokens) to check.</param>
        /// <returns></returns>
        private static void ValidateParentheses(List<string> tokens)
        {
            int right;
            var left = right = 0;

            foreach (var token in tokens)
            {
                if (token == "(") left++;
                if (token == ")") right++;
                if (right > left) throw new FormulaFormatException("Invalid order of parentheses.");
            }
            if (left != right) throw new FormulaFormatException("Invalid number of parentheses.");
        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            Stack<object> operatorStack = new Stack<object>();
            Stack<double> valueStack = new Stack<double>();

            if (_tokens == null) _tokens = new List<string> {"0"};

            try
            {
                ValidateFormula();
            }
            catch (Exception e)
            {
                throw new FormulaEvaluationException(e.Message);
            }

            foreach (var token in _tokens)
            {
                // If it is a variable.
                if (Regex.IsMatch(token, $"^{varPattern}$", RegexOptions.IgnorePatternWhitespace))
                {
                    HandleVariable(lookup, token, operatorStack, valueStack);
                }

                // If it is a double.
                if (Regex.IsMatch(token, $"^{doublePattern}$", RegexOptions.IgnorePatternWhitespace))
                {
                    HandleDouble(token, operatorStack, valueStack);
                }

                // If it is a operator or a '('.
                if (Regex.IsMatch(token, $"({opPattern}) | ({lpPattern})", RegexOptions.IgnorePatternWhitespace))
                {
                    HandleOpAndLp(token, operatorStack, valueStack);
                }

                //If it is a ')'.
                if (Regex.IsMatch(token, rpPattern, RegexOptions.IgnorePatternWhitespace))
                {
                    HandleRightParenthesis(operatorStack, valueStack);
                }

            }

            if (operatorStack.Count > 0)
            {
                return Operate(operatorStack.Pop(), valueStack.Pop(), valueStack.Pop());
            }
            return valueStack.Pop();
        }

        /// <summary>
        /// If + or - is at the top of the operator stack, pop the value stack twice and the 
        /// operator stack once.  Apply the popped operator to the popped numbers. 
        /// Push the result onto the value stack.
        /// </summary>
        private static void HandleOpAndLp(string token, Stack<object> operatorStack, Stack<double> valueStack)
        {
            if (token == "+" || token == "-")
            {
                if (operatorStack.Count >= 1 && new[] {"+", "-"}.Contains(operatorStack.Peek()))
                {
                    valueStack.Push(Operate(operatorStack.Pop(), valueStack.Pop(), valueStack.Pop()));
                }
            }


            //push token onto the operator stack no matter what.
            operatorStack.Push(token);
        }

        /// <summary>
        /// If * or / is at the top of the operator stack, pop the value stack, pop the operator stack,
        /// and apply the popped operator to t and the popped number. Push the result onto the value stack. 
        /// Otherwise, push t onto the value stack
        /// </summary>
        private static void HandleDouble(string token, Stack<object> operatorStack, Stack<double> valueStack)
        {
            double value = double.Parse(token);


            if (operatorStack.Count > 0 && new[] {"/", "*"}.Contains(operatorStack.Peek()))
            {
                valueStack.Push(Operate(operatorStack.Pop(), value, valueStack.Pop()));
            }
            else
            {
                valueStack.Push(value);
            }
        }

        /// <summary>
        /// If * or / is at the top of the operator stack, pop the value stack, pop the operator stack,
        /// and apply the popped operator to t and the popped number.Push the result onto the value stack. 
        /// Otherwise, push t onto the value stack
        /// </summary>
        private static void HandleVariable(Lookup lookup, string token, Stack<object> operatorStack, Stack<double> valueStack)
        {
            double value;
            try
            {
                value = lookup(token);
            }
            catch (CircularException e)
            {
                throw new FormulaEvaluationException(e.Message);
            }
            catch (UndefinedVariableException e)
            {
                throw new FormulaEvaluationException(e.Message);
            }

            if (double.IsNaN(value))
            {
                throw new UndefinedVariableException($"Could not find value for variable: {token}.");
            }

            if (operatorStack.Count > 0 && new[] {"/", "*"}.Contains(operatorStack.Peek()))
            {
                valueStack.Push(Operate(operatorStack.Pop(), value, valueStack.Pop()));
            }
            else
            {
                valueStack.Push(value);
            }
        }

        /// <summary>
        /// If + or - is at the top of the operator stack, pop the value stack twice and the operator stack once.
        /// Apply the popped operator to the popped numbers.Push the result onto the value stack.
        ///
        /// Whether or not you did the first step, the top of the operator stack will be a (. Pop it.
        ///
        /// After you have completed the previous step, if *or / is at the top of the operator stack,
        /// pop the value stack twice and the operator stack once. Apply the popped operator to the
        /// popped numbers. Push the result onto the value stack.
        /// </summary>
        private static void HandleRightParenthesis(Stack<object> operatorStack, Stack<double> valueStack)
        {
            if (operatorStack.Count > 0 && new[] {"+", "-"}.Contains(operatorStack.Peek()))
            {
                valueStack.Push(Operate(operatorStack.Pop(), valueStack.Pop(), valueStack.Pop()));
            }
            operatorStack.Pop();
            if (operatorStack.Count > 0 && new[] {"*", "/"}.Contains(operatorStack.Peek()))
            {
                valueStack.Push(Operate(operatorStack.Pop(), valueStack.Pop(), valueStack.Pop()));
            }
        }

        /// <summary>
        /// Applies the operator between two doubles.
        /// </summary>
        /// <param name="op">The operator to apply.</param>
        /// <returns></returns>
        public static double Operate(object op, double num1, double num2)
        {
            switch (op.ToString())
            {
                case "+": return num2 + num1;
                case "-": return num2 - num1;
                case "*": return num2 * num1;
                case "/":
                    if (num1 == 0) throw new FormulaEvaluationException("Divide by zero.");
                    return num2 / num1;
                case "%": return num2 % num1;
                default: throw new FormulaFormatException($"Invalid operation: {op}");
            }   
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(string formula, Normalizer normalizer = null)
        {
            string pattern = $"({lpPattern}) | ({rpPattern}) | ({opPattern}) | ({varPattern}) | ({doublePattern}) | ({spacePattern})";
            // Enumerate matching tokens that don't consist solely of white space.
            foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    if (normalizer != null && Regex.IsMatch(s, $"^{varPattern}$")) yield return normalizer.Invoke(s);
                    else yield return s;
                }
            }
        }

        /// <summary>
        /// Returns the original formula as it was entered to begin with.
        /// </summary>
        public override string ToString()
        {
            if (_tokens == null) _tokens = new List<string> { "0" };
            string result = _tokens.Aggregate("", (current, token) => current + token + " ");
            return result.Trim();
        }

        public ISet<string> GetVariables()
        {
            if (_tokens == null) _tokens = new List<string>{ "0" };
            return new HashSet<string>(_tokens.Where(token => Regex.IsMatch(token, $"^{varPattern}$")));
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string s);

    public delegate string Normalizer(string s);
    public delegate bool Validator(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Thrown to indicate that a change to a cell will cause a circular dependency.
    /// </summary>
    public class CircularException : Exception
    {
        /// <summary>
        /// Creates the exception with a message
        /// </summary>
        public CircularException(string msg)
            : base(msg)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}


