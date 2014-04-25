using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeTypeDescriptor : DebuggerSessionObject, IType 
    {
        private NetDebuggerSession _session;
        private ICorDebugType _comType;
        private RuntimeClass _class;
        private RuntimeTypeDescriptor _baseDescriptor;

        internal RuntimeTypeDescriptor(NetDebuggerSession session, ICorDebugType comType)
        {
            _session = session;
            _comType = comType;
        }

        public override NetDebuggerSession Session
        {
            get { return _session; }
        }

        internal ICorDebugType ComType
        {
            get { return _comType; }
        }

        #region IType Members

        IType IType.BaseType
        {
            get { return BaseType; }
        }

        public RuntimeTypeDescriptor BaseType
        {
            get
            {
                ICorDebugType baseType;
                _comType.GetBase(out baseType);

                if (baseType == null)
                    return null;

                if (_baseDescriptor == null || _baseDescriptor.ComType != baseType)
                {
                    _baseDescriptor = new RuntimeTypeDescriptor(Session, baseType);
                }
                return _baseDescriptor;
            }
        }

        #endregion

        public RuntimeClass Class
        {
            get
            {
                if (ElementType == Wrappers.ElementType.Class || ElementType == Wrappers.ElementType.ValueType)
                {
                    ICorDebugClass @class;
                    _comType.GetClass(out @class);

                    if (@class == null)
                        return null;

                    if (_class == null || _class.ComClass != @class)
                    {
                        _class = new RuntimeClass(Session, @class);
                    }
                }

                return _class;
            }
        }

        public ElementType ElementType
        {
            get
            {
                ElementType type;
                _comType.GetType(out type);
                return type;
            }
        }

        public override string ToString()
        {
            if (Class != null)
                return string.Format("{0} ({1:X8})", ElementType, Class.Token.GetToken());
            return ElementType.ToString();
        }
    }
}
