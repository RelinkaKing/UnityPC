// WWebViewDemo.cs : WWebViewDemo implementation file
//
// Description      : WWebViewDemo
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// Created          : 2017/07/12
// Last Update      : 2017/12/03
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

using UnityEngine;
using UnityEngine.UI;
using ICODES.STUDIO.WWebView;

public class WWebViewDemo : MonoBehaviour
{
    public WWebView webView = null;
    public Text status = null;
    public Text showText = null;
    public RawImage background = null;
    public Button goBack = null;
    public Button goForward = null;
    public int margine = 30;
    public string url ;//"https://www.google.com"
    public string userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0)";

    public string script =
        @"function Test()
        {
            var msg = 'Hello JavaScript';
            alert(msg);
            return msg;
        } Test();";

    public string html =
        @"<html><body>
        <p><font size='5'>Hello WWebView - This is simple code string</font></p><br>
        <p><a href='wwebview://openfile/'>wwebview://openfile/ - open local html file</a></p>
        <p><a href='wwebview://close/'>wwebview://close/</a></p>
        <p><a href='javascript:window.close();'>window.close() - works only on Win32 Webview</a></p>
        <p><a href='http://www.icodes.studio/'>http://www.icodes.studio/</a></p>
        <p><a href='https://www.microsoft.com/store/apps/9ph2smnbphnq'>Windows 10 sample</a></p>
        <p><a href='https://www.assetstore.unity3d.com/#!/content/97395'>Unity AssetStore</a></p>
        </body></html>";

    public string localHTML = "demo.html";
    protected bool showFlag = false;

    protected void Awake()
    {
        url = "file://" + Application.streamingAssetsPath + "/" + "demo.html";//"https://www.google.com"
        webView.OnStartNavigation += OnStartNavigation;
        webView.OnNavigationCompleted += OnNavigationCompleted;
        webView.OnNavigationFailed += OnNavigationFailed;
        webView.OnReceiveMessage += OnReceiveMessage;
        webView.OnEvaluateJavaScript += OnEvaluateJavaScript;
        webView.OnClose += OnClose;

#if UNIWEBVIEW3_SUPPORTED
        webView.Initialize(new Vector4(0, margine, 0, 0), new Vector2(Screen.width, Screen.height - (margine * 2)));
#else
        webView.Initialize(new Vector4(0, margine, 0, margine), new Vector2(0, 0));
#endif
    }

    protected virtual void OnStartNavigation(WWebView webView, string url)
    {
        DebugLog.DebugLogInfo("00ff00", "OnStartNavigation");
        if (status != null)
            status.text = "OnStartNavigation : " + url;
    }

    protected virtual void OnNavigationCompleted(WWebView webView, string data)
    {
        DebugLog.DebugLogInfo("00ff00", "OnNavigationCompleted");
        if (goBack != null)
            goBack.interactable = webView.CanGoBack;

        if (goForward != null)
            goForward.interactable = webView.CanGoForward;

        if (status != null)
            status.text = "OnNavigationCompleted : " + data;
    }

    protected virtual void OnNavigationFailed(WWebView webView, int code, string url)
    {
        DebugLog.DebugLogInfo("00ff00", "OnNavigationFailed");
        if (status != null)
            status.text = "OnNavigationFailed : errorcode: " + code;
    }

    protected virtual void OnReceiveMessage(WWebView webView, string message)
    {
        DebugLog.DebugLogInfo("00ff00", "OnReceiveMessage");
        if (message == "wwebview://openfile/")
            LoadLocalFile();

        else if (message == "wwebview://loadstring/")
            LoadHTML();

        if (status != null)
            status.text = "OnReceiveMessage : " + message;
    }

    protected virtual void OnEvaluateJavaScript(WWebView webView, string result)
    {
        DebugLog.DebugLogInfo("00ff00", "OnEvaluateJavaScript");
        if (status != null)
            status.text = "OnEvaluateJavaScript : " + result;
    }

    protected virtual bool OnClose(WWebView webView)
    {
        DebugLog.DebugLogInfo("00ff00", "OnClose");
        if (status != null)
            status.text = "OnClose";

#if UNITY_EDITOR
        // NOTE: Keep in mind that you are just watching the DEMO now.
        // Destroy webview instance not WWebView component.
        webView.Destroy();

        // Don't destroy WWebView component for this demo.
        // In most cases, you will return true to remove the component.
        return false; 
#else
        // In "real" game, you will never quit your app since the webview is closed.
        UnityEngine.Application.Quit();
        return true;
#endif
    }

    public void Navigate()
    {
        Navigate(url);
    }

    public void Navigate(string url)
    {
        webView.Navigate(url);
        Show();
    }

    public void LoadHTML()
    {
        webView.NavigateString(html);
        Show();
    }

    public void LoadLocalFile()
    {
        webView.NavigateFile(localHTML);
        Show();
    }

    public void Refresh()
    {
        webView.Refresh();
    }

    public void JavaScript()
    {
        webView.EvaluateJavaScript(script);
    }

    public void GoBack()
    {
        webView.GoBack();
    }

    public void GoForward()
    {
        webView.GoForward();
    }

    public void Stop()
    {
        webView.Stop();
    }

    public void UserAgent()
    {
        webView.SetUserAgent(userAgent);
        Navigate("http://whatsmyuseragent.com");
    }

    public void Texturing()
    {
#if UNITY_EDITOR_WIN || ((UNITY_STANDALONE_WIN || UNITY_WSA) && !UNITY_EDITOR)
        Navigate("https://youtu.be/ctNF6QlLBWo");
        Texture2D texture = new Texture2D(webView.GetActualWidth(), webView.GetActualHeight(), TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Trilinear; texture.Apply();
        background.texture = texture;
        webView.SetTexture(texture);
        Hide();
#else
        Debug.LogWarning("Texturing feature is only supported on Win32/WSA/Windows Editor.");
#endif
    }

    public void Transparent()
    {
        webView.Alpha = 0.5f;
    }

    public void ShowToggle()
    {
        if (showFlag)
            Hide();
        else
            Show();
    }

    public void Show()
    {
        webView.Show();
        showFlag = true;

        if (showText != null)
            showText.text = "Hide";
    }

    public void Hide()
    {
        webView.Hide();
        showFlag = false;

        if (showText != null)
            showText.text = "Show";
    }

    public void UniWebView2()
    {
        Navigate("http://www.icodes.studio/2017/10/how-to-integrate-with-uniwebview2.html");
    }

    public void UniWebView3()
    {
        Navigate("http://www.icodes.studio/2017/10/how-to-integrate-with-uniwebview3.html");
    }

    public void ReleaseNotes()
    {
        Navigate("http://www.icodes.studio/2017/10/wwebview-release-notes.html");
    }

    public void BugReporting()
    {
        Navigate("http://www.icodes.studio/2017/10/wwebview-bug-reporting.html");
    }

    public void MailMe()
    {
        Application.OpenURL("mailto:icodes.studio@gmail.com");
    }

    public void WWebViewStore()
    {
        Application.OpenURL("ms-windows-store://pdp/?productid=9ph2smnbphnq");
    }
}
