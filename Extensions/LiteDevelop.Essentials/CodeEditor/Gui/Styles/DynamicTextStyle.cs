using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FastColoredTextBoxNS;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Essentials.CodeEditor.Gui.Styles
{
    public class DynamicTextStyle : TextStyle 
    {
        private static SolidBrush CreateBrush(Color color)
        {
            return color.ToArgb() != 0 ? new SolidBrush(color) : null;
        }

        public DynamicTextStyle(AppearanceDescription description)
            : base(CreateBrush(description.ForeColor), CreateBrush(description.BackColor), description.FontStyle)
        {
            Description = description;
            description.ForeColorChanged += description_ForeColorChanged;
            description.BackColorChanged += description_BackColorChanged;
            description.FontStyleChanged += description_FontStyleChanged;
        }

        private void description_ForeColorChanged(object sender, EventArgs e)
        {
            if (ForeBrush != null)
                ForeBrush.Dispose();
            ForeBrush = CreateBrush(Description.ForeColor);
        }

        private void description_BackColorChanged(object sender, EventArgs e)
        {
            if (BackgroundBrush != null)
                BackgroundBrush.Dispose();
            BackgroundBrush = CreateBrush(Description.BackColor);
        }

        private void description_FontStyleChanged(object sender, EventArgs e)
        {
            FontStyle = Description.FontStyle;
        }

        public AppearanceDescription Description
        {
            get;
            set;
        }
        
    }
}
