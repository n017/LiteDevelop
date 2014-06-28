using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    // UNDONE: Function evaluation not functioning properly yet!
    //
    // For some reason function evaluation always fails, throwing an exception when calling the requested method,
    // complaining about an unsafe GC point, even when the program made sure that the evaluation thread is 
    // at a GC-safe point (it did right?).
    //
    // Func-Eval rules got from blog of Mike Stall.
    // http://blogs.msdn.com/b/jmstall/archive/2005/03/23/400794.aspx
    // 

    public class RuntimeEvaluation : DebuggerSessionObject, IEvaluation
    {
        public event DebuggerEventHandler EvaluationCompleted; 
        private readonly RuntimeThread _thread;
        private readonly ICorDebugEval _comEvaluation;
        private readonly ManualResetEvent _manualEvaluationCompleted = new ManualResetEvent(false);
        private RuntimeValue _result;

        internal RuntimeEvaluation(RuntimeThread thread, ICorDebugEval comEvaluation)
        {
            if (thread.State == ThreadState.Suspended)
                throw new InvalidOperationException("Thread is suspended.");
            if (!thread.IsManaged)
                throw new InvalidOperationException("Thread is at native code.");
            if (!thread.IsAtGCSafePoint) 
                throw new InvalidOperationException("Thread is at GC unsafe point.");

            _thread = thread;
            _comEvaluation = comEvaluation;
        }

        internal ICorDebugEval ComEvaluation
        {
            get { return _comEvaluation; }
        }

        internal ICorDebugEval2 ComEvaluation2
        {
            get { return _comEvaluation as ICorDebugEval2; }
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
            _manualEvaluationCompleted.Reset();
            _comEvaluation.Abort();
        }

        void IEvaluation.Call(IFunction function, params IValue[] arguments)
        {
            Call((RuntimeFunction)function, (RuntimeValue[])arguments);
        }

        public void Call(RuntimeFunction function, params RuntimeValue[] arguments)
        {
            Call(function.ComFunction, arguments.GetComValues().ToArray());
        }

        internal void Call(ICorDebugFunction function, ICorDebugValue[] arguments)
        {
            _manualEvaluationCompleted.Reset();
            ComEvaluation2.CallParameterizedFunction(function, 0, new ICorDebugType[0], (uint)arguments.Length, arguments);
        }

        void IEvaluation.CreateObject(IFunction constructor, params IValue[] arguments)
        {
            CreateObject((RuntimeFunction)constructor, (RuntimeValue[])arguments);
        }

        public void CreateObject(RuntimeFunction constructor, params RuntimeValue[] arguments)
        {
            _manualEvaluationCompleted.Reset();
            _comEvaluation.NewObject(constructor.ComFunction, (uint)arguments.Length, arguments.GetComValues().ToArray());
        }

        public bool WaitForResult(int timeout)
        {
            return _manualEvaluationCompleted.WaitOne(timeout);
        }

        #endregion


        internal void DispatchEvaluationCompleted(DebuggerEventArgs e)
        {
            if (EvaluationCompleted != null)
                EvaluationCompleted(this, e);
            _manualEvaluationCompleted.Set();
        }

        public static RuntimeValue InvokeMethod(RuntimeThread thread, SymbolToken functionToken, params RuntimeValue[] arguments)
        {
            return InvokeMethod(thread, functionToken.GetToken(), arguments);
        }

        public static RuntimeValue InvokeMethod(RuntimeThread thread, int functionToken, params RuntimeValue[] arguments)
        {
            var resolvedModule = thread.Session.Resolver.ResolveModule(thread.CurrentFrame.Function.Module.Name);
            if (resolvedModule != null)
            {
                var resolvedMember = resolvedModule.ResolveMember(functionToken);
                if (resolvedModule != null)
                    return InvokeMethod(thread, thread.CurrentFrame.Function.Module.GetFunction(unchecked((uint)resolvedMember.MetaDataToken)), arguments);
            }
            return null;
        }

        public static RuntimeValue InvokeMethod(RuntimeThread thread, RuntimeFunction function, params RuntimeValue[] arguments)
        {
            var evaluation = thread.CreateEvaluation();
            evaluation.Call(function, arguments);
            if (!evaluation.WaitForResult(3000))
                return null;
            return evaluation.Result;
        }

    }
}
