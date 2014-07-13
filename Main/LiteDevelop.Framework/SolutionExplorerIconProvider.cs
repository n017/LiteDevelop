using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Framework
{
    /// <summary>
    /// Represents a storage of icons used in the solution explorer of LiteDevelop.
    /// </summary>
    public class SolutionExplorerIconProvider : IconProvider
    {
        public const int Index_Directory = 0;
        public const int Index_Solution = 1;
        public const int Index_Project = 2;
        public const int Index_ReferencesDirectory = 3;
        public const int Index_AssemblyRef = 4;
        public const int Index_Properties = 5;
        public const int Index_File = 6;

        private readonly ImageList _imageList;
        private readonly List<string> _cachedExtensions;
        private readonly int _startIndex;

        public SolutionExplorerIconProvider()
        {
            _imageList = new ImageList();
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _imageList.TransparentColor = Color.Green;

            _cachedExtensions = new List<string>();

            var iconTable = Properties.Resources.browserIcons;
            _imageList.Images.AddRange(new Image[]
                                       {
                                       		IconProvider.GetIconFromSpriteTable(iconTable, new Size(16, 16), AssemblyIconProvider.Index_Directory + 1),
                                       		Properties.Resources.solution,
                                       		Properties.Resources.project,
                                  			IconProvider.GetIconFromSpriteTable(iconTable, new Size(16, 16), AssemblyIconProvider.Index_ReferenceDirectory + 1),
	                                  		IconProvider.GetIconFromSpriteTable(iconTable, new Size(16, 16), AssemblyIconProvider.Index_AssemblyRef + 1),
	                                  		Properties.Resources.config,
	                                  		IconProvider.GetIconFromSpriteTable(iconTable, new Size(16, 16), AssemblyIconProvider.Index_File + 1),
                                       });
                   
            _startIndex = _imageList.Images.Count;
        }

        /// <inheritdoc />
        public override ImageList ImageList
        {
            get { return _imageList; }
        }

        /// <inheritdoc />
        public override int GetImageIndex(object member)
        {
            if (member is Solution)
                return Index_Solution;
            if (member is ProjectEntry || member is Project)
                return Index_Project;
            if (member is SolutionFolder)
                return Index_Directory;
            if (member is AssemblyReference)
                return Index_AssemblyRef;

            string filePath = member as string;
			var fileInfo = member as FileInfo;

            if (fileInfo != null)
                filePath = fileInfo.FullName;
            else if (member is IFilePathProvider)
                filePath = (member as IFilePathProvider).FilePath.FullPath;
            else if (member is DirectoryInfo || member is SolutionFolder)
                return Index_Directory;
            
            string extension = Path.GetExtension(filePath);
            int index = _cachedExtensions.IndexOf(extension);
            if (index == -1)
            {
                if (extension == ".exe" || extension == ".dll")
                    return Index_AssemblyRef;

                if (File.Exists(filePath) && !filePath.StartsWith("\\", StringComparison.Ordinal))
                {
                    _imageList.Images.Add(Icon.ExtractAssociatedIcon(filePath).ToBitmap());
                    index = _cachedExtensions.Count;
                    _cachedExtensions.Add(extension);
                }
                else
                {
                	return Index_File;
                }
            }
            return index + _startIndex;
        }
    }
}
