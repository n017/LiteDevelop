using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Essentials.CodeEditor.Gui.Styles
{
    public class UnderliningStyle : DynamicTextStyle
    {
        private Pen _pen;

        public UnderliningStyle(AppearanceDescription description, Brush brush)
            : base(description)
        {
            BackgroundBrush.Dispose();
            BackgroundBrush = null;
            _pen = new Pen(brush, 1.5F);
        }

        public override void Draw(System.Drawing.Graphics gr, System.Drawing.Point position, FastColoredTextBoxNS.Range range)
        {
            base.Draw(gr, position, range);
            const int size = 2;
            var end = Math.Max(((range.End.iChar - range.Start.iChar) * range.tb.CharWidth) / size, size);

            position.Offset(0, range.tb.CharHeight - size);

            for (int i = 0; i < end; i++)
            {
                var start = position;
                position.Offset(size, size * (int)Math.Pow(-1, i));
                gr.DrawLine(_pen, start, position);
            }
        }
    }

    public class ErrorStyle : UnderliningStyle
    {
        public ErrorStyle(AppearanceDescription description)
            : base(description, Brushes.Red)
        {
        }
    }

    public class WarningStyle : UnderliningStyle
    {
        public WarningStyle(AppearanceDescription description)
            : base(description, Brushes.Green)
        {
        }
    }
}
