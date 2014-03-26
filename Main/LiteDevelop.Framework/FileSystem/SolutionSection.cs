using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteDevelop.Framework.FileSystem
{
    /// <summary>
    /// Represents a section defined in a solution node.
    /// </summary>
    public class SolutionSection : EventBasedCollection<KeyValuePair<string,string>>
    {
        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets the type of the section, e.g. Project, Global.
        /// </summary>
        public string SectionType { get; set; }

        /// <summary>
        /// Gets the user defined type, e.g. preSolution, postSolution.
        /// </summary>
        public string Type { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("Name={0}, Type={1}, Entries={2}", Name, Type, Count);
        }
        
    }
}
