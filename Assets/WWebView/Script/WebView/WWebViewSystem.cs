// WWebViewSystem.cs : WWebViewSystem implementation file
//
// Description      : WWebViewSystem
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// Created          : 2017/07/22
// Last Update      : 2017/12/03
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

using UnityEngine;
using System;
using System.Collections;

#if UNITY_STANDALONE_WIN
using Win32;
#endif

#if UNITY_WSA
using ICODES.STUDIO.WSA;
using ICODES.STUDIO.WSA.Controls;
#endif

namespace ICODES.STUDIO.WWebView
{
    public sealed class WWebViewSystem : MonoBehaviour
    {
        public void Initialize()
        {
#if UNITY_EDITOR_WIN
            InitializeWinEditor();
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            InitializeWinPlayer();
#elif UNITY_WSA && !UNITY_EDITOR
            InitializeWSA();
#else
            Debug.Assert(false);
#endif
        }

#if UNITY_EDITOR_WIN
        private void InitializeWinEditor()
        {
            StartCoroutine("DispatchMessage");
        }
#endif

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        private void InitializeWinPlayer()
        {
            WWebViewWin32.ModifyStyle(WWebViewWin32.FindUnityPlayerWindow(), 0, PARAMS.WS_CLIPCHILDREN, 0);
            WWebViewWin32.SubclassWindow();
            StartCoroutine("DispatchMessage");
        }
#endif

#if UNITY_WSA
        private bool holoLensVR = false;
        private void InitializeWSA()
        {
            holoLensVR = HoloLensVR;

            if (Dispatcher.InvokeOnAppThread == null)
                Dispatcher.InvokeOnAppThread = InvokeOnAppThread;

            if (Dispatcher.InvokeOnUIThread == null)
                Dispatcher.InvokeOnUIThread = InvokeOnUIThread;
        }

        private void InvokeOnAppThread(Action callback, bool wait)
        {
            wait =
#if UNITY_2017_2_OR_NEWER
                false;
#else
                holoLensVR ? false : wait;
#endif
            UnityEngine.WSA.Application.InvokeOnAppThread(() =>
            {
                callback();
            },
            wait);
        }

        private void InvokeOnUIThread(Action callback, bool wait)
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                callback();
            },
            wait);
        }
#endif

#if UNITY_EDITOR_WIN || (UNITY_STANDALONE_WIN && !UNITY_EDITOR)
        private IEnumerator DispatchMessage()
        {
            // Whenever Win32 window has run in editor mode, the window message loop is gradually frozen.
            // I'm not exactly sure how this works. I can only guess the Unity3D is trying to consume more resources 
            // on the renderer in editor mode. It seems to be a sort of optimization. 
            // so, I just tried to dispatch window messages by force.

            while (true)
            {
                yield return new WaitForFixedUpdate();
                WWebViewWin32.DispatchMessage();
            }
        }
#endif

        public bool HoloLensVR
        {
            get
            {
#if NETFX_CORE
                return 
                (
#if UNITY_2017_2_OR_NEWER
                    UnityEngine.XR.XRSettings.enabled == true &&
#else
                    UnityEngine.VR.VRSettings.enabled == true && 
#endif
                    Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Holographic"
                );
#else
                return false;
#endif
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void OnApplicationQuit()
        {
#if UNIWEBVIEW3_SUPPORTED
#if UNITY_EDITOR_WIN || ((UNITY_STANDALONE_WIN || UNITY_WSA) && !UNITY_EDITOR)
            UniWebViewInterface.Release();
#endif
#elif UNIWEBVIEW2_SUPPORTED
#if UNITY_EDITOR_WIN || ((UNITY_STANDALONE_WIN || UNITY_WSA) && !UNITY_EDITOR)
            UniWebViewPlugin.Release();
#endif
#else
            WWebViewPlugin.Release();
#endif
        }

        public static string EscapeJsonText(string data)
        {
            return data.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("/", "\\/");
        }

        private static WWebViewSystem instance = null;
        public static WWebViewSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(WWebViewSystem)) as WWebViewSystem;

                    if (instance == null)
                    {
                        GameObject go = new GameObject("WWebViewSystem");
                        DontDestroyOnLoad(go);
                        instance = go.AddComponent<WWebViewSystem>();
                    }
                }
                return instance;
            }
        }
    }
}
