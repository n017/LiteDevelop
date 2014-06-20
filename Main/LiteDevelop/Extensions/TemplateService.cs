using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Templates;

namespace LiteDevelop.Extensions
{
    public class TemplateService : ITemplateService
    {
        private static readonly Dictionary<string, Image> _cachedImages = new Dictionary<string, Image>();

        private readonly List<FilePath> _fileDirectories = new List<FilePath>();
        private readonly List<FilePath> _projectDirectories = new List<FilePath>();
        private readonly List<FilePath> _iconDirectories = new List<FilePath>();

        #region ITemplateService Members

        public void AddProjectSearchDirectory(FilePath filePath)
        {
            _projectDirectories.Add(filePath);
        }

        public void AddFileSearchDirectory(FilePath filePath)
        {
            _fileDirectories.Add(filePath);
        }

        public void AddIconSearchDirectory(FilePath filePath)
        {
            _iconDirectories.Add(filePath);
        }

        public IEnumerable<Template> GetFileTemplates()
        {
            return GetTemplates(_fileDirectories, TemplateType.File);
        }

        public IEnumerable<Template> GetProjectTemplates()
        {
            return GetTemplates(_projectDirectories, TemplateType.Project);
        }

        public Image GetIcon(string fileName)
        {
            Image image;

            if (!_cachedImages.TryGetValue(fileName, out image))
            {
                foreach (var directory in _iconDirectories)
                {
                    var file = Directory.GetFiles(directory.FullPath, fileName, SearchOption.AllDirectories).FirstOrDefault();

                    if (file != null)
                    {
                        image = Image.FromFile(file);
                        _cachedImages.Add(fileName, image);
                        break;
                    }
                }
            }

            return image;
        }

        #endregion

        private static IEnumerable<Template> GetTemplates(IEnumerable<FilePath> directories, TemplateType type)
        {
            foreach (var directory in directories)
            {
                foreach (var file in Directory.GetFiles(directory.FullPath, "*.template", SearchOption.AllDirectories))
                {
                    var template = Template.FromFile(new FilePath(file));
                    if (template.Type == type)
                        yield return template;
                }
            }
        }
    }
}
