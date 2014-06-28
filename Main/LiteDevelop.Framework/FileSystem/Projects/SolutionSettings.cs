using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace LiteDevelop.Framework.FileSystem.Projects
{
    /// <summary>
    /// Represents extra settings for storing user data for a specific solution.
    /// </summary>
	public sealed class SolutionSettings
	{
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(SolutionSettings));

        public event EventHandler Changed;
        private bool _hasChanged;

		public SolutionSettings()
		{
			StartupProjects = new EventBasedCollection<Guid>();
            OpenedFiles = new EventBasedCollection<SolutionOpenedFileInfo>();

            StartupProjects.InsertedItem += CollectionChanged;
            StartupProjects.RemovedItem += CollectionChanged;
            OpenedFiles.InsertedItem += CollectionChanged;
            OpenedFiles.RemovedItem += CollectionChanged;
		}

        [XmlIgnore]
        public bool HasChanged
        {
            get { return _hasChanged; }
            internal set
            {
                if (_hasChanged != value)
                {
                    _hasChanged = value;
                    if (value && Changed != null)
                        Changed(this, EventArgs.Empty);
                }
            }
        }

		public static SolutionSettings ReadSettings(string file)
		{
			using (var fileStream = File.OpenRead(file))
			{
				return serializer.Deserialize(fileStream) as SolutionSettings;
			}
		}

        public EventBasedCollection<Guid> StartupProjects
        {
            get;
            private set;
        }

        public EventBasedCollection<SolutionOpenedFileInfo> OpenedFiles
        {
            get;
            private set;
        }
        
		public void Save(string file)
		{
			using (var fileStream = File.Create(file))
			{
                serializer.Serialize(fileStream, this);
                HasChanged = false;
			}
		}

        private void CollectionChanged(object sender, CollectionChangedEventArgs e)
        {
            HasChanged = true;
        }

	}

    public struct SolutionOpenedFileInfo : IXmlSerializable
    {
        private string _relativePath, _extensionTypeName;

        public string RelativePath { get { return _relativePath; } }
        public string ExtensionTypeName { get { return _extensionTypeName; } }

        public SolutionOpenedFileInfo(string path, string extensionType)
        {
            _relativePath = path;
            _extensionTypeName = extensionType;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            _extensionTypeName = reader.GetAttribute("Extension");
            _relativePath = reader.ReadElementString();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Extension", ExtensionTypeName);
            writer.WriteString(RelativePath);
        }

        #endregion
    }
}