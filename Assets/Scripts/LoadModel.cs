using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using VesalDecrypt;
using XLua;

[Hotfix]
public class LoadModel : MonoBehaviour {

    public static LoadModel instance;
    public GameObject tree_panel;
    public GameObject tree_panel_parent;
    public GameObject Catalog;
    //获取范围模型的中拼音名字对应字典
    public Dictionary<string, string> named;
    public Dictionary<string, List<string>> menuMessage;

    public Dictionary<string, string> english;

    // public Dictionary<string, MenuControl> AllCreatedMenus;
    public Model[] models ;
    [LuaCallCSharp]
    void Awake () {
        instance = this;
    }
    [LuaCallCSharp]
    void Start () {
        SearchModel();
    }


    void SearchModel()
    {
        models = SceneModels.instance.Get_scope_models();

        named = new Dictionary<string, string>();
        menuMessage = new Dictionary<string, List<string>>();
        english = new Dictionary<string, string>();
        // AllCreatedMenus = new Dictionary<string, MenuControl>();
        for (int l = 0; l < SceneModels.instance.Get_scope_models().Length; l++)
        {
            ModelInfo tmp_info = PublicClass.infoDic[SceneModels.instance.Get_scope_models()[l].name];
            try
            {
                named.Add(tmp_info.Chinese, tmp_info.ModelName);
            }
            catch (System.Exception)
            {
                DebugLog.DebugLogInfo(tmp_info.Chinese + "  异常");
            }
            try
            {
                english.Add(tmp_info.ModelName, tmp_info.Chinese);
            }
            catch (System.Exception)
            {
                DebugLog.DebugLogInfo(tmp_info.Chinese + "  异常");
            }
        }
        DebugLog.DebugLogInfo("chinese 字典赋值");
        InitMainStructure();
    }

    //初始化递归
    [LuaCallCSharp]
    void InitMainStructure () {
        XmlDocument doc = new XmlDocument ();
        doc.Load (AppOpera.model_path + "SearchTree.xml");
        XmlNode rootNode = doc.FirstChild;

        GetPartElement (rootNode, "allmenu", 0);
        XmlNode mainNode = doc.SelectSingleNode ("SA");

        XmlNodeList mainList = mainNode.SelectNodes ("//mainpart");

        DebugLog.DebugLogInfo ("初始化递归：");
        for (int i = 0; i < mainList.Count; i++) {
            string LabelName = mainList.Item (i).Attributes["name"].Value;

            DebugLog.DebugLogInfo ("主节点：" + LabelName);

            //初始化主结构
            GameObject game = Instantiate (Catalog);
            game.transform.Find ("Text").GetComponent<Button> ().interactable = false;
            game.SetActive (true);
            game.transform.SetParent (tree_panel_parent.transform);
            game.transform.localPosition = Vector3.zero;
            game.transform.localScale = Vector3.one;

            MenuControl menuControl = game.GetComponent<MenuControl> ();
            menuControl.text.transform.GetComponent<Text> ().fontSize = 42 - 2;
            menuControl.text.GetComponent<Text> ().text = PublicClass.app.struct_name;//LabelName; //外部显示的中文名字
            menuControl.LabelName = LabelName; //脚本记录的中文名字
            game.name = LabelName;
            menuControl.PosY = game.transform.localPosition.y;
            menuMessage.TryGetValue (LabelName, out menuControl.nameList); //子节点名字数组

            // print("<color=#333333> 总节点子节点长度："+menuControl.nameList.Count+"</color>");
            // AllCreatedMenus.Add(LabelName, menuControl);
        }
    }

    //递归生成菜单列表
    [LuaCallCSharp]
    void GetPartElement (XmlNode node, string str, int pos) {
        XmlNodeList nodeList = node.ChildNodes;
        List<string> nameList = new List<string> ();
        pos++;
        nameList.Add (pos.ToString ());
        for (int n = 0; n < nodeList.Count; n++) {
            string names = string.Empty;
            string pinyinName = nodeList[n].Attributes["name"].Value;
            bool isHavName = named.TryGetValue (pinyinName, out names);
            if (!isHavName) {
                names = nodeList[n].Attributes["name"].Value;
            }
            nameList.Add (names);
            GetPartElement (nodeList[n], names, pos);
        }
        if (!menuMessage.ContainsKey (str)) {
            menuMessage.Add (str, nameList);
        }
    }
    [LuaCallCSharp]
    public void OpenSearchTreePanel () {
        tree_panel.SetActive (true);
    }


    public T Readjson<T>(string path) where T : new()
    {
        string jsonfile = path;
        T instance = new T();
        using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o = (JObject)JToken.ReadFrom(reader);
                Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                if (r.ContainsKey("app_id_submodel"))
                {
                    instance = JsonConvert.DeserializeObject<T>(r["app_id_submodel"].ToString());
                }
            }
        }
        return instance;
    }
}