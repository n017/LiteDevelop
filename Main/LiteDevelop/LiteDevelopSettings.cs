using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using LiteDevelop.Framework;
using LiteDevelop.Extensions;
using LiteDevelop.Framework.FileSystem.Projects;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop
{
    internal class LiteDevelopSettings : SettingsMap
    {
        public class LiteDevelopSettingsEvaluator : DictionaryStringEvaluator
        {
            private static Dictionary<string, string> _argumentMapping = new Dictionary<string, string>()
            {
                {"Documents", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)},
                {"FrameworkInstallationDirectory", Path.GetDirectoryName(typeof(object).Assembly.Location)},
            };

            public LiteDevelopSettingsEvaluator()
                : base(_argumentMapping)
            {

            }
        }

        public static LiteDevelopSettings Instance { get; set; }
        public static LiteDevelopSettings Default { get; private set; }

        private static readonly string _settingsPath = Path.Combine(Constants.AppDataDirectory, "settings.xml");
        private static readonly IStringEvaluator _evaluator = new LiteDevelopSettingsEvaluator();

        static LiteDevelopSettings()
        {
            Default = new LiteDevelopSettings(new FilePath(Application.StartupPath, "default_settings.xml"));

            if (File.Exists(_settingsPath))
                Instance = new LiteDevelopSettings(new FilePath(_settingsPath));
            else
                Reset();
        }
        
        private LiteDevelopSettings()
        {
            FallbackMap = Default;
        }

        public LiteDevelopSettings(FilePath path)
            : base(path)
        {
            FallbackMap = Default;
        }

        public override IStringEvaluator Evaluator
        {
            get { return _evaluator; }
        }

        public void Save()
        {
            this.Save(new FilePath(_settingsPath));
        }
        
        public static void Reset()
        {
            LiteDevelopApplication.Current.EnsureAppDataDirectoryIsCreated();
            LiteDevelopSettings.Default.CopyTo(Instance = new LiteDevelopSettings());
            Instance.Save();
        }

    }
}
