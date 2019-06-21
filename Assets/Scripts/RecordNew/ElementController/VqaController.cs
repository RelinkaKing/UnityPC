using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 微课问答控制器
/// </summary>
public class VqaController : BaseController
{
    //Dictionary<int, VqaElement> vqaDic;
    //当前问答文档对象
    public VqaElement currentVqa;
    //public int index;
    //问题物体
    public GameObject question;
    //确定面板
    public GameObject ensurePanel;
    //问题文本
    public Text questionText;
    //初始化颜色
    public static Color initColor = new Color(104f/255f,213f/255f,240f/255f);
    //正确颜色
    public static Color rightColor = new Color(55f/255f, 227f/255f, 70f/255f);
    //错误颜色
    public static Color errorColor = new Color(238f / 255f, 53f / 255f, 53f / 255f);
    //选择颜色
    public static Color selectColor = new Color(91f / 255f, 236f / 255f, 242f / 255f);
    //答案物体
    public GameObject answer;
    //答案物体集合
    List<GameObject> answers = new List<GameObject>();

    bool isEnd;
    //是单选题
    bool isSingleAnswer = true;
    //正确声音片段
    public AudioClip clipIsTrue;
    //错误声音片段
    public AudioClip clipIsFalse;
    //音源
    public AudioSource audioSc;
    //public void initVqaDic(VqaElement[] qas)
    //{
    //    vqaDic = initDic(qas);
    //}

    public override void Begin()
    {
        //base.Begin();
    }

    public override void Do(float slideTime)
    {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (!isEnd && currentVqa != null && currentVqa.appearTime <= slideTime)
            {
                initVqa();
            }

        }
        }

    public override void QuicklyComeIntoBowl(XmlElement element)
    {
        XmlElement currElement = (XmlElement)element.SelectSingleNode("Qas/Qa[@id='" + currentVqa.id + "']");
        if (currElement == null) {
            Debug.Log("currElement == NULL !!!!!!!!!!!!1");
            return;
        }
        if (currentVqa.appearTime >= currentVqa.endTime)
        {
            currentVqa.endTime = float.Parse(element.GetAttribute("totalTime"));
        }
        currElement.SetAttribute("appearTime", currentVqa.appearTime + "");
        currElement.SetAttribute("endTime", currentVqa.endTime + "");
        currElement.RemoveAttribute("shapeId");
    }

    public override void GoDie(float slideTime)
    {
        //base.GoDie();
        isEnd = true;
        endQa();
    }
    //问题基础面板
    public GameObject basePanel;
    public void hide() {
        basePanel.SetActive(false);
    }
    public void show() {
        basePanel.SetActive(true);
        transform.SetParent(topButtonPanel.transform,false);
        transform.SetAsFirstSibling();
        
        //tmpVqaObj = GameObject.Instantiate(this.gameObject, topButtonPanel.transform.parent);
        //tmpVqaObj.transform.SetAsFirstSibling();
    }
    //当前页对应物体
    Transform slide;
    //顶层面板
    GameObject topButtonPanel;
    //关闭问答面板
    public void closeVqa() {
        Debug.Log("closeVqa");
        isEnd = true;
        transform.SetParent(slide, false);
        transform.SetAsFirstSibling();
        //Destroy(tmpVqaObj);
        hide();
    }
    public override void Init()
    {
        base.Init();
        hide();
        isEnd = false;
        //index = 0;
        //if (vqaDic.ContainsKey(index)) {
        //    currentVqa = vqaDic[index];
        //}


        //loadVqa()
    }
    /// <summary>
    /// 结束问答
    /// </summary>
    public void endQa() {
        hide();
        foreach (GameObject obj in answers)
        {
            Destroy(obj);
        }
        answers.Clear();
        isEnd = true;
    }
    /// <summary>
    /// 初始化问答
    /// </summary>
    public void initVqa() {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            gameObject.SendMessageUpwards("PausePlaying");
        }
            endQa();
        
        if (loadVqa(currentVqa.fileName))
        {
            isEnd = false;
            ensurePanel.SetActive(false);
            selectedAnswer.Clear();
            selectedImg.Clear();
            rightAnswer.Clear();
            rightImg.Clear();

            InitRect(copy(currentVqa));
            questionText.text = currentVqa.vqa.content.Trim();
            char option = 'A';
            isSingleAnswer = true;
            rightImg.Clear();
            try
            {
                if (currentVqa.vqa.type == 1) {
                    if (currentVqa.vqa.answers.Count == 1)
                    {
                        //currentVqa.vqa.answers[0].content = currentVqa.vqa.answers[0].isAnswer;
                        Answer tmp = new Answer();
                        if (currentVqa.vqa.answers[0].isAnswer == "True") {
                            currentVqa.vqa.answers[0].content = "Yes";
                            tmp.content = "No";
                            tmp.isAnswer = "False";
                        }
                        else {
                            currentVqa.vqa.answers[0].content = "No";
                            currentVqa.vqa.answers[0].isAnswer = "True";
                            tmp.content = "Yes";
                            tmp.isAnswer = "False";
                        }
                        currentVqa.vqa.answers.Add(tmp);
                    }
                    else {
                        endQa();
                        return;
                    }
                }
            }
            catch {
                endQa();
                return;
            }
            int fontSize = questionText.resizeTextMaxSize -10;
            foreach (Answer tmpAnswer in currentVqa.vqa.answers) {
                GameObject tmpObj =  GameObject.Instantiate(answer,basePanel.transform);
                tmpObj.SetActive(true);
                tmpObj.name = tmpAnswer.isAnswer;
               
                tmpObj.transform.Find("AnswerText").GetComponentInChildren<Text>().text = tmpAnswer.content.Trim();
                tmpObj.transform.Find("AnswerText").GetComponentInChildren<Text>().resizeTextMaxSize = fontSize;
                tmpObj.transform.Find("optionText").GetComponentInChildren<Text>().text = option+"";
                tmpObj.transform.Find("optionText").GetComponentInChildren<Text>().resizeTextMaxSize = fontSize;
                option = (char)((int)option + 1);
                answers.Add(tmpObj);
                RawImage tmpImg = tmpObj.transform.Find("backImg").GetComponent<RawImage>();
                tmpImg.color = initColor;
                if (tmpAnswer.isAnswer == "True") {
                    rightAnswer.Add(tmpAnswer.content);
                    rightImg.Add(tmpImg);
                }
                
            }
            if (rightAnswer.Count >1) {
                isSingleAnswer = false;
                ensurePanel.SetActive(true);
                ensurePanel.transform.SetAsLastSibling();
            }
            show();
        }
        else {
            hide();
        }
       
        
        
    }
    public override void DoAnimation(PPTAnimation ani)
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (ani.shapeId != currentVqa.shapeId)
            {
                return;
            }
        }
        if (ani.action == PPTAction.entr)
        {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                currentVqa.appearTime = ani.slideTime;
            }
            initVqa();
            //player.Play();
            //image.texture = player.texture;
            //player.Pause();
        }
      if (ani.action == PPTAction.exit)
        {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                currentVqa.endTime = ani.slideTime;
            }
            GoDie(0);
        }
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            PPTController.isExecuted = true;
        }
    }
    /// <summary>
    /// 显示结果
    /// </summary>
    public void showResult() {
        isEnd = true;
        if (compareList())
        {
            //播放正确音效
            audioSc.PlayOneShot(clipIsTrue);
            changeListColor(selectedImg,rightColor);
            StartCoroutine(wait());

        }
        else {
            //播放错误音效
            audioSc.PlayOneShot(clipIsFalse);
            changeListColor(selectedImg, errorColor);
            changeListColor(rightImg, rightColor);
        }


    }
    /// <summary>
    /// 等1秒继续
    /// </summary>
    /// <returns></returns>
    public IEnumerator wait() {
        yield return new WaitForSecondsRealtime(1);
        try
        {
            GameObject.Find("ControllerCanvas").SendMessage("goOn");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    /// <summary>
    /// 改变集合中所有图片元素
    /// </summary>
    /// <param name="tmpList"></param>
    /// <param name="target"></param>
    public void changeListColor(List<RawImage> tmpList,Color target) {
        foreach (RawImage tmp in tmpList) {
            tmp.color = target;
        }
    }
    /// <summary>
    /// 比较正确答案和选择的答案
    /// </summary>
    /// <returns></returns>
    public bool compareList() {
        if (rightAnswer.Count != selectedAnswer.Count) {
            return false;
        }
        foreach (string answer in selectedAnswer) {
            if (!rightAnswer.Contains(answer)) {
                return false;
            }
        }
        return true;
    }
    //选择的答案
    List<string> selectedAnswer = new List<string>();
    //正确答案
    List<string> rightAnswer = new List<string>();
    //选择的图片集合
    List<RawImage> selectedImg = new List<RawImage>();
    //正确图片集合
    List<RawImage> rightImg = new List<RawImage>();
    //选择答案
    public void selectAnswer(GameObject obj)
    {
        if (isEnd) {
            return;
        }
        string answer = obj.GetComponentInChildren<Text>().text;
        RawImage tmpImage = obj.transform.Find("backImg").GetComponent<RawImage>();
        if (selectedAnswer.Contains(answer))
        {
            selectedAnswer.Remove(answer);
            selectedImg.Remove(tmpImage);
            tmpImage.color = initColor;
        }
        else {
            selectedImg.Add(tmpImage);
            selectedAnswer.Add(answer);
            tmpImage.color = selectColor;
        }
        if (isSingleAnswer) {
            showResult();
        }
    }
    public override void InitCompent(int pageNum)
    {
        this.pageNum = pageNum;
        slide = this.transform.parent;
        transform.SetAsFirstSibling();
        topButtonPanel = GameObject.Find("ControllerCanvas").GetComponent<PPTResourcePool>().VqaTopCanvas;
        audioSc = this.gameObject.AddComponent<AudioSource>();
    }

    public override void InitRect<T>(T t)
    {
        if (currentVqa.style == "default") {
            if (currentVqa.vqa.type == 1)
            {
                t.h =(t.h+25) * 3;
            }
            else {
                t.h = (t.h + 25) * (currentVqa.vqa.answers.Count + 1);
            }
        }
        base.InitRect(t);
         
    }

    public override void Pause()
    {
        //base.Pause();
    }
    /// <summary>
    /// 加载问题
    /// </summary>
    /// <param name="fileName">加载关联文件</param>
    /// <returns></returns>
    public bool loadVqa(string fileName) {
        string filePath = PPTGlobal.PPTPath + this.pageNum + "/" + fileName;
        if (!File.Exists(filePath))
        {
            string tmpName = fileName.Replace("SL_", "").Replace("YL_", "");
            filePath = PPTGlobal.PPTPath + this.pageNum + "/SL_" + tmpName;
            if (!File.Exists(filePath))
            {
                filePath = PPTGlobal.PPTPath + this.pageNum + "/YL_" + tmpName;
                if (!File.Exists(filePath))
                {
                    filePath = PPTGlobal.PPTPath + this.pageNum + "/" + tmpName;
                }
            }
        }

        if (File.Exists(filePath))
        {
            VqaRoot vqaRoot = PublicTools.Deserialize<VqaRoot>(filePath);
            currentVqa.vqa = vqaRoot.vqa;
            return true;
        }

        Debug.Log("loadVqa not exists:" + filePath);
        return false;
    }
}
