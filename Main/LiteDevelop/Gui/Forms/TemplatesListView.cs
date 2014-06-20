using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem.Templates;

namespace LiteDevelop.Gui.Forms
{
    public class TemplatesListView : ListView
    {
        public TemplatesListView()
        {
            SmallImageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(24, 24),
            };

            LargeImageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(32, 32),
            };
        }

        public ITemplateService TemplateService
        {
            get;
            set;
        }

        public void Populate(IEnumerable<Template> templates)
        {
            Items.Clear();
            SmallImageList.Images.Clear();
            LargeImageList.Images.Clear();

            foreach (var template in templates)
            {
                int index = -1;
                if (!string.IsNullOrEmpty(template.IconFile))
                {
                    var image = TemplateService.GetIcon(template.IconFile);
                    if (image != null)
                    {
                        index = SmallImageList.Images.Count;
                        SmallImageList.Images.Add(image);
                        LargeImageList.Images.Add(image);
                    }
                }
                
                Items.Add(new ListViewItem(template.Name)
                {
                    Tag = template,
                    ImageIndex = index,
                });
            }
            
            if (Items.Count > 0)
               Items[0].Selected = true;
        }

        public Template SelectedTemplate
        {
            get { return SelectedItems[0].Tag as Template; }
        }
    }
}
