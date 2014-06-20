using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using LiteDevelop.Debugger.Net.Gui;
using LiteDevelop.Framework.Debugging;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Projects;
using LiteDevelop.Framework.FileSystem.Projects.Net;
using LiteDevelop.Framework.Gui;
using LiteDevelop.Framework.Mui;

namespace LiteDevelop.Debugger.Net
{
    public sealed class NetDebuggerExtension : DebuggerExtension
    {
        public static NetDebuggerExtension Instance { get; private set; }

        private NetDebuggerSession _session;

        public NetDebuggerExtension()
        {
            if (Instance != null)
                throw new InvalidOperationException("Cannot create a second instance of NetDebuggerExtension.");

            Instance = this;
        }

        #region LiteExtension Members

        public override string Author
        {
            get { return "Jerre S."; }
        }

        public override string Copyright
        {
            get { return "Copyright © Jerre S. 2014"; }
        }

        public override string Description
        {
            get { return "LiteDevelop .NET debugger"; }
        }

        public override string Name
        {
            get { return "Debugger .NET"; }
        }

        public override Version Version
        {
            get { return typeof(NetDebuggerExtension).Assembly.GetName().Version; }
        }

        public override string ReleaseInformation
        {
            get
            {
                return @"Main programmer: Jerre S.";
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            Instance = null;
        }

        #endregion

        #region DebuggerExtension Members
        
        protected override void InitializeCore(InitializationContext context)
        {
            ExtensionHost = context.Host;
        }

        public override bool CanDebugProject(Framework.FileSystem.Projects.Project project)
        {
            var netProject = project as NetProject;
            if (netProject != null)
            {
                return netProject.IsExecutable && netProject.TargetFramework.Version >= new Version(2, 0);
            }

            return false;
        }

        public override DebuggerSession CreateSession()
        {
            var session = CurrentSession = new NetDebuggerSession();
            if (ExtensionHost != null)
            {
                session.ProgressReporter = ExtensionHost.CreateOrGetReporter("Debug");

                foreach (var breakpoint in ExtensionHost.BookmarkManager.GetBookmarks<BreakpointBookmark>())
                {
                    CurrentSession.AddBreakpoint(breakpoint);
                }
            }
            return session;
        }

        #endregion

        public ILiteExtensionHost ExtensionHost
        {
            get;
            private set;
        }

        public NetDebuggerSession CurrentSession
        {
            get { return _session; }
            private set
            {
                if (_session != value)
                {
                    if (_session != null && ExtensionHost != null)
                    {
                        ExtensionHost.BookmarkManager.Bookmarks.InsertedItem -= Bookmarks_InsertedItem;
                        ExtensionHost.BookmarkManager.Bookmarks.RemovedItem -= Bookmarks_RemovedItem;
                    }
                    if ((_session = value) != null && ExtensionHost != null)
                    {
                        ExtensionHost.BookmarkManager.Bookmarks.InsertedItem += Bookmarks_InsertedItem;
                        ExtensionHost.BookmarkManager.Bookmarks.RemovedItem += Bookmarks_RemovedItem;
                    }

                }
            }
        }

        private void Bookmarks_InsertedItem(object sender, Framework.CollectionChangedEventArgs e)
        {
            var breakpoint = e.TargetObject as BreakpointBookmark;
            if (breakpoint != null)
            {
                CurrentSession.AddBreakpoint(breakpoint);
            }
        }

        private void Bookmarks_RemovedItem(object sender, Framework.CollectionChangedEventArgs e)
        {
            var breakpoint = e.TargetObject as BreakpointBookmark;
            if (breakpoint != null)
            {
                CurrentSession.RemoveBreakpoint(breakpoint);
            }
        }
    }
}
