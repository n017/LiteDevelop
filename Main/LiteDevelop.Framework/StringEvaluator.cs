using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LiteDevelop.Framework
{
    public static class StringEvaluator
    {
        private static readonly Regex _parametersRegex = new Regex(@"\$\((?<parameter>[\w\.]+)\)");

        /// <summary>
        /// Applies parameter values to a specific string.
        /// </summary>
        /// <param name="input">The text to apply parameters to.</param>
        /// <param name="arguments">The parameters to use.</param>
        /// <returns></returns>
        public static string EvaluateString(string input, IDictionary<string, string> arguments)
        {
            StringBuilder builder = new StringBuilder(input);
            int offset = 0;

            foreach (Match match in _parametersRegex.Matches(input))
            {
                string value = arguments[match.Groups["parameter"].Value];
                bool isNull = string.IsNullOrEmpty(value);

                builder.Remove(match.Index + offset, match.Length);

                if (!isNull)
                    builder.Insert(match.Index + offset, value);

                offset -= match.Length;

                if (!isNull)
                    offset += value.Length;
            }

            return builder.ToString();
        }

    }
}
