using LiteDevelop.Framework.Debugging;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Provides members for holding a <see cref="DebuggerSession"/> instance.
    /// </summary>
    public interface IDebuggerSessionProvider
    {
        DebuggerSession Session { get; }
    }
}
