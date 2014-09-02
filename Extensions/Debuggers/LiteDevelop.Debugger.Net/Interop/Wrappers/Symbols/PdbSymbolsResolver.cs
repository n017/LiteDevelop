using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com.Symbols;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers.Symbols
{
    public class PdbSymbolsResolver : ISymbolsResolver
    {
        private readonly IMetaDataDispenser _dispenser;
        private readonly ISymUnmanagedBinder2 _symbolBinder;
        private readonly Dictionary<string, PdbSymbols> _readers = new Dictionary<string, PdbSymbols>();
        private readonly ComInstanceCollector _instanceCollector;

        internal PdbSymbolsResolver(ComInstanceCollector instanceCollector)
        {
            _instanceCollector = instanceCollector;
            _instanceCollector.AddComObject(_dispenser = new IMetaDataDispenser());
            _instanceCollector.AddComObject(_symbolBinder = new ISymUnmanagedBinder2());
        }

        #region ISymbolsResolver Members

        ISymbolsProvider ISymbolsResolver.GetSymbolsProviderForFile(string assemblyFile)
        {
            return GetSymbolsProviderForFile(assemblyFile);
        }

        #endregion

        public PdbSymbols GetSymbolsProviderForFile(string assemblyFile)
        {
            PdbSymbols reader;
            if (!_readers.TryGetValue(assemblyFile, out reader))
            {
                ISymUnmanagedReader rawReader;
                var importerIID = typeof(IMetaDataImport).GUID;
                var importer = _dispenser.OpenScope(assemblyFile, 0, ref importerIID);

                int result = _symbolBinder.GetReaderForFile2(importer, 
                    assemblyFile,
                    Path.GetDirectoryName(assemblyFile), 
                    SymSearchPolicies.AllowOriginalPathAccess,
                    out rawReader);

                if (result == (int)DiaErrors.E_PDB_NOT_FOUND)
                    return null;

                if (result < 0)
                    throw new Win32Exception(result);

                _readers.Add(assemblyFile, reader = new PdbSymbols(rawReader));
                _instanceCollector.AddComObject(rawReader);
                _instanceCollector.AddComObject(importer);
            }

            return reader;
        }

    }
}
