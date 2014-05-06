using System;
using System.Linq;
using LiteDevelop.Framework;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Extensions
{
    internal sealed class ErrorManager : IErrorManager
    {
        private EventBasedCollection<BuildError> _errors = new EventBasedCollection<BuildError>();


        #region IErrorManager Members

        public EventBasedCollection<BuildError> Errors
        {
            get { return _errors; }
        }

        #endregion
    }
}
