using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class SearchButton : MonoBehaviour
{

    public string modelName = string.Empty;
    public string LabelName;
    public Text text;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSelectModels()
    {
        // for (int i = 0; i < PublicClass.AllModels.Count; i++)
        // {
        //     if (!PublicClass.AllModels[i].isTranslucent)
        //         PublicClass.AllModels[i].BecomeNormal();
        // }
        // PublicClass.isMultiple = true;
        // PublicClass.currentModels.Clear();
        // PublicClass.currentModel = null;
        // XT_AllButton.Instance.closeMu.SetActive(false);
        // XT_AllButton.Instance.openMu.SetActive(true);
        // XT_AllButton.Instance.ExplainPanel.SetActive(true);
        // XT_AllButton.Instance.littlemapObject.transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0.12f, 0.24f, 0.135f);
        // SelectModels(LabelName);

    }


    void SelectModels(string temp)
    {
        // if (LoadModel.instance.pinyind.ContainsKey(temp))
        // {
        //     //string pinyin = string.Empty;
        //     //LoadModel.instance.pinyind.TryGetValue(temp,out pinyin);
        //     //DebugLog.DebugLogInfo(pinyin);
        //     //XT_TouchContorl.Instance.GiveMainModelInfo(pinyin);
        //     for (int i = 0; i < PublicClass.AllModels.Count; i++)
        //     {
        //         if (PublicClass.AllModels[i].name == LoadModel.instance.pinyind[temp])
        //         {
        //             PublicClass.AllModels[i].BecomeDisplay();
        //             PublicClass.AllModels[i].BecomeHight();
        //             PublicClass.currentModels.Add(PublicClass.AllModels[i]);
        //             PublicClass.currentModel = PublicClass.AllModels[i];
        //             break;
        //         }
        //     }
        // }
        // else
        // {
        //     List<string> tempList = new List<string>();
        //     bool isHavNext = LoadModel.instance.menuMessage.TryGetValue(temp, out tempList);
        //     if (isHavNext)
        //     {
        //         for (int i = 1; i < tempList.Count; i++)
        //         {
        //             SelectModels(tempList[i]);
        //         }
        //     }
        // }
        // Camera.main.GetComponent<XT_AllButton>().SearchCompleteClick();


    }
}
