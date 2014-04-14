using LiteDevelop.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LiteDevelop.Framework.FileSystem;
using System.Text;

namespace LiteDevelop.Framework.Mui
{
    /// <summary>
    /// Processor for the Multilingual User Interface (MUI) of LiteDevelop
    /// </summary>
    public class MuiProcessor
    {
        private readonly Dictionary<UILanguage, UILanguagePack> _cachedLanguagePacks = new Dictionary<UILanguage, UILanguagePack>();
        private readonly ILiteExtensionHost _extensionHost;
        private readonly string _muiDirectory;

        public MuiProcessor(ILiteExtensionHost extensionHost, string muiDirectory)
        {
            _extensionHost = extensionHost;
            _muiDirectory = muiDirectory;
        }

        /// <summary>
        /// Gets the default language pack.
        /// </summary>
        public UILanguagePack DefaultLanguagePack
        {
            get { return GetLanguagePack(UILanguage.Default); }
        }

        /// <summary>
        /// Gets the language pack that is currently in use.
        /// </summary>
        /// <returns>The language pack currently in use.</returns>
        public UILanguagePack GetCurrentLanguagePack()
        {
            try
            {
                var pack = GetLanguagePack(_extensionHost.UILanguage);
                return pack ?? DefaultLanguagePack;
            }
            catch (FileNotFoundException)
            {
                return DefaultLanguagePack;
            }
        }

        /// <summary>
        /// Gets the language pack by its language identifier.
        /// </summary>
        /// <param name="language">The language of the language pack to get.</param>
        /// <returns>A language pack holding the given identifier, or null if none can be found.</returns>
        public virtual UILanguagePack GetLanguagePack(UILanguage language)
        {
            UILanguagePack pack;
            if (!_cachedLanguagePacks.TryGetValue(language, out pack))
            {
                var path = GetLanguagePackFilePath(language);
                if (File.Exists(path.FullPath))
                {
                    var fallbackPack = language == UILanguage.Default ? null : DefaultLanguagePack;
                    pack = new UILanguagePack(path, fallbackPack);
                    _cachedLanguagePacks.Add(language, pack);
                }
            }
            return pack;
        }

        /// <summary>
        /// Gets the language pack file path by its language identifier.
        /// </summary>
        /// <param name="language">The language of the language pack to get.</param>
        /// <returns>A file path to the language pack.</returns>
        public virtual FilePath GetLanguagePackFilePath(UILanguage language)
        {
            return new FilePath(_muiDirectory).Combine(language.PackIdentifier + ".xml");
        }

        /// <summary>
        /// Gets a string in the current language by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the string.</param>
        /// <returns>A language specific string.</returns>
        public string GetString(string id)
        {
            return GetString(id, new Dictionary<string, string>());
        }

        /// <summary>
        /// Gets a string in the current language by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the string.</param>
        /// <param name="arguments">Additional parameters in format 'parameter=value'.</param>
        /// <returns>A formatted language specific string, with the parameters replaced to the argument values.</returns>
        public string GetString(string id, params string[] arguments)
        {
            var dictionary = new Dictionary<string, string>(arguments.Length);
            for (int i = 0; i < arguments.Length; i++)
            {
                var keyPair = arguments[i].Split('=');
                dictionary.Add(keyPair[0], keyPair[1]);
            }
            return GetString(id, dictionary);
        }

        /// <summary>
        /// Gets a string in the current language by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the string.</param>
        /// <param name="arguments">Additional parameters.</param>
        /// <returns>A formatted language specific string, with the parameters replaced to the argument values.</returns>
        public string GetString(string id, IDictionary<string, string> arguments)
        {
            string value;

            if (!GetCurrentLanguagePack().TryGetValue(id, arguments, out value) && !DefaultLanguagePack.TryGetValue(id, arguments, out value))
                throw new ArgumentException(string.Format("Could not find string value '{0}'.", id));

            return value;
        }
        
        /// <summary>
        /// Sets the Text or HeaderText property of objects to a language specific string using the corresponding identifiers.
        /// </summary>
        /// <param name="objects">The controls and their corresponding string identifiers to use.</param>
        public void ApplyLanguageOnComponents(IDictionary<object, string> objects)
        {
            ApplyLanguageOnComponents(GetCurrentLanguagePack(), objects);
        }

        /// <summary>
        /// Sets the Text or HeaderText property of objects to a language specific string using the corresponding identifiers.
        /// </summary>
        /// <param name="languagePack">The language pack to use.</param>
        /// <param name="objects">The controls and their corresponding string identifiers to use.</param>
        public static void ApplyLanguageOnComponents(UILanguagePack languagePack, IDictionary<object, string> objects)
        {
            foreach (var keyPair in objects)
                TryApplyLanguageOnComponent(keyPair.Key, languagePack, keyPair.Value);
        }

        /// <summary>
        /// Tries to set the Text or HeaderText property on an object by using the specified string identifier.
        /// </summary>
        /// <param name="component">The component to set the tex to.</param>
        /// <param name="id">The string identifier to use.</param>
        public bool TryApplyLanguageOnComponent(object component, string id)
        {
            return TryApplyLanguageOnComponent(component, GetCurrentLanguagePack(), id);
        }

        /// <summary>
        /// Tries to set the Text or HeaderText property on an object by using the specified string identifier.
        /// </summary>
        /// <param name="component">The component to set the tex to.</param>
        /// <param name="languagePack">The language pack to use.</param>
        /// <param name="id">The string identifier to use.</param>
        public static bool TryApplyLanguageOnComponent(object component, UILanguagePack languagePack, string id)
        {
            var property = FindTextProperty(component);
            if (property == null || !property.CanWrite)
                return false;

            string value;
            if ((languagePack.TryGetValue(id, out value)) ||
                (languagePack.FallbackMap != null && languagePack.FallbackMap.TryGetValue(id, out value)))
            {
                property.SetValue(component, CheckLength(component, value), null);
                return true;
            }

            return false;
        }

        private static PropertyInfo FindTextProperty(object component)
        {
            var componentType = component.GetType();
            var possibilities = new string[] { "Text", "HeaderText" };

            for (int i = 0; i < possibilities.Length; i++)
            {
                var property = componentType.GetProperty(possibilities[i]);
                if (property != null)
                    return property;
            }

            return null;
        }

        private static string CheckLength(object component, string value)
        {
            if (component.GetType() != typeof(System.Windows.Forms.Label))
                return value;

            System.Windows.Forms.Label cLabel = (System.Windows.Forms.Label)component;
            if (cLabel.Name == "commitSelectedItemWhenLabel")
                return value;

            if (value.Length > 30)
            {
                int index = value.LastIndexOf(" ");

                StringBuilder sb = new StringBuilder(value);
                sb[index] = '\n';
                return sb.ToString();
            }
            return value;
        }
    }
}
