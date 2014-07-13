using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LiteDevelop.Framework.FileSystem.Projects;
using LiteDevelop.Framework.Languages;

namespace LiteDevelop.Framework.FileSystem.Templates
{
    public sealed class ProjectTemplateNode : TemplateNode
    {
        private readonly Dictionary<string, XmlNode> _children = new Dictionary<string, XmlNode>();
        private bool _childNodesRead = false;

        internal ProjectTemplateNode(XmlNode node)
            : base(node)
        {

        }

        private XmlNode ReferencesNode
        {
            get { return GetNode("References"); }
        }

        private XmlNode PropertiesNode
        {
            get { return GetNode("Properties"); }
        }

        private XmlNode FilesNode
        {
            get { return GetNode("Files"); }
        }

        public string UnevaluatedName
        {
            get { return GetAttribute("Name"); }
        }

        public string LanguageName
        {
            get { return GetAttribute("Language"); }
        }

        public override IEnumerable<ISavableFile> Create(FileCreationContext context)
        {
            // TODO: better detection of project descriptor...
            var descriptor = LanguageDescriptor.GetLanguageByName(LanguageName).GetProjectDescriptors().FirstOrDefault();
            var evaluator = context.GetStringEvaluator();

            var project = descriptor.CreateProject(evaluator.EvaluateString(Path.GetFileNameWithoutExtension(UnevaluatedName)));
            project.FilePath = context.FilePath.Combine(evaluator.EvaluateString(UnevaluatedName));
            context.CurrentProject = project;

            var referenceProvider = project as IAssemblyReferenceProvider;
            var propertyProvider = project as IPropertyProvider;

            if (referenceProvider != null && ReferencesNode != null)
            {
                foreach (XmlNode child in ReferencesNode.ChildNodes)
                    referenceProvider.References.Add(new AssemblyReference(evaluator.EvaluateString(child.InnerText)));
            }

            if (propertyProvider != null && PropertiesNode != null)
            {
                foreach (XmlNode child in PropertiesNode.ChildNodes)
                    propertyProvider.SetProperty(
                        evaluator.EvaluateString(child.Attributes["Name"].Value),
                        evaluator.EvaluateString(child.InnerText));
            }

            if (FilesNode != null)
            {
                foreach (XmlNode child in FilesNode.ChildNodes)
                {
                    var newContext = context.Clone() as FileCreationContext;
                    newContext.FilePath = project.FilePath;
                    new FileTemplateNode(child).Create(newContext);
                }
            }

            context.CurrentProject = null;

            return new ISavableFile[] { project };
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

        private XmlNode GetNode(string name)
        {
            EnsureChildNodesRead();
            XmlNode node;
            _children.TryGetValue(name, out node);
            return node;
        }
    }
}
