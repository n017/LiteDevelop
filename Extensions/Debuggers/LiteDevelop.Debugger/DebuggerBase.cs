using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Debugger.Gui;
using LiteDevelop.Debugger.Net.Gui;
using LiteDevelop.Framework;
using LiteDevelop.Framework.Debugging;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.Gui;
using LiteDevelop.Framework.Mui;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Provides the debugger base of all debugger extensions. This extension must be loaded before any other extension of type <see cref="DebuggerExtension"/> is loaded.
    /// </summary>
    public sealed class DebuggerBase : LiteExtension, ISettingsProvider
    {
        /// <summary>
        /// The instance of the debugger base.
        /// </summary>
        public static DebuggerBase Instance
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets whether the debugger base is initialized or not.
        /// </summary>
        public static bool HasInstance
        {
            get { return Instance != null; }
        }

        /// <summary>
        /// Ensures that the debugger base extension is loaded into LiteDevelop. 
        /// </summary>
        /// <param name="manager">The extension manager to use for loading the base if needed.</param>
        public static void EnsureBaseIsLoaded(IExtensionManager manager)
        {
            if (manager.GetLoadedExtension<DebuggerBase>() == null)
            {
                var result = manager.LoadExtension(typeof(DebuggerBase));
                if (!result.SuccesfullyLoaded)
                    throw result.Error;
            }
        }

        public event ControllerPauseEventHandler Paused;
        public event EventHandler CurrentFrameChanged;
        public event EventHandler Resumed;

        private readonly Dictionary<object, string> _componentMuiIdentifiers = new Dictionary<object, string>();
        private DebuggerToolWindow[] _toolWindows;
        private IFrame _currentFrame;

        public DebuggerBase()
        {
            if (HasInstance)
                throw new InvalidOperationException("Cannot create a second instance of LiteDebugger.");

            Instance = this;
        }

        #region LiteExtension Members

        /// <inheritdoc />
        public override string Author
        {
            get { return "Jerre S."; }
        }

        /// <inheritdoc />
        public override string Copyright
        {
            get { return "Copyright © Jerre S. 2014"; }
        }

        /// <inheritdoc />
        public override string Description
        {
            get { return "Provides the base of debugger extensions."; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return "Debugger Base"; }
        }

        /// <inheritdoc />
        public override Version Version
        {
            get { return typeof(DebuggerBase).Assembly.GetName().Version; }
        }

        /// <inheritdoc />
        public override string ReleaseInformation
        {
            get
            {
                return @"Main programmer: Jerre S.

Translations:
Dutch: Jerre S.";
            }
        }

        /// <inheritdoc />
        public override void Initialize(InitializationContext context)
        {
            if (IsInitialized)
                throw new InvalidOperationException("Cannot initialize the debugger a second time.");

            ExtensionHost = context.Host;
            ExtensionHost.ControlManager.ResolveToolWindow += ControlManager_ResolveToolWindow;

            try { Settings = DebuggerBaseSettings.LoadSettings(ExtensionHost.SettingsManager); }
            catch { ResetSettings(); }

            MuiProcessor = new MuiProcessor(ExtensionHost,
                new FilePath(typeof(DebuggerBase).Assembly.Location).ParentDirectory.Combine("Mui").FullPath);

            InitializeToolWindows();

            if (context.InitializationTime == InitializationTime.Startup)
                ExtensionHost.Initialized += ExtensionHost_Initialized;
            else
                ExtensionHost_Initialized(ExtensionHost, EventArgs.Empty);

            IsInitialized = true;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();
            Instance = null;
        }

        #endregion
        
        #region ISettingsProvider Members

        public event EventHandler AppliedSettings;

        public void ApplySettings()
        {
            RootSettingsNode.ApplySettingsInAllNodes();
            Settings.SaveSettings(ExtensionHost.SettingsManager);

            if (AppliedSettings != null)
                AppliedSettings(this, EventArgs.Empty);
        }

        public void LoadUserDefinedPresets()
        {
            RootSettingsNode.LoadUserDefinedPresetsInAllNodes();
        }

        public void ResetSettings()
        {
            DebuggerBaseSettings.Default.CopyTo(Settings);
            Settings.SaveSettings(ExtensionHost.SettingsManager);
        }

        public SettingsNode RootSettingsNode
        {
            get;
            private set;
        }

        #endregion

        public DebuggerBaseSettings Settings
        {
            get;
            private set;
        }

        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the extension host used to load the components.
        /// </summary>
        public ILiteExtensionHost ExtensionHost
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the multilingual userinterface (MUI) processor used for appling language specific strings to components.
        /// </summary>
        public MuiProcessor MuiProcessor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the current active frame.
        /// </summary>
        public IFrame CurrentFrame
        {
            get { return _currentFrame; }
            set
            {
                if (_currentFrame != value)
                {
                    _currentFrame = value;
                    if (CurrentFrameChanged != null)
                        CurrentFrameChanged(null, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets a tool window by its type.
        /// </summary>
        /// <typeparam name="TToolWindow">The type of the tool window.</typeparam>
        /// <returns>An instance of <typeparamref name="TToolWindow"/>, or null if none exists.</returns>
        public TToolWindow GetToolWindow<TToolWindow>() where TToolWindow:LiteToolWindow
        {
            return _toolWindows.FirstOrDefault(x => x is TToolWindow) as TToolWindow;
        }

        /// <summary>
        /// Notifies all components the debugger has paused.
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        public void DispatchPaused(ControllerPauseEventArgs e)
        {

            if (Paused != null)
                Paused(null, e);
            CurrentFrame = e.Thread.CurrentFrame;

            if (e.Reason == PauseReason.Exception)
            {
                using (var dialog = new ExceptionDialog(e.Thread.CurrentException, e.Thread.GetCurrentSourceRange()))
                {
                    dialog.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Notifies all components the debugger has resumed.
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        public void DispatchResumed(EventArgs e)
        {
            CurrentFrame = null;
            if (Resumed != null)
                Resumed(null, e);
        }
        
        private void SetupGui()
        {
            SetupSettingsControls();

            AddToMuiIdentifiers(SetupMenu());

            ExtensionHost.UILanguageChanged += ExtensionHost_UILanguageChanged;
            ExtensionHost_UILanguageChanged(null, null);
        }
        
        private void SetupSettingsControls()
        {
            var generalSettingsControl = new GeneralSettingsControl(ExtensionHost, MuiProcessor, Settings);

            var rootNode = new SettingsNode("Debugging", generalSettingsControl);
            var generalSettingsNode = new SettingsNode("General", generalSettingsControl);

            _componentMuiIdentifiers.Add(rootNode, "DebuggerBase.Settings.Root");
            _componentMuiIdentifiers.Add(generalSettingsNode, "DebuggerBase.Settings.General");

            rootNode.Nodes.AddRange(new SettingsNode[]
                {
                    generalSettingsNode,
                });

            RootSettingsNode = rootNode;
        }

        private void InitializeToolWindows()
        {
            _toolWindows = new DebuggerToolWindow[]
            {
                new VariablesContent(),
                new ThreadsContent(),
                new CallStackContent(),
            };
        }

        private Dictionary<object, string> SetupMenu()
        {
            var iconProvider = IconProvider.GetProvider<AssemblyIconProvider>();

            var variablesItem = new ToolStripMenuItem("Variables", iconProvider.ImageList.Images[iconProvider.GetImageIndex(typeof(System.Reflection.FieldInfo))],
                (o, e) =>
                {
                    ExtensionHost.ControlManager.ShowAndActivate(GetToolWindow<VariablesContent>());
                });

            var threadsItem = new ToolStripMenuItem("Threads", Properties.Resources.threads,
                 (o, e) =>
                 {
                     ExtensionHost.ControlManager.ShowAndActivate(GetToolWindow<ThreadsContent>());
                 });    

            var callStackItem = new ToolStripMenuItem("Call stack", Properties.Resources.stack,
                 (o, e) =>
                 {
                     ExtensionHost.ControlManager.ShowAndActivate(GetToolWindow<CallStackContent>());
                 });

            ExtensionHost.ControlManager.ViewMenuItems.AddRange(new ToolStripItem[]
                {
                    new ToolStripSeparator(),
                    variablesItem, 
                    threadsItem,
                    callStackItem,
                });

            return new Dictionary<object, string>()
            {
                {variablesItem, "DebuggerBase.Variables"},
                {threadsItem, "DebuggerBase.Threads"},
                {callStackItem, "DebuggerBase.CallStack"},
            };
        }

        private void AddToMuiIdentifiers(Dictionary<object, string> dictionary)
        {
            foreach (var keyPair in dictionary)
                _componentMuiIdentifiers.Add(keyPair.Key, keyPair.Value);
        }

        private void ExtensionHost_Initialized(object sender, EventArgs e)
        {
            SetupGui();
        }

        private LiteToolWindow ControlManager_ResolveToolWindow(object sender, ResolveToolWindowEventArgs e)
        {
            return _toolWindows.FirstOrDefault(x => x.GetPersistName() == e.PersistName);
        }

        private void ExtensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            MuiProcessor.ApplyLanguageOnComponents(_componentMuiIdentifiers);
        }
    }
}
