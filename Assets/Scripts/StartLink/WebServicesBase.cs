using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class WebServicesBase : IWebServicesBase
{

    //服务器根目录
    //http://localhost:8080/studentClass/isStudentExists?no=821453366
    public static string baseUrl = "http://47.105.38.68/oj/";

    public virtual UnityWebRequest Get() { return null; }
    public virtual UnityWebRequest Post() { return null; }
    public virtual UnityWebRequest PostWithParams(params KeyValue[] postParams)
    {
        return PostWithParams(servelets.none,postParams);
    }
    public virtual UnityWebRequest PostWithParams(servelets servelet, params KeyValue[] postParams) {
        string url = baseUrl + GetEnumDescription(servelet)+ "?";
        
        KeyValue kv;
        for (int i = 0;i<postParams.Length;i++) {
            kv = postParams[i];
            if (i!=0) {
                url += "&&";
            }
            url += kv.key + "=" + kv.value;
        }
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //加时间戳
        url += "&&timeFlag=" + Convert.ToInt64(ts.TotalMilliseconds); ;
        Debug.Log("request:"+url);
        UnityWebRequest uw = new UnityWebRequest(url);
        uw.method = "POST";
        uw.downloadHandler = new DownloadHandlerTexture();
        uw.timeout = 20;
        return uw;
    }
    //DownloadHandler downloadFile(string filePath);
    /// <summary>  
    /// 获取枚举类子项描述信息（注意：使用DescriptionAttribute这个类需要引入using System.ComponentModel;这个命名空间，别引错了。）  
    /// </summary>  
    /// <param name="enumSubitem">枚举类子项</param>          
    public static string GetEnumDescription(Enum enumSubitem)
    {
        string strValue = enumSubitem.ToString();

        FieldInfo fieldinfo = enumSubitem.GetType().GetField(strValue);
        System.Object[] objs = fieldinfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (objs == null || objs.Length == 0)
        {
            return strValue;
        }
        else
        {
            DescriptionAttribute da = (DescriptionAttribute)objs[0];
            return da.Description;
        }

    }
    public static List<int>  addedClassId = new List<int>();
    public static Dictionary<int, string> classnameDic = new Dictionary<int, string>();
    /// <summary>
    /// 获得所有题库json信息
    /// </summary>
    /// <returns></returns>
    public IEnumerator getAllBankJson()
    {
        //当网络不可用时              
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("网络不可用！");
        }
        else
        {

            foreach (int tmpId in addedClassId)
            {
                #region 下载试题
                KeyValue kv = new KeyValue("classesId", tmpId + "");
                UnityWebRequest uw = this.PostWithParams(servelets.studentTestpaperTitle, kv);
                yield return uw.Send();
                Debug.Log(uw.downloadHandler.text);
                Debug.Log(uw.isNetworkError);
                string filePath = PublicClass.filePath + "classId_" + tmpId + "_QAJSON.json";
                Debug.Log("getAllBankJson:"+filePath);
                if (!uw.isNetworkError)
                {
                    if (uw.downloadHandler.text.Contains("查询失败")) {
                        continue;
                    }
                    try
                    {

                        Debug.Log(filePath);
                        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        //Json字符串格式化
                        string jsondata = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(uw.downloadHandler.text), Formatting.Indented);
                        byte[] b = System.Text.Encoding.UTF8.GetBytes(jsondata);
                        fs.Write(b, 0, b.Length);
                        fs.Flush();
                        fs.Close();
                        JsonToXmlTool.JsonToXml(tmpId, classnameDic[tmpId]);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        Debug.Log(e.StackTrace);
                        Debug.Log(tmpId + "--------error");
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                    
                }
                else
                {
                    continue;
                }
                #endregion

                #region 下载错题
                //if (!File.Exists(PublicClass.filePath + "classId_" + tmpId + "_QAJSON.xml"))
                //{
                //    continue;
                //}
                KeyValue[] kvs = new KeyValue[4];
                kvs[0] = new KeyValue("uuid", AppOpera.myClass.rykjMemberId);
                kvs[1] = new KeyValue("classesId", tmpId+"");
                kvs[2] = new KeyValue("studentId", ClassPanelControll.sin.id + "");
                kvs[3] = new KeyValue("no", ClassPanelControll.sin.no + "");
                uw = this.PostWithParams(servelets.selectStudentWrongTitlteNo, kvs);
                yield return uw.Send();
                Debug.Log(uw.downloadHandler.text);
                Debug.Log(uw.isNetworkError);
                filePath = PublicClass.filePath + "classId_" + tmpId + "_Error_QAJSON.json";
                if (!uw.isNetworkError)
                {
                    if (uw.downloadHandler.text.Contains("查询失败") || !uw.downloadHandler.text.Contains("option"))
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        if (File.Exists(PublicClass.filePath + "classId_" + tmpId + "_Error_QAXML.xml")) {
                            File.Delete(PublicClass.filePath + "classId_" + tmpId + "_Error_QAXML.xml");
                        }
                        continue;
                    }
                    try
                    {

                        Debug.Log(filePath);
                        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        //Json字符串格式化
                        Debug.Log(uw.downloadHandler.text);
                        string jsondata = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(uw.downloadHandler.text), Formatting.Indented);
                        byte[] b = System.Text.Encoding.UTF8.GetBytes(jsondata);
                        fs.Write(b, 0, b.Length);
                        fs.Flush();
                        fs.Close();
                        JsonToXmlTool.JsonToXml(tmpId, classnameDic[tmpId], "_Error");
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        Debug.Log(e.StackTrace);
                        Debug.Log(tmpId + "--------error");
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }

                }
                else
                {
                    continue;
                }
                #endregion
            }
        }
        yield return null;
    }


    

}



[Serializable]
public class KeyValue {
    public string key;
    public string value;
    public KeyValue()
    {

    }
    public KeyValue(string key,string value)
    {
        this.key = key;
        this.value = value;
    }
}