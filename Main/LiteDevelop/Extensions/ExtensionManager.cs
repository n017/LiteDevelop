using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LiteDevelop.Framework;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Extensions
{
    /// <summary>
    /// A manager that handles operations for loading extensions.
    /// </summary>
    internal sealed class ExtensionManager : IExtensionManager 
    {
        private readonly Dictionary<string, Assembly> _resolvedAssemblies = new Dictionary<string, Assembly>();

        private ILiteExtensionHost _extensionHost;

        public ExtensionManager(ILiteExtensionHost extensionHost)
        {
            this._extensionHost = extensionHost;
            this.LoadedExtensions = new EventBasedCollection<LiteExtension>();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            LoadExtension(typeof(LiteDevelopExtension));
            LoadExtension(typeof(CoreExtension));
        }

        public EventBasedCollection<LiteExtension> LoadedExtensions
        {
            get;
            private set;
        }

        #region IExtensionManager Members

        IList<LiteExtension> IExtensionManager.LoadedExtensions
        {
            get { return LoadedExtensions.AsReadOnly(); }
        }

        public ExtensionLoadResult LoadExtension(Type extensionType)
        {
            try
            {
                var extension = Activator.CreateInstance(extensionType) as LiteExtension;
                LoadedExtensions.Add(extension);
                extension.Initialize(new ExtensionInitializationContext(LiteDevelopApplication.Current.IsInitialized ? InitializationTime.UserLoad : InitializationTime.Startup));
                return new ExtensionLoadResult(extensionType, extension);
            }
            catch (Exception ex)
            {
                return new ExtensionLoadResult(extensionType, ex);
            }
        }

        public LiteExtension GetLoadedExtension(Type extensionType)
        {
            return LoadedExtensions.FirstOrDefault(x => x.GetType() == extensionType);
        }

        public T GetLoadedExtension<T>() where T : LiteExtension
        {
            return LoadedExtensions.FirstOrDefault(x => x is T) as T;
        }

        public IEnumerable<IFileHandler> GetFileHandlers(FilePath filePath)
        {
            foreach (var extension in LoadedExtensions)
            {
                if (extension is IFileHandler)
                {
                    var fileHandler = extension as IFileHandler;
                    if (fileHandler.CanOpenFile(filePath))
                        yield return fileHandler;
                }
            }
        }

        public IFileHandler GetPreferredFileHandler(FilePath filePath)
        {
            return GetFileHandlers(filePath).FirstOrDefault();
        }

        public IEnumerable<IDebugger> GetDebuggers()
        {
            foreach (var extension in LoadedExtensions)
                if (extension is IDebugger)
                    yield return extension as IDebugger;
        }

        public IEnumerable<IDebugger> GetDebuggers(Project project)
        {
            foreach (var extension in LoadedExtensions)
                if (extension is IDebugger && (extension as IDebugger).CanDebugProject(project))
                    yield return extension as IDebugger;
        }

        public IDebugger GetPreferredDebugger(Project project)
        {
            return GetDebuggers(project).FirstOrDefault();
        }

        #endregion

        /// <summary>
        /// Searches for extension types in a specific file.
        /// </summary>
        /// <param name="file">The file to search for extensions.</param>
        /// <returns>Returns an array of types that are extensions.</returns>
        public Type[] SearchExtensionTypes(ExtensionLibraryData file)
        {
            var extensionTypes = new List<Type>();

            var assembly = Assembly.LoadFile(file.GetAbsolutePath());
            _resolvedAssemblies.Add(assembly.FullName, assembly);

            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsAbstract && type.BaseType.IsBasedOn(typeof(LiteExtension)))
                {
                    extensionTypes.Add(type);
                }
            }

            return extensionTypes.ToArray();
        }

        /// <summary>
        /// Searches for extensions in a particular file and tries to load them. 
        /// </summary>
        /// <param name="file">The file to load.</param>
        /// <param name="extension">The extension that the file contains</param>
        /// <returns>Returns an array of loading results, indicating which extension loaded succesfully and which didn't.</returns>
        public ExtensionLoadResult[] LoadExtensions(ExtensionLibraryData file)
        {
            try
            {
                if (LoadedExtensions.FirstOrDefault(x => x.GetType().Assembly.Location.Equals(file.GetAbsolutePath(), StringComparison.OrdinalIgnoreCase)) != null)
                    throw new InvalidOperationException("The extension library is already loaded.");

                var extensionTypes = SearchExtensionTypes(file);

                if (extensionTypes.Length == 0)
                    throw new BadImageFormatException(string.Format("No extension classes found in file {0}", file));
                
                var results = new ExtensionLoadResult[extensionTypes.Length];
                for (int i = 0; i < extensionTypes.Length; i++)
                {
                    results[i] = LoadExtension(extensionTypes[i]);
                }
                return results;
            }
            catch(Exception ex)
            {
                return new ExtensionLoadResult[] { new ExtensionLoadResult(file.GetAbsolutePath(), ex) };
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asmName = new AssemblyName(args.Name);
            string fileName = asmName.Name + ".dll";

            Assembly resolvedAssembly = null;

            if (args.RequestingAssembly != null && !_resolvedAssemblies.TryGetValue(asmName.FullName, out resolvedAssembly))
            {
                string asmPath = args.RequestingAssembly.Location;
                string asmFolder = Path.GetDirectoryName(asmPath);

                if (File.Exists(Path.Combine(asmFolder, fileName)))
                {
                    try
                    {
                        resolvedAssembly = Assembly.LoadFile(Path.Combine(asmFolder, fileName));
                        _resolvedAssemblies.Add(asmName.FullName, resolvedAssembly);
                        return resolvedAssembly;
                    }
                    catch { }
                }
            }

            return resolvedAssembly;
        }
    }
}
