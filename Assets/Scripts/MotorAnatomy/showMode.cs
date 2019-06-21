using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

[Hotfix]
public class showMode : MonoBehaviour
{
    public Material[] mainHighLightMaterials;
    public Material[] secondaryHighLightMaterials;
    public string showname;
    public string activeMuscleName;
    public string secondaryMuscleName;
    public string[] str;
    public string[] activeMuscleStr;
    public string[] secondaryMuscleStr;
    public GameObject model;
    Transform[] grandFa;
    public List<SkinnedMeshRenderer> activeMuscles = new List<SkinnedMeshRenderer>();
    public List<SkinnedMeshRenderer> secondaryMuscles = new List<SkinnedMeshRenderer>();
    public List<GameObject> muscle = new List<GameObject>();
    public List<GameObject> bones = new List<GameObject>();
    public GameObject[] showp;
    public CreateUI createUI;
    public Material MAT;
    public UIFunction uiFunction;
    public Camera uicamera;
    public string fa;
    public string[] fas;
    public string[] MuscleCngName;
    public string[] MuscleEngName;
    public Texture2D[] muscleTex;
    // Use this for initialization
    [LuaCallCSharp]
    public void Init()
    {
        str = showname.Split(',');
        activeMuscleStr = activeMuscleName.Split(',');
        if (secondaryMuscleName != null)
            secondaryMuscleStr = secondaryMuscleName.Split(',');
        if (PublicClass.modelAndChild == null)
        {
            grandFa = model.GetComponentsInChildren<Transform>();
            PublicClass.modelAndChild = grandFa;
        }
        else
        {
            if (PublicClass.modelAndChild[0] == null)
            {
                grandFa = model.GetComponentsInChildren<Transform>();
                PublicClass.modelAndChild = grandFa;
            }
            else
                grandFa = PublicClass.modelAndChild;
        }
        DebugLog.DebugLogInfo("Start model Count"+grandFa.Length);
        fas = fa.Split(',');

        for (int i = 0; i < muscleTex.Length; i++)
        {
            mainHighLightMaterials[i].SetTexture("_MainTex", muscleTex[i]);
            secondaryHighLightMaterials[i].SetTexture("_MainTex", muscleTex[i]);
        }



        foreach (Transform child in grandFa)
        {
            child.gameObject.SetActive(false);
            for (int i = 0; i < str.Length; i++)
            {
                for (int j = 0; j < fas.Length; j++)
                {
                    if (child.name == fas[j])
                    {
                        child.gameObject.SetActive(true);
                        break;
                    }
                    //   
                }





                if (child.name == str[i])
                {
                    bool ismuscle = false;
                    child.gameObject.SetActive(true);

                    for (int j = 0; j < secondaryMuscleStr.Length; j++)
                    {
                        if (child.name == secondaryMuscleStr[j])
                        {
                            secondaryMuscles.Add(child.GetComponent<SkinnedMeshRenderer>());
                            child.gameObject.AddComponent<MeshCollider>();
                            child.gameObject.AddComponent<MuscleData>();
                            addMuscleData(child.gameObject, false);
                            muscle.Add(child.gameObject);
                            ismuscle = true;
                            break;
                        }

                    }

                    for (int j = 0; j < activeMuscleStr.Length; j++)
                    {
                        if (child.name == activeMuscleStr[j])
                        {
                            activeMuscles.Add(child.GetComponent<SkinnedMeshRenderer>());
                            child.gameObject.AddComponent<MeshCollider>();
                            child.gameObject.AddComponent<MuscleData>();
                            muscle.Add(child.gameObject);
                            addMuscleData(child.gameObject, true);
                            ismuscle = true;
                            break;
                        }

                    }

                    if (!ismuscle)
                    {
                        bones.Add(child.gameObject);
                    }
                    //  Debug.Log(child.GetComponent<SkinnedMeshRenderer>().sharedMaterial.name);
                    break;
                }

            }
        }
        //for (int i = 0; i < showp.Length; i++)
        //{
        //    showp[i].SetActive(true);
        //}
        createUI.activeMeshRenderers = activeMuscles;
        createUI.secondaryMeshRenderers = secondaryMuscles;
        createUI.InitMuscleNameUI();
        uiFunction.muscles = muscle;
        uiFunction.bones = bones;
        model.SetActive(true);
    }
    [LuaCallCSharp]
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < muscle.Count; i++)
        {
            SkinnedMeshRenderer skren = muscle[i].GetComponent<SkinnedMeshRenderer>();
            MeshCollider collider = skren.GetComponent<MeshCollider>();
            Mesh colliderMesh = new Mesh();
            skren.BakeMesh(colliderMesh);
            Destroy(collider.sharedMesh);
            collider.sharedMesh = null;
          
            collider.sharedMesh = colliderMesh;
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonUp(0)) //点击鼠标右键
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;                                                     //射线对象是：结构体类型（存储了相关信息）
            bool isHit = Physics.Raycast((Ray)ray, out hit);             //发出射线检测到了碰撞   isHit返回的是 一个bool值
            if (isHit && !EventSystem.current.IsPointerOverGameObject())
            {
                GameObject go = hit.transform.GetComponent<MuscleData>().button.gameObject;
                ExecuteEvents.Execute<IPointerClickHandler>(go.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);

            }

        }

#endif
        if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Ended) //点击鼠标右键
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;                                                     //射线对象是：结构体类型（存储了相关信息）
            bool isHit = Physics.Raycast((Ray)ray, out hit);             //发出射线检测到了碰撞   isHit返回的是 一个bool值
            if (isHit && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                GameObject go = hit.transform.GetComponent<MuscleData>().button.gameObject;
                ExecuteEvents.Execute<IPointerClickHandler>(go.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);

            }

        }
    }
    [LuaCallCSharp]
    void addMuscleData(GameObject child, bool ismain)
    {
        MuscleData muscleData = child.GetComponent<MuscleData>();

        for (int i = 0; i < MuscleCngName.Length; i++)
        {
            if (child.gameObject.name == MuscleEngName[i])
            {
                muscleData.CngName = MuscleCngName[i];
                break;
            }
        }

        muscleData.localMaterial = child.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        string name = child.GetComponent<SkinnedMeshRenderer>().sharedMaterial.name;
        if (ismain)
            switch (name)
            {
                case "DB_Diff":
                    muscleData.highLightMaterial = mainHighLightMaterials[0];
                    break;
                case "DT_Diff":
                    muscleData.highLightMaterial = mainHighLightMaterials[1];
                    break;
                case "JB_Diff":
                    muscleData.highLightMaterial = mainHighLightMaterials[2];
                    break;
                case "JZ_Diff":
                    muscleData.highLightMaterial = mainHighLightMaterials[3];
                    break;
                case "QB_Diff":
                    muscleData.highLightMaterial = mainHighLightMaterials[4];
                    break;
                case "TB_Diff":
                    muscleData.highLightMaterial = mainHighLightMaterials[5];
                    break;
                case "XF_Diff":
                    muscleData.highLightMaterial = mainHighLightMaterials[6];
                    break;
                case "XT_Diff":
                    muscleData.highLightMaterial = mainHighLightMaterials[7];
                    break;

            }
        else
            switch (name)
            {
                case "DB_Diff":
                    muscleData.highLightMaterial = secondaryHighLightMaterials[0];
                    break;
                case "DT_Diff":
                    muscleData.highLightMaterial = secondaryHighLightMaterials[1];
                    break;
                case "JB_Diff":
                    muscleData.highLightMaterial = secondaryHighLightMaterials[2];
                    break;
                case "JZ_Diff":
                    muscleData.highLightMaterial = secondaryHighLightMaterials[3];
                    break;
                case "QB_Diff":
                    muscleData.highLightMaterial = secondaryHighLightMaterials[4];
                    break;
                case "TB_Diff":
                    muscleData.highLightMaterial = secondaryHighLightMaterials[5];
                    break;
                case "XF_Diff":
                    muscleData.highLightMaterial = secondaryHighLightMaterials[6];
                    break;
                case "XT_Diff":
                    muscleData.highLightMaterial = secondaryHighLightMaterials[7];
                    break;

            }
        muscleData.ismain = ismain;
    }

    public void closeSence()
    {
        for (int i = 0; i < muscle.Count; i++)
        {
            muscle[i].GetComponent<SkinnedMeshRenderer>().material = muscle[i].GetComponent<MuscleData>().localMaterial;
            muscle[i].GetComponent<MuscleData>().isHighLight =false;
        }


        foreach (Transform child in PublicClass.modelAndChild)
        {
            child.gameObject.SetActive(true);

        }
        DebugLog.DebugLogInfo("end model Count" + grandFa.Length);
        Debug.Log("successCloseSence");
    }
}
