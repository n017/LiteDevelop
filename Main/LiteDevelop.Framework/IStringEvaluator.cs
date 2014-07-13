using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LiteDevelop.Framework
{
    public interface IStringEvaluator
    {
        /// <summary>
        /// Applies parameter values to a specific string.
        /// </summary>
        /// <param name="unevaluatedValue">The text to apply parameters to.</param>
        /// <returns></returns>
        string EvaluateString(string unevaluatedValue);
    }

    public abstract class ParameterizedStringEvaluator : IStringEvaluator
    {
        private static readonly Regex _parametersRegex = new Regex(@"\$\((?<parameter>[\w\.]+)\)");
        
        /// <summary>
        /// Gets an argument value by its name.
        /// </summary>
        /// <param name="argumentName">The name of the argument to get the value from.</param>
        /// <returns></returns>
        public abstract string GetArgumentValue(string argumentName);

        public string EvaluateString(string unevaluatedValue)
        {
            StringBuilder builder = new StringBuilder(unevaluatedValue);
            int offset = 0;

            foreach (Match match in _parametersRegex.Matches(unevaluatedValue))
            {
                string value = GetArgumentValue(match.Groups["parameter"].Value);
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

    public class DictionaryStringEvaluator : ParameterizedStringEvaluator
    {
        public DictionaryStringEvaluator()
            : this(new Dictionary<string, string>())
        { 
        }

        public DictionaryStringEvaluator(IDictionary<string,string> arguments)
        {
            Arguments = arguments;
        }

        public IDictionary<string, string> Arguments
        {
            get;
            protected set;
        }

        public override string GetArgumentValue(string argumentName)
        {
            return Arguments[argumentName];
        }
    }
}
