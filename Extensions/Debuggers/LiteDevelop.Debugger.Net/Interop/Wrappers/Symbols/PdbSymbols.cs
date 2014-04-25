using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com.Symbols;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers.Symbols
{
    public class PdbSymbols  : ISymbolsProvider
    {
        private ISymUnmanagedReader _comReader;
        private readonly Dictionary<SymbolToken, MethodSymbols> _cachedSymbols = new Dictionary<SymbolToken, MethodSymbols>();

        internal PdbSymbols(ISymUnmanagedReader comReader)
        {
            _comReader = comReader;
            
        }

        #region ISymbolsProvider Members

        public bool TryGetFunctionByLocation(SourceLocation location, out SymbolToken token)
        {
            token = default(SymbolToken);

            try
            {
                var document = _comReader.GetDocument(location.FilePath.FullPath, default(Guid), default(Guid), default(Guid));
                if (document == null)
                    return false;

                var method = _comReader.GetMethodFromDocumentPosition(document, location.Line, location.Column);
                if (method == null)
                    return false;

                token = method.GetToken();
                return true;
            }
            catch (COMException)
            {
                return false;
            }
        }

        public bool IsCompilerGenerated(SymbolToken token)
        {
            return GetSymMethod(token) == null;
        }

        IFunctionSymbols ISymbolsProvider.GetFunctionSymbols(IFunction function)
        {
            return GetFunctionSymbols((RuntimeFunction)function);
        }

        #endregion

        public SymbolToken Entrypoint
        {
            get
            {
                return _comReader.GetUserEntryPoint();
            }
        }

        private ISymUnmanagedMethod GetSymMethod(SymbolToken token)
        {
            try
            {
                return _comReader.GetMethod(token);
            }
            catch
            {
                return null;
            }
        }

        public MethodSymbols GetFunctionSymbols(RuntimeFunction function)
        {
            MethodSymbols symbols;
            if (!_cachedSymbols.TryGetValue(function.Token, out symbols))
            {
                var symMethod = GetSymMethod(function.Token);
                if (symMethod == null)
                    return null;

                _cachedSymbols.Add(function.Token, symbols = new MethodSymbols(function, symMethod));
            }

            return symbols;
        }
    }
}
