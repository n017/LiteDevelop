using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Framework.Extensions
{
    /// <summary>
    /// Provides information about the result of a loading extension operation.
    /// </summary>
    public class ExtensionLoadResult
    {
        public ExtensionLoadResult(string file, Exception error)
        {
            this.FilePath = file;
            this.Error = error;
        }

        public ExtensionLoadResult(Type type, LiteExtension extension)
        {
            this.ExtensionType = type;
            this.Extension = extension;
            this.FilePath = type.Assembly.Location;
        }

        public ExtensionLoadResult(Type type, Exception error)
        {
            this.ExtensionType = type;
            this.Error = error;
            this.FilePath = type.Assembly.Location;
        }

        /// <summary>
        /// Gets the file path of the extension library.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets the original type of the extension.
        /// </summary>
        public Type ExtensionType { get; private set; }

        /// <summary>
        /// Gets the instance of an extension which is being created if succesfully loaded.
        /// </summary>
        public LiteExtension Extension { get; private set; }

        /// <summary>
        /// Gets the exception object of the error that occured while loading the extension if available.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets a value indicating the extension has been loaded succesfully in LiteDevelop.
        /// </summary>
        public bool SuccesfullyLoaded { get { return Extension != null; } }
    }
}
