using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using Assets.Scripts.Public;
/// <summary>
/// ※我的班级逻辑入口
/// </summary>
public class ClassPanelControll : MonoBehaviour
{
    [Header("班级列表")]
    //子项
    public GameObject originObject;
    //父物体
    public Transform parentTransForm;
    [Space(5)]
    [Header("面板")]
    //退出应用面板
    public GameObject quitAppPanel;
    //消息提示面板
    public GameObject messagePanel;
    //添加班级面板
    public GameObject addClassPanel;
    //班级详情面板
    public GameObject TheClassPanel;
    //选择注册学校面板
    public GameObject RegisterPanel;
    //注册表单面板
    public GameObject RegisterInfoPanel;
    [Header("Texts")]
    //提示消息文本
    public Text message;
    //加入班级，输入班级code
    public Text inputClassId;
    //班级名称文本
    public Text ClassNameText;
    [Header("Button")]
    //刷新按钮
    public Button refreshButton;
    //添加班级按钮
    public Button addClassButton;
    //手机号
    public static string phoneNumber;
    //用户id
    public static string MemberId;
    //用户邮箱
    public static string email;

    [Header("Register")]
    //注册表单
    public Text[] RegisterParams;
    //web请求类
    WebServicesBase wsb = new WebServicesBase();
    //学生信息
    public static StuInfo sin;
//+----+-----------------+------------+--------+---------------------+------------+
//+----+-----------------+------------+--------+---------------------+------------+
//|  1 | 测试01          | 1          | 156375 | 2018-06-21 15:30:31 | 2018-04-26 |
//|  2 | 测试手机        | 1          | 423356 | 2018-06-13 15:30:41 | 2018-04-26 |
//|  3 | 李昕励测试      | 1          | 504197 | 2018-04-28 16:03:52 | 2018-04-26 |
//+----+-----------------+------------+--------+---------------------+------------+

//+----------+----------+--------+-----------+--------------+-----------+--------------+------------------+
//| 007      | 123456   |      2 |         1 | 老王         | 教师      | 1333311313   | NULL             |
//| 008      | 123456   |      3 |         1 | 老李         | 学生      |              |                  |
//| 333222   | 123456   |      3 |         1 | 老李         | 学生      | 13541242     |                  |
//| 1111ssss | 123456   |      3 |         1 | s1           | 学生      | 15666232345  |                  |
//| 13123ss  | 123456   |      3 |         1 | s2           | 学生      | 1566632345   |                  |
//| qqweqwe  | 123456   |      3 |         1 | s3           | 学生      | 15666325     |                  |
//| 13141414 | 123456   |      3 |         1 | S4           | 学生      | 15666213325  |                  |
//| 13141414 | 123456   |      3 |         1 | s6           | 学生      | 156662113325 |                  |
//| 41241    | 123456   |      3 |         1 | 案发时的     | 学生      | 1566613325   |                  |
//| 41241    | 123456   |      3 |         1 | asdf         | 学生      | 15613325     |                  |
//+----------+----------+--------+-----------+--------------+-----------+--------------+------------------+

    public void OnEnable()
    {
#if UNITY_EDITOR
        phoneNumber = "17629029506";
        //MemberId = "96FBBA30-A1BC-4BD3-A463-B82DC967FD70";
        MemberId = "C28C4F87-41A1-4782-8165-B5E662B94244";
        //SceneSwitch.mm.MemberId = MemberId;
        email = "";
        AppOpera.myClass = new AppOpera.MyClass
        {
            mbTell = phoneNumber,
            rykjMemberId = MemberId,
            mbEmail = email
        };
        PublicClass.filePath = Application.persistentDataPath + "/";
#endif
        GlobalVariable.practiceState = PracticeState.classes;
if(AppOpera.myClass!=null){
phoneNumber = AppOpera.myClass.mbTell;
MemberId = AppOpera.myClass.rykjMemberId;
email = AppOpera.myClass.mbEmail;
}else{
#if UNITY_EDITOR
            return;
#endif
            ExitChoice(true);
}
        //Application.OpenURL("http://127.0.0.1:8000/myclass/");
    }
    /// <summary>
    /// 等待指定时长
    /// </summary>
    /// <param name="seconds">指定时长</param>
    /// <returns></returns>
    IEnumerator wait(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
    }
    // Use this for initialization
    void Start()
    {
        //刷新班级列表
        refeshClass();
    }
    /// <summary>
    /// 获取班级信息
    /// </summary>
    /// <returns></returns>
    public IEnumerator getClassInfo()
    {
        // 广播会从自身开始查找这个DestoryMe方法，查找完自身后会查找所有子物体  
        //BroadcastMessage("DestoryMe");

        KeyValue kv = new KeyValue("stuNo", sin.no);

        UnityWebRequest uw = wsb.PostWithParams(servelets.getClasses, kv);
        message.text = "正在获取服务器数据······";
        messagePanel.SetActive(true);
        yield return uw.Send();
        Debug.Log(uw.responseCode);
        Debug.Log(uw.downloadHandler.text);

        if (uw.isNetworkError)
        {
            message.text = "连接失败";
            Debug.Log(uw.error);

        }
        else
        {
            if (uw.downloadHandler.text.Contains("200"))
            {
                message.text = "连接成功，正在加载数据！";
                WebServicesBase.addedClassId.Clear();
                ResponseJson rj = new ResponseJson();
                JObject jsonText = JObject.Parse(uw.downloadHandler.text);
                // get JSON result objects into a list  
                rj.stateCode = jsonText["stateCode"].Value<string>();
                Debug.Log(jsonText["classesList"]);
                classes = JsonConvert.DeserializeObject<ClassInfo[]>(jsonText["classesList"].ToString());
                //Debug.Log(jsonText["classesList"].Value<string>());
                //List<JToken> list = jsonText["classesList"].Children().ToList();
                //Debug.Log(list.Count());
                //classes = list.Values<ClassInfo>().ToArray();


                //List<JToken> list
                //.Children().ToList()
                //classes = list.Values<ClassInfo>().ToArray();
                //classes = 

                //ClassInfo[] tmpclass = new ClassInfo[4];
                //tmpclass[0] = new ClassInfo("1", "比尔`盖茨", "Windows从入门到放弃");
                //tmpclass[1] = new ClassInfo("2", "史蒂夫·乔布斯", "IOS从入门到放弃");
                //tmpclass[2] = new ClassInfo("3", "安迪·罗宾", "Android从入门到放弃");
                //tmpclass[3] = new ClassInfo("4", "林纳斯·托瓦兹", "Linux从入门到放弃");

                //MyArrays<ClassInfo> ma = new MyArrays<ClassInfo>();
                //ma.Items = tmpclass;
                //string jsonData = JsonUtility.ToJson(ma);
                ////uw.downloadHandler.text
                //Debug.Log(jsonData);
                //classes = JsonUtility.FromJson<MyArrays<ClassInfo>>(jsonData).Items;
                if (classes.Length == 0)
                {
                    message.text = "没有加入班级？点击右上方加入！";
                }
                else
                {
                    yield return resetClassList();
                }
            }
        }
        yield return wait(1.5f);
        messagePanel.SetActive(false);
    }
    /// <summary>
    /// 切换添加班级面板
    /// </summary>
    public void switchAddClassPanel()
    {
        if (sin == null)
        {
            refeshClass();
        }else{
            showClassPanel.SetActive(false);
            classInfoShowText.text = "";
            inputClassId.text = "";
            inputClassId.transform.parent.GetComponent<InputField>().text = "";
            //refreshButton.interactable = !refreshButton.interactable;
            addClassPanel.SetActive(!addClassPanel.activeSelf);
        }
    }
    /// <summary>
    /// 添加班级
    /// </summary>
    public void addClass()
    {

        if (sin == null) {
            refeshClass();
        }
        else {
            StartCoroutine(queryClassInfo());
        }

    }
    /// <summary>
    /// 确认加入班级
    /// </summary>
    public void EnsureAddClass()
    {


        StartCoroutine(addClassCoroutine());

    }
    [Header("show Class")]
    //展示班级信息面板
    public GameObject showClassPanel;
    //班级信息文本
    public Text classInfoShowText;
    //public GameObject showClassPanel;

    /// <summary>
    /// 查询班级信息
    /// </summary>
    /// <returns></returns>
    IEnumerator queryClassInfo() {

        messagePanel.SetActive(true);
        message.text = "正在获取服务器数据······";
        if (inputClassId.text.Trim() == "")
        {
            message.text = "班课号不能为空！！！";
            yield return wait(1.5f);
        }
        else
        {
            Debug.Log("加入班级" + inputClassId.text.Trim());
            KeyValue[] kvs = new KeyValue[1];
            //kvs[0] = new KeyValue("uuid", MemberId);
            kvs[0] = new KeyValue("code", inputClassId.text.Trim());
            UnityWebRequest uw = wsb.PostWithParams(servelets.isClassesExists, kvs);

            yield return uw.Send();
            message.text = "正在获取服务器数据······";
            string response = uw.downloadHandler.text;
            Debug.Log(response);
            if (!uw.isNetworkError && response.Contains("200") && response.Contains("classesList"))
            {
                showClassPanel.SetActive(true);
                ResponseJson rj = JsonConvert.DeserializeObject<ResponseJson>(response);
                classInfoShowText.text = "班级："+rj.classesList[0].name+"\n教师："+ rj.classesList[0].teaName;
            }
            else if (!uw.isNetworkError && response.Contains("409")) {
                message.text = "班课码不存在，请确认！";
                yield return wait(1.5f);
            } else
            {
                message.text = "未知错误，查询班课信息失败。";
                yield return wait(1.5f);
            }
        }
        messagePanel.SetActive(false);
        
    }
    /// <summary>
    /// 加入班级
    /// </summary>
    /// <returns></returns>
    IEnumerator addClassCoroutine()
    {
        messagePanel.SetActive(true);
        message.text = "正在获取服务器数据······";
        
        Debug.Log("加入班级" + inputClassId.text.Trim());
        KeyValue[] kvs = new KeyValue[2];
        kvs[0] = new KeyValue("uuid", MemberId);
        kvs[1] = new KeyValue("classCode", inputClassId.text.Trim());
        UnityWebRequest uw = wsb.PostWithParams(servelets.joinClass, kvs);

        yield return uw.Send();
        message.text = "正在获取服务器数据······";
        string response = uw.downloadHandler.text;
        Debug.Log(response);
        if (!uw.isNetworkError && response.Contains("200") && response.Contains("班课信息录入成功"))
        {
            message.text = "加入成功。";
            addClassPanel.SetActive(false);
            refeshClass();
        }
        else if (response.Contains("500") && response.Contains("重复录入班课信息"))
        {
            message.text = "重复录入班课信息";
            yield return wait(1.5f);
        }
        else if (response.Contains("500") && response.Contains("该班课时间已截止"))
        {
            message.text = "该班课时间已截止";
            yield return wait(1.5f);
        }
        else
        {
            message.text = "未知错误，加入失败。";
            yield return wait(1.5f);
        }
        

        messagePanel.SetActive(false);

    }
    /// <summary>
    /// 进入测试
    /// </summary>
    public void startPractice()
    {
        ChangeStartScence();
        //StartCoroutine(startPracticeCoroutine());
        //ClassListPanelControll.currentClassId;
    }
    /// <summary>
    /// 开始测试协程
    /// </summary>
    /// <returns></returns>
    IEnumerator startPracticeCoroutine()
    {
        //messagePanel.SetActive(true);
        //message.text = "正在获取服务器数据······";
        //message.text = "正在获取服务器数据······";
        //if (currentClassId == "")
        //{
        //    message.text = "ClassId异常！！！";
        //    yield return wait(1.5f);
        //}
        //else
        //{
        //    Debug.Log("进入答题" + currentClassId);
        //    KeyValue[] kvs = new KeyValue[2];
        //    kvs[0] = new KeyValue("phoneNumber", phoneNumber);
        //    kvs[1] = new KeyValue("classId", inputClassId.text);
        //    UnityWebRequest uw = wsb.PostWithParams(servelets.getClasses, kvs);
        //    uw.timeout = 3;
        //    uw.downloadHandler = new DownloadHandlerTexture();
        //    yield return uw.Send();
        //    message.text = "正在获取服务器数据······";

        //    if (!uw.isError && uw.downloadHandler.text.Contains("00010"))
        //    {


        //    }
        //    else
        //    {
        //        //message.text = "未知错误！！！";
        //        //yield return wait(1.5f);
        //    }

        //    ChangeStartScence();
        //}
        //messagePanel.SetActive(false);
        yield return null;

    }
    /// <summary>
    /// 切换至答题场景
    /// </summary>
    public void ChangeStartScence()
    {
        Debug.Log("进入答题--"+ PublicClass.filePath + "classId_" + currentClassId + "_QAXML.xml");
        if (File.Exists(PublicClass.filePath + "classId_" + currentClassId + "_QAXML.xml"))
        {
            GlobalVariable.practiceState = PracticeState.classes;
            BankScriControl.classInfo = classes;
            BankScriControl.selectClassId = currentClassId;
            SceneManager.LoadScene("Scenes/totalScence");
        }
        else
        {
            StartCoroutine(requestFailed("老师未布置作业！"));
        }


        //PublicClass.JsonMessage = "结构测验_骨学";
        //SceneManager.LoadScene("Scenes/Start");

    }
    /// <summary>
    /// 是否Mysql端注册
    /// </summary>
    /// <returns></returns>
    public IEnumerator isMySqlRegister()
    {
        messagePanel.SetActive(true);
        message.text = "正在获取服务器数据······";
        //当网络不可用时              
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //message.text = "网络不可达，请稍后再试。";
            yield return StartCoroutine(requestFailed());
            Debug.Log("网络不可用！");
        }
        else {
            KeyValue[] kv = new KeyValue[1];
            kv[0] = new KeyValue("uuid", MemberId);
            UnityWebRequest uw = wsb.PostWithParams(servelets.isStudentExists, kv);
            yield return uw.Send();
            string jsonData = uw.downloadHandler.text;
            if (!uw.isNetworkError)
            {
                Debug.Log(uw.downloadHandler.text);
                ResponseJson rj = new ResponseJson();

                //ResponseJson rj = JsonConvert.DeserializeObject<ResponseJson>(uw.downloadHandler.text);
                //var v = JsonConvert.DeserializeObject<dynamic>(uw.downloadHandler.text);
                try
                {
                    rj = JsonConvert.DeserializeObject<ResponseJson>(uw.downloadHandler.text);
                    //JObject jsonText = JObject.Parse(uw.downloadHandler.text);
                    // get JSON result objects into a list  
                    // rj.stateCode = jsonText["stateCode"].Value<string>();
                    // rj.message = jsonText["result"].Value<bool>().ToString();
                    //List<Newtonsoft.Json.Linq.JToken> listJToken = googleSearch["responseData"]["results"].Children().ToList();
                    //Debug.Log(JsonConvert.SerializeObject(rj));
                    Debug.Log(rj.stateCode);
                    Debug.Log(rj.message);
                    Debug.Log(rj.stuInfo);
                    Debug.Log(rj.result);
                    if (rj.stateCode == WebServicesBase.GetEnumDescription(stateCode.access) && (rj.message == "True" || rj.result == "true"))
                    {
                        Debug.Log("用户存在=====");
                        isReg = true;
                        sin = rj.stuInfo[0];
                        Debug.Log(sin.no);
                        StartCoroutine(getClassInfo());

                    }
                    else if (rj.stateCode == "409")
                    {
                        messagePanel.SetActive(false);

                        Debug.Log("开始注册协程");

                        //schoolListParent.DetachChildren();
                        clearToggle();
                        selectSchoolFlag = true;
                        if (rj.school != null && rj.school.Length != 0)
                        {
                            foreach (schoolInfo tmpInfo in rj.school)
                            {
                                addSchoolListItem(tmpInfo.name, tmpInfo.code);
                            }
                        }
                        else {
                            addSchoolListItem("西安欧亚学院", "OYXY");
                            addSchoolListItem("西安培华学院", "PHXY");
                        }
                        RegisterPanel.SetActive(true);
                        //StartCoroutine(sendRegisterParams());
                    }
                    else
                    {
                        StartCoroutine(requestFailed());
                    }
                }
                catch (Exception e) {
                    Debug.Log(e.Message);
                    Debug.Log(e.StackTrace);
                    StartCoroutine(requestFailed());
                }
                
            }
            else
            {
                StartCoroutine(requestFailed());
            }
        }

       
    }
    /// <summary>
    /// 清理注册学校选择列表
    /// </summary>
    public void clearToggle() {
        int sumTg = tg.transform.childCount;
        for (int i = 0; i < sumTg; i++)
        {
            if (tg.transform.GetChild(i).name != "T1")
            {
                Destroy(tg.transform.GetChild(i).gameObject);
            }
        }
    }
    /// <summary>
    /// 注册确认
    /// </summary>
    public void registerEnsure() {
        foreach (Text tmpT in RegisterParams)
        {
            if (tmpT.text.Trim() == "") {
                StartCoroutine(requestFailed("请完善基本信息！"));
                return;
            }
        }
        StartCoroutine(sendRegisterParams());
    }
    /// <summary>
    /// 发送注册参数
    /// </summary>
    /// <returns></returns>
    public IEnumerator sendRegisterParams()
    {
        messagePanel.SetActive(true);
        message.text = "正在获取服务器数据······";
        //Dictionary<string, string> tmp = new Dictionary<string, string>();
        //MyArrays<KeyValue> myArrays = new MyArrays<KeyValue>();
        //myArrays.Items = new KeyValue[RegisterParams.Length];
        KeyValue[] kvs = new KeyValue[3+ RegisterParams.Length];
        kvs[0] = new KeyValue("uuid", MemberId);
        kvs[1] = new KeyValue("phone", phoneNumber);
        kvs[2] = new KeyValue("email", email);
        //从注册表单获取数据代码，直接发送手机号邮箱和识别码，暂不用注册
        int i = 3;
        foreach (Text tmpT in RegisterParams)
        {
            KeyValue kv = new KeyValue();
            kv.key = tmpT.name.Trim();
            kv.value = tmpT.name == "no" ? selectSchool +tmpT.text.Trim(): tmpT.text.Trim();
            kvs[i++] = kv;
            //tmp.Add(tmpT.name,tmpT.text);
            //JsonUtility.FromJson<Serialization<String, bool>>(jsonStr).ToDictionary();
        }

        string jsonData = JsonConvert.SerializeObject(kvs);
        Debug.Log(jsonData);
        UnityWebRequest uw = wsb.PostWithParams(servelets.studentRegister, kvs);
        yield return uw.Send();
        Debug.Log(uw.downloadHandler.text);
        if (!uw.isNetworkError)
        {
            Debug.Log(uw.downloadHandler.text);
            ResponseJson rj = new ResponseJson();

            //ResponseJson rj = JsonConvert.DeserializeObject<ResponseJson>(uw.downloadHandler.text);
            //var v = JsonConvert.DeserializeObject<dynamic>(uw.downloadHandler.text);
            JObject jsonText = JObject.Parse(uw.downloadHandler.text);
            // get JSON result objects into a list  
            rj.stateCode = jsonText["stateCode"].Value<string>();
            rj.message = jsonText["result"].Value<string>();
            Debug.Log(rj.stateCode);
            if (rj.stateCode == "500")
            {
                StartCoroutine(requestFailed("该学号已注册"));
                Debug.Log(rj.stateCode);
            }
            else if (rj.stateCode == WebServicesBase.GetEnumDescription(stateCode.access) && rj.message == "学生信息注册成功")
            {
                Debug.Log("注册成功！");
                RegisterPanel.SetActive(false);
                RegisterInfoPanel.SetActive(false);
                refeshClass();
            }
            else {
                StartCoroutine(requestFailed("注册失败，请重试"));
            }
        }
        else {
            StartCoroutine(requestFailed());
        }

        //messagePanel.SetActive(false);
        //JsonConvert.DeserializeObject<Product>(output);



    }
    /// <summary>
    /// 请求失败时显示提示文本
    /// </summary>
    /// <param name="mess">提示文本</param>
    /// <returns></returns>
    public IEnumerator requestFailed(string mess = "数据请求失败，请刷新重试！") {
        messagePanel.SetActive(true);
        message.text = mess;
        yield return wait(1.5f);
        messagePanel.SetActive(false);
    }
    //是否已注册
    bool isReg = false;
    /// <summary>
    /// 更新班级列表
    /// </summary>
    public void refeshClass()
    {
        Debug.Log("更新班级列表");
        if (!isReg)
        {
            //没注册，开启注册协程
            StartCoroutine(isMySqlRegister());
        }
        else
        {
            //获取班级列表信息
            StartCoroutine(getClassInfo());
        }

    }
    //public void hidePanel(GameObject panel) {
    //    panel.SetActive
    //}

    //班级信息
    public static ClassInfo[] classes;
    /// <summary>
    /// 重置班级列表
    /// </summary>
    /// <returns></returns>
    IEnumerator resetClassList()
    {
        WebServicesBase.addedClassId.Clear();
        WebServicesBase.classnameDic.Clear();
        for (int i = 0; i < parentTransForm.childCount; i++)
        {
            Destroy(parentTransForm.GetChild(i).gameObject);
        }
        parentTransForm.DetachChildren();
        foreach (ClassInfo tmpClass in classes)
        {
            WebServicesBase.addedClassId.Add(tmpClass.id);
            WebServicesBase.classnameDic.Add(tmpClass.id,tmpClass.name);
            InstantiateList(tmpClass.name, tmpClass.teaName, tmpClass.id);
        }
        yield return StartCoroutine(wsb.getAllBankJson());
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>  
    /// 克隆一个GameObject  
    /// </summary>  
    public void InstantiateList(string className, string teacherName, int classId)
    {
        GameObject g = GameObject.Instantiate(originObject, parentTransForm);
        g.SetActive(true);
        g.GetComponent<RectTransform>().anchoredPosition = new Vector2(g.GetComponent<RectTransform>().anchoredPosition.x, originObject.GetComponent<RectTransform>().anchoredPosition.y);

        Button buttonTmp = g.GetComponentInChildren<Button>();
        //buttonTmp.transform.name;//题库名
        buttonTmp.transform.name = classId + "";
        g.transform.Find("ClassName").GetComponent<Text>().text = className;
        g.transform.Find("TeacherName").GetComponent<Text>().text = "代课老师：" + teacherName;

        UIEventListener btnListener = buttonTmp.gameObject.AddComponent<UIEventListener>();
        btnListener.OnClickEvent += delegate (GameObject gb)
        {
            Debug.Log(gb.name + " OnClick");
            //Camera.main.GetComponent<StartPanel>().changeBankName(gb);
            //this.SelectButton(gb);
            this.enterClass(gb);

        };
        g.name = classId + "";
        btnListener = g.AddComponent<UIEventListener>();
        btnListener.OnClickEvent += delegate (GameObject gb)
        {
            Debug.Log(gb.name + " OnClick");
            //Camera.main.GetComponent<StartPanel>().changeBankName(gb);
            //this.SelectButton(gb);
            this.enterClassByParent(gb);

        };
    }
    /// <summary>
    /// 返回
    /// </summary>
    public void backButton() {
        if (reportCards.activeSelf)
        {
            reportCards.SetActive(false);
        }
        else {
            TheClassPanel.SetActive(false);
        }
    }
 
    //当前班级id
    public static string currentClassId;
    //选择一个班级子项
    public void enterClass(GameObject gb)
    {

        ClassNameText.text = gb.transform.parent.Find("ClassName").GetComponent<Text>().text;
        currentClassId = gb.name;

        TheClassPanel.SetActive(true);
    }
    //点整个列表子项也可以进入班级
    public void enterClassByParent(GameObject gb)
    {

        ClassNameText.text = gb.transform.Find("ClassName").GetComponent<Text>().text;
        currentClassId = gb.name;

        TheClassPanel.SetActive(true);
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
            Debug.Log("Application");
            Unity_Tools.ui_return_to_platform();
            SceneManager.LoadScene("SceneSwitch");
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

    /// <summary>
    /// 进入测验练习(错题回顾模式)
    /// </summary>
    public void enterErrorColl() {
        Debug.Log("进入错题集");
        if (File.Exists(PublicClass.filePath + "classId_" + currentClassId + "_Error_QAXML.xml"))
        {
            GlobalVariable.practiceState = PracticeState.errors;
            BankScriControl.classInfo = classes;
            BankScriControl.selectClassId = currentClassId;
            SceneManager.LoadScene("Scenes/totalScence");
        }
        else {
            StartCoroutine(requestFailed("没有检测到错题记录，先去答题吧！"));
        }
    }
    /// <summary>
    /// 查询成绩列表
    /// </summary>
    public void querryReportCards() {
        for (int i = 0;i< reportParentTransForm.childCount;i++ ) {
            Destroy(reportParentTransForm.GetChild(i).gameObject);
        }
        StartCoroutine(getScoreList());
        Debug.Log("查询成绩");
        //DateTime tmp = DateTime.Parse("2018-4-19 15:49:00");
        //Debug.Log(tmp.ToLocalTime());
        //Debug.Log(tmp.ToString());
        //Debug.Log(tmp.ToUniversalTime());
        //ReportCards[] rcs = new ReportCards[10];
        //for (int i = 0;i<10;i++) {
        //    InstantiateReportList("xxxx",DateTime.Now.ToString(),90.0f);
        //}
        reportCards.SetActive(true);

    }
    [Header("schoolList")]
    //学校列表子项
    public GameObject schoolListOrigin;
    //学校列表子项父物体
    public Transform schoolListParent;
    //学校选择单选组
    public ToggleGroup tg;
    //选择班级
    public string selectSchool = string.Empty;
    //选择班级Flag
    public bool selectSchoolFlag = true;
    /// <summary>
    /// 添加学校子项
    /// </summary>
    /// <param name="schoolName"></param>
    /// <param name="code"></param>
    public void addSchoolListItem(string schoolName,string code) {
        GameObject g = GameObject.Instantiate(schoolListOrigin, schoolListParent);
        g.SetActive(true);
        g.GetComponent<RectTransform>().anchoredPosition = new Vector2(g.GetComponent<RectTransform>().anchoredPosition.x, originObject.GetComponent<RectTransform>().anchoredPosition.y);
        g.GetComponentInChildren<Text>().text = schoolName;
        Toggle t = g.GetComponent<Toggle>();
        t.group = tg;
        //取消难度切换
        g.name = code;
        if (selectSchoolFlag)
        {
            t.isOn = true;
            //changeMaxGrade(g.name);
            selectSchool = code;
            selectSchoolFlag = false;
        }
        else
        {
            t.isOn = false;
        }
        UIEventListener btnListener = t.gameObject.AddComponent<UIEventListener>();
        btnListener.OnClickEvent += delegate (GameObject gb)
        {
            this.changSchool(gb);
        };
    }
    /// <summary>
    /// 改变学校
    /// </summary>
    /// <param name="gb">列表子项</param>
    public void changSchool(GameObject gb) {
        Debug.Log("change School:"+gb.name);
        this.selectSchool = gb.name;

    }
    /// <summary>
    /// 获得当前班级成绩列表
    /// </summary>
    /// <returns></returns>
    public IEnumerator getScoreList() {

        
        KeyValue[] kvs = new KeyValue[2];
        kvs[0] = new KeyValue("stuId",sin.id);
        kvs[1] = new KeyValue("classId", currentClassId);

        UnityWebRequest uw = wsb.PostWithParams(servelets.queryScore, kvs);
        yield return uw.Send();
        string jsonData = uw.downloadHandler.text;
        Debug.Log(jsonData);
        if (!uw.isNetworkError && jsonData.Contains("name"))
        {
            ResponseJson rj = JsonConvert.DeserializeObject<ResponseJson>(jsonData);
            Debug.Log(rj.data.ToString());
            ScoreList[] rcs = JsonConvert.DeserializeObject<ScoreList[]>(rj.data.ToString());
            for (int i = 0; i < rcs.Length; i++)
            {
                InstantiateReportList(rcs[i].name, rcs[i].submission_time, rcs[i].testpaper_student_score);
            }
        }
        else {
            StartCoroutine(requestFailed("没有查询到成绩！"));
        }

    }
    /// <summary>
    /// 成绩列表项
    /// </summary>
    [Serializable]
    public class ScoreList {
        //提交时间
        public string submission_time;
        //试卷分数
        public float testpaper_student_score;
        //试卷名称
        public string name;
    }
    [Header("Report")]
    //成绩单面板
    public GameObject reportCards;
    //成绩单子项
    public GameObject originReportItem;
    //成绩父物体
    public Transform reportParentTransForm;
    /// <summary>
    /// 实例化表单列表
    /// </summary>
    /// <param name="ReportName"></param>
    /// <param name="time"></param>
    /// <param name="Score"></param>
    public void InstantiateReportList(string ReportName, string time, float Score)
    {
        
        GameObject g = GameObject.Instantiate(originReportItem, reportParentTransForm);
        g.transform.SetAsLastSibling();
        g.SetActive(true);
        g.GetComponent<RectTransform>().anchoredPosition = new Vector2(g.GetComponent<RectTransform>().anchoredPosition.x, originObject.GetComponent<RectTransform>().anchoredPosition.y);
        g.transform.Find("ReportName").GetComponent<Text>().text = ReportName;
        g.transform.Find("time").GetComponent<Text>().text = "提交日期：" + time;
        g.transform.Find("Text").GetComponent<Text>().text = string.Format("{0:N0}", Score);
        
    }

}
/// <summary>
/// 学生信息类
/// </summary>
[Serializable]
public class StuInfo
{
    //学生id
    public string id;
    //姓名
    public string name;
    //学生编号
    public string no;
    //性别
    public string gender;
    //手机号
    public string phone;
    //班级id
    public string classesId;
}

/// <summary>
/// 班级名称
/// </summary>
[Serializable]
public class ClassInfo
{
    //班级id
    public int id;
    //教师名称
    public string teaName;
    //班级名称
    public string name;
    //班级代码
    public string code;
    //
    public string time;
    //班级代码失效日期
    public string codeEndTime;

    public ClassInfo() { }
    public ClassInfo(int classId, string teacherName, string className)
    {
        this.id = classId;
        this.name = className;
        this.teaName = teacherName;
    }
}
//[Serializable]
//public class MyKeyValue<K, V>
//{
//    public MyKeyValue()
//    {
//    }
//    public MyKeyValue(K key, V value)
//    {
//        this.key = key;
//        this.value = value;
//    }
//    public K key;
//    public V value;
//}

[Serializable]
class MyArrays<T>
{
    public MyArrays()
    {

    }
    public T[] Items;
}