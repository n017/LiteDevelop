using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    public interface IEvaluation
    {
        bool IsActive { get; }
        IThread Thread { get; }
        IValue Result { get; }

        void Abort();
        void Call(IFunction function, params IValue[] arguments);
        void CreateObject(IFunction constructor, params IValue[] arguments);
    }
}
