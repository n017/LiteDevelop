using System;
using System.Linq;
using System.Net;

namespace LiteDevelop.Framework.Extensions
{
    /// <summary>
    /// Provides members for requesting credentials.
    /// </summary>
    public interface ICredentialManager
    {
        /// <summary>
        /// Requests credentials by prompting the user with a special login dialog.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <param name="credential">The credentials the user has put in, if available.</param>
        /// <returns><c>True</c> if the user accepted the request and filled in the credentials, otherwise <c>False</c>.</returns>
        bool RequestCredential(CredentialRequest request, out NetworkCredential credential);
    }
}
