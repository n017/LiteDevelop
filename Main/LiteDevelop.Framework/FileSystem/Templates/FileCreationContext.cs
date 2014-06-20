using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Framework.FileSystem.Templates
{
    public sealed class FileCreationContext : ICloneable
    {
        public Solution CurrentSolution { get; set; }
        public Project CurrentProject { get; set; }
        public FilePath FilePath { get; set; }
        public IFileService FileService { get; set; }
        public IProgressReporter ProgressReporter { get; set; }

        public IDictionary<string, string> GetEvaluatorArguments()
        {
            // TODO: escape strings.
            var solutionName = CurrentSolution != null ? CurrentSolution.Name : null;
            var solutionDirectory = CurrentSolution != null ? CurrentSolution.FilePath.ParentDirectory.FullPath : null;
            var projectName = CurrentProject != null ? CurrentProject.Name : null;
            var projectDirectory = CurrentProject != null ? CurrentProject.FilePath.ParentDirectory.FullPath : null;

            var @namespace = FormatToNamespace(CurrentProject != null ? FilePath.ParentDirectory.GetRelativePath(CurrentProject) : FilePath.FileName);

            var namespaceProvider = CurrentProject as IRootNamespaceProvider;
            if (namespaceProvider != null)
            {
                if (@namespace.Length > 0)
                    @namespace = namespaceProvider.RootNamespace + "." + @namespace;
                else
                    @namespace = namespaceProvider.RootNamespace;
            }
        
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"FullPath", FilePath.FullPath},
                {"Directory", FilePath.ParentDirectory.FullPath},
                {"Namespace", @namespace},
                {"FileName", FilePath.FileName},
                {"EscapedFileName", FilePath.FileName},
                {"SolutionName", solutionName},
                {"EscapedSolutionName", solutionName},
                {"SolutionDirectory", solutionDirectory},
                {"EscapedSolutionDirectory", solutionDirectory},
                {"ProjectName", projectName},
                {"EscapedProjectName", projectName},
                {"ProjectDirectory", projectDirectory},
                {"EscapedProjectDirectory", projectDirectory},
            };
        }

        private static string FormatToNamespace(string input)
        {
            var builder = new StringBuilder(input);

            for (int i = 0; i < input.Length; i++)
            {
                char previous = (i == 0 ? '.' : builder[i - 1]);

                if ((previous == '.' && !char.IsLetter(builder[i])) ||          // after '.' can only come letters or _
                    (builder[i] != '.' && !char.IsLetterOrDigit(builder[i])))   // replace special characters with _
                {
                    builder[i] = '_';
                }
            }

            return builder.ToString();
        }

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}
