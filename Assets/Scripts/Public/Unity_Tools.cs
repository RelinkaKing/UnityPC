using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Public
{
    public class Unity_Tools
    {
        //获取场景物体，没有则创建
        public static GameObject Create_scene_gameObject(string object_name)
        {
            if (GameObject.Find(object_name) == null)
            {
                Debug.Log("创建实例---" + object_name);
                return new GameObject(object_name);
            }
            else
            {
                return GameObject.Find(object_name);
            }
        }


        public static T Get_scene_instance<T>(string object_name) where T : class
        {
            if (GameObject.Find(object_name) == null)
            {
                Debug.Log("场景不存在实例---" + object_name);
                return null;
            }
            else
            {
                return GameObject.Find(object_name).GetComponent<T>();
            }
        }

        public static T CreateInstance<T>(string prefab_path) where T : class
        {
            return null;
        }

        static double startTime;
        static string log;
        public static void StarTime(string debug_log)
        {
            log = debug_log;
            DebugLog.DebugLogInfo(debug_log);
            startTime = (double)Time.time;
        }
        public static void CanculateLoadTime()
        {
            startTime = (double)Time.time - startTime;
            DebugLog.DebugLogInfo(log + "时长记录完成 ，耗时： " + startTime + "s");
        }
        

        public static bool is_moblie_system()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_STANDALONE_WIN
            return false;
#elif UNITY_IOS
            return true;
#elif UNITY_ANDROID
            return true;
#else
            return false;
#endif
        }


        public static void send_message_to_platform(string type, string content)
        {
#if UNITY_EDITOR
#elif UNITY_STANDALONE_WIN
#elif UNITY_IOS
            send_message_to_platform_for_ios(type,content);
#elif UNITY_ANDROID
            send_message_to_platform_for_android(type,content);
#else
#endif
        }

        public static void set_value_to_platform(string varname, string varvalue)
        {
#if UNITY_EDITOR
#elif UNITY_STANDALONE_WIN
#elif UNITY_IOS
#elif UNITY_ANDROID

         //set_value_to_platform_for_android(varname, varvalue);
        //   AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //     AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        //     jo.Call("Set_Unity_Struct_msg",varvalue);
#else
#endif

        }

        public static void ui_return_sceneSwitch(Action callback=null)
        {
            if (SceneModels.instance != null)
            {
                SceneModels.instance.destoryTempParent();
            }
            AppOpera.SetOperaToLoop();
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.demo_pc)
            {
                if (SceneManager.GetActiveScene().name != "SceneSwitchInteral")
                {
                    SceneManager.LoadScene("SceneSwitchInteral");
                }
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SceneSwitch")
                {
                    SceneManager.LoadScene("SceneSwitch");
                }
            }
            PublicClass.app = null;
            if (callback != null)
            {
                callback();
            }
        }

        //从unity切换界面到android平台的方法

        public static void ui_return_to_platform(string type = null, string content = null)
        {
            try
            {
                    if (SceneModels.instance != null) {
                    SceneModels.instance.destoryTempParent();
                }
           
            
                AppOpera.SetOperaToLoop();
                if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.demo_pc)
                {
                    if (SceneManager.GetActiveScene().name != "SceneSwitchInteral")
                    {
                        SceneManager.LoadScene("SceneSwitchInteral");
                    }
                }
                else
                {
                    if (SceneManager.GetActiveScene().name != "SceneSwitch")
                    {
                        SceneManager.LoadScene("SceneSwitch");
                    }
                }
    #if UNITY_EDITOR || UNITY_STANDALONE_WIN
                ui_return_to_platform_for_win();
    #elif UNITY_IOS
                if(type!=null && content !=null)
                    send_message_to_platform_for_ios(type,content);
                ui_return_to_platform_for_ios();
    #elif UNITY_ANDROID
                if(type!=null && content !=null)
                    send_message_to_platform_for_android(type,content);
                ui_return_to_platform_for_android();
    #else
    #endif
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }
#if UNITY_IOS
        // [DllImport("__Internal")]
        // private static extern void _PressButton();
        // [DllImport("__Internal")]
        // private static extern void _SendMessageToIOS(string type,string value);
#endif

        public static void ui_return_to_platform_for_ios()
        {
#if UNITY_IOS
        // _PressButton();
#endif
        }
        public static void send_message_to_platform_for_ios(string type, string content)
        {
            #if UNITY_IOS
            // _SendMessageToIOS(type,content);
            #endif
        }
        public static void ui_return_to_platform_for_win()
        {
        }
        public static void ui_return_to_platform_for_android()
        {
            // AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            // AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            // jo.Call("ToLastActivity");
            //AppOpera.SetOperaToLoop();
        }
        public static void send_message_to_platform_for_android(string type, string content)
        {
            // AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            // AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            // jo.Call("showAlert", type, content);
        }

        public static void set_value_to_platform_for_android(string varname, string varvalue)
        {
            // AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            // AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            // jo.SetStatic<string>(varname, varvalue);
        }

        public static string get_message_from_platform_for_ios()
        {
            return GetMessage.temp_msg;
        }

        public static string get_message_from_platform_for_android()
        {
            return GetMessage.temp_msg;
        }

        public static void clear_message_from_platform_for_android()
        {
            GetMessage.temp_msg = "";
        }
        public static void clear_message_from_platform_for_ios()
        {
            GetMessage.temp_msg = "";
        }
        public static void clear_message_from_platform() {
            GetMessage.temp_msg = "";
        }
        public static void HideAndroidSplash()
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("hideSplash");
        }
    }

}
