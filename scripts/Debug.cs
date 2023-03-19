using Godot;
using System;


namespace SpaceThing
{
    internal static class Debug
    {
        internal static void Assert(bool cond, string msg)
#if DEBUG
        {
            if (cond) return;

            GD.PrintErr(msg);
            throw new ApplicationException($"Assert Failed: {msg}");
        }
#else
    {}
#endif
    }
}
