using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleEditor : Editor
{
    public static string scourcePath = Application.dataPath + "/Resource";
    private static string AssetBundlesOutputPath = Application.dataPath + "/StreamingAssets";

    [MenuItem("AssetBundles/BuildAssetBundle")]
    public static void BuildAssetBundle()
    {
        string outputPath = Path.Combine(AssetBundlesOutputPath, Platform.GetPlatformFolder(BuildTarget.iOS));
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        //根据BuildSetting里面所激活的平台进行打包 设置过AssetBundleName的都会进行打包  
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows );

        AssetDatabase.Refresh();

        Debug.Log("打包完成");

    }
}

public class Platform
{
	public static string GetPlatformFolder(BuildTarget target)  
	{  
		switch (target)  
		{  
		case BuildTarget.Android:  
			return "Android";  
		case BuildTarget.iOS:  
			return "IOS";  
		case BuildTarget.StandaloneWindows:  
		case BuildTarget.StandaloneWindows64:  
			return "Windows";  
		case BuildTarget.StandaloneOSXIntel:  
		case BuildTarget.StandaloneOSXIntel64:  
		case BuildTarget.StandaloneOSXUniversal:  
			return "OSX";  
		default:  
			return null;  
		}  
	}  	
}
