using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{

    public GameObject text, arrow, line, eye;
    public List<GameObject> childs = new List<GameObject>();
    public string modelName = string.Empty;
    public float PosY;

    public int pos; //当前菜单的层数
    public List<string> nameList = new List<string>();
    //子模型列表，可为空
    public List<string> modelNameList = new List<string>();
    public List<string> check_NameList = new List<string>();
    public string LabelName;
    bool IsCreateMenu = false;

    void Start()
    {
        check_NameList.Clear();
        if (nameList.Count == 1)
        {
            IsCreateMenu = false; //关闭按钮创建
            arrow.SetActive(false); //关闭小箭头，表示为最后一级
            eye.SetActive(false);          
        }
        else
        {
            IsCreateMenu = true;
        }

    }

    public bool switchMenu = true;

    //点击生成子物体，如果最有一级，选中模型
    public void MenuSwitch()
    {
        if (nameList.Count == 1)
        {
            Debug.Log(modelName);
            SelectModel();
        }
        else
        {
            if (IsCreateMenu)
            {
                OnCreateClick();
            }
            else
            {
                if (switchMenu)
                {
                    PickUpTheMenu(gameObject);
                }
                else
                {
                    ExpandTheMenu();
                }
            }
        }
    }

    public void SelectModel()
    {

    }

    public int siblingIndex; //记录物体transform

    //点击时，判断是否有子节点,如果有，进行生成
    //需要输入：  本节点名字 对应  子节点列表，列表计数为0，没有子节点
    //打开子节点，并为子节点赋值
    public void OnCreateClick()
    {
        GetComponent<MenuControl>().arrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
        siblingIndex = gameObject.transform.GetSiblingIndex();
        print("当前条目位置 "+siblingIndex);
        if (nameList.Count > 0)
        {
            pos = int.Parse(nameList[0]);
            List<string > outlist=new List<string>();
            for (int i = 1; i < nameList.Count; i++)
            {
                bool is_create_node=false;
                check_NameList.Clear();
                Select_check(nameList[i]);
                bool isbreak=true;
                // print(nameList[i]+"  总树形结构查询长度："+check_NameList.Count);
                //不是最后一级
                if(isbreak)
                {
                    for (int k = 0; k <check_NameList.Count; k++)
                    {
                        for (int m = 0; m < LoadModel.instance.models.Length; m++)
                        {
                            if(check_NameList[k]==LoadModel.instance.models[m].name)
                            {
                                // print("<color=#33FF00>   "+check_NameList[k]+" </color>");
                                is_create_node=true;
                                isbreak=false;;
                            }
                        }
                    }
                }
                if(is_create_node)
                {
                    // print("<color=#33FF00>   "+nameList[i]+" </color>");
                    outlist.Add(nameList[i]);
                }
            }

            //按父节点数组，生成子节点，每个子节点进行检查下一级赋值
            //遍历父节点记录的子节点
            for (int i = 0; i < outlist.Count; i++)
            {
                //判断是否包含子模型
                List<string> tem_nameList=new List<string>();
                LoadModel.instance.menuMessage.TryGetValue(outlist[i], out tem_nameList);
                GameObject game = Instantiate(LoadModel.instance.Catalog);
                game.SetActive(true);
                Transform trans = game.transform;
                trans.SetParent(LoadModel.instance.tree_panel_parent.transform);
                trans.localPosition = Vector3.zero;
                trans.localScale = Vector3.one;

                game.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex()+i+1);
                // print("<color=#33FF00>set siblingindex:"+game.transform.GetSiblingIndex()+" </color>");

                MenuControl menuControl = game.GetComponent<MenuControl>();
                menuControl.arrow.transform.localPosition += new Vector3(pos * 43f - 43f, 0, 0);
                menuControl.eye.transform.localPosition += new Vector3(pos * 43f - 43f, 0, 0);
                menuControl.text.transform.localPosition += new Vector3(pos * 43f - 43f, 0, 0);
                menuControl.text.GetComponent<Text>().fontSize = 42 - pos * 2;
                menuControl.line.SetActive(false);
                menuControl.LabelName = outlist[i];
                if(LoadModel.instance.english.ContainsKey(outlist[i]))
                {
                    string txt="";
                    LoadModel.instance.english.TryGetValue(outlist[i],out txt);
                    menuControl.text.GetComponent<Text>().text = txt;
                }
                else
                {
                    menuControl.text.GetComponent<Text>().text = outlist[i];
                }
                game.GetComponent<MenuControl>().nameList.AddRange(tem_nameList);
                game.name = outlist[i];
                if (!LoadModel.instance.named.ContainsKey(game.name))
                {
                    menuControl.modelName = string.Empty;
                }
                else
                {
                    menuControl.modelName = LoadModel.instance.named[game.name];
                }
                //排除中文显示错误
                if(!ContainChinese(menuControl.text.GetComponent<Text>().text))
                {
                    DebugLog.DebugLogInfo(menuControl.text.GetComponent<Text>().text+" 模型未找到中文");
                    game.SetActive(false);
                }
                else
                {
                   childs.Add(game);
                }
            }
        }
        IsCreateMenu = false; //关闭子节点创建功能，（已经创建完成）
    }

    
    void Select_check(string chinese)
    {
        //中文有下一级，递归
        if (ContainChinese(chinese))
        {
            List<string> tempList = new List<string>();
            bool isHavNext = LoadModel.instance.menuMessage.TryGetValue(chinese, out tempList);
            if (isHavNext)
            {
                for (int i = 1; i < tempList.Count; i++)
                {
                    Select_check(tempList[i]);
                }
            }
            else
            {
                DebugLog.DebugLogInfo("树形名称不存在");
            }
        }
        else
        {
            // print("-><color=#00FF00>" + "chinese  " + chinese + "</color>");
            check_NameList.Add(chinese);
        }
    }



    //点击模型组
    public void OnSelectModels()
    {
        SelectModels(LabelName);
        print("-><color=#F0FF02>" + "SearchCompleteClick  " + "</color>");
        SceneModels.instance.DisplayModels(modelNameList);
        // SceneModels.instance.OperaModelByNameList ((model) => { model.BecomeHight (); }, modelNameList);
        SendMessageUpwards("SearchCompleteClick");
    }

    //递归选择模型
    void SelectModels(string chinese)
    {
        //中文有下一级，递归
        if (ContainChinese(chinese))
        {
            List<string> tempList = new List<string>();
            bool isHavNext = LoadModel.instance.menuMessage.TryGetValue(chinese, out tempList);
            if (isHavNext)
            {
                for (int i = 1; i < tempList.Count; i++)
                {
                    SelectModels(tempList[i]);
                }
            }
            else
            {
                DebugLog.DebugLogInfo("树形名称不存在");
            }
        }
        else
        {
            // print("-><color=#00FF00>" + "chinese  " + chinese + "</color>");
            modelNameList.Add(chinese);
        }
    }

    void hide(Model model) { }

    //展开列表
    void ExpandTheMenu()
    {
        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].SetActive(true);
        }
        arrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
        switchMenu = true;
    }

    //收起列表
    void PickUpTheMenu(GameObject game)
    {
        for (int i = 0; i < game.GetComponent<MenuControl>().childs.Count; i++)
        {
            game.GetComponent<MenuControl>().childs[i].SetActive(false);
            PickUpTheMenu(game.GetComponent<MenuControl>().childs[i]);
        }
        game.GetComponent<MenuControl>().arrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
        game.GetComponent<MenuControl>().switchMenu = false;
    }

    static bool ContainChinese(string input)
    {
        string pattern = "[\u4e00-\u9fbb]";
        return Regex.IsMatch(input, pattern);
    }
    static bool ContainWord(string input)
    {
        string pattern = "[a-zA-Z]";
        return Regex.IsMatch(input, pattern);
    }

    string GetKeyFromValue(string v)
    {
        var keys = LoadModel.instance.named.Where(q => q.Value == v).Select(q => q.Key); //get all keys

        List<string> keyList = (from q in LoadModel.instance.named where q.Value == v select q.Key).ToList<string>(); //get all keys

        var firstKey = LoadModel.instance.named.FirstOrDefault(q => q.Value == v).Key; //get first key
        return firstKey;
    }
}