﻿using LarkFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

public class XLuaManager : SingletonMono<XLuaManager>
{
    //Lua代码相对于工作目录的路径
    public const string LuaPath = "luaScripts/";
    //LuaPath相对于WorkPath的根目录
    public const string LuaRootPath = "Assets/Scripts/";

    public LuaEnv luaEnv;

    public void Init()
    {
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(MyLoader);
        luaEnv.DoString("require'LuaEntry.lua'");
    }

    /// <summary>
    /// 自定义Load
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public byte[] MyLoader(ref string fileName)
    {
        string path = PathUtil.LuaPath + fileName;
        return Encoding.UTF8.GetBytes(File.ReadAllText(path));
    }

    private float lastGCTime = 0;
    private void Update()
    {
        if (Time.time - lastGCTime > 1f)
        {
            luaEnv.Tick();
            lastGCTime = Time.time;
        }
    }
}
