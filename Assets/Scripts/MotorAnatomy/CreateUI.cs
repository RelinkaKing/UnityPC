using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[Hotfix]
public class CreateUI : MonoBehaviour {

    public GameObject activeMuscleNamePrefab;
    public GameObject secondaryMuscleNamePrefab;
    public List<SkinnedMeshRenderer> activeMeshRenderers = new List<SkinnedMeshRenderer>();
    public List<SkinnedMeshRenderer> secondaryMeshRenderers = new List<SkinnedMeshRenderer>();
    public Transform muscleNameParent;
    public Transform muscleShowButton;
    public Material highLightMuscleMat;
    public Material lastHighLightMuscleMat;
    public SkinnedMeshRenderer lastSkinnedMeshRenderer ;
    public List<GameObject> muscleButton = new List<GameObject>();

    public Sprite[] buttonImage;
    public Color[] buttonColor;

    public Text tittleText;
    [LuaCallCSharp]
    public void InitMuscleNameUI()
    {
        int num = activeMeshRenderers.Count;
       
        for (int i = 0; i < num; i++)
        {
            int a = i;
            SkinnedMeshRenderer skinnedMesh = activeMeshRenderers[a];
            GameObject go = Instantiate(activeMuscleNamePrefab, muscleNameParent);
            go.transform.GetChild(0).GetComponent<Text>().text = activeMeshRenderers[i].GetComponent<MuscleData>().CngName;
            activeMeshRenderers[i].GetComponent<MuscleData>().button = go.GetComponent<Button>();
            go.GetComponent<Button>().onClick.AddListener(delegate () { HighLightMuscleClike(skinnedMesh,go); });
            HighLightMuscleClike(skinnedMesh, go);
        }

        num = secondaryMeshRenderers.Count;

        for (int i = 0; i < num; i++)
        {
            int a = i;
            SkinnedMeshRenderer skinnedMesh = secondaryMeshRenderers[a];
            GameObject go = Instantiate(secondaryMuscleNamePrefab, muscleNameParent);
            go.transform.GetChild(0).GetComponent<Text>().text = secondaryMeshRenderers[i].GetComponent<MuscleData>().CngName;
            secondaryMeshRenderers[i].GetComponent<MuscleData>().button = go.GetComponent<Button>();
            go.GetComponent<Button>().onClick.AddListener(delegate () { HighLightMuscleClike(skinnedMesh,go); });
        }

        if (muscleNameParent.childCount<4)
        {
            muscleShowButton.GetComponent<RectTransform>().sizeDelta = new Vector2(0,muscleNameParent.childCount*100);
        }
        else
            muscleShowButton.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 350);
    }
    //public void AdjustmentMuscleNameUI()
    //{

    //    float y = muscleNameParent.transform.localPosition.y;
    //    Debug.Log(new Vector3(muscleNameParent.transform.localPosition.x, ((int)(y / 100) + 1) * 100, 0));
    //    if (y % 100 >= 50)
    //    {
    //        // iTween.MoveTo(muscleNameParent.gameObject, new Vector3(muscleNameParent.transform.localPosition.x, ((int)(y / 100) + 1) * 100, 0), 0.5f);
    //        muscleNameParent.transform.localPosition = new Vector3(muscleNameParent.transform.localPosition.x, ((int)(y / 100) + 1) * 100, 0);
    //    }
    //    else
    //    {
    //        //iTween.MoveTo(muscleNameParent.gameObject, new Vector3(muscleNameParent.transform.localPosition.x, ((int)(y / 100)) * 100, 0) * 100, 0.5f);
    //        muscleNameParent.transform.localPosition = new Vector3(muscleNameParent.transform.localPosition.x, ((int)(y / 100)) * 100, 0);
    //    }

    //}
    [LuaCallCSharp]
    void HighLightMuscleClike(SkinnedMeshRenderer skinnedMeshRenderer,GameObject go)
    {
        MuscleData muscleData = skinnedMeshRenderer.gameObject.GetComponent<MuscleData>();
        if (!muscleData.isHighLight)
        {
            skinnedMeshRenderer.sharedMaterial = muscleData.highLightMaterial;
            muscleData.isHighLight = true;
            tittleText.text = muscleData.CngName;
            if (muscleData.ismain)
            {
                go.GetComponent<Image>().color = buttonColor[0];
                go.transform.GetChild(1).GetComponent<Image>().sprite = buttonImage[0];
            }
            else
            {
                go.GetComponent<Image>().color = buttonColor[1];
                go.transform.GetChild(1).GetComponent<Image>().sprite = buttonImage[1];
            }
        }
        else
        {
            skinnedMeshRenderer.sharedMaterial = muscleData.localMaterial;
            muscleData.isHighLight = false;
            tittleText.text = "";
            if (muscleData.ismain)
            {
                go.GetComponent<Image>().color = buttonColor[2];
                go.transform.GetChild(1).GetComponent<Image>().sprite = buttonImage[2];
            }
            else
            {
                go.GetComponent<Image>().color = buttonColor[2];
                go.transform.GetChild(1).GetComponent<Image>().sprite = buttonImage[3];
            }
        }
      
       // lastSkinnedMeshRenderer
    }



}
