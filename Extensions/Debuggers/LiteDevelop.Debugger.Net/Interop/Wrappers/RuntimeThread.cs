using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Framework.Debugging;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{

    /*
     * A thread's DebugState determines whether the debugger lets a thread
     * run or not.  Possible states are:
     *
     * THREAD_RUN - thread runs freely, unless a debug event occurs
     * THREAD_SUSPEND - thread cannot run.
     *
     * NOTE: We allow for message pumping via a callback provided to the Hosting
     *		API, thus we don't need an 'interrupted' state here.
     */

    public enum CorDebugThreadState
    {
        Run,
        Suspend
    }

    public class RuntimeThread : DebuggerSessionObject, IThread
    {
        private RuntimeAppDomain _domain;
        private ICorDebugThread _comThread;
        private RuntimeFrame _currentFrame;
        private RuntimeValue _currentException;
        private RuntimeValue _threadObject;

        internal RuntimeThread(RuntimeAppDomain domain, ICorDebugThread comThread)
        {
            _domain = domain;
            _comThread = comThread;
        }

        internal ICorDebugThread ComThread
        {
            get { return _comThread; }
        }

        #region IThread Members

        public IntPtr Handle
        {
            get
            {
                IntPtr handle;
                _comThread.GetHandle(out handle);
                return handle;
            }
        }

        int IThread.Id
        {
            get
            {
                return (int)Id;
            }
        }

        IThreadProvider IThread.Parent
        {
            get { return AppDomain; }
        }

        IFrame IThread.CurrentFrame
        {
            get { return CurrentFrame; }
        }

        public RuntimeFrame CurrentFrame
        {
            get
            {
                ICorDebugFrame comFrame;
                _comThread.GetActiveFrame(out comFrame);

                if (comFrame == null)
                    return null;

                if (_currentFrame == null || _currentFrame.ComFrame != comFrame)
                {
                    ICorDebugFunction function;
                    comFrame.GetFunction(out function);
                    ICorDebugModule module;
                    function.GetModule(out module);
                    ICorDebugAssembly assembly;
                    module.GetAssembly(out assembly);

                    var runtimeFunction = AppDomain.GetAssembly(assembly).GetModule(module).GetFunction(function);
                    _currentFrame = new RuntimeFrame(runtimeFunction, this, comFrame);
                }
                return _currentFrame;
            }
        }

        IChain IThread.CurrentChain
        {
            get { return CurrentChain; }
        }

        public RuntimeChain CurrentChain
        {
            get
            {
                return CurrentFrame != null ? CurrentFrame.Chain : null;
            }
        }

        IValue IThread.CurrentException
        {
            get { return CurrentException; }
        }

        public RuntimeValue CurrentException
        {
            get
            {
                ICorDebugValue exceptionValue;
                _comThread.GetCurrentException(out exceptionValue);

                if (exceptionValue == null)
                    return null;

                if (_currentException == null || _currentException.ComValue != exceptionValue)
                {
                    _currentException = new RuntimeValue(Session, exceptionValue);
                }

                return _currentException;
            }
        }

        public ThreadState State
        {
            get
            {
                ThreadState state;
                _comThread.GetUserState(out state);
                return state;
            }
        }

        IEvaluation IThread.CreateEvaluation()
        {
            return CreateEvaluation();
        }

        public RuntimeEvaluation CreateEvaluation()
        {
            ICorDebugEval comEval;
            _comThread.CreateEval(out comEval);
            return new RuntimeEvaluation(this, comEval);
        }

        #endregion

        public override NetDebuggerSession Session
        {
            get { return _domain.Session; }
        }

        public uint Id
        {
            get
            {
                uint id;
                _comThread.GetID(out id);
                return id;
            }
        }

        public RuntimeAppDomain AppDomain
        {
            get { return _domain; }
        }

        public RuntimeValue ThreadObject
        {
            get
            {
                ICorDebugValue value;
                _comThread.GetObject(out value);
                if (value == null)
                    return null;

                if (_threadObject == null || _threadObject.ComValue != value)
                {
                    _threadObject = new RuntimeValue(Session, value);
                }
                return _threadObject;
            }
        }

        public DebuggeeProcess Process
        {
            get { return _domain.Process; }
        }
    }
}
