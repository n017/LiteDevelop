using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Framework.FileSystem
{
    public class AssemblyReference 
    {
        public AssemblyReference(string assemblyName)
        {
            AssemblyName = assemblyName;
        }

        public AssemblyReference(string assemblyName, bool specificVersion, string hintPath)
        {
            AssemblyName = assemblyName;
            SpecificVersion = specificVersion;
            HintPath = hintPath;
        }

        public string AssemblyName { get; set; }

        public bool SpecificVersion { get; set; }
        public string HintPath { get; set; }

        public override string ToString()
        {
            return AssemblyName;
        }
    }
}
