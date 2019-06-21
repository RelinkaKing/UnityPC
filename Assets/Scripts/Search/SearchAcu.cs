using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System;
using UnityEngine.UI;
using SearchComponet;

public class SearchAcu : MonoBehaviour {

    List<Acu> acu_list;
    public TreeView view;
    public TreeView line_view;

    public GameObject SearchContent;
    public GameObject SearchPanel;
    public GameObject TreePanel;
    void Awake()
    {
        Readjson();
        FilterAcuList(PublicClass.app.app_id);
        InitTree();
    }

    void InitTree()
    {
        CrossAllData();
        CreateTree(view);
        CreateTreeViewData();
        //view.AddData<CustomTreeItemData>(0, 1, "经络");
        //view.AddData<CustomTreeItemData>(1, 2, "手阳明经");
        //view.AddData<CustomTreeItemData>(1, 3, "足阳明经");
        //view.AddData<CustomTreeItemData>(3, 4, "少商");
        //view.AddData<CustomTreeItemData>(2, 5, "涌泉");
        view.Generate();
    }


    public void SetSearchPanelActive(bool active)
    {
        SearchContent.SetActive(active);
    }

    public void ClickSearchIndex(int index)
    {
        Val _val;
        bool isHave = search_index_dic.TryGetValue(index, out _val);
        if (isHave)
        {
            if (_val.qiu == "")
            {
                if (acu_list.Count > 1)
                {
                    acuTEST.Instance.SearchJingluoEvent(_val.code);
                    SetSearchPanelActive(false);
                }
            }
            else
            {
                acuTEST.Instance.SearchAcuEvent(_val.qiu);
                SetSearchPanelActive(false);
            }
        }
        else
        {
            Debug.LogError("not find index " + index);
        }
    }

    public void CreateLineTree(TreeView view)
    {
        view.CustomsChildSize(0, new Vector2(1000, 80));
        view.CustomsSpace(0, 80);
        view.onClick = (item, isOpen) =>
        {
            var target = item as CustomTreeItemData;
            target.toggle.isOn = isOpen;
            Debug.Log("点击了:" + item.name.text + " " + target.id);
            ClickSearchIndex(target.id);
        };

        view.onHideItem = item =>
        {
            var target = item as CustomTreeItemData;
            target.toggle.isOn = false;
        };

        view.onCreateItem = item =>
        {
            if (item.childs.Count == 0)
            {
                var target = item as CustomTreeItemData;
                target.toggle.gameObject.SetActive(false);
                target.toggle.isOn = true;
            }
        };

    }

    public void ClickIndex(int index)
    {
        Val _val;
        bool isHave=index_val_dic.TryGetValue(index, out _val);
        if (isHave)
        {
            if (_val.qiu == "")
            {
                if (acu_list.Count > 1)
                {
                    acuTEST.Instance.SearchJingluoEvent(_val.code);
                    SetSearchPanelActive(false);
                }
            }
            else
            {
                acuTEST.Instance.SearchAcuEvent(_val.qiu);
                SetSearchPanelActive(false);
            }
        }
        else
        {
            Debug.LogError("not find index "+index);
        }
    }

    public void CreateTree(TreeView view)
    {
        view.CustomsChildSize(0, new Vector2(1000, 80));
        view.CustomsSpace(0, 80);
        view.onClick = (item, isOpen) =>
        {
            var target = item as CustomTreeItemData;
            target.toggle.isOn = isOpen;
            Debug.Log("点击了:" + item.name.text+" "+ target.id);
            ClickIndex(target.id);
        };

        view.onHideItem = item =>
        {
            var target = item as CustomTreeItemData;
            target.toggle.isOn = false;
        };

        view.onCreateItem = item =>
        {
            if (item.childs.Count == 0)
            {
                var target = item as CustomTreeItemData;
                target.toggle.gameObject.SetActive(false);
                target.toggle.isOn = true;
            }
        };

    }


    int index = 0;
    int parent = 0;
    Dictionary<int, int> index_parent = new Dictionary<int, int>();
    Dictionary<string, int> name_index= new Dictionary<string, int>();
    public void CrossAllData()
    {
        index = 2;
        parent = 1;
        for (int i = 0; i < acu_list.Count; i++)
        {
            Val val = acu_list[i].val;
            name_index.Add(val.name,index);
            index_parent.Add(index,parent);
            //Debug.Log(val.name);
            index++;
        }
        parent++;
        for (int i = 0; i < acu_list.Count; i++)
        {
            Val val = acu_list[i].val;
            if (val.children.Length > 0)
            {
                for (int j = 0; j < val.children.Length; j++)
                {
                    name_index.Add(val.children[j].name, index);
                    index_parent.Add(index, parent);
                    //Debug.Log(val.children[j].name);
                    index++;
                }
            }
            parent++;
        }
        print(name_index.Count);
        print(index_parent.Count);
    }

    public void jingluo(string app_id)
    {
        for (int i = 0; i < acu_list.Count; i++)
        {
            if (acu_list[i].val.code == app_id)
            {
                CreateTree(acu_list[i].val);
            }
        }
    }

    Dictionary<int, Val> search_index_dic;
    List<Val> search_val_list;
    public GameObject SearchParent;
    public void SearchAcuWithName(InputField input)
    {
        print("search "+input.text);
        search_val_list = new List<Val>();
        SearchPanel.SetActive(true);
        TreePanel.SetActive(false);
        int count = SearchParent.transform.childCount;
        int childCount = SearchParent.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(SearchParent.transform.GetChild(0).gameObject);
        }

        string[] names=name_index.Keys.ToArray();
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Contains(input.text))
            {
                search_val_list.Add(index_val_dic[name_index[names[i]]]);
            }
        }
        search_index_dic=new Dictionary<int, Val> ();
        line_view.Clear();
        CreateLineTree(line_view);
        for (int i = 0; i < search_val_list.Count; i++)
        {
            line_view.AddData<CustomTreeItemData>(0, i + 1, search_val_list[i].name);
            search_index_dic.Add(i + 1, search_val_list[i]);
        }
        line_view.Generate();
    }

    public void BackTree()
    {
        SearchContent.SetActive(false);
        SearchPanel.SetActive(false);
        TreePanel.SetActive(true);
    }

    void FilterAcuList(string app_id)
    {
        List<Acu> tem_acu_list=new List<Acu>();
        tem_acu_list.AddRange(acu_list);
        acu_list.Clear();
        for (int i = 0; i < tem_acu_list.Count; i++)
        {
            if (tem_acu_list[i].val.code == app_id)
            {
                acu_list.Add(tem_acu_list[i]);
            }
        }
        if (acu_list.Count == 0)
        {
            for (int i = 0; i < tem_acu_list.Count; i++)
            {
                acu_list.Add(tem_acu_list[i]);
            }
        }
    }

    public void CreateTreeViewData()
    {
        List<Val> val_array = new List<Val>();
        foreach (var item in acu_list)
        {
            val_array.Add(item.val);
        }
        Val _val = new Val
        {
            code="",
            isOpen="false",
            children= val_array.ToArray(),
            name ="经络腧穴",
        };
        CreateTree(_val);
    }

    Dictionary<int, Val> index_val_dic = new Dictionary<int, Val>();

    public void CreateTree(Val val)
    {
        if (val.name == "经络腧穴")
        {
            view.AddData<CustomTreeItemData>(0, 1, val.name);
        }
        else
        {
            view.AddData<CustomTreeItemData>(index_parent[name_index[val.name]], name_index[val.name], val.name );
            index_val_dic.Add(name_index[val.name], val);
        }
        if (val.children.Length > 0)
        {
            for (int i = 0; i < val.children.Length; i++)
            {
                CreateTree(val.children[i]);
            }
        }
    }

    public void Readjson()
    {
        string jsonfile = PublicClass.filePath+"/AcuSearchTree.json";

        using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o = (JObject)JToken.ReadFrom(reader);
                Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                if (r.ContainsKey("AcuList"))
                {
                    acu_list =JsonConvert.DeserializeObject<List<Acu>>(r["AcuList"].ToString());
                }
            }
        }

    }

}
public class Acu
{
    public string id;
    public Val val;
}

public class Val
{
    public string code;
    public string isOpen;
    public Val[] children;
    public string name;
    public string qiu;
    public string zhen;
    public string isShow;
    public int siblingIndex;
}

public class CustomTreeItemData : TreeItemData
{
    public Toggle toggle;

    protected override void Init(Transform value)
    {
        toggle = target.GetComponentInChildren<Toggle>();
    }
}