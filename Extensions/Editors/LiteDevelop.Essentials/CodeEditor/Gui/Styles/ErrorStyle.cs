using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Essentials.CodeEditor.Gui.Styles
{
    public class ErrorStyle : DynamicTextStyle
    {
        private Pen _pen;

        public ErrorStyle(AppearanceDescription description)
            : base(description)
        {
            _pen = Pens.Red;
        }


        public override void Draw(System.Drawing.Graphics gr, System.Drawing.Point position, FastColoredTextBoxNS.Range range)
        {
            base.Draw(gr, position, range);
            gr.DrawLine(_pen, position, new Point(position.X + (range.End.iChar * range.tb.CharWidth), position.Y));
        }
        
    }
}
