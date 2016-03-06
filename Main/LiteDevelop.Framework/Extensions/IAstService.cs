using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Framework.Extensions
{
    public interface IAstService
    {
        
        void UpdateSyntaxTree(OpenedFile file);
    }
}
