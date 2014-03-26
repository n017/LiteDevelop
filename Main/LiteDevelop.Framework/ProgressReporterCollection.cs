using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteDevelop.Framework
{
    /// <summary>
    /// Represents a strongly typed collection of progress reporters which can be accessed all in once.
    /// </summary>
    public class ProgressReporterCollection<TProgressReporter> : EventBasedCollection<TProgressReporter>, IProgressReporter 
        where TProgressReporter: IProgressReporter
    {
        #region IProgressReporter Members

        /// <inheritdoc />
        public void Report(MessageSeverity severity, string message)
        {
            foreach (var item in this)
                item.Report(severity, message);
        }

        /// <inheritdoc />
        public int ProgressPercentage
        {
            get
            {
                return this[0].ProgressPercentage;
            }
            set
            {
                foreach (var item in this)
                    item.ProgressPercentage = value;
            }
        }

        /// <inheritdoc />
        public bool ProgressVisible
        {
            get
            {
                return this[0].ProgressVisible;
            }
            set
            {
                foreach (var item in this)
                    item.ProgressVisible = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a collection of progress reporters which can be accessed all in once.
    /// </summary>
    public class ProgressReporterCollection : ProgressReporterCollection<IProgressReporter>
    {

    }

    /// <summary>
    /// Provides extension methods related to the <see cref="LiteDevelop.Framework.IProgressReporter"/> interface.
    /// </summary>
    public static class ProgressReporterCollectionExtensions
    {
        /// <summary>
        /// Gets the first progress reporter holding the given identifier.
        /// </summary>
        /// <param name="reporters">The reporters to test the identifier on.</param>
        /// <param name="id">The identifier of the reporter to look up.</param>
        /// <returns>An instance of <see cref="LiteDevelop.Framework.INamedProgressReporter"/> holding an identifier of value <paramref name="id"/></returns>
        public static INamedProgressReporter GetReporterById(this IEnumerable<INamedProgressReporter> reporters, string id)
        {
            return reporters.FirstOrDefault(x => x.Identifier == id);
        }
    }

    
}
