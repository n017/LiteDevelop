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
        public class FileCreationStringEvaluator : DictionaryStringEvaluator
        {
            public FileCreationStringEvaluator(FileCreationContext context)
            {
                // TODO: escape strings.
                var solutionName = context.CurrentSolution != null ? context.CurrentSolution.Name : null;
                var solutionDirectory = context.CurrentSolution != null ? context.CurrentSolution.FilePath.ParentDirectory.FullPath : null;
                var projectName = context.CurrentProject != null ? context.CurrentProject.Name : null;
                var projectDirectory = context.CurrentProject != null ? context.CurrentProject.FilePath.ParentDirectory.FullPath : null;

                var @namespace = FormatToNamespace(context.CurrentProject != null ? context.FilePath.ParentDirectory.GetRelativePath(context.CurrentProject) : context.FilePath.FileName);

                var namespaceProvider = context.CurrentProject as IRootNamespaceProvider;
                if (namespaceProvider != null)
                {
                    if (@namespace.Length > 0)
                        @namespace = namespaceProvider.RootNamespace + "." + @namespace;
                    else
                        @namespace = namespaceProvider.RootNamespace;
                }

                Arguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    {"FullPath", context.FilePath.FullPath},
                    {"Directory", context.FilePath.ParentDirectory.FullPath},
                    {"Namespace", @namespace},
                    {"FileName", context.FilePath.FileName},
                    {"EscapedFileName", context.FilePath.FileName},
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
        }

        public FileCreationContext()
        {
        }

        public Solution CurrentSolution { get; set; }
        public Project CurrentProject { get; set; }
        public FilePath FilePath { get; set; }
        public IFileService FileService { get; set; }
        public IProgressReporter ProgressReporter { get; set; }

        public IStringEvaluator GetStringEvaluator()
        {
            return new FileCreationStringEvaluator(this);
        }

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}
