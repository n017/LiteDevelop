using System;
using System.Linq;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Framework.Extensions
{
    /// <summary>
    /// Provides members for holding appearance mappings.
    /// </summary>
    public interface IAppearanceMapProvider
    {
        /// <summary>
        /// Gets the appearance map that is currently in use.
        /// </summary>
        AppearanceMap CurrentAppearanceMap { get; }

        /// <summary>
        /// Gets the appearance map that is used when either the current appearance map, or a key in the mapping could not be located.
        /// </summary>
        AppearanceMap DefaultAppearanceMap { get; }
    }
}
