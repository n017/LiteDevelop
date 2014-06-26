using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeEvaluation : DebuggerSessionObject, IEvaluation
    {
        private readonly RuntimeThread _thread;
        private readonly ICorDebugEval _comEvaluation;
        private RuntimeValue _result;

        internal RuntimeEvaluation(RuntimeThread thread,  ICorDebugEval comEvaluation)
        {
            _thread = thread;
            _comEvaluation = comEvaluation;
        }

        internal ICorDebugEval ComEvaluation
        {
            get { return _comEvaluation; }
        }

        public override NetDebuggerSession Session
        {
            get { return _thread.Session; }
        }

        #region IEvaluation Members

        public bool IsActive
        {
            get
            {
                int value;
                _comEvaluation.IsActive(out value);
                return value == 1;
            }
        }
        
        IThread IEvaluation.Thread
        {
            get { return _thread; }
        }

        public RuntimeThread Thread
        {
            get { return _thread; }
        }

        IValue IEvaluation.Result
        {
            get { return Result; }
        }

        public RuntimeValue Result
        {
            get
            {
                ICorDebugValue comResult;
                _comEvaluation.GetResult(out comResult);

                if (comResult == null)
                    return null;

                if (_result == null || _result.ComValue != comResult)
                {
                    _result = Session.ComInstanceCollector.GetWrapper<RuntimeValue>(comResult);
                    if (_result == null)
                        Session.ComInstanceCollector.SetWrapper(comResult, _result = new RuntimeValue(Session, comResult));
                }

                return _result;
            }
        }

        public void Abort()
        {
            _comEvaluation.Abort();
        }

        void IEvaluation.Call(IFunction function, params IValue[] arguments)
        {
            Call((RuntimeFunction)function, (RuntimeValue[])arguments);
        }

        public void Call(RuntimeFunction function, RuntimeValue[] arguments)
        {
            _comEvaluation.CallFunction(function.ComFunction, (uint)arguments.Length, GetComValues(arguments));
        }

        void IEvaluation.CreateObject(IFunction constructor, params IValue[] arguments)
        {
            CreateObject((RuntimeFunction)constructor, (RuntimeValue[])arguments);
        }

        public void CreateObject(RuntimeFunction constructor, RuntimeValue[] arguments)
        {
            _comEvaluation.NewObject(constructor.ComFunction, (uint)arguments.Length, GetComValues(arguments));
        }

        #endregion

        private static ICorDebugValue[] GetComValues(RuntimeValue[] values)
        {
            var comValues = new ICorDebugValue[values.Length];
            for (int i = 0; i < values.Length; i++)
                comValues[i] = values[i].ComValue;
            return comValues;
        }
    }
}
