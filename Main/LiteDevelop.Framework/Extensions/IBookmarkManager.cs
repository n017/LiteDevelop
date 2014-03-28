using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Framework.Extensions
{
    /// <summary>
    /// Provides members for managing bookmarks registered in LiteDevelop.
    /// </summary>
    public interface IBookmarkManager
    {
        /// <summary>
        /// Gets a collection of bookmarks registered in LiteDevelop.
        /// </summary>
        EventBasedCollection<Bookmark> Bookmarks { get; }

        /// <summary>
        /// Gets a collection of bookmarks of a specific type.
        /// </summary>
        /// <typeparam name="TBookmark">The type of the bookmark.</typeparam>
        /// <returns>A colletion of bookmarks of type <typeparamref name="TBookmark"/></returns>
        IEnumerable<TBookmark> GetBookmarks<TBookmark>() where TBookmark : Bookmark;

        /// <summary>
        /// Gets a collection of bookmarks registered in a specific file.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns>A collection of bookmarks registered in file <paramref name="file"/></returns>
        IEnumerable<Bookmark> GetBookmarks(FilePath file);

        /// <summary>
        /// Gets a collection of bookmarks registered in a specific file.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <param name="line">The line to remove all bookmarks from.</param>
        /// <returns>A collection of bookmarks registered in file <paramref name="file"/> on line <paramref name="line"/>.</returns>
        IEnumerable<Bookmark> GetBookmarksOnLine(FilePath file, int line);

        /// <summary>
        /// Clears all bookmarks registered in a specific file.
        /// </summary>
        /// <param name="path">The file path of the file to remove the bookmarks from.</param>
        void ClearBookmarksFromFile(FilePath path);

        /// <summary>
        /// Clears all bookmarks registered on a particular line in a specific file.
        /// </summary>
        /// <param name="path">The file path of the file to remove the bookmarks from.</param>
        /// <param name="line">The line to remove all bookmarks from.</param>
        void ClearBookmarksOnLine(FilePath path, int line);
    }
}
