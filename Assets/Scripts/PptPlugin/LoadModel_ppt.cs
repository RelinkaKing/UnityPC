using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using vesal_network;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;
using LiteDB;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using Newtonsoft.Json;
using Assets.Scripts.Repository;
using Assets.Scripts.Model;
using UnityEngine.SceneManagement;

public class LoadModel_ppt : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);

    //[DllImport("user32.dll")]
    //static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(string lp1, string lp2);
    [DllImport("user32.dll", EntryPoint = "ShowWindow")]
    public static extern IntPtr ShowWindow(IntPtr hWnd, int _value);
    [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
    public static extern bool SetF(IntPtr hWnd); //设置此窗体为活动窗体
    [DllImport("user32.dll", EntryPoint = "PostMessage")]
    public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    private const int WM_KEYDOWN = 0X100;
    private const int WM_KEYUP = 0X101;

    private const int VK_RETURN = 0x0D;
    private const int VK_ESCAPE = 0x1B;
    private const int VK_SPACE = 0x20;
    private const int VK_PRIOR = 0x21;
    private const int VK_NEXT = 0x22;
    private const int VK_UP = 0x26;
    private const int VK_DOWN = 0x28;
    private const int VK_LEFT = 0x25;
    private const int VK_RIGHT = 0x27;

    //记录windows 任务栏和PPT播放窗口的句柄
    public IntPtr windows_task, windows_ppt = IntPtr.Zero;
    //记录PPT当前播放页码
    public int pageno_ppt = 1;
    vesal_network.vesal_socket _listen_sock = new vesal_network.vesal_socket();
    ArrayList _client_socks = new ArrayList();

    [DllImport("User32.dll")]
    public static extern void keybd_event(Byte bVk, Byte bScan, Int32 dwFlags, Int32 dwExtraInfo);

    byte up = 38;
    byte down = 40;
    byte left = 37;
    byte right = 39;
    byte esc = 27;
    byte pagedown = 34;
    byte pageup = 33;
    byte enter = 13;

    public void press_key(byte key)
    {
        keybd_event(key, 0, 0, 0);
        keybd_event(key, 0, 2, 0);
    }

    
    // Use this for initialization
    void Start()
    {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.plugin) {
            DestroyImmediate(this.gameObject);
            return;
        }
        //不要摧毁此物体，切换控制
        DontDestroyOnLoad(this.gameObject);
        //找到windows 任务栏窗口，并设置为隐藏
        windows_task = FindWindow("Shell_TrayWnd", null);
//        ShowWindow(windows_task, 0);
        vesal_log.vesal_write_log("start!!");
        vesal_log._log_path = GetEnvironmentVariableData() + "\\unity.log";
        //输出版本
        vesal_log.vesal_write_log("unity start function. version:1.0.20180310");
        //获取窗口
        Process[] localByName = Process.GetProcessesByName("vesal_3d_ppt_unity");
        //当已有打开的窗口时，关闭此窗口
        if (localByName.Length > 1)
        {
            vesal_log.vesal_write_log("unity start again function.");
            Application.Quit();
        }
        try
        {
        
            vesal_log.vesal_write_log("start.s");
            //初始化网络协议
            bool ret = _listen_sock.listen(vesal_network.vesal_socket.get_vesal_port());
            if (ret)
            {
                vesal_log.vesal_write_log("listen ok");
            }
            else
            {
                vesal_log.vesal_write_log("listen failed");
            }
        }
        catch
        {
            vesal_log.vesal_write_log("cant listen!");
        }
        
        //最小化
        GetComponent<WindowControl>().WindowMin();
#if UNITY_EDITOR
        //SA0200000
        //SA0103041
        //标注
        //91*C:\VesalDigital\PPT\Data\vesal.dat*C13B491BEB*SA0104018

        //心脏
        //SA0601013
        //SA0104025尾骨
        //GameObject.Find("GameObject").SendMessage("ChangeModel", "91*C:\\VesalDigital\\PPT\\Data\\vesal.dat*C13B491BEB*SA0200000");
        //2D
        //SA0607001

        //SA0104025尾骨

        //SA0104004

        //新标注 SA0104004
        //旧标注 SA0903001
        //3D动画 SA0107018
        //心脏2D动画 SA0607001
        //全身骨骼 SA0101047
        //肩关节 SA0102030

        //蝶骨旧标注 SA0104018

        ChangeModel("91*C:\\VesalDigital\\PPT\\Data\\vesal.dat*C13B491BEB*SA0101047");
        
#endif
    }


    
    string V3ToStr(Vector3 tmp) {
        return "("+tmp.x.ToString("f6") + ","+tmp.y.ToString("f6") + ","+tmp.z.ToString("f6") + ")";
    }

    // Update is called once per frame
    void Update()
    {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.plugin)
        {
            return;
        }
#if UNITY_EDITOR
            //新标注 SA0104004
            //旧标注 SA0903001
            //3D动画 SA0107018
            //心脏2D动画 SA0607001
            //全身骨骼 SA0101047
        //肩关节 SA0102030C13B491BEB
        if (Input.GetKeyDown(KeyCode.F1)) {
            ChangeModel("91*C:\\VesalDigital\\PPT\\Data\\vesal.dat*C13B491BEB*SA0107016");
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ChangeModel("91*C:\\VesalDigital\\PPT\\Data\\vesal.dat*C13B491BEB*SA0607013");
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ChangeModel("91*C:\\VesalDigital\\PPT\\Data\\vesal.dat*C13B491BEB*SA0607007");
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ChangeModel("91*C:\\VesalDigital\\PPT\\Data\\vesal.dat*C13B491BEB*SA0607001");
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ChangeModel("91*C:\\VesalDigital\\PPT\\Data\\vesal.dat*C13B491BEB*SA0101047");
        }
#endif
        //点击任何按键
        if (Input.anyKeyDown)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                //将按钮模拟发给PPT
                if (windows_ppt != IntPtr.Zero)
                {
                    SetF(windows_ppt);
                    press_key(down);
                }
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                //将按钮模拟发给PPT
                if ((windows_ppt != IntPtr.Zero) && (pageno_ppt != 1))
                {
                    SetF(windows_ppt);
                    press_key(up);
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                //将按钮模拟发给PPT
                if ((windows_ppt != IntPtr.Zero) && (pageno_ppt != 1))
                {
                    SetF(windows_ppt);
                    press_key(left);
                }
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                //将按钮模拟发给PPT
                if ((windows_ppt != IntPtr.Zero))
                {
                    SetF(windows_ppt);
                    press_key(right);
                }
            }

            else if (Input.GetKey(KeyCode.PageDown))
            {
                //将按钮模拟发给PPT
                if (windows_ppt != IntPtr.Zero)
                {
                    SetF(windows_ppt);
                    press_key(pagedown);
                }
            }
            else if (Input.GetKey(KeyCode.PageUp))
            {
                //将按钮模拟发给PPT
                if ((windows_ppt != IntPtr.Zero) && (pageno_ppt != 1))
                {
                    SetF(windows_ppt);
                    press_key(pageup);
                }
            }
            else if (Input.GetKey(KeyCode.Escape))
            {
                //将按钮模拟发给PPT
                if (windows_ppt != IntPtr.Zero)
                {
                    SetF(windows_ppt);
                    press_key(esc);
                }
            }
            else if (Input.GetKey(KeyCode.Return))
            {
                //将按钮模拟发给PPT
                if (windows_ppt != IntPtr.Zero)
                {
                    SetF(windows_ppt);
                    press_key(enter);
                }
            }
        }
    }

    private void FixedUpdate()
    {

        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.plugin)
        {
            return;
        }
        try
        {
            ArrayList listenList = new ArrayList();
            ArrayList clientList = new ArrayList();
            listenList.Add(_listen_sock._sock);
            Socket.Select(listenList, null, null, 1000);
            if (listenList.Count > 0)
            {
                vesal_network.vesal_socket sok = new vesal_network.vesal_socket();
                sok._sock = _listen_sock.accept();
                _client_socks.Add(sok);
                vesal_log.vesal_write_log("peer connect!");
            }

            for (int i = 0; i < _client_socks.Count; i++)
            {
                clientList.Add(((vesal_network.vesal_socket)_client_socks[i])._sock);
            }
            if (clientList.Count>0) {
                Socket.Select(clientList, null, null, 1000);
            }
            for (int i = 0; i < clientList.Count; i++)
            {
                int index = find_sock_seted((Socket)clientList[i]);
                bool get_packet = false;
                try
                {
                    packet pk = new packet();
                    //((vesal_network.vesal_socket)_client_socks[index])._get_header = true;
                    get_packet = ((vesal_network.vesal_socket)_client_socks[index]).recv_packet(ref pk);
                    if (get_packet)
                    {
                        byte[] buff = new byte[pk._data_len];
                        for (int j = 0; j < pk._data_len; j++)
                        {
                            buff[j] = pk._data[j];
                        }
                        string str = System.Text.Encoding.Default.GetString(buff);
#if UNITY_EDITOR
                        UnityEngine.Debug.LogError("str:"+str);
#endif
                        String log_content = String.Format("get pack {0}:{1}", pk._cmd_code, str);
                        vesal_log.vesal_write_log(log_content);
                        if (pk._cmd_code == (byte)VESAL_CMD_CODE.MSG_CMD)
                        {
                            
                            ChangeModel(str);
                            
                            ((vesal_network.vesal_socket)_client_socks[index]).send_confirm();
                            vesal_log.vesal_write_log("change model.");
                        }
                        else if (pk._cmd_code == (byte)VESAL_CMD_CODE.CTRL_CMD)
                        {
                            if (str == "shutdown")
                            {
                                vesal_log.vesal_write_log("I will exit.");
                                // 首先发送确认消息。
                                ((vesal_network.vesal_socket)_client_socks[index]).send_confirm();
                                // 增加进程退出代码。
                                //Application.runInBackground = true;
                                Process.GetCurrentProcess().Kill();
                                //Application.Quit();
                            } else if (str == "min") {
                                //最小化
                                GetComponent<WindowControl>().WindowMin();
                            }
                        }
                        else if (pk._cmd_code == (byte)VESAL_CMD_CODE.WIN_HWND)
                        {
                            //获得PPT播放窗口和PPT页码
                            string[] res = str.Split(',');
                            if (res.Count() > 1)
                            {
                                windows_ppt = new IntPtr(int.Parse(res[0]));
                                pageno_ppt = int.Parse(res[1]);
                                pageNo = pageno_ppt;
                                pptName = res[3]+"_"+res[2];
                                //if (res.Count() >= 8)
                                //{
                                //    int x = int.Parse(res[4]);
                                //    int y = int.Parse(res[5]);
                                //    int width = int.Parse(res[6]);
                                //    int height = int.Parse(res[7]);

                                //    int HWND_TOPMOST = -1;
                                //    int SWP_FRAMECHANGED = 0x0020;
                                //    int SWP_DRAWFRAME = SWP_FRAMECHANGED;
                                //    IntPtr hw= FindWindow(null, "PPTPlugin");
                                //    SetWindowPos(  hw, -1, x, y, width, height, SWP_DRAWFRAME);
                                //}
                            }
                        }
                    }
                }
                catch(Exception e)
                {
#if UNITY_EDITOR
      
                    UnityEngine.Debug.Log(e.Message);
                    UnityEngine.Debug.Log(e.StackTrace);
#endif
                    // link broken.
                    ((vesal_network.vesal_socket)_client_socks[index]).close();
                    remove_sock_list((Socket)clientList[i]);
                    vesal_log.vesal_write_log("link broken");
                }
            }
        }
        catch(Exception e)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(e.Message);
            UnityEngine.Debug.Log(e.StackTrace);
#endif
        }
    }

    private void OnDestroy()
    {
     
        //恢复显示windows 任务栏
        ShowWindow(windows_task, 5);
    }

    private void write_process_info()
    {
        //创建一个有名互斥锁，作为进程启动标志，如果ppt获取不到这个锁，就认为unity程序没有启动
        System.Threading.Mutex mutex = new Mutex(true, "vesal");
        //将自己的进程id和窗口句柄，写到文件中
        process_info.record_process_info();
        vesal_log.vesal_write_log("zouni");
    }

    private int find_sock_seted(Socket sock)
    {
        for (int i = 0; i < _client_socks.Count; i++)
        {
            if (sock.Handle == ((vesal_network.vesal_socket)_client_socks[i])._sock.Handle)
                return i;
        }
        return -1;
    }

    private void remove_sock_list(Socket sock)
    {
        for (int i = 0; i < _client_socks.Count; i++)
        {
            if (sock.Handle == ((vesal_network.vesal_socket)_client_socks[i])._sock.Handle)
            {
                _client_socks.RemoveAt(i);
                return;
            }
        }
    }

    //获取环境变量中用户变量中的信息
    private string GetEnvironmentVariableData()
    {
        string sPath = Environment.GetEnvironmentVariable("Vesal_Path", EnvironmentVariableTarget.User);
        vesal_log.vesal_write_log(sPath);
        return sPath;
    }
    public static string url;
    public static string sceneName;
    public static string User;
    public static string password;
    public static int pageNo;
    public static string pptName;
    public static bool isUpdateDbDown;
    public IEnumerator waitForUpdate(string xmlName) {
        UnityEngine.Debug.Log("waitForUpdate");
        UnityEngine.Debug.Log(AppOpera.OperaStatus);
        while (AppOpera.OperaStatus != AppOperState.Loop && AppOpera.OperaStatus != AppOperState.Apprun && AppOpera.OperaStatus != AppOperState.LoadScene)
        {
            yield return null;
        }
        UnityEngine.Debug.Log("end wait");
        string tmpName;
        tmpName = "PPTVesal";
        try
        {
            char[] separator = { '*' };
            string[] message = new string[4];
            //"91*C:\\VesalDigital\\PPT\\Data\\vesal.dat*C13B491BEB*SA0101047"
            message = xmlName.Split(separator);
            User = message[0];
            url = message[1];
            password = message[2];
            sceneName = message[3];
            //CollectionB = "";
            UnityEngine.Debug.Log("message:" + message);


            //不要摧毁此物体，切换控制
            DontDestroyOnLoad(this.gameObject);

            tmpName = LoadXml();
            if (tmpName == "PPTBookmark")
            {
                string jsonPath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.LastIndexOf("AppData") + 7) + "/roaming/Vesal/sign/NewAndOldSign.json";
                Dictionary<int, NewAndOldSign> nao = JsonConvert.DeserializeObject<Dictionary<int, NewAndOldSign>>(File.ReadAllText(jsonPath));
                foreach (NewAndOldSign tmpNao in nao.Values)
                {
                    if (sceneName == tmpNao.app_id_old && tmpNao.app_id_new != "")
                    {
                        sceneName = tmpNao.app_id_new;
                        //UnityEngine.Debug.LogError(sceneName);
                        tmpName = "UI_sign_all";
                        break;
                    }
                }
            }
            UnityEngine.Debug.Log("tmpName:" + tmpName+"--"+sceneName);
            string result = getType(sceneName);
            string[] tmpAyyay = result.Split('@');
            PublicClass.app = null;
            if (SceneModels.instance != null) {
                SceneModels.instance.destoryTempParent();
            }
            //判断模式
            switch (tmpName)
            {
                case "PPTAnimation":
                    //3D动画
                    AnimationControl.url = url;
                    AnimationControl.pwd = password;
                    AnimationControl.aniNo = sceneName;
                    ClearSceneData.LoadLevel(tmpName);
                    break;
                case "PPTAnimation2D":
                    ClearSceneData.LoadLevel(tmpName);
                    //2d动画
                    break;
                case "PPTBookmark":
                    //旧标注
                    PublicClass.app = new App();
                    PublicClass.app.app_type = "sign";
                    PublicClass.app.struct_name = tmpAyyay[1];
                    PublicClass.app.app_id = sceneName;
                    //PublicClass.app.ab_path = sceneName + "_1.assetbundle";
                    PublicClass.app.xml_path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.LastIndexOf("AppData") + 7) + "/roaming/Vesal/sign/"+sceneName + "_1.assetbundle";
                    break;
                case "UI_sign_all":
                    //新标注
                    PublicClass.app = new App();
                    PublicClass.app.ab_path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.LastIndexOf("AppData") + 7) + "/roaming/Vesal/sign_ssp_path/SignNewJG.db";
                    PublicClass.app.app_id = sceneName;
                    PublicClass.app.app_type = "sign_new";
                    break;
                case "PPTVesal":
                    PublicClass.app = new App();
                    //PublicClass.app.ab_path = ;
                    PublicClass.app.app_id = sceneName;
                    PublicClass.app.app_type = "model";
                    //模型
                    break;
            }
            if (!tmpName.Contains("PPTAnimation")) {
                if (GameObject.Find("DataOpera") != null)
                {
                    if (PublicClass.app != null)
                    {
                        GameObject.Find("DataOpera").GetComponent<AppOpera>().Get_app(PublicClass.app);
                    }
                }
                else {
                    SceneManager.LoadScene("SceneSwitch");
                }
            }
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
            UnityEngine.Debug.Log(e.StackTrace);
        }

        //isReadyToSwitch = true;
        //SwitchSceneName = tmpName;
    }

    
    public string getType(string modelId)
    {
        var local_db = new DbRepository<GetStructList>();
        local_db.DataService("vesali.db");
        GetStructList tmpIe = local_db.SelectOne<GetStructList>((tempNo) =>
        {
            if (tempNo.nounNo == modelId)
            {
                return true;
            }
            else
            {
                return false;
            }

        });
        if (tmpIe == null)
        {
            local_db.Close();
            return "model";
        }
        local_db.Close();
        string result = "model";

        if (tmpIe.nounType == "1")
        {
            string jsonPath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.LastIndexOf("AppData") + 7) + "/roaming/Vesal/sign/NewAndOldSign.json";
            Dictionary<int, NewAndOldSign> nao = JsonConvert.DeserializeObject<Dictionary<int, NewAndOldSign>>(File.ReadAllText(jsonPath));
            foreach (NewAndOldSign tmpNao in nao.Values)
            {
                if (modelId == tmpNao.app_id_old && tmpNao.app_id_new != "")
                {
                    result = "newSign@" + tmpNao.app_id_new;
                    break;
                }
            }
            result = "oldSign@" + tmpIe.nounName;
        }
        else
        {
            result = "model";
        }
        return result;
    }
  

    class NewAndOldSign {
        public string fy_name_new;
        public string struct_name_new;
        public string app_type_new;
        public string app_id_new;
        public string fy_name_old;
        public string struct_name_old;
        public string app_id_old;
        public string app_type_old;
        public string platform_old;
    }
    bool isReadyToSwitch = false;
    string SwitchSceneName = string.Empty;
    //模型切换，信息解析
    private void ChangeModel(string xmlName)
    {
        //try {
        //    if (isSplit)
        //    {
        //        Camera.main.GetComponent<MouseFollowRotation>().SplitModel();
        //        //Camera.main.BroadcastMessage("SplitModel", null, SendMessageOptions.DontRequireReceiver);

        //    }
        //    if (PublicClass.isMultiple) {
        //        Camera.main.GetComponent<TouchContorl>().multipleChoiceTrans();
        //        //Camera.main.BroadcastMessage("multipleChoiceTrans", null, SendMessageOptions.DontRequireReceiver);
        //    }
        //}
        //catch (Exception e) {
        //    UnityEngine.Debug.Log(e.Message);
        //    UnityEngine.Debug.Log(e.StackTrace);
        //}
        
        StartCoroutine(waitForUpdate(xmlName));
    }

    //读取XML
    private string LoadXml()
    {
        string mode = "PPTVesal";
        try
        {
            vesal_log.vesal_write_log("开始从DB库中读取XML文件");
            ConnectionString connect1 = new ConnectionString();
            connect1.Filename = url;
            connect1.LimitSize = 10000000000;
            connect1.Journal = false;
            connect1.Mode = LiteDB.FileMode.ReadOnly;
            using (var db = new LiteDatabase(connect1))
            {
                bool isHavScene = db.CollectionExists(sceneName + ".xml");
                if (!isHavScene)
                {
                    var stream = db.FileStorage.OpenRead(sceneName + ".xml");
                    XmlDocument doc = new XmlDocument();
                    doc.Load(stream);
                    vesal_log.vesal_write_log("开始读取XML内容");
                    XmlElement rootElement = doc.DocumentElement;
                    XmlNodeList personNodes = rootElement.GetElementsByTagName("scene");
                    string modeC = ((XmlElement)personNodes[0]).GetAttribute("mode");
                    //判断模式
                    switch (modeC)
                    {
                        case "normal":
                            mode = "PPTVesal";
                            break;
                        case "Animation3D":
                            mode = "PPTAnimation";
                            break;
                        case "Animation2D":
                            mode = "PPTAnimation2D";
                            break;
                        case "Bookmark":
                            mode = "PPTBookmark";
                            break;
                    }
                }
            }
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
            UnityEngine.Debug.Log(e.StackTrace);
            mode = "PPTVesal";
        }
        return mode;
    }
}


[Serializable]
[XmlRoot("document")]
public class InitInfoDoc
{
    [XmlElement("Item")]
    public List<InitInfoItem> items;
}
[Serializable]
public class InitInfoItem
{
    [XmlAttribute("CamPos")]
    public string camPos;
    [XmlAttribute("CamParentPos")]
    public string camParentPos;
    [XmlAttribute("CamParentRot")]
    public string camParentRot;
    [XmlAttribute("id")]
    public string id;
}