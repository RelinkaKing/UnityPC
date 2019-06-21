using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchStructTree : MonoBehaviour
{
    public static SearchStructTree instance;
    public bool isEnterSearch = false;
    //public Model[] models;
    void Awake()
    {
        instance = this;
    }
    //void Start()
    //{
    //    models = SceneModels.instance.Get_scope_models();
    //}

    public void asrSearch(string str) {

    }
    public void CloseSearchTreePanel()
    {
        if (newSearch.activeSelf)
        {
            newSearch.SetActive(false);
            oldSearch.SetActive(true);

        }
        else
        {
            SearchCompleteClick();
        }
    }
    public void SearchCompleteClick()
    {
        isEnterSearch = false;
//        #if UNITY_EDITOR ||UNITY_STANDALONE_WIN
//         this.transform.gameObject.SetActive(false);
// #elif UNITY_ANDROID || UNITY_IOS
        this.transform.parent.gameObject.SetActive(false);
// #endif
    }

    public GameObject oldSearch, newSearch;

    //搜索菜单切换
    void RealSearchClick()
    {
        oldSearch.SetActive(false);
        newSearch.SetActive(true);
    }
    void FakerSearchClick()
    {
        oldSearch.SetActive(true);
        newSearch.SetActive(false);
    }
    bool RFS = false;
    public void RealFakerSwitch()
    {
        Debug.Log(RFS);
        if (RFS)
        {
            FakerSearchClick();
            RFS = false;
        }
        else
        {
            RealSearchClick();
            RFS = true;
        }
    }

    public GameObject RealSearchParent;

    public void OnTextChange(InputField inputField)
    {
        string name=inputField.text;
        if ((name.StartsWith("SA") || name.StartsWith("RA")) && name.Length == 9)
        {
            List<string> Sa_list = SceneModels.instance.getListByNo(name);
            if (Sa_list.Count > 0)
            {
                SearchCompleteClick();
                SceneModels.instance.DisplayModels(Sa_list);
                return;
            }

        }
        if (name != string.Empty)
        {
            for (int d = 0; d < RealSearchParent.transform.childCount; d++)
            {
                Destroy(RealSearchParent.transform.GetChild(d).gameObject);
            }
            //搜寻节点
            // List<string> tempAllSearch = new List<string>();
            // tempAllSearch.AddRange(LoadModel.instance.menuMessage.Keys);
            // for (int i = 0; i < tempAllSearch.Count; i++)
            // {
            //     if (tempAllSearch[i].Contains(name))
            //     {
            //         GameObject searchButton = Instantiate(LoadModel.instance.Catalog);
            //         searchButton.SetActive(true);
            //         searchButton.name = tempAllSearch[i];
            //         searchButton.GetComponent<MenuControl>().LabelName = tempAllSearch[i];
            //         searchButton.GetComponent<MenuControl>().text.GetComponent<Text>().text = tempAllSearch[i];
            //         searchButton.GetComponent<MenuControl>().arrow.GetComponentInChildren<Button>().interactable=false;//.SetActive(false);
            //         searchButton.transform.SetParent(RealSearchParent.transform);
            //         searchButton.transform.localPosition = Vector3.zero;
            //         searchButton.transform.localScale = Vector3.one;
            //     }
            // }
            //搜寻模型
            Model[] sceneSearchModels=SceneModels.instance.Get_scope_models();
            for (int i = 0; i < sceneSearchModels.Length; i++)
            {
                string temp=sceneSearchModels[i].name;
                if (PublicClass.infoDic.ContainsKey(temp))
                {
                    ModelInfo tempInfo = PublicClass.infoDic[temp];
                    if(tempInfo.Chinese.Contains(name))
                    {
                        GameObject searchButton = Instantiate(LoadModel.instance.Catalog);
                        searchButton.SetActive(true);
                        searchButton.name = tempInfo.Chinese;
                        searchButton.GetComponent<MenuControl>().LabelName = temp;
                        searchButton.GetComponent<MenuControl>().text.GetComponent<Text>().text = tempInfo.Chinese;
                        searchButton.GetComponent<MenuControl>().arrow.SetActive(false);
                        searchButton.transform.SetParent(RealSearchParent.transform);
                        searchButton.transform.localPosition = Vector3.zero;
                        searchButton.transform.localScale = Vector3.one;
                    }
                }
            }
            oldSearch.SetActive(false);
            newSearch.SetActive(true);
        }
    }
}