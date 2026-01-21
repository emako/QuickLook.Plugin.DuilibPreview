using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace QuickLook.Plugin.DuilibPreview;

public class DuilibPreviewControl : HwndHost
{
    private readonly string _path;
    private IntPtr _childHandle = IntPtr.Zero;
    private IntPtr _dllHandle = IntPtr.Zero;

    public DuilibPreviewControl(string path)
    {
        _path = path;
    }

    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        _childHandle = IntPtr.Zero;
        string assemblyDir = Path.GetDirectoryName(typeof(Plugin).Assembly.Location);
        string dllPath = Path.Combine(assemblyDir, "DuilibPreview.dll");

        try
        {
            if (File.Exists(dllPath))
            {
                // Ensure dependencies like DuiLib_u.dll are found
                SetDllDirectory(assemblyDir);
                try
                {
                    _dllHandle = LoadLibrary(dllPath);
                }
                finally
                {
                    SetDllDirectory(null!);
                }
            }

            if (_dllHandle != IntPtr.Zero)
            {
                try
                {
                    _childHandle = CreateDuilibPreview(hwndParent.Handle, _path);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }

        if (_childHandle == IntPtr.Zero)
        {
            // Create a basic error window instead of crashing
            // WS_CHILD | WS_VISIBLE
            _childHandle = CreateWindowEx(
                0, "STATIC", "DuilibPreview load failed.\nCheck if DuilibPreview.dll and DuiLib_u.dll exist.",
                0x50000000,
                0, 0, 800, 600,
                hwndParent.Handle,
                IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        return new HandleRef(this, _childHandle);
    }

    [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateWindowEx(
        int dwExStyle,
        string lpClassName,
        string lpWindowName,
        int dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    protected override void DestroyWindowCore(HandleRef hwnd)
    {
        if (_childHandle != IntPtr.Zero)
        {
            DestroyWindow(_childHandle);
            _childHandle = IntPtr.Zero;
        }

        if (_dllHandle != IntPtr.Zero)
        {
            FreeLibrary(_dllHandle);
            _dllHandle = IntPtr.Zero;
        }
    }

    public void GetDefinitionSize(out int width, out int height)
    {
        width = 0;
        height = 0;
        string assemblyDir = Path.GetDirectoryName(typeof(DuilibPreviewControl).Assembly.Location);

        SetDllDirectory(assemblyDir);
        try
        {
            GetDuilibXmlDefinitionSize(_path, out width, out height);
        }
        catch
        {
            // ignore
        }
        finally
        {
            SetDllDirectory(null!);
        }
    }

    [DllImport("DuilibPreview.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern void GetDuilibXmlDefinitionSize(string xmlPath, out int width, out int height);

    [DllImport("DuilibPreview.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreateDuilibPreview(IntPtr hParent, string xmlPath);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string libname);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetDllDirectory(string lpPathName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool DestroyWindow(IntPtr hWnd);
}
