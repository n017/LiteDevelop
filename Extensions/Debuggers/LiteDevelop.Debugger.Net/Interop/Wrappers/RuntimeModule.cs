using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Framework;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Debugger.Net.Interop.Wrappers.Symbols;
using LiteDevelop.Framework.Debugging;
namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeModule : DebuggerSessionObject, IModule
    {
        private readonly RuntimeAssembly _assembly;
        private readonly ICorDebugModule _comModule;
        private readonly Dictionary<ICorDebugFunction, RuntimeFunction> _cachedFunctions = new Dictionary<ICorDebugFunction, RuntimeFunction>();
    
        internal RuntimeModule(RuntimeAssembly assembly, ICorDebugModule comModule)
        {
            _assembly = assembly;
            _comModule = comModule;

            Symbols = _assembly.Domain.Process.Session.SymbolsServer.GetSymbolsProviderForFile(this.Name);
            if (Symbols != null)
            {
                int index = 0, max = Session.PendingBreakpoints.Count;
                Session.ProgressReporter.Report("Symbol reader present. Finding pending breakpoints to set.");
                while (index < max)
                {
                    if (TrySetBreakpoint(Session.PendingBreakpoints[index]))
                    {
                        max--;
                        Session.PendingBreakpoints.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }

        internal ICorDebugModule ComModule
        {
            get { return _comModule; }
        }

        public override NetDebuggerSession Session
        {
            get { return _assembly.Session; }
        }

        #region IModule Members

        public string Name
        {
            get
            {
                uint size = 255;
                var buffer = new char[size];
                ComModule.GetName(size, out size, buffer);
                return new string(buffer).TrimEnd('\0');
            }
        }

        public ISymbolsProvider Symbols
        {
            get;
            private set;
        }

        #endregion

        public RuntimeAssembly Assembly
        {
            get { return _assembly; }
        }

        public bool TrySetBreakpoint(BreakpointBookmark breakpoint)
        {
            SymbolToken token;

            Session.ProgressReporter.Report("Trying to set breakpoint {0}:{1} in module {2}", breakpoint.Location.FilePath, breakpoint.Location.Line, Name);
            if (Symbols.TryGetFunctionByLocation(breakpoint.Location, out token))
            {
                Session.ProgressReporter.Report("Method token found.");
                var function = GetFunction((uint)token.GetToken());
                if (function == null)
                    return false;

                Session.ProgressReporter.Report("Method found. Finding IL offset");
                var sequencePoint = function.Symbols.GetSequencePointByLine(breakpoint.Location.Line);
                if (sequencePoint == null)
                    return false;
                
                Session.ProgressReporter.Report("Setting breakpoint at offset {0}", sequencePoint.Offset);
                function.Code.CreateBreakpoint(sequencePoint.Offset);
                return true;
            }
            return false;
        }

        public RuntimeFunction GetFunction(uint token)
        {
            var function = _cachedFunctions.FirstOrDefault(x => x.Value.Token.GetToken() == token).Value;
            if (function == null)
            {
                ICorDebugFunction comFunction;
                _comModule.GetFunctionFromToken(token, out comFunction);
                function = GetFunction(comFunction);
            }
            return function;
        }

        internal RuntimeFunction GetFunction(ICorDebugFunction comFunction)
        {
            RuntimeFunction function;
            if (!_cachedFunctions.TryGetValue(comFunction, out function))
            {
                _cachedFunctions.Add(comFunction, function = new RuntimeFunction(this, comFunction));
            }
            return function;
        }
    }
}
