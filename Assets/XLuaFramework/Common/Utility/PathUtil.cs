using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    /// <summary>
    /// Lua根路径
    /// </summary>
    public static string LuaPath
    {
        get
        {
            if (ResManager.IsEditorMode)
            {
                return DataPath +XLuaManager.LuaRootPath+ XLuaManager.LuaPath;
            }
            else
            {
                return DataPath + XLuaManager.LuaPath;
            }
        }
    }

    /// <summary>
    /// AB包根路径
    /// </summary>
    public static string ABPath
    {
        get
        {
            return DataPath + ResManager.ABPath;
        }
    }

    /// <summary>
    /// 资源根路径
    /// </summary>
    public static string DataPath
    {
        get
        {
            string appName = Application.companyName + "/" + Application.productName;
            if (Application.isMobilePlatform)   //移动端真机
            {
                return Application.persistentDataPath + "/" + appName + "/";
            }
            else
            {
                if (ResManager.IsEditorMode)    //编辑器加载资源模式
                {
                    return GetWorkPath;
                }
                else
                {
                    //编辑器下模拟从persistentDataPath加载资源，即从更新地址下载到这里
                    if (ResManager.UpdaeMode) 
                    {
                        return "c:/" + appName + "/";
                    }
                    else
                    {
                        return BuilderPath;     //从出包根路径加载
                    }
                }
            }
        }
    }

    /// <summary>
    /// 出包根路径
    /// </summary>
    public static string BuilderPath
    {
        get
        {
            return ResManager.BuilderPath;
        }
    }

    /// <summary>
    /// 程序工作跟路径
    /// </summary>
    /// <returns>返回绝对路径，如：E:/Project/XLuaFramewrok/</returns>
    public static string GetWorkPath
    {
        get
        {
            return Environment.CurrentDirectory.Replace("\\", "/") + "/";
        }
    }
}
