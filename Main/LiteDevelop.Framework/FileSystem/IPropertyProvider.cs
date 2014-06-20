using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Framework.FileSystem
{
    public interface IPropertyProvider
    {
        string GetProperty(string name);
        void SetProperty(string name, string unevaluatedValue);
    }
}
