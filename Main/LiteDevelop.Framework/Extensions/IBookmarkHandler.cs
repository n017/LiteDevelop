using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Framework.Extensions
{
    public interface IBookmarkHandler
    {
        bool CanSetBookmark
        {
            get;
        }

        bool CanRemoveBookmark
        {
            get;
        }

        void SetBookmark();

        void RemoveBookmark();
    }
}
