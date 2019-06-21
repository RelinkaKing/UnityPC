using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UploadImageTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CaptureScreen();
    }
    void CaptureScreen()
    {
        ScreenCapture.CaptureScreenshot("Screenshot.png", 0);

        byte[] bytes = File.ReadAllBytes("Screenshot.png");
        string url = "http://127.0.0.1:8000/image/uploadImg/";//要上传到的地址
        StartCoroutine(PostData(url, bytes));//启动子线程
    }
  
    string csrftoken = "";
    string csrfmiddlewaretoken = "";
    string COOKIE = "";
    IEnumerator PostData(string url,byte[] bytes) {
        if (csrftoken == "") {
            WWW www2 = new WWW(url);
            yield return www2;
            foreach (string key in www2.responseHeaders.Keys) {
                //print(key);
                if (key == "SET-COOKIE") {
                    //print(www2.responseHeaders[key]);
                    csrftoken=Regex.Match(www2.responseHeaders[key], "csrftoken=(.+?);").Groups[1].Value;
                    COOKIE = www2.responseHeaders[key];
                }
            }
            csrfmiddlewaretoken = Regex.Match(www2.text, "csrfmiddlewaretoken.+?value=\"(.+?)\"").Groups[1].Value;
        }
        
       
        // 所有表单数据
        ArrayList bytesArray = new ArrayList();
        // 普通表单
        //bytesArray.Add(CreateFieldData("FileName", txtFilename.Text));
        bytesArray.Add(CreateFieldData("csrfmiddlewaretoken", csrfmiddlewaretoken));
        // 文件表单
        bytesArray.Add(CreateFieldData("img", "哈哈.png","image/png", bytes));

        // 合成所有表单并生成二进制数组
        byte[] formDatas = JoinBytes(bytesArray);



        HttpWebRequest hwr = HttpWebRequest.Create(url) as HttpWebRequest;
        hwr.Headers.Add("Cookie", "csrftoken=" + csrftoken);
        //hwr.ContentType = "multipart/form-data";
        hwr.ContentType = ContentType;
        hwr.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:30.0) Gecko/20100101 Firefox/30.0";
        hwr.ContentLength = formDatas.Length;
        hwr.Method = "POST";
        //hwr.CookieContainer.SetCookies
        using (Stream reqStream = hwr.GetRequestStream())
        {
            reqStream.Write(formDatas, 0, formDatas.Length);
        }
        using (WebResponse wr = hwr.GetResponse())
        {
            //Debug.Log(wr.ToString());
        }
    }
    Encoding encoding = Encoding.UTF8;
    //-----------------------------6611323384671
    //Content-Disposition: form-data; name="csrfmiddlewaretoken"

    //YAPcqmKpBE2APgKZ3t78BqeYuyHdPKLOROS6EtHgZghGHdu2L77IdrSQpqDBXKT5
    //-----------------------------6611323384671
    //Content-Disposition: form-data; name="img"; filename="中文asdasd.PNG"
    //Content-Type: image/png

    public byte[] JoinBytes(ArrayList byteArrays)
    {
        int length = 0;
        int readLength = 0;

        // 加上结束边界
        string endBoundary = Boundary + "--\r\n"; //结束边界
        byte[] endBoundaryBytes = encoding.GetBytes(endBoundary);
        byteArrays.Add(endBoundaryBytes);

        foreach (byte[] b in byteArrays)
        {
            length += b.Length;
        }
        byte[] bytes = new byte[length];

        // 遍历复制
        //
        foreach (byte[] b in byteArrays)
        {
            b.CopyTo(bytes, readLength);
            readLength += b.Length;
        }

        return bytes;
    }


    public byte[] CreateFieldData(string fieldName, string fieldValue)
    {
        string textTemplate = Boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
        string text = String.Format(textTemplate, fieldName, fieldValue);
        byte[] bytes = encoding.GetBytes(text);
        return bytes;
    }
    public byte[] CreateFieldData(string fieldName, string filename, string contentType, byte[] fileBytes)
    {
        //string tmpFiled = Boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n{1}\r\n";
        //string csToken = String.Format(tmpFiled, "csrfmiddlewaretoken",csrftoken);
        string end = "\r\n";
        string textTemplate = Boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

        // 头数据
        string data = String.Format(textTemplate, fieldName, filename, contentType);
        byte[] bytes = encoding.GetBytes(data);



        // 尾数据
        byte[] endBytes = encoding.GetBytes(end);

        // 合成后的数组
        byte[] fieldData = new byte[bytes.Length + fileBytes.Length + endBytes.Length];

        bytes.CopyTo(fieldData, 0); // 头数据
        fileBytes.CopyTo(fieldData, bytes.Length); // 文件的二进制数据
        endBytes.CopyTo(fieldData, bytes.Length + fileBytes.Length); // \r\n

        return fieldData;
    }
    public string Boundary
    {
        get
        {
            string[] bArray, ctArray;
            string contentType = ContentType;
            ctArray = contentType.Split(';');
            if (ctArray[0].Trim().ToLower() == "multipart/form-data")
            {
                bArray = ctArray[1].Split('=');
                return "--" + bArray[1];
            }
            return null;
        }
    }
    public string ContentType
    {
        get
        {
                return "multipart/form-data; boundary=--ABCD";   
        }
    }

    //累计时间
    private float AccumilatedTime = 0f;
    //每帧刷新时间
    private float FrameLength = 0.1f; //0.04 50 miliseconds  
    //帧数统计
    private int GameFrame = 0;
    public Image img;
    /// <summary>
    /// 随机颜色
    /// </summary>
    /// <returns></returns>
    public Color randomColor()
    {
        int r = UnityEngine.Random.Range(0, 256);
        int g = UnityEngine.Random.Range(0, 256);
        int b = UnityEngine.Random.Range(0, 256);
        return new Color(r/255f, g/255f, b/255f);
    }
    // Update is called once per frame
    void Update () {
        AccumilatedTime = AccumilatedTime + Time.deltaTime;
        //in case the FPS is too slow, we may need to update the game multiple times a frame   
        while (AccumilatedTime > FrameLength)
        {
            //img.color = randomColor();
            CaptureScreen();
            AccumilatedTime = AccumilatedTime - FrameLength;
        }
        
	}
    public void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;    //这是设置背景填充的
        bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
        bb.fontSize = 40;       //当然，这是字体大小

        ////居中显示FPS
        //GUI.Label(new Rect((Screen.width / 2) - 40, 0, 200, 200), "FPS: " + m_FPS, bb);

        GUI.Label(new Rect((Screen.width / 2) - 100, 0, 200, 200), "reallyTime: " + System.DateTime.Now.Second, bb);

    }
}
