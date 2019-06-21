// WWebViewPluginWSA.cs : WWebViewPluginWSA implementation file
//
// Description      : WWebViewPluginWSA
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// Created          : 2017/07/12
// Last Update      : 2017/12/03
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

#if UNITY_WSA && !UNITY_EDITOR

using UnityEngine;
using System;
using ICODES.STUDIO.WSA;
using ICODES.STUDIO.WSA.Controls;
using ICODES.STUDIO.WWebView;

#if !UNIWEBVIEW2_SUPPORTED && !UNIWEBVIEW3_SUPPORTED
namespace ICODES.STUDIO.WWebView
{
#endif
    /// <summary>
    /// A plugin wrapper class that supports the Windows Store platform.
    /// This class is only activated on Windows Store platform mode.
    /// </summary>
#if UNIWEBVIEW2_SUPPORTED
    public sealed class UniWebViewPlugin
#elif UNIWEBVIEW3_SUPPORTED
    public sealed class UniWebViewInterface
#else
    public sealed class WWebViewPlugin
#endif
    {
        private static bool initialize = false;

        /// <summary>
        /// Initialize the WWebView.
        /// </summary>
        public static bool Init(string name, int top, int left, int bottom, int right, int width, int height) 
        {
            if (initialize == false)
            {
                WWebViewSystem.Instance.Initialize();
                initialize = true;
            }

            WebViewSystem.Instance.Create(
                name,
                OnNavigationStarting,
                OnNavigationCompleted,
                OnNavigationFailed,
                OnScriptNotify,
                OnEvaluatingJavaScriptFinished,
                left, top, right, bottom, width, height);

#if UNIWEBVIEW2_SUPPORTED || UNIWEBVIEW3_SUPPORTED
            AddUrlScheme(name, "uniwebview");
#endif
            AddUrlScheme(name, "wwebview");

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
            // N/A
        }

        /// <summary>
        /// Changes the position and dimensions of current webview.
        /// </summary>
        public static void ChangeInsets(string name, int top, int left, int bottom, int right, int width, int height)
        {
            WebViewSystem.Instance.ChangeLayout(name, left, top, right, bottom, width, height);
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
            WebViewSystem.Instance.EnableContextMenu(name, enable);
        }

        /// <summary>
        /// Real-time rendering of webview content to the specified texture.
        /// </summary>
        public static void SetTexture(string name, Texture texture)
        {
            WebViewSystem.Instance.SetTexture(name, texture.GetNativeTexturePtr(), texture.width, texture.height);
        }

        /// <summary>
        /// Process the input event.
        /// </summary>
        public static void InputEvent(string name, int state, int key, int x, int y)
        {
            // To be honest, I couldn't find a way to programmatically pass the mouse event and keyboard input to the webview UI.
            // And I think there is a way to implement this obviously. Because I'm not used to developing Windows Store App.
            // So, I would be very happy if someone could tell me how.

            UnityEngine.Debug.LogWarning("The input processing does not supported yet.");
        }

        /// <summary>
        /// Specifies the zoom factor of the webview.
        /// </summary>
        public static void SetZoom(string name, int factor) 
        {
            // TODO: ...
        }

        /// <summary>
        /// Retrieves the render event handler function of the webview plugin.
        /// </summary>
        public static IntPtr GetRenderEventFunc()
        {
            return WebViewSystem.Instance.GetRenderEventFunc();
        }

        /// <summary>
        /// Sets the visibility of all(both) scroll bar for the webview.
        /// </summary>
        public static void ShowScroll(string name, bool show)
        {
            WebViewSystem.Instance.ShowScroll(name, show);
        }

        /// <summary>
        /// Sets the visibility of horizontal scroll bar for the webview.
        /// </summary>
        public static void ShowScrollX(string name, bool show)
        {
            WebViewSystem.Instance.ShowScrollX(name, show);
        }

        /// <summary>
        /// Sets the visibility of horizontal scroll bar for the webview.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetHorizontalScrollBarShow(string name, bool show)
        {
            WebViewSystem.Instance.ShowScrollX(name, show);
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
            WebViewSystem.Instance.ShowScrollY(name, show);
        }

        /// <summary>
        /// Sets the visibility of vertical scroll bar for the webview.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetVerticalScrollBarShow(string name, bool show)
        {
            WebViewSystem.Instance.ShowScrollY(name, show);
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
        /// Switch to WWebView specific screen. 
        /// </summary>
        public static void SwitchXamlView()
        {
            WebViewSystem.Instance.SwitchXamlView();
        }

        /// <summary>
        /// Switch to Unity specific screen. 
        /// </summary>
        public static void SwitchMainView()
        {
            WebViewSystem.Instance.SwitchMainView();
        }

        /// <summary>
        /// Adds a url scheme to WWebview message interpreter.
        /// </summary>
        public static void AddUrlScheme(string name, string scheme) 
        {
            WebViewSystem.Instance.AddUrlScheme(name, scheme);
        }

        /// <summary>
        /// Remove a url scheme from WWebView message interpreter.
        /// </summary>
        public static void RemoveUrlScheme(string name, string scheme) 
        {
            WebViewSystem.Instance.RemoveUrlScheme(name, scheme);
        }

        /// <summary>
        /// Navigate a url in current webview.
        /// </summary>
        public static void Load(string name, string url) 
        {
            WebViewSystem.Instance.Navigate(name, url);
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
            WebViewSystem.Instance.NavigateToString(name, html);
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
            WebViewSystem.Instance.Refresh(name);
        }

        /// <summary>
        /// Add some javascript to the web page.
        /// </summary>
        public static void AddJavaScript(string name, string script) 
        {
            WebViewSystem.Instance.AddJavaScript(name, script);
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
            WebViewSystem.Instance.EvaluateJavaScript(name, script);
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
            WebViewSystem.Instance.CleanCache();
        }

        /// <summary>
        /// Clears all cookies from webview.
        /// </summary>
        public static void CleanCookie(string name, string key)
        {
            WebViewSystem.Instance.CleanCookie(name);
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
            return WebViewSystem.Instance.GetCookie(url, key);
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
            // TODO: parse the cookie more detail(path, expire, etc...)
            // eg. name = value; path = /abc/; expires = Sat, 01-Jan-3000 00:00:00 GMT;

            string[] keyValue = cookie.Split("="[0]);
            UnityEngine.Debug.Assert(keyValue.Length == 2);
            WebViewSystem.Instance.SetCookie(url, keyValue[0].Trim(), keyValue[1].Trim(), "/", DateTime.MinValue);
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
            WebViewSystem.Instance.Destroy(name);
        }

        /// <summary>
        /// Navigates the webview to the previous page in the navigation history, if one is available.
        /// </summary>
        public static void GoBack(string name) 
        {
            WebViewSystem.Instance.GoBack(name);
        }

        /// <summary>
        /// Navigates the webview to the next page in the navigation history, if one is available.
        /// </summary>
        public static void GoForward(string name) 
        {
            WebViewSystem.Instance.GoForward(name);
        }

        /// <summary>
        /// Cancels any pending navigation and stops any dynamic page elements, such as background sounds and animations.
        /// </summary>
        public static void Stop(string name)
        {
            WebViewSystem.Instance.Stop(name);
        }

        /// <summary>
        /// Retrieves current navigating url.
        /// </summary>
        public static string GetCurrentUrl(string name) 
        {
            return WebViewSystem.Instance.CurrentUrl(name);
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
            WebViewSystem.Instance.Show(name, true);
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
            WebViewSystem.Instance.Show(name, false);
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
        /// Gets the user-agent string currently used in webview.
        /// If a customized user-agent is not set, the default user-agent in current platform will be returned.
        /// </summary>
        public static string GetUserAgent(string name)
        {
            return WebViewSystem.Instance.GetUserAgent(name);
        }

        /// <summary>
        /// Sets the user-agent used in the webview.
        /// </summary>
        public static void SetUserAgent(string userAgent)
        {
            WebViewSystem.Instance.SetUserAgent(null, userAgent);
        }

        /// <summary>
        /// Sets the user-agent used in the webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetUserAgent(string name, string userAgent)
        {
            WebViewSystem.Instance.SetUserAgent(name, userAgent);
        }

        /// <summary>
        /// Sets the background of webview to transparent.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void TransparentBackground(string name, bool transparent) 
        {
            WebViewSystem.Instance.Transparent(name, transparent);
        }

        /// <summary>
        /// Gets alpha-blending value in current webview window.
        /// </summary>
        public static float GetAlpha(string name)
        {
            return WebViewSystem.Instance.GetAlpha(name);
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
            WebViewSystem.Instance.SetAlpha(name, alpha);
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
            return WebViewSystem.Instance.GetActualWidth(name);
        }

        /// <summary>
        /// Get the webview's height, as a value in device-independent units (1/96th inch per unit).
        /// The default is 0. The default might be encountered if the webview has not been loaded 
        /// and hasn't yet been involved in a layout pass that renders the UI.
        /// </summary>
        public static int GetActualHeight(string name)
        {
            return WebViewSystem.Instance.GetActualHeight(name);
        }

        /// <summary>
        /// Gets a value indicating whether a previous page in navigation history is available, 
        /// which allows the GoBack method to succeed.
        /// </summary>
        public static bool CanGoBack(string name)
        {
            return WebViewSystem.Instance.CanGoBack(name);
        }

        /// <summary>
        /// Gets a value indicating whether a subsequent page in navigation history is available, 
        /// which allows the GoForward method to succeed.
        /// </summary>
        public static bool CanGoForward(string name)
        {
            return WebViewSystem.Instance.CanGoForward(name);
        }

        /// <summary>
        /// Set the header field.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetHeaderField(string name, string key, string value)
        {
            WebViewSystem.Instance.SetHeaderField(name, key, value);
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
        /// NOTE: WebViewNavigationStartingEventArgs.Cancel does not work on Unity version 2017.2.0
        /// The internal structure of UWP support seems to have changed a lot on 2017.2.0
        /// Unity has a mechanism called AppCallback to synchronize Xaml UI thread and Unity App thread.
        /// BTW, UnityEngine.WSA.Application.InvokeOnAppThread occasionally gets freezed.
        /// I don't know exactly why this method does not work. If it's a simple bug, Unity will be patched later
        /// </summary>
        private static void OnNavigationStarting(WebViewClient sender, WebViewNavigationStartingEventArgs args)
        {
            GameObject webView = GameObject.Find(sender.Name);
            if (webView != null)
            {
                if (string.IsNullOrEmpty(args.CustomMessage) == false)
                {
                    webView.SendMessage(
#if UNIWEBVIEW3_SUPPORTED
                        "MessageReceived",
#else
                        "ReceivedMessage",
#endif
                        args.Uri.ToString(),
                        SendMessageOptions.DontRequireReceiver);

                    if (args.CustomMessage == "close" || args.CustomMessage == "close/")
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
                        args.Uri.ToString(),
                        SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        /// <summary>
        /// Fired from native webview when a page is completely loaded.
        /// </summary>
        private static void OnNavigationCompleted(WebViewClient sender, WebViewNavigationCompletedEventArgs args)
        {
            GameObject webView = GameObject.Find(sender.Name);
            if (webView != null)
            {
                webView.SendMessage(
#if UNIWEBVIEW3_SUPPORTED
                    "PageFinished", 
                    "{\"data\":\"" + WWebViewSystem.EscapeJsonText(args.Uri.ToString()) + 
                    "\",\"resultCode\":\"" + args.WebErrorStatus.ToString() + "\",\"identifier\":\"\"}",
#elif UNIWEBVIEW2SUPPORTED
                    "LoadComplete", (args.IsSuccess == true) ? string.Empty : args.WebErrorStatus.ToString(), 
#else
                    "LoadComplete", (args.IsSuccess == true) ? args.Uri.ToString() : args.WebErrorStatus.ToString(),
#endif
                    SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Invoked when the navigation is failed.
        /// </summary>
        private static void OnNavigationFailed(WebViewClient sender, WebViewNavigationFailedEventArgs args)
        {
            GameObject webView = GameObject.Find(sender.Name);
            if (webView != null)
            {
                webView.SendMessage(
#if UNIWEBVIEW3_SUPPORTED
                    "PageErrorReceived", 
                    "{\"data\":\"" + WWebViewSystem.EscapeJsonText(args.Uri.ToString()) + 
                    "\",\"resultCode\":\"" + args.WebErrorStatus.ToString() + "\",\"identifier\":\"\"}",
#else
                    "LoadComplete", args.WebErrorStatus.ToString(),
#endif
                    SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Occurs when the content contained in the webview passes a string to the application by using JavaScript.
        /// NOTE: To enable an external web page to fire the ScriptNotify event when calling window.external.notify, 
        /// You must include the page's URI in the ApplicationContentUriRules section of the app manifest. 
        /// You can do this in Visual Studio on the Content URIs tab of the Package.appxmanifest designer. 
        /// The URIs in this list must use HTTPS and may contain subdomain wildcards (for example, "https://*.microsoft.com"), 
        /// but they can't contain domain wildcards (for example, "https://*.com" and "https://*.*"). 
        /// The manifest requirement does not apply to content that originates from the app package, 
        /// uses an ms-local-stream:// URI, or is loaded using NavigateToString.
        /// </summary>
        private static void OnScriptNotify(WebViewClient sender, NotifyEventArgs args)
        {
            // REF: https://social.msdn.microsoft.com/Forums/vstudio/en-US/f322a505-87af-42a1-b196-1143011ba327/uwpc-script-notify-webview-not-working?forum=wpdevelop
            // REF: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.notifyeventhandler
            // REF: https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.WebView#Windows_UI_Xaml_Controls_WebView_AllowedScriptNotifyUris
        }

        /// <summary>
        /// Called from the native webview when JavaScript evaluating is finished.
        /// </summary>
        private static void OnEvaluatingJavaScriptFinished(WebViewClient sender, string result)
        {
            GameObject webView = GameObject.Find(sender.Name);
            if (webView != null)
            {
#if UNIWEBVIEW3_SUPPORTED
                result = 
                    "{\"data\":\"" + 
                    WWebViewSystem.EscapeJsonText(result) + 
                    "\",\"resultCode\":\"0\",\"identifier\":\"\"}";
#endif
                webView.SendMessage("EvalJavaScriptFinished", result, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
#if !UNIWEBVIEW2_SUPPORTED && !UNIWEBVIEW3_SUPPORTED
}
#endif
#endif
