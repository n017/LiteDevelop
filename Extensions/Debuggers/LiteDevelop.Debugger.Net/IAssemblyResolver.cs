using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger.Net
{
    public interface IAssemblyResolver : IDisposable
    {
        IModuleDefinition ResolveModule(string name);
    }

    public interface IMetaDataTokenProvider
    {
        int MetaDataToken { get; }
    }

    public interface IModuleDefinition : IMetaDataTokenProvider
    {
        IMemberDefinition ResolveMember(int token);
    }

    public interface IMemberDefinition : IMetaDataTokenProvider
    {
        string Name { get; }
    }

    public interface ITypeDefinition : IMemberDefinition
    {
        IMemberDefinition FindField(string name);
        IMemberDefinition FindMethod(string name);
    }
}
