using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.Gui;
using LiteDevelop.Gui.DockContents;
using WeifenLuo.WinFormsUI.Docking;

namespace LiteDevelop
{
    public static class GlobalMethods
    {
        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, String pszSubAppName, String pszSubIdList);

        public static void ShowAndActivate(this DockContent content, DockPanel dockPanel)
        {
            content.Show(dockPanel);

            if (content.DockState == DockState.DockBottomAutoHide ||
                content.DockState == DockState.DockLeftAutoHide ||
                content.DockState == DockState.DockRightAutoHide ||
                content.DockState == DockState.DockTopAutoHide)
            {
                dockPanel.ActiveAutoHideContent = content;
            }
            
            content.Activate();
        }

        public static DockContent GetActiveDocument(this DockPanel dockPanel)
        {
            return GetContainer(dockPanel, x => x.DockHandler.IsActivated);
        }

        public static ViewContentContainer GetContainer(this DockPanel dockPanel, LiteViewContent liteContent)
        {
            return GetContainer(dockPanel, x => x.ViewContent == liteContent);
        }

        public static ViewContentContainer GetContainerByFile(this DockPanel dockPanel, OpenedFile file)
        {
            return GetContainerByFilePath(dockPanel, file.FilePath);
        }

        public static ViewContentContainer GetContainerByFilePath(this DockPanel dockPanel, string filePath)
        {
            return GetContainerByFilePath(dockPanel, filePath);
        }

        public static ViewContentContainer GetContainerByFilePath(this DockPanel dockPanel, FilePath filePath)
        {
            return GetContainer(dockPanel, x => 
                x.DocumentContent != null &&
                x.DocumentContent.AssociatedFile != null && 
                x.DocumentContent.AssociatedFile.FilePath == filePath);
        }

        public static ViewContentContainer GetContainer(this DockPanel dockPanel, Func<ViewContentContainer, bool> condition)
        {
            foreach (DockContent document in dockPanel.Contents)
            {
                var container = document as ViewContentContainer;
                if (container != null && condition(container))
                    return container;
            }
            return null;
        }

        public static T FindContent<T>(this DockPanel dockPanel) where T : DockContent
        {
        	foreach (var pane in dockPanel.Panes)
        		foreach (DockContent content in pane.Contents)
        			if (content is T)
        				return content as T;
        	return null;
        }

        public static void SetActiveDocument(this DockPanel dockPanel, LiteDocumentContent documentContent)
        {
            foreach (DockContent document in dockPanel.DocumentsToArray())
            {
                if (document is ViewContentContainer && 
                    (document as ViewContentContainer).DocumentContent == documentContent)
                {
                    document.Activate();
                    break;
                }
            }
        }
        
        public static T[] MergeWith<T>(this T[] array1, T[] array2)
        {
            if (array1 == null || array1.Length == 0)
                return array2;
            if (array2 == null || array2.Length == 0)
                return array1;

            T[] newArray = new T[array1.Length + array2.Length];
            Array.Copy(array1, 0, newArray, 0, array1.Length);
            Array.Copy(array2, 0, newArray, array1.Length, array2.Length);
            return newArray;
        }

        public static void Sort(this TreeNodeCollection collection)
        {
            var nodes = new TreeNode[collection.Count];
            collection.CopyTo(nodes, 0);
            nodes.Sort();
            collection.Clear();
            collection.AddRange(nodes);
        }

        public static void Sort(this TreeNode[] collection)
        {
            Array.Sort(collection, (x, y) => string.Compare(x.Text, y.Text, StringComparison.OrdinalIgnoreCase));
        }

        private static readonly Dictionary<LiteToolWindowDockState, DockState> _dockStates = new Dictionary<LiteToolWindowDockState, DockState>()
        {
            {LiteToolWindowDockState.Bottom, DockState.DockBottom},
            {LiteToolWindowDockState.BottomAutoHide, DockState.DockBottomAutoHide},
            {LiteToolWindowDockState.Left, DockState.DockLeft},
            {LiteToolWindowDockState.LeftAutoHide, DockState.DockLeft},
            {LiteToolWindowDockState.Right, DockState.DockRight},
            {LiteToolWindowDockState.RightAutoHide, DockState.DockRight},
            {LiteToolWindowDockState.Top, DockState.DockTop},
            {LiteToolWindowDockState.TopAutoHide, DockState.DockTop},
            {LiteToolWindowDockState.Float, DockState.Float},
            {LiteToolWindowDockState.Hidden, DockState.Hidden},
            {LiteToolWindowDockState.Unknown, DockState.Unknown},
            {LiteToolWindowDockState.Document, DockState.Document},
        };
        
        public static DockState ToDockPanelSuite(this LiteToolWindowDockState dockState)
        {
            return _dockStates[dockState];
        }

        public static LiteToolWindowDockState ToLiteDevelop(this DockState dockState)
        {
            return _dockStates.First(x => x.Value == dockState).Key;
        }


    }
}
