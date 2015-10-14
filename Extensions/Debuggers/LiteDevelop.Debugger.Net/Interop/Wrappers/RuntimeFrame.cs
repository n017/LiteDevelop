using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum CorDebugMappingResult
    {
        MAPPING_APPROXIMATE = 0x20,
        MAPPING_EPILOG = 2,
        MAPPING_EXACT = 0x10,
        MAPPING_NO_INFO = 4,
        MAPPING_PROLOG = 1,
        MAPPING_UNMAPPED_ADDRESS = 8
    }

    public class RuntimeFrame : DebuggerSessionObject, IFrame
    {
        private readonly RuntimeFunction _function;
        private readonly RuntimeThread _thread;
        private readonly ICorDebugFrame _comFrame;
        private RuntimeChain _chain;

        internal RuntimeFrame(RuntimeFunction function, RuntimeThread thread, ICorDebugFrame comFrame)
        {
            _thread = thread;
            _function = function;
            _comFrame = comFrame;
        }

        internal RuntimeFrame(RuntimeThread thread, ICorDebugFrame comFrame)
        {
            _thread = thread; 
            _comFrame = comFrame;

            if (!IsExternal)
            {
                ICorDebugFunction comFunction;
                comFrame.GetFunction(out comFunction);
                ICorDebugModule comModule;
                comFunction.GetModule(out comModule);

                var module = Session.FindModule(x => x.ComModule == comModule);
                _function = module.GetFunction(comFunction);
            }
        }

        public override NetDebuggerSession Session
        {
            get { return _thread.Session; }
        }

        internal ICorDebugFrame ComFrame
        {
            get { return _comFrame; }
        }

        internal ICorDebugILFrame ComILFrame
        {
            get { return _comFrame as ICorDebugILFrame; }
        }

        internal ICorDebugNativeFrame ComNativeFrame
        {
            get { return _comFrame as ICorDebugNativeFrame; }
        }

        #region IFrame Members

        public RuntimeFunction Function
        {
            get { return _function; }
        }

        IFunction IFrame.Function
        {
            get { return Function; }
        }

        public RuntimeThread Thread
        {
            get { return _thread; }
        }

        IThread IFrame.Thread
        {
            get { return Thread; }
        }

        public RuntimeChain Chain
        {
            get
            {
                ICorDebugChain comChain;
                _comFrame.GetChain(out comChain);

                if (comChain == null)
                    return null;

                if (_chain == null || _chain.ComChain != comChain)
                {
                    _chain = Session.ComInstanceCollector.GetWrapper<RuntimeChain>(comChain);
                    if (_chain == null)
                        Session.ComInstanceCollector.SetWrapper(comChain, _chain = new RuntimeChain(Thread, comChain));
                }

                return _chain;
            }
        }

        IChain IFrame.Chain
        {
            get { return Chain; }
        }

        public bool IsUserCode
        {
            get
            {
                return Function != null && Function.Symbols != null;
            }
        }

        public bool IsExternal
        {
            get
            {
                return ComILFrame == null && ComNativeFrame == null;
            }
        }

        public uint GetOffset()
        {
            CorDebugMappingResult result;
            if (IsIL)
                return GetILOffset(out result);
            return GetNativeOffset();
        }

        /// <summary>
        /// Gets the value of the given local variable.
        /// </summary>
        /// <param name="index">The index of the variable to get the value from.</param>
        /// <returns></returns>
        IValue IFrame.GetLocalVariableValue(uint index)
        {
            return GetLocalVariableValue(index);
        }

        #endregion

        public bool IsIL
        {
            get { return _comFrame is ICorDebugILFrame; }
        }

        public uint GetILOffset(out CorDebugMappingResult result)
        {
            result = CorDebugMappingResult.MAPPING_NO_INFO;

            if (IsIL)
            {
                uint offset;
                ComILFrame.GetIP(out offset, out result);

                return offset;
            }

            throw new InvalidOperationException("Can only get IL offset from an IL frame.");
        }

        public uint GetNativeOffset()
        {
            if (!IsIL)
            {
                uint offset;
                ComNativeFrame.GetIP(out offset);
                return offset;
            }

            throw new InvalidOperationException("Can only get native offset from a native frame.");
        }

        public SourceStepper CreateStepper()
        {
            ICorDebugStepper comStepper;
            _comFrame.CreateStepper(out comStepper);

            var stepper = Function.Module.Assembly.Domain.GetStepper(comStepper);
            if (stepper == null)
                Function.Module.Assembly.Domain.AddStepper(stepper = new SourceStepper(Thread, comStepper));
            
            return stepper;
        }

        public RuntimeValue GetLocalVariableValue(uint index)
        {
            if (IsIL)
            {
                ICorDebugValue value;
                ComILFrame.GetLocalVariable(index, out value);
                return new RuntimeValue(Session, value);
            }

            throw new ArgumentException("Can only get variable value from an IL frame.");
        }
    }
}
