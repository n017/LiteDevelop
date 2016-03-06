﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using LiteDevelop.Framework;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Projects;
using LiteDevelop.Framework.Gui;
using LiteDevelop.Gui;
using LiteDevelop.Gui.DockContents;
using LiteDevelop.Gui.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace LiteDevelop.Extensions
{
    internal sealed class ControlManager : IControlManager
    {
        private EventBasedCollection<LiteDocumentContent> _documentContents;
        private EventBasedCollection<LiteToolWindow>_toolWindows;
        private EventBasedCollection<ToolStrip> _toolBars;
        private EventBasedCollection<ToolStripMenuItem> _menuItems;
        private EventBasedCollection<ToolStripItem> _editItems;
        private EventBasedCollection<ToolStripItem> _viewItems;
        private EventBasedCollection<ToolStripItem> _debugItems;
        private EventBasedCollection<ToolStripItem> _toolsItems;
        private EventBasedCollection<ToolStripItem> _statusStripItems;
        private EventBasedCollection<ToolStripItem> _solutionMenuItems;

        
        private readonly ToolStripAeroRenderer _renderer = new ToolStripAeroRenderer(ToolbarTheme.Toolbar);
        private readonly ILiteExtensionHost _extensionHost;
        private readonly SynchronizationContext _syncContext;

        public ControlManager(ILiteExtensionHost extensionHost, SynchronizationContext syncContext)
        {
            _extensionHost = extensionHost;
            _syncContext = syncContext;

            _documentContents = new EventBasedCollection<LiteDocumentContent>();
            _documentContents.InsertedItem += viewContent_InsertedItem;
            _documentContents.RemovedItem += viewContent_RemovedItem;

            _toolWindows = new EventBasedCollection<LiteToolWindow>();
            _toolWindows.InsertingItem += _toolWindows_InsertingItem;
            _toolWindows.InsertedItem += viewContent_InsertedItem;
            _toolWindows.RemovedItem += viewContent_RemovedItem;

            _toolBars = new EventBasedCollection<ToolStrip>();
            _toolBars.InsertedItem += toolBars_InsertedItem;
            _toolBars.RemovedItem += toolBars_RemovedItem;

            _menuItems = new EventBasedCollection<ToolStripMenuItem>();
            _menuItems.InsertedItem += menuItems_InsertedItem;
            _menuItems.RemovedItem += menuItems_RemovedItem;

            _editItems = new EventBasedCollection<ToolStripItem>();
            _editItems.InsertedItem += editItems_InsertedItem;
            _editItems.RemovedItem += editItems_RemovedItem;

            _viewItems = new EventBasedCollection<ToolStripItem>();
            _viewItems.InsertedItem += viewItems_InsertedItem;
            _viewItems.RemovedItem += viewItems_RemovedItem;

            _debugItems = new EventBasedCollection<ToolStripItem>();
            _debugItems.InsertedItem += debugItems_InsertedItem;
            _debugItems.RemovedItem += debugItems_RemovedItem;

            _toolsItems = new EventBasedCollection<ToolStripItem>();
            _toolsItems.InsertedItem += toolsItems_InsertedItem;
            _toolsItems.RemovedItem += toolsItems_RemovedItem;

            _statusStripItems = new EventBasedCollection<ToolStripItem>();
            _statusStripItems.InsertedItem += _statusStripItems_InsertedItem;
            _statusStripItems.RemovedItem += _statusStripItems_RemovedItem;

            _solutionMenuItems = new EventBasedCollection<ToolStripItem>();
            _solutionMenuItems.InsertedItem += solutionMenuItems_InsertedItem;
            _solutionMenuItems.RemovedItem += solutionMenuItems_RemovedItem;

            NotifyUnsavedFilesWhenClosing = true;
        }

        #region IControlManager Members

        public event ResolveToolWindowEventHandler ResolveToolWindow;

        public EventBasedCollection<LiteDocumentContent> OpenDocumentContents
        {
            get { return _documentContents; }
        }

        public LiteDocumentContent SelectedDocumentContent
        {
            get 
            {
                var selectedContent = DockPanel.GetActiveDocument() as ViewContentContainer;

                if (selectedContent != null)
                    return selectedContent.DocumentContent;

                return null;
            }
            set 
            {
                DockPanel.SetActiveDocument(value);
            }
        }

        public event EventHandler SelectedDocumentContentChanged;

        public EventBasedCollection<LiteToolWindow> ToolWindows
        {
            get { return _toolWindows; }
        }

        public void ShowAndActivate(LiteViewContent viewContent)
        {
            if (viewContent == null)
                throw new ArgumentNullException("viewContent");

            var dockContent = DockPanel.GetContainer(viewContent);

            if (dockContent == null)
            {
                if (viewContent is LiteToolWindow)
                    _toolWindows.Add(viewContent as LiteToolWindow);
                else
                    _documentContents.Add(viewContent as LiteDocumentContent);
            }
            else
                dockContent.ShowAndActivate(DockPanel);
        }
        
        public EventBasedCollection<ToolStrip> ToolBars
        {
            get { return _toolBars; }
        }

        public EventBasedCollection<ToolStripMenuItem> MenuItems
        {
            get { return _menuItems; }
        }

        public EventBasedCollection<ToolStripItem> EditMenuItems
        {
            get { return _editItems; }
        }

        public EventBasedCollection<ToolStripItem> ViewMenuItems
        {
            get { return _viewItems; }
        }

        public EventBasedCollection<ToolStripItem> DebugMenuItems
        {
            get { return _debugItems; }
        }

        public EventBasedCollection<ToolStripItem> ToolsMenuItems
        {
            get { return _toolsItems; }
        }

        public EventBasedCollection<ToolStripItem> StatusBarItems
        {
            get { return _statusStripItems; }
        }

        public EventBasedCollection<ToolStripItem> SolutionExplorerItems
        {
            get { return _solutionMenuItems; }
        }

        public AppearanceMap GlobalAppearanceMap
        {
            get { return _extensionHost.ExtensionManager.GetLoadedExtension<LiteDevelopExtension>().CurrentAppearanceMap; }
        }

        public ToolStripRenderer MenuRenderer
        {
            get { return _renderer; }
        }

        public event EventHandler AppearanceChanged;

        #endregion

        #region Components

        public DockPanel DockPanel
        {
            get;
            internal set;
        }

        public ToolStripPanel ToolStripPanel
        {
            get;
            internal set;
        }

        public MenuStrip MenuStrip
        {
            get;
            internal set;
        }

        public ToolStripMenuItem EditMenu
        {
            get;
            internal set;
        }

        public ToolStripMenuItem ViewMenu
        {
            get;
            internal set;
        }

        public ToolStripMenuItem DebugMenu
        {
            get;
            internal set;
        }

        public ToolStripMenuItem ToolsMenu
        {
            get;
            internal set;
        }

        public StatusStrip StatusStrip
        {
            get;
            internal set;
        }

        public ContextMenuStrip SolutionExplorerMenu
        {
            get;
            internal set;
        }

        #endregion

        public bool NotifyUnsavedFilesWhenClosing
        {
            get;
            set;
        }
        
        public void InvokeOnMainThread(Action action)
        {
            _syncContext.Post(new SendOrPostCallback((o) => action()), null);
        }

        internal void DispatchSelectedDocumentContentChanged(EventArgs e)
        {
            if (SelectedDocumentContentChanged != null)
                SelectedDocumentContentChanged(this, e);
        }

        internal void DispatchAppearanceChanged(EventArgs e)
        {
            if (AppearanceChanged != null)
                AppearanceChanged(this, e);
        }

        internal LiteViewContent DispatchResolveViewContent(ResolveToolWindowEventArgs e)
        {
            if (ResolveToolWindow != null)
            {
                foreach (var method in ResolveToolWindow.GetInvocationList())
                {
                    var result = method.DynamicInvoke(this, e) as LiteViewContent;
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
        
        internal ViewContentContainer AddContainer(LiteViewContent viewContent)
        {
            var container = new ViewContentContainer(viewContent);

            var dockState = DockState.Document;
            if (container.ToolWindow != null)
            {
                dockState = container.ToolWindow.DockState.ToDockPanelSuite();
                if (dockState == DockState.Hidden || dockState == DockState.Unknown)
                {
                    dockState = DockState.DockBottomAutoHide;
                }
            }

            container.Show(DockPanel, dockState);
            return container;
        }

        private void _toolWindows_InsertingItem(object sender, CollectionChangingEventArgs e)
        {
            e.Cancel = DockPanel.GetContainer(e.TargetObject as LiteViewContent) != null;
        }

        private void viewContent_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            AddContainer(e.TargetObject as LiteViewContent).ShowAndActivate(DockPanel);
        }

        private void viewContent_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            LiteViewContent viewContent = e.TargetObject as LiteViewContent;
            var dockContent = DockPanel.GetContainer(viewContent);
            dockContent.DockHandler.Close();
        }

        private void toolBars_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            var toolstrip = e.TargetObject as ToolStrip;
            var lastRow = ToolStripPanel.Rows[ToolStripPanel.Rows.Length - 1];

            Point nextControlLocation = lastRow.DisplayRectangle.Location;

            foreach (var toolbar in lastRow.Controls)
            {
                nextControlLocation.Offset(toolbar.Margin.Horizontal + toolbar.Padding.Horizontal + toolbar.Width, toolbar.Margin.Top);
            }

            ToolStripPanel.Join(toolstrip, nextControlLocation);
        }

        private void toolBars_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            ToolStripPanel.Controls.Remove(e.TargetObject as ToolStrip);
        }

        private void menuItems_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            // insert before 'Window' and 'Help' menu items.
            MenuStrip.Items.Insert(MenuStrip.Items.Count - 2, e.TargetObject as ToolStripMenuItem);
        }

        private void menuItems_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            MenuStrip.Items.Remove(e.TargetObject as ToolStripItem);
        }

        private void editItems_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            EditMenu.DropDownItems.Add(e.TargetObject as ToolStripItem);
        }

        private void editItems_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            EditMenu.DropDownItems.Remove(e.TargetObject as ToolStripItem);
        }

        private void viewItems_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            // insert before full screen menu item.
            ViewMenu.DropDownItems.Insert(ViewMenu.DropDownItems.Count - 2, e.TargetObject as ToolStripItem);
        }

        private void viewItems_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            ViewMenu.DropDownItems.Remove(e.TargetObject as ToolStripItem);
        }

        private void debugItems_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            DebugMenu.DropDownItems.Add(e.TargetObject as ToolStripItem);
        }

        private void debugItems_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            DebugMenu.DropDownItems.Remove(e.TargetObject as ToolStripItem);
        }

        private void toolsItems_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            ToolsMenu.DropDownItems.Add(e.TargetObject as ToolStripItem);
        }

        private void toolsItems_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            ToolsMenu.DropDownItems.Remove(e.TargetObject as ToolStripItem);
        }

        private void _statusStripItems_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            StatusStrip.Items.Add(e.TargetObject as ToolStripItem);
        }

        private void _statusStripItems_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            StatusStrip.Items.Remove(e.TargetObject as ToolStripItem);
        }

        private void solutionMenuItems_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            SolutionExplorerMenu.Items.Remove((ToolStripItem)e.TargetObject);
        }

        private void solutionMenuItems_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            SolutionExplorerMenu.Items.Add((ToolStripItem)e.TargetObject);
        }
    }
}
