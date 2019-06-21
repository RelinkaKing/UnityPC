// WWebViewPlugin.cs : WWebViewPlugin implementation file
//
// Description      : WWebViewPlugin
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// Created          : 2017/07/12
// Last Update      : 2017/12/03
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

#if UNITY_EDITOR_OSX || (!UNITY_EDITOR_WIN && !UNITY_STANDALONE_WIN && !UNITY_WSA)

using UnityEngine;
using System;
using ICODES.STUDIO.WWebView;

#if !UNIWEBVIEW2_SUPPORTED && !UNIWEBVIEW3_SUPPORTED
namespace ICODES.STUDIO.WWebView
{
#endif
    /// <summary>
    /// Placeholder class that is activated when no platform is supported.
    /// </summary>
    public sealed class WWebViewPlugin
    {
        /// <summary>
        /// Displays a simple warning message if not supported
        /// </summary>
        private static void LogWarning()
        {
            Debug.LogWarning("WWebView only supports Win32/WSA/Windows Editor. Your current platform " + Application.platform + " is not supported.");
        }

        /// <summary>
        /// Initialize the WWebView.
        /// </summary>
        public static bool Init(string name, int top, int left, int bottom, int right, int width, int height)
        {
            LogWarning();
            return false;
        }

        /// <summary>
        /// Initialize the WWebView.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static bool Init(string name, int top, int left, int bottom, int right)
        {
            LogWarning();
            return false;
        }

        /// <summary>
        /// Release the WWebView.
        /// </summary>
        public static void Release()
        {
            LogWarning();
        }

        /// <summary>
        /// Changes the position and dimensions of current webview.
        /// </summary>
        public static void ChangeInsets(string name, int top, int left, int bottom, int right, int width, int height)
        {
            LogWarning();
        }

        /// <summary>
        /// UniWebView2 interface.
        /// Changes the position and dimensions of current webview.
        /// </summary>
        public static void ChangeInsets(string name, int top, int left, int bottom, int right)
        {
            LogWarning();
        }

        /// <summary>
        /// UniWebView3 interface.
        /// Changes the position and dimensions of current webview.
        /// </summary>
        public static void SetFrame(string name, int x, int y, int width, int height)
        {
            LogWarning();
        }

        /// <summary>
        /// UniWebView3 interface.
        /// Changes the position of current webview.
        /// </summary>
        public static void SetPosition(string name, int x, int y)
        {
            LogWarning();
        }

        /// <summary>
        /// UniWebView3 interface.
        /// Changes the size of current webview.
        /// </summary>
        public static void SetSize(string name, int width, int height)
        {
            LogWarning();
        }

        /// <summary>
        /// Specifies a value indicating whether to enable the context menu, which appears when the right mouse button is clicked.
        /// </summary>
        public static void EnableContextMenu(string name, bool enable)
        {
            LogWarning();
        }

        /// <summary>
        /// Real-time rendering of webview content to the specified texture.
        /// </summary>
        public static void SetTexture(string name, Texture texture)
        {
            LogWarning();
        }

        /// <summary>
        /// Specifies the zoom factor of the webview.
        /// </summary>
        public static void SetZoom(string name, int factor)
        {
            LogWarning();
        }

        /// <summary>
        /// Retrieves the render event handler function of the webview plugin.
        /// </summary>
        public static IntPtr GetRenderEventFunc()
        {
            LogWarning();
            return IntPtr.Zero;
        }

        /// <summary>
        /// Sets the visibility of all(both) scroll bar for the webview.
        /// </summary>
        public static void ShowScroll(string name, bool show)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets the visibility of horizontal scroll bar for the webview.
        /// </summary>
        public static void ShowScrollX(string name, bool show)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets the visibility of horizontal scroll bar for the webview.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetHorizontalScrollBarShow(string name, bool show)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets the visibility of horizontal scroll bar for the webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetHorizontalScrollBarEnabled(string name, bool enabled)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets the visibility of vertical scroll bar for the webview.
        /// </summary>
        public static void ShowScrollY(string name, bool show)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets the visibility of vertical scroll bar for the webview.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetVerticalScrollBarShow(string name, bool show)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets the visibility of vertical scroll bar for the webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetVerticalScrollBarEnabled(string name, bool enabled)
        {
            LogWarning();
        }

        /// <summary>
        /// Adds a url scheme to WWebview message interpreter.
        /// </summary>
        public static void AddUrlScheme(string name, string scheme)
        {
            LogWarning();
        }

        /// <summary>
        /// Remove a url scheme from WWebView message interpreter.
        /// </summary>
        public static void RemoveUrlScheme(string name, string scheme)
        {
            LogWarning();
        }

        /// <summary>
        /// Navigate a url in current webview.
        /// </summary>
        public static void Load(string name, string url)
        {
            LogWarning();
        }

        /// <summary>
        /// Navigate a url in current webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void Load(string name, string url, bool skipEncoding)
        {
            LogWarning();
        }

        /// <summary>
        /// Navigate an HTML string in current webview.
        /// </summary>
        public static void LoadHTMLString(string name, string html, string baseUrl)
        {
            LogWarning();
        }

        /// <summary>
        /// Navigate an HTML string in current webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void LoadHTMLString(string name, string html, string baseUrl, bool skipEncoding)
        {
            LogWarning();
        }

        /// <summary>
        /// Reload current page.
        /// </summary>
        public static void Reload(string name)
        {
            LogWarning();
        }

        /// <summary>
        /// Add some javascript to the web page.
        /// </summary>
        public static void AddJavaScript(string name, string script)
        {
            LogWarning();
        }

        /// <summary>
        /// Add some javascript to the web page.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void AddJavaScript(string name, string script, string identifier)
        {
            LogWarning();
        }

        /// <summary>
        /// Evaluate a JavaScript string on current page.
        /// </summary>
        public static void EvaluatingJavaScript(string name, string script)
        {
            LogWarning();
        }

        /// <summary>
        /// Evaluate a JavaScript string on current page.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void EvaluateJavaScript(string name, string script, string identifier)
        {
            LogWarning();
        }

        /// <summary>
        /// Clears all caches. This will removes cached local data of webview.
        /// </summary>
        public static void CleanCache(string name)
        {
            LogWarning();
        }

        /// <summary>
        /// Clears all cookies from webview.
        /// </summary>
        public static void CleanCookie(string name, string key)
        {
            LogWarning();
        }

        /// <summary>
        /// Clears all cookies from webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void ClearCookies()
        {
            LogWarning();
        }

        /// <summary>
        /// Gets the cookie value under a url and key.
        /// </summary>
        public static string GetCookie(string url, string key)
        {
            LogWarning();
            return string.Empty;
        }

        /// <summary>
        /// Gets the cookie value under a url and key.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static string GetCookie(string url, string key, bool skipEncoding)
        {
            LogWarning();
            return string.Empty;
        }

        /// <summary>
        /// Sets a cookie for a certain url.
        /// </summary>
        public static void SetCookie(string url, string cookie)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets a cookie for a certain url.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetCookie(string url, string cookie, bool skipEncoding)
        {
            LogWarning();
        }

        /// <summary>
        /// Destroy the webview instance.
        /// </summary>
        public static void Destroy(string name)
        {
            LogWarning();
        }

        /// <summary>
        /// Navigates the webview to the previous page in the navigation history, if one is available.
        /// </summary>
        public static void GoBack(string name)
        {
            LogWarning();
        }

        /// <summary>
        /// Navigates the webview to the next page in the navigation history, if one is available.
        /// </summary>
        public static void GoForward(string name)
        {
            LogWarning();
        }

        /// <summary>
        /// Cancels any pending navigation and stops any dynamic page elements, such as background sounds and animations.
        /// </summary>
        public static void Stop(string name)
        {
            LogWarning();
        }

        /// <summary>
        /// Retrieves current navigating url.
        /// </summary>
        public static string GetCurrentUrl(string name)
        {
            LogWarning();
            return string.Empty;
        }

        /// <summary>
        /// Retrieves current navigating url.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static string GetUrl(string name)
        {
            LogWarning();
            return string.Empty;
        }

        /// <summary>
        /// Shows the webview window.
        /// </summary>
        public static bool Show(string name, bool fade, int direction, float duration)
        {
            LogWarning();
            return false;
        }

        /// <summary>
        /// Shows the webview window.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static bool Show(string name, bool fade, int edge, float duration, string identifier)
        {
            LogWarning();
            return false;
        }

        /// <summary>
        /// Hides the webview window.
        /// </summary>
        public static bool Hide(string name, bool fade, int direction, float duration)
        {
            LogWarning();
            return false;
        }

        /// <summary>
        /// Hides the webview window.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static bool Hide(string name, bool fade, int edge, float duration, string identifier)
        {
            LogWarning();
            return false;
        }

        /// <summary>
        /// Sets a value indicating whether this webview can be zoomed or not.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetZoomEnable(string name, bool enable)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets a value indicating whether this webview can be zoomed or not.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetZoomEnabled(string name, bool enabled)
        {
            LogWarning();
        }

        /// <summary>
        /// Gets the user-agent string currently used in webview.
        /// If a customized user-agent is not set, the default user-agent in current platform will be returned.
        /// </summary>
        public static string GetUserAgent(string name)
        {
            LogWarning();
            return string.Empty;
        }

        /// <summary>
        /// Sets the user-agent used in the webview.
        /// </summary>
        public static void SetUserAgent(string userAgent)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets the user-agent used in the webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetUserAgent(string name, string userAgent)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets the background of webview to transparent.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void TransparentBackground(string name, bool transparent)
        {
            LogWarning();
        }

        /// <summary>
        /// Gets alpha-blending value in current webview window.
        /// </summary>
        public static float GetAlpha(string name)
        {
            LogWarning();
            return 1f;
        }

        /// <summary>
        /// Gets alpha-blending value in current webview window.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static float GetWebViewAlpha(string name)
        {
            LogWarning();
            return 0f;
        }

        /// <summary>
        /// Sets alpha-blending value in current webview window.
        /// </summary>
        public static void SetAlpha(string name, float alpha)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets alpha-blending value in current webview window.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetWebViewAlpha(string name, float alpha)
        {
            LogWarning();
        }

        /// <summary>
        /// Get the webview's width, as a value in device-independent units (1/96th inch per unit).
        /// The default is 0. The default might be encountered if the webview has not been loaded 
        /// and hasn't yet been involved in a layout pass that renders the UI.
        /// </summary>
        public static int GetActualWidth(string name)
        {
            LogWarning();
            return 0;
        }

        /// <summary>
        /// Get the webview's height, as a value in device-independent units (1/96th inch per unit).
        /// The default is 0. The default might be encountered if the webview has not been loaded 
        /// and hasn't yet been involved in a layout pass that renders the UI.
        /// </summary>
        public static int GetActualHeight(string name)
        {
            LogWarning();
            return 0;
        }

        /// <summary>
        /// Gets a value indicating whether a previous page in navigation history is available, 
        /// which allows the GoBack method to succeed.
        /// </summary>
        public static bool CanGoBack(string name)
        {
            LogWarning();
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether a subsequent page in navigation history is available, 
        /// which allows the GoForward method to succeed.
        /// </summary>
        public static bool CanGoForward(string name)
        {
            LogWarning();
            return false;
        }

        /// <summary>
        /// Set if a default spinner should show when loading the webpage.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetSpinnerShowWhenLoading(string name, bool show)
        {
            LogWarning();
        }

        /// <summary>
        /// Set if a default spinner should show when loading the webpage.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetShowSpinnerWhileLoading(string name, bool show)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets the label text for the spinner showing when webview loading.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetSpinnerText(string name, string text)
        {
            LogWarning();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this webview can bounces or not.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetBounces(string name, bool enable)
        {
            LogWarning();
        }

        /// <summary>
        /// Retrieves the screen scale.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static int ScreenScale()
        {
            LogWarning();
            return 1;
        }

        /// <summary>
        /// Retrieves the unique id of the webview.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static int GetId(string name)
        {
            LogWarning();
            return 0;
        }

        /// <summary>
        /// Process the input event.
        /// </summary>
        public static void InputEvent(string name, int state, int key, int x, int y)
        {
            LogWarning();
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
            LogWarning();
        }

        /// <summary>
        /// Whether the new page should be opened in an external browser when user clicks a link. 
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static bool GetOpenLinksInExternalBrowser(string name)
        {
            LogWarning();
            return false;
        }

        /// <summary>
        /// Whether the new page should be opened in an external browser when user clicks a link. 
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetOpenLinksInExternalBrowser(string name, bool value)
        {
            LogWarning();
        }

        /// <summary>
        /// Set the background of webview to transparent.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetBackgroundColor(string name, float r, float g, float b, float a)
        {
            LogWarning();
        }

        /// <summary>
        /// Set the header field.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetHeaderField(string name, string key, string value)
        {
            LogWarning();
        }

        /// <summary>
        /// Set whether the video in HTML page should be auto played or not.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetAllowAutoPlay(string name, bool value)
        {
            LogWarning();
        }

        /// <summary>
        /// Set whether the video in HTML page should be auto played or not.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetAllowAutoPlay(bool flag)
        {
            LogWarning();
        }

        /// <summary>
        /// Set allow inline play for current webview.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetAllowInlinePlay(bool flag)
        {
            LogWarning();
        }

        /// <summary>
        /// Set all web views allowing third party cookies set from a non-main frame or other third party web pages.
        /// Compatibility Level: UniWebView2
        /// </summary>
        public static void SetAllowThirdPartyCookies(bool allowed)
        {
            LogWarning();
        }

        /// <summary>
        /// Select log level.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetLogLevel(int level)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets whether JavaScript can open windows without user interaction.
        /// </summary>
        public static void SetAllowJavaScriptOpenWindow(bool flag)
        {
            LogWarning();
        }

        /// <summary>
        /// Animates the webview from current position and size to another position and size.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static bool AnimateTo(string name, int x, int y, int width, int height, float duration, float delay, string identifier)
        {
            LogWarning();
            return false;
        }

        /// <summary>
        /// Adds a domain to the SSL checking white list.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void AddSslExceptionDomain(string name, string domain)
        {
            LogWarning();
        }

        /// <summary>
        /// Removes a domain from the SSL checking white list.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void RemoveSslExceptionDomain(string name, string domain)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets whether JavaScript should be enabled in current webview. Default is enabled.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetJavaScriptEnabled(bool flag)
        {
            LogWarning();
        }

        /// <summary>
        /// Clears any saved credentials for HTTP authentication for both Basic and Digest.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void ClearHttpAuthUsernamePassword(string host, string realm)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets whether the webview should show with a bounces effect when scrolling to page edge.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetBouncesEnabled(string name, bool enabled)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets whether to show a toolbar which contains navigation buttons and Done button.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetShowToolbar(string name, bool show, bool animated, bool onTop, bool adjustInset)
        {
            LogWarning();
        }

        /// <summary>
        /// Sets the done button text in toolbar.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetToolbarDoneButtonText(string name, string text)
        {
            LogWarning();
        }

        /// <summary>
        /// Enables debugging of web contents.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void SetWebContentsDebuggingEnabled(bool enabled)
        {
            LogWarning();
        }

        /// <summary>
        /// Prints current page.
        /// Compatibility Level: UniWebView3
        /// </summary>
        public static void Print(string name)
        {
            LogWarning();
        }
    }
#if !UNIWEBVIEW2_SUPPORTED && !UNIWEBVIEW3_SUPPORTED
}
#endif
#endif
