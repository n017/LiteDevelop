using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LiteDevelop.Framework
{
    /// <summary>
    /// Represents an enumerator not yielding anything.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class EmptyEnumerator<T> : IEnumerator<T>, IEnumerator
    {
        /// <inheritdoc />
        public T Current
        {
            get { return default(T); }
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        object IEnumerator.Current
        {
            get { return default(T); }
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
        }
    }
}
