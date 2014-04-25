using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a virtual or fake variable.
    /// </summary>
    public class VirtualVariable : IVariable
    {
        private string _name;
        private IValue _value;

        public VirtualVariable(string name, IValue value)
        {
            _name = name;
            _value = value;
        }

        #region IVariable Members
        
        /// <inheritdoc />
        /// <inheritdoc />
        public int Index
        {
            get { return -1; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
        }

        /// <inheritdoc />
        public bool IsCompilerGenerated
        {
            get { return false; }
        }

        /// <inheritdoc />
        public IValue GetValue(IFrame frame)
        {
            return _value;
        }

        #endregion

        /// <summary>
        /// Sets the variable's value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void SetValue(IValue value)
        {
            _value = value;
        }
    }

}
