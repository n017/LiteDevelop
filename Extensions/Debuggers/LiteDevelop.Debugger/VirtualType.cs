using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a virtual or fake type.
    /// </summary>
    public class VirtualType : IType 
    {
        public VirtualType()
        {

        }

        #region IType Members

        public IType BaseType
        {
            get { return null; }
        }

        #endregion

    }
}
