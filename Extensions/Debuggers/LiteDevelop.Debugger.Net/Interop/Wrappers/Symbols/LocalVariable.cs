using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com.Symbols;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers.Symbols
{
    public class LocalVariable : IVariable
    {
        internal LocalVariable(ISymUnmanagedVariable symVariable)
        {
            SymVariable = symVariable;
        }

        internal ISymUnmanagedVariable SymVariable
        {
            get;
            private set;
        }

        #region IVariable Members

        public string Name
        {
            get
            {
                int capacity = 255;
                StringBuilder builder = new StringBuilder(capacity);
                SymVariable.GetName(capacity, out capacity, builder);
                return builder.ToString().Trim('\0');
            }
        }

        public int Index
        {
            get
            {
                return SymVariable.GetAddressField1();
            }
        }

        public bool IsCompilerGenerated
        {
            get
            {
                return (SymVariable.GetAttributes() & 1) == 1;
            }
        }

        IValue IVariable.GetValue(IFrame frame)
        {
            return GetValue((RuntimeFrame)frame);
        }

        public RuntimeValue GetValue(RuntimeFrame frame)
        {
            return frame.GetLocalVariableValue((uint)Index);
        }

        #endregion
    }

    internal static class LocalVariableExtensions
    {
        public static LocalVariable[] ToLocalVariables(this ISymUnmanagedVariable[] variables)
        {
            var locals = new LocalVariable[variables.Length];
            for (int i =0; i < locals.Length; i++)
            {
                locals[i] = new LocalVariable(variables[i]);
            }
            return locals;
        }
    }
}
