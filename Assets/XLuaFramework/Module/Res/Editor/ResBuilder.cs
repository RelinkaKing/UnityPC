using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ResBuilder
{

    #region Menu
    [MenuItem("XLuaFramework/Build iPhone Resource", false, 100)]
    public static void BuildiPhoneResource()
    {
        BuildAssetResource(BuildTarget.iOS);
    }

    [MenuItem("XLuaFramework/Build Android Resource", false, 101)]
    public static void BuildAndroidResource()
    {
        BuildAssetResource(BuildTarget.Android);
    }

    [MenuItem("XLuaFramework/Build Windows Resource", false, 102)]
    public static void BuildWindowsResource()
    {
        BuildAssetResource(BuildTarget.StandaloneWindows);
    }
    #endregion

    private static void BuildAssetResource(BuildTarget target)
    {
        Debug.Log("=============== Build " + target.ToString() + " Start =============== ");

        if (Directory.Exists(PathUtil.BuilderPath + ResManager.ABPath))
        {
            Directory.Delete(PathUtil.BuilderPath + ResManager.ABPath, true);
        }
        // Directory.CreateDirectory(PathUtil.BuilderPath + ResManager.ABPath);

        // AssetBundleBuild[] buildMap = new AssetBundleBuild[4];

        // //预置件
        // buildMap[0].assetBundleName = "Prefabs/prefab.unity3d";
        // buildMap[0].assetNames = new string[]{ "Assets/_Project/Prefabs/Cube01.prefab" };
        // //材质
        // buildMap[1].assetBundleName = "Materials/material.unity3d";
        // buildMap[1].assetNames = new string[] { "Assets/_Project/Materials/m1.mat" };
        // //贴图
        // buildMap[2].assetBundleName = "Textures/texture.unity3d";
        // buildMap[2].assetNames = new string[] { "Assets/_Project/Textures/t1.png" };

        // //场景打包
        // buildMap[3].assetBundleName = "Scenes/scene.unity3d";
        // buildMap[3].assetNames = new string[] { "Assets/_Project/Scenes/Home.unity" };

        // BuildPipeline.BuildAssetBundles(PathUtil.BuilderPath + ResManager.ABPath, buildMap, BuildAssetBundleOptions.None, target);

        CopyLuaToBuildPath();

        CreateFileList();
        Debug.Log("=============== Build " + target.ToString() + " Finished ============= ");
    }

    /// <summary>
    /// 生成文件列表
    /// </summary>
    [MenuItem("XLuaFramework/Create File List", false, 103)]
    static void CreateFileList()
    {
        Debug.Log("=============== Create File List Start ===============");
        //生成文件
        string filePath = PathUtil.BuilderPath + ResManager.FileListName;
        StreamWriter sw;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        sw = File.CreateText(filePath);

        //获取文件列表
        string[] files = Directory.GetFiles(PathUtil.BuilderPath, "*.*", SearchOption.AllDirectories);
        //以行为单位写入字符串
        foreach (var f in files)
        {
            var path = f.Replace("\\", "/");
            if (Path.GetExtension(path).Equals(".meta")) continue;      //忽略unity的meta文件
            if (path.Equals(filePath)) continue;
            Debug.Log(path);
            sw.WriteLine(path.Substring((PathUtil.BuilderPath).Length) + "|" + Util.Md5file(path));
        }
        sw.Close();
        sw.Dispose();

        AssetDatabase.Refresh();
        Debug.Log("=============== Create File List End ===============");
    }

    /// <summary>
    /// 复制Lua脚本到出包根目录
    /// </summary>
    [MenuItem("XLuaFramework/Copy Lua to BuildPath", false, 104)]
    public static void CopyLuaToBuildPath()
    {
        var buildLuaPath = ResManager.BuilderPath + XLuaManager.LuaPath;
        if (Directory.Exists(buildLuaPath))
        {
            Directory.Delete(buildLuaPath, true);
        }
        Directory.CreateDirectory(buildLuaPath);

        //复制文件夹
        Util.CopyDirectory(PathUtil.GetWorkPath + XLuaManager.LuaRootPath + XLuaManager.LuaPath, buildLuaPath);

        //todo:压缩文件

        AssetDatabase.Refresh();
    }
}
