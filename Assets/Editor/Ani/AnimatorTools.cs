using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using VesalCommon;

public class AnimatorTools : EditorWindow
{

    public static List<string> getAllFile(string path) {
        List<string> result = new List<string>();
        DirectoryInfo dif = new DirectoryInfo(path);
        foreach (FileSystemInfo fi in dif.GetFileSystemInfos())
        {
            if (fi is FileInfo)
            {
                result.Add(fi.Name);
            }
            else {
                result.AddRange(getAllFile(fi.FullName));
            }
        }
        return result;
    }
    [MenuItem("Ani/复制AnimationClip")]
    static void CopyClip()
    {
        string selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log("测试被选择物体的路径：" + selectPath);
        
        List<string> allAniPath = getAllFile(selectPath);
        for (int i = 0; i < allAniPath.Count; i++)
        {
            string tmpPath = selectPath + "/" + allAniPath[i];
            if (!allAniPath[i].ToLower().EndsWith(".fbx")) {
                continue;
            }
            AnimationClip newClip = new AnimationClip();
            AnimationClip oldClip = AssetDatabase.LoadAssetAtPath(tmpPath, typeof(AnimationClip)) as AnimationClip;
            if (oldClip == null) {
                Debug.LogError("oldClip == null");
                continue;
            }
            Debug.Log(oldClip.name);
            EditorUtility.CopySerialized(oldClip, newClip);

            AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(newClip);
            clipSetting.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(newClip, clipSetting);
            AssetDatabase.CreateAsset(newClip, selectPath+"/" + allAniPath[i].Replace(".FBX","").Replace(".fbx", "") + ".anim");
        }
        allAniPath.Clear();
        AssetDatabase.Refresh();


    }
}