
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Keeps track of COM object instances and wrappers which can be released all in once.
    /// </summary>
    public class ComInstanceCollector : IDisposable
    {
        private Dictionary<object, object> _comObjects = new Dictionary<object, object>();

        /// <summary>
        /// Gets all COM object instances registered by this collector.
        /// </summary>
        public IEnumerable<object> ComObjects
        {
            get { return _comObjects.Keys; }
        }

        /// <summary>
        /// Gets all wrapper instances registered by this collector.
        /// </summary>
        public IEnumerable<object> Wrappers
        {
            get { return _comObjects.Values; }
        }

        /// <summary>
        /// Gets the wrapper associated by the specified COM object.
        /// </summary>
        /// <typeparam name="TWrapper">The type of the wrapper.</typeparam>
        /// <param name="comObject">The COM object to get the wrapper from.</param>
        /// <returns>An instance of <typeparamref name="TWrapper"/> representing a wrapper of the specified COM object, or <c>null</c> if either none is available or the wrapper is not of type <typeparamref name="TWrapper"/>.</returns>
        public TWrapper GetWrapper<TWrapper>(object comObject) where TWrapper:class
        {
            return GetWrapper(comObject) as TWrapper;
        }

        /// <summary>
        /// Gets the wrapper associated by the specified COM object.
        /// </summary>
        /// <param name="comObject">The COM object to get the wrapper from.</param>
        /// <returns>An instance representing a wrapper of the specified COM object, or <c>null</c> if none is available.</returns>
        public object GetWrapper(object comObject)
        {
            object wrapper;
            _comObjects.TryGetValue(comObject, out wrapper);
            return wrapper;
        }

        /// <summary>
        /// Sets a COM object, or registers the object if it hasn't been added yet, and associates it with the specified wrapper.
        /// </summary>
        /// <param name="comObject">The COM object to register.</param>
        /// <param name="wrapper">The wrapper instance to register.</param>
        public void SetWrapper(object comObject, object wrapper)
        {
            if (_comObjects.ContainsKey(comObject))
                _comObjects[comObject] = wrapper;
            else
                AddComObject(comObject, wrapper);
        }

        /// <summary>
        /// Registers a COM object.
        /// </summary>
        /// <param name="comObject">The COM object to register.</param>
        public void AddComObject(object comObject)
        {
            AddComObject(comObject, null);
        }

        /// <summary>
        /// Registers a COM object with the specified wrapper.
        /// </summary>
        /// <param name="comObject">The COM object to register.</param>
        /// <param name="wrapper">The wrapper instance to register.</param>
        public void AddComObject(object comObject, object wrapper)
        {
            if (!Marshal.IsComObject(comObject))
                throw new ArgumentException("Value must be a COM object.");

            if (!_comObjects.ContainsKey(comObject))
                _comObjects.Add(comObject, wrapper);
        }

        /// <summary>
        /// Releases all the COM objects.
        /// </summary>
        public void ReleaseAll()
        {
            int count = 0;
            foreach (var obj in ComObjects)
            {
                count++;
                while (Marshal.ReleaseComObject(obj) > 0) ;
            }

            _comObjects.Clear();
        }

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseAll();
        }

        #endregion
    }
}