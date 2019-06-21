using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelSearch : MonoBehaviour {
    public Button reset;
    public static ModelSearch instance;
    private void Awake()
    {
        instance = this;
    }

    public void ModelSearchEvent(string names)
    {
        reset.onClick.Invoke();
        var namess = names.Split(',');
        XT_AllButton.Instance.OpenMultipleModel();
        foreach (string tmp in namess)
        {
            SceneModels.instance.ChooseModelByName(tmp);
        }
        XT_AllButton.Instance.ISO();
        foreach (string tmp in namess)
        {
            SceneModels.instance.ChooseModelByName(tmp);
        }
        XT_AllButton.Instance.OpenMultipleModel();
    //    SceneModels.instance.ChooseModelByName(texInfoList[num].name);
    }
}
