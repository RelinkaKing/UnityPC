using XLua;
using UnityEngine;
using System.IO;
using System;

namespace Scripts
{
    public class XLuaManager : MonoBehaviour
    {

        LuaEnv m_luaEnv = new LuaEnv();
        public static XLuaManager instance;
        
        public void OnEnable()
        {
            PublicClass.filePath = Application.persistentDataPath + "/";
            instance = this;
            LuaEnv.CustomLoader method = CustomLoaderMethod;
            //添加自定义装载机Loader  
            m_luaEnv.AddLoader(method);
            try {
                m_luaEnv.DoString(@" require('main')");
            }
            catch (Exception e) {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
            // m_luaEnv.DoString("require 'main'");
        }




        private byte[] CustomLoaderMethod(ref string fileName)
        {
            print("<color=#00ff00>"+fileName+"</color>");
            fileName = PublicClass.filePath+"xlua/" + fileName+".lua.txt";
            if (File.Exists(fileName))
            {
                return File.ReadAllBytes(fileName);
            }
            else
            {
                return null;
            }
        }

    }
}

