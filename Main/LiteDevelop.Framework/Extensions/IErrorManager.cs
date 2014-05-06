using System;
using System.Linq;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Framework.Extensions
{
    /// <summary>
    /// Provides methods for reporting and navigating to errors.
    /// </summary>
    public interface IErrorManager
    {
        EventBasedCollection<BuildError> Errors { get; }
    }
}
