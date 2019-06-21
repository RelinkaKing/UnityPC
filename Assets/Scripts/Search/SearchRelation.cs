using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.UI;
using SearchComponet;
using Assets.Scripts.Model;

public class SearchRelation : MonoBehaviour {

    public static List<Relation> Relations;
    public static List<ResWeb> ResWebs;

    private void Awake()
    {
        //初始化 relations
        Relations = new List<Relation>();
        ResWebs = new List<ResWeb>();
        Readjson(PublicClass.filePath+"json/ResRelation.json",out Relations);
        Readjson(PublicClass.filePath + "json/newResWeb.json", out ResWebs);

        //测试
        //List<ResWeb> res_web_s;
        //SearchWithName("DaYuanJi_R",out res_web_s);
    }


    public static void SearchWithName(string input,out List<ResWeb> res_web_s)
    {
        //查询关系表，解析出rs_ids ,再解析出content内容，解析分发内容（资源路径，文本信息）
        string _rw_ids="";
        for (int i = 0; i < Relations.Count; i++)
        {
            if (Relations[i].sm_name == input.Trim())
            {
                _rw_ids = Relations[i].rw_ids;
                break;
            }
        }
        string[] ids = _rw_ids.Split(',');
        if (ids.Length == 0) {
            Debug.LogError("未解析出关系："+input);
            res_web_s = null;
            return;
        }
        List<ResWeb> _res_web_s = new List<ResWeb>();
        for (int j = 0; j < ids.Length; j++)
        {
            for (int i = 0; i < ResWebs.Count; i++)
            {
                if (ResWebs[i].rw_id == int.Parse(ids[j]))
                {
                    _res_web_s.Add(ResWebs[i]);
                    break;
                }
            }
        }
        if (_res_web_s.Count == 0)
        {
            Debug.LogError("未解析出内容：" + input);
            res_web_s = null;
            return;
        }
        res_web_s = _res_web_s;
    }

    public void Readjson<T>(string path,out T out_obj) where T : new()
    {
        string jsonfile = path;
        T instance = new T();
        using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o = (JObject)JToken.ReadFrom(reader);
                Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                if (r.ContainsKey("List"))
                {
                    instance = JsonConvert.DeserializeObject<T>(r["List"].ToString());
                }
            }
        }
        out_obj = instance;
    }
}

public class Relation {
    public string rs_ids { get; set; }
    public string rv_ids { get; set; }
    public string sm_name { get; set; }
    public int id { get; set; }
    public string rw_ids { get; set; }
    public string app_id { get; set; }
}

public class ResWeb
{
    public string icon_url { get; set; }
    public int level { get; set; }
    public int del { get; set; }
    public int rw_id { get; set; }
    public string type { get; set; }
    public string title { get; set; }
    public string content { get; set; }
    public int secondSort { get; set; }
    public string res_fy_icon_url { get; set; }
    public string fy_state { get; set; }
    public string charge_id { get; set; }
    public string select_icon_url { get; set; }
    public string secondFyName { get; set; }
    public int secondFyId { get; set; }
    public string desc { get; set; }
}