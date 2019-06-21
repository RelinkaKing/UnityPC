using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine.SceneManagement;
using XLua;

[Hotfix]
public class CameraSynchronization : MonoBehaviour
{
    public MeshRenderer[] littleMap;
    public Material[] littleMapTexture;
    public GameObject little_model;//导航模型
    Model temp = new Model();
    Dictionary<string, string> littleDic = new Dictionary<string, string>();
    public static CameraSynchronization instance;
    [LuaCallCSharp]
    void Start()
    {
        instance = this;
    }
    void Ismap_complete()
    {

    }

    //切换横竖屏小人位置
    public void Rotate_screen()
    {
        int z_rot = 180;
        Rect came_rect;
        if (little_model.transform.localEulerAngles.z == 180)
        {
            z_rot = -90;//0.86 ，0.06 ，0.1 ，0.17
            came_rect = new Rect(0.86f, 0.06F, 0.1f, 0.17f);
        }
        else
        {
            z_rot = 180;  //0 ，0.06 ，0.1 ，0.17
            came_rect = new Rect(0, 0.06F, 0.1f, 0.17f);
        }
        little_model.transform.localEulerAngles = new Vector3(little_model.transform.localEulerAngles.x, little_model.transform.localEulerAngles.y, z_rot);
        GetComponentInChildren<Camera>().rect = came_rect;
    }


    // void Update()
    // {   
    //     //位置
    //     GetComponent<Camera>().rect = new Rect(0, 46 / 768F, 0.128f, 0.128f * Screen.width / Screen.height);
    //     //背景色
    //     GetComponent<Camera>().backgroundColor = Camera.main.backgroundColor; 
    //     transform.parent.transform.rotation = Camera.main.transform.parent.transform.rotation;
    // }

    // Update is called once per frame
    void Update()
    {
        if ((PublicClass.currentModle == ModleChoose.MainModel))
        {

            this.transform.rotation = Camera.main.transform.parent.transform.rotation;
            if (temp != null && !temp.isSeleted)
            {

                for (int i = 0; i < littleMap.Length; i++)
                {
                    littleMap[i].material = littleMapTexture[0];
                }
                temp = null;
            }
            if (SceneModels.instance!= null &&  SceneModels.instance.getLastChoose() != temp)
            {
                string tempString = string.Empty;
                //非多选模式
                if (!SceneModels.instance.get_Multi_Selection() && SceneModels.instance.getLastChoose() != null)
                {
                    string model_name = SceneModels.instance.getLastChoose().name;
                    if (PublicClass.infoDic.ContainsKey(model_name))
                    {
                        tempString = PublicClass.infoDic[model_name].littleMap;

                        for (int i = 0; i < littleMap.Length; i++)
                        {
                            if (littleMap[i].name == tempString)
                            {
                                littleMap[i].material = littleMapTexture[1];
                            }
                            else
                            {
                                littleMap[i].material = littleMapTexture[0];
                            }
                        }
                        temp = SceneModels.instance.getLastChoose();
                    }
                }
                else
                {
                    for (int i = 0; i < littleMap.Length; i++)
                    {
                        littleMap[i].material = littleMapTexture[0];
                    }
                    for (int l = 0; l < SceneModels.instance.getSelects().Count; l++)
                    {
                        string model_name = SceneModels.instance.getSelects()[l].name;
                        if (PublicClass.infoDic.ContainsKey(model_name))
                        {
                            tempString = PublicClass.infoDic[model_name].littleMap;

                            //tempString = SceneModels.instance.getSelects()[l].Info.littleMap;
                            temp = SceneModels.instance.getSelects()[l];
                            for (int i = 0; i < littleMap.Length; i++)
                            {
                                if (littleMap[i].name == tempString)
                                {
                                    littleMap[i].material = littleMapTexture[1];
                                }
                            }
                        }
                    }
                }
            }
        }
        if ((PublicClass.currentModle == ModleChoose.SignModel))
        {
            this.transform.rotation = Camera.main.transform.parent.transform.rotation;
            if(ShowSignModel.littleMapInfo==null)
            {
                return;
            }
            else
            {
                string tempString = ShowSignModel.littleMapInfo.littleMap;
                for (int i = 0; i < littleMap.Length; i++)
                {
                    if (littleMap[i].name == tempString)
                    {
                        littleMap[i].material = littleMapTexture[1];
                    }
                    else
                    {
                        littleMap[i].material = littleMapTexture[0];
                    }
                }           
            }
        }
    }

    //void LoadLittleMapXml(Action Ismap_complete)
    //{
    //    string url = string.Empty;
    //    XmlDocument doc = new XmlDocument();
    //    doc.Load(PublicClass.filePath +"/"+ LinkStart.message + "littlemap.xml");
    //    XmlElement root = doc.DocumentElement;
    //    XmlNodeList littemap = root.ChildNodes;
    //    for(int lm = 0;lm<littemap.Count;lm++)
    //    {
    //        string tempValue = string.Empty;
    //        tempValue = littemap[lm].Attributes["name"].Value;
    //        XmlNodeList models = littemap[lm].ChildNodes;
    //        for(int m =0;m<models.Count;m++)
    //        {
    //            string tempKey = models[m].Attributes["name"].Value;
    //            try
    //            {
    //                littleDic.Add(tempKey, tempValue);
    //            }
    //            catch
    //            {
    //                Debug.Log(tempKey);
    //            }
    //        }
    //    }

    //    if(Ismap_complete!=null)
    //    {
    //        Ismap_complete();
    //    }
    //}

    //刷新小人高亮显示
    public void FlushLittleMapShow()
    {
        SceneModels.instance.set_Multi_Selection(false);

        for (int i = 0; i < littleMap.Length; i++)
        {
            littleMap[i].material = littleMapTexture[0];
        }
    }
    private void OnDestroy()
    {
        Destroy(this);
    }
}
