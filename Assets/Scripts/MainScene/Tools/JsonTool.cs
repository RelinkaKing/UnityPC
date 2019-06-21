using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Repository;
using Assets.Scripts.Model;
using VesalCommon;
using Newtonsoft.Json.Linq;

public class JsonTool{

    public static void SaveClassAsJson(object class_object)
    {
        string jsonstr = JsonConvert.SerializeObject(class_object);
    }

    public static T SwitchJsonToClass<T>(string str)
    {
        return JsonConvert.DeserializeObject<T>(str);
    }

    public static void SaveStrToJson(string str,string path)
    {
        System.IO.File.WriteAllText(path, str);
    }

    public static string Readjson(string json_path)
    {
        using (System.IO.StreamReader file = System.IO.File.OpenText(json_path))
        {
            return file.ReadToEnd();
        }
    }

    //返回节点列表
    public T Readjson<T>(string path,string simple_node_name) where T : new()
    {
        string jsonfile = path;
        T instance = new T();
        using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o = (JObject)JToken.ReadFrom(reader);
                Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                if (r.ContainsKey(simple_node_name))
                {
                    instance = JsonConvert.DeserializeObject<T>(r[simple_node_name].ToString());
                }
            }
        }
        return instance;
    }

}