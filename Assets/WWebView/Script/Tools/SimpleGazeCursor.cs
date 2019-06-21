// SimpleGazeCursor.cs : SimpleGazeCursor implementation file
//
// Description      : SimpleGazeCursor
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// Created          : 2017/11/08
// Last Update      : 2017/11/08
// References       : http://www.immersivelimit.com/simple-gaze-cursor/
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

using UnityEngine;

public class SimpleGazeCursor : MonoBehaviour
{
    public Camera viewCamera;
    public GameObject cursorPrefab;
    public float maxCursorDistance = 30;
    private GameObject cursorInstance;

    protected void Start()
    {
        cursorInstance = Instantiate(cursorPrefab);
    }

    protected void Update()
    {
        UpdateCursor();
    }

    protected void UpdateCursor()
    {
        RaycastHit hit;
        Ray ray = new Ray(viewCamera.transform.position, viewCamera.transform.rotation * Vector3.forward);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            cursorInstance.transform.position = hit.point;
            cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            cursorInstance.transform.position = ray.origin + ray.direction.normalized * maxCursorDistance;
            cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, -ray.direction);
        }
    }
}
