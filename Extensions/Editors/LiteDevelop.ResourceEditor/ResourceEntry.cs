using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiteDevelop.ResourceEditor
{
    public class ResourceEntry
    {
        public event EventHandler NameChanged;
        public event EventHandler ValueChanged;
        private string _name;
        private object _value;

        public ResourceEntry(string name, object value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnNameChanged(EventArgs.Empty);
                }
            }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnValueChanged(EventArgs.Empty);
                }
            }
        }

        public void AddToWriter(IResourceWriter writer)
        {
            writer.AddResource(Name, Value);
        }

        public override string ToString()
        {
            if (Value == null)
                return "null";

            switch (Value.GetType().FullName)
            {
                case "System.Byte[]":
                    return string.Format("Byte array (Length = {0})", ((byte[])Value).Length);
                case "System.Drawing.Bitmap":
                    var bitmap = (Bitmap)Value;
                    return string.Format("Bitmap (Width = {0}, Height = {1})", bitmap.Width, bitmap.Height);
                case "System.Drawing.Icon":
                    var icon = (Icon)Value;
                    return string.Format("Icon (Width = {0}, Height = {1})", icon.Width, icon.Height);
                case "System.Drawing.Cursor":
                    var cursor = (Cursor)Value;
                    return string.Format("Icon (Width = {0}, Height = {1})", cursor.Size.Width, cursor.Size.Height);
                default:
                    return Value.ToString();
            }
        }

        protected virtual void OnNameChanged(EventArgs e)
        {
            if (NameChanged != null)
                NameChanged(this, e);
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }
    }
}
