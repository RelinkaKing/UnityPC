using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Repository;
using Assets.Scripts.Model;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;
using VesalCommon;
using Newtonsoft.Json.Linq;

public class SignNewTest : MonoBehaviour
{
    public static SignNewTest instance;
    public GameObject sgnParent;
    public static App tmpApp;
    DbRepository<SignNewInfo> local_db;
    //DbRepository<SignNewTex> tex_db;
    public List<SignNewInfo> sgnInfoList;
    List<SignNewTex> texInfoList = new List<SignNewTex>();
    //public Dictionary<string, Model> dic=new Dictionary<string, Model>();
//    public Dictionary<Model, Texture2D> model_maskTex_dic = new Dictionary<Model, Texture2D>();
    //存放贴图名称以及贴图信息
    Dictionary<string, Texture2D> map_dic = new Dictionary<string, Texture2D>();

    [HideInInspector]
    public List<SignNewElement> sgnEleList;

    Material mymat;
    [HideInInspector]
    public GameObject objPa;
    Texture2D maskTexPNG;
    //public List<Material> mymat;

    public Text sgnChName;
    public Text sgnEnName;
    public Text sgnDes;

    [HideInInspector]
    public List<Model> models = new List<Model>();


    List<Color> sgnColList;
    Color chosCol;
    Color responseCol;
    //  Color selectedCol;
    //  Color answerCol;

    public GameObject sgnExpPanel;
    public GameObject sgnMenuPanel;
    bool isListOpen = false;


    //public List<texture_mask> maskList;



    // public string db_path;

    [HideInInspector]
    public GameObject lastLabel;
    [HideInInspector]
    public GameObject currentLabel;

    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
        if (PublicClass.app != null && PublicClass.app.app_type != "sign_new")
            return;
        try
        {

            GameObject.Find("XiaoRen").SetActive(false);
            GameObject.Find("CursorModeCanvas").SetActive(false);
            GameObject.Find("RightMenuManager").SetActive(false);
        }
        catch (System.Exception)
        {
            Debug.Log("rightmenumanager error");
        }

        sgnEleList = new List<SignNewElement>();
        sgnColList = new List<Color>();
        ReadSgnTable();
        Debug.Log("---------链接数据库成功-----------");

        // CreateTexInfo();
        GetModel();

        DebugLog.DebugLogInfo("----------肌肉起止点测试信息加载成功---------");

    }
    
    Dictionary<string, Texture2D> loaded_mark_list = new Dictionary<string, Texture2D>();//mark
    Dictionary<string, Texture2D> loaded_channal_list = new Dictionary<string, Texture2D>();//
    public Dictionary<Model, Texture2D> model_maskTex_dic = new Dictionary<Model, Texture2D>();
    public void GetModel()
    {
        mymat = new Material(Shader.Find("MyShader/SignSelect"));
        mymat.SetColor("_ResponseColor", Color.magenta);
        List<string> remove_repeatstr = new List<string>();
        Model[] tempAllmodel = SceneModels.instance.Get_init_models();
        DebugLog.DebugLogInfo("------Shader更换成功----------");
        objPa = PublicClass.Transform_parent.gameObject;
//        objPa = GameObject.Find("ModelParent");
        for (int i = 0; i < tempAllmodel.Length; i++)
        {
            string mark_texName = string.Empty;
            if (model_tex_dic.TryGetValue(tempAllmodel[i].name, out mark_texName))
            {
                string mask_texName=main_mask_dic[mark_texName];
                Texture2D normMap = tempAllmodel[i].GetComponent<MeshRenderer>().material.GetTexture("_BumpMap") as Texture2D;
                tempAllmodel[i].ChangeMaterial(mymat);
                if (!loaded_mark_list.ContainsKey(mark_texName))
                {
                    Texture2D mainMap = GetTextureWithName(mark_texName + ".png");
                    loaded_mark_list.Add(mark_texName, mainMap);
                }

                if (!loaded_channal_list.ContainsKey(mask_texName))
                {
                    Texture2D maskMap = GetTextureWithName(mask_texName + ".png");
                    loaded_channal_list.Add(mask_texName, maskMap);
                }

                tempAllmodel[i].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", loaded_mark_list[mark_texName]);
                tempAllmodel[i].GetComponent<MeshRenderer>().material.SetTexture("_MaskTex", loaded_channal_list[mask_texName]);
                tempAllmodel[i].GetComponent<MeshRenderer>().material.SetTexture("_NormalMap", normMap);
            }
        }
        DebugLog.DebugLogInfo("--------加载贴图成功-------------");
    }

    public void OpenSSPList()
    {

        if (!isListOpen)
        {
            sgnMenuPanel.SetActive(true);
            isListOpen = true;
        }
        else
        {

            sgnMenuPanel.SetActive(false);
            isListOpen = false;
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (PublicClass.app != null && PublicClass.app.app_type != "sign_new")
            return;
    #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {

            Debug.Log("r已被按下");
            for (int i = 0; i < sgnEleList.Count; i++)
            {
                Debug.Log("进入循环，查找目前记录的物体");
                if (sgnEleList[i].gameObject.GetComponent<Image>().color == new Color(33 / 255.0f, 167 / 255.0f, 212 / 255.0f, 85 / 255.0f))
                {
                    camera_params str = new camera_params();
                    str.camera_pos = new vector3();
                    str.camera_pos.x = Camera.main.transform.position.x;
                    str.camera_pos.y = Camera.main.transform.position.y;
                    str.camera_pos.z = Camera.main.transform.position.z;

                    str.camera_parent_rot = new vector3();
                    str.camera_parent_rot.x = Camera.main.transform.parent.transform.eulerAngles.x;
                    str.camera_parent_rot.y = Camera.main.transform.parent.transform.eulerAngles.y;
                    str.camera_parent_rot.z = Camera.main.transform.parent.transform.eulerAngles.z;

                    str.camera_parent_pos = new vector3();
                    str.camera_parent_pos.x = Camera.main.transform.parent.transform.position.x;
                    str.camera_parent_pos.y = Camera.main.transform.parent.transform.position.y;
                    str.camera_parent_pos.z = Camera.main.transform.parent.transform.position.z;
                    sgnInfoList[i].camera_params = JsonConvert.SerializeObject(str);
                    Debug.Log(sgnInfoList[i].camera_params);
                    Debug.Log("记录摄像机数据成功");
                }
            }


        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            local_db.DataService("vesali.db");
            for (int i = 0; i < sgnInfoList.Count; i++)
            {
                Debug.Log(sgnInfoList[i].sm_ch_name);
            }
            Debug.Log("打印表数据成功");
            local_db.Close();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            // local_db.DataService("StartStop.db");
            local_db.DataService("vesali.db");
            UpdateSSPTable();
            Debug.Log("写入摄像机数据成功");
            local_db.Close();
        }
    #endif

        if (ChooseModel.CheckGuiRaycastObjects()) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                string model_name = hit.transform.gameObject.GetComponent<Model>().name;
                if (model_tex_dic.ContainsKey(model_name))
                {
                    string str = model_tex_dic[model_name];
                    if (main_mask_dic.ContainsKey(str))
                    {
                        string str_2 = main_mask_dic[str];
                        maskTexPNG = loaded_channal_list[str_2];
                    }
                }
                Vector2 pixelUV = hit.textureCoord2;
                pixelUV.x *= maskTexPNG.width;
                pixelUV.y *= maskTexPNG.height;
                chosCol = maskTexPNG.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                Debug.Log(chosCol);
                if (chosCol == Color.black) return;
                for (int i = 0; i < sgnInfoList.Count; i++)
                {
                    if (chosCol == sgnColList[i])
                    {
                        sgnExpPanel.SetActive(true);
                        sgnMenuPanel.SetActive(false);
                        isListOpen = false;
                        sgnChName.text = sgnInfoList[i].sm_ch_name;
                        sgnEnName.text = sgnInfoList[i].sm_en_name;
                        sgnDes.text = string.Format("<size=48><color=#00FFFF>相关介绍</color></size>：\r\n{0}", sgnInfoList[i].description);
                        objPa.BroadcastMessage("ChangeChoseColor", chosCol);
                        objPa.BroadcastMessage("ChangeHightColor", new Color(float.Parse(sgnInfoList[i].high_r) / 255.0f, float.Parse(sgnInfoList[i].high_g) / 255.0f, float.Parse(sgnInfoList[i].high_b) / 255.0f, 1));
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    void UpdateSSPTable()
    {
        for (int i = 0; i < sgnInfoList.Count; i++)
        { local_db.Update(sgnInfoList[i]); }
    }
#endif
    public void MenuReset()
    {
        DebugLog.DebugLogInfo("________________________________________________");
        SceneModels.ResetDistance(SceneModels.instance.Get_init_models()[0].transform.parent.transform);
    }


    //主贴图，遮罩贴图对应关系
    Dictionary<string, string> main_mask_dic = new Dictionary<string, string>();
    ////主贴图，子模型对应关系
    //Dictionary<string, string[]> tex_modelarr_dic = new Dictionary<string, string[]>();
    //子模型，主贴图对应关系
    Dictionary<string, string> model_tex_dic = new Dictionary<string, string>();
    List<string> model_name_list = new List<string>();
    void ReadSgnTable()
    {
        //产品表(包含产品用到的相关贴图，其中text_pair_list包含主贴图遮罩贴图的对应关系)
        DbRepository<noun_no_info> temp_db = new DbRepository<noun_no_info>();
        temp_db.DataService("vesali.db");
        IEnumerable<noun_no_info> noun_no_list = temp_db.Select<noun_no_info>((tmp) =>
        {
            if (tmp != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
        string[] ids_array = null;

        foreach (var item in noun_no_list)
        {
            if ((PublicClass.app != null && item.noun_no == PublicClass.app.app_id) || (tmpApp != null && tmpApp.app_id == item.noun_no))
            {
                ids_array = item.sign_new_ids.Split(',');
                main_mask_dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.text_pair_list);
                print("标注长度：" + ids_array.Length);
                break;
            }
        }
        temp_db.Close();

        //主贴图，子模型对应表<model,texture_name>
        DbRepository<GetTextureModelList> tex_db = new DbRepository<GetTextureModelList>();
        tex_db.DataService("vesali.db");
        IEnumerable<GetTextureModelList> tex_list = tex_db.Select<GetTextureModelList>((tmp) =>
        {
            if (main_mask_dic.ContainsKey(tmp.tex_name))
            {
                return true;
            }
            else
            {
                return false;
            }
        });
        string[] model_array = null;
        foreach (var item in tex_list)
        {
            if (item.tex_name != null)
            {
                model_array = item.model_list.Split(',');

                //tex_modelarr_dic.Add(item.tex_name, model_array);

                for (int i = 0; i < model_array.Length; i++)
                {
                    if (MainConfig.isLoadLocalSignNew)
                    {
                        model_array[i] = "o_" + model_array[i];
                    }
                    if (!model_tex_dic.ContainsKey(model_array[i]))
                    {

                        model_tex_dic.Add(model_array[i], item.tex_name);
                    }
                    else
                    {
                        Debug.Log(model_array[i] + "  repeat!!!!!!");
                    }

                    model_name_list.Add(model_array[i]);

                }
                print("模型长度：" + model_array.Length);
            }
        }
        tex_db.Close();

        //新标注信息总表
        local_db = new DbRepository<SignNewInfo>();
        local_db.DataService("vesali.db");
        var tmpsgn = local_db.Select<SignNewInfo>((ss_id) =>
        {
            if (ss_id != null)
            {
                //DebugLog.DebugLogInfo(ss_id.bone_name+" "+ss_id.main_tex+" "+ ss_id.mask_tex);
                return true;
            }
            else
            {
                return false;
            }

        });
        sgnInfoList = new List<SignNewInfo>();

        foreach (SignNewInfo tmp in tmpsgn)
        {
            for (int i = 0; i < ids_array.Length; i++)
            {
                if (tmp.id == ids_array[i])
                {
                    sgnInfoList.Add(tmp);
                    break;
                }
            }
        }
        Dictionary<string, SignNewInfo> temp_list = new Dictionary<string, SignNewInfo>();
        foreach (SignNewInfo tmp in sgnInfoList)
        {
            if (!temp_list.ContainsKey(tmp.sm_ch_name) && tmp.camera_params != null)
            {
                temp_list.Add(tmp.sm_ch_name, tmp);
            }
        
        }

        local_db.Close();
        //DebugLog.DebugLogInfo("名词："+PublicClass.app.app_id+" sign info list count :"+sgnInfoList.Count);
        for (int i = 0; i < sgnInfoList.Count; i++)
        {
            Color col = new Color(float.Parse(sgnInfoList[i].r) / 255.0f, float.Parse(sgnInfoList[i].g) / 255.0f, float.Parse(sgnInfoList[i].b) / 255.0f, 1);
            sgnColList.Add(col);
        }
        List<SignNewInfo> element_info_list = new List<SignNewInfo>();
        element_info_list.AddRange(temp_list.Values);
        print("element  changdu  " + element_info_list.Count);
        for (int i = 0; i < temp_list.Values.Count; i++)
        {
            GameObject tempMark = Instantiate(Resources.Load<GameObject>("Prefab/SGNElement"));
            // DebugLog.DebugLogInfo("读取下方菜单预制体");
            tempMark.transform.SetParent(sgnParent.transform);
            tempMark.transform.localScale = Vector3.one;
            tempMark.transform.localPosition = new Vector3(tempMark.transform.localPosition.x, tempMark.transform.localPosition.y, 1);
            tempMark.GetComponent<SignNewElement>().SetData(element_info_list[i]);
            tempMark.GetComponentInChildren<Text>().text = element_info_list[i].sm_ch_name;
            sgnEleList.Add(tempMark.GetComponent<SignNewElement>());
        }
    }


    void CreateTexInfo()
    {
        for (int i = 0; i < model_name_list.Count; i++)
        {
            SignNewTex tex = new SignNewTex();
            tex.mod_name = model_name_list[i];
            tex.main_tex = model_tex_dic[model_name_list[i]];
            tex.mask_tex = main_mask_dic[model_tex_dic[model_name_list[i]]];
            texInfoList.Add(tex);
        }
    }

    //从文件中获取图片
    public Texture2D GetTexture(string url)
    {
        Texture2D texture = new Texture2D(1024, 1024);
        FileStream fileStream = new FileStream(url, FileMode.Open, FileAccess.Read);
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        fileStream.Close();
        texture.LoadImage(bytes);
        return texture;
    }


    public Texture2D GetTextureWithName(string filename)
    {
        return GetTexture(File.ReadAllBytes(PublicClass.sign_texture_path+"/"+filename));
    }

    //从文件中获取图片
    public Texture2D GetTexture(byte[] bytes)
    {
        Texture2D texture = new Texture2D(1024, 1024);
        texture.LoadImage(bytes);
        return texture;
    }

   
}

public class fix_submodel{
    public string app_id { get; set; }
    public JObject submodel { get; set; }
}