using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Framework.FileSystem
{
    public class BuildException : Exception
    {
        public BuildException(string message, BuildResult result)
            : base(message)
        {
            Result = result;
        }

        public BuildResult Result { get; private set; }
    }
}
