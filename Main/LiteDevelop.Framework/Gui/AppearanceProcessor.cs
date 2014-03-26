using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace LiteDevelop.Framework.Gui
{
    /// <summary>
    /// Represents a processor that applies appearance descriptions to components.
    /// </summary>
    public class AppearanceProcessor
    {
        private AppearanceMap _appearanceMap;

        public AppearanceProcessor(AppearanceMap map)
        {
            _appearanceMap = map;
        }

        /// <summary>
        /// Applies a description onto an object.
        /// </summary>
        /// <param name="obj">The object to apply the description to.</param>
        /// <param name="definition">The definition of the description to apply.</param>
        public void ApplyAppearanceOnObject(object obj, DefaultAppearanceDefinition definition)
        {
            ApplyAppearanceOnObject(obj, definition.ToString());
        }

        /// <summary>
        /// Applies a description onto an object.
        /// </summary>
        /// <param name="obj">The object to apply the description to.</param>
        /// <param name="descriptionIdentifier">The identifier of the description to apply.</param>
        public void ApplyAppearanceOnObject(object obj, string descriptionIdentifier)
        {
            var description = _appearanceMap.GetDescriptionById(descriptionIdentifier);
            if (description != null)
            {
                SetPropertyValue(obj, "ForeColor", description.ForeColor);
                SetPropertyValue(obj, "BackColor", description.BackColor);
                SetPropertyValue(obj, "Font", new Font(GetPropertyValue<Font>(obj, "Font"), description.FontStyle));
            }
        }

        private static void SetPropertyValue(object instance, string name, object value)
        {
            instance.GetType().GetProperty(name).SetValue(instance, value, null);
        }

        private static T GetPropertyValue<T>(object instance, string name)
        {
            return (T)instance.GetType().GetProperty(name).GetValue(instance, null);
        }
    }
}
