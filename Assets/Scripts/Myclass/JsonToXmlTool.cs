using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System;
/// <summary>
/// 拉取的【我的班级】提供的测试题 Json转【结构测验】可读取的xml
/// </summary>
public class JsonToXmlTool : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void getOption() {
        string filepath = PublicClass.filePath + GlobalVariable.bankName + ".json";
        filepath = filepath.Replace("QAXML", "QAJSON");
        if (!File.Exists(filepath)) {
            return;
        }
        string jsonData = File.ReadAllText(filepath);
        jsonTextModel jtm = JsonConvert.DeserializeObject<jsonTextModel>(jsonData);
        Dictionary<string, Dictionary<string, string>> options = new Dictionary<string, Dictionary<string, string>>();
        foreach (jsonLibModel tmpLib in jtm.data) {
            if (tmpLib.testpaperName == GlobalVariable.libName) {
                foreach (jsonQuestionModel tmpJqm in tmpLib.SubjectData) {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    if(tmpJqm.option_a!=null)
                        dic.Add(tmpJqm.option_a.Trim(),"A");
                    if (tmpJqm.option_b != null)
                        dic.Add(tmpJqm.option_b.Trim(), "B");
                    if (tmpJqm.option_c != null)
                        dic.Add(tmpJqm.option_c.Trim(), "C");
                    if (tmpJqm.option_d != null)
                        dic.Add(tmpJqm.option_d.Trim(), "D");
                    if (tmpJqm.option_e != null)
                        dic.Add(tmpJqm.option_e.Trim(), "E");
                    options.Add(tmpJqm.id,dic);
                }
            }
        }
        
        //Debug.Log(JsonConvert.SerializeObject(QuestingPanel.selecteds));
        Selected[] tmpSelecteds = QuestingPanel.selecteds;
        for (int i =0;i<tmpSelecteds.Length;i++) {
            if (tmpSelecteds[i].selectContent == "Z")
            {
                tmpSelecteds[i].option = "Z";
            }
            else {
                try
                {
                    //单选
                    if (!tmpSelecteds[i].selectContent.Contains("-"))
                    {
                        tmpSelecteds[i].option = options[tmpSelecteds[i].qid][tmpSelecteds[i].selectContent];
                    }
                    else {//多选
                        string[] result = tmpSelecteds[i].selectContent.Split('-');
                        for (int m =0;m<result.Length;m++) {
                            if (result[m].Trim() !="") {
                                tmpSelecteds[m].option += options[tmpSelecteds[m].qid][result[m].Trim()];
                            }
                            
                        }
                    }
                }
                catch (Exception e){
                    Debug.Log(e.StackTrace);
                    tmpSelecteds[i].option = "Z";
                }
            }
        }

        QuestingPanel.selecteds = tmpSelecteds;
    }
    /// <summary>
    /// Json转xml
    /// </summary>
    /// <param name="classId">班级id</param>
    /// <param name="className">班级名称</param>
    /// <param name="isError">是错题集</param>
    public static void JsonToXml(int classId,string className,string isError = "") {
        string filepath = PublicClass.filePath + "classId_" + classId + isError + "_QAJSON.json";
        string xmlpath = PublicClass.filePath + "classId_" + classId + isError + "_QAXML.xml";
        //FileStream fs = new FileStream(filepath,FileMode.Open,FileAccess.Read);
        if (!File.Exists(filepath)) {
            return;
        }
        string jsonData = File.ReadAllText(filepath);
        jsonTextModel jtm = JsonConvert.DeserializeObject<jsonTextModel>(jsonData);
        //Debug.Log(jtm.msg);
        //Debug.Log(jtm.result);
        //Debug.Log(jtm.data.Length);
        if (jtm.data.Length == 0) {
            return;
        }
        //JsonConvert.DeserializeXmlNode();
        //Json字符串格式化
        xmlRootModel xr = new xmlRootModel();
        xmlDocumentModel xdm = new xmlDocumentModel();
        xdm.name = className;
        List<xmlLibModel> library = new List<xmlLibModel>();
        xr.document = xdm;
        xdm.library = library;
        int i = 0;
        List<string> errorList,rightList;
        foreach (jsonLibModel tmpJl in jtm.data) {
            xmlLibModel tmpXlm= new xmlLibModel();
            tmpXlm.Question = new List<Question>();
            tmpXlm.id = tmpJl.testpaperId+"";
            tmpXlm.name = tmpJl.testpaperName;
            tmpXlm.score = tmpJl.score == null? "100":tmpJl.score;
            tmpXlm.startTime = tmpJl.startTime;
            tmpXlm.closeTime = tmpJl.closeTime;
            errorList = new List<string>();
            rightList = new List<string>();
            foreach (jsonQuestionModel tmpQues in tmpJl.SubjectData)
            {
                Question tmpQuestion = new Question();
                tmpQues.getList(out errorList,out rightList);
                tmpQuestion.qid = tmpQues.id;
                tmpQuestion.q = tmpQues.subject;
                tmpQuestion.questionType = Question.QuestionType.text;
                tmpQuestion.grade = "1";
                tmpQuestion.rightAnswers = rightList;
                tmpQuestion.erroranswers = errorList;
                tmpXlm.Question.Add(tmpQuestion);
            }

            library.Add(tmpXlm);
        }

        //xr.root = jtm;

        //XmlDocument xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(xr));

        //xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
        //xml.Save(xmlpath);

        try { 
            FileStream fs = null;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer xs = new XmlSerializer(typeof(xmlDocumentModel));
            if (File.Exists(xmlpath)) {
                File.Delete(xmlpath);
            }
            fs = new FileStream(xmlpath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs,Encoding.UTF8);
            xs.Serialize(sw,xdm,ns);
            fs.Flush();
            sw.Close();
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            Debug.Log(xmlpath + "--------error");
            if (File.Exists(xmlpath))
            {
                File.Delete(xmlpath);
            }
            
        }

    }

    /// <summary>
    /// Json格式问题基类
    /// </summary>
    public class jsonQuestionModel
    {
        //五个选项
        public string option_a, option_b, option_d, option_c, option_e;
        public string id;
        public string subject;
        public string correct;
        public string type = "text";
        public void getList(out List<string> errorList, out List<string> rightList) {
            errorList = new List<string>();
            rightList = new List<string>();
            List<string> tmpList = new List<string>();
            char[] tmpChars = correct.ToLower().ToCharArray();
            foreach (char c in tmpChars) {
                tmpList.Add("option_" + c);
            }
            if (tmpList.Contains("option_a"))
            {
                rightList.Add(option_a);
            }
            else {
                errorList.Add(option_a);
            }
            if (tmpList.Contains("option_b"))
            {
                rightList.Add(option_b);
            }
            else
            {
                errorList.Add(option_b);
            }
            if (tmpList.Contains("option_c"))
            {
                rightList.Add(option_c);
            }
            else
            {
                errorList.Add(option_c);
            }
            if (tmpList.Contains("option_d"))
            {
                rightList.Add(option_d);
            }
            else
            {
                errorList.Add(option_d);
            }
            if (tmpList.Contains("option_e"))
            {
                rightList.Add(option_e);
            }
            else
            {
                errorList.Add(option_e);
            }
        }
        
    }
    /// <summary>
    /// Json格式题库文档
    /// </summary>
    public class jsonTextModel {
        public string msg;
        public bool result;
        public jsonLibModel[] data;

    }
    /// <summary>
    /// Json格式题库文档中包含的科目
    /// </summary>
    public class jsonLibModel {
        public jsonQuestionModel[] SubjectData;
        public string startTime = string.Empty;
        public string closeTime = string.Empty;
        public string testpaperName;
        public string testpaperId;
        public string score;
    }
    /// <summary>
    /// Xml根节点
    /// </summary>
    public class xmlRootModel
    {
        public xmlDocumentModel document;
    }
    /// <summary>
    /// Xml文档对应类
    /// </summary>
    [XmlRoot("document")]
    public class xmlDocumentModel
    {
        [XmlAttribute("name")]
        public string name;
        [XmlElement]
        public List<xmlLibModel> library;
    }
    /// <summary>
    /// Xml科目对应类
    /// </summary>
    public class xmlLibModel
    {
        [XmlAttribute("startTime")]
        public string startTime { get; set; }
        [XmlAttribute("closeTime")]
        public string closeTime { get; set; }
        [XmlAttribute("score")]
        public string score { get; set; }
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlElement("Question")]
        public List<Question> Question;
    }
 }
