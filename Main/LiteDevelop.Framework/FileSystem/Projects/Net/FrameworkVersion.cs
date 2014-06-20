using System;
using System.Linq;

namespace LiteDevelop.Framework.FileSystem.Projects.Net
{
    /// <summary>
    /// Represents a version of the .NET Framework.
    /// </summary>
    public sealed class FrameworkVersion
    {
        public FrameworkVersion(Version version, int? servicePack, FrameworkInstallationType installationType)
        {
            Version = version;
            ServicePack = servicePack;
            InstallationType = installationType;
        }

        /// <summary>
        /// Gets the version of the .NET Framework.
        /// </summary>
        public Version Version
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets (if available) the service pack of this version of the .NET Framework.
        /// </summary>
        public int? ServicePack
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the installation type of this version of the .NET Framework.
        /// </summary>
        public FrameworkInstallationType InstallationType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the display name of this version of the .NET Framework.
        /// </summary>
        public string DisplayName
        {
            get
            {
                string installationType = InstallationType == FrameworkInstallationType.ClientProfile ? " Client Profile" : string.Empty;
                return string.Format(".NET Framework {0}{1}", Version, installationType);
            }
        }

        /// <summary>
        /// Gets the display version number of this version of the .NET Framework.
        /// </summary>
        public string DisplayVersion
        {
            get
            {
                return string.Format("v{0}.{1}{2}", Version.Major, Version.Minor, (Version.Build == -1 ? "" : "." + Version.Build));
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is FrameworkVersion)
                return Equals(obj as FrameworkVersion);
            return false;
        }

        public bool Equals(FrameworkVersion version)
        {
            return this.Version == version.Version && ServicePack == version.ServicePack && InstallationType == version.InstallationType;
        }

        public static bool operator ==(FrameworkVersion a, FrameworkVersion b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FrameworkVersion a, FrameworkVersion b)
        {
            return !a.Equals(b);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Version.GetHashCode() ^ InstallationType.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
