// WWebViewDemoHoloLens.cs : WWebViewDemoHoloLens implementation file
//
// Description      : WWebViewDemoHoloLens
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// Created          : 2017/12/03
// Last Update      : 2017/12/03
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

using UnityEngine;
using UnityEngine.UI;
using ICODES.STUDIO.WWebView;

public class WWebViewDemoHoloLens : WWebViewDemo
{
    public new void Navigate()
    {
        SwitchWebView();
        Navigate(url);
    }

    public void WWebViewHoloLensStore()
    {
        Application.OpenURL("ms-windows-store://pdp/?productid=9ndwhjc4td5b");
    }

    protected bool SwitchWebView()
    {
#if UNITY_WSA && !UNITY_EDITOR
        if (WWebViewSystem.Instance.HoloLensVR)
        {
#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SwitchXamlView();
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.SwitchXamlView();
#else
            WWebViewPlugin.SwitchXamlView();
#endif
            return true;
        }
#endif
        return false;
    }

    protected bool SwitchUnityView()
    {
#if UNITY_WSA && !UNITY_EDITOR
        if (WWebViewSystem.Instance.HoloLensVR)
        {
#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.SwitchMainView();
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.SwitchMainView();
#else
            WWebViewPlugin.SwitchMainView();
#endif
            return true;
        }
#endif
        return false;
    }
}
