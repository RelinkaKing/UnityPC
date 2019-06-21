// WWebViewPluginWinEditor.cs : WWebViewPluginWinEditor implementation file
//
// Description      : WWebViewPluginWinEditor
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// Created          : 2017/07/12
// Last Update      : 2017/12/03
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

#if UNITY_EDITOR_WIN

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Win32;
using ICODES.STUDIO.WWebView;

#if !UNIWEBVIEW2_SUPPORTED && !UNIWEBVIEW3_SUPPORTED
namespace ICODES.STUDIO.WWebView
{
#endif
    /// <summary>
    /// A plugin wrapper class that supports the Windows Unity Editor.
    /// This class is only activated on Windows Unity Editor mode.
    /// </summary>
#if UNIWEBVIEW2_SUPPORTED
    public sealed class UniWebViewPlugin
#elif UNIWEBVIEW3_SUPPORTED
    public sealed class UniWebViewInterface
#else
    public sealed class WWebViewPlugin
#endif
    {
        private static WWebViewWin32.ActionDocumentComplete documentComplete = null;
        private static WWebViewWin32.ActionBeforeNavigate beforeNavigate = null;
        private static WWebViewWin32.ActionWindowClosing windowClosing = null;
        private static WWebViewWin32.ActionTitleChange titleChange = null;
        private static WWebViewWin32.ActionNewWindow newWindow = null;
        private static WWebViewWin32.ActionNavigateComplete navigateComplete = null;
        private static bool initialize = false;

        /// <summary>
        /// Initialize the WWebView.
        /// </summary>
        public static bool Init(string name, int top, int left, int bottom, int right, int width, int height)
        {
            if (initialize == false)
            {
                IntPtr module = Kernel32.GetModuleHandle(null);
                if (module == IntPtr.Zero)
                {
                    Debug.LogError("Can't find process module.");
                    return false;
                }

                IntPtr window = WWebViewWin32.FindUnityEditorWindow();
                if (window == IntPtr.Zero)
                {
                    Debug.LogError("Can't find Unity editor window handle.");
                    return false;
                }

                // IE Version description.
                // 11001: IE11. Webpages are displayed in IE11 edge mode, regardless of the declared !DOCTYPE directive.
                // 11000: IE11. Webpages containing standards - based !DOCTYPE directives are displayed in IE11 edge mode.
                // 10001: IE10. Webpages are displayed in IE10 Standards mode, regardless of the !DOCTYPE directive.
                // 10000: IE10. Webpages containing standards - based !DOCTYPE directives are displayed in IE10 Standards mode.
                //  9999:  IE9. Webpages are displayed in IE9 Standards mode, regardless of the declared !DOCTYPE directive.
                //  9000:  IE9. Webpages containing standards - based !DOCTYPE directives are displayed in IE9 mode.
                //  8888:  IE8. Webpages are displayed in IE8 Standards mode, regardless of the declared !DOCTYPE directive.
                //  8000:  IE8. Webpages containing standards - based !DOCTYPE directives are displayed in IE8 mode.
                //  7000:  IE7. Webpages containing standards - based !DOCTYPE directives are displayed in IE7 Standards mode.
                // REFER: https://msdn.microsoft.com/en-us/library/ee330730(v=vs.85).aspx

                WWebViewWin32.Initialize(module, IntPtr.Zero, null, 0, window, Screen.width, Screen.height, 11000);
                WWebViewSystem.Instance.Initialize();

                documentComplete = new WWebViewWin32.ActionDocumentComplete(OnDocumentComplete);
                beforeNavigate = new WWebViewWin32.ActionBeforeNavigate(OnBeforeNavigate);
                windowClosing = new WWebViewWin32.ActionWindowClosing(OnWindowClosing);
                titleChange = new WWebViewWin32.ActionTitleChange(OnTitleChange);
                newWindow = new WWebViewWin32.ActionNewWindow(OnNewWindow);
                navigateComplete = new WWebViewWin32.ActionNavigateComplete(OnNavigateComplete);
                initialize = true;
            }

            WWebViewWin32.Create(
                name,
                documentComplete,
                beforeNavigate,
                windowClosing,
                titleChange,
                newWindow,
                navigateComplete,
                left, top, right, bottom, width, height, true);

            WWebViewWin32.SetTitleText(name, "WWebView - In the editor mode, it is displayed as a popup window.");

#if UNIWEBVIEW2_SUPPORTED || UNIWEBVIEW3_SUPPORTED
            WWebViewWin32.AddUrlScheme(name, "uniwebview");
#endif
            WWebViewWin32.AddUrlScheme(name, "wwebview");

            return true;
        }

#if UNIWEBVIEW3_SUPPORTED
        /// <summary>
        /// Initialize the WWebView.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static bool Init(string name, int x, int y, int width, int height)
        {
            return Init(name, y, x, -1, -1, width, height);
        }
#else
        /// <summary>
        /// Initialize the WWebView.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static bool Init(string name, int top, int left, int bottom, int right)
        {
            return Init(name, top, left, bottom, right, 0, 0);
        }
#endif
        /// <summary>
        /// Release the WWebView.
        /// </summary>
        public static void Release()
        {
            WWebViewWin32.Release();
        }

        /// <summary>
        /// Changes the position and dimensions of current webview.
        /// </summary>
        public static void ChangeInsets(string name, int top, int left, int bottom, int right, int width, int height)
        {
            WWebViewWin32.ChangeLayout(name, left, top, right, bottom, width, height);
        }

        /// <summary>
        /// Changes the position and dimensions of current webview.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void ChangeInsets(string name, int top, int left, int bottom, int right)
        {
            ChangeInsets(name, top, left, bottom, right, 0, 0);
        }

        /// <summary>
        /// Changes the position and dimensions of current webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetFrame(string name, int x, int y, int width, int height)
        {
            ChangeInsets(name, y, x, -1, -1, width, height);
        }

        /// <summary>
        /// Changes the position of current webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetPosition(string name, int x, int y)
        {
            ChangeInsets(name, y, x, -1, -1, -1, -1);
        }

        /// <summary>
        /// Changes the size of current webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetSize(string name, int width, int height)
        {
            ChangeInsets(name, -1, -1, -1, -1, width, height);
        }

        /// <summary>
        /// Specifies a value indicating whether to enable the context menu, which appears when the right mouse button is clicked.
        /// </summary>
        public static void EnableContextMenu(string name, bool enable)
        {
            WWebViewWin32.EnableContextMenu(name, enable);
        }

        /// <summary>
        /// Real-time rendering of webview content to the specified texture.
        /// </summary>
        public static void SetTexture(string name, Texture texture)
        {
            WWebViewWin32.SetTexture(name, texture.GetNativeTexturePtr(), texture.width, texture.height);
        }

        /// <summary>
        /// Process the input event.
        /// </summary>
        public static void InputEvent(string name, int state, int key, int x, int y)
        {
            WWebViewWin32.InputEvent(name, state, key, x, y);
        }

        /// <summary>
        /// Specifies the zoom factor of the webview.
        /// </summary>
        public static void SetZoom(string name, int factor)
        {
            WWebViewWin32.SetZoom(name, factor);
        }

        /// <summary>
        /// Retrieves the render event handler function of the webview plugin.
        /// </summary>
        public static IntPtr GetRenderEventFunc()
        {
            return WWebViewWin32.GetRenderEventFunc();
        }

        /// <summary>
        /// Sets the visibility of all(both) scroll bar for the webview.
        /// </summary>
        public static void ShowScroll(string name, bool show)
        {
            WWebViewWin32.ShowScroll(name, show);
        }

        /// <summary>
        /// Sets the visibility of horizontal scroll bar for the webview.
        /// </summary>
        public static void ShowScrollX(string name, bool show)
        {
            WWebViewWin32.ShowScrollX(name, show);
        }

        /// <summary>
        /// Sets the visibility of horizontal scroll bar for the webview.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetHorizontalScrollBarShow(string name, bool show)
        {
            WWebViewWin32.ShowScrollX(name, show);
        }

        /// <summary>        
        /// Sets the visibility of horizontal scroll bar for the webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetHorizontalScrollBarEnabled(string name, bool enabled)
        {
            ShowScrollX(name, enabled);
        }

        /// <summary>
        /// Sets the visibility of vertical scroll bar for the webview.
        /// </summary>
        public static void ShowScrollY(string name, bool show)
        {
            WWebViewWin32.ShowScrollY(name, show);
        }

        /// <summary>
        /// Sets the visibility of vertical scroll bar for the webview.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetVerticalScrollBarShow(string name, bool show)
        {
            WWebViewWin32.ShowScrollY(name, show);
        }

        /// <summary>
        /// Sets the visibility of vertical scroll bar for the webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetVerticalScrollBarEnabled(string name, bool enabled)
        {
            ShowScrollY(name, enabled);
        }

        /// <summary>
        /// Adds a url scheme to WWebview message interpreter.
        /// </summary>
        public static void AddUrlScheme(string name, string scheme)
        {
            WWebViewWin32.AddUrlScheme(name, scheme);
        }

        /// <summary>
        /// Remove a url scheme from WWebView message interpreter.
        /// </summary>
        public static void RemoveUrlScheme(string name, string scheme)
        {
            WWebViewWin32.RemoveUrlScheme(name, scheme);
        }

        /// <summary>
        /// Navigate a url in current webview.
        /// </summary>
        public static void Load(string name, string url)
        {
            WWebViewWin32.Navigate(name, url);
        }

        /// <summary>
        /// Navigate a url in current webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void Load(string name, string url, bool skipEncoding)
        {
            Load(name, url);
        }

        /// <summary>
        /// Navigate an HTML string in current webview.
        /// </summary>
        public static void LoadHTMLString(string name, string html, string baseUrl)
        {
            WWebViewWin32.NavigateToString(name, html);
        }

        /// <summary>
        /// Navigate an HTML string in current webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void LoadHTMLString(string name, string html, string baseUrl, bool skipEncoding)
        {
            LoadHTMLString(name, html, baseUrl);
        }

        /// <summary>
        /// Reload current page.
        /// </summary>
        public static void Reload(string name)
        {
            WWebViewWin32.Refresh(name);
        }

        /// <summary>
        /// Add some javascript to the web page.
        /// </summary>
        public static void AddJavaScript(string name, string script)
        {
            WWebViewWin32.AddJavaScript(name, script);
        }

        /// <summary>
        /// Add some javascript to the web page.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void AddJavaScript(string name, string script, string identifier)
        {
            AddJavaScript(name, script);
        }

        /// <summary>
        /// Evaluate a JavaScript string on current page.
        /// </summary>
        public static void EvaluatingJavaScript(string name, string script)
        {
            GameObject webView = GameObject.Find(name);
            if (webView != null)
            {
                string result =
                    Marshal.PtrToStringAuto(
                        WWebViewWin32.EvaluateJavaScript(name, script));

#if UNIWEBVIEW3_SUPPORTED
                result =
                    "{\"data\":\"" + 
                    WWebViewSystem.EscapeJsonText(result) + 
                    "\",\"resultCode\":\"0\",\"identifier\":\"\"}";
#endif
                webView.SendMessage("EvalJavaScriptFinished", result, SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Evaluate a JavaScript string on current page.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void EvaluateJavaScript(string name, string script, string identifier)
        {
            EvaluatingJavaScript(name, script);
        }

        /// <summary>
        /// Clears all caches. This will removes cached local data of webview.
        /// </summary>
        public static void CleanCache(string name)
        {
            WWebViewWin32.CleanCache();
        }

        /// <summary>
        /// Clears all cookies from webview.
        /// </summary>
        public static void CleanCookie(string name, string key)
        {
            WWebViewWin32.CleanCookie(name);
        }

        /// <summary>
        /// Clears all cookies from webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void ClearCookies()
        {
            CleanCookie(string.Empty, string.Empty);
        }

        /// <summary>
        /// Gets the cookie value under a url and key.
        /// </summary>
        public static string GetCookie(string url, string key)
        {
            return Marshal.PtrToStringAuto(WWebViewWin32.GetCookie(url, key));
        }

        /// <summary>
        /// Gets the cookie value under a url and key.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static string GetCookie(string url, string key, bool skipEncoding)
        {
            return GetCookie(url, key);
        }

        /// <summary>
        /// Sets a cookie for a certain url.
        /// </summary>
        public static void SetCookie(string url, string cookie)
        {
            WWebViewWin32.SetCookie(url, cookie);
        }

        /// <summary>
        /// Sets a cookie for a certain url.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetCookie(string url, string cookie, bool skipEncoding)
        {
            SetCookie(url, cookie);
        }

        /// <summary>
        /// Destroy the webview instance.
        /// </summary>
        public static void Destroy(string name)
        {
            WWebViewWin32.Destroy(name);
        }

        /// <summary>
        /// Navigates the webview to the previous page in the navigation history, if one is available.
        /// </summary>
        public static void GoBack(string name)
        {
            WWebViewWin32.GoBack(name);
        }

        /// <summary>
        /// Navigates the webview to the next page in the navigation history, if one is available.
        /// </summary>
        public static void GoForward(string name)
        {
            WWebViewWin32.GoForward(name);
        }

        /// <summary>
        /// Cancels any pending navigation and stops any dynamic page elements, such as background sounds and animations.
        /// </summary>
        public static void Stop(string name)
        {
            WWebViewWin32.Stop(name);
        }

        /// <summary>
        /// Retrieves current navigating url.
        /// </summary>
        public static string GetCurrentUrl(string name)
        {
            return Marshal.PtrToStringAuto(WWebViewWin32.CurrentUrl(name));
        }

        /// <summary>
        /// Retrieves current navigating url.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static string GetUrl(string name)
        {
            return GetCurrentUrl(name);
        }

        /// <summary>
        /// Shows the webview window.
        /// </summary>
        public static bool Show(string name, bool fade, int direction, float duration)
        {
            WWebViewWin32.Show(name, true);
            return true;
        }

        /// <summary>
        /// Shows the webview window.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static bool Show(string name, bool fade, int edge, float duration, string identifier)
        {
            return Show(name, fade, edge, duration);
        }

        /// <summary>
        /// Hides the webview window.
        /// </summary>
        public static bool Hide(string name, bool fade, int direction, float duration)
        {
            WWebViewWin32.Show(name, false);
            return true;
        }

        /// <summary>
        /// Hides the webview window.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static bool Hide(string name, bool fade, int edge, float duration, string identifier)
        {
            return Hide(name, fade, edge, duration);
        }

        /// <summary>
        /// Sets a value indicating whether this webview can be zoomed or not.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetZoomEnable(string name, bool enable)
        {
            // N/A
        }

        /// <summary>
        /// Sets a value indicating whether this webview can be zoomed or not.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetZoomEnabled(string name, bool enabled)
        {
            // N/A
        }

        /// <summary>
        /// Retrieves the screen scale.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static int ScreenScale()
        {
            return 1;
        }

        /// <summary>
        /// Retrieves the unique id of the webview.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static int GetId(string name)
        {
            return 0;
        }

        /// <summary>
        /// Process the input event.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void InputEvent(
            string name, int x, int y, float deltaY,
            bool buttonDown, bool buttonPress, bool buttonRelease,
            bool keyPress, short keyCode, string keyChars, int textureId)
        {
            // N/A
        }

        /// <summary>
        /// Gets the user-agent string currently used in webview.
        /// If a customized user-agent is not set, the default user-agent in current platform will be returned.
        /// </summary>
        public static string GetUserAgent(string name)
        {
            return Marshal.PtrToStringAuto(WWebViewWin32.GetUserAgent(name));
        }

        /// <summary>
        /// Sets the user-agent used in the webview.
        /// </summary>
        public static void SetUserAgent(string userAgent)
        {
            WWebViewWin32.SetUserAgent(null, userAgent);
        }

        /// <summary>
        /// Sets the user-agent used in the webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetUserAgent(string name, string userAgent)
        {
            WWebViewWin32.SetUserAgent(name, userAgent);
        }

        /// <summary>
        /// Sets the background of webview to transparent.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void TransparentBackground(string name, bool transparent)
        {
            WWebViewWin32.Transparent(name, transparent);
        }

        /// <summary>
        /// Gets alpha-blending value in current webview window.
        /// </summary>
        public static float GetAlpha(string name)
        {
            return WWebViewWin32.GetAlpha(name);
        }

        /// <summary>
        /// Gets alpha-blending value in current webview window.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static float GetWebViewAlpha(string name)
        {
            return GetAlpha(name);
        }

        /// <summary>
        /// Sets alpha-blending value in current webview window.
        /// </summary>
        public static void SetAlpha(string name, float alpha)
        {
            WWebViewWin32.SetAlpha(name, alpha);
        }

        /// <summary>
        /// Sets alpha-blending value in current webview window.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetWebViewAlpha(string name, float alpha)
        {
            SetAlpha(name, alpha);
        }

        /// <summary>
        /// Get the webview's width, as a value in device-independent units (1/96th inch per unit).
        /// The default is 0. The default might be encountered if the webview has not been loaded 
        /// and hasn't yet been involved in a layout pass that renders the UI.
        /// </summary>
        public static int GetActualWidth(string name)
        {
            return WWebViewWin32.GetActualWidth(name);
        }

        /// <summary>
        /// Get the webview's height, as a value in device-independent units (1/96th inch per unit).
        /// The default is 0. The default might be encountered if the webview has not been loaded 
        /// and hasn't yet been involved in a layout pass that renders the UI.
        /// </summary>
        public static int GetActualHeight(string name)
        {
            return WWebViewWin32.GetActualHeight(name);
        }

        /// <summary>
        /// Gets a value indicating whether a previous page in navigation history is available, 
        /// which allows the GoBack method to succeed.
        /// </summary>
        public static bool CanGoBack(string name)
        {
            return WWebViewWin32.CanGoBack(name);
        }

        /// <summary>
        /// Gets a value indicating whether a subsequent page in navigation history is available, 
        /// which allows the GoForward method to succeed.
        /// </summary>
        public static bool CanGoForward(string name)
        {
            return WWebViewWin32.CanGoForward(name);
        }

        /// <summary>
        /// Set the header field.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetHeaderField(string name, string key, string value)
        {
            WWebViewWin32.SetHeaderField(name, key, value);
        }

        /// <summary>
        /// Set if a default spinner should show when loading the webpage.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetSpinnerShowWhenLoading(string name, bool show)
        {
            // N/A
        }

        /// <summary>
        /// Set if a default spinner should show when loading the webpage.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetShowSpinnerWhileLoading(string name, bool show)
        {
            // N/A
        }

        /// <summary>
        /// Sets the label text for the spinner showing when webview loading.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetSpinnerText(string name, string text)
        {
            // N/A
        }

        /// <summary>
        /// Gets or sets a value indicating whether this webview can bounces or not.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetBounces(string name, bool enable)
        {
            // N/A
        }

        /// <summary>
        /// Whether the new page should be opened in an external browser when user clicks a link. 
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static bool GetOpenLinksInExternalBrowser(string name)
        {
            return false;
        }

        /// <summary>
        /// Whether the new page should be opened in an external browser when user clicks a link. 
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetOpenLinksInExternalBrowser(string name, bool value)
        {
            // N/A
        }

        /// <summary>
        /// Set the background of webview to transparent.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetBackgroundColor(string name, float r, float g, float b, float a)
        {
            // N/A
        }

        /// <summary>
        /// Set whether the video in HTML page should be auto played or not.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetAllowAutoPlay(string name, bool value)
        {
            // N/A
        }

        /// <summary>
        /// Set whether the video in HTML page should be auto played or not.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetAllowAutoPlay(bool flag)
        {
            // N/A
        }

        /// <summary>
        /// Set allow inline play for current webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetAllowInlinePlay(bool flag)
        {
            // N/A
        }

        /// <summary>
        /// Set all web views allowing third party cookies set from a non-main frame or other third party web pages.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetAllowThirdPartyCookies(bool allowed)
        {
            // N/A
        }

        /// <summary>
        /// Select log level.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetLogLevel(int level)
        {
            // N/A
        }

        /// <summary>
        /// Animates the webview from current position and size to another position and size.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static bool AnimateTo(string name, int x, int y, int width, int height, float duration, float delay, string identifier)
        {
            return false;
        }

        /// <summary>
        /// Adds a domain to the SSL checking white list.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void AddSslExceptionDomain(string name, string domain)
        {
            // N/A
        }

        /// <summary>
        /// Removes a domain from the SSL checking white list.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void RemoveSslExceptionDomain(string name, string domain)
        {
            // N/A
        }

        /// <summary>
        /// Sets whether JavaScript can open windows without user interaction.
        /// </summary>
        public static void SetAllowJavaScriptOpenWindow(bool flag)
        {
            // N/A
        }

        /// <summary>
        /// Sets whether JavaScript should be enabled in current webview. Default is enabled.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetJavaScriptEnabled(bool flag)
        {
            // N/A
        }

        /// <summary>
        /// Clears any saved credentials for HTTP authentication for both Basic and Digest.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void ClearHttpAuthUsernamePassword(string host, string realm)
        {
            // N/A
        }

        /// <summary>
        /// Sets whether the webview should show with a bounces effect when scrolling to page edge.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetBouncesEnabled(string name, bool enabled)
        {
            // N/A
        }

        /// <summary>
        /// Sets whether to show a toolbar which contains navigation buttons and Done button.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetShowToolbar(string name, bool show, bool animated, bool onTop, bool adjustInset)
        {
            // N/A
        }

        /// <summary>
        /// Sets the done button text in toolbar.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetToolbarDoneButtonText(string name, string text)
        {
            // N/A
        }

        /// <summary>
        /// Enables debugging of web contents.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetWebContentsDebuggingEnabled(bool enabled)
        {
            // N/A
        }

        /// <summary>
        /// Prints current page.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void Print(string name)
        {
            // N/A
        }

        /// <summary>
        /// Called from the native webview when navigation is started.
        /// </summary>
        /// <param name="name">Target(event dispatcher) identifier.</param>
        /// <param name="url">Url to be navigated.</param>
        /// <param name="message">Registered url scheme message.</param>
        /// <param name="cancel">Decides whether to actually navigate or not.</param>
        private static void OnBeforeNavigate(IntPtr name, IntPtr url, IntPtr message, ref bool cancel)
        {
            GameObject webView = GameObject.Find(Marshal.PtrToStringAuto(name));
            if (webView != null)
            {
                string navigateUrl = Marshal.PtrToStringAuto(url);
                string customMessage = Marshal.PtrToStringAuto(message);

                if (string.IsNullOrEmpty(customMessage) == false)
                {
                    webView.SendMessage(
#if UNIWEBVIEW3_SUPPORTED
                        "MessageReceived",
#else
                        "ReceivedMessage",
#endif
                        navigateUrl,
                        SendMessageOptions.DontRequireReceiver);

                    if (customMessage == "close" || customMessage == "close/")
                        webView.SendMessage("WebViewDone", string.Empty, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    webView.SendMessage(
#if UNIWEBVIEW3_SUPPORTED
                        "PageStarted",
#else
                        "LoadBegin",
#endif
                        navigateUrl,
                        SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        /// <summary>
        /// Fired from native webview when a page is completely loaded.
        /// </summary>
        /// <param name="name">Target(event dispatcher) identifier.</param>
        /// <param name="url">Navigated url</param>
        private static void OnDocumentComplete(IntPtr name, IntPtr url)
        {
            GameObject webView = GameObject.Find(Marshal.PtrToStringAuto(name));
            if (webView != null)
            {
                webView.SendMessage(
#if UNIWEBVIEW3_SUPPORTED
                    "PageFinished", 
                    "{\"data\":\"" + WWebViewSystem.EscapeJsonText(Marshal.PtrToStringAuto(url)) + "\",\"resultCode\":\"200\",\"identifier\":\"\"}",
#elif UNIWEBVIEW2_SUPPORTED
                    "LoadComplete", string.Empty,
#else
                    "LoadComplete",
                    Marshal.PtrToStringAuto(url),
#endif
                    SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Invoked when the webview is closed or needs to be closed.
        /// </summary>
        /// <param name="name">Target(event dispatcher) identifier.</param>
        /// <param name="cancel">Decides whether to actually close or not.</param>
        private static void OnWindowClosing(IntPtr name, bool childWindow, ref bool cancel)
        {
            GameObject webView = GameObject.Find(Marshal.PtrToStringAuto(name));
            if (webView != null)
            {
                if (childWindow == false)
                    webView.SendMessage("WebViewDone", string.Empty, SendMessageOptions.DontRequireReceiver);
            }
        }

        private static void OnTitleChange(IntPtr name, IntPtr title)
        {
            // N/A
        }

        private static void OnNewWindow(IntPtr name, ref bool cancel)
        {
            // N/A
        }

        private static void OnNavigateComplete(IntPtr name, IntPtr url)
        {
            // N/A
        }
    }
#if !UNIWEBVIEW2_SUPPORTED && !UNIWEBVIEW3_SUPPORTED
}
#endif
#endif
