using QuickLook.Common.Plugin;
using System;
using System.IO;
using System.Windows;

namespace QuickLook.Plugin.DuilibPreview;

public class Plugin : IViewer
{
    public int Priority => 0;

    public void Init()
    {
    }

    public bool CanHandle(string path)
    {
        if (!File.Exists(path)) return false;
        
        string ext = Path.GetExtension(path);
        if (string.IsNullOrEmpty(ext) || !ext.Equals(".xml", StringComparison.OrdinalIgnoreCase))
             return false;

        try
        {
            char[] buffer = new char[2048];
            using (var sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                int len = sr.Read(buffer, 0, buffer.Length);
                string text = new string(buffer, 0, len);
                
                if (text.Contains("<Window") || text.Contains("<Global"))
                {
                    return true;
                }
            }
        }
        catch 
        {
            return false;
        }
        
        return false;
    }

    public void Prepare(string path, ContextObject context)
    {
        context.PreferredSize = new Size(1000, 720);
    }

    public void View(string path, ContextObject context)
    {
        context.ViewerContent = new DuilibPreviewControl(path);
        context.Title = Path.GetFileName(path);
    }

    public void Cleanup()
    {
    }
}
