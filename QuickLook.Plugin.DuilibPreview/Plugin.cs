using QuickLook.Common.Plugin;
using System;

namespace QuickLook.Plugin.DuilibPreview;

public class Plugin : IViewer
{
    public int Priority => int.MinValue;

    public void Init()
    {
    }

    public bool CanHandle(string path)
    {
        return false;
    }

    public void Prepare(string path, ContextObject context)
    {
    }

    public void View(string path, ContextObject context)
    {
    }

    public void Cleanup()
    {
        GC.SuppressFinalize(this);
    }
}
