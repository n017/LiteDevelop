using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LiteDevelop.Framework.FileSystem
{
    /// <summary>
    /// Represents a bookmark in LiteDevelop.
    /// </summary>
    public class Bookmark
    {
        public event EventHandler LocationChanged;
        public event EventHandler TooltipChanged;
        public event EventHandler ImageChanged;

        private SourceLocation _location;
        private string _toolTip;
        private System.Drawing.Image _image;

        public Bookmark(SourceLocation location)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            _location = location;
            Image = Properties.Resources.bookmark;
        }

        public SourceLocation Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnLocationChanged(EventArgs.Empty);
                }
            }
        }

        public string Tooltip
        {
            get{return _toolTip;}
            set
            {
                if (_toolTip != value)
                {
                    _toolTip = value;
                    OnTooltipChanged(EventArgs.Empty);
                }
            }
        }

        public Image Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnImageChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnLocationChanged(EventArgs e)
        {
            if (LocationChanged != null)
                LocationChanged(this, e);
        }

        protected virtual void OnTooltipChanged(EventArgs e)
        {
            if (TooltipChanged != null)
                TooltipChanged(this, e);
        }

        protected virtual void OnImageChanged(EventArgs e)
        {
            if (ImageChanged != null)
                ImageChanged(this, e);
        }
    }
}
