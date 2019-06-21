using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBone : MonoBehaviour {
    public Material BoneMakerMat;
    public Texture BoneMakerTex;
    public GameObject DingziProfeb;
    public GameObject dingzi;
    List<GameObject> newBone;
    string db_path;
    public static NewBone instance;

    camera_params Oldcamera_Params = new camera_params();
    public GameObject searchBtn;
    public class camera_params
    {
        public vector3 camera_pos;
        public vector3 camera_parent_rot;
        public vector3 camera_parent_pos;
    }
    public GameObject boneLeftUI;
    public GameObject oldLeftUI;
    public GameObject oldRightUI;
    public struct initMessage
    {
        public string mark_noun_no;
        public string nail_no;
        public string nail_camera_params;
    }
    public struct nailMessage
    {
        public string nail_no;
        public string nail_camera_params;
    }

    DbRepository<Sheet2> temp_db = new DbRepository<Sheet2>();


    DbRepository<MarkNoun> getMarkNoun_db = new DbRepository<MarkNoun>();
    void Awake()
    {
        instance = this;
    }
    public Sheet2 thisacu;
    public Transform thisacuT;
#if UNITY_EDITOR
    
#endif
    void changeCamera(string camera)
    {
      
            Debug.Log(camera);
            camera_params str = JsonConvert.DeserializeObject<camera_params>(camera);

            Camera.main.transform.parent.position = PublicTools.vector2Vecotor(str.camera_parent_pos);
            Camera.main.transform.parent.rotation = Quaternion.Euler(PublicTools.vector2Vecotor(str.camera_parent_rot));
            Camera.main.transform.position = PublicTools.vector2Vecotor(str.camera_pos);
            float z = str.camera_parent_rot.z;
            if (z < 90 && z > -90)
                Interaction.instance.setParamValue2();
            else
                Interaction.instance.setParamValue3();
        
    }
    void changeCamera()
    {

       // Debug.Log(camera);
        camera_params str = JsonConvert.DeserializeObject<camera_params>(thisacu.camera_params);

        Camera.main.transform.parent.position = PublicTools.vector2Vecotor(str.camera_parent_pos);
        Camera.main.transform.parent.rotation = Quaternion.Euler(PublicTools.vector2Vecotor(str.camera_parent_rot));
        Camera.main.transform.position = PublicTools.vector2Vecotor(str.camera_pos);
        float z = str.camera_parent_rot.z;
        if (z < 90 && z > -90)
            Interaction.instance.setParamValue2();
        else
            Interaction.instance.setParamValue3();

    }
    void saveCamera()
    {
        Oldcamera_Params.camera_pos = new vector3();
        Oldcamera_Params.camera_pos.x = Camera.main.transform.position.x;
        Oldcamera_Params.camera_pos.y = Camera.main.transform.position.y;
        Oldcamera_Params.camera_pos.z = Camera.main.transform.position.z;

        Oldcamera_Params.camera_parent_rot = new vector3();
        Oldcamera_Params.camera_parent_rot.x = Camera.main.transform.parent.eulerAngles.x;
        Oldcamera_Params.camera_parent_rot.y = Camera.main.transform.parent.eulerAngles.y;
        Oldcamera_Params.camera_parent_rot.z = Camera.main.transform.parent.eulerAngles.z;

        Oldcamera_Params.camera_parent_pos = new vector3();
        Oldcamera_Params.camera_parent_pos.x = Camera.main.transform.parent.position.x;
        Oldcamera_Params.camera_parent_pos.y = Camera.main.transform.parent.position.y;
        Oldcamera_Params.camera_parent_pos.z = Camera.main.transform.parent.position.z;
    }

    public void initmark(string msg)
    {
        if (boneLeftUI.activeSelf)
        {
            CloseBoneMarkers();
        }

        initMessage initmessage = JsonConvert.DeserializeObject<initMessage>(msg);
        searchBtn.SetActive(false);
        ShowBoneMarkers(initmessage.mark_noun_no);
        SeletQiu(initmessage.nail_no, initmessage.nail_camera_params);

#if UNITY_EDITOR
        initshuju();

#endif
    }


    public  void initshuju()
    {
        
      //  Destroy(qiu[0].GetComponent<BoxCollider>());

    }
    public void selectqiuqiu(string msg)
    {
        nailMessage initmessage = JsonConvert.DeserializeObject<nailMessage>(msg);     
        SeletQiu(initmessage.nail_no, initmessage.nail_camera_params);
    }
    GameObject tGO;
    public GameObject xiaoren;

    public void ShowBoneMarkers(string modelName)
    {
        boneLeftUI.SetActive(true);
        oldLeftUI.SetActive(false);
        oldRightUI.SetActive(false);
        xiaoren.SetActive(false);


        saveCamera();
        MarkNoun mark = getMarkNoun(modelName);

        Camera.main.transform.parent.localPosition = Vector3.zero;
        Camera.main.transform.localPosition = new Vector3(0, 0, Camera.main.transform.localPosition.z);
      
        newBone = new List<GameObject>();

        //AssetBundle ab = AssetBundle.LoadFromFile(PublicClass.filePath + "MarkBone/markbonetexture");
        //Texture tx = ab.LoadAsset<Texture>(mark.map_name);
        //ab.Unload(false);

        //   ab = AssetBundle.LoadFromFile(PublicClass.filePath + "MarkBone/markqiu");
        AssetBundle ab = AssetBundle.LoadFromFile(PublicClass.filePath + "MarkBone/markmodel");
        dingzi = ab.LoadAsset<GameObject>(mark.nail_model_name);
        ab.Unload(false);
        dingzi = Instantiate(dingzi);

        Transform b = dingzi.transform.GetChild(dingzi.transform.childCount - 1);
        int a = b.childCount;
        float x = 0;
        float y = 0;
        float z = 0;
        if (a >= 1)
        {
            tGO = new GameObject("tGO");
            for (int i = 0; i < a; i++)
            {
                GameObject mo = b.GetChild(i).gameObject;              
                x += mo.transform.position.x;
                y += mo.transform.position.y;
                z += mo.transform.position.z;          
            }
            x = x / a;
            y = y / a;
            z = z / a;
            tGO.transform.position = new Vector3(-x, -y, -z);
            dingzi.transform.position = tGO.transform.position;
        }
        else
        {
            GameObject mo = b.gameObject;                   
            dingzi.transform.position = -mo.transform.position;

          //  dingzi.transform.position = -mo.transform.position;
            //Destroy(go.GetComponent<MeshCollider>());
            //go.GetComponent<MeshRenderer>().sharedMaterial = BoneMakerMat;
            //go.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", tx);
          //  newBone.Add(go);
        }
        clearnames(dingzi);
           PublicClass.Transform_parent.gameObject.SetActive(false);
            init();
#if UNITY_EDITOR
        initshuju();

#endif
    }

    void clearnames(GameObject go)
    {
        var childs = go.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in childs)
        {
            t.name = t.name.Split(' ')[0];
        }
    }
    public MarkNoun getMarkNoun(string name)
    {
        db_path = PublicClass.vesal_db_path + "/vesali.db";//AppOpera.acu_path+"SignAcu.db";


        getMarkNoun_db.CreateDb(db_path);
        var tmpsssss = getMarkNoun_db.SelectOne<MarkNoun>((tmp) =>
        {
            if (tmp.mark_noun_no == name)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
        return tmpsssss;
    }
    public void CloseBoneMarkers()
    {
        foreach (GameObject na in newBone)
        {
            Destroy(na);
        }
        Camera.main.transform.parent.position = PublicTools.vector2Vecotor(Oldcamera_Params.camera_parent_pos);
        Camera.main.transform.parent.rotation = Quaternion.Euler(PublicTools.vector2Vecotor(Oldcamera_Params.camera_parent_rot));
        Camera.main.transform.position = PublicTools.vector2Vecotor(Oldcamera_Params.camera_pos);
        float z = Oldcamera_Params.camera_parent_rot.z;
        if (z < 90 && z > -90)
            Interaction.instance.setParamValue2();
        else
            Interaction.instance.setParamValue3();

        
        boneLeftUI.SetActive(false);
        oldLeftUI.SetActive(true);
        searchBtn.SetActive(true);
        oldRightUI.SetActive(true);
        xiaoren.SetActive(true);
        qiu.Clear();
        if (tGO != null)
            Destroy(tGO);   
        Destroy(dingzi);
        PublicClass.Transform_parent.gameObject.SetActive(true);
    }
    public Transform[] xiaoqiu;

    void init()
    {
        int a = dingzi.transform.childCount;

        for (int i = 0; i < a - 1; i++)
        {
            qiu.Add(dingzi.transform.GetChild(i).gameObject);
        }
       //// xiaoqiu = dingzi.transform.GetChild(0).GetComponentsInChildren<Transform>();
       // Debug.Log(xiaoqiu.Length);
       // foreach (Transform t in xiaoqiu)
       // {
       //     if(t.transform.childCount == 0)
       //     qiu.Add(t.gameObject);
       // }
       
       // qiu.RemoveAt(qiu.Count - 1);
        Debug.Log(qiu.Count);
    }


    public void SeletQiu(string name,string camera_params)
    {
        foreach (GameObject t in qiu)
        {
            t.SetActive(false);
        }
      //  qiu[0].SetActive(true);
        foreach (GameObject t in qiu)
        {
            if (t.name == name)
                t.SetActive(true);
        }
        changeCamera(camera_params);
    }
    public Transform thisqiu;
    public RaycastHit RayInfo;
    public Material oldQiu;
    public Material newQiu;
    public Material toumingqiu;
    public List<GameObject> qiu = new List<GameObject>();
    private bool _rayResult = false;   
}
