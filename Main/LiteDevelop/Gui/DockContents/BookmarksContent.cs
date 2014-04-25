using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Extensions;
using LiteDevelop.Framework;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.Gui;
using WeifenLuo.WinFormsUI.Docking;

namespace LiteDevelop.Gui.DockContents
{
    internal partial class BookmarksContent : DockContent
    {
        private readonly Dictionary<object, string> _componentMuiIdentifiers = new Dictionary<object, string>();
        private readonly Dictionary<Bookmark, ListViewItem> _bookmarkItems = new Dictionary<Bookmark, ListViewItem>();
        private readonly ImageList _imageList;
        private LiteExtensionHost _extensionHost;
        private int _currentBookmarkIndex;

        public BookmarksContent()
        {
            InitializeComponent();

            Icon = Icon.FromHandle(Properties.Resources.bookmarks.GetHicon());

            listView1.SmallImageList = _imageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(16, 16),
            };

            _componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {this, "BookmarkContent.Title"},
                {addBookmarkToolStripButton, "BookmarkContent.Toolbar.AddBookmark"},
                {removeBookmarkToolStripButton, "BookmarkContent.Toolbar.RemoveBookmark"},
                {previousToolStripButton, "BookmarkContent.Toolbar.Previous"},
                {nextToolStripButton, "BookmarkContent.Toolbar.Next"},
                {deleteAllToolStripButton, "BookmarkContent.Toolbar.DeleteAll"},
                {columnHeader1, "BookmarkContent.ListHeaders.Line"},
                {columnHeader3, "BookmarkContent.ListHeaders.File"},
            };

            LiteDevelopApplication.Current.InitializedApplication += Current_InitializedApplication;

            if (LiteDevelopApplication.Current.IsInitialized)
                Current_InitializedApplication(LiteDevelopApplication.Current, EventArgs.Empty);
            
        }

        private void Current_InitializedApplication(object sender, EventArgs e)
        {
            _extensionHost = LiteDevelopApplication.Current.ExtensionHost;
            _extensionHost.BookmarkManager.Bookmarks.InsertedItem += Bookmarks_InsertedItem;
            _extensionHost.BookmarkManager.Bookmarks.RemovedItem += Bookmarks_RemovedItem;

            _extensionHost.UILanguageChanged += _extensionHost_UILanguageChanged;
            _extensionHost_UILanguageChanged(_extensionHost, EventArgs.Empty);
        }

        private void _extensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            LiteDevelopApplication.Current.MuiProcessor.ApplyLanguageOnComponents(_componentMuiIdentifiers);
        }

        private void Bookmarks_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            var bookmark = e.TargetObject as Bookmark;

            var item = new ListViewItem(new string[] { 
                bookmark.Location.Line.ToString(), 
                // bookmark.Tooltip, 
                bookmark.Location.FilePath.ToString() })
            {
                ImageIndex = listView1.Items.Count
            };

            _imageList.Images.Add(bookmark.Image);
            listView1.Items.Add(item);

            bookmark.LocationChanged += bookmark_LocationChanged;
            bookmark.ImageChanged += bookmark_ImageChanged;

            _bookmarkItems.Add(bookmark, item);
        }

        private void bookmark_ImageChanged(object sender, EventArgs e)
        {
            var bookmark = sender as Bookmark;
            _imageList.Images[_bookmarkItems[bookmark].Index] = bookmark.Image;
        }

        private void bookmark_LocationChanged(object sender, EventArgs e)
        {
            var bookmark = sender as Bookmark;
            var listItem = _bookmarkItems[bookmark];
            listItem.SubItems[0].Text = bookmark.Location.Line.ToString();
            //listItem.SubItems[1].Text = bookmark.Tooltip;
            listItem.SubItems[1].Text = bookmark.Location.FilePath.FullPath;
        }

        private void Bookmarks_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            var bookmark = e.TargetObject as Bookmark;
            var listItem = _bookmarkItems[bookmark];

            for (int i = listItem.Index + 1; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].ImageIndex--;
            }

            _imageList.Images.RemoveAt(listItem.Index);
            listItem.Remove();
        }

        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            columnHeader3.Width = listView1.ClientRectangle.Width - columnHeader1.Width;
        }

        private void addBookmarkToolStripButton_Click(object sender, EventArgs e)
        {
            var bookmarkHandler = _extensionHost.ControlManager.SelectedDocumentContent as IBookmarkHandler;
            if (bookmarkHandler != null && bookmarkHandler.CanSetBookmark)
            {
                bookmarkHandler.SetBookmark();
            }
        }

        private void removeBookmarkToolStripButton_Click(object sender, EventArgs e)
        {
            var bookmarkHandler = _extensionHost.ControlManager.SelectedDocumentContent as IBookmarkHandler;
            if (bookmarkHandler != null && bookmarkHandler.CanRemoveBookmark)
            {
                bookmarkHandler.RemoveBookmark();
            }
        }

        private void previousToolStripButton_Click(object sender, EventArgs e)
        {
            _currentBookmarkIndex--;
            if (_currentBookmarkIndex < 0)
                _currentBookmarkIndex = listView1.Items.Count - 1; 
            ActivateBookmark();
        }

        private void nextToolStripButton_Click(object sender, EventArgs e)
        {
            _currentBookmarkIndex++;
            if (_currentBookmarkIndex >= listView1.Items.Count)
                _currentBookmarkIndex = 0;
            ActivateBookmark();
        }

        private void deleteAllToolStripButton_Click(object sender, EventArgs e)
        {
            _extensionHost.BookmarkManager.Bookmarks.Clear();
        }

        private void ActivateBookmark()
        {
            if (_currentBookmarkIndex <= listView1.Items.Count)
            {
                listView1.Items[_currentBookmarkIndex].Selected = true;
                listView1_ItemActivate(null, null);
            }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            var bookmark =  _bookmarkItems.First(x=> x.Value == listView1.SelectedItems[0]).Key;
            var fileHandlers = _extensionHost.ExtensionManager.GetFileHandlers(bookmark.Location.FilePath).Where(x => x is ISourceNavigator);
            var handler = _extensionHost.FileService.SelectFileHandler(fileHandlers, bookmark.Location.FilePath);
            var file = _extensionHost.FileService.OpenFile(bookmark.Location.FilePath);
            handler.OpenFile(file);

            ISourceNavigator navigator = file.CurrentDocumentContent as ISourceNavigator ??
                file.RegisteredDocumentContents.FirstOrDefault(x => x is ISourceNavigator) as ISourceNavigator ??
                handler as ISourceNavigator;

            if (navigator != null)
                navigator.NavigateToLocation(bookmark.Location);
        }



    }
}
