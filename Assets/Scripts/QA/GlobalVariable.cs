//公共参数
using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using VesalDecrypt;
using System.Xml.Serialization;

/// <summary>
/// 常量公用参数
/// </summary>
public class GlobalVariable
{
    //答题场景状态预定义
    public static PracticeState practiceState = PracticeState.normal;
    //总分
    public static float score = 100;
    //一波问题数量
    public static int questionCount = 10;
    //错题重现系数
    public static float errorReappear = 0.3f;
    //开启音效、倒计时按钮后方块的颜色
    public static Color colorOn = new Color(97.0f / 255.0f, 135.0f / 255.0f, 156.0f / 255.0f);
    //打开音效、倒计时按钮后方块的位置
    public static Vector3 buttonPosition0 = new Vector3(58, 0, 0);
    //关闭音效、倒计时按钮后方块的位置
    public static Vector3 buttonPosition1 = new Vector3(-58, 0, 0);
    //缺省404图片路径
    public static string missingPicPath = PublicClass.filePath + "404.PNG";
    //题库名，初始值为题库xml的文件名，即bankName.xml
    public static string bankName = "bankName";
    //科目名
    public static string libName;
    //是否只答题
    //public static bool isQaOnly = false;
    //public static parameterTransmit pt = null;
    //模型高亮色（弃用）
    public static Color ModelHighLightColor = new Color(0,255,0);
    public static Color SignHighLightColor = new Color(0, 130, 255);
    //问题xml设定的高亮色（弃用）
    public static Color tmpHighLightColor = Color.white;
}
////微课或网页端传递消息中间类
//public class parameterTransmit
//{
//    //问答系统初始化时，读取为false销毁此实例
//    public bool isFirstRead = true;
//    //只使用答题功能
//    public bool isQaOnly = true;
//    //题库文件名
//    public string bankName { get; set; }
//    //科目名
//    public string libName { get; set; }
//    //是否竖屏
//    public bool isPortrait = false;
//    //微课答题屏幕上方显示文字
//    //public string[] wkTexts;
//}

/// <summary>
/// 变量公用参数
/// </summary>
public class ScenceData
{
    //试卷时间信息
    public static string testPaperTimeInfo = string.Empty;
    //测试用下载字典
    //public static Dictionary<string, string> downloadDic = new Dictionary<string, string>();
    //记录一波次错题
    public static List<Question> errorQts = new List<Question>();
    //储存模型（旧模型加载方法弃用）
    public static Dictionary<string, GameObject> models = new Dictionary<string, GameObject>();
    //public static int maxLoadModels = 20;
    //加载AssetBundle
    //（旧模型加载方法弃用）
    static Dictionary<string, AssetBundle> absDic = new Dictionary<string, AssetBundle>();
    //（旧模型加载方法弃用）
    public static void clear()
    {
        foreach (GameObject gm in models.Values)
        {
            try
            {
                GameObject.DestroyImmediate(gm);
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                Debug.Log(e.Message);
                continue;
            }
        }

        foreach (AssetBundle ab in absDic.Values)
        {
            try
            {
                ab.Unload(true);
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                Debug.Log(e.Message);
                continue;
            }
        }
    }
    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="filepath"></param>
    public static void mkdir(string filepath)
    {
        string direName = Path.GetDirectoryName(filepath);
        if (!Directory.Exists(direName))
        {
            Directory.CreateDirectory(direName);
        }
    }
    /// <summary>
    /// 获得Ab包路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string getFilePath(string fileName)
    {
        return PublicClass.filePath + fileName + ".assetbundle";
    }


    //public static void loadMultipleModel(string abName, string modelName) {
    //    abName = abName.Replace("：",":");
    //    modelName = modelName.Replace("：", ":");
    //    string[] abs = abName.Split(':');
    //    string[] names = modelName.Split(':');

    //    Debug.Log(modelName);
    //    Debug.Log(abName);
    //    Debug.Log(modelName);
    //    Debug.Log(abs.Length);
    //    Debug.Log(names.Length);
    //    if (abs.Length != names.Length || abs.Length<1) {
    //        return;
    //    }
    //    Debug.Log(modelName);
    //    GameObject target = null;
    //    try
    //    {
    //        for (int i = 0; i < abs.Length; i++)
    //        {
    //            if (!absDic.ContainsKey(abs[i]))
    //            {
    //                loadModel(abs[i], names[i], false,true);
    //            }
    //            GameObject tmpGm = GameObject.Instantiate(absDic[abs[i]].LoadAsset(names[i])) as GameObject;
    //            if (tmpGm == null)
    //            {
    //                GameObject.Destroy(tmpGm);
    //                GameObject.Destroy(target);
    //                return;
    //            }

    //            if (i == 0)
    //            {
    //                target = tmpGm;
    //            }
    //            else {
    //                tmpGm.transform.SetParent(target.transform);
    //            }

    //        }
    //    }
    //    catch (Exception e){
    //        Debug.Log(e.Message);
    //        Debug.Log(e.StackTrace);
    //    }
    //    if (target == null)
    //    {
    //        Debug.Log("target == null");
    //    }
    //    else {
    //        models.Add(modelName, target);
    //        models[modelName].SetActive(false);
    //    }


    //}
    ///// <summary>
    ///// 加载模型
    ///// </summary>
    ///// <param name="abName"></param>
    ///// <param name="modelName"></param>
    //public static void loadModel(string abName, string modelName, bool isSign,bool isMultiple=false)
    //{
    //    if (!models.ContainsKey(modelName))
    //    {
    //        ///Debug.Log(modelName);
    //        if (modelName.Contains(":") || modelName.Contains("："))
    //        {
    //            loadMultipleModel(abName,modelName);
    //            return;
    //        }
    //        if (!absDic.ContainsKey(abName))
    //        {
    //            string filepath = getFilePath(abName);
    //            try
    //            {
    //                if (!File.Exists(filepath)) {
    //                    Debug.Log("!File.Exists");
    //                    return;
    //                }
    //                if (!isSign)
    //                {
    //                    encrypt.DecryptFile(filepath, filepath + "vesal.temp", "Vesal17788051918");
    //                    filepath += "vesal.temp";
    //                }
    //                if (!File.Exists(filepath))
    //                {
    //                    Debug.Log("!File.Exists");
    //                    return;
    //                }
    //                Debug.Log(filepath + "--" + abName);
    //                AssetBundle abs = AssetBundle.LoadFromMemory(File.ReadAllBytes(filepath));
    //                absDic.Add(abName, abs);

    //            }
    //            catch (SystemException e)
    //            {
    //                Debug.Log(e.StackTrace);
    //                Debug.Log(e.Message);
    //            }
    //            if (!isSign)
    //            {
    //                File.Delete(filepath);
    //            }
    //        }
    //        try
    //        {

    //            GameObject target = GameObject.Instantiate(absDic[abName].LoadAsset(modelName)) as GameObject;


    //        }
    //        catch (SystemException e)
    //        {
    //            Debug.Log(e.StackTrace);
    //            Debug.Log(e.Message);
    //        }
    //    }
    //    //if (models.Count > maxLoadModels) {
    //    //    Warning("加载模型超过"+maxLoadModels+"个");
    //    //}


    //}

    //储存N个问题
    public static Question[] questions;
    //结果列表索引
    public static int listIndex = 0;
    //场景状态
    public static ScenceState currentState;

    //倒计时是否结束
    public static bool isTimeOver = false;

    //答题总时间
    public static long useTime;
    //问题集问题总数
    public static int questionTotal;
    //得分
    public static float score = 0;
    //当前问题正确答案对应 textInButton 位置
    public static int rightIndex = 0;

    //声音和计时
    public static bool isOpenSound;
    public static bool isOpenTime;
    //测试界面中按钮的初始颜色
    public static Color initColor = new Color(97.0f / 255.0f, 135.0f / 255.0f, 156.0f / 255.0f);
    //测试中答对后按钮的颜色
    public static Color buttonIsTrue = new Color(43.0f / 255.0f, 192.0f / 255.0f, 189.0f / 255.0f, 1);
    //测试中答错后按钮的颜色
    public static Color buttonIsFalse = new Color(242.0f / 255.0f, 81.0f / 255.0f, 92.0f / 255.0f, 1);
    //正确结果列表的颜色
    public static Color listIsTrue = new Color(79 / 255.0f, 186 / 255.0f, 218 / 255.0f, 1);
    //错误结果列表的颜色 
    public static Color listIsFalse = new Color(236 / 255.0f, 115 / 255.0f, 127 / 255.0f, 1);
    //字体大小
    public static int fontSize = 45;
    //测试界面按钮文本字体标准尺寸
    public static int shrinkedFontsize = 33;
    //限定列表长度
    public static int imageListTextCharNum = 24;



}
/// <summary>
/// 场景状态
/// </summary>
public enum ScenceState
{
    //准备状态
    prepare,
    //载入题目状态
    loadQuestion,
    //测试状态
    exam,
    //总结状态
    summary,
    //结束状态
    end,
    //停止状态
    stop,
    //重新开始状态
    rePrepare
}

/// <summary>
/// 答题状态
/// </summary>
public enum PracticeState
{
    //微课状态
    weike,
    //班课状态
    classes,
    //一般练习状态
    normal,
    //错题本状态
    errors
}
/// <summary>
/// 测验问题类
/// </summary>
public class Question
{
    [XmlIgnore]
    //科目id
    public static string LibId;
    //问题类型
    public enum QuestionType
    {
        text,
        movie,
        picture,
        model,
        //图钉模式
        thumbtack
    }
    [XmlAttribute("qid")]
    //问题id
    public string qid;
    [XmlIgnore]
    //答案集合
    public List<string> answers = new List<string>();
    [XmlElement("error")]
    //错误答案集合
    public List<string> erroranswers = new List<string>();
    //问题文本
    public string q;
    [XmlElement("right")]
    //正确答案集合
    public List<string> rightAnswers = new List<string>();
    //public string highLightName;
    [XmlIgnore]
    //选择模型字典，true高亮
    public Dictionary<String, bool> selectModelDic;
    [XmlIgnore]
    //名词编号
    public string nounNo;
    //ab包名称
    public string abName;
    //模型名称
    public string modelName;
    //关联链接
    public string url;
    //模型题提示图片
    public string modelTipPic;
    //public string questionType;
    //问题等级
    public string grade = "1";
    //问题类型
    public QuestionType questionType;


    [XmlIgnore]
    //相机参数
    public cameraParameter cp;
    /// <summary>
    /// 初始化cameraParameter （预留参数）
    /// </summary>
    public class cameraParameter
    {
        //float positionX;
        //float positionY;
        //float positionZ;
        //float RotationX;
        //float RotationY;
        //float RotationZ;
        //float distance;
        float minDistance;
        float maxDistance;
        public Vector3 boxRotation;
        public Vector3 localPos;
        public Color highLightColor;
        Vector3 convertToV3(string data) {
            if (data == string.Empty || data == null) {
                return Vector3.zero;
            }
            data = data.Replace("(","");
            data = data.Replace(")", "");
            string[] datas = data.Split(',');
            Vector3 result = Vector3.zero;
            try {
                result = new Vector3(float.Parse(datas[0]), float.Parse(datas[1]), float.Parse(datas[2]));
            }
            catch (Exception e) {
                Debug.Log(e.StackTrace);
            }
            return result;
        }
        Color convertToColor(string data)
        {
            if (data == string.Empty || data == null)
            {
                return Color.white;
            }
            data = data.Replace("(", "");
            data = data.Replace(")", "");
            string[] datas = data.Split(',');

            Color result = Color.white;
            try
            {
                result = new Color(float.Parse(datas[0]), float.Parse(datas[1]), float.Parse(datas[2]), float.Parse(datas[3]));
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
            }
            return result;
        }
        public cameraParameter(XmlElement root)
        {
            boxRotation = convertToV3(root.GetAttribute("boxRotation"));
            localPos = convertToV3(root.GetAttribute("localPos"));

            
            
            //this.positionX = float.Parse(root.GetAttribute("cameraBackPosX"));
            //this.positionY = float.Parse(root.GetAttribute("cameraBackPosY"));
            //this.positionZ = float.Parse(root.GetAttribute("cameraBackPosZ"));
            //this.RotationX = float.Parse(root.GetAttribute("cameraBackRotX"));
            //this.RotationY = float.Parse(root.GetAttribute("cameraBackRotY"));
            //this.RotationZ = float.Parse(root.GetAttribute("cameraBackRotZ"));
            //this.distance = float.Parse(root.GetAttribute("cameraDistance"));
            this.minDistance = float.Parse(root.GetAttribute("cameraMinDis"));
            this.maxDistance = float.Parse(root.GetAttribute("cameraMaxDis"));
            this.highLightColor = convertToColor(root.GetAttribute("highLightColor"));
        }
        public void initCamera()
        {
            //XT_MouseControl.instance.positionX = this.positionX;
            //XT_MouseControl.instance.positionY = this.positionY;
            //XT_MouseControl.instance.positionZ = this.positionZ;
            //XT_MouseControl.instance.RotationX = this.RotationX;
            //XT_MouseControl.instance.RotationY = this.RotationY;
            //XT_MouseControl.instance.RotationZ = this.RotationZ;
            //XT_MouseControl.instance.x1 = XT_MouseControl.instance.positionX;
            //XT_MouseControl.instance.y1 = XT_MouseControl.instance.positionY;
            //transform.GetComponent<ModelControl>().initCamPos = new Vector3(positionX, positionY, positionZ);
            //transform.GetComponent<ModelControl>().initCamRot = new Vector3(RotationX, RotationY, RotationZ);


            //XT_MouseControl.instance.distance = -70;//this.distance;
            //XT_MouseControl.instance.minDistance = this.minDistance;
            //XT_MouseControl.instance.maxDistance = this.maxDistance;
            //XT_MouseControl.instance.resetDistance = XT_MouseControl.instance.distance;
            //XT_MouseControl.instance.Reset();
        }
    }
    /// <summary>
    /// 获得问题类型
    /// </summary>
    /// <param name="tmp"></param>
    /// <returns></returns>
    public QuestionType getType(string tmp)
    {
        switch (tmp)
        {
            case "movie": return QuestionType.movie;
            case "picture": return QuestionType.picture;
            case "model": return QuestionType.model;
            case "thumbtack": return QuestionType.thumbtack;
        }
        return QuestionType.text;
    }
    public Question()
    {
    }
    /// <summary>
    /// 初始化问题
    /// </summary>
    /// <param name="xe">Question节点</param>
    public Question(XmlElement xe)
    {
        this.qid = xe.GetAttribute("qid");
        this.url = xe["url"] == null ? "null" : xe["url"].InnerText;

        string tmp = xe["questionType"] == null ? "text" : xe["questionType"].InnerText;
        this.questionType = getType(tmp);
        this.q = xe["q"] == null ? "null" : xe["q"].InnerText;
        this.modelName = xe["modelName"] == null ? "null" : xe["modelName"].InnerText;

        if (this.modelName != "null")
        {
            this.modelTipPic = xe["modelName"].HasAttribute("modelTipPic") ? xe["modelName"].GetAttribute("modelTipPic") : "null";

            this.abName = xe["modelName"].GetAttribute("assetbundle");
            this.nounNo = xe["modelName"].GetAttribute("nounNo");
            //this.highLightName = xe["highLightName"] == null ? "null" : xe["highLightName"].InnerText;
            string jsonStr = xe["highLightName"] == null ? "null" : xe["highLightName"].InnerText;
            if (jsonStr != "null")
            {
                selectModelDic = JsonUtility.FromJson<Serialization<String, bool>>(jsonStr).ToDictionary();
            }

        }

        this.grade = xe["grade"] == null ? "1" : xe["grade"].InnerText;
        if (xe["cameraParameter"] != null)
        {
            this.cp = new cameraParameter(xe["cameraParameter"]);
        }
        for (int i = 1; ; i++)
        {
            try
            {
                addRightAnswer(xe["right" + i].InnerText);
                addAnswer(xe["right" + i].InnerText);
            }
            catch
            {
                break;
            }
        }
        foreach (XmlElement xel in xe.GetElementsByTagName("right"))
        {
            addRightAnswer(xel.InnerText.Trim());
            addAnswer(xel.InnerText.Trim());
        }
        foreach (XmlElement xel in xe.GetElementsByTagName("error"))
        {
            addAnswer(xel.InnerText.Trim());
        }
        if (this.rightAnswers.Count == 0)
        {
            Debug.Log("问题" + xe.GetAttribute("qid") + "无正确答案");
        }
        for (int i = 1; ; i++)
        {
            try
            {
                addAnswer(xe["a" + i].InnerText);
            }
            catch
            {
                break;
            }
        }
    }

    /// <summary>
    /// 添加所有答案
    /// </summary>
    /// <param name="s"></param>
    public void addAnswer(string s)
    {
        if (s != "null")
        {
            this.answers.Add(s);
        }
    }
    /// <summary>
    /// 添加正确答案
    /// </summary>
    /// <param name="s"></param>
    public void addRightAnswer(string s)
    {
        if (s != "null")
        {
            this.rightAnswers.Add(s);
        }
    }
    /// <summary>
    /// 打乱答案顺序
    /// </summary>
    /// <returns></returns>
    public List<string> getRadomAnswer()
    {
        int index = 0;
        string temp;
        for (int i = 0; i < answers.Count; i++)
        {
            index = UnityEngine.Random.Range(0, answers.Count - 1);
            if (index != i)
            {
                temp = answers[i];
                answers[i] = answers[index];
                answers[index] = temp;
            }
        }
        return answers;
    }
}

/// <summary>
/// 序列化字典
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
[Serializable]
public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;

    Dictionary<TKey, TValue> target;
    public Dictionary<TKey, TValue> ToDictionary() { return target; }

    public Serialization(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }
    
    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            if (target.ContainsKey(keys[i])) {
                Debug.Log("<color=red>"+ keys[i] + "</color>");
                continue;
            }
            target.Add(keys[i], values[i]);
        }
    }
}

