using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class HotFixControll : MonoBehaviour
{
    LuaEnv luaEnv;
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    public void loads()
    {
        luaEnv = new LuaEnv();
        StartCoroutine(load(PublicClass.xluaPath));
    }
    IEnumerator load(string luaPath)
    {
        DirectoryInfo direction = new DirectoryInfo(luaPath);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        int num = files.Length;
        luaEnv.AddLoader(MyLoader);
        for (int i = 0; i < num; i++)
        {
            if (files[i].Name.Split('.')[0] != "DisposeList")
            {
                Debug.Log(files[i]);
                luaEnv.DoString("require '" + files[i].Name.Split('.')[0] + "'");
            }

        }
        yield return 0;
        PublicClass.DataManagerStatus = DataIntState.GoNextScence;

    }
    private byte[] MyLoader(ref string filepath)
    {
        string absPath = @PublicClass.xluaPath + filepath + ".lua.txt";
        StreamReader sr = new StreamReader(absPath);
        return System.Text.Encoding.UTF8.GetBytes(sr.ReadToEnd());
    }
    private void OnDestroy()
    {
        string absPath = @PublicClass.xluaPath + "DisposeList.lua.txt";
        if (File.Exists(absPath))
            luaEnv.DoString("require 'DisposeList'");

        if (luaEnv != null)
            luaEnv.Dispose();
    }
}
