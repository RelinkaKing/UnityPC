//��������
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
/// �������ò���
/// </summary>
public class GlobalVariable
{
    //���ⳡ��״̬Ԥ����
    public static PracticeState practiceState = PracticeState.normal;
    //�ܷ�
    public static float score = 100;
    //һ����������
    public static int questionCount = 10;
    //��������ϵ��
    public static float errorReappear = 0.3f;
    //������Ч������ʱ��ť�󷽿����ɫ
    public static Color colorOn = new Color(97.0f / 255.0f, 135.0f / 255.0f, 156.0f / 255.0f);
    //����Ч������ʱ��ť�󷽿��λ��
    public static Vector3 buttonPosition0 = new Vector3(58, 0, 0);
    //�ر���Ч������ʱ��ť�󷽿��λ��
    public static Vector3 buttonPosition1 = new Vector3(-58, 0, 0);
    //ȱʡ404ͼƬ·��
    public static string missingPicPath = PublicClass.filePath + "404.PNG";
    //���������ʼֵΪ���xml���ļ�������bankName.xml
    public static string bankName = "bankName";
    //��Ŀ��
    public static string libName;
    //�Ƿ�ֻ����
    //public static bool isQaOnly = false;
    //public static parameterTransmit pt = null;
    //ģ�͸���ɫ�����ã�
    public static Color ModelHighLightColor = new Color(0,255,0);
    public static Color SignHighLightColor = new Color(0, 130, 255);
    //����xml�趨�ĸ���ɫ�����ã�
    public static Color tmpHighLightColor = Color.white;
}
////΢�λ���ҳ�˴�����Ϣ�м���
//public class parameterTransmit
//{
//    //�ʴ�ϵͳ��ʼ��ʱ����ȡΪfalse���ٴ�ʵ��
//    public bool isFirstRead = true;
//    //ֻʹ�ô��⹦��
//    public bool isQaOnly = true;
//    //����ļ���
//    public string bankName { get; set; }
//    //��Ŀ��
//    public string libName { get; set; }
//    //�Ƿ�����
//    public bool isPortrait = false;
//    //΢�δ�����Ļ�Ϸ���ʾ����
//    //public string[] wkTexts;
//}

/// <summary>
/// �������ò���
/// </summary>
public class ScenceData
{
    //�Ծ�ʱ����Ϣ
    public static string testPaperTimeInfo = string.Empty;
    //�����������ֵ�
    //public static Dictionary<string, string> downloadDic = new Dictionary<string, string>();
    //��¼һ���δ���
    public static List<Question> errorQts = new List<Question>();
    //����ģ�ͣ���ģ�ͼ��ط������ã�
    public static Dictionary<string, GameObject> models = new Dictionary<string, GameObject>();
    //public static int maxLoadModels = 20;
    //����AssetBundle
    //����ģ�ͼ��ط������ã�
    static Dictionary<string, AssetBundle> absDic = new Dictionary<string, AssetBundle>();
    //����ģ�ͼ��ط������ã�
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
    /// ����Ŀ¼
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
    /// ���Ab��·��
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string getFilePath(string fileName)
    {
        return PublicClass.filePath + fileName + ".assetbundle";
    }


    //public static void loadMultipleModel(string abName, string modelName) {
    //    abName = abName.Replace("��",":");
    //    modelName = modelName.Replace("��", ":");
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
    ///// ����ģ��
    ///// </summary>
    ///// <param name="abName"></param>
    ///// <param name="modelName"></param>
    //public static void loadModel(string abName, string modelName, bool isSign,bool isMultiple=false)
    //{
    //    if (!models.ContainsKey(modelName))
    //    {
    //        ///Debug.Log(modelName);
    //        if (modelName.Contains(":") || modelName.Contains("��"))
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
    //    //    Warning("����ģ�ͳ���"+maxLoadModels+"��");
    //    //}


    //}

    //����N������
    public static Question[] questions;
    //����б�����
    public static int listIndex = 0;
    //����״̬
    public static ScenceState currentState;

    //����ʱ�Ƿ����
    public static bool isTimeOver = false;

    //������ʱ��
    public static long useTime;
    //���⼯��������
    public static int questionTotal;
    //�÷�
    public static float score = 0;
    //��ǰ������ȷ�𰸶�Ӧ textInButton λ��
    public static int rightIndex = 0;

    //�����ͼ�ʱ
    public static bool isOpenSound;
    public static bool isOpenTime;
    //���Խ����а�ť�ĳ�ʼ��ɫ
    public static Color initColor = new Color(97.0f / 255.0f, 135.0f / 255.0f, 156.0f / 255.0f);
    //�����д�Ժ�ť����ɫ
    public static Color buttonIsTrue = new Color(43.0f / 255.0f, 192.0f / 255.0f, 189.0f / 255.0f, 1);
    //�����д���ť����ɫ
    public static Color buttonIsFalse = new Color(242.0f / 255.0f, 81.0f / 255.0f, 92.0f / 255.0f, 1);
    //��ȷ����б����ɫ
    public static Color listIsTrue = new Color(79 / 255.0f, 186 / 255.0f, 218 / 255.0f, 1);
    //�������б����ɫ 
    public static Color listIsFalse = new Color(236 / 255.0f, 115 / 255.0f, 127 / 255.0f, 1);
    //�����С
    public static int fontSize = 45;
    //���Խ��水ť�ı������׼�ߴ�
    public static int shrinkedFontsize = 33;
    //�޶��б���
    public static int imageListTextCharNum = 24;



}
/// <summary>
/// ����״̬
/// </summary>
public enum ScenceState
{
    //׼��״̬
    prepare,
    //������Ŀ״̬
    loadQuestion,
    //����״̬
    exam,
    //�ܽ�״̬
    summary,
    //����״̬
    end,
    //ֹͣ״̬
    stop,
    //���¿�ʼ״̬
    rePrepare
}

/// <summary>
/// ����״̬
/// </summary>
public enum PracticeState
{
    //΢��״̬
    weike,
    //���״̬
    classes,
    //һ����ϰ״̬
    normal,
    //���Ȿ״̬
    errors
}
/// <summary>
/// ����������
/// </summary>
public class Question
{
    [XmlIgnore]
    //��Ŀid
    public static string LibId;
    //��������
    public enum QuestionType
    {
        text,
        movie,
        picture,
        model,
        //ͼ��ģʽ
        thumbtack
    }
    [XmlAttribute("qid")]
    //����id
    public string qid;
    [XmlIgnore]
    //�𰸼���
    public List<string> answers = new List<string>();
    [XmlElement("error")]
    //����𰸼���
    public List<string> erroranswers = new List<string>();
    //�����ı�
    public string q;
    [XmlElement("right")]
    //��ȷ�𰸼���
    public List<string> rightAnswers = new List<string>();
    //public string highLightName;
    [XmlIgnore]
    //ѡ��ģ���ֵ䣬true����
    public Dictionary<String, bool> selectModelDic;
    [XmlIgnore]
    //���ʱ��
    public string nounNo;
    //ab������
    public string abName;
    //ģ������
    public string modelName;
    //��������
    public string url;
    //ģ������ʾͼƬ
    public string modelTipPic;
    //public string questionType;
    //����ȼ�
    public string grade = "1";
    //��������
    public QuestionType questionType;


    [XmlIgnore]
    //�������
    public cameraParameter cp;
    /// <summary>
    /// ��ʼ��cameraParameter ��Ԥ��������
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
    /// �����������
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
    /// ��ʼ������
    /// </summary>
    /// <param name="xe">Question�ڵ�</param>
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
            Debug.Log("����" + xe.GetAttribute("qid") + "����ȷ��");
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
    /// ������д�
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
    /// �����ȷ��
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
    /// ���Ҵ�˳��
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
/// ���л��ֵ�
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

