using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Framework.Extensions
{
    /// <summary>
    /// Provides members for holding an instance of a <see cref="LiteDevelop.Framework.Extensions.PropertyContainer"/>.
    /// </summary>
    public interface IPropertyContainerProvider
    {
        /// <summary>
        /// Gets the property container used for visualizing properties.
        /// </summary>
        PropertyContainer PropertyContainer { get; }
    }

    /// <summary>
    /// Represents a container for properties that can be visualized.
    /// </summary>
    public class PropertyContainer
    {
        /// <summary>
        /// Occurs when the <see cref="SelectedObjects"/> property has changed.
        /// </summary>
        public event EventHandler SelectedObjectsChanged;

        private object[] _selectedObjects;

        /// <summary>
        /// Gets or sets the selected objects to get the properties from.
        /// </summary>
        public object[] SelectedObjects
        {
            get { return _selectedObjects; }
            set
            {
                if (_selectedObjects !=value)
                {
                    _selectedObjects = value;
                    OnSelectedObjectsChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnSelectedObjectsChanged(EventArgs e)
        {
            if (SelectedObjectsChanged != null)
                SelectedObjectsChanged(this, e);
        }


    }
}
