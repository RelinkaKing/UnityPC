// WWebViewDemo3D.cs : WWebViewDemo3D implementation file
//
// Description      : WWebViewDemo3D
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// Created          : 2017/12/14
// Last Update      : 2017/12/16
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

using UnityEngine;
using System.Collections;

public class WWebViewDemo3D : WWebViewDemo
{
    public Camera viewCamera;

    protected enum InputEvent
    {
        MouseMove = 0,
        MouseDown,
        MouseUp,
        MouseWheel
    }

    private IEnumerator Start()
    {
        while (!AreYouReady)
            yield return null;

#if UNITY_EDITOR_WIN || ((UNITY_STANDALONE_WIN || UNITY_WSA) && !UNITY_EDITOR)
        webView.Hide();
        webView.Navigate(url);
        Texture2D texture = new Texture2D(webView.GetActualWidth(), webView.GetActualHeight(), TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Trilinear; texture.Apply();
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.mainTexture = texture;
        webView.SetTexture(texture);
#else
        Debug.LogWarning("Texturing feature is only supported on Win32/WSA/Windows Editor.");
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR_WIN || ((UNITY_STANDALONE_WIN || UNITY_WSA) && !UNITY_EDITOR)
        int state = 0;
        int key = 0;

        if (GetMouseState(ref state, ref key))
        {
            RaycastHit hit;
            if (!Physics.Raycast(viewCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Debug.Log("There was an input outside the webview area.");
                return;
            }

            Renderer renderer = hit.transform.GetComponent<Renderer>();
            MeshCollider meshCollider = hit.collider as MeshCollider;

            if (renderer == null ||
                renderer.sharedMaterial == null ||
                renderer.sharedMaterial.mainTexture == null ||
                meshCollider == null)
            {
                Debug.LogWarning("There are no Renderer or Texture or MeshCollider.");
                return;
            }

            Texture2D texture = renderer.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= texture.width;
            pixelUV.y *= texture.height;
            pixelUV.y = texture.height - pixelUV.y;

            // NOTE: WSA does not support input processing YET.
            // To be honest, I couldn't find a way to programmatically pass the mouse event and keyboard input to the webview on WSA.
            // And I think there is a way to implement this obviously. Because I'm not used to developing Windows Store App.
            // So, I would be very happy if someone could tell me how.

            webView.InputEvent(state, key, (int)pixelUV.x, (int)pixelUV.y);
        }
#else
        Debug.LogWarning("Texturing feature is only supported on Win32/WSA/Windows Editor.");
#endif
    }

    private bool GetMouseState(ref int state, ref int key)
    {
        if (DidMouseMove())
        {
            state = (int)InputEvent.MouseMove;
            return true;
        }

        for (key = 0; key < 3; ++key)
        {
            if (DidMouseClick(ref state, ref key))
                return true;
        }

        if (DidMouseWheel(ref key))
        {
            state = (int)InputEvent.MouseWheel;
            return true;
        }

        return false;
    }

    private bool DidMouseClick(ref int state, ref int key)
    {
        if (Input.GetMouseButtonDown(key))
        {
            state = (int)InputEvent.MouseDown;
            return true;
        }

        if (Input.GetMouseButtonUp(key))
        {
            state = (int)InputEvent.MouseUp;
            return true;
        }

        return false;
    }

    private bool DidMouseMove()
    {
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }

    private bool DidMouseWheel(ref int key)
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            key = +1;
            return true;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            key = -1;
            return true;
        }

        return false;
    }

    private bool AreYouReady
    {
        get
        {
#if UNITY_EDITOR_WIN || ((UNITY_STANDALONE_WIN || UNITY_WSA) && !UNITY_EDITOR)
            return (webView.GetActualWidth() > 0);
#else
            return true;
#endif
        }
    }
}
