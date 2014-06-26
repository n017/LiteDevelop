using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeValue : DebuggerSessionObject, IValue 
    {
        private readonly NetDebuggerSession _session;
        private readonly ICorDebugValue _comValue;
        private readonly Dictionary<SymbolToken, RuntimeValue> _fieldValues = new Dictionary<SymbolToken, RuntimeValue>();
        private readonly Dictionary<SymbolToken, RuntimeValue> _propertyValues = new Dictionary<SymbolToken, RuntimeValue>();
        private RuntimeTypeDescriptor _type;
        private RuntimeValue _dereferencedValue;

        internal RuntimeValue(NetDebuggerSession session, ICorDebugValue comValue)
        {
            _session = session;
            _comValue = comValue;
        }

        public override NetDebuggerSession Session
        {
            get { return _session; }
        }

        internal ICorDebugValue ComValue
        {
            get { return _comValue; }
        }

        internal ICorDebugValue2 ComValue2
        {
            get { return _comValue as ICorDebugValue2; }
        }

        internal ICorDebugArrayValue ComArrayValue
        {
            get { return _comValue as ICorDebugArrayValue; }
        }

        internal ICorDebugBoxValue ComBoxValue
        {
            get { return _comValue as ICorDebugBoxValue; }
        }

        internal ICorDebugGenericValue ComGenericValue
        {
            get { return _comValue as ICorDebugGenericValue; }
        }

        internal ICorDebugHeapValue ComHeapValue
        {
            get { return _comValue as ICorDebugHeapValue; }
        }

        internal ICorDebugObjectValue ComObjectValue
        {
            get { return _comValue as ICorDebugObjectValue; }
        }

        internal ICorDebugReferenceValue ComReferenceValue
        {
            get { return _comValue as ICorDebugReferenceValue; }
        }

        internal ICorDebugStringValue ComStringValue
        {
            get { return _comValue as ICorDebugStringValue; }
        }

        #region IValue Members

        public uint Size
        {
            get
            {
                uint size;
                _comValue.GetSize(out size);
                return size;
            }
        }

        public ulong Address
        {
            get
            {
                ulong address;
                _comValue.GetAddress(out address);
                return address;
            }
        }

        public bool IsNull
        {
            get
            {
                if (ComReferenceValue != null)
                {
                    int value;
                    ComReferenceValue.IsNull(out value);
                    return value == 1;
                }
                return false;
            }
        }

        public bool IsReference
        {
            get
            {
                return ComReferenceValue != null;
            }
        }

        public bool IsObject
        {
            get
            {
                return ComObjectValue != null;
            }
        }

        public bool IsArray
        {
            get { return ComArrayValue != null; }
        }

        public bool IsString
        {
            get { return ComStringValue != null; }
        }

        IType IValue.Type
        {
            get { return Type; }
        }

        public RuntimeTypeDescriptor Type
        {
            get
            {
                ICorDebugType type;
                ComValue2.GetExactType(out type);

                if (type == null)
                    return null;

                if (_type == null || _type.ComType != type)
                {
                    _type = new RuntimeTypeDescriptor(Session, type);
                }
                return _type;
            }
        }

        public string ValueAsString()
        {
            if (IsNull)
                return "null";

            if (IsReference)
                return Dereference().ValueAsString();
            if (IsString)
                return GetStringValue();
            if (IsArray)
                return string.Format("{0}[{1}]", GetArrayElementType(), string.Join(",", GetArrayDimensions()));
            if (IsObject)
                // TODO: call ToString()
                return "{" + Type.ToString() + "}";

            var primitive = GetPrimitiveValue();
            if (primitive == null)
                return "null";
            return primitive.ToString();
        }

        #endregion

        public ElementType ElementType
        {
            get
            {
                ElementType type;
                _comValue.GetType(out type);
                return type;
            }
        }

        public object GetPrimitiveValue()
        {
            if (IsNull)
                return null;
            
            if (ElementType == ElementType.String)
            {
                ICorDebugValue value;
                ComReferenceValue.Dereference(out value);
                var stringValue = value as ICorDebugStringValue;
            
                if (stringValue != null)
                {
                    uint size = 0;
                    stringValue.GetString(size, out size, new char[0]);
                    char[] buffer = new char[size];
                    stringValue.GetString(size, out size, buffer);
                    return new string(buffer);
                }
            
                return null;
            }
            else
            {
                return GetElementValue();
            }
        }

        private object GetElementValue()
        {
            int size = (int)this.Size;
            IntPtr bytesPtr = Marshal.AllocHGlobal(size);
            try
            {
                ComGenericValue.GetValue(bytesPtr);
                byte[] bytes = new byte[size];
                Marshal.Copy(bytesPtr, bytes, 0, size);

                switch (ElementType)
                {
                    case ElementType.Boolean: return bytes[0] == 1;
                    case ElementType.Char: return BitConverter.ToChar(bytes, 0);
                    case ElementType.I1: return (sbyte)((sbyte)(bytes[0] & 0x7F) & bytes[0] & 0x80);
                    case ElementType.I2: return BitConverter.ToInt16(bytes, 0);
                    case ElementType.I4: return BitConverter.ToInt32(bytes, 0);
                    case ElementType.I8: return BitConverter.ToInt64(bytes, 0);
                    case ElementType.R4: return BitConverter.ToSingle(bytes, 0);
                    case ElementType.R8: return BitConverter.ToDouble(bytes, 0);
                    case ElementType.U1: return bytes[0];
                    case ElementType.U2: return BitConverter.ToUInt16(bytes, 0);
                    case ElementType.U4: return BitConverter.ToUInt32(bytes, 0);
                    case ElementType.U8: return BitConverter.ToUInt64(bytes, 0);
                    case ElementType.I:
                        if (size == sizeof(int))
                            return new IntPtr(BitConverter.ToInt32(bytes, 0));
                        return new IntPtr(BitConverter.ToInt64(bytes, 0));
                    case ElementType.U:
                        if (size == sizeof(uint))
                            return new UIntPtr(BitConverter.ToUInt32(bytes, 0));
                        return new UIntPtr(BitConverter.ToUInt64(bytes, 0));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(bytesPtr);
            }
            return null;
        }

        public RuntimeValue Dereference()
        {
            if (!IsReference)
                throw new InvalidOperationException("Value must be a reference in order to be dereferenced.");

            ICorDebugValue value;
            ComReferenceValue.Dereference(out value);

            if (value == null)
                return null;

            if (_dereferencedValue == null || _dereferencedValue.ComValue != value)
            {
                _dereferencedValue = new RuntimeValue(Session, value);
            }
            return _dereferencedValue;
        }

        public ElementType GetArrayElementType()
        {
            if (!IsArray)
                throw new InvalidOperationException("Value is not an array.");

            ElementType type;
            ComArrayValue.GetElementType(out type);
            return type;

        }
        public uint GetArrayElementCount()
        {
            if (!IsArray)
                throw new InvalidOperationException("Value is not an array.");

            uint count;
            ComArrayValue.GetCount(out count);
            return count;
        }

        public uint[] GetArrayDimensions()
        {
            if (!IsArray)
                throw new InvalidOperationException("Value is not an array.");

            if (ElementType == Wrappers.ElementType.SzArray)
                return new uint[1] { GetArrayElementCount() };

            uint rank = 0;
            ComArrayValue.GetRank(out rank);
            uint[] dimensions = new uint[rank];
            ComArrayValue.GetDimensions(rank, dimensions);
            return dimensions;
        }

        public RuntimeValue GetArrayElement(uint index)
        {
            if (!IsArray)
                throw new InvalidOperationException("Value is not an array.");

            ICorDebugValue value;
            ComArrayValue.GetElementAtPosition(index, out value);
            return new RuntimeValue(Session, value);
        }

        public string GetStringValue()
        {
            if (!IsString)
                throw new InvalidOperationException("Value is not a string.");

            uint length = 0;
            ComStringValue.GetLength(out length);

            if (length == 0)
                return string.Empty;

            char[] buffer = new char[length];
            ComStringValue.GetString(length, out length, buffer);
            return new string(buffer);
        }

        public RuntimeValue GetFieldValue(RuntimeThread thread, SymbolToken token)
        {
            // TODO: static members

            if (!IsObject)
                throw new InvalidOperationException("Value must be an object in order to get values of fields.");

            RuntimeValue value;
            if (!_fieldValues.TryGetValue(token, out value))
            {
                ICorDebugValue comValue;
                ComObjectValue.GetFieldValue(Type.Class.ComClass, (uint)token.GetToken(), out comValue);
                _fieldValues.Add(token, value = new RuntimeValue(Session, comValue));
            }
            return value;
        }

        public RuntimeValue CallFunction(RuntimeThread thread, SymbolToken token)
        {
            return null;
        }
    }
}
