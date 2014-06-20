using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LiteDevelop.Framework.FileSystem.Templates
{
    public abstract class TemplateNode
    {
        internal TemplateNode(XmlNode node)
        {
            XmlNode = node;
        }

        protected XmlNode XmlNode
        {
            get;
            private set;
        }

        public abstract IEnumerable<ISavableFile> Create(FileCreationContext context);

        protected string GetAttribute(string name)
        {
            var attribute = XmlNode.Attributes[name];
            return attribute != null ? attribute.Value : null;
        }
    }
}
