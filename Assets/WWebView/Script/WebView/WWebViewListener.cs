// WWebViewListener.cs : WWebViewListener implementation file
//
// Description      : WWebViewListener
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

namespace ICODES.STUDIO.WWebView
{
    /// <summary>
    /// A listener class for message sent from the plugin wrapper modules.
    /// Normally this component will be attached to a sub 'GameObject' under the 'WWebView' one.
    /// It will be added automatically and destroyed as needed. So there is rarely a need for you 
    /// to manipulate on this class.
    /// </summary>
    public class WWebViewListener : MonoBehaviour
    {
        /// <summary>
        /// A payload received from the plugin wrapper module. It contains information to identify the message sender,
        /// as well as some necessary field to bring data from native side to Unity.
        /// </summary>
        [Serializable] public class WWebViewResultPayload
        {
            public string identifier;
            public string resultCode;
            public string data;
        }

        /// <summary>
        /// The webview holder of this listener.
        /// It will be linked to original webview so you should never set it yourself.
        /// </summary>
        public WWebView WebView
        {
            get; set;
        }

        /// <summary>
        /// Name of current listener. 
        /// This is a GUID string by which native side could use to find the message destination.
        /// </summary>
        public string Name
        {
            get { return gameObject.name; }
        }

        /// <summary>
        /// Called from the plugin module when the webview need to be closed.
        /// </summary>
        private void WebViewDone(string message)
        {
            WebView.InternalOnClose();
        }

        /// <summary>
        /// Called from the plugin module when navigation is started.
        /// </summary>
#if UNIWEBVIEW3_SUPPORTED
        private void PageStarted(string url)
#else
        private void LoadBegin(string url)
#endif
        {
            WebView.InternalOnStartNavigation(url);
        }

        /// <summary>
        /// Called from the plugin module when navigation is completed.
        /// </summary>
#if UNIWEBVIEW3_SUPPORTED
        private void PageFinished(string result)
        {
            var payload = JsonUtility.FromJson<WWebViewResultPayload>(result);
            WebView.InternalOnNavigationCompleted(payload.data);
        }
#else
        private void LoadComplete(string message)
        {
            WebView.InternalOnNavigationCompleted(message);
        }
#endif

        /// <summary>
        /// Called from the plugin module when JavaScript evaluating is finished.
        /// </summary>
        private void EvalJavaScriptFinished(string result)
        {
#if UNIWEBVIEW3_SUPPORTED
            var payload = JsonUtility.FromJson<WWebViewResultPayload>(result);
            result = payload.data;
#endif
            WebView.InternalOnEvaluateJavaScript(result);
        }

        /// <summary>
        /// Called from the plugin module when the url scheme message is received.
        /// </summary>
#if UNIWEBVIEW3_SUPPORTED
        private void MessageReceived(string result)
#else
        private void ReceivedMessage(string result)
#endif
        {
            WebView.InternalOnReceiveMessage(result);
        }

#if UNIWEBVIEW3_SUPPORTED
        /// <summary>
        /// Called from the plugin module when navigation failed.
        /// </summary>
        private void PageErrorReceived(string result)
        {
            var payload = JsonUtility.FromJson<WWebViewResultPayload>(result);

            int code = 0;
            int.TryParse(payload.resultCode, out code);
            WebView.InternalOnNavigationFailed(code, payload.data);
        }

        private void ShowTransitionFinished(string identifer)
        {
            // N/A
        }

        private void HideTransitionFinished(string identifer)
        {
            // N/A
        }

        private void AnimateToFinished(string identifer)
        {
            // N/A
        }

        private void AddJavaScriptFinished(string result)
        {
            // N/A
        }

        private void WebViewKeyDown(string keyCode)
        {
            // N/A
        }
#elif UNIWEBVIEW2_SUPPORTED
        private void WebViewKeyDown(string message)
        {
            // N/A
        }

        private void AnimationFinished(string identifier)
        {
            // N/A
        }

        private void ShowTransitionFinished(string message)
        {
            // N/A
        }

        private void HideTransitionFinished(string message)
        {
            // N/A
        }
#endif
    }
}
