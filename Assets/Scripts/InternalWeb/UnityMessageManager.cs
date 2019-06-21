using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ICODES.STUDIO.WWebView;
using UnityEngine.UI;

public class UnityMessageManager : MonoBehaviour {

 	public static UnityMessageManager Instance { get; private set; }
    public GameObject web_obj;
    public Transform parent;
	public Browser main_browser;  //网页主入口
    public WWebView webView = null;
    public delegate void InternalGetMessage(string msg);
    public static event InternalGetMessage InternalMessageEvent;
    public delegate void ShowUnityMessage(UnityMessage msg);
    public static event ShowUnityMessage ShowMessageEvent;
    public string url;//"https://www.google.com"
    public int margine = 30;

    private void Awake() {
		Instance = this;
        DontDestroyOnLoad(this);
        url = "file://" + Application.streamingAssetsPath + "/" + "root.html";//"https://www.google.com"
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            main_browser.CallFunction("ResizeWeb");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayVideo("C:/Users/Raytine/Desktop/620x252_search.mp4");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            InitWebView(webView);
            webView.NavigateFile("demo.html");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            InitWeb(url);
        }
    }

    public float x = 0, y = 0;

    public void InitWebView(WWebView webView)
    {
        webView.OnStartNavigation += OnStartNavigation;
        webView.OnNavigationCompleted += OnNavigationCompleted;
        webView.OnNavigationFailed += OnNavigationFailed;
        webView.OnReceiveMessage += OnReceiveMessage;
        webView.OnEvaluateJavaScript += OnEvaluateJavaScript;
        webView.OnClose += OnClose;

        webView.Initialize(new Vector4(0, 100f, 0, 0), new Vector2(x, y));
        //#if UNIWEBVIEW3_SUPPORTED
        //#else
        //webView.Initialize(new Vector4(0, margine, 0, margine), new Vector2(0, 0));
        //#endif
    }

    public void CloseWebView()
    {
        webView.InternalOnClose();
    }

    //初始化网页，
    public void InitWeb(string url)
    {
        GameObject obj = Instantiate(web_obj, parent);
        main_browser = obj.GetComponent<Browser>();
        //注册网页函数，用于接收网页参数传递，app 信息，控制命令等
        main_browser.pub_url = url;
        main_browser.RegisterFunction("sendWebMessage", args => {
            OnGetMessageFromWeb(args[0]);
        });
        main_browser.CallFunction("ResizeWeb");
    }

    public void LoadLocalFile()
    {
        webView.NavigateFile("触诊.html");
        DebugLog.DebugLogInfo("LoadLocalFile demo.html");
    }

    void Navigate(string url)
    {
        webView.Navigate(url);
    }

    public void PlayVideo(string _url)
    {
        InitWebView(webView);
        //string _url = "C:/Users/Raytine/Desktop/620x252_search.mp4";
        string html = "<html>" +
            "<body>" +
            "<div style=\"position:absolute;width:100%;height:100%;\">" +
            "<video controls=\"controls\"><source src=" + _url + ">" +
            "</video>" +
            "</div></body>" +
            "<script type=\"application/javascript\">" +
            "window.onload=function(){alert('script use');}</script>" +
        "</html>";
        webView.NavigateString(html);
    }

    public void LoadImageInHtml(string _url,Rect view_rect)
    {
        x = view_rect.width*Screen.width/1920f;
        y = view_rect.height * Screen.height / 1080f;
        InitWebView(webView);
        //string _url = "C:/Users/Raytine/Desktop/620x252_search.mp4";
        //img width = "100%" src = "./作用_files/76be19359c89463680964346767f39ba.gif"
        string html = "<html>" +
            "<body style=\"overflow-x:hidden;overflow-y:hidden;\">" +
            "<div style=\"position:absolute;width:100%;height:100%;\">" +
            "<img width=\"100%\" height=\"100%\" src=" + _url + ">" +
            "</div></body>" +
        "</html>";
        webView.NavigateString(html);
    }

    public void SetStyle(Browser browser,Vector4 position)
	{
		//设置网页窗体大小位置
	}

    void OnGetMessageFromWeb(string msg)
    {
        DebugLog.DebugLogInfo("00ff00","get web json :"+ msg);
        MessageHandler handler= MessageHandler.Deserialize(msg);
        switch (handler.name)
        {
            case "app":
                    InternalMessageEvent(msg);
                break;
            case "hide":
                break;
            case "fade":
                break;
            case "hideOther":
                break;
            case "fadeOther":
                break;
            case "resize":
                JObject jo = (JObject)JsonConvert.DeserializeObject(msg);
                string data = jo["data"].ToString();
                JObject jo2 = (JObject)JsonConvert.DeserializeObject(data);
                main_browser.ResizeWebContent(int.Parse(jo2["marginLeft"].ToString()), int.Parse(jo2["marginTop"].ToString()), int.Parse(jo2["marginRight"].ToString()), int.Parse(jo2["marginBottom"].ToString()));
                break;
            default:
                break;
        }
    }

    //发送模型信息到网页，网页搜索 查询数据库，加载对应数据
	public void SendMessageToWeb(string msg, JObject obj)
	{
        JObject o = JObject.FromObject(new
        {
            msg,
            obj
        });
        main_browser.CallFunction("SetState", o.ToString());
	}

    protected virtual void OnReceiveMessage(WWebView webView, string message)
    {
        DebugLog.DebugLogInfo("00ff00", "OnReceiveMessage");
        if (message == "wwebview://openfile/")
            LoadLocalFile();

        else if (message == "wwebview://loadstring/")
            LoadHTML();
    }

    public void OnAddJavaScript(WWebView webView,string js_string)
    {
        webView.AddJavaScript(js_string);
    }

    protected virtual void OnEvaluateJavaScript(WWebView webView, string result)
    {
        DebugLog.DebugLogInfo("00ff00", "OnEvaluateJavaScript");
    }

    protected virtual void OnStartNavigation(WWebView webView, string url)
    {
        DebugLog.DebugLogInfo("00ff00", "OnStartNavigation");
    }

    protected virtual void OnNavigationCompleted(WWebView webView, string data)
    {
        DebugLog.DebugLogInfo("00ff00", "OnNavigationCompleted");
    }

    protected virtual void OnNavigationFailed(WWebView webView, int code, string url)
    {
        DebugLog.DebugLogInfo("00ff00", "OnNavigationFailed");
    }

    protected virtual bool OnClose(WWebView webView)
    {
        DebugLog.DebugLogInfo("00ff00", "OnClose");
//#if UNITY_EDITOR
        // NOTE: Keep in mind that you are just watching the DEMO now.
        // Destroy webview instance not WWebView component.
        webView.Destroy();

        // Don't destroy WWebView component for this demo.
        // In most cases, you will return true to remove the component.
        return false;
//#else
//        // In "real" game, you will never quit your app since the webview is closed.
//        UnityEngine.Application.Quit();
//        return true;
//#endif
    }

    public void LoadHTML()
    {
        webView.NavigateString(@"<html></html>");
    }

    public static string ToSixteen(string input)
    {
        char[] values = input.ToCharArray();
        string end = string.Empty;
        foreach (char letter in values)
        {
            int value = Convert.ToInt32(letter);
            string hexoutput = string.Format("{0:X}", value); //0 表示占位符 x或X表示十六进制
            end += hexoutput + "_";
        }
        end = end.Remove(end.Length - 1);
        return end;
    }

    public void SendMessageToRN(UnityMessage message)
    {
        if (ShowMessageEvent != null)
        {
            ShowMessageEvent(message);
        }
    }

    public void SendMessageToRN(string message, JObject jobjct, string log)
    {
        SendMessageToRN(new UnityMessage()
        {
            name = message,
            data = jobjct,
            callBack = (data) => { DebugLog.DebugLogInfo("message : " + log); }
        });
    }
}

public class UnityMessage
{
    public String name;
    public JObject data;
    public Action<object> callBack;
}

public class MessageHandler
{
    public string name;
    private JToken data;

    public static MessageHandler Deserialize(string message)
    {
        JObject m = JObject.Parse(message);
        MessageHandler handler = new MessageHandler(
            m.GetValue("name").Value<string>(),
            m.GetValue("data")
        );
        return handler;
    }

    public MessageHandler(string name, JToken data)
    {
        this.name = name;
        this.data = data;
    }

    public void send(object data)
    {
        //JObject data_obj = new JObject();
        //data_obj["littleMap"] = tempInfo.littleMap;
        //data_obj["model_Id"] = tempInfo.model_Id;
        //data_obj["ModelName"] = tempInfo.ModelName;
        //if (String.IsNullOrEmpty(tempInfo.Chinese))
        //    data_obj["Chinese"] = "";
        //else
        //    data_obj["Chinese"] = ToSixteen(tempInfo.Chinese);

        //if (String.IsNullOrEmpty(tempInfo.English))
        //    data_obj["English"] = "";
        //else
        //    data_obj["English"] = tempInfo.English;

        //if (String.IsNullOrEmpty(tempInfo.Note))
        //    data_obj["Note"] = "";
        //else
        //    data_obj["Note"] = ToSixteen(tempInfo.Note); ;
        //UnityMessageManager.Instance.SendMessageToRN("model", data_obj, "");
    }
}