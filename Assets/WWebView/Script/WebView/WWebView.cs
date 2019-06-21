// WWebView.cs : WWebView implementation file
//
// Description      : WWebView
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// Created          : 2017/11/28
// Last Update      : 2017/12/03
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

using UnityEngine;
using System;
using System.Collections;
using System.IO;

namespace ICODES.STUDIO.WWebView
{
    /// <summary>
    /// The main class of WWebView.
    /// The GameObject with this component represent a webview object in system.
    /// </summary>
    public class WWebView : MonoBehaviour
    {
        public delegate void ActionStartNavigation(WWebView webView, string url);
        public delegate void ActionNavigationCompleted(WWebView webView, string data);
        public delegate void ActionNavigationFailed(WWebView webView, int code, string url);
        public delegate void ActionReceiveMessage(WWebView webView, string message);
        public delegate void ActionEvaluateJavaScript(WWebView webView, string result);
        public delegate bool ActionClose(WWebView webView);

        public event ActionStartNavigation OnStartNavigation;
        public event ActionNavigationCompleted OnNavigationCompleted;
        public event ActionNavigationFailed OnNavigationFailed;
        public event ActionReceiveMessage OnReceiveMessage;
        public event ActionEvaluateJavaScript OnEvaluateJavaScript;
        public event ActionClose OnClose;

        private string guid = Guid.NewGuid().ToString();
        private WWebViewListener listener;
        private bool setup = false;

        [SerializeField] private string url = null;
        [SerializeField] private Vector4 position = Vector4.zero;
        [SerializeField] private Vector2 size = Vector2.zero;

        /// <summary>
        /// If the url field is specified in the inspector, it's navigated right here
        /// </param>
        private void Start()
        {
            if (string.IsNullOrEmpty(url) == false)
                Navigate(url);
        }

        /// <summary>
        /// Destroys the associated webview instance and event listener when this component is destroyed.
        /// </param>
        private void OnDestroy()
        {
            Destroy();
        }

        /// <summary>
        /// Initialize the WWebView.
        /// </summary>
        /// <param name="position">
        /// Specifies the location information where the WebView will be displayed.
        /// In WWebView and UniWebView2 environment, this value is used as a margin.
        /// In UniWebView3 environment, this value used as the position and only x and y values are used.
        /// </param>
        /// <param name="size">
        /// The size of the webview. 
        /// In UniWebView2 environment, This value is not used.
        /// In WWebView environment, If the size value is greater than 0, the webview is displayed as a fixed size.
        /// It also becomes the reference size of the margin specified by the position value.
        /// </param>
        public void Initialize(Vector4 position, Vector2 size)
        {
            this.position = position;
            this.size = size;

            if (WWebViewSystem.Instance.HoloLensVR == false)
                Setup();
        }

        /// <summary>
        /// Setup the WWebView.
        /// </summary>
        private void Setup()
        {
            if (setup == false)
            {
                var listenerObject = new GameObject(guid);
                listener = listenerObject.AddComponent<WWebViewListener>();
                listenerObject.transform.parent = transform;
                listener.WebView = this;

#if UNIWEBVIEW3_SUPPORTED
                UniWebViewInterface.Init(listener.Name, (int)position.x, (int)position.y, (int)size.x, (int)size.y);
#elif UNIWEBVIEW2_SUPPORTED
                UniWebViewPlugin.Init(listener.Name, (int)position.y, (int)position.x, (int)position.w, (int)position.z);
#else
                WWebViewPlugin.Init(listener.Name, (int)position.y, (int)position.x, (int)position.w, (int)position.z, (int)size.x, (int)size.y);
#endif
                setup = true;
            }
        }

        /// <summary>
        /// Destroy the webview instance.
        /// This does not remove this component. Just remove the webview instance and event listener.
        /// </summary>
        public void Destroy()
        {
            if (setup == true)
            {
#if UNIWEBVIEW3_SUPPORTED
                UniWebViewInterface.Destroy(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
                UniWebViewPlugin.Destroy(listener.Name);
#else
                WWebViewPlugin.Destroy(listener.Name);
#endif
                Destroy(listener.gameObject);
                setup = false;
            }
        }

        /// <summary>
        /// Navigate a url in current webview.
        /// </summary>
        /// <param name="url">The URL to be navigated</param>
        public void Navigate(string url)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.Load(listener.Name, url, false);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.Load(listener.Name, url);
#else
            WWebViewPlugin.Load(listener.Name, url);
#endif
        }

        /// <summary>
        /// Navigate an HTML string in current webview.
        /// </summary>
        /// <param name="html">The HTML string to use as the contents of the webpage.</param>
        public void NavigateString(string html)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.LoadHTMLString(listener.Name, html, string.Empty, false);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.LoadHTMLString(listener.Name, html, string.Empty);
#else
            WWebViewPlugin.LoadHTMLString(listener.Name, html, string.Empty);
#endif
        }

        /// <summary>
        /// Navigate a local file in current webview.
        /// </summary>
        /// <param name="file">Full path file name to be navigated.</param>
        public void NavigateFile(string file)
        {
            Setup();

            string fullPath =
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_IOS
            Path.Combine("file://" + Application.streamingAssetsPath, file);
#elif UNITY_WSA
            "ms-appx-web:///Data/StreamingAssets/" + file;
#elif UNITY_ANDROID
            Path.Combine("file:///android_asset/", file);
#else
            string.Empty;
#endif

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.Load(listener.Name, fullPath, false);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.Load(listener.Name, fullPath);
#else
            WWebViewPlugin.Load(listener.Name, fullPath);
#endif
        }

        /// <summary>
        /// Set the header field.
        /// </summary>
        /// <param name="key">Key of the header field.</param>
        /// <param name="value">Value of the header field.</param>
        public void SetHeaderField(string key, string value)
        {
#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SetHeaderField(listener.Name, key, value);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.SetHeaderField(listener.Name, key, value);
#else
            WWebViewPlugin.SetHeaderField(listener.Name, key, value);
#endif
        }

#if UNIWEBVIEW3_SUPPORTED
        /// <summary>
        /// Changes the position and dimensions of current webview.
        /// </summary>
        /// <param name="left">The new position of the left side of the webview.</param>
        /// <param name="top">The new position of the top side of the webview.</param>
        /// <param name="width">The new width of the webview.</param>
        /// <param name="height">The new height of the webview.</param>
        public void SetWindowLayout(int left, int top, int width, int height)
#else
        /// <summary>
        /// Changes the position and dimensions of current webview.
        /// If the width or height value is greater than 0, the webview is displayed as a fixed size.
        /// It also becomes the reference size of the margin specified by left, top, right and bottom value.
        /// </summary>
        /// <param name="left">The new value of the left margin of the webview.</param>
        /// <param name="top">The new value of the top margin of the webview.</param>
        /// <param name="right">The new value of the right margin of the webview.</param>
        /// <param name="bottom">The new value of the bottom margin of the webview.</param>
        /// <param name="width">The new width of the webview.</param>
        /// <param name="height">The new height of the webview.</param>
        public void SetWindowLayout(int left, int top, int right, int bottom, int width, int height)
#endif
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SetFrame(listener.Name, left, top, width, height);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.ChangeInsets(listener.Name, top, left, bottom, right);
#else
            WWebViewPlugin.ChangeInsets(listener.Name, top, left, bottom, right, width, height);
#endif
        }

        /// <summary>
        /// Adds a url scheme to WWebview message interpreter.
        /// This scheme will be sent as a message to WWebView instead.
        /// </summary>
        /// <param name="scheme">
        /// The url scheme to add. It should not contain "://" part. 'wwebview' is added by default.
        /// </param>
        public void AddUrlScheme(string scheme)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.AddUrlScheme(listener.Name, scheme);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.AddUrlScheme(listener.Name, scheme);
#else
            WWebViewPlugin.AddUrlScheme(listener.Name, scheme);
#endif
        }

        /// <summary>
        /// Remove a url scheme from WWebView message interpreter.
        /// </summary>
        /// <param name="scheme">
        /// The url scheme to remvoe.
        /// </param>
        public void RemoveUrlScheme(string scheme)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.RemoveUrlScheme(listener.Name, scheme);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.RemoveUrlScheme(listener.Name, scheme);
#else
            WWebViewPlugin.RemoveUrlScheme(listener.Name, scheme);
#endif
        }

        /// <summary>
        /// Sets the user-agent used in the webview.
        /// </summary>
        /// <param name="userAgent">
        /// The new user-agent string to use.
        /// eg. Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0)
        /// </param>
        public void SetUserAgent(string userAgent)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SetUserAgent(listener.Name, userAgent);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.SetUserAgent(userAgent);
#else
            WWebViewPlugin.SetUserAgent(userAgent);
#endif
        }

        /// <summary>
        /// Gets the user-agent string currently used in webview.
        /// If a customized user-agent is not set, the default user-agent in current platform will be returned.
        /// </summary>
        /// <returns>The user-agent string in use.</returns>
        public string GetUserAgent()
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            return UniWebViewInterface.GetUserAgent(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
            return UniWebViewPlugin.GetUserAgent(listener.Name);
#else
            return WWebViewPlugin.GetUserAgent(listener.Name);
#endif
        }

        /// <summary>
        /// Clears all caches. This will removes cached local data of webview.
        /// </summary>
        public void ClearCache()
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.CleanCache(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.CleanCache(listener.Name);
#else
            WWebViewPlugin.CleanCache(listener.Name);
#endif
        }

        /// <summary>
        /// Clears all cookies from webview.
        /// </summary>
        public void ClearCookies()
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.ClearCookies();
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.CleanCookie(listener.name, string.Empty);
#else
            WWebViewPlugin.ClearCookies();
#endif
        }

        /// <summary>
        /// Sets a cookie for a certain url.
        /// </summary>
        /// <param name="url">The url to which cookie will be set.</param>
        /// <param name="cookie">The cookie string to set.</param>
        public void SetCookie(string url, string cookie)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SetCookie(url, cookie, false);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.SetCookie(url, cookie);
#else
            WWebViewPlugin.SetCookie(url, cookie);
#endif
        }

        /// <summary>
        /// Gets the cookie value under a url and key.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetCookie(string url, string key)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            return UniWebViewInterface.GetCookie(url, key, false);
#elif UNIWEBVIEW2_SUPPORTED
            return UniWebViewPlugin.GetCookie(url, key);
#else
            return WWebViewPlugin.GetCookie(url, key);
#endif
        }

        /// <summary>
        /// Sets the visibility of all(both) scroll bar for the webview.
        /// </summary>
        /// <param name="show">Whether the scroll bar should be visible or not</param>
        public void ShowScroll(bool show)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SetHorizontalScrollBarEnabled(listener.Name, show);
            UniWebViewInterface.SetVerticalScrollBarEnabled(listener.Name, show);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.SetHorizontalScrollBarShow(listener.Name, show);
#else
            WWebViewPlugin.ShowScroll(listener.Name, show);
#endif
        }

        /// <summary>
        /// Sets the visibility of horizontal scroll bar for the webview.
        /// </summary>
        /// <param name="show">Whether the horizontal scroll bar should be visible or not</param>
        public void ShowScrollX(bool show)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SetHorizontalScrollBarEnabled(listener.Name, show);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.SetHorizontalScrollBarShow(listener.Name, show);
#else
            WWebViewPlugin.ShowScrollX(listener.Name, show);
#endif
        }

        /// <summary>
        /// Sets the visibility of vertical scroll bar for the webview.
        /// </summary>
        /// <param name="show">Whether the vertical scroll bar should be visible or not</param>
        public void ShowScrollY(string name, bool show)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SetVerticalScrollBarEnabled(listener.Name, show);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.SetVerticalScrollBarShow(listener.Name, show);
#else
            WWebViewPlugin.ShowScrollY(listener.Name, show);
#endif
        }

        /// <summary>
        /// Reload current page.
        /// </summary>
        public void Refresh()
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.Reload(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.Reload(listener.Name);
#else
            WWebViewPlugin.Reload(listener.Name);
#endif
        }

        /// <summary>
        /// Add some javascript to the web page.
        /// </summary>
        /// <param name="script">Some javascript code you want to add to the page.</param>
        public void AddJavaScript(string script)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.AddJavaScript(listener.Name, script, string.Empty);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.AddJavaScript(listener.Name, script);
#else
            WWebViewPlugin.AddJavaScript(listener.Name, script);
#endif
        }

        /// <summary>
        /// Evaluate a JavaScript string on current page.
        /// Although you can write complex javascript code and evaluate it at once, 
        /// a suggest way is calling this method with a single js method name.
        /// The webview will try evaluate(execute) the javascript. 
        /// When it finished, OnEvaluateJavaScript will be raised with the result.
        /// </summary>
        /// <param name="script">The JavaScript string to evaluate.</param>
        public void EvaluateJavaScript(string script)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.EvaluateJavaScript(listener.Name, script, string.Empty);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.EvaluatingJavaScript(listener.Name, script);
#else
            WWebViewPlugin.EvaluatingJavaScript(listener.Name, script);
#endif
        }

        /// <summary>
        /// Gets and Sets alpha-blending value in current webview window.
        /// </summary>
        public float Alpha
        {
            get
            {
                Setup();

#if UNIWEBVIEW3_SUPPORTED
                return UniWebViewInterface.GetWebViewAlpha(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
                return UniWebViewPlugin.GetAlpha(listener.Name);
#else
                return WWebViewPlugin.GetAlpha(listener.Name);
#endif
            }
            set
            {
                Setup();

#if UNIWEBVIEW3_SUPPORTED
                UniWebViewInterface.SetWebViewAlpha(listener.Name, value);
#elif UNIWEBVIEW2_SUPPORTED
                UniWebViewPlugin.SetAlpha(listener.Name, value);
#else
                WWebViewPlugin.SetAlpha(listener.Name, value);
#endif
            }
        }

        /// <summary>
        /// Gets a value indicating whether a previous page in navigation history is available, 
        /// which allows the GoBack method to succeed.
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                Setup();

#if UNIWEBVIEW3_SUPPORTED
                return UniWebViewInterface.CanGoBack(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
                return UniWebViewPlugin.CanGoBack(listener.Name);
#else
                return WWebViewPlugin.CanGoBack(listener.Name);
#endif
            }
        }

        /// <summary>
        /// Gets a value indicating whether a subsequent page in navigation history is available, 
        /// which allows the GoForward method to succeed.
        /// </summary>
        public bool CanGoForward
        {
            get
            {
                Setup();

#if UNIWEBVIEW3_SUPPORTED
                return UniWebViewInterface.CanGoForward(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
                return UniWebViewPlugin.CanGoForward(listener.Name);
#else
                return WWebViewPlugin.CanGoForward(listener.Name);
#endif
            }
        }

        /// <summary>
        /// Navigates the webview to the previous page in the navigation history, if one is available.
        /// </summary>
        public void GoBack()
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.GoBack(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.GoBack(listener.Name);
#else
            WWebViewPlugin.GoBack(listener.Name);
#endif
        }

        /// <summary>
        /// Navigates the webview to the next page in the navigation history, if one is available.
        /// </summary>
        public void GoForward()
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.GoForward(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.GoForward(listener.Name);
#else
            WWebViewPlugin.GoForward(listener.Name);
#endif
        }

        /// <summary>
        /// Cancels any pending navigation and stops any dynamic page elements, such as background sounds and animations.
        /// </summary>
        public void Stop()
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.Stop(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.Stop(listener.Name);
#else
            WWebViewPlugin.Stop(listener.Name);
#endif
        }

        /// <summary>
        /// Shows the webview window.
        /// </summary>
        public void Show()
        {
#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.Show(listener.Name, false, 0, 0f, string.Empty);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.Show(listener.Name, false, 0, 0f);
#else
            WWebViewPlugin.Show(listener.Name, false, 0, 0f);
#endif
        }

        /// <summary>
        /// Hides the webview window.
        /// </summary>
        public void Hide()
        {
#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.Hide(listener.Name, false, 0, 0f, string.Empty);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.Hide(listener.Name, false, 0, 0f);
#else
            WWebViewPlugin.Hide(listener.Name, false, 0, 0f);
#endif
        }

#if UNITY_EDITOR_WIN || ((UNITY_STANDALONE_WIN || UNITY_WSA) && !UNITY_EDITOR)
        /// <summary>
        /// Real-time rendering of webview content to the specified texture.
        /// </summary>
        /// <param name="texture">
        /// Texture in which the content of the webview will be rendered
        /// </param>
        public void SetTexture(Texture2D texture)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SetTexture(listener.Name, texture);
            StartCoroutine("OnRenderTexture", UniWebViewInterface.GetRenderEventFunc());
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.SetTexture(listener.Name, texture);
            StartCoroutine("OnRenderTexture", UniWebViewPlugin.GetRenderEventFunc());
#else
            WWebViewPlugin.SetTexture(listener.Name, texture);
            StartCoroutine("OnRenderTexture", WWebViewPlugin.GetRenderEventFunc());
#endif
        }

        /// <summary>
        /// Sends the real-time rendering event to a native plugin of webview.
        /// </summary>
        /// <param name="handler">
        /// the render event handler function of the webview plugin.
        /// </param>
        private IEnumerator OnRenderTexture(IntPtr handler)
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                GL.IssuePluginEvent(handler, 0);
            }
        }

        /// <summary>
        /// Process the input event.
        /// </summary>
        /// <param name="state">
        /// Specifies the input status value.
        ///  0: mouse moving.
        ///  1: mouse button down.
        ///  2: mouse button up.
        ///  3: mouse wheel
        /// </param>
        /// <param name="key">
        /// Specifies which key handles the input.
        ///   0: left mouse button (in case of state 1 and 2)
        ///   1: right mouse button (in case of state 1 and 2)
        ///   2: middle mouse button (in case of state 1 and 2)
        ///   delta value: in case of state 3
        /// </param>
        /// <param name="x">The current x coordinate value of the mouse pointer.</param>
        /// <param name="y">The current x coordinate value of the mouse pointer.</param>
        public void InputEvent(int state, int key, int x, int y)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.InputEvent(listener.Name, state, key, x, y);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.InputEvent(listener.Name, state, key, x, y);
#else
            WWebViewPlugin.InputEvent(listener.Name, state, key, x, y);
#endif
        }

        /// <summary>
        /// Get the webview's width, as a value in device-independent units (1/96th inch per unit).
        /// The default is 0. The default might be encountered if the webview has not been loaded 
        /// and hasn't yet been involved in a layout pass that renders the UI.
        /// </summary>
        public int GetActualWidth()
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            return UniWebViewInterface.GetActualWidth(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
            return UniWebViewPlugin.GetActualWidth(listener.Name);
#else
            return WWebViewPlugin.GetActualWidth(listener.Name);
#endif
        }

        /// <summary>
        /// Get the webview's height, as a value in device-independent units (1/96th inch per unit).
        /// The default is 0. The default might be encountered if the webview has not been loaded 
        /// and hasn't yet been involved in a layout pass that renders the UI.
        /// </summary>
        public int GetActualHeight()
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            return UniWebViewInterface.GetActualHeight(listener.Name);
#elif UNIWEBVIEW2_SUPPORTED
            return UniWebViewPlugin.GetActualHeight(listener.Name);
#else
            return WWebViewPlugin.GetActualHeight(listener.Name);
#endif
        }

        /// <summary>
        /// Specifies a value indicating whether to enable the context menu, which appears when the right mouse button is clicked.
        /// This feature is only supported in Windows Standalone.
        /// </summary>
        /// <param name="enable">Indicates whether to enable or disable the webview.</param>
        public void EnableContextMenu(bool enable)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.EnableContextMenu(listener.Name, enable);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.EnableContextMenu(listener.Name, enable);
#else
            WWebViewPlugin.EnableContextMenu(listener.Name, enable);
#endif
        }

        /// <summary>
        /// Specifies the zoom factor of the webview.
        /// This feature is only supported in Windows Standalone.
        /// </summary>
        /// <param name="factor">
        /// Specify how much to zoom in or out as a percentage value.
        /// </param>
        public void SetZoom(int factor)
        {
            Setup();

#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SetZoom(listener.Name, factor);
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.SetZoom(listener.Name, factor);
#else
            WWebViewPlugin.SetZoom(listener.Name, factor);
#endif
        }
#endif

        /// <summary>
        /// Called from 'WWebViewListener' when navigation started.
        /// </summary>
        internal void InternalOnStartNavigation(string url)
        {
            if (OnStartNavigation != null)
                OnStartNavigation(this, url);
        }

        /// <summary>
        /// Called from 'WWebViewListener' when navigation completed.
        /// </summary>
        internal void InternalOnNavigationCompleted(string data)
        {
            if (OnNavigationCompleted != null)
                OnNavigationCompleted(this, data);
        }

        /// <summary>
        /// Called from 'WWebViewListener' when navigation failed.
        /// </summary>
        internal void InternalOnNavigationFailed(int code, string url)
        {
            if (OnNavigationFailed != null)
                OnNavigationFailed(this, code, url);
        }

        /// <summary>
        /// Called from 'WWebViewListener' when the url scheme message received.
        /// </summary>
        internal void InternalOnReceiveMessage(string message)
        {
            if (OnReceiveMessage != null)
                OnReceiveMessage(this, message);
        }

        /// <summary>
        /// Called from 'WWebViewListener' when JavaScript evaluating finished.
        /// </summary>
        internal void InternalOnEvaluateJavaScript(string result)
        {
            if (OnEvaluateJavaScript != null)
                OnEvaluateJavaScript(this, result);
        }

        /// <summary>
        /// Called from 'WWebViewListener' when the webview need to be closed.
        /// </summary>
        internal void InternalOnClose()
        {
            if (OnClose != null)
            {
                if (OnClose(this) == true)
                    Destroy(this);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}

