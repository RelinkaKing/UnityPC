using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AssetBundleBuilder : MonoBehaviour
{
    //资源存放路径
    static string assetsDir = Application.dataPath + "/Prefab";//图钉/模型";
    static string signmap = Application.dataPath + "/图钉/map";
    //打包后存放路径
    static string assetBundlesPath = Application.streamingAssetsPath;

    void OnEnable()
    {
        //if (Directory.Exists(AssetDatabase.GetAssetPath(target)))
        //{
        //    LoadFiles(data, AssetDatabase.GetAssetPath(Selection.activeObject));
        //    EditorGUIUtility.SetIconSize(Vector2.one * 16);
        //}
    }

    [MenuItem("AssetBundle/AutoBuildAllSignMap")]
    static void AutoBuildAllSignMap()
    {
        //清除所有的AssetBundleName
        ClearAssetBundlesName();

        //设置指定路径下所有需要打包的assetbundlename
        SetAssetBundlesName(signmap);
        //打包所有需要打包的asset
        BuildPipeline.BuildAssetBundles(assetBundlesPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
    }

    [MenuItem("AssetBundle/AutoBuildAll")]
    static void AutoBuildAll()
    {
        //清除所有的AssetBundleName
        ClearAssetBundlesName();

        //设置指定路径下所有需要打包的assetbundlename
        SetAssetBundlesName(assetsDir);
        //打包所有需要打包的asset
        BuildPipeline.BuildAssetBundles(assetBundlesPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
    }

    [MenuItem("AssetBundle/BuildWithName")]
    static void BuildAllAssetBundle()
    {
        BuildPipeline.BuildAssetBundles(assetBundlesPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android );
    }

    /// <summary>
    /// 清除所有的AssetBundleName，由于打包方法会将所有设置过AssetBundleName的资源打包，所以自动打包前需要清理
    /// </summary>
    [MenuItem("AssetBundle/ClearAllName")]
    static void ClearAssetBundlesName()
    {
        //获取所有的AssetBundle名称
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();

        //强制删除所有AssetBundle名称
        for (int i = 0; i < abNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(abNames[i], true);
        }
    }

    /// <summary>
    /// 设置所有在指定路径下的AssetBundleName
    /// </summary>
    static void SetAssetBundlesName(string _assetsPath)
    {
        //先获取指定路径下的所有Asset，包括子文件夹下的资源
        DirectoryInfo dir = new DirectoryInfo(_assetsPath);
        FileSystemInfo[] files = dir.GetFileSystemInfos(); //GetFileSystemInfos方法可以获取到指定目录下的所有文件以及子文件夹
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i] is DirectoryInfo)  //如果是文件夹则递归处理
            {
                SetAssetBundlesName(files[i].FullName);
            }
            else if (!files[i].Name.EndsWith(".meta")) //如果是文件的话，则设置AssetBundleName，并排除掉.meta文件
            {
                SetABName(files[i].FullName);     //逐个设置AssetBundleName
            }
        }

    }

    /// <summary>
    /// 设置单个AssetBundle的Name
    /// </summary>
    /// <param name="filePath"></param>
    static void SetABName(string assetPath)
    {
        string importerPath = "Assets" + assetPath.Substring(Application.dataPath.Length);  //这个路径必须是以Assets开始的路径
        AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);  //得到Asset

        string tempName = assetPath.Substring(assetPath.LastIndexOf(@"\") + 1);
        string assetName = tempName.Remove(tempName.LastIndexOf(".")); //获取asset的文件名称
        assetImporter.assetBundleName = assetName;    //最终设置assetBundleName
    }
}