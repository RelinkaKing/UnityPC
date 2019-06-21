using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.UI;
using SearchComponet;
using Assets.Scripts.Model;

public class SearchCfd : MonoBehaviour {

    public TreeView view;
    public TreeView line_view;

    public GameObject SearchContent;
    public GameObject SearchPanel;
    public GameObject TreePanel;
    public GameObject SearchParent;

    List<TriggerSubmodel> triggleList;
    Dictionary<int, TriggerSubmodel> search_index_dic = new Dictionary<int, TriggerSubmodel>();
    List<TriggerSubmodel> search_val_list = new List<TriggerSubmodel>();
    Dictionary<int, TriggerSubmodel> index_t_dic = new Dictionary<int, TriggerSubmodel>();

    void Awake()
    {
        //读触发点json 并对固定树数组进行初始化
        Readjson(ref triggleList, PublicClass.filePath + "json/trigger.json");
        //筛选json 数据
        FilterAcuList(PublicClass.app.app_id);
        //初始化固定树
        CreateTreeViewData(view, ref index_t_dic, triggleList);
    }

    private void Start()
    {
        SetSearchPanelActive(true);
    }

    //search 函数
    public void SearchWithName(InputField input)
    {
        print("search " + input.text);
        SearchPanel.SetActive(true);
        TreePanel.SetActive(false);
        for (int i = 0; i < SearchParent.transform.childCount; i++)
        {
            DestroyImmediate(SearchParent.transform.GetChild(i).gameObject, true);
        }
        search_val_list.Clear();
        for (int i = 0; i < triggleList.Count; i++)
        {
            if (triggleList[i].ch_name.Contains(input.text))
            {
                search_val_list.Add(triggleList[i]);
            }
        }
        CreateTreeViewData(line_view, ref search_index_dic, search_val_list,true);
    }

    public void BackTree()
    {
        SearchContent.SetActive(false);
        SearchPanel.SetActive(false);
        TreePanel.SetActive(true);
    }

    //固定树点击
    public void ClickIndex(int index)
    {
        TriggerSubmodel _val;
        bool isHave = index_t_dic.TryGetValue(index, out _val);
        if (isHave)
        {
            CFDControll.instance.SearchEvent(_val.trigger_no);
            SetSearchPanelActive(false);
        }
        else
        {
            Debug.LogError("not find index " + index);
        }
    }

    //search  结果点击
    public void ClickSearchIndex(int index)
    {
        TriggerSubmodel _val;
        bool isHave = search_index_dic.TryGetValue(index, out _val);
        if (isHave)
        {
            CFDControll.instance.SearchEvent(_val.trigger_no);
            SetSearchPanelActive(false);
        }
        else
        {
            Debug.LogError("not find index " + index);
        }
    }

    //创建树，并添加绑定事件
    void CreateTree(TreeView view, bool isChangeData = false)
    {
        view.CustomsChildSize(0, new Vector2(1000, 80));
        view.CustomsSpace(0, 80);
        view.onClick = (item, isOpen) =>
        {
            var target = item as CustomTreeItemData;
            target.toggle.isOn = isOpen;
            Debug.Log("点击了:" + item.name.text + " " + target.id);
            if (isChangeData)
            {
                ClickSearchIndex(target.id);
            }
            else {
                ClickIndex(target.id);
            }
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

    public void CreateTreeViewData(TreeView view, ref Dictionary<int, TriggerSubmodel> t, List<TriggerSubmodel> triggleList,bool isChangeData=false)
    {
        CreateTree(view,isChangeData);
        t.Clear();
        for (int i = 0; i < triggleList.Count; i++)
        {
            view.AddData<CustomTreeItemData>(0, i + 1, triggleList[i].ch_name);
            t.Add(i + 1, triggleList[i]);
        }
        view.Generate();
    }

    void FilterAcuList(string app_id)
    {
        List<TriggerSubmodel> tem_acu_list = new List<TriggerSubmodel>();
        tem_acu_list.AddRange(triggleList);
        triggleList.Clear();
        for (int i = 0; i < tem_acu_list.Count; i++)
        {
            if (tem_acu_list[i].noun_no == app_id)
            {
                triggleList.Add(tem_acu_list[i]);
            }
        }
    }

    public void Readjson(ref List<TriggerSubmodel> triggleList,string path)
    {
        string jsonfile = path;
        using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o = (JObject)JToken.ReadFrom(reader);
                Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                if (r.ContainsKey("trigger"))
                {
                    triggleList = JsonConvert.DeserializeObject<List<TriggerSubmodel>>(r["trigger"].ToString());
                }
            }
        }
    }

    public void SetSearchPanelActive(bool active)
    {
        SearchContent.SetActive(active);
    }
}