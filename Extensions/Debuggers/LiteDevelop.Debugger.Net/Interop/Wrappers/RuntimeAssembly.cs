using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Framework.Debugging;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeAssembly : DebuggerSessionObject
    {
        public event GenericDebuggerEventHandler<RuntimeModule> ModuleLoad;
        public event GenericDebuggerEventHandler<RuntimeModule> ModuleUnload;

        private readonly RuntimeAppDomain _domain;
        private readonly ICorDebugAssembly _comAssembly;
        private readonly Dictionary<ICorDebugModule, RuntimeModule> _modules = new Dictionary<ICorDebugModule, RuntimeModule>();

        internal RuntimeAssembly(RuntimeAppDomain domain, ICorDebugAssembly comAssembly)
        {
            _domain = domain;
            _comAssembly = comAssembly;
        }

        internal ICorDebugAssembly ComAssembly
        {
            get { return _comAssembly; }
        }

        public override NetDebuggerSession Session
        {
            get { return _domain.Session; }
        }

        public string Name
        {
            get
            {
                uint size = 255;
                var buffer = new char[size];
                ComAssembly.GetName(size, out size, buffer);
                return new string(buffer).TrimEnd('\0');
            }
        }

        public RuntimeAppDomain Domain
        {
            get { return _domain; }
        }

        public IEnumerable<RuntimeModule> Modules
        {
            get { return _modules.Values; }
        }

        public RuntimeModule FindModule(Predicate<RuntimeModule> predicate)
        {
            return Modules.FirstOrDefault(x => predicate(x));
        }

        internal RuntimeModule GetModule(ICorDebugModule module)
        {
            return _modules[module];
        }

        internal void DispatchModuleLoadEvent(GenericDebuggerEventArgs<RuntimeModule> e)
        {
            _modules.Add(e.TargetObject.ComModule, e.TargetObject);
            OnModuleLoad(e);
        }

        internal void DispatchModuleUnloadEvent(GenericDebuggerEventArgs<RuntimeModule> e)
        {
            _modules.Remove(e.TargetObject.ComModule);
            OnModuleUnload(e);
        }

        protected virtual void OnModuleLoad(GenericDebuggerEventArgs<RuntimeModule> e)
        {
            if (ModuleLoad != null)
                ModuleLoad(this, e);
        }

        protected virtual void OnModuleUnload(GenericDebuggerEventArgs<RuntimeModule> e)
        {
            if (ModuleUnload != null)
                ModuleUnload(this, e);
        }
    }
}
