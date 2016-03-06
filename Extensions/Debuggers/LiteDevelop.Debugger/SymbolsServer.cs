using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    public class SymbolsServer : ISymbolsResolver
    {
        private readonly List<ISymbolsResolver> _resolvers = new List<ISymbolsResolver>();

        public SymbolsServer()
        {
        }

        public SymbolsServer(params ISymbolsResolver[] resolvers)
        {
            _resolvers.AddRange(resolvers);
        }

        public IList<ISymbolsResolver> Resolvers
        {
            get { return _resolvers; }
        } 

        public ISymbolsProvider GetSymbolsProviderForFile(string assemblyFile)
        {
            foreach (var resolver in Resolvers)
            {
                var provider = resolver.GetSymbolsProviderForFile(assemblyFile);
                if (provider != null)
                    return provider;
            }

            return null;
        }
    }
}
