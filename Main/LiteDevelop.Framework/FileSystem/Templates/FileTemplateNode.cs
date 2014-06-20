using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Framework.FileSystem.Templates
{
    public sealed class FileTemplateNode : TemplateNode 
    {
        private readonly Dictionary<string, XmlNode> _children = new Dictionary<string, XmlNode>();
        private bool _childNodesRead = false;

        internal FileTemplateNode(XmlNode node)
            : base(node)
        {

        }

        private XmlNode ContentsNode
        {
            get
            {
                EnsureChildNodesRead();
                return _children["Contents"];
            }
        }

        public string UnevaluatedName
        {
            get { return GetAttribute("Name"); }
        }

        public string UnevaluatedContents
        {
            get { return ContentsNode.InnerText; }
        }

        public string DependentUpon
        {
            get { return GetAttribute("DependentUpon"); }
        }

        public ContentsType ContentsType
        {
            get { return (ContentsType)Enum.Parse(typeof(ContentsType), ContentsNode.Attributes["Type"].Value); }
        }

        public override IEnumerable<ISavableFile> Create(FileCreationContext context)
        {
            byte[] data = null;
            var arguments = context.GetEvaluatorArguments();

            if (ContentsType == Templates.ContentsType.Text)
                data = Encoding.UTF8.GetBytes(StringEvaluator.EvaluateString(UnevaluatedContents, arguments));
            else
                data = Convert.FromBase64String(UnevaluatedContents);

            var file = context.FileService.CreateFile(context.FilePath.ParentDirectory.Combine(StringEvaluator.EvaluateString(UnevaluatedName, arguments)), data);

            if (!string.IsNullOrEmpty(DependentUpon))
                file.Dependencies.Add(StringEvaluator.EvaluateString(DependentUpon, arguments));

            if (context.CurrentProject != null)
            {
                var entry = new ProjectFileEntry(file.FilePath);
                entry.Dependencies.AddRange(file.Dependencies);

                context.CurrentProject.ProjectFiles.Add(entry);
            }
            
            return new ISavableFile[] { file };
        }

        private void EnsureChildNodesRead()
        {
            if (!_childNodesRead)
            {
                foreach (XmlNode node in XmlNode.ChildNodes)
                    _children.Add(node.Name, node);
                _childNodesRead = true;
            }
        }
    }

    public enum ContentsType
    {
        Text,
        Base64,
    }
}
