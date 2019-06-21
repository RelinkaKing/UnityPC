using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VesalCommon;
/// <summary>
/// ※测验练习逻辑入口
/// </summary>
public class BankScriControl : MonoBehaviour
{
    //题库选择列表子项
    public GameObject originObject;
    //题库选择列表父物体
    public Transform parentTransForm;
    //退出面板
    public GameObject quitAppPanel;
    //当前测验文档路径
    public static string currentQaPath;
    //【我的班级】参数
    //选择的班级id
    public static string selectClassId = string.Empty;
    //班级信息
    public static ClassInfo[] classInfo;

    //public static GameObject QaController;
    private void Awake()
    {


        //#if UNITY_EDITOR
        //        LinkStart.message = "JGCY_GX";
        //        PublicClass.filePath = Application.persistentDataPath + "/";
        //#endif
        if (GlobalVariable.practiceState == PracticeState.classes || GlobalVariable.practiceState == PracticeState.errors)
        {
            Debug.Log("GlobalVariable.practiceState :" + GlobalVariable.practiceState);
            currentQaPath = PublicClass.filePath;
            return;
        }
        Debug.Log("BanckScript: " + JsonConvert.SerializeObject(PublicClass.app));
        string basePath = AppOpera.QA_path;
        currentQaPath = basePath + PublicClass.app.app_id + "_" + PublicClass.app.app_version;
        if (Directory.Exists(currentQaPath))
        {
            currentQaPath = Vesal_DirFiles.findFileInPath(currentQaPath, PublicClass.app.struct_code + ".xml");
        }
        else
        {
            StartCoroutine(transform.GetComponent<StartPanel>().requestFailed("资源获取失败，请返回重试！"));
        }


        //if (!File.Exists(currentQaPath + PublicClass.app.struct_code + ".xml"))
        //{
        //    Debug.Log("BanckScript: " + currentQaPath + PublicClass.app.struct_code + ".xml");

        //    Debug.Log("BanckScript:  不存在xml,解压文件夹。");
        //    //pass
        //    string firstDirName = string.Empty;
        //    Vesal_DirFiles.ExtractAppData(ref basePath, ref firstDirName);
        //    if (File.Exists(currentQaPath + PublicClass.app.struct_code + ".xml"))
        //    {
        //        //pass
        //    }
        //    else if (File.Exists(currentQaPath + firstDirName + "/" + PublicClass.app.struct_code + ".xml"))
        //    {
        //        currentQaPath = currentQaPath + firstDirName + "/";
        //    }
        //    else
        //    {

        //        StartCoroutine(transform.GetComponent<StartPanel>().requestFailed("资源获取失败，请返回重试！"));
        //    }
        //    Debug.Log(firstDirName);
        //}
        GlobalVariable.bankName = PublicClass.app.struct_code;
        Debug.Log(currentQaPath);
        Debug.Log(GlobalVariable.bankName);
    }

    /// <summary>
    /// 开始时加载题库列表
    /// </summary>
    void Start()
    {



        if (selectClassId != string.Empty)
        {
            if (GlobalVariable.practiceState == PracticeState.classes)
            {
                initClassInfo();
                GlobalVariable.bankName = "classId_" + selectClassId + "_QAXML";
                Debug.Log(GlobalVariable.bankName);
            }
            else if (GlobalVariable.practiceState == PracticeState.errors)
            {
                GlobalVariable.bankName = "classId_" + selectClassId + "_Error_QAXML";
                Debug.Log(GlobalVariable.bankName);
            }
            this.transform.GetComponent<StartPanel>().changeBankName();
        }
        else
        {
            //正常加载QA系统 ， 初始化题库旋转列表
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc = Vesal_DirFiles.loadXml(currentQaPath, GlobalVariable.bankName + ".xml");

            if (xmlDoc == null)
            {
                Debug.Log("BankScriControl missing Xml");
                return;
            }
            string xPath = "//banks";
            XmlNodeList node = xmlDoc.SelectSingleNode(xPath).ChildNodes;

            foreach (XmlElement nodeElement in node)
            {
                InstantiateList(nodeElement["bankName"].InnerText,
                    nodeElement["tips"].InnerText,
                    nodeElement["fileName"].InnerText);
            }
            //获得班级题库
            // StartCoroutine(getClassInfo());
        }
    }
    /// <summary>
    /// 初始化班级信息，生成题库列表
    /// </summary>
    public void initClassInfo()
    {
        if (classInfo != null)
        {
            foreach (ClassInfo tmpInfo in classInfo)
            {
                string filePath = PublicClass.filePath + "classId_" + tmpInfo.id + "_QAXML.xml";
                Debug.Log("initClassInfo:" + filePath);
                if (File.Exists(filePath))
                {
                    InstantiateList(tmpInfo.name, "代课老师：" + tmpInfo.teaName, "classId_" + tmpInfo.id + "_QAXML");
                }
            }
        }
    }
    /// <summary>
    /// 生成一个题库选择子项
    /// </summary>
    /// <param name="bankName">题库名</param>
    /// <param name="tips">提示</param>
    /// <param name="filename">关联的xml名</param>
    public void InstantiateList(string bankName, string tips, string filename)
    {
        GameObject g = GameObject.Instantiate(originObject, parentTransForm);
        g.SetActive(true);
        g.GetComponent<RectTransform>().anchoredPosition = new Vector2(g.GetComponent<RectTransform>().anchoredPosition.x, originObject.GetComponent<RectTransform>().anchoredPosition.y);

        //Button buttonTmp = g.GetComponentInChildren<Button>();
        Button buttonTmp = g.GetComponent<Button>();
        //buttonTmp.transform.name;//题库名
        buttonTmp.transform.name = filename;
        g.transform.Find("BankName").GetComponent<Text>().text = bankName;
        g.transform.Find("tips").GetComponent<Text>().text = tips;

        UIEventListener btnListener = buttonTmp.gameObject.AddComponent<UIEventListener>();
        btnListener.OnClickEvent += delegate (GameObject gb)
        {
            //Debug.Log(gb.name + " OnClick");
            transform.GetComponent<StartPanel>().changeBankName(gb);
            //this.SelectButton(gb);
        };
        buttonTmp = g.transform.Find("Image").GetComponentInChildren<Button>();
        buttonTmp.transform.name = filename;
        //g.transform.Find("BankName").GetComponent<Text>().text = bankName;
        //g.transform.Find("tips").GetComponent<Text>().text = tips;
        btnListener = buttonTmp.gameObject.AddComponent<UIEventListener>();
        btnListener.OnClickEvent += delegate (GameObject gb)
        {
            //Debug.Log(gb.name + " OnClick");
            transform.GetComponent<StartPanel>().changeBankName(gb);
            //this.SelectButton(gb);
        };
    }

    /// <summary>
    /// 退出按钮
    /// </summary>
    public void ExitButton()
    {
        //冻结时间
        Time.timeScale = 0;
        //打开退出界面
        quitAppPanel.SetActive(true);
    }

    /// <summary>
    /// 确认、取消按钮
    /// </summary>
    /// <param name="isConfirm">是否是确认按钮</param>
    public void ExitChoice(bool isConfirm)
    {
        if (isConfirm)
        //如果是确认按钮
        {
            Time.timeScale = 1;
            ScenceData.clear();
            Debug.Log("BankScriControl quit!");
            Camera.main.GetComponent<XT_AllButton>().ExitProgram();
            //if (ClassPanelControll.initflag)
            //{
            //    AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //    AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            //    jo.Call("Quit");
            //}
            //else {
            //    Application.Quit();
            //}
            //退出程序
        }
        else
        //如果是取消按钮
        {
            Time.timeScale = 1;
            //解冻时间
            quitAppPanel.SetActive(false);
            //关闭退出界面
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape))
        {
            ExitButton();
        }
    }

    WebServicesBase wsb = new WebServicesBase();
    /// <summary>
    /// 请求班级信息
    /// </summary>
    /// <returns></returns>
    public IEnumerator getClassInfo()
    {
        // 广播会从自身开始查找这个DestoryMe方法，查找完自身后会查找所有子物体  
        //BroadcastMessage("DestoryMe");
        if (AppOpera.myClass != null && AppOpera.myClass.rykjMemberId != null)
        {

            KeyValue kv = new KeyValue("stuNo", AppOpera.myClass.rykjMemberId);

            UnityWebRequest uw = wsb.PostWithParams(servelets.getClasses, kv);

            yield return uw.Send();
            Debug.Log(uw.responseCode);
            Debug.Log(uw.downloadHandler.text);

            if (uw.isNetworkError)
            {
                Debug.Log(uw.error);

            }
            else
            {
                if (uw.downloadHandler.text.Contains("200"))
                {
                    WebServicesBase.addedClassId.Clear();
                    ResponseJson rj = new ResponseJson();
                    JObject jsonText = JObject.Parse(uw.downloadHandler.text);
                    // get JSON result objects into a list  
                    rj.stateCode = jsonText["stateCode"].Value<string>();
                    Debug.Log(jsonText["classesList"]);
                    classInfo = JsonConvert.DeserializeObject<ClassInfo[]>(jsonText["classesList"].ToString());
                    foreach (ClassInfo tmpClass in classInfo)
                    {
                        WebServicesBase.addedClassId.Add(tmpClass.id);
                        WebServicesBase.classnameDic.Add(tmpClass.id, tmpClass.name);
                    }

                }
            }

        }
        yield return StartCoroutine(wsb.getAllBankJson());
        initClassInfo();
    }
}
