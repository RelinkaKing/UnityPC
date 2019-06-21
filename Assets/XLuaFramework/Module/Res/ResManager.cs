using LarkFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ResManager : SingletonMono<ResManager>
{
    #region Config
    //编辑器加载模式，true为通过AB包加载，false为通过AssetDatabase加载
    public static readonly bool IsEditorMode = false;

    //出包根路径
    public static readonly string BuilderPath = PathUtil.GetWorkPath + "Assets/AssetsPackage/";

    //总的Manifest的文件名，也是同级目录名
    public const string ManifestName = "AB";
    //AB包路径
    public const string ABPath = ManifestName + "/";

    //更新模式,是否开启从服务端下载资源
    public static readonly bool UpdaeMode = true;
    //文件列表,存储AB包MD5
    public static readonly string FileListName = "FileList.txt";

#if UNITY_STANDALONE
    //PC平台资源更新地址
    public static readonly string UpdateAddress = "http://ab.evesgf.com/XLuaFrameworkRes/PC/";
#elif UNITY_IPHONE
    //IOS平台资源更新地址
    public static readonly string UpdateAddress = "http://ab.evesgf.com/XLuaFrameworkRes/IOS/";
#elif UNITY_ANDROID
    //Android平台资源更新地址
    public static readonly string UpdateAddress = "http://ab.evesgf.com/XLuaFrameworkRes/Android/";
#endif
    #endregion

    #region 引用计数相关
    /// <summary>
    /// 资源引用相关类
    /// </summary>
    public class AssetBundleInfo
    {
        public AssetBundle m_AssetBundle;       //AB包资源
        public int m_ReferencedCount;           //引用计数

        public AssetBundleInfo(AssetBundle assetBundle)
        {
            m_AssetBundle = assetBundle;
            m_ReferencedCount = 1;      //构造时都会加入管理队列，所以默认为1
        }
    }

    //已经加载了的AB包的列表，key为AB包名，如Prefabs/prefab.unity3d
    private Dictionary<string, AssetBundleInfo> m_loadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
    #endregion

    #region 初始化相关
    //总的Manifest，用于查询依赖
    private AssetBundleManifest m_assetBundleManifest;

    /// <summary>
    /// ResManager 初始化
    /// </summary>
    /// <param name="initOK">初始化完成后的回调方法</param>
    public void Init(Action initOK = null)
    {
        StartCoroutine(OnInit(initOK));  
    }
    IEnumerator OnInit(Action initOK)
    {
        yield return StartCoroutine(CheckLocalResource());

        if (!IsEditorMode)      //走AssetDatabase时不需要加载
        {
            //加载assetBundleManifest文件    
            AssetBundle manifestBundle = AssetBundle.LoadFromFile(PathUtil.ABPath + ManifestName);
            if (manifestBundle == null)
            {
                Debug.LogError("[ResManager]Init Error! manifestBundle is nil!");
                yield break;
            }
            m_assetBundleManifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            //卸载资源
            manifestBundle.Unload(false);
            manifestBundle = null;
        }

        Debug.Log("[ResManager] Init Finished!");
        if (initOK != null) initOK.Invoke();
    }

    #region 资源更新
    /// <summary>
    /// 检查资源
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckLocalResource()
    {
        if (!UpdaeMode)
        {
            Debug.Log("更新模式未开启！");
            yield break;
        }

        //检测路径是否存在，即是否是第一次安装
        if (!Directory.Exists(PathUtil.DataPath))
        {
            Directory.CreateDirectory(PathUtil.DataPath);
        }

        //检测是否联网，不联网则不更新,跳出函数块
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("未检测到网络连接");
            yield break;
        }

        //删除旧的文件列表
        if (File.Exists(PathUtil.DataPath + FileListName))
        {
            File.Delete(PathUtil.DataPath + FileListName);
        }

        //下载列表文件
        yield return StartCoroutine(DownloadFile(UpdateAddress + FileListName, PathUtil.DataPath + FileListName));
        //逐行读取文件列表
        StreamReader sr = new StreamReader(PathUtil.DataPath + FileListName);
        //用于存储每行的资源路径和MD5值
        Dictionary<string, string> files = new Dictionary<string, string>();
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            var info = line.Split('|');
            var filePath = info[0];
            var fileMd5 = info[1];
            files.Add(filePath, fileMd5);
        }
        if (sr != null) sr.Close();

        //比对资源列表
        int i = 0;
        foreach (KeyValuePair<string, string> pair in files)
        {
            i++;
            if (File.Exists(PathUtil.DataPath + pair.Key))
            {
                if (Path.GetExtension(pair.Key).Equals(".meta")) continue;      //忽略unity的meta文件
                if (Util.Md5file(PathUtil.DataPath + pair.Key).Equals(pair.Value)) continue;
                File.Delete(PathUtil.DataPath + pair.Key);
            }

            //下载资源
            Debug.Log("[ResManager] DownLoad File:" + UpdateAddress + pair.Key);
            yield return StartCoroutine(DownloadFile(UpdateAddress + pair.Key, PathUtil.DataPath + pair.Key));
        }

        Debug.Log("=============== Check ResUpdate Finished ===============");
    }

    /// <summary>
    /// 文件下载
    /// </summary>
    /// <param name="url"></param>
    /// <param name="savePath"></param>
    /// <returns></returns>
    IEnumerator DownloadFile(string url, string savePath)
    {
        var www = new UnityWebRequest(url);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return www.Send();
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            //创建目录
            var saveDir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            FileStream fs = new FileStream(savePath, FileMode.Create);      //有同名文件则直接覆盖
            fs.Write(results, 0, results.Length);
            fs.Close();
            fs.Dispose();
        }
    }
    #endregion
    #endregion

    #region 资源加载

    #region LoadPrefab
    /// <summary>
    /// 同步加载类型为GameObject的预置件资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public GameObject LoadPrefab(string abName, string assetName)
    {
        GameObject reObj = null;
        if (IsEditorMode && Application.isEditor)//使用编辑器模拟模式
        {
#if UNITY_EDITOR
            reObj = LoadAssetsFromAD<GameObject>(assetName);
#endif
        }
        else
        {
            var ab = LoadAssetsFromAB(abName);
            reObj = ab.LoadAsset<GameObject>(assetName);
        }

        if (reObj == null) Debug.LogError("[ResManager] 资源不存在! abName:" + abName + ",assetName:" + assetName);

        return reObj;
    }

    /// <summary>
    /// 异步加载类型为GameObject的预置件资源
    /// </summary>
    /// <param name="abName">ab包全名,如Assets/_Project/Prefab/prefab.unity3d</param>
    /// <param name="assetName">资源全名,如Assets/_Project/Prefab/Cube01.prefab</param>
    /// <param name="action">加载结束时调用，并传入一个AssetBundle类型的参数</param>
    /// <returns></returns>
    public IEnumerator LoadPrefabAsync(string abName, string assetName, Action<GameObject> action)
    {
        GameObject reObj = null;
        if (IsEditorMode && Application.isEditor)//使用编辑器模拟模式
        {
#if UNITY_EDITOR
            reObj = LoadAssetsFromAD<GameObject>(assetName);
#endif
        }
        else
        {
            yield return LoadAssetsFromABAsync(abName, delegate (AssetBundle ab)
            {
                reObj = ab.LoadAsset<GameObject>(assetName);
            });
        }
        if (reObj == null) Debug.LogError("[ResManager] 资源不存在! abName:" + abName + ",assetName:" + assetName);

        if (action != null) action.Invoke(reObj);
    }
    #endregion

    #region LoadScene
    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
    public IEnumerator LoadSceneAsync(string abName, string assetName, Action action)
    {
        if (IsEditorMode && Application.isEditor)//使用编辑器模拟模式
        {
#if UNITY_EDITOR
            yield return StartCoroutine(OnLoadSceneAsync(assetName, action));
            if (action != null) action.Invoke();
#endif
        }
        else
        {
            yield return StartCoroutine(LoadAssetsFromABAsync(abName, delegate
            {
                StartCoroutine(OnLoadSceneAsync(assetName, action));
            }));
        }
    }
    IEnumerator OnLoadSceneAsync(string assetName, Action action)
    {
        yield return SceneManager.LoadSceneAsync(assetName);
        if (action != null) action.Invoke();
    }
    #endregion

    #region LoadAssets
    /// <summary>
    /// AB同步方式加载资源
    /// </summary>
    /// <param name="abName">ab包全名,如Assets/_Project/Prefab/prefab.unity3d</param>
    /// <returns></returns>
    public AssetBundle LoadAssetsFromAB(string abName)
    {
        abName = abName.ToLower();      //AB包规定全部小写

        //AB包依赖加载
        string[] dependencies = m_assetBundleManifest.GetAllDependencies(abName);
        foreach (var dep in dependencies)
        {
            AssetBundleInfo depBundleInfo;
            //检查AB包依赖是否已经加载
            if (m_loadedAssetBundles.TryGetValue(dep, out depBundleInfo))
            {
                depBundleInfo.m_ReferencedCount++;
            }
            else
            {
                //依赖包路径
                var depPath = PathUtil.ABPath + dep;
                var depAB = AssetBundle.LoadFromFile(depPath);
                if (depAB == null)
                {
                    Debug.LogError("[ResManager] AB包:" + abName + " 的依赖包:" + depPath + " 未能正确加载！");
                    return null;
                }
                depBundleInfo = new AssetBundleInfo(depAB);
                //加入加载列表
                m_loadedAssetBundles.Add(dep, depBundleInfo);
            }
        }

        AssetBundleInfo bundleInfo;
        //检查AB包本体是否已经加载
        if (m_loadedAssetBundles.TryGetValue(abName, out bundleInfo))
        {
            bundleInfo.m_ReferencedCount++;
        }
        else
        {
            //AB包本体加载
            var loadPath = PathUtil.ABPath + abName;
            var reAB = AssetBundle.LoadFromFile(loadPath);
            if (reAB == null)
            {
                Debug.LogError("[ResManager] AB包:" + abName + " 未能正确加载:" + loadPath);
                return null;
            }
            bundleInfo = new AssetBundleInfo(reAB);
            //加入加载列表
            m_loadedAssetBundles.Add(abName, bundleInfo);
        }

        return bundleInfo.m_AssetBundle;
    }

    /// <summary>
    /// AB异步方式加载资源
    /// </summary>
    /// <param name="abName">ab包全名,如Assets/_Project/Prefab/Cube01.prefab</param>
    /// <param name="action">加载结束时调用，并传入一个AssetBundle类型的参数</param>
    /// <returns></returns>
    public IEnumerator LoadAssetsFromABAsync(string abName, Action<AssetBundle> action)
    {
        abName = abName.ToLower();      //AB包规定全部小写

        //AB包依赖加载
        string[] dependencies = m_assetBundleManifest.GetAllDependencies(abName);
        foreach (var dep in dependencies)
        {
            AssetBundleInfo depBundleInfo;
            //检查是否已经加载
            if (m_loadedAssetBundles.TryGetValue(dep, out depBundleInfo))
            {
                depBundleInfo.m_ReferencedCount++;
            }
            else
            {
                //依赖包加载
                var depPath = PathUtil.ABPath + dep;
                var depRequest = AssetBundle.LoadFromFileAsync(depPath);
                yield return depRequest;
                var depAB = depRequest.assetBundle;
                if (depAB == null)
                {
                    Debug.LogError("[ResManager] AB包:" + abName + " 的依赖包:" + depPath + " 未能正确加载！");
                    yield break;
                }
                depBundleInfo = new AssetBundleInfo(depAB);
                //加入加载列表
                m_loadedAssetBundles.Add(dep, depBundleInfo);
            }
        }

        AssetBundleInfo bundleInfo;
        //检查是否已经加载
        if (m_loadedAssetBundles.TryGetValue(abName, out bundleInfo))
        {
            bundleInfo.m_ReferencedCount++;
        }
        else
        {
            //AB包本体加载
            var loadPath = PathUtil.ABPath + abName;
            var abRequest = AssetBundle.LoadFromFileAsync(loadPath);
            yield return abRequest;
            var reAB = abRequest.assetBundle;
            if (reAB == null)
            {
                Debug.LogError("[ResManager] AB包：" + abName + "，不存在:" + loadPath);
                yield break;
            }

            bundleInfo = new AssetBundleInfo(reAB);
            //加入加载列表
            m_loadedAssetBundles.Add(abName, bundleInfo);
        }
        if (action != null) action.Invoke(bundleInfo.m_AssetBundle);
    }

#if UNITY_EDITOR
    /// <summary>
    /// AssetDatabase方式加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="abName"></param>
    /// <returns></returns>
    public T LoadAssetsFromAD<T>(string abName) where T : UnityEngine.Object
    {
        var obj = UnityEditor.AssetDatabase.LoadAssetAtPath(abName, typeof(T)) as T;

        if (obj == null) Debug.LogError("[ResManager] AD资源：" + abName + "，不存在!");

        return obj;
    }
#endif
    #endregion

    #endregion

    #region 资源卸载
    /// <summary>
    /// 卸载AB包
    /// </summary>
    /// <param name="abName">ab包名,如ab.unity3d</param>
    /// <param name="isThorough">是否强制清除</param>
    public void UnLoadAssetBundle(string abName, bool isThorough = false)
    {
        if (IsEditorMode) return;

        abName = abName.ToLower();      //AB包规定全部小写

        AssetBundleInfo bundleInfo = null;
        if (m_loadedAssetBundles.TryGetValue(abName, out bundleInfo))
        {
            //卸载依赖
            string[] dependencies = m_assetBundleManifest.GetAllDependencies(abName);
            foreach (var dep in dependencies)
            {
                AssetBundleInfo depBundleInfo = null;
                if (m_loadedAssetBundles.TryGetValue(dep, out depBundleInfo))
                {
                    if (--depBundleInfo.m_ReferencedCount <= 0)
                    {
                        depBundleInfo.m_AssetBundle.Unload(isThorough);

                        m_loadedAssetBundles.Remove(dep);

                        Debug.Log("[ResManager] 资源:" + dep + "成功卸载");
                    }
                    else
                    {
                        Debug.Log("[ResManager] 资源:" + dep + "引用计数:" + depBundleInfo.m_ReferencedCount);
                    }
                }
            }

            //卸载AB包本体
            if (--bundleInfo.m_ReferencedCount <= 0)
            {
                bundleInfo.m_AssetBundle.Unload(isThorough);

                m_loadedAssetBundles.Remove(abName);

                Debug.Log("[ResManager] 资源:" + abName + "成功卸载");
            }
            else
            {
                Debug.Log("[ResManager]" + "资源:" + abName + "引用计数:" + bundleInfo.m_ReferencedCount);
            }
        }
    }
    #endregion
}
