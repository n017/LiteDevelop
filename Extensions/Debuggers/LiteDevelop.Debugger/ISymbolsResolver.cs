using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    public interface ISymbolsResolver
    {
        ISymbolsProvider GetSymbolsProviderForFile(string assemblyFile);
    }
}
