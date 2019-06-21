using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
/// <summary>
/// 结束界面控制器
/// </summary>
public class EndPanel : MonoBehaviour
{
    //显示得分（正确率）的图片，会在结束界面出现转动（图片完整率递增）
    public Image imageScore;
    //分数旋转的速度
    public float rotateScoreIv;
    //得分文本
    public Text textScore;
    //测试时间文本
    public Text textTotalTime;
    //得分文本（会转动有动画的文本）
    public Text textRate;
    //最高得分文本
    public Text textBestScore;
    //得分文本（会转动有动画的文本）
    public Text textMove;
    // public GameObject lightPoint;
    //返回界面
    public GameObject backPanel_2;
    //学渣、学霸、柳叶刀客....
    public Sprite[] judgeWordArray;
    //不同分数显示不同结果的图片
    public Image judgeWord;
    //正确率图片与文字旋转速度
    public float rotateSpeed;
    //提交分数按钮
    public GameObject export;

    //初始化旋转文本位置
    private Vector3 initTextPos = new Vector3(-200, -225, 0);
    //初始化旋转亮点位置
    private Vector3 initLightPointPos = new Vector3(-250, -318, 0);
    //旋转末尾限定
    public static float fillPointOver = 0.788f;
    //旋转比
    float totalFillAmount = -1f;
    private void Awake()
    {

    }
    private void Start()
    {
        // totalFillAmount = 10f;
    }
    
    private void OnEnable()
    {
        
    }
    /// <summary>
    /// 随机颜色
    /// </summary>
    /// <returns></returns>
    public Color randomColor()
    {
        int r = UnityEngine.Random.Range(0, 256);
        int g = UnityEngine.Random.Range(0, 256);
        int b = UnityEngine.Random.Range(0, 256);
        return new Color(r, g, b);
    }
    //得分画面旋转速度
    float speed = 0.007f;
    private void FixedUpdate()
    {
        //控制得分动画旋转
        if (totalFillAmount != -1f)
        {
            if (imageScore.fillAmount + speed >= totalFillAmount)
            {
                imageScore.fillAmount = totalFillAmount;

                //textMove.gameObject.transform.position = initTextPos;
                textMove.GetComponent<RectTransform>().anchoredPosition3D = initTextPos;
                //文本旋转到位
                textMove.gameObject.transform.RotateAround(imageScore.transform.position, Vector3.back, totalFillAmount * 360f);
                //四元数角度保持不变
                textMove.gameObject.transform.localRotation = Quaternion.identity;
                //textMove.gameObject.transform.rotation;
                //亮点到位
                //lightPoint.GetComponent<RectTransform>().anchoredPosition3D = initLightPointPos;
                // lightPoint.transform.RotateAround(imageScore.transform.position, Vector3.back, totalFillAmount * 360f);
                // lightPoint.transform.localRotation = Quaternion.identity;
                totalFillAmount = -1f;
               
                //lightPoint.GetComponent<ParticleSystem>().Stop();
            }
            else
            {
                imageScore.fillAmount += speed;
                //转动得分的递增
                textMove.gameObject.transform.RotateAround(imageScore.transform.position, Vector3.back, speed * 360f);
                textMove.gameObject.transform.localRotation = Quaternion.identity;
                // lightPoint.transform.RotateAround(imageScore.transform.position, Vector3.back, speed * 360f);
                // lightPoint.transform.localRotation = Quaternion.identity;

            }
        }
    }
    private void Update()
    {
    }
    /// <summary>
    /// 提交分数类
    /// </summary>
    public class submitScore {
        //memberId
        //学生编号
        public string no;
        //唯一识别码
        public string uuid;
        //试卷id
        public string testpaperId;
        //学生id
        public string studentId;
        //班级id
        public string classesId;
        //测试得分
        public float testpaperStudentScore;
        //选项信息
        public Option[] data;
    }
    /// <summary>
    /// 提交的选项结果
    /// </summary>
    public class Option
    {
        public Option(string id, string option) {
            this.id = id;
            this.option = option;
        }
        public string id;
        public string option;
    }
    /// <summary>
    /// 根据答案内容确定ABCDE
    /// </summary>
    public  IEnumerator GetOption() {
        JsonToXmlTool.getOption();
        submitScore ss = new submitScore();
        ss.uuid = AppOpera.myClass.rykjMemberId;
        ss.testpaperId = Question.LibId;
        ss.testpaperStudentScore = ScenceData.score;
        ss.studentId = ClassPanelControll.sin.id;
        ss.no = ClassPanelControll.sin.no;
        ss.classesId = ClassPanelControll.currentClassId;
        List<Option> datas = new List<Option>();
        Debug.Log(JsonConvert.SerializeObject(QuestingPanel.selecteds));
        foreach (Selected st in QuestingPanel.selecteds) {
            if (!st.isRight) {
                datas.Add(new Option(st.qid,st.option));
            }
        }
        ss.data = datas.ToArray();
        Debug.Log(JsonConvert.SerializeObject(ss));
        KeyValue kv = new KeyValue("testpaperjson", JsonConvert.SerializeObject(ss));
        WebServicesBase wsb = new WebServicesBase();
        UnityWebRequest uw = wsb.PostWithParams(servelets.commitScore,kv);
        yield return uw.Send();
        Debug.Log(uw.downloadHandler.text);
        //servelets
        //string filePath = 
    }
    /// <summary>
    /// 进入结束界面后的数据显示
    /// </summary>
    public void EndShow()
    {
        if (GlobalVariable.practiceState == PracticeState.classes) {
            if (ScenceData.testPaperTimeInfo.Contains("~")) {
                string[] timeInfo = ScenceData.testPaperTimeInfo.Split('~');
                Debug.Log(timeInfo[0]);
                Debug.Log(timeInfo[1]);
                
                if (DateTime.Compare(DateTime.Now, DateTime.Parse(timeInfo[0])) >= 0 && DateTime.Compare(DateTime.Now, DateTime.Parse(timeInfo[1])) <= 0) {
                    Debug.Log("在期限范围内");
                     StartCoroutine(GetOption());
                }
            }
        }
       
        textMove.GetComponent<RectTransform>().anchoredPosition3D = initTextPos;
        //lightPoint.GetComponent<RectTransform>().anchoredPosition3D = initLightPointPos;
        //将测试时间转换为时间格式
        System.TimeSpan timeSpan = new System.TimeSpan(0, 0, (int)ScenceData.useTime);
        //显示测试时间     
        textTotalTime.text = "总 用 时 " + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
        float score = (float)ScenceData.score / GlobalVariable.score;

        textRate.text = String.Format("正 确 率 {0:N2} %", ScenceData.score);
        //textMove.text = tmpQp.score.ToString();
        Debug.Log(score);
        if (score != 0f)
        {
            totalFillAmount = score * fillPointOver;
        }
      
        //imageScore.fillAmount = ;
        //{0:N2}两位小数
        textBestScore.text = String.Format("最 高 得 分 {0:N0}" , ScenceData.score);
        textScore.text = String.Format("{0:N0} 分", ScenceData.score);
        //显示得分的递增
        textMove.text = String.Format("{0:N0}", ScenceData.score);


        //如果得分小于60分
        if (score < 0.6)
        {
            //显示学渣图片
            judgeWord.sprite = judgeWordArray[0];
        }
        else if (score < 0.8)
        {
            //显示学霸图片
            judgeWord.sprite = judgeWordArray[1];
        }
        else if (score < 1)
        {
            //显示柳叶刀客图
            judgeWord.sprite = judgeWordArray[2];
        }
        else
        {
            //显示史诗级外科医生图
            judgeWord.sprite = judgeWordArray[3];
        }
        //显示提交分数按钮
        //export.SetActive(true);


    }



    /// <summary>
    /// 返回按钮
    /// </summary>
    public void Back()
    {
        //冻结时间
        //Time.timeScale = 0;
       
            //打开退出界面
            backPanel_2.SetActive(true);
        
    }

    /// <summary>
    /// 确认返回开始界面
    /// </summary>
    public void BackConfirm()
    {
        //解冻时间
        Time.timeScale = 1;
        totalFillAmount = -1f;
        //图片的完整比率重置为0
        imageScore.fillAmount = 0;
        //转动文本的位置设为初始位置
        textMove.GetComponent<RectTransform>().anchoredPosition3D = initTextPos;
        //lightPoint.GetComponent<RectTransform>().anchoredPosition3D = initLightPointPos;
        transform.GetComponent<QuestingPanel>().ResetFactor();
        //重置数据
        backPanel_2.SetActive(false);
        //关闭返回界面
        ScenceData.currentState = ScenceState.rePrepare;


    }

    /// <summary>
    /// 取消返回开始界面
    /// </summary>
    public void BackCancel()
    {
        Time.timeScale = 1;
        //解冻时间
        backPanel_2.SetActive(false);
        //关闭返回界面
    }

    /// <summary>
    /// 再来一波按钮
    /// </summary>
    public void Again()
    {
        totalFillAmount = -1f;


        transform.GetComponent<StartPanel>().randomQuestion(GlobalVariable.questionCount);

        //关闭结束界面
        transform.GetComponent<QuestingPanel>().endPanel.SetActive(false);
        //打开做题界面
        transform.GetComponent<QuestingPanel>().questingPanel.SetActive(true);
        //解冻时间
        Time.timeScale = 1;


        //正确率图片完整率重置为0
        imageScore.fillAmount = 0;
        //转动文本的位置设为初始位置
        textMove.GetComponent<RectTransform>().anchoredPosition3D = initTextPos;
        //重置数据
        transform.GetComponent<QuestingPanel>().ResetFactor();
        //重新开始做题
        transform.GetComponent<StartPanel>().StartChoose();
    }


}
