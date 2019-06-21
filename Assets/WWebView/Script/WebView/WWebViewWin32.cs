// WWebViewWin32.cs : WWebViewWin32 implementation file
//
// Description      : WWebViewWin32
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// Created          : 2017/10/07
// Last Update      : 2017/12/03
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

#if UNITY_EDITOR_WIN || (UNITY_STANDALONE_WIN && !UNITY_EDITOR)

using UnityEngine;
using System;
using System.Text;
using System.Runtime.InteropServices;
using Win32;

namespace ICODES.STUDIO.WWebView
{
    public class WWebViewWin32
    {
        public delegate void ActionDocumentComplete(IntPtr name, IntPtr url);
        public delegate void ActionBeforeNavigate(IntPtr name, IntPtr url, IntPtr message, ref bool cancel);
        public delegate void ActionWindowClosing(IntPtr name, bool childWindow, ref bool cancel);
        public delegate void ActionTitleChange(IntPtr name, IntPtr title);
        public delegate void ActionNewWindow(IntPtr name, ref bool cancel);
        public delegate void ActionNavigateComplete(IntPtr name, IntPtr url);
        public delegate IntPtr ActionWindowProc(IntPtr window, uint message, IntPtr wparam, IntPtr lparam);

        private static IntPtr defaultWindowProc = IntPtr.Zero;
        private static MSG lastMsg;

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern int Initialize(
            IntPtr mainInstance,
            IntPtr prevInstance,
            string commandLine,
            int commandShow,
            IntPtr windowTemplate,
            int windowWidth,
            int windowHeight,
            int version);

        [DllImport("Win32-WebView")]
        public static extern bool SetResizeMode(uint mode);

        [DllImport("Win32-WebView")]
        public static extern void Release();

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern bool Create(
            string name,
            ActionDocumentComplete documentComplete,
            ActionBeforeNavigate beforeNavigate,
            ActionWindowClosing windowClosing,
            ActionTitleChange titleChange,
            ActionNewWindow newWindow,
            ActionNavigateComplete navigateComplete,
            int left, int top, int right, int bottom, int width, int height, bool popup);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void Navigate(string name, string url);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void NavigateToString(string name, string text);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern IntPtr AddJavaScript(string name, string script);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern IntPtr EvaluateJavaScript(string name, string script);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void Destroy(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void EnableContextMenu(string name, bool enable);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void CleanCache();

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void CleanCookie(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern IntPtr GetCookie(string url, string key);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void SetCookie(string url, string cookie);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void Refresh(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void GoBack(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void GoForward(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void Stop(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void Show(string name, bool show);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void ChangeLayout(string name, int left, int top, int right, int bottom, int width, int height);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern IntPtr CurrentUrl(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void AddUrlScheme(string name, string scheme);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void RemoveUrlScheme(string name, string scheme);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void SetZoom(string name, int factor);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void SetUserAgent(string name, string userAgent);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern IntPtr GetUserAgent(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void SetTexture(string name, IntPtr texture, int width, int height);

        [DllImport("Win32-WebView")]
        public static extern IntPtr GetRenderEventFunc();

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void Transparent(string name, bool transparent);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern float GetAlpha(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void SetAlpha(string name, float alpha);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern int GetActualWidth(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern int GetActualHeight(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void ShowScroll(string name, bool show);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void ShowScrollX(string name, bool show);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void ShowScrollY(string name, bool show);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void SetTitleText(string name, string text);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern bool CanGoBack(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern bool CanGoForward(string name);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void SetSilent(string name, bool mode);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void InputEvent(string name, int state, int key, int x, int y);

        [DllImport("Win32-WebView", CharSet = CharSet.Auto)]
        public static extern void SetHeaderField(string name, string key, string value);

        [DllImport("Win32-WebView")]
        public static extern void DispatchAccelerator();

        public static void DispatchMessage()
        {
#if UNITY_EDITOR_WIN
            if (User32.PeekMessage(out lastMsg, IntPtr.Zero, 0, 0, 0))
            {
                if (User32.GetMessage(out lastMsg, IntPtr.Zero, 0, 0))
                {
                    User32.TranslateMessage(ref lastMsg);
                    User32.DispatchMessage(ref lastMsg);
                }
            }
#endif
            DispatchAccelerator();
        }

        public static bool ModifyStyle(IntPtr window, long remove, long add, uint flags)
        {
            IntPtr style = User32.GetWindowLong(window, PARAMS.GWL_STYLE);
            IntPtr newStyle = new IntPtr((style.ToInt32() & ~remove) | add);

            if (style == newStyle)
                return false;

            User32.SetWindowLong(window, PARAMS.GWL_STYLE, newStyle);

            if (flags != 0)
            {
                User32.SetWindowPos(window, IntPtr.Zero, 0, 0, 0, 0,
                    PARAMS.SWP_NOSIZE | PARAMS.SWP_NOMOVE | PARAMS.SWP_NOZORDER | PARAMS.SWP_NOACTIVATE | flags);
            }

            return true;
        }

        public static IntPtr FindUnityPlayerWindow()
        {
            IntPtr window = FindUnityWindow("UnityWndClass");
            if (window != IntPtr.Zero)
                return window;

            return User32.FindWindow("UnityWndClass", Application.productName);
        }

        public static IntPtr FindUnityEditorWindow()
        {
            return FindUnityWindow("UnityContainerWndClass", "Unity");
        }

        public static IntPtr FindUnityWindow(string targetClassName, string prefixWindowText = null)
        {
            uint unityPID = Kernel32.GetCurrentProcessId();
            IntPtr currentWindow = User32.GetTopWindow(IntPtr.Zero);
            while (currentWindow != IntPtr.Zero)
            {
                uint currentPID;
                User32.GetWindowThreadProcessId(currentWindow, out currentPID);

                if (currentPID == unityPID)
                {
                    if (User32.IsWindowVisible(currentWindow) != 0)
                    {
                        StringBuilder className = new StringBuilder(1024);
                        User32.GetClassName(currentWindow, className, className.Capacity);

                        if (className.ToString() == targetClassName)
                        {
                            StringBuilder windowText = new StringBuilder(1024);
                            User32.GetWindowText(currentWindow, windowText, windowText.Capacity);

                            if (string.IsNullOrEmpty(prefixWindowText) || windowText.ToString().StartsWith(prefixWindowText))
                                return currentWindow;
                        }
                    }
                }
                currentWindow = User32.GetWindow(currentWindow, PARAMS.GW_HWNDNEXT);
            }

            return IntPtr.Zero;
        }

        public static void SubclassWindow()
        {
            IntPtr unityWindow = FindUnityPlayerWindow();
            ActionWindowProc windowProc = new ActionWindowProc(WindowProc);
            IntPtr windowProcPtr = Marshal.GetFunctionPointerForDelegate(windowProc);
            defaultWindowProc = User32.SetWindowLong(unityWindow, PARAMS.GWL_WNDPROC, windowProcPtr);
        }

        public static void UnsubclassWindow()
        {
            if (defaultWindowProc != IntPtr.Zero)
            {
                User32.SetWindowLong(FindUnityPlayerWindow(), PARAMS.GWL_WNDPROC, defaultWindowProc);
                defaultWindowProc = IntPtr.Zero;
            }
        }

        private static IntPtr DefWindowProc(IntPtr window, uint message, IntPtr wparam, IntPtr lparam)
        {
            if (defaultWindowProc != IntPtr.Zero)
                return User32.CallWindowProc(defaultWindowProc, window, message, wparam, lparam);

            return User32.DefWindowProcW(window, message, wparam, lparam);
        }

        private static IntPtr WindowProc(IntPtr window, uint message, IntPtr wparam, IntPtr lparam)
        {
            bool handled = false;
            IntPtr result = IntPtr.Zero;

            switch (message)
            {
                case PARAMS.WM_CLOSE:
                    handled = OnClose(window, message, wparam, lparam, ref result);
                    break;
            }

            if (handled == false)
                result = DefWindowProc(window, message, wparam, lparam);

            return result;
        }

        private static bool OnClose(IntPtr window, uint message, IntPtr wparam, IntPtr lparam, ref IntPtr result)
        {
            // NOTE: DO NOT call following commented method.
            // result = DefWindowProc(window, message, wparam, lparam);

            UnsubclassWindow();
            Application.Quit();
            return true;
        }
    }
}

#endif
