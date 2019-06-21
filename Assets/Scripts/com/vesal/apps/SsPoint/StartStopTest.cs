using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Repository;
using Assets.Scripts.Model;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;
using VesalCommon;


public class param {
    public string name;
    public Color color;
    public Color color2;
}
public class StartStopTest : MonoBehaviour
{


    [Tooltip("下方菜单列表生成的父物体")]
    public GameObject Parent;

    [Tooltip("标题显示框")]
    public Text ChName;
    [Tooltip("副标题显示框")]
    public Text BiName;
    [Tooltip("描述内容显示框")]
    public Text Description;
    [Tooltip("菜单切换栏")]
    public GameObject ExpPanel;
    [Tooltip("下方信息显示框")]
    public GameObject MenuPanel;
    [Tooltip("清除肌肉按钮")]
    public GameObject ClearMuscleBtn;
    //是否关闭下方列表
    protected bool isListOpen = false;

   
    DbRepository<StartStopInfo> local_db;
    DbRepository<StartStopTex> tex_db;
   
    List<StartStopPoint> sspList;
    public List<StartStopInfo> sspInfoList;
    List<StartStopTex> texInfoList;
    public Dictionary<string, Model> dic=new Dictionary<string, Model>();

    [HideInInspector]
    public List<StartStopElement> sspEleList;

    
    Material mymat;
    [HideInInspector]
    public GameObject objPa;
    
    Texture2D maskTexPNG;
    public Dictionary<Model,Texture2D> model_maskTex_dic=new Dictionary<Model, Texture2D>();


    [HideInInspector]
    public List<Model> models = new List<Model>();

    Color chosCol;
    Color transCol;

    public string db_path;

    GameObject currentMuscle;
    [HideInInspector]
    public GameObject lastMuscle;
    [HideInInspector]
    public GameObject lastLabel;
    [HideInInspector]
    public GameObject currentLabel;

    //点击次数
    int clickTimes=0;

    void Start() {

        if (PublicClass.app.app_type != "sign_ssp")
            return;

        db_path=AppOpera.sign_ssp_path+PublicClass.app.ab_path;

        try
        {
            
            GameObject.Find("RightMenuManager").SetActive(false);
            GameObject.Find("XiaoRen").SetActive(false);
            GameObject.Find("CursorModeCanvas").SetActive(false);
        }
        catch (System.Exception)
        {
            Debug.Log("rightmenumanager error");            
        }

        ClearMuscleBtn.SetActive(true);

        sspList = new List<StartStopPoint>();
        sspEleList = new List<StartStopElement>();
       
       
        ReadSSPTable();
        ReadTexTable();
        
       
        Debug.Log("---------链接数据库成功-----------");

        ReadTexture();
        GetModel();

        DebugLog.DebugLogInfo("----------肌肉起止点测试信息加载成功---------");
        PublicClass.appstate = App_State.Running;

    }

   
    void GetModel()
    {

        mymat = new Material(Shader.Find("MyShader/SignSelect"));
        dic = SceneModels.instance.getModels();
        Model[] tempAllmodel = SceneModels.instance.Get_init_models();
        objPa = GameObject.Find("ModelParent");

        DebugLog.DebugLogInfo("------Shader更换成功----------");


        for (int i = 0; i < tempAllmodel.Length; i++)
        {
            for (int j = 0; j < texInfoList.Count; j++)

                if (tempAllmodel[i].name == texInfoList[j].bone_name)
                {

                    Texture2D normMap = tempAllmodel[i].GetComponent<MeshRenderer>().material.GetTexture("_BumpMap") as Texture2D;
                    tempAllmodel[i].ChangeMaterial(mymat);
                    Texture2D mainMap=GetTextureWithName(texInfoList[j].main_tex+".png");
                    Texture2D maskMap = GetTextureWithName(texInfoList[j].mask_tex + ".png");
                    tempAllmodel[i].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", mainMap);
                    tempAllmodel[i].GetComponent<MeshRenderer>().material.SetTexture("_NormalMap", normMap);
                    tempAllmodel[i].GetComponent<MeshRenderer>().material.SetTexture("_MaskTex", maskMap);
                    if (!model_maskTex_dic.ContainsKey(tempAllmodel[i]))
                        model_maskTex_dic.Add(tempAllmodel[i], maskMap);
                }

        }

    }
    
    List<texture_mask> mask_list=new List<texture_mask>();
    List<string> list = new List<string>();



    void Update()
    {
        if (PublicClass.app.app_type != "sign_ssp")
            return;


        if (Input.GetKeyDown(KeyCode.T))
        {
            MakeStaticDBTable<StartStopInfo>.SetTextureDataToDB(PublicClass.filePath + "StartStop", db_path);
        }
#if UNITY_DEBUG

        if(Input.GetKeyDown(KeyCode.F1))
        {
           MakeStaticDBTable<StartStopInfo>.sign_UpdateTable(PublicClass.server_ip + "v1/app/xml/getAllStartStop",db_path,true);
        }


        if (Input.GetKeyDown(KeyCode.R))
        {

            Debug.Log("r已被按下");
            for(int i=0;i<sspEleList.Count;i++)
            {
                Debug.Log("进入循环，查找目前记录的物体");
                if(sspEleList[i].gameObject.GetComponent<Image>().color == new Color(33 / 255.0f, 167 / 255.0f, 212 / 255.0f, 85 / 255.0f))
                {
                    camera_params str=new camera_params();
                    str.camera_pos=new vector3();
                    str.camera_pos.x=Camera.main.transform.position.x;
                    str.camera_pos.y=Camera.main.transform.position.y;
                    str.camera_pos.z=Camera.main.transform.position.z;
                    
                    str.camera_parent_rot=new vector3();
                    str.camera_parent_rot.x=Camera.main.transform.parent.transform.eulerAngles.x;
                    str.camera_parent_rot.y=Camera.main.transform.parent.transform.eulerAngles.y;
                    str.camera_parent_rot.z=Camera.main.transform.parent.transform.eulerAngles.z;
                    
                    str.camera_parent_pos=new vector3();
                    str.camera_parent_pos.x=Camera.main.transform.parent.transform.position.x;
                    str.camera_parent_pos.y=Camera.main.transform.parent.transform.position.y;
                    str.camera_parent_pos.z=Camera.main.transform.parent.transform.position.z;
                    sspInfoList[i].camera_params = JsonConvert.SerializeObject( str);
                    // sspInfoList[i].app_id="SA0C01001";
                    Debug.Log("记录摄像机数据成功");
                }
            }

           
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            // local_db.DataService("StartStop.db");
            local_db.CreateDb(db_path);
            UpdateSSPTable();
            Debug.Log("写入摄像机数据成功");
            local_db.Close();

        }


#endif

        if (ChooseModel.CheckGuiRaycastObjects()) return;

        if (Input.GetMouseButtonDown(0))
        {

            List<Ray> ray_list = new List<Ray>();
            List<RaycastHit> hit_list = new List<RaycastHit>();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            ray_list.Add(ray);
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, 2, 0));
            ray_list.Add(ray1);
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, -2, 0));
            ray_list.Add(ray2);
            Ray ray3 = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(2, 0, 0));
            ray_list.Add(ray3);
            Ray ray4 = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(-2, 0, 0));
            ray_list.Add(ray4);



            for (int k = 0; k < ray_list.Count; k++)
            {
                RaycastHit hit;

                if (Physics.Raycast(ray_list[k], out hit))
                {

                    hit_list.Add(hit);
                    //for (int i = 0; i < sspInfoList.Count; i++)
                    //{

                    //dic[sspInfoList[i].mod_name].gameObject.SetActive(false);

                    //}                

                    if (model_maskTex_dic.ContainsKey(hit.transform.gameObject.GetComponent<Model>()))
                    {

                        maskTexPNG = model_maskTex_dic[hit.transform.gameObject.GetComponent<Model>()];

                    }
                    Vector2 pixelUV = hit.textureCoord2;

                    pixelUV.x *= maskTexPNG.width;
                    pixelUV.y *= maskTexPNG.height;
                    chosCol = maskTexPNG.GetPixel((int)pixelUV.x, (int)pixelUV.y);

                    if (chosCol == Color.black) continue;

                    clickTimes++;
                    for (int i = 0; i < sspList.Count; i++)
                    {
                        ExpPanel.SetActive(true);
                        MenuPanel.SetActive(false);
                        isListOpen = false;

                        if (sspList[i].startCol == new Vector3(chosCol.r, chosCol.g, chosCol.b))
                        {
                            ShowInfo(sspInfoList[i]);

                            objPa.BroadcastMessage("ChangeChoseColor", chosCol);
                            if (new Color(sspList[i].stopCol.x, sspList[i].stopCol.y, sspList[i].stopCol.z, 1) != Color.black)
                                objPa.BroadcastMessage("ChangeResColor", new Color(sspList[i].stopCol.x, sspList[i].stopCol.y, sspList[i].stopCol.z, 1));
                            else
                                objPa.BroadcastMessage("ChangeResColor", Color.magenta);

                            ShowMuscle(sspInfoList[i]);
                            return;

                        }
                        else if (sspList[i].stopCol == new Vector3(chosCol.r, chosCol.g, chosCol.b))
                        {

                            ShowInfo(sspInfoList[i]);

                            objPa.BroadcastMessage("ChangeResColor", chosCol);
                            if (new Color(sspList[i].startCol.x, sspList[i].startCol.y, sspList[i].startCol.z, 1) != Color.black)
                                objPa.BroadcastMessage("ChangeChoseColor", new Color(sspList[i].startCol.x, sspList[i].startCol.y, sspList[i].startCol.z, 1));
                            else
                                objPa.BroadcastMessage("ChangeChoseColor", Color.magenta);

                            ShowMuscle(sspInfoList[i]);
                            return;

                        }


                    }
                }
                else
                {
                    ExpPanel.SetActive(false);
                    MenuPanel.SetActive(false);
                }

            }
        }

    }

    public void ShowInfo(StartStopInfo info)
    {

        ChName.text = info.ch_name;
        BiName.text = info.en_name;
        Description.text = string.Format("<size=48><color=#00FFFF>起点</color></size>：\r\n{0}\r\n<size=48><color=#00FFFF>止点</color></size>：\r\n{1}", info.start_desc, info.stop_desc);
    }

    public void ShowMuscle(StartStopInfo info)
    {
      
        dic[info.mod_name].gameObject.SetActive(true);
        //dic[info.mod_name].BecomeTranslucent();
        //dic[info.mod_name].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, 1, 1, 1f));


    }
    public void Clear()
    {

        if (clickTimes == 1)
        {

            ClearMuscle();
            clickTimes = 0;
        }

        else
        {
            clickTimes = 0;
            ClearMuscle();
            MenuPanel.SetActive(false);
            ExpPanel.SetActive(false);
            isListOpen = false;
            for (int i = 0; i < sspList.Count; i++)
            {

                objPa.BroadcastMessage("ChangeChoseColor", Color.magenta);
                objPa.BroadcastMessage("ChangeResColor", Color.magenta);

            }
        }
      
    }

    //清除肌肉
    public void ClearMuscle()
    {
        for (int i = 0; i < sspInfoList.Count; i++)
        {

            dic[sspInfoList[i].mod_name].gameObject.SetActive(false);

        }

    }

    public void MenuReset()
    {
        DebugLog.DebugLogInfo("________________________________________________");
        print(SceneModels.instance.Get_init_models()[0].transform.parent.name);
        //SceneModels.SetCameraPosition(PublicClass.Transform_parent.transform, Vector3.zero);
        if (PublicClass.app.app_type == "sign_ssp")
            GetModel();
        if (PublicClass.app.app_type == "sign_new")
            SignNewTest.instance.GetModel();
    }


    public void OpenSSPList()
    {

        if (!isListOpen)
        {
            MenuPanel.SetActive(true);
            isListOpen = true;
        }
        else
        {

            MenuPanel.SetActive(false);
            isListOpen = false;
        }

    }


    void UpdateSSPTable()
    {
        for (int i = 0; i < sspInfoList.Count; i++)
        { local_db.Update(sspInfoList[i]); }
    }



    //从数据库中读取起止点相关信息，并将起点颜色信息，止点颜色信息提取出来
    void ReadSSPTable()
        {
            Model[] models=SceneModels.instance.Get_scope_models();
            local_db = new DbRepository<StartStopInfo>();
            local_db.CreateDb(db_path);
            var tmpssp = local_db.Select<StartStopInfo>((mod_name) =>
            {
                if (mod_name != null)
                {
                    //DebugLog.DebugLogInfo(string.Format("起点（{0}，{1}，{2}）,止点（{3}，{4}，{5}）", mod_name.qr, mod_name.qg, mod_name.qb, mod_name.zr, mod_name.zg, mod_name.zb));
                    return true;
                }
                else
                {
                    return false;
                }

            });

            sspInfoList = new List<StartStopInfo>();

            foreach (StartStopInfo tmp in tmpssp)
            {
                for (int i = 0; i < models.Length; i++)
                {
                    if(tmp.mod_name==models[i].name)
                    {
                        sspInfoList.Add(tmp);
                    }
                }
            }

        local_db.Close();
        for (int i = 0; i < sspInfoList.Count; i++)
            {



            StartStopPoint point = new StartStopPoint(sspInfoList[i]);
                sspList.Add(point);


                GameObject tempMark = Instantiate(Resources.Load<GameObject>("Prefab/SSPElement"));
                // DebugLog.DebugLogInfo("读取下方菜单预制体");
                tempMark.transform.SetParent(Parent.transform);
                tempMark.transform.localScale = Vector3.one;
                tempMark.transform.localPosition = new Vector3(tempMark.transform.localPosition.x, tempMark.transform.localPosition.y, 1);
                // Debug.Log(tempMark);
                tempMark.GetComponent<StartStopElement>().SetData(sspInfoList[i]);
                tempMark.GetComponentInChildren<Text>().text = sspInfoList[i].ch_name;
                sspEleList.Add(tempMark.GetComponent<StartStopElement>());

                //muscleList.Add(sspInfoList[i].ch_name);

            }
        }


        Dictionary<string,Texture2D> map_dic=new Dictionary<string, Texture2D>();
       
        //读取贴图相关列表
        void ReadTexTable()
        {

        tex_db = new DbRepository<StartStopTex>();

        tex_db.CreateDb(db_path);

        var tmpssp = tex_db.Select<StartStopTex>((ss_id) =>
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
            texInfoList = new List<StartStopTex>();

            foreach (StartStopTex tmp in tmpssp)
            {
                if (tmp.bone_name != null)
                {
                    texInfoList.Add(tmp);
                }
        
            }

        tex_db.Close();
    }

    AssetBundle ab;
    void ReadTexture()
    {
        DirectoryInfo di = new DirectoryInfo(PublicClass.sign_texture_path);
        FileInfo[] fis = di.GetFiles();
        ab = AssetBundle.LoadFromFile(PublicClass.filePath + "/pic.assetbundle");
        foreach (FileInfo fi in fis)
        {
            map_dic.Add(fi.Name, GetTexture(File.ReadAllBytes(fi.FullName),fi.Name));
        }
        Debug.Log(map_dic.Count);
        ab.Unload(false);
    }

    public Texture2D GetTextureWithName(string filename)
    {
        Texture2D tem_tex = null;
        if (map_dic.TryGetValue(filename, out tem_tex))
        {
            return tem_tex;
        }
        else
        {
            Debug.LogError("无贴图数据：" + filename);
            return null;
        }

    }

   
    //从文件中获取图片
    public Texture2D GetTexture(byte[] bytes, string name )
        {
       // Texture2D texture = new Texture2D(1024, 1024);
        Texture2D texture2 = new Texture2D(1024, 1024);
        //texture.LoadImage(bytes);
        texture2 = ab.LoadAsset<Texture2D>(name);

       
        //   Debug.Log(texture2.format);
      

       
            //texture.LoadImage(texture.EncodeToPNG());

            // Debug.Log(texture.width + "--" + texture.height);
            return texture2;
        }

       
    }

    

   