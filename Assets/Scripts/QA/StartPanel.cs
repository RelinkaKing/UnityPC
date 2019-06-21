using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Networking;
using System.Net;
using System.Collections;
using VesalCommon;
/// <summary>
/// 科目选择面板控制器
/// </summary>
public class StartPanel : MonoBehaviour
{

    //声音，计时开关物体
    public GameObject soundCube, timeCube;
    //开始界面与退出界面
    public GameObject startPanel, exitPanel;
    //题库选择界面
    public GameObject bankPanel;
    //实例出来的模型
    public GameObject modelObject;
    //当前选择最大难度等级
    private int CurrentMaxGrade = 100;
    //public int currentMinGrade = 1;
    //最大适用难度
    public int maxGrade = 1;
    //最小适用难度
    public int minGrade = 1;

    //难度系数选择父物体
    public GameObject grade;
    //难度系数选择下拉框
    public GameObject maxDropDown;
    //单选框组件
    public ToggleGroup tg;
    //记录当前科目选项
    GameObject checkToggle;
    ////题库名
    //private string bankName;
    ////题类型名
    //private string libName;

    //科目列表选项
    public GameObject originObject;
    //列表中占位物体
    public GameObject placeholderObject;
    //科目列表Scroll Content
    public Transform parentTransForm;
    //题数上限
    public GameObject inputField;
    public GameObject inputFieldParent;
    //【我的班级】试卷列表子项
    public GameObject testPaperItem;
    private void Awake()
    {

    }
    //底部菜单
    public GameObject bottomMenu;
    private void Start()
    {
        if (GlobalVariable.practiceState == PracticeState.classes)
        {
            originObject = testPaperItem;
        }
    }
    private void OnEnable()
    {

        GlobalVariable.questionCount = 10;

        inputField.transform.GetComponent<InputField>().text = "10";

    }
    /// <summary>
    /// 选择题库
    /// </summary>
    public bool initLibs()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc = Vesal_DirFiles.loadXml(BankScriControl.currentQaPath, GlobalVariable.bankName + ".xml");
        if (GlobalVariable.bankName.StartsWith("classId"))
        {
            inputFieldParent.SetActive(false);
        }
        else
        {
            inputFieldParent.SetActive(true);
        }

        if (xmlDoc == null)
        {
            Debug.Log("startPanel missing Xml");
            return false;
        }
        string xPath = "//library";
        XmlNodeList node = xmlDoc.SelectNodes(xPath);
        string tmpLibName;

        Toggle t;

        GameObject g;
        bool flag = true;
        foreach (XmlElement nodeElement in node)
        {
            tmpLibName = nodeElement.GetAttribute("name");
            int n = xmlDoc.SelectNodes("//library[@name='" + tmpLibName + "']/Question").Count;
            if (n == 0)
            {
                continue;
            }
            g = GameObject.Instantiate(originObject, parentTransForm);
            g.SetActive(true);
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(g.GetComponent<RectTransform>().anchoredPosition.x, originObject.GetComponent<RectTransform>().anchoredPosition.y);
            g.GetComponentInChildren<Text>().text = tmpLibName;
            if (GlobalVariable.practiceState == PracticeState.classes)
            {
                XmlElement xm = (XmlElement)xmlDoc.SelectSingleNode("//library[@name='" + tmpLibName + "']");
                if (xm.HasAttribute("startTime") && xm.HasAttribute("closeTime"))
                {
                    g.transform.Find("Time").GetComponent<Text>().text = (xm.GetAttribute("startTime") + " ~ " + xm.GetAttribute("closeTime")).Replace(".0", "");
                }
                else
                {
                    g.transform.Find("Time").GetComponent<Text>().text = "";
                }

            }
            t = g.GetComponent<Toggle>();
            t.group = tg;
            //取消难度切换
            //g.name = nodeElement.GetAttribute("minGrade") + "_" + nodeElement.GetAttribute("maxGrade");
            if (flag)
            {
                t.isOn = true;
                //changeMaxGrade(g.name);
                if (GlobalVariable.practiceState == PracticeState.classes)
                {
                    ScenceData.testPaperTimeInfo = g.transform.Find("Time").GetComponent<Text>().text;
                }
                flag = false;
            }
            else
            {
                t.isOn = false;
            }
            UIEventListener btnListener = t.gameObject.AddComponent<UIEventListener>();
            btnListener.OnClickEvent += delegate (GameObject gb)
            {
                this.OnToggleClick(gb);
            };
        }
        //占位元素，防止列表被下方挡住
        GameObject tmpGo;
        for (int i = 0; i < 2; i++)
        {
            tmpGo = GameObject.Instantiate(placeholderObject, parentTransForm);
            tmpGo.SetActive(true);
        }


        Toggle[] tmp = tg.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < tmp.Length; i++)
        {
            t = tmp[i];
            if (t.isOn)
            {
                checkToggle = t.gameObject;
                GlobalVariable.libName = t.GetComponentInChildren<Text>().text;
            }
        }
        return true;
    }
    //GameObject.Instantiate(originObject, parentTransForm)
    /// <summary>
    /// 下拉组件值改变
    /// </summary>
    /// <param name="n"></param>
    //public void mdValue(int n)
    //{
    //    string text = maxDropDown.GetComponent<Dropdown>().options.GetRange(n, 1)[0].text;
    //    ("-----------------------"+text);
    //    CurrentMaxGrade = getLevelNumber(text);
    //}

    //题数选择最大数量
    public GameObject maxQuesNum;

    /// <summary>
    /// 题数选择下拉组件值改变
    /// </summary>
    /// <param name="n">最大数</param>
    public void maxQuesNumValue(int n)
    {
        string text = maxQuesNum.GetComponent<Dropdown>().options.GetRange(n, 1)[0].text;
        if (text == "Max")
        {
            text = "10000";
        }
        GlobalVariable.questionCount = int.Parse(text);
        Debug.Log(text);
        Debug.Log("---" + GlobalVariable.questionCount);
        Debug.Log(n);
    }
    /// <summary>
    /// 验证输入为数字
    /// </summary>
    /// <param name="text">输入文本</param>
    public void verifyInput(string text)
    {
        try
        {
            int.Parse(text);
        }
        catch
        {
            inputField.transform.GetComponent<InputField>().text = "";
        }

    }
    /// <summary>
    /// 由数字转适用难度
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    string getLevelText(int n)
    {
        switch (n)
        {
            case 1: return "大专护理类题目";
            case 2: return "本科临床类题目";
            case 3: return "升学考研类题目";
        }
        return null;
    }
    /// <summary>
    /// 由适用难度字符串转数字
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    int getLevelNumber(string str)
    {
        switch (str)
        {
            case "大专护理类题目": return 1;
            case "本科临床类题目": return 2;
            case "升学考研类题目": return 3;
        }
        return 1;
    }

    /// <summary>
    /// 修改难度上限
    /// </summary>
    /// <param name="name"></param>
    //public void changeMaxGrade(string name)
    //{
    //    string[] tmp = name.Split('_');
    //    maxGrade = int.Parse(tmp[1]);
    //    minGrade = int.Parse(tmp[0]);
    //    changeGrade(minGrade, maxGrade, maxDropDown);
    //    CurrentMaxGrade = minGrade;
    //    maxDropDown.transform.Find("maxLabel").GetComponent<Text>().text = getLevelText(minGrade);
    //    // changeGrade(minGrade, maxGrade, minDropDown);
    //}
    /// <summary>
    /// 改变问题适用难度
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="dropDown"></param>
    //public void changeGrade(int min, int max, GameObject dropDown)
    //{
    //    Dropdown tmpDown = dropDown.GetComponent<Dropdown>();
    //    tmpDown.ClearOptions();
    //    List<Dropdown.OptionData> tmpList = new List<Dropdown.OptionData>();
    //    Dropdown.OptionData tmpOd;
    //    for (int i = min; i <= max; i++)
    //    {
    //        tmpOd = new Dropdown.OptionData(getLevelText(i));
    //        tmpList.Add(tmpOd);
    //    }
    //    tmpDown.AddOptions(tmpList);

    //}
    /// <summary>
    /// 改变题库
    /// </summary>
    /// <param name="gm"></param>
    public void changeBankName(GameObject gm)
    {
        GlobalVariable.bankName = gm.name;
        //(gm.name);
        changeBankName();
    }
    /// <summary>
    /// 改变题库名词
    /// </summary>
    public void changeBankName()
    {
        if (initLibs())
        {

            bankPanel.SetActive(false);
            startPanel.SetActive(true);
        }
    }

    private void Update()
    {

    }
    /// <summary>
    /// 初始化声音和限时按钮
    /// </summary>
    public void initSoundAndTimeButton()
    {
        transform.GetComponent<QuestingPanel>().questingPanel.SetActive(false);
        transform.GetComponent<QuestingPanel>().endPanel.SetActive(false);
        ScenceData.isOpenSound = true;
        ScenceData.isOpenTime = true;
        SoundButton();
        TimeButton();
    }

    /// <summary>
    /// 声音开关按钮
    /// </summary>
    public void SoundButton()
    {
        //置换开关状态
        ScenceData.isOpenSound = !ScenceData.isOpenSound;
        //如果打开声音开关
        if (ScenceData.isOpenSound)
        {
            //更换按钮位置
            soundCube.transform.localPosition = GlobalVariable.buttonPosition0;
            //置换按钮为白色
            soundCube.GetComponent<Image>().color = Color.white;
        }
        else
        {
            //更换按钮位置
            soundCube.transform.localPosition = GlobalVariable.buttonPosition1;
            //置换按钮初始色
            soundCube.GetComponent<Image>().color = GlobalVariable.colorOn;
        }
    }

    /// <summary>
    /// 倒计时开关按钮
    /// </summary>
    public void TimeButton()
    {
        //置换开关状态
        ScenceData.isOpenTime = !ScenceData.isOpenTime;
        //如果打开倒计时开关
        if (ScenceData.isOpenTime)
        {
            //更换按钮位置
            timeCube.GetComponent<RectTransform>().localPosition = GlobalVariable.buttonPosition0;
            //置换按钮为白色
            timeCube.GetComponent<Image>().color = Color.white;
        }
        else
        {
            //更换按钮位置
            timeCube.GetComponent<RectTransform>().localPosition = GlobalVariable.buttonPosition1;
            //置换按钮初始色
            timeCube.GetComponent<Image>().color = GlobalVariable.colorOn;
        }
    }
    /// <summary>
    /// 答题科目选择切换
    /// </summary>
    /// <param name="toggle"></param>
    public void OnToggleClick(GameObject toggle)
    {
        //(toggle.name);
        if (checkToggle == toggle)
        {
            //("重复点击一个Toggle");
            return;
        }

        if (GlobalVariable.practiceState == PracticeState.classes)
        {
            ScenceData.testPaperTimeInfo = toggle.transform.Find("Time").GetComponent<Text>().text;
        }
        checkToggle = toggle;
        //changeMaxGrade(toggle.name);
        if (GlobalVariable.libName != toggle.GetComponentInChildren<Text>().text)
        {
            isChangeLib = true;
            GlobalVariable.libName = toggle.GetComponentInChildren<Text>().text;
        }

    }
    //是否改变了科目
    bool isChangeLib = true;
    //bool hasModels = false;
    /// <summary>
    /// 初始化questions
    /// </summary>
    public void initQuestions()
    {
        //if (isChangeLib)
        //{
        //    isChangeLib = false;
        //    if (ScenceData.models.Count != 0)
        //    {
        //        foreach (GameObject g in ScenceData.models.Values)
        //        {
        //            Destroy(g);
        //        }
        //    }
        //    ScenceData.questions = null;
        //    ScenceData.models.Clear();
        //    (libName);
        //}


        Debug.Log(GlobalVariable.questionCount);
        randomQuestion(GlobalVariable.questionCount);


    }
#if UNITY_EDITOR
    public static XmlDocument editXmlDoc = null;
#endif
    /// <summary>
    /// 创建答题列表
    /// </summary>
    /// <param name="n">列表题数</param>
    /// <param name="errorQts">上次答错题集合</param>
    public void randomQuestion(int n)
    {
        bool tmpFlag = true;
        List<string> loadedSign = new List<string>();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc = Vesal_DirFiles.loadXml(BankScriControl.currentQaPath, GlobalVariable.bankName + ".xml");
        //#if UNITY_EDITOR
        //        n = 1000;
        //        tmpFlag = false;
        //        if (editXmlDoc == null)
        //        {
        //            editXmlDoc = xmlDoc;
        //        }
        //#endif
        Debug.Log("------" + GlobalVariable.bankName);
        Debug.Log("------" + GlobalVariable.libName);
        Question.LibId = ((XmlElement)xmlDoc.SelectSingleNode("//library[@name='" + GlobalVariable.libName + "']")).GetAttribute("id");
        Debug.Log(Question.LibId);
        //试卷答全部题
        if (GlobalVariable.bankName.StartsWith("classId"))
        {
            n = xmlDoc.SelectNodes("//library[@name='" + GlobalVariable.libName + "']/Question").Count;
            Debug.Log("=========总题数" + n);
            GlobalVariable.questionCount = n;
        }



        List<string> idLists = QaDao.instance.getTopErrorQaId(Mathf.FloorToInt(n * GlobalVariable.errorReappear), GlobalVariable.bankName, GlobalVariable.libName);
        List<Question> errorQts = new List<Question>();
        string tmpStr;
        string xPath;
        XmlElement xe;
        Question tmpQues;
        Debug.Log(idLists.Count);
        if (tmpFlag)
        {
            for (int m = 0; m < idLists.Count; m++)
            {
                tmpStr = idLists[m];
                xPath = "//library[@name='" + GlobalVariable.libName + "']/Question[grade <=" + CurrentMaxGrade + " and @qid = \"" + tmpStr + "\"]";
                xe = (XmlElement)xmlDoc.SelectSingleNode(xPath);
                Debug.Log(tmpStr);
                if (xe != null)
                {

                    tmpQues = new Question(xe);
                    //                    if (tmpQues.modelName != "null") {
                    //                        ScenceData.loadModel(tmpQues.abName, tmpQues.modelName, tmpQues.questionType == Question.QuestionType.thumbtack?true:false);
                    //                    }
                    //                    if (tmpQues.questionType == Question.QuestionType.thumbtack) {
                    ////                        initThumbtack(ref tmpQues, ref loadedSign);
                    //                    }
                    errorQts.Add(tmpQues);

                }
            }
        }
        Debug.Log(loadedSign.Count);
        // hasModels = false;
        //int max = int.Parse(maxDropDown.transform.Find("maxLabel").GetComponent<Text>().text);
        xPath = "//library[@name='" + GlobalVariable.libName + "']/Question[grade <=" + CurrentMaxGrade + "]";
        //and grade >= "+currentMinGrade+"]";
        //Question[grade<4 and grade>2]
        ////Question[contains("23",grade)]
        if (xmlDoc == null)
        {
            return;
        }
        //SelectSingleNode(xPath).ChildNodes;
        XmlNodeList node = xmlDoc.SelectNodes(xPath);
        if (n > node.Count)
        {
            n = node.Count;
        }
        Debug.Log(n);
        ScenceData.questions = new Question[n];
        int i = 0;

        //添加错题并记录错题里问题排重
        List<string> ques = new List<string>();
        if (tmpFlag)
        {
            if (errorQts.Count != 0)
            {
                while (i < errorQts.Count)
                {
                    ScenceData.questions[i] = errorQts[i];
                    //if (ScenceData.questions[i].modelName != null && ScenceData.questions[i].modelName != "null")
                    //{
                    //    hasModels = true;
                    //}
                    ques.Add(errorQts[i].qid);
                    i++;
                }

            }
        }

        if (i != n)
        {
            int tmpIndex = 0;

            Question[] tmpQs;
            if (errorQts != null && errorQts.Count != 0)
            {
                Debug.Log(node.Count + "--" + errorQts.Count);
                tmpQs = new Question[node.Count - errorQts.Count];
            }
            else
            {
                tmpQs = new Question[node.Count];
            }
            Debug.Log(tmpQs.Length);
            foreach (XmlElement nodeList in node)
            {

                if (nodeList["q"] != null && !ques.Contains(nodeList.GetAttribute("qid")))
                {
                    tmpQs[tmpIndex] = new Question(nodeList);
                    tmpIndex++;
                }

            }
            if (tmpFlag)
            {
                if (tmpQs.Length > 2)
                {
                    //打乱数组中元素顺序
                    int y; Question tmp;
                    for (int m = 0; m < tmpQs.Length; m++)
                    {
                        do
                        {
                            y = UnityEngine.Random.Range(0, tmpQs.Length);
                        } while (y == m);
                        tmp = tmpQs[m];
                        tmpQs[m] = tmpQs[y];
                        tmpQs[y] = tmp;
                    }
                }
            }
            tmpIndex = 0;

            while (i <= n - 1)
            {
                //Debug.Log(i+"_"+tmpIndex+ "--tmpQs-"+ tmpQs.Length);
                ScenceData.questions[i] = tmpQs[tmpIndex];
                tmpIndex++;
                if (ScenceData.questions[i] == null)
                {
                    break;
                }
                //                if (ScenceData.questions[i].modelName != null && ScenceData.questions[i].modelName != "null")
                //                {
                //                    //hasModels = true;
                //                    //ScenceData.loadModel(ScenceData.questions[i].abName, ScenceData.questions[i].modelName, ScenceData.questions[i].questionType == Question.QuestionType.thumbtack ? true : false);
                //                    if (ScenceData.questions[i].questionType == Question.QuestionType.thumbtack) {

                ////                        initThumbtack(ref ScenceData.questions[i],ref loadedSign);
                //                    }
                //                }
                i++;
            }
        }
        //初始化本波次错题记录
        ScenceData.errorQts = new List<Question>();
        Debug.Log("end RandomQuestion");
    }
    /// <summary>
    /// 初始化标注（弃用）
    /// </summary>
    /// <param name="tmpQues"></param>
    /// <param name="loadedSign"></param>
    public void initThumbtack(ref Question tmpQues, ref List<string> loadedSign)
    {
        //string mapModelName;
        //string mapAbName;
        //QaDao.instance.getSignInfo(tmpQues.modelName, out mapAbName, out mapModelName);
        ////Debug.Log("getSignInfo=====" + mapModelName);
        //if (mapModelName != null && mapAbName != null)
        //{
        //    //钉子只初始化一次
        //    if (loadedSign.Contains(tmpQues.modelName))
        //    {
        //        //i++;
        //        return;
        //    }
        //    else
        //    {
        //        loadedSign.Add(tmpQues.modelName);
        //    }
        //    ScenceData.loadModel(mapAbName, mapModelName, false);
        //    GameObject mapModel = ScenceData.models[mapModelName];
        //    GameObject signModel = ScenceData.models[tmpQues.modelName];
        //    //mapModel.SetActive(true);
        //    Transform[] signTfs = signModel.transform.GetComponentsInChildren<Transform>();
        //    Transform[] mapTfs = mapModel.transform.GetComponentsInChildren<Transform>();
        //    Transform tmpTfs;

        //    for (int s = 0; s < signTfs.Length; s++)
        //    {
        //        tmpTfs = signTfs[s];
        //        if (!tmpTfs.name.StartsWith("PS") && tmpTfs.childCount == 0)
        //        {
        //            //Debug.Log(tmpTfs.name);
        //            for (int m = 0; m < mapTfs.Length; m++)
        //            {
        //                if (mapTfs[m].name == tmpTfs.name)
        //                {
        //                    tmpTfs.GetComponent<MeshRenderer>().material = mapTfs[m].GetComponent<MeshRenderer>().material;
        //                    tmpTfs.gameObject.AddComponent<MeshCollider>();
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    //mapModel.SetActive(false);
        //}
    }
    //临时下载面板（弃用）
    public GameObject downPanel;
    /// <summary>
    /// 打乱数组
    /// </summary>
    /// <param name="total"></param>
    /// <returns></returns>
    int[] randowArray(int total)
    {
        int[] tmp = new int[total];

        int index;
        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i] = i + 1;
        }
        for (int i = 0; i < tmp.Length; i++)
        {
            index = UnityEngine.Random.Range(0, tmp.Length - 1);
            if (index != i)
            {
                tmp[i] += tmp[index];
                tmp[index] = tmp[i] - tmp[index];
                tmp[i] -= tmp[index];
            }
        }

        return tmp;
    }

    /// <summary>
    /// 打乱数组
    /// </summary>
    /// <param name="total"></param>
    /// <param name="n"> 取N个</param>
    /// <returns></returns>
    int[] randowArray(int total, int n)
    {
        int[] tmp = new int[total];
        int[] resutlt = new int[n];
        int index;
        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i] = i + 1;
        }
        for (int i = 0; i < tmp.Length; i++)
        {
            index = UnityEngine.Random.Range(0, tmp.Length - 1);
            if (index != i)
            {
                tmp[i] += tmp[index];
                tmp[index] = tmp[i] - tmp[index];
                tmp[i] -= tmp[index];
            }
        }
        for (int s = 0; s < n; s++)
        {
            resutlt[s] = tmp[s];
        }
        return resutlt;
    }
    /// <summary>
    /// 开始做题按钮（进入测试）
    /// </summary>
    public void StartChoose()
    {
        //输入题数
        //if (inputFieldParent.activeSelf && !GlobalVariable.isQaOnly) {
        //    //inputField.GetComponent<InputField>().CancelInvoke();
        //    try
        //    {
        //        GlobalVariable.questionCount =  int.Parse(inputField.GetComponent<InputField>().text);
        //    }
        //    catch {
        //        inputField.transform.GetComponent<InputField>().text = "10";
        //        return;
        //    }
        //}

        //("StartChoose");
        //如果准备或重新准备状态
        //if (ScenceData.currentState == ScenceState.prepare || ScenceData.currentState == ScenceState.rePrepare || ScenceData.currentState == ScenceState.exam)
        //{
        //}
#if UNITY_EDITOR
        transform.GetComponent<QuestingPanel>().recordDic.Clear();
#endif
        //切换为载入题库状态
        ScenceData.currentState = ScenceState.loadQuestion;
        ////进入下载界面
        //ScenceData.downloadDic.Clear();
        ////getDownLoadLinks();
        //if (ScenceData.downloadDic.Count != 0)
        //{
        //    downPanel.SetActive(true);
        //    downPanel.GetComponent<tmpDownScri>().startDownLoad();


        //}
        //else {


        //}
        StartCoroutine(waitIe());

    }

    /// <summary>
    /// 退出按钮
    /// </summary>
    public void ExitButton()
    {
        //冻结时间
        Time.timeScale = 0;
        if (GlobalVariable.practiceState == PracticeState.classes || GlobalVariable.practiceState == PracticeState.errors)
        {
            backClassPanel.SetActive(true);
        }
        else
        {
            //打开退出界面
            exitPanel.SetActive(true);
        }
        Time.timeScale = 1;
    }
    //返回我的班级面板
    public GameObject backClassPanel;
    //切回我的班级
    public void backClass()
    {
        SceneManager.LoadScene("Scenes/MyClass");
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
            exitPanel.SetActive(false);
            int sumTg = tg.transform.childCount;
            for (int i = 0; i < sumTg; i++)
            {
                if (tg.transform.GetChild(i).name != "T1")
                {
                    Destroy(tg.transform.GetChild(i).gameObject);
                }
            }
            startPanel.SetActive(false);
            if (GlobalVariable.practiceState == PracticeState.classes || GlobalVariable.practiceState == PracticeState.errors)
            {
                ScenceData.clear();
                Debug.Log("MyClass quit!");
                Camera.main.GetComponent<XT_AllButton>().ExitProgram();
            }
            else
            {
                bankPanel.SetActive(true);
            }

        }
        else
        //如果是取消按钮
        {
            Time.timeScale = 1;
            //解冻时间
            exitPanel.SetActive(false);
            //关闭退出界面
        }
    }

    //void getDownLoadLinks() {
    //    string xPath = "//library[@name='" + libName + "']/Question[grade <=" + CurrentMaxGrade + "]";
    //    XmlDocument xmlDoc = new XmlDocument();
    //    xmlDoc = ScenceData.loadXml(bankName);
    //    XmlNodeList node = xmlDoc.SelectNodes(xPath);
    //    string tmp,down = "" ,save = "";
    //    foreach (XmlElement tmpElement in node) {
    //        tmp = tmpElement["url"] == null ? "null" : tmpElement["url"].InnerText;
    //        if (tmp != "null") {
    //            down = GlobalVariable.basicUrl + tmp;
    //            save = PublicClass.filePath  + "/Vesal_QA"+ tmp;
    //            if (!ScenceData.downloadDic.ContainsKey(down) && !File.Exists(save)) {
    //                ScenceData.downloadDic.Add(down,save);
    //            }
    //        }
    //        (down);
    //        (save);
    //        tmp = tmpElement["modelName"] == null ? "null" : tmpElement["modelName"].InnerText;
    //        if (tmp != "null") {
    //            tmp = tmpElement["modelName"].GetAttribute("assetbundle");
    //            down = GlobalVariable.basicAbUrl + tmp + ".assetbundle";
    //            save = PublicClass.filePath  + "/Vesal_QA/AssetBundles/" + tmp + ".assetbundle";
    //            if (!ScenceData.downloadDic.ContainsKey(down) && !File.Exists(save))
    //            {
    //                ScenceData.downloadDic.Add(down, save);
    //            }
    //        }
    //        (down);
    //        (save);
    //    }
    //}
    [Header("wait")]
    //提示加载等待面板
    public GameObject startWaitPanel;
    //提示加载等待文本
    public Text startWaitText;
    /// <summary>
    /// 显示加载等待提示，开始初始化进入答题
    /// </summary>
    /// <returns></returns>
    public IEnumerator waitIe()
    {
        startWaitPanel.SetActive(true);
        yield return null;
        EnterExam();
    }
    /// <summary>
    /// 进入测试界面主操作
    /// </summary>
    public void EnterExam()
    {

        //("EnterExam");
        //try
        //{

        Debug.Log("initQuestions");
        initQuestions();
        Debug.Log(ScenceData.currentState);
        if (ScenceData.questions.Length == 0)
        {
            Debug.Log("未加载到问题！！！");
            StartCoroutine(requestFailed("未加载到问题！！！"));
            return;
        }
        //Debug.Log(ScenceData.currentState);
        //if (hasModels == true && ScenceData.models.Count == 0)
        //{
        //    Debug.Log("未加载到模型！！！");
        //    StartCoroutine(requestFailed("未加载到模型！！！"));
        //    return;
        //}
        Debug.Log(ScenceData.currentState);
        //如果是载入题目状态

        if (ScenceData.currentState == ScenceState.loadQuestion)
        {
            transform.GetComponent<QuestingPanel>().init();

            //打开测试界面
            transform.GetComponent<QuestingPanel>().questingPanel.SetActive(true);
            ScenceData.useTime = PublicTools.GetTimeStamp();
            //关闭开始界面
            startPanel.SetActive(false);
            startWaitPanel.SetActive(false);
        }
        //downPanel.SetActive(false);
        //}
        //catch (SystemException e)
        //{
        //    StartCoroutine(requestFailed("加载失败，请反馈！"));

        //    Debug.Log(e.StackTrace);
        //    Debug.Log(e.Message);
        //}
    }
    /// <summary>
    /// 显示等待提示文本
    /// </summary>
    /// <param name="mess">提示文本</param>
    /// <returns></returns>
    public IEnumerator requestFailed(string mess = "请等待，模型加载中······")
    {
        startWaitPanel.SetActive(true);
        startWaitText.text = mess;
        yield return wait(1.5f);
        startWaitPanel.SetActive(false);
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
}
