using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Xml;
using Assets.Scripts.Model;
/// <summary>
/// 测试主逻辑控制器
/// </summary>
public class QuestingPanel : MonoBehaviour
{
    //模型操作UI相机
    public Camera ModelUICamera;
    //模型展示背景相机
    public Camera ModelBgCamera;
    //重置按钮
    public Button resetButton;
    
    //按钮内容
    Text[] textInButton;
    //选项标志
    Image[] target;
    //选项按钮
    Button[] buttons;
    //最底层按钮
    public Button[] initButton;
    //最底层按钮图片
    public Image[] initImage;
    //正确声音片段
    public AudioClip clipIsTrue;
    //错误声音片段
    public AudioClip clipIsFalse;
    //问题文本
    public Text questionText;
    //纯文本题显示
    public Text questionTextBig;
    //得分文本
    public Text textScore;
    //题目索引文本
    public Text textQuestionIndex;
    //结果列表预制体
    public GameObject imageList;
    //结果列表父物体
    public GameObject imageListFather;
    //音源
    public AudioSource audioSc;
    //倒计时时间
    public float timeDown;
    //限制答题时间
    public float totalTime = 15;
    //倒计时时间文本
    public Text textTimeDown;
    //倒计时显示图片
    public Image imageTimeDown;
    //测试界面
    public GameObject questingPanel;
    //结束界面
    public GameObject endPanel;
    //返回界面
    public GameObject QueBackPanel;
    //是多选题
    bool isMultipleChoice = false;
    //提示单选多选
    public Text questionTip;
    //记录选择答案
    public List<string> selectAnswers;
    //下一题按钮
    public Text nextButtonText;
    public Button nextButton;
    //显示答案按钮
    public Button showAnswerButton;
    //3D功能按钮
    public GameObject ModelButtonPanel;
    //当前问题在集合中位置
    public int questionIndex = 0;
    //当前问题用户是否已选择
    bool isSelect = false;
    //所有问题用户选择记录
    public static Selected[] selecteds;
    //正确答案图片
    public GameObject rightImage;
    //错误答案图片
    public GameObject falseImage;
    //上一题按钮
    public Button lastButton;
    [Header("Only Text")]
    //纯文本按钮布局
    //纯文本题选项模板
    public GameObject[] optionItems;
    //纯文本题临时寄存指针，切换题时销毁
    public GameObject[] tmpParent;
    //纯文本题选项scrollView
    public GameObject scrollView;
    //public GameObject parent;
    //public GameObject parent;
    private void Awake()
    {
        try
        {
            ModelUICamera.enabled = false;
            ModelUICamera.depth = 6;
            Camera.main.depth = 5;
            ModelBgCamera.depth = 0;
            resetButton.onClick.AddListener(resetHighLight);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }

    void Start()
    {

    }
    /// <summary>
    /// 答题初始化函数
    /// </summary>
    public void init()
    {

        //开启限时答题
        if (ScenceData.isOpenTime)
        {
            imageTimeDown.gameObject.SetActive(true);
            lastButton.gameObject.SetActive(false);
        }
        else
        {
            imageTimeDown.gameObject.SetActive(false);
            lastButton.gameObject.SetActive(true);
        }
        ScenceData.score = 0;
        ScenceData.rightIndex = 0;

        isMultipleChoice = false;
        questionTip.text = "单选";
        questionIndex = 0;
#if UNITY_EDITOR

#else
        //打乱存题数组
        DisturbAnswer();
        input.gameObject.SetActive(false);
#endif
        ScenceData.questionTotal = ScenceData.questions.Length;
        selecteds = new Selected[ScenceData.questionTotal];
        initQuestion();
        ScenceData.currentState = ScenceState.exam;
        timeDown = 15;
        if (ScenceData.questionTotal == 1)
        {
            nextButtonText.text = "显示结果";
        }
        lastButton.interactable = false;
        Time.timeScale = 1;
    }
    /// <summary>
    /// 初始化并隐藏所有答题按钮
    /// </summary>
    /// <param name="n" >按钮数量</param>
    public void initbutton(int n, bool flag = false)
    {
        scrollView.SetActive(false);
        //先销毁所有按钮
        if (textInButton != null)
        {
            // transButton.transform.localPosition -= new Vector3(0,150*Mathf.Ceil(textInButton.Length/2),0);
            for (int i = 0; i < textInButton.Length; i++)
            {
                try
                {
                    Destroy(buttons[i].gameObject);
                    Destroy(target[i].gameObject);
                }
                catch
                {
                    continue;
                }
            }
        }
        if (tmpParent != null)
        {
            for (int i = 0; i < tmpParent.Length; i++)
            {
                try
                {
                    Destroy(tmpParent[i]);
                }
                catch
                {
                    continue;
                }
            }
            tmpParent = null;
        }
        //n为0 返回 不为0初始化button和target
        if (n == 0)
        {
            return;
        }

        Button buttonTmp;
        Image tmpImage;
        if (flag)
        {
            scrollView.SetActive(true);
            tmpParent = new GameObject[n];
        }
        int buttonCount = initButton.Length;
        //A 65
        int start = 65;
        string tmpText;
        target = new Image[n];
        textInButton = new Text[n];
        buttons = new Button[n];
        int lineNumber = 1;
        int translationDis = 119;
        //if (GlobalVariable.isQaOnly) {
        //    //
        //    translationDis = -83-((720 - 83 * n) / (n + 1));

        //    //initButton[0].transform.localPosition += new Vector3(0, translationDis, 0);
        //    //initImage[0].transform.localPosition += new Vector3(0, translationDis, 0);
        //}

        for (int i = 0; i < n; i++)
        {
            //if (GlobalVariable.isQaOnly)
            //{
            //    tmpText = asciiToChar((n-1)+start--);
            //}
            //else {
            //}
            tmpText = asciiToChar(start++);

            if (flag)
            {
                tmpParent[i] = GameObject.Instantiate(optionItems[i], optionItems[i].transform.parent);
                tmpParent[i].SetActive(true);
                buttonTmp = tmpParent[i].transform.Find("Button-A (1)").GetComponent<Button>();
                buttonTmp.transform.name = "" + i;
                tmpImage = tmpParent[i].transform.Find("Image-A (1)").GetComponent<Image>();
                tmpImage.GetComponentInChildren<Text>().text = tmpText;
            }
            else
            {

                buttonTmp = Button.Instantiate(initButton[i % buttonCount], initButton[i % buttonCount].transform.parent);
                buttonTmp.transform.name = "" + i;
                tmpImage = Image.Instantiate(initImage[i % buttonCount], initImage[i % buttonCount].transform.parent);
                tmpImage.GetComponentInChildren<Text>().text = tmpText;
                if (i != 0 && i % buttonCount == 0)
                {
                    //加一行Button
                    for (int j = 0; j < i; j++)
                    {
                        target[j].GetComponent<RectTransform>().localPosition += new Vector3(0, translationDis, 0);
                        buttons[j].transform.localPosition += new Vector3(0, translationDis, 0);
                    }
                    lineNumber++;
                }
            }
            //添加监听器
            UIEventListener btnListener = buttonTmp.gameObject.AddComponent<UIEventListener>();
            btnListener.OnClickEvent += delegate (GameObject gb)
            {
                this.SelectButton(gb);
            };
            btnListener = tmpImage.gameObject.AddComponent<UIEventListener>();
            tmpImage.name = i + "";
            btnListener.OnClickEvent += delegate (GameObject gb)
            {
                this.selectImg(gb);
            };
            target[i] = tmpImage;
            textInButton[i] = buttonTmp.GetComponentInChildren<Text>();
            buttons[i] = buttonTmp;
        }

    }
    /// <summary>
    /// 选项按钮监听方法
    /// </summary>
    /// <param name="gb"></param>
    public void selectImg(GameObject gb)
    {
        SelectButton(buttons[int.Parse(gb.name)].gameObject);
    }
    /// <summary>
    /// ascii码转char
    /// </summary>
    /// <param name="asciiCode"></param>
    /// <returns></returns>
    public string asciiToChar(int asciiCode)
    {
        if (asciiCode >= 0 && asciiCode <= 255)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)asciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
        else
        {
            throw new Exception("ASCII Code is not valid.");
        }
    }
    /// <summary>
    /// 显示隐藏模型操作相关按钮
    /// </summary>
    /// <param name="flag"></param>
    public void ModelButtonPanelSwitch(bool flag)
    {
        //bool flag = imageTimeDown.gameObject.activeSelf;
        Transform tf = ModelButtonPanel.transform;
        for (int i = 0; i < tf.childCount; i++)
        {
            GameObject go = tf.GetChild(i).gameObject;
            if (go.name == "Image-Time")
            {
                continue;
            }
            go.SetActive(flag);
        }

        if (GlobalVariable.practiceState == PracticeState.errors)
        {
            deleteButton.SetActive(true);
        }
        else
        {
            deleteButton.SetActive(false);
        }
    }
    /// <summary>
    /// 上一题
    /// </summary>
    public void lastQuestion()
    {
        if (questionIndex > 0)
        {
            changQueation(false);
        }
        if (questionIndex != selecteds.Length - 1)
        {
            nextButtonText.text = "下一题";
        }
    }
    //是否为查看上一题状态
    bool isRollback;
    //SceneModels脚本临时父物体
    public static GameObject SceneModelsObj;
    /// <summary>
    /// 加载模型
    /// </summary>
    public void loadModel()
    {
        Question q = ScenceData.questions[questionIndex];
        if (SceneModelsObj != null)
        {
            DestroyImmediate(SceneModelsObj);
        }
        SceneModelsObj = new GameObject("SceneModelsObj");
        SceneModels tempSm = SceneModelsObj.AddComponent<SceneModels>();
        if (q.nounNo != string.Empty && q.nounNo != null)
        {
            tempSm.Init_SceneModels(q.nounNo,false);
        }
        else
        {
            tempSm.Init_SceneModelsByList(new List<string>(q.selectModelDic.Keys), null, null, null,false);
        }
        resetHighLight();
    }
    /// <summary>
    /// 重置模型照射相机位置
    /// </summary>
    public void resetPos()
    {
        Debug.Log(autoPos);
        if (!autoPos)
        {
            return;
        }

        Question q = ScenceData.questions[questionIndex];
        Debug.Log(q.cp.localPos);
        Interaction tmpInteraction = Camera.main.GetComponent<Interaction>();
        tmpInteraction.TranslateOpera(q.cp.localPos);
        tmpInteraction.RotateOpera(q.cp.boxRotation);
        tmpInteraction.distance = q.cp.localPos.z;
    }
    //高亮的模型名词拼接字符串
    string highLightNames = string.Empty;
    /// <summary>
    /// 重置高亮
    /// </summary>
    public void resetHighLight()
    {
        Camera.main.GetComponent<XT_AllButton>().ResetModel();
        Camera.main.GetComponent<XT_AllButton>().ResetView();
        Camera.main.GetComponent<XT_MouseFollowRotation>().Reset();


        Question q = ScenceData.questions[questionIndex];
        Camera.main.GetComponent<Interaction>().TranslateOpera(new Vector3(0, 0, Camera.main.transform.localPosition.z));
        Camera.main.GetComponent<Interaction>().RotateOpera(Vector3.zero);
        Debug.Log("resetPos");
        resetPos();
        //if (!(q.nounNo != string.Empty && q.nounNo != null)) {
        //}
        highLightNames = "";
        flashModels(q, true);

    }
    /// <summary>
    /// 闪烁高亮模型
    /// </summary>
    /// <param name="q">当前问题</param>
    /// <param name="flag">显示/隐藏</param>
    public void flashModels(Question q, bool flag)
    {
        if (q.selectModelDic != null && SceneModels.instance != null)
        {
            SceneModels.instance.set_Multi_Selection(true);
            SceneModels.instance.CancleSelect();
            highLightNames = "";
            foreach (string key in q.selectModelDic.Keys)
            {
                if (q.selectModelDic[key])
                {
                    highLightNames += key + " @ ";
                    if (flag)
                    {
                        SceneModels.instance.ChooseModelByName(key);
                    }
                    else
                    {
                        SceneModels.instance.Fade();
                    }
                }
            }
        }
    }
    /// <summary>
    /// 初始化问题
    /// </summary>
    public void initQuestion()
    {
        if (GlobalVariable.practiceState != PracticeState.classes)
        {
            lastButton.gameObject.SetActive(false);
        }
        flash = true;
        if (SceneModels.instance != null)
        {
            SceneModels.instance.closeTempParent();
            SceneModels.instance.destoryTempParent();
            SceneModels.instance = null;
        }
        ModelUICamera.enabled = false;
        //transform.enabled = false;
        isSelect = false;
        Question tmpQues = ScenceData.questions[questionIndex];
        try
        {
            if (tmpQues.cp.highLightColor != Color.white)
            {
                GlobalVariable.tmpHighLightColor = tmpQues.cp.highLightColor;
            }
            else
            {
                GlobalVariable.tmpHighLightColor = Color.white;
            }
            Debug.Log("initQuestion tmpHighLightColor <color=blue>" + GlobalVariable.tmpHighLightColor + "</color>");
        }
        catch
        {
            Debug.Log("initQuestion tmpHighLightColor");
        }
        isRollback = false;
        if (selecteds[questionIndex] == null)
        {
            if (selecteds!= null && selecteds.Length>questionIndex && ScenceData.questions!=null && ScenceData.questions.Length > questionIndex) {
                selecteds[questionIndex] = new Selected();
                selecteds[questionIndex].qid = ScenceData.questions[questionIndex].qid;
            }
        }
        else
        {
            isRollback = true;
        }
        initbutton(tmpQues.answers.Count, tmpQues.questionType == Question.QuestionType.text);
        //是否显示问题面板
        if (tmpQues.q == null || tmpQues.q == "null")
        {
            questionText.gameObject.SetActive(false);
        }
        else
        {
            questionTextBig.text = tmpQues.q;
            questionText.text = tmpQues.q;
            questionText.gameObject.SetActive(true);
        }
        if (tmpQues.rightAnswers.Count > 1)
        {
            isMultipleChoice = true;
            questionTip.text = "多选";
            selectAnswers = new List<string>();
            //nextButton.interactable = true;
            if (GlobalVariable.practiceState != PracticeState.classes)
            {
                nextButtonText.text = "确定";//确定
            }
        }
        else
        {
            isMultipleChoice = false;
            questionTip.text = "单选";
            if (GlobalVariable.practiceState != PracticeState.classes)
            {

                nextButtonText.text = "下一题";
            }
        }
        List<string> tmp;
        if (!isRollback)
        {
            tmp = tmpQues.getRadomAnswer();
            selecteds[questionIndex].randomAnswers = tmp;
        }
        else
        {
            tmp = selecteds[questionIndex].randomAnswers;
        }
        for (int i = 0; i < tmp.Count; i++)
        {
            FontsizeAdjust(i);
            textInButton[i].text = tmp[i];
            //打开按钮
            target[i].gameObject.SetActive(true);
            textInButton[i].transform.parent.gameObject.SetActive(true);
            if (!isMultipleChoice)
            {
                if (tmp[i] == ScenceData.questions[questionIndex].rightAnswers[0])
                {
                    ScenceData.rightIndex = i;
                }
            }

        }
        textQuestionIndex.text = (questionIndex + 1) + "/" + ScenceData.questionTotal;

        //ModelButtonPanel.SetActive(false);
        ModelButtonPanelSwitch(false);
        questionTextBig.gameObject.SetActive(false);
        //判断是模型题
        if (tmpQues.questionType == Question.QuestionType.model || tmpQues.questionType == Question.QuestionType.thumbtack)
        {
            ModelButtonPanelSwitch(true);
            //ModelUICamera.enabled = true;

            loadModel();
            //transform.enabled = true;

            ////ModelButtonPanel.SetActive(true);
            //ModelControl.type = tmpQues.questionType;
            //transform.GetComponent<ModelControl>().init(tmpQues.modelName, ScenceData.questions[questionIndex].selectModelDic);
            //tmpQues.cp.initCamera();
            //transform.GetComponent<ModelControl>().highLight();
        }
        //判断是图片题
        else if (tmpQues.questionType == Question.QuestionType.picture)
        {
            transform.GetComponent<PictureControl>().initImage(tmpQues.url);
        }
        else if (tmpQues.questionType == Question.QuestionType.movie)
        {
            transform.GetComponent<MovieControl>().initMovie(tmpQues.url);
        }
        else if (tmpQues.questionType == Question.QuestionType.text && GlobalVariable.practiceState != PracticeState.weike)
        {
            questionText.gameObject.SetActive(false);
            questionTextBig.gameObject.SetActive(true);
        }

        if (ScenceData.isOpenTime)
        {
            //时间递减
            timeDown = totalTime;
            //显示倒计时时间
            textTimeDown.text = ((int)timeDown).ToString();
            //图片以圆的形式缩短
            imageTimeDown.fillAmount = totalTime / totalTime;
        }
        //重置选择状态
        if (isRollback)
        {
            if (selectAnswers != null)
            {
                selectAnswers.Clear();
            }
            foreach (int i in selecteds[questionIndex].selectIndexsList)
            {
                setButtonColor(i, ScenceData.buttonIsTrue);
                //  selectAnswers.Add(buttons[i].GetComponentInChildren<Text>().text);
            }
            Debug.Log(selecteds[questionIndex].selectContent + "`````````````2`2`2`2`");
            Debug.Log(selecteds[questionIndex].selectContent.Split('-')[0]);

            //isSelect = selecteds[questionIndex].isSelected;
            //if (questionIndex + 1 != selecteds.Length && selecteds[questionIndex+1] != null)
            //{
            //    isSelect = true;

            //}
            //if (questionIndex+1 == selecteds.Length) {
            //isSelect = false;
            //}
        }
        //if (selecteds[questionIndex] != null) {
        //    selecteds
        //}
    }
    /// <summary>
    /// 获得模型选择字典
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, bool> getDic()
    {
        return ScenceData.questions[questionIndex].selectModelDic;
    }
    /// <summary>
    /// 切换下一题
    /// </summary>
    public void changQueation(bool flag = true)
    {

        //if (transform.GetComponent<ModelControl>().isStartControl)
        //{
        //    transform.GetComponent<ModelControl>().stop();
        //}

        transform.GetComponent<PictureControl>().stop();
        transform.GetComponent<MovieControl>().stop();

        if (flag)
        {
            if (questionIndex < selecteds.Length)
            {
                questionIndex++;
            }
        }
        else
        {
            questionIndex--;
        }


        if (questionIndex < selecteds.Length)
        {
            initQuestion();
        }
        else
        {
            if (GlobalVariable.practiceState == PracticeState.classes)
            {
                testEndPanel.SetActive(true);
            }
            else
            {
                nextButton.GetComponentInChildren<Text>().text = "下一题";
                StartCoroutine(EnterEndPanel());
            }
        }
        //if (ScenceData.isOpenTime)
        //{
        //    lastButton.gameObject.SetActive(false);
        //}
        //else {
        //    lastButton.gameObject.SetActive(true);
        //}

        if (questionIndex != 0 && !ScenceData.isOpenTime)
        {
            lastButton.interactable = true;
        }
        else
        {
            lastButton.interactable = false;
        }
    }
    //调试用跳转问题输入框
    public InputField input;
    /// <summary>
    /// 调试用跳转问题至指定索引
    /// </summary>
    /// <param name="text"></param>
    public void skipQuestion(string text)
    {
        Debug.Log(text);
        try
        {
            float target = float.Parse(text);
            while (true)
            {
                int index = ScenceData.questions[questionIndex].qid.LastIndexOf('.');
                float curr = float.Parse(ScenceData.questions[questionIndex].qid.Substring(index + 1));
                if (curr > target)
                {
                    lastQuestion();
                }
                else if (curr < target)
                {
                    NextButton();
                }
                else
                {
                    break;
                }
                if (questionIndex == 0 || questionIndex == ScenceData.questionTotal - 1)
                {
                    break;
                }
                //curr = float.Parse(ScenceData.questions[questionIndex].qid);
            }
        }
        catch
        {
            Debug.Log("输入非法！");
        }
    }
    //测验结束面板
    public GameObject testEndPanel;
    /// <summary>
    /// 测验结束，显示结束面板
    /// </summary>
    public void testEnterEnd()
    {
        nextButton.GetComponentInChildren<Text>().text = "下一题";
        StartCoroutine(EnterEndPanel());
    }
    /// <summary>
    /// 测验结束取消函数
    /// </summary>
    public void testEnterEndCancle()
    {
        questionIndex--;
    }
    //编辑器环境修改xml信息时使用
    public SortedList<string, string> recordDic = new SortedList<string, string>();
    //自动定位
    public bool autoPos = true;
    //累计时间
    private float AccumilatedTime = 0f;
    //每帧刷新时间
    public float FrameLength = 0.5f;
    void Update()
    {

        //if (selecteds[questionIndex] != null && selecteds[questionIndex].selectIndexsList.Count == 0)
        //{
        //    nextButton.interactable = false;

        //}
        //else {
        //    nextButton.interactable = true;
        //}
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.F2))
        {
            NextButton();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            RecordPosInfo();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            TransLoctMethod();
        }

#endif
        if (ScenceData.currentState == ScenceState.exam && ScenceData.questions != null && ScenceData.questions.Length > questionIndex && ScenceData.questions[questionIndex].questionType == Question.QuestionType.model)
        {
            AccumilatedTime = AccumilatedTime + Time.deltaTime;
            //in case the FPS is too slow, we may need to update the game multiple times a frame   

            while (AccumilatedTime > Mathf.Max(0.3f, FrameLength))
            {
                GameFrameTurn();
                AccumilatedTime = AccumilatedTime - Mathf.Max(0.3f, FrameLength);
            }


        }
        if (ScenceData.isOpenTime && !isSelect && ScenceData.currentState == ScenceState.exam)
        {
            //选择计时
            TimeControl();
        }
    }
    //闪烁控制Flag
    bool flash = true;
    /// <summary>
    /// 帧同步执行函数，控制高亮模型闪烁
    /// </summary>
    public void GameFrameTurn()
    {
        try
        {
            flash = !flash;
            flashModels(ScenceData.questions[questionIndex], flash);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }
    /// <summary>
    /// 切换定位模式（调试用）
    /// </summary>
    public void TransLoctMethod()
    {
        //autoPos = !autoPos;
        //try
        //{
        //    if (autoPos)
        //    {
        //        resetPos();
        //    }
        //}
        //catch (Exception e)
        //{
        //    Debug.Log(e.Message);
        //}
    }
    /// <summary>
    /// 记录相机信息（调试用）
    /// </summary>
    public void RecordPosInfo()
    {
        //Transform box = Camera.main.transform.parent;
        //Debug.Log("<color=blue>" + ScenceData.questions[questionIndex].qid + "!!" + box.rotation.eulerAngles + "@@" + Camera.main.transform.localPosition + "</color>");
        //string qid = ScenceData.questions[questionIndex].qid;
        //if (recordDic.ContainsKey(qid))
        //{
        //    recordDic.Remove(qid);
        //}
        //recordDic.Add(qid, box.rotation.eulerAngles + "@@" + Camera.main.transform.localPosition);

        ////FileStream fs = new FileStream(Application.persistentDataPath+"/location.json",FileMode.Create,FileAccess.ReadWrite);
        ////fs.Write();
        ////StartPanel.editXmlDoc
        //XmlElement xn = (XmlElement)StartPanel.editXmlDoc.SelectSingleNode("//Question[@qid='" + qid + "']/cameraParameter");
        //xn.SetAttribute("localPos", PublicTools.Vector32Str(Camera.main.transform.localPosition));
        //xn.SetAttribute("boxRotation", PublicTools.Vector32Str(box.rotation.eulerAngles));
        //StartPanel.editXmlDoc.Save(BankScriControl.currentQaPath + GlobalVariable.bankName + ".xml");
        //Question q = ScenceData.questions[questionIndex];
        //q.cp.localPos = Camera.main.transform.localPosition;
        //q.cp.boxRotation = box.rotation.eulerAngles;

        //File.WriteAllText(BankScriControl.currentQaPath + "location" + GlobalVariable.libName + ".json", JsonConvert.SerializeObject(recordDic), Encoding.UTF8);
    }


    void OnGUI()
    {
        //时间锁
        if (DateTime.Now < new DateTime(2018, 8, 1, 0, 0, 0, DateTimeKind.Local))
        {

            //if (Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.clickCount == 2)
            //{

            //    ("double click ");
            //    transform.GetComponent<ModelControl>().overturn();
            //}
            GUIStyle bb = new GUIStyle();
            bb.normal.background = null;    //这是设置背景填充的
            bb.normal.textColor = Color.red;// new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
            bb.fontSize = 40;       //当然，这是字体大小

            ////居中显示FPS
            //GUI.Label(new Rect((Screen.width / 2) - 40, 0, 200, 200), "FPS: " + m_FPS, bb);

            if (ScenceData.questions != null && ScenceData.questions.Length>questionIndex) {
                // GUI.Label(new Rect((Screen.width / 5), 0, 200, 200), "reallyTime: " + System.DateTime.Now.Second, bb);
                // GUI.Label(new Rect((Screen.width / 5), 50, 200, 200), " qid: " + ScenceData.questions[questionIndex].qid, bb);
                // GUI.Label(new Rect(10, 100, 200, 200),"Highlight:"+ highLightNames.Replace("@","\r\n"), bb);
            }

        }
    }


    /// <summary>
    /// 时间控制
    /// </summary>
    void TimeControl()
    {
        //("TimeControl");
        //如果在测试状态下
        if (!isSelect && ScenceData.currentState == ScenceState.exam)
        {
            //如果倒计时时间大于0
            if (timeDown > 0)
            {
                //时间递减
                timeDown -= Time.deltaTime;
                //显示倒计时时间
                textTimeDown.text = ((int)timeDown).ToString();
                //图片以圆的形式缩短
                imageTimeDown.fillAmount = timeDown / totalTime;
            }
            else
            {
                //倒计时是否结束置为true
                ScenceData.isTimeOver = true;
                //状态切换为停止状态
                ScenceData.currentState = ScenceState.stop;
            }
            //如果倒计时已经结束
            if (ScenceData.isTimeOver)
            {

                ScenceData.currentState = ScenceState.exam;
                ScenceData.isTimeOver = false;
                NextButton();
                ////时间递减
                //timeDown = totalTime;
                ////显示倒计时时间
                //textTimeDown.text = ((int)timeDown).ToString();
                ////图片以圆的形式缩短
                //imageTimeDown.fillAmount = totalTime / totalTime;
            }
        }
    }
    /// <summary>
    /// 按钮上的题目尺寸大小调整
    /// </summary>
    /// <param name="index">四个按钮在按钮数组中的索引</param>
    void FontsizeAdjust(int index)
    {
        //该按钮上的文本字体上的尺寸置为标准尺寸
        textInButton[index].fontSize = ScenceData.fontSize;
        //获取按钮上的文本内容
        string tempStr = textInButton[index].text;
        //按钮上的文本内容置为空
        textInButton[index].text = "";
        //设置字节数（随意设尽量大）
        int temp = 100;
        //按钮上的文本是否居左（默认不居左，写反了有误，请注意）
        bool isLeft = true;
        //遍历字节数
        for (int i = 0; i < tempStr.Length; i++)
        {
            //如果文本内容已经大于390，在递增两个字节就自动换行
            if (i == temp + 2)
            {
                //换行
                textInButton[index].text += "\n";
            }
            //如果按钮上的文本长度大于390
            if (textInButton[index].preferredWidth > 390)
            {
                //该按钮上的文本字体上的尺寸置为缩小尺寸
                textInButton[index].fontSize = ScenceData.shrinkedFontsize;
                //获得其字节数
                temp = i;
                //居左
                isLeft = false;
            }
            //以字节形式获取文本内容
            textInButton[index].text += tempStr.Substring(i, 1);
            //如果文本不居左
            if (isLeft)
            {
                //设置不居左文本的坐标
                textInButton[index].rectTransform.anchoredPosition3D = new Vector3(37, 0, 0);
                //当文本长度大于380并且是最后一个字节时
                if (textInButton[index].preferredWidth > 380 && i == tempStr.Length - 1)
                {
                    //文本字体上的尺寸置为缩小尺寸
                    textInButton[index].fontSize = ScenceData.shrinkedFontsize;
                    //如果当文本长度还是大于380
                    if (textInButton[index].preferredWidth > 380)
                    {
                        //设置居左文本坐标
                        textInButton[index].rectTransform.anchoredPosition3D = new Vector3(10, 0, 0);
                    }
                }
            }
            //如果不居左
            else
            {
                //设置居左文本的坐标
                textInButton[index].rectTransform.anchoredPosition3D = new Vector3(10, 0, 0);
            }
        }
    }
    /// <summary>
    /// 打乱从题库中挑选题目的顺序
    /// </summary>
    public void DisturbAnswer()
    {
        //("DisturbAnswer");
        if (ScenceData.questions.Length < 2)
        {
            return;
        }
        //打乱数组中元素顺序
        int x, y; Question tmp;
        for (int i = 0; i < ScenceData.questions.Length; i++)
        {
            x = UnityEngine.Random.Range(0, ScenceData.questions.Length);
            do
            {
                y = UnityEngine.Random.Range(0, ScenceData.questions.Length);
            } while (y == x);
            tmp = ScenceData.questions[x];
            ScenceData.questions[x] = ScenceData.questions[y];
            ScenceData.questions[y] = tmp;
        }
    }
    /// <summary>
    /// 重置参数
    /// </summary>
    public void ResetFactor()
    {
        if (SceneModels.instance != null)
        {
            SceneModels.instance.closeTempParent();
            SceneModels.instance.destoryTempParent();
            SceneModels.instance = null;
        }

        isMultipleChoice = false;
        questionTip.text = "单选";
        selectAnswers = null;
        //nextButton.interactable = false;
        showAnswerButton.interactable = true;
        //状态切换为重新开始状态
        ScenceData.currentState = ScenceState.rePrepare;
        //if (transform.GetComponent<ModelControl>().isStartControl)
        //{
        //    transform.GetComponent<ModelControl>().stop();
        //}
        //如果倒计时开关开启
        if (ScenceData.isOpenTime)
        {
            //重置倒计时时间文本
            textTimeDown.text = totalTime.ToString();
            //倒计时图片设为完整状态（未倒计时的整圆）
            imageTimeDown.fillAmount = 1;
        }
        for (int i = 0; i < imageListFather.transform.childCount; i++)
        {
            //销毁所有结果列表
            Destroy(imageListFather.transform.GetChild(i).gameObject);
        }
        ScenceData.listIndex = 0;
        ScenceData.score = 0;
        questionIndex = 0;
        ScenceData.rightIndex = 0;
        selecteds = null;
        ScenceData.questionTotal = 0;
        //重置得分文本
        textScore.text = "得分：" + 0;
        isSelect = false;
        transform.GetComponent<QuestingPanel>().endPanel.SetActive(false);
        transform.GetComponent<StartPanel>().startPanel.SetActive(true);
    }

    //List<int> selectButtonIndexs;
    /// <summary>
    /// 点击选项按钮后的触发事件
    /// </summary>
    /// <param name="index">按钮在数组中的索引</param>
    public void SelectButton(GameObject obj)
    {
        if (GlobalVariable.practiceState != PracticeState.classes && !isMultipleChoice)
        {
            if (isSelect)
            {
                Debug.Log("已选择！！！！");
                return;
            }
            ensureSelect(obj);
            isSelect = true;
            return;
        }
        int tmpIndex = int.Parse(obj.name);
        Debug.Log(tmpIndex + "====Button Down");


        if (selecteds[questionIndex].selectIndexsList.Contains(tmpIndex))
        {
            //初始化颜色
            setButtonColor(tmpIndex, ScenceData.initColor);

            selecteds[questionIndex].selectIndexsList.Remove(tmpIndex);
            selectAnswers.Remove(buttons[tmpIndex].GetComponentInChildren<Text>().text);
            Debug.Log("删除：" + buttons[tmpIndex].GetComponentInChildren<Text>().text);
        }
        else
        {
            if (!isMultipleChoice && selecteds[questionIndex].selectIndexsList.Count != 0)
            {
                setButtonColor(selecteds[questionIndex].selectIndexsList[0], ScenceData.initColor);
                selecteds[questionIndex].selectIndexsList.Clear();
            }
            setButtonColor(tmpIndex, ScenceData.buttonIsTrue);

            selecteds[questionIndex].selectIndexsList.Add(tmpIndex);

        }
    }

    /// <summary>
    /// 设置选项颜色
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="color">颜色</param>
    public void setButtonColor(int index, Color color)
    {
        textInButton[index].transform.parent.GetComponent<Image>().color = color;
        target[index].color = color;
    }
    public class MyButton
    {
        //嗯这是一个按钮
        public Button oneButton;
        public Image oneImage;
    }
    /// <summary>
    /// 确认选择
    /// </summary>
    /// <param name="obj"></param>
    public void ensureSelect(GameObject obj)
    {
        Debug.Log("ensureSelect");
        //if(ListA.Count == ListB.Count && ListA.Count(t => !ListB.Contains(c)) == 0)
        string tmp = obj.name;
        //tmp = tmp.Replace("Button", "");
        string answer = obj.GetComponentInChildren<Text>().text;
        int index = (int)float.Parse(tmp);
        //(isMultipleChoice);
        Debug.Log("isSelect:=====================" + isSelect);
        if (!isSelect)
        {
            if (isMultipleChoice)
            {
                if (selectAnswers.Contains(answer))
                {
                    Debug.Log("selectAnswers.Remove(answer)::" + answer);
                    selectAnswers.Remove(answer);
                    textInButton[index].transform.parent.GetComponent<Image>().color = ScenceData.initColor;
                    target[index].color = ScenceData.initColor;
                }
                else
                {
                    Debug.Log("selectAnswers.Add(answer)" + answer);
                    selectAnswers.Add(answer);
                    //按钮颜色置为正确颜色
                    textInButton[index].transform.parent.GetComponent<Image>().color = ScenceData.buttonIsTrue;
                    //选项颜色置为正确颜色
                    target[index].color = ScenceData.buttonIsTrue;
                }
            }
            else
            {

                isSelect = true;
                nextButtonText.text = "下一题";
                //nextButton.interactable = true;
                Debug.Log("isSelect:=====================" + isSelect);
                showAnswerButton.interactable = false;
                if (ScenceData.currentState == ScenceState.exam)
                {
                    //(textInButton.Length);
                    string text = textInButton[index].GetComponent<Text>().text;
                    //(text);
                    Selected select = selecteds[questionIndex];
                    select.qid = ScenceData.questions[questionIndex].qid;
                    Debug.Log("select.qid ");
                    select.questionIndex = questionIndex;
                    select.selectContent = text;
                    if (index == ScenceData.rightIndex)
                    {
                        //rightImage.SetActive(true);
                        select.isRight = true;
                        //按钮颜色置为正确颜色
                        textInButton[index].transform.parent.GetComponent<Image>().color = ScenceData.buttonIsTrue;
                        //选项颜色置为正确颜色
                        target[index].color = ScenceData.buttonIsTrue;
                    }
                    else
                    {
                        //falseImage.SetActive(true);
                        select.isRight = false;
                        if (GlobalVariable.practiceState == PracticeState.classes)
                        {
                            //按钮颜色置为正确颜色
                            textInButton[index].transform.parent.GetComponent<Image>().color = ScenceData.buttonIsTrue;
                            //选项颜色置为正确颜色
                            target[index].color = ScenceData.buttonIsTrue;
                        }
                        else
                        {
                            Debug.Log("isSelect:=====================" + isSelect);
                            //按钮颜色置为错误颜色
                            textInButton[index].transform.parent.GetComponent<Image>().color = ScenceData.buttonIsFalse;
                            //选项颜色置为错误颜色
                            target[index].color = ScenceData.buttonIsFalse;
                            Debug.Log("isSelect:=====================" + target[index].color + " _" + target[index].name);
                            //按钮颜色置为正确颜色
                            textInButton[ScenceData.rightIndex].transform.parent.GetComponent<Image>().color = ScenceData.buttonIsTrue;
                            //选项颜色置为正确颜色
                            target[ScenceData.rightIndex].color = ScenceData.buttonIsTrue;


                        }
                    }
                    selecteds[questionIndex] = select;
                    resultForButton(select.isRight);
                }

            }
        }
        //if (!isMultipleChoice && questionIndex == ScenceData.questionTotal - 1)
        if (questionIndex == selecteds.Length - 1)
        {
            nextButton.GetComponentInChildren<Text>().text = "显示结果";
        }
        //nextButton.onClick.Invoke();
    }
    /// <summary>
    /// 点击下一题按钮触发事件
    /// </summary>
    public void NextButton()
    {

        falseImage.SetActive(false);
        rightImage.SetActive(false);
        //if (nextButtonText.text == "显示结果")
        //{
        //    nextButton.interactable = false;

        //    showAnswerButton.interactable = true;
        //    changQueation();
        //    return;
        //}
        if (isMultipleChoice)
        {
            selectAnswers.Clear();
        }
        if (GlobalVariable.practiceState == PracticeState.classes)
        {
            isSelect = false;
            foreach (int tmpIndex in selecteds[questionIndex].selectIndexsList)
            {
                Debug.Log("存在：" + buttons[tmpIndex].GetComponentInChildren<Text>().text);
                ensureSelect(buttons[tmpIndex].gameObject);
            }
            if (isMultipleChoice)
            {
                resultForButton(false);
            }
        }
        else
        {
            if (nextButtonText.text == "确定")
            {
                isSelect = true;
                resultForButton(false);

                return;
            }

        }
        selecteds[questionIndex].isSelected = true;
        //if (!isSelect) {

        //}

        //if (nextButtonText.text == "确定")
        //{
        //    showAnswerButton.interactable = false;
        //    isSelect = true;
        //    resultForButton(false);
        //    if (questionIndex == ScenceData.questionTotal - 1)
        //    {
        //        nextButtonText.text = "显示结果";
        //        ("最后一题");
        //    }
        //    return;
        //}
        //if (!isSelect)
        //{
        //    showAnswer();
        //    return;
        //}
        if (ScenceData.currentState == ScenceState.exam)
        {
            //nextButton.interactable = false;
            //showAnswerButton.interactable = true;
            //赋值选项GameObject
            //selecteds[questionIndex].selectDic = selectDic;

            changQueation();
        }
        Debug.Log(questionIndex + "-!!-!!!-" + selecteds.Length);
        if (questionIndex == selecteds.Length - 1)
        {
            nextButtonText.text = "显示结果";
        }
    }
    /// <summary>
    /// 显示答案
    /// </summary>
    public void showAnswer()
    {
        if (!isSelect)
        {
            showAnswerButton.interactable = false;
            nextButton.interactable = true;
            nextButtonText.text = "下一题";
            resultForButton(false);
            if (questionIndex == selecteds.Length - 1)
            {
                nextButton.GetComponentInChildren<Text>().text = "显示结果";
            }
            isSelect = true;
            return;
        }
    }
    /// <summary>
    /// 点击返回按钮
    /// </summary>
    public void ResetButton()
    {
        //如果在测验状态下
        if (ScenceData.currentState == ScenceState.exam)
        {
            //冻结时间
            Time.timeScale = 0;
            //打开返回界面
            //if (transform.GetComponent<ModelControl>().isStartControl)
            //{
            //    transform.GetComponent<ModelControl>().modelImage.SetActive(false);

            //}
            QueBackPanel.SetActive(true);
            //切换为结束状态下
            ScenceData.currentState = ScenceState.end;
        }
    }
    /// <summary>
    /// 点击确认返回
    /// </summary>
    public void ConfirmReset()
    {
        //if (transform.GetComponent<ModelControl>().isStartControl)
        //{
        //    transform.GetComponent<ModelControl>().stop();
        //}
        transform.GetComponent<PictureControl>().stop();
        transform.GetComponent<MovieControl>().stop();
        //解冻时间
        Time.timeScale = 1;
        //重置数据
        ResetFactor();
        //关闭测试界面
        questingPanel.SetActive(false);
        //关闭返回界面
        QueBackPanel.SetActive(false);
    }
    /// <summary>
    /// 点击取消返回
    /// </summary>
    public void cancelReset()
    {
        //if (transform.GetComponent<ModelControl>().isStartControl)
        //{
        //    transform.GetComponent<ModelControl>().modelImage.SetActive(true);
        //}
        //解冻时间
        Time.timeScale = 1;
        //关闭返回界面
        QueBackPanel.SetActive(false);
        //切换为测试时间
        ScenceData.currentState = ScenceState.exam;
    }
    /// <summary>
    /// 选择后的结果
    /// </summary>
    /// <param name="isTrue">是否答对</param>
    void resultForButton(bool isTrue)
    {
        Debug.Log("isTrue:=====================" + isTrue);
        //多选题判断，否则单选题判断
        if (isMultipleChoice)
        {
            List<string> rightAnswersTmp = ScenceData.questions[questionIndex].rightAnswers;
            Selected select = selecteds[questionIndex];
            select.qid = ScenceData.questions[questionIndex].qid;
            select.questionIndex = questionIndex;
            isTrue = true;
            if (selectAnswers.Count > 1 && selectAnswers.Contains("Z"))
            {
                selectAnswers.Remove("Z");
            }
            select.selectContent = string.Join("-", selectAnswers.ToArray());
            Debug.Log(select.selectContent + "---------resultForButton");
            //判断两列表内容一样
            if (rightAnswersTmp.Count == selectAnswers.Count)
            {
                for (int i = 0; i < selectAnswers.Count; i++)
                {
                    if (!rightAnswersTmp.Contains(selectAnswers[i]))
                    {
                        isTrue = false;
                        break;
                    }
                }
            }
            else
            {
                isTrue = false;
            }
            select.isRight = isTrue;
            if (!isTrue)
            {
                for (int i = 0; i < textInButton.Length; i++)
                {
                    if (rightAnswersTmp.Contains(textInButton[i].text))
                    {
                        //按钮颜色置为正确颜色
                        textInButton[i].transform.parent.GetComponent<Image>().color = ScenceData.buttonIsTrue;
                        //选项颜色置为正确颜色
                        target[i].color = ScenceData.buttonIsTrue;
                    }
                    else if (selectAnswers.Contains(textInButton[i].text))
                    {

                        if (GlobalVariable.practiceState != PracticeState.classes)
                        {
                            // 按钮颜色置为错误颜色
                            textInButton[i].transform.parent.GetComponent<Image>().color = ScenceData.buttonIsFalse;
                            //选项颜色置为错误颜色
                            target[i].color = ScenceData.buttonIsFalse;
                        }
                        else
                        {
                            //按钮颜色置为正确颜色
                            textInButton[i].transform.parent.GetComponent<Image>().color = ScenceData.buttonIsTrue;
                            //选项颜色置为正确颜色
                            target[i].color = ScenceData.buttonIsTrue;
                        }
                    }

                }
            }
            selecteds[questionIndex] = select;
            nextButtonText.text = "下一题";
        }
        else
        {
            if (!isTrue)
            {
                //没有选择 直接显示正确答案
                if (!isSelect)
                {
                    isSelect = true;
                    Selected select = selecteds[questionIndex];
                    select.questionIndex = questionIndex;
                    select.selectContent = "Z";
                    select.qid = ScenceData.questions[questionIndex].qid;
                    select.isRight = false;
                    for (int i = 0; i < textInButton.Length; i++)
                    {
                        if (i == ScenceData.rightIndex)
                        {
                            //按钮颜色置为正确颜色
                            textInButton[i].transform.parent.GetComponent<Image>().color = ScenceData.buttonIsTrue;
                            //选项颜色置为正确颜色
                            target[i].color = ScenceData.buttonIsTrue;
                        }

                    }
                    selecteds[questionIndex] = select;
                }

            }
        }
        if (isTrue)
        {
            //如果开启音效
            if (ScenceData.isOpenSound)
            {
                //播放正确音效
                audioSc.PlayOneShot(clipIsTrue);
            }
            ScenceData.score += 1.0f / (float)ScenceData.questions.Length * GlobalVariable.score;
            textScore.text = string.Format("得分：{0:N0}", ScenceData.score);
        }
        else
        {
            //如果开启音效
            if (ScenceData.isOpenSound)
            {
                //播放错误音效
                audioSc.PlayOneShot(clipIsFalse);
            }
        }
        selecteds[questionIndex].isRight = isTrue;
        QaDao.instance.recordErrorQa(ScenceData.questions[questionIndex].qid, GlobalVariable.bankName, GlobalVariable.libName, !selecteds[questionIndex].isRight);

    }
    //【我的班级】测试结束遮挡答案
    public GameObject ClassHidePanel;
    /// <summary>
    /// 进入结束界面
    /// </summary>
    /// <returns></returns>
    IEnumerator EnterEndPanel()
    {
        Time.timeScale = 0;
        questionIndex = 0;
        //结束时计算得分和结果列表
        ScenceData.score = 0;
        while (questionIndex < selecteds.Length)
        {
            if (selecteds[questionIndex] != null && selecteds[questionIndex].isSelected && selecteds[questionIndex].isRight)
            {
                ScenceData.score += 1.0f / (float)ScenceData.questions.Length * GlobalVariable.score;

            }
            CreatList();
            questionIndex++;
        }
        Time.timeScale = 1;
        if (GlobalVariable.practiceState == PracticeState.classes)
        {
            //考试不显示结果列表
            ClassHidePanel.SetActive(true);
        }
        //切换为结束状态
        ScenceData.currentState = ScenceState.end;
        transform.GetComponent<PictureControl>().stop();
        transform.GetComponent<MovieControl>().stop();

        Resources.UnloadUnusedAssets();
        //销毁模型
        //if (transform.GetComponent<ModelControl>().isStartControl)
        //{
        //    transform.GetComponent<ModelControl>().stop();
        //}
        //延迟1秒钟
        //yield return new WaitForSeconds(1);
        initbutton(0);
        //关闭测试界面
        questingPanel.SetActive(false);
        //打开结束界面
        endPanel.SetActive(true);

        //切换为结束状态
        ScenceData.currentState = ScenceState.summary;
        ScenceData.useTime = PublicTools.GetTimeStamp() - ScenceData.useTime;
        //显示结束状态下的数据
        transform.GetComponent<EndPanel>().EndShow();
        yield return null;
    }
    /// <summary>
    /// 进入下一题的线程
    /// </summary>
    /// <returns></returns>
    IEnumerator waitSeconds(float time = 1f)
    {
        yield return new WaitForSeconds(1);
    }
    /// <summary>
    /// 创建结果列表
    /// </summary>
    void CreatList()
    {
        GameObject temp;
        //实例生成出列表内容
        temp = Instantiate(imageList) as GameObject;
        //设置父物体
        temp.transform.SetParent(imageListFather.transform);
        //设置结果列表的位置
        temp.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(temp.transform.localPosition.x, temp.transform.localPosition.y, 0);
        //设置结果列表的大小
        temp.transform.localScale = Vector3.one;
        //设置结果列表内容名
        temp.name = "ImageList_" + ScenceData.listIndex + "_" + questionIndex;
        ScenceData.listIndex++;
        //显示结果列表内容正确答案
        temp.transform.GetChild(0).GetComponent<Text>().text = questionIndex + ".\t" + string.Join("\t", ScenceData.questions[questionIndex].rightAnswers.ToArray());

        if (selecteds[questionIndex].isRight)
        {
            temp.transform.GetChild(0).GetComponent<Text>().color = ScenceData.listIsTrue;
        }
        else
        {
            temp.transform.GetChild(0).GetComponent<Text>().color = ScenceData.listIsFalse;
            temp.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Shape 16");
        }
        //结果列表的补充
        NextList(temp);
    }
    /// <summary>
    /// 补充结果列表内容（自适应题目长度，如果图片存放不下，在创建一张图片继续题目文本内容）
    /// </summary>
    /// <param name="longTemp">此前结果列表的内容</param>
    void NextList(GameObject longTemp)
    {
        //如果结果列表内容超过列表本身容量
        if (longTemp.transform.GetChild(0).GetComponent<Text>().text.Length >= ScenceData.imageListTextCharNum)
        {
            //获取结果列表内容长度
            int length = longTemp.transform.GetChild(0).GetComponent<Text>().text.Length;
            //获取超长文本内容
            string tempStr = longTemp.transform.GetChild(0).GetComponent<Text>().text;
            //列表内容字符串改为仅到最大字符数的字符串
            longTemp.transform.GetChild(0).GetComponent<Text>().text = tempStr.Substring(0, ScenceData.imageListTextCharNum);
            //正确、错误图标隐藏
            longTemp.transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 0);
            GameObject temp2;
            //在实例生成一个图片
            temp2 = Instantiate(imageList) as GameObject;
            //设置父物体
            temp2.transform.SetParent(imageListFather.transform);
            //设置位置
            temp2.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(temp2.transform.localPosition.x, temp2.transform.localPosition.y, 0);
            //设置大小
            temp2.transform.localScale = Vector3.one;
            //设置结果列表名
            temp2.name = "ImageList_" + ScenceData.listIndex + "_" + questionIndex;
            ScenceData.listIndex++;
            //列表内容字符串紧接之前的字符串
            temp2.transform.GetChild(0).GetComponent<Text>().text = tempStr.Substring(ScenceData.imageListTextCharNum, length - ScenceData.imageListTextCharNum);
            //如果是正确的结果列表
            if (selecteds[questionIndex].isRight)
            {
                //最大字符数结果列表颜色置为正确颜色
                longTemp.transform.GetChild(0).GetComponent<Text>().color = ScenceData.listIsTrue;
                //连接字符结果列表颜色置为正确颜色
                temp2.transform.GetChild(0).GetComponent<Text>().color = ScenceData.listIsTrue;
            }
            else
            {
                //最大字符数结果列表颜色置为错误颜色
                longTemp.transform.GetChild(0).GetComponent<Text>().color = ScenceData.listIsFalse;
                //连接字符结果列表颜色置为错误颜色
                temp2.transform.GetChild(0).GetComponent<Text>().color = ScenceData.listIsFalse;
                //图标换位错误图标
                temp2.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Shape 16");
            }
        }
        //如果结果列表内容不超过列表本身容量
        else
        {
            //如果是正确的结果列表
            if (selecteds[questionIndex].isRight)
            {
                //结果列表颜色置为正确颜色
                longTemp.transform.GetChild(0).GetComponent<Text>().color = ScenceData.listIsTrue;
            }
            else
            {
                //结果列表颜色置为错误颜色
                longTemp.transform.GetChild(0).GetComponent<Text>().color = ScenceData.listIsFalse;
                //图标换位错误图标
                longTemp.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Shape 16");
            }
        }
    }


    WebServicesBase wsb = new WebServicesBase();
    //【我的班级】删除错题按钮
    public GameObject deleteButton;
    /// <summary>
    /// 删除错题调用
    /// </summary>
    public void deleteMethod()
    {
        showAnswer();
        NextButton();
        StartCoroutine(deleteErrorQues());
    }
    /// <summary>
    /// 删除错题
    /// </summary>
    public IEnumerator deleteErrorQues()
    {
        KeyValue[] kvs = new KeyValue[2];
        kvs[0] = new KeyValue("id", ScenceData.questions[questionIndex - 1].qid);
        kvs[1] = new KeyValue("stu", ClassPanelControll.sin.id);
        UnityWebRequest uw = wsb.PostWithParams(servelets.deleteWrongTitle, kvs);
        yield return uw.Send();
        Debug.Log(uw.downloadHandler.text);
        if (!uw.isNetworkError)
        {

        }

    }
}
/// <summary>
/// 选项记忆类
/// </summary>
public class Selected
{
    //乱序答案列表
    public List<String> randomAnswers;
    //public Dictionary<string, GameObject> selectDic;
    //选择的答案索引列表
    public List<int> selectIndexsList = new List<int>();
    //问题索引
    public int questionIndex;
    //是否答题正确
    public bool isRight;
    //选项内容
    public string selectContent = "Z";
    //选项
    public string option;
    //问题id
    public string qid;
    //是否已选择
    public bool isSelected;
}
