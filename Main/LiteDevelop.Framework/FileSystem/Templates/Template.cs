using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace LiteDevelop.Framework.FileSystem.Templates
{
    /// <summary>
    /// Represents a template which can be used by users creating a new file or project in LiteDevelop.
    /// </summary>
    public sealed class Template : TemplateNode
    {
        private List<TemplateNode> _cachedChildren = new List<TemplateNode>();

        internal Template(XmlNode node)
            : base(node)
        {
        }

        public static Template FromFile(FilePath filePath)
        {
            var document = new XmlDocument();
            document.Load(filePath.FullPath);
            var templateNode = document.FirstChild.NextSibling;

            if (templateNode == null || templateNode.Name != "Template")
                throw new FormatException("File does not start with a Template tag.");

            return new Template(templateNode);
        }

        /// <summary>
        /// Gets the name of the template.
        /// </summary>
        public string Name
        {
            get { return GetAttribute("Name"); }
        }

        /// <summary>
        /// Gets the author of the template.
        /// </summary>
        public string Author
        {
            get { return GetAttribute("Author"); }
        }

        /// <summary>
        /// Gets the version of the template.
        /// </summary>
        public Version Version
        {
            get { return Version.Parse(GetAttribute("Version")); }
        }

        /// <summary>
        /// Gets the category name this template belongs to.
        /// </summary>
        public string Category
        {
            get { return GetAttribute("Category"); }
        }

        /// <summary>
        /// Gets the template type.
        /// </summary>
        public TemplateType Type
        {
            get { return (TemplateType)Enum.Parse(typeof(TemplateType), XmlNode.Attributes["Type"].Value); }
        }

        /// <summary>
        /// Gets the icon being used in the template list.
        /// </summary>
        public string IconFile
        {
            get { return GetAttribute("Icon"); }
        }

        public IEnumerable<TemplateNode> GetProjectNodes()
        {
            if (Type == TemplateType.File)
                throw new ArgumentException("Cannot get project nodes from a file template.");

            if (_cachedChildren.Count == 0)
            {
                foreach (XmlNode node in XmlNode.ChildNodes)
                {
                    var item = new ProjectTemplateNode(node);
                    _cachedChildren.Add(item);
                }
            }

            return _cachedChildren;
        }

        public IEnumerable<TemplateNode> GetFileNodes()
        {
            if (Type == TemplateType.Project)
                throw new ArgumentException("Cannot get file nodes from a project template.");

            if (_cachedChildren.Count == 0)
            {
                foreach (XmlNode node in XmlNode.ChildNodes)
                {
                    var item = new FileTemplateNode(node);
                    _cachedChildren.Add(item);
                }
            }

            return _cachedChildren;
        }

        public override IEnumerable<ISavableFile> Create(FileCreationContext context)
        {
            var nodes = Type == TemplateType.Project ? GetProjectNodes() : GetFileNodes();

            foreach (var node in nodes)
            {
                foreach (var file in node.Create(context))
                    yield return file;
            }
        }


    }

    public enum TemplateType
    {
        File,
        Project,
    }
}
