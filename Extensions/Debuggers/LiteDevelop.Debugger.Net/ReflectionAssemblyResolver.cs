using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

namespace LiteDevelop.Debugger.Net
{
    public class ReflectionAssemblyResolver : IAssemblyResolver
    {
        private readonly AppDomain _domain;

        public ReflectionAssemblyResolver()
        {
            _domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath, AppDomain.CurrentDomain.ShadowCopyFiles);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asmName = new AssemblyName(args.Name);
            try
            {
                var assembly =  Assembly.Load(asmName);
                if (assembly != null)
                    return assembly;
            }
            catch
            {

            }
            
            return Assembly.LoadFrom(Path.Combine(
                     Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                     asmName.Name + ".dll"));
        }

        #region IAssemblyResolver Members

        public IModuleDefinition ResolveModule(string name)
        {
            // TODO: quite ugly, need to have better handling with transparent proxies.
            dynamic moduleProxy = _domain.CreateInstanceFromAndUnwrap(
                typeof(ReflectionModuleProxy).Assembly.Location, 
                typeof(ReflectionModuleProxy).FullName);

            moduleProxy.LoadModule(name);
            return moduleProxy;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            AppDomain.Unload(_domain);
        }

        #endregion
    }

    [Serializable]
    public class ReflectionModuleProxy : MarshalByRefObject, IModuleDefinition
    {
        private Module _module;

        public ReflectionModuleProxy()
        {
            
        }

        public void LoadModule(string location)
        {
            _module = Assembly.LoadFile(location).ManifestModule;
        }

        #region IModuleDefinition Members

        public IMemberDefinition ResolveMember(int token)
        {
            var member = _module.ResolveMember(token);
            var type = member as Type;
            if (type != null)
                return new ReflectionTypeProxy(type);
            return new ReflectionMemberProxy(member);
        }

        #endregion

        #region IMetaDataTokenProvider Members

        public int MetaDataToken
        {
            get { return _module.MetadataToken; }
        }

        #endregion
    }

    [Serializable]
    public class ReflectionMemberProxy : MarshalByRefObject, IMemberDefinition
    {
        public ReflectionMemberProxy(MemberInfo member)
        {
            MemberInfo = member;
        }

        public MemberInfo MemberInfo
        {
            get;
            private set;
        }

        #region IMemberDefinition Members

        public string Name
        {
            get { return MemberInfo.Name; }
        }

        #endregion

        #region IMetaDataTokenProvider Members

        public int MetaDataToken
        {
            get { return MemberInfo.MetadataToken; }
        }

        #endregion
    }


    [Serializable]
    public class ReflectionTypeProxy : MarshalByRefObject, ITypeDefinition
    {
        public ReflectionTypeProxy(Type type)
        {
            Type = type;
        }

        public Type Type
        {
            get;
            private set;
        }

        #region IMemberDefinition Members

        public string Name
        {
            get { return Type.Name; }
        }

        #endregion

        #region IMetaDataTokenProvider Members

        public int MetaDataToken
        {
            get { return Type.MetadataToken; }
        }

        #endregion

        #region ITypeDefinition Members

        public IMemberDefinition FindField(string name)
        {
            var field = Type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic);

            if (field == null)
                return null;

            return new ReflectionMemberProxy(field);
        }

        public IMemberDefinition FindMethod(string name)
        {
            var method = Type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            if (method == null)
                return null;

            return new ReflectionMemberProxy(method);
        }

        #endregion

    }
}
