using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using QuickLook.Common.NativeMethods;

namespace QuickLook.Plugin.DuilibPreview;

public class DuilibPreviewControl : HwndHost
{
    private readonly string _path;
    private Process _process;
    private IntPtr _childHandle = IntPtr.Zero;

    public DuilibPreviewControl(string path)
    {
        _path = path;
    }

    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        // Path to DuilibPreview.exe
        string assemblyDir = Path.GetDirectoryName(typeof(Plugin).Assembly.Location);
        string exePath = Path.Combine(assemblyDir, "DuilibPreview.exe");

        if (!File.Exists(exePath))
        {
            // Try to find it in the usual build output location if debugging
            // But for release, it should be next to the plugin.
            // If not found, create a dummy label or similar? HwndHost can't easily return nothing.
            // We'll just let it fail or return zero handle which might crash.
            // Better: check exist externally.
        }

        ProcessStartInfo psi = new ProcessStartInfo(exePath)
        {
            Arguments = $"\"{_path}\"",
            WindowStyle = ProcessWindowStyle.Minimized, // Start minimized to avoid flicker
            UseShellExecute = false
        };

        _process = Process.Start(psi);

        // Wait for main window handle
        // Simple polling
        int retries = 20;
        while (retries > 0)
        {
            try
            {
                _process.Refresh();
                if (_process.MainWindowHandle != IntPtr.Zero)
                {
                    _childHandle = _process.MainWindowHandle;
                    break;
                }
            }
            catch { } // Process might exit immediately if error

            if (_process.HasExited) return new HandleRef(this, IntPtr.Zero);

            System.Threading.Thread.Sleep(100);
            retries--;
        }

        if (_childHandle != IntPtr.Zero)
        {
            // Remove Border and Caption
            int style = User32.GetWindowLong(_childHandle, -16); // GWL_STYLE
            style &= ~0x00C00000; // WS_CAPTION
            style &= ~0x00040000; // WS_THICKFRAME
            style |= 0x40000000; // WS_CHILD
            User32.SetWindowLong(_childHandle, -16, style);

            // Parent it
            SetParent(_childHandle, hwndParent.Handle);

            // Show window (it was minimized)
            // SW_SHOW = 5, SW_RESTORE = 9
            // Use User32.ShowWindow if available or PInvoke
            ShowWindow(_childHandle, 9); // Restore
        }

        return new HandleRef(this, _childHandle);
    }

    protected override void DestroyWindowCore(HandleRef hwnd)
    {
        if (_process != null && !_process.HasExited)
        {
            // Using Kill as CloseMainWindow might require message loop processing which might be stuck
            try
            {
                _process.Kill();
            }
            catch { }
            _process.Dispose();
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
