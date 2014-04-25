using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LiteDevelop.Framework;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Debugger
{
    public class DebuggerBaseSettings : SettingsMap
    {      
        public static DebuggerBaseSettings Default { get; private set; }

        private static FilePath _defaultPath = new FilePath(typeof(DebuggerBaseSettings).Assembly.Location).ParentDirectory.Combine("default_settings.xml");

        public DebuggerBaseSettings(FilePath path)
            : base(path)
        {
            
        }

        public override string DocumentRoot
        {
            get
            {
                return "Settings";
            }
        }

        public static DebuggerBaseSettings LoadSettings(ISettingsManager manager)
        {
            string settingsFile = Path.Combine(manager.GetSettingsDirectory(DebuggerBase.Instance), "settings.xml");
            if (!File.Exists(settingsFile))
            {
                return new DebuggerBaseSettings(_defaultPath);
            }

            return new DebuggerBaseSettings(new FilePath(settingsFile));
        }

        public void SaveSettings(ISettingsManager manager)
        {
            string settingsFile = Path.Combine(manager.GetSettingsDirectory(DebuggerBase.Instance), "settings.xml");

            Save(new FilePath(settingsFile));
        }
    }
}
