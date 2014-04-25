using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiteDevelop.Debugger.Gui
{
    internal static class Extensions 
    {
        public static string Join(this ListViewItem.ListViewSubItemCollection subItems, string separator)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < subItems.Count; i ++)
            {
                builder.Append(subItems[i].Text + (i < subItems.Count - 1 ? separator : string.Empty));
            }
            return builder.ToString();
        }
    }
}
