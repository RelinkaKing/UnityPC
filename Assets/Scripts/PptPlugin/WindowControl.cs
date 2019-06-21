using UnityEngine;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Graphics = System.Drawing.Graphics;
using System.Management;

public class WindowControl : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("----------init-----------" + (int)ScreenData.instance.width + "x" + (int)ScreenData.instance.high );
        ScreenData.instance.SetChangeWindowsSize((int)ScreenData.instance.width, (int)ScreenData.instance.high);
    }

    public static float GetWindowsScaling()
    {
        Graphics currentGraphics = Graphics.FromHwnd(IntPtr.Zero);
        double dpixRatio = currentGraphics.DpiX / 96;
        print("------------------" + currentGraphics.DpiX / 96 + "  " + currentGraphics.DpiY / 96);
        return currentGraphics.DpiX / 96;
    }

    public void Exit()
    {
        Application.Quit();
    }

    [DllImport("user32.dll")]
    static extern int GetDpiForWindow(IntPtr hWnd);

    public float GetDisplayScaleFactor(IntPtr windowHandle)
    {
        try
        {
            return GetDpiForWindow(windowHandle) / 96f;
        }
        catch
        {
            // Or fallback to GDI solutions above
            return 1;
        }
    }

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(System.IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
    public static extern System.IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


    [DllImport("user32.dll")]
    private static extern bool EnableWindow(IntPtr hwnd, bool enable);
    [DllImport("user32.dll")]
    private static extern bool MoveWindow(IntPtr handle, int x, int y, int width,
    int height, bool redraw);

    public void WindowMin()
    {
        ShowWindow(FindWindow(null, "PPTPlugin"), 7);
    }
}
