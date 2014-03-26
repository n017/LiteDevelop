
using System;
using System.Linq;

namespace LiteDevelop.Framework.Languages
{
    /// <summary>
    /// Provides members for holding and updating a source code snapshot.
    /// </summary>
    public interface ISnapshotProvider
    {
        /// <summary>
        /// Occurs when the <see cref="CurrentSnapshot"/> property has been updated.
        /// </summary>
        event EventHandler SnapshotUpdated;

        /// <summary>
        /// Gets or sets the current snapshot of the source code that should be used to parse members.
        /// </summary>
        SourceSnapshot CurrentSnapshot
        {
            get;
            set;
        }

        /// <summary>
        /// Creates and updates the snapshot with the specific source code.
        /// </summary>
        /// <param name="source"></param>
        void UpdateSnapshot(string source);
    }
}
