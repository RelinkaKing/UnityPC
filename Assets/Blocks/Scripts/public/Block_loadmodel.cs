using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using VesalCommon;
using VesalDecrypt;

public class Block_loadmodel : MonoBehaviour
{

    public static Block_loadmodel Instance;
    [Header("model_Assets_info")]
    [Space(10)]
    public string Asset_ab_name = "framework.assetbundle";
    public string Prefab_Inner_name = "framework";
    public string Last_group_name = "GG_LuGu_Diff";
    public float SizeOf_Model = 1;

    [Header("Out_position_info")]
    [Space(10)]
    public DifficultyController difficultyControl;
    public Transform Model_parent;
    public Dictionary<string, string> named;


    [Header("Inner_Reset_info")]
    [Space(10)]
    public List<GameObject> Model_Group;
    GameObject realObj;
    Transform Manager_Trans;
    Transform last_Object;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BlockManager.Instance.OnDifficultyChange(PublicClass.Difficult_Index);

        AppOpera.game_path = Application.streamingAssetsPath + "/game/";

        ReadXml();
        ReadModel();
        // StartCoroutine(LoadPrefabModel());
    }

    //查询返回  对应平台语言的字典表
    void ReadXml()
    {
        named = new Dictionary<string, string>();
        XmlDocument docM = new XmlDocument();
        docM.Load(AppOpera.game_path + "/bonemodels.xml");
        XmlElement xmlElement = docM.DocumentElement;
        XmlNodeList xmlNodeList = xmlElement.ChildNodes;
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            string chinese = xmlNodeList[i].Attributes["chinese"].Value.ToString();
            string pinyin = xmlNodeList[i].Attributes["pinyin"].Value.ToString();
            named.Add(pinyin, chinese);
        }
    }

    Vector3 GetBound(Transform parent)
    {
        Bounds objBounds;
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        objBounds = new Bounds(parent.transform.position, Vector3.zero);
        for (int i = 0; i < renders.Length; i++)
        {
            objBounds.Encapsulate(renders[i].bounds);
        }
        return objBounds.center;
        // float length=Vector3.Distance(objBounds.max, objBounds.center);
    }
    IEnumerator LoadPrefabModel()
    {
        DebugLog.DebugLogInfo("--------487055555555555555555555552800-----" + PublicClass.Transform_parent.transform.Find("framework/GG_LuGu_Diff").childCount);

        Model_Group = new List<GameObject>();
        string temp_path = PublicClass.tempPath + "temp.dat";
        AssetBundleCreateRequest assetBundleCreateRequest = null;
        try
        {
            assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(Vesal_DirFiles.GetAbfile(PublicClass.filePath + Asset_ab_name, temp_path));
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.Log(ex.Message);
            UnityEngine.Debug.Log(ex.StackTrace);
        }
        
        if (assetBundleCreateRequest != null)
        {
            yield return assetBundleCreateRequest;
            GameObject obj = (GameObject)assetBundleCreateRequest.assetBundle.LoadAsset(Prefab_Inner_name, typeof(GameObject));
            realObj = Instantiate(obj);

            //载入模型的 信息初始化 destory多余脚本 添加block 脚本
            Manager_Trans = GameObject.Find(Last_group_name).transform;
            last_Object = Instantiate(Manager_Trans);
            last_Object.SetParent(Model_parent, true);
            last_Object.localPosition = Vector3.zero;
            last_Object.localScale *= SizeOf_Model;
            realObj.SetActive(false);
            Model[] remove_models = last_Object.GetComponentsInChildren<Model>();
            List<Model> ya_list = new List<Model>();
            for (int i = 0; i < remove_models.Length; i++)
            {
                if (remove_models[i].name.Contains("Ya"))
                {
                    ya_list.Add(remove_models[i]);
                    continue;
                }
                Model_Group.Add(remove_models[i].gameObject);
            }
            for (int i = 0; i < ya_list.Count; i++)
            {
                Destroy(ya_list[i].gameObject);
            }
            DebugLog.DebugLogInfo("拼图个数：  " + Model_Group.Count);
            Add_block_componet(Model_Group);
            BlockManager.Instance.InitTargetPosition();//记录显示模型的初始化位置
            //ChangeModel_position(Model_Group);//随机位置

            assetBundleCreateRequest.assetBundle.Unload(false);
        }
    }

    List<string>  GetNounoNameList(string app_id)
    {
        List<string> submodelList =new List<string>();
        var local_db = new DbRepository<GetStructList>();
        //获取名词范围
        local_db.DataService("vesali.db");
        GetStructList tmpIe = local_db.SelectOne<GetStructList>((tempNo) =>
        {
            if (tempNo.nounNo == app_id)
            {
                return true;
            }
            else
            {
                return false;
            }

        });
        if (tmpIe == null)
        {
            local_db.Close();
        }
        submodelList = new List<string>( JsonConvert.DeserializeObject<Dictionary<string, float>>(tmpIe.submodelList).Keys);
        return submodelList;
    }


    void ReadModel()
    {
        Model_Group = new List<GameObject>();


        // string temp_path = PublicClass.tempPath + "temp.dat";
        // Vesal_DirFiles.GetAbfile_Synchronize(PublicClass.filePath + Asset_ab_name, temp_path);

        // AssetBundle model = AssetBundle.LoadFromFile(temp_path);
        // DebugLog.DebugLogInfo("-------------" + Asset_ab_name);
        // //读取信息，显示目标模型
        // // AssetBundle model = AssetBundle.LoadFromFile(AppOpera.game_path + Asset_ab_name);
        // GameObject obj = (GameObject)model.LoadAsset(Prefab_Inner_name, typeof(GameObject));
        // realObj = Instantiate(obj);
        // model.Unload(false);

        // Vesal_DirFiles.DelFile(temp_path);

        //载入模型的 信息初始化 destory多余脚本 添加block 脚本
        //Model_parent = new GameObject("model").transform;//PublicClass.Transform_parent.transform.Find("framework/GG_LuGu_Diff");
        //last_Object = Instantiate(Manager_Trans);

        List<string> tempList = GetNounoNameList(PublicClass.app.app_id);
        for (int i = 0; i < tempList.Count; i++)
        {
            Transform temp_obj = PublicClass.Transform_parent.transform.Find("framework/GG/GG_LuGu/" + tempList[i]);
            Transform last_Object = Instantiate(temp_obj);
            last_Object.gameObject.SetActive(true);
            // Manager_Trans = GameObject.Find(Last_group_name).transform;
            last_Object.SetParent(Model_parent, true);
            last_Object.gameObject.SetActive(true);
            //last_Object.localPosition = Vector3.zero;
        }
        Model_parent.transform.localPosition = new Vector3(PublicClass.Transform_parent.transform.localPosition.x, PublicClass.Transform_parent.transform.localPosition.y+1.5f, PublicClass.Transform_parent.transform.localPosition.z-3.2f);// PublicClass.Transform_parent.transform.localPosition+2*Vector3.up ;
        Model_parent.transform.localRotation =Quaternion.Euler(-90,0,0);//PublicClass.Transform_parent.transform.localRotation;
        Model_parent.transform.localScale = 2 * Vector3.one;
        // realObj.SetActive(false);
        Model[] remove_models = Model_parent.GetComponentsInChildren<Model>();
        List<Model> ya_list = new List<Model>();

        string[] fix_model = GetFixModelName(PublicClass.Difficult_Index);

        for (int i = 0; i < remove_models.Length; i++)
        {
            if (remove_models[i].name.Contains("Ya"))
            {
                ya_list.Add(remove_models[i]);
                continue;
            }

            bool is_fix = false;
            for (int j = 0; j < fix_model.Length; j++)
            {
                if (remove_models[i].name.Contains(fix_model[j]))
                {
                    is_fix = true;
                    continue;
                }
            }
            if(!is_fix)
                Model_Group.Add(remove_models[i].gameObject);
        }
        for (int i = 0; i < ya_list.Count; i++)
        {
            Destroy(ya_list[i].gameObject);
        }
        DebugLog.DebugLogInfo("拼图个数：  " + Model_Group.Count);
        Add_block_componet(Model_Group);
        BlockManager.Instance.InitTargetPosition();//记录显示模型的初始化位置

        SetFrameMesh();


        ChangeModel_position(Model_Group);//随机位置
    }

    public Material wireFrame_m;
    Model[] models=null;
    Transform trans = null;
    public void SetFrameMesh()
    {
        trans=Instantiate(Model_parent);
        models = trans.GetComponentsInChildren<Model>();
        for (int i = 0; i < models.Length; i++)
        {
            Material transparent_m = models[i].GetComponent<MeshRenderer>().material;
            transparent_m.color = new Color(transparent_m.color.r, transparent_m.color.g, transparent_m.color.b,0.2f);
            SetTransparent(transparent_m);
            //models[i].GetComponent<MeshRenderer>().sharedMaterial = wireFrame_m;
            models[i].gameObject.SetActive(false);
        }
        trans.gameObject.SetActive(false);
    }

    public void SetTransparent(Material transparent_m)
    {
        transparent_m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        transparent_m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        transparent_m.SetInt("_ZWrite", 0);
        transparent_m.DisableKeyword("_ALPHATEST_ON");
        transparent_m.DisableKeyword("_ALPHABLEND_ON");
        transparent_m.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        transparent_m.renderQueue = 3000;
    }

    public void SetWireMeshWithName(string _name,bool is_active)
    {
        for (int i = 0; i < models.Length; i++)
        {
            if (_name == models[i].name)
            {
                models[i].gameObject.SetActive(is_active);
            }
            else {
                models[i].gameObject.SetActive(false);
            }
        } 
    }

    public void SetWireFrameActive(bool active)
    {
        if(trans!=null)
            trans.gameObject.SetActive(active);
    }

    public string[] GetFixModelName(int index)
    {
        string[] submodel_list=null;
        List<GameFixSubmodel> temp_fix_list = new List<GameFixSubmodel>();
        temp_fix_list.AddRange(Readjson<List<GameFixSubmodel>>(PublicClass.filePath+"json/GameConfig.json"));
        for (int i = 0; i < temp_fix_list.Count; i++)
        {
            if (temp_fix_list[i].app_id == PublicClass.app.app_id)
            {
                switch (index)
                {
                    case 0:
                        if (temp_fix_list[i].s_submodellist == "")
                            submodel_list = new string[0];
                        else
                            submodel_list = temp_fix_list[i].s_submodellist.Split(',');
                        break;
                    case 2:
                        if (temp_fix_list[i].d_submodellist == "")
                            submodel_list = new string[0];
                        else
                            submodel_list = temp_fix_list[i].d_submodellist.Split(',');
                        break;
                    default:
                        break;
                }
            }
        }
        if (submodel_list==null)
        {
            Debug.LogError("not found fix model config file");
        }
        return submodel_list;
    }


    public T Readjson<T>(string path) where T : new()
    {
        string jsonfile = path;
        T instance = new T();
        if (!File.Exists(path))
        {
            return instance;
        }
        using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o = (JObject)JToken.ReadFrom(reader);
                Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                if (r.ContainsKey("fix_model"))
                {
                    instance = JsonConvert.DeserializeObject<T>(r["fix_model"].ToString());
                }
            }
        }
        return instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            //ReadModel();
            //ChangeModel_position(Model_Group);
        }
    }


    [Header("Center bound")]
    [Space(10)]
    public Vector3 center;
    public Vector3 size;

    //添加控制脚本,控制模型进行拼图操作
    void Add_block_componet(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].AddComponent<Block>();
            objects[i].AddComponent<RotatableObject>();
            objects[i].AddComponent<DraggableObject>();
            TouchableObject tempInfo = objects[i].AddComponent<TouchableObject>();
            named.TryGetValue(objects[i].name, out tempInfo.ModelName);
            // print(objects[i].name+"  "+tempInfo.ModelName);
        }
    }

    //改变对应模型组位置
    void ChangeModel_position(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
            objects[i].transform.position = pos;
        }
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].transform.rotation = Quaternion.identity;
        }
        BlockManager.Instance.blocksNumber = objects.Count;
    }


    //物体选中时，绘制
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
    //unity 编辑器中，每帧绘制
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.5f);
        Gizmos.DrawWireCube(center, size);
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawSphere(transform.position, 1);
    }
}

public class GameFixSubmodel {
    public string s_submodellist { get; set; }
    public string d_submodellist { get; set; }
    public string app_id { get; set; }
}