using System;
using System.Text;

namespace SymbolicDifferentiation
{
    internal static class StringExtensions
    {
        public static int First(this string str, char character)
        {
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == character)
                    return i;
            }
            return -1;
        }

        public static int Last(this string str, char character)
        {
            for (var i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] == character)
                    return i;
            }
            return -1;
        }

        public static string OptimizeSign(this string str)
        {
            var nIndex = 0;
            // var operators = new[] {'(', '+', '-', '/', '*', '^'};
            // replace "--" with "" or "+"
            while ((nIndex = str.IndexOf("--", nIndex, StringComparison.Ordinal)) != -1)
                if (nIndex == 0 || "(+-/*^".Contains(str[nIndex - 1].ToString()))
                    str = str.Remove(nIndex, 2);
                else
                {
                    str = str.Remove(nIndex, 1);
                    var strChars = str.ToCharArray();
                    strChars[nIndex] = '+';
                    str = new string(strChars);
                }

            nIndex = 0;
            // replace "+-" with "-"

            while ((nIndex = str.IndexOf("+-", nIndex, StringComparison.Ordinal)) != -1)
                str = str.Remove(nIndex);

            return str;
        }

        public static string InsertValues(this string str, string[] vars, double[] values)
        {
            var res = str;
            if (vars.Length != values.Length) throw new ArgumentOutOfRangeException("Variables length must be equal to Values length.");
            for (int i = 0; i < vars.Length; i++)
            {
                //res = str.Replace("+" + vars[i], "+" + values[i]);
                //res = str.Replace("-" + vars[i], "-" + values[i]);
                //res = str.Replace("*" + vars[i], "*" + values[i]);
                //res = str.Replace("/" + vars[i], "/" + values[i]);
                //res = str.Replace(vars[i]+"+", values[i]+"+");
                //res = str.Replace(vars[i] + "-", values[i] + "-");
                //res = str.Replace(vars[i] + "*", values[i] + "*");
                //res = str.Replace(vars[i] + "/", values[i] + "/");
                //res = str.Replace("^" + vars[i], "^" + values[i]);             
                //res = str.Replace(vars[i] + "^", values[i] + "^");
                res = str.Replace(vars[i], values[i].ToString());

            }
            return res;
        }

        public static string RemoveConstants(this string str, string variable)
        {

            //if (!str.Contains(variable)) return "0";
            //var leftOp = LeftOperand(str);
            //var rightOp = RightOperand(str);
            //string result="";
            //if (leftOp != null && leftOp.Contains(variable))
            //{
            //    result += leftOp;
            //}
            if (!str.Contains(variable))
                return "0";
            var operIdx = MathStack.GetOperator(str, ExpressionItem.AdditionSubtractionOperator);
            if (operIdx == -1) return str;
            var leftOp = str.Substring(0, operIdx);
            var rightOp = str.Substring(operIdx + 1);
            //var result = ""//new StringBuilder("");
            var varInLeftOp = leftOp.Contains(variable);
            var varInRightOp = rightOp.Contains(variable);
            if (varInLeftOp && varInRightOp)
            {
                return str;
            }
            if(varInLeftOp)
            {
                return leftOp;
            }
            if (varInRightOp)
            {
                return rightOp;
            }
            return "0";
        }

        public static string LeftOperand(this string str)
        {
            if (str.IsNumeric())
                return null;
            var operIdx = MathStack.GetOperator(str, ExpressionItem.Operators);
            if (operIdx == -1) return null;
            return str.Substring(0, operIdx);
        }

        public static string RightOperand(this string str)
        {
            if (str.IsNumeric())
                return null;
            var operIdx = MathStack.GetOperator(str, ExpressionItem.Operators);
            if (operIdx == -1) return null;
            return str.Substring(operIdx + 1);
        }

        public static bool IsNumeric(this string lpcs)
        {
            var p = 0;
            if (lpcs[p] == '-' || lpcs[p] == '+')
                p++;
            if (lpcs[p] == 'e' && lpcs[p + 1] == 0)
                return true;
            if (lpcs[p] == 'p' && lpcs[p + 1] == 'i' && lpcs[p + 2] == 0)
                return true;
            while (p < lpcs.Length)
            {
                if (!char.IsDigit(lpcs[p]))
                    return false;
                p++;
            }
            return true;
        }

        public static int GetClose(this string p, int i)
        {
            // initialize parenthesis count to one
            var nOpen = 1;
            // loop in the input to find the close parenthesis
            //var i = 0;
            while (++i < p.Length)
                if (p[i] == '(')
                    nOpen++;
                else if (p[i] == ')')
                    if (--nOpen == 0)
                        return ++i;
            return -1;
        }

        public static string Optimize(this string str)
        {
            var nLength = str.Length;

            str = OptimizeSign(str);

            var pClose = 0;
            var nIndex = -1;
            // replace "((....))"  with "(....)"
            while ((nIndex < str.Length) && (nIndex = str.IndexOf("((", nIndex + 1, StringComparison.Ordinal)) != -1)
            {
                
                if (str[pClose = GetClose(str, nIndex + 1)] == ')')
                {
                    str = str.Remove(pClose, 1);
                    str = str.Remove(nIndex, 1);
                }
            }

            nIndex = -1;
            // remove any 1*
            while ((nIndex < str.Length) && (nIndex = str.IndexOf("1*", nIndex + 1, StringComparison.Ordinal)) != -1)
                if (nIndex == 0 || "+-*(".Contains(str[nIndex - 1].ToString()))
                    str = str.Remove(nIndex, 2);
            
            nIndex = -1;
            // remove any *1
            while ((nIndex < str.Length) && (nIndex = str.IndexOf("*1", nIndex + 1, StringComparison.Ordinal)) != -1)
                if (nIndex + 2 == str.Length || "+-*(".Contains(str[nIndex + 2].ToString()))
                    str = str.Remove(nIndex, 2);

            nIndex = -1;
            // remove any exponent equal 1
            while ((nIndex < str.Length) && (nIndex = str.IndexOf("^1", nIndex + 1, StringComparison.Ordinal)) != -1)
                if (nIndex + 2 == str.Length || "+-*(".Contains(str[nIndex + 2].ToString()))
                    str = str.Remove(nIndex, 2);

            nIndex = 0;
            // remove unneeded parentheses 
            while ((nIndex < str.Length) && (nIndex = str.IndexOf('(', nIndex)) != -1)
            {
                // "nscthg0" is the end characters of all supported functions
                if (nIndex > 0 && "nscthg0".Contains(str[nIndex - 1].ToString()))
                {
                    nIndex++;
                    continue;
                }
                // find the parenthesis close
                pClose = GetClose(str, nIndex);
                if (pClose == -1)
                    return str;
                // get the index of the close char
                var nCloseIndex = pClose - 1;
                // check if the parentheses in the start and the end of the string
                if ((nIndex == 0 && nCloseIndex == str.Length - 1) ||
                    nCloseIndex == nIndex + 2 ||
                    // check if the string doesn't include any operator
                    IsNumeric(str.Substring(nIndex + 1, nCloseIndex - nIndex - 1)))
                {
                    // delete the far index of ')'
                    str = str.Remove(nCloseIndex, 1);
                    // delete the near index of '('
                    str = str.Remove(nIndex, 1);
                }
                else
                    nIndex++;
            }

            str = str.Replace("1*", "");
            str = str.Replace("*1", "");

            if (nLength != str.Length)
                return str.Optimize();
            return str;
        }

        public static string InsertTabs(this string str)
        {
            if (str == "")
                return str;
            str = str.Insert(0, "\t");
            str = str.Replace("\n", "\n\t");
            str = str.TrimEnd('\t');
            return str;
        }
    }
}