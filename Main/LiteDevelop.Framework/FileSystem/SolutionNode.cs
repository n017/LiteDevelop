using System;
using System.Linq;
using System.Threading;

namespace LiteDevelop.Framework.FileSystem
{
    /// <summary>
    /// Represents a node in a solution file.
    /// </summary>
    public abstract class SolutionNode : IFilePathProvider
    {
        public event SolutionNodeLoadEventHandler LoadComplete;
        public event PathChangedEventHandler FilePathChanged;

        private FilePath _filePath;

        public SolutionNode()
        {
            Sections = new EventBasedCollection<SolutionSection>();
        }

        /// <summary>
        /// Gets the solution folder holding this solution node.
        /// </summary>
        public SolutionFolder Parent { get; internal set; }

        /// <summary>
        /// Gets the unique identifier indicating the type of the node.
        /// </summary>
        public Guid TypeGuid { get; internal set; }

        /// <summary>
        /// Gets the name of the node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file path of the node.
        /// </summary>
        public FilePath FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    var old = _filePath;
                    _filePath = value;
                    OnFilePathChanged(new PathChangedEventArgs(old, _filePath));
                }
            }
        }

        /// <summary>
        /// Gets the unique identifier of this node.
        /// </summary>
        public Guid ObjectGuid { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the contents of this node is loaded.
        /// </summary>
        public abstract bool IsLoaded
        {
            get;
        }

        /// <summary>
        /// Gets a collection of sections defined in the solution file holding information about this node.
        /// </summary>
        public EventBasedCollection<SolutionSection> Sections { get; private set; }

        /// <summary>
        /// Begins loading the contents of this node asynchronously.
        /// </summary>
        /// <param name="reporter">The progress reporter to use for logging.</param>
        public void BeginLoad(IProgressReporter reporter)
        {
            new Thread(() => Load(reporter)).Start();
        }

        /// <summary>
        /// Loads the contents of this node.
        /// </summary>
        /// <param name="reporter">The progress reporter to use for logging.</param>
        public abstract void Load(IProgressReporter reporter);

        /// <summary>
        /// Saves the contents of this node.
        /// </summary>
        /// <param name="reporter">The progress reporter to use for logging.</param>
        public abstract void Save(IProgressReporter reporter);

        protected virtual void OnLoadComplete(SolutionNodeLoadEventArgs e)
        {
            if (LoadComplete != null)
                LoadComplete(this, e);
        }

        protected virtual void OnFilePathChanged(PathChangedEventArgs e)
        {
            if (FilePathChanged != null)
                FilePathChanged(this, e);
        }

        /// <summary>
        /// Gets the root container of this solution node.
        /// </summary>
        /// <returns></returns>
        public SolutionFolder GetRoot()
        {
            SolutionNode node = this;
            while (node.Parent != null)
                node = node.Parent;
            return node as SolutionFolder;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("TypeGuid={0}, Name={1}, HintPath={2}, ProjectGuid={3}", TypeGuid, Name, FilePath, ObjectGuid);
        }
    }
}
