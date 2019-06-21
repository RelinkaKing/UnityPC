using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//unity 后台启动 ，开始线程监听
public class testScript 
{
    static vesal_network.vesal_socket sc;
    static vesal_network.packet pc;
    static Thread thread;
    static ushort port;
    public static void OpenLinsen()
    {
        // vesal_log.vesal_write_log("start");
        // vesal_log.vesal_write_log(Hwnd + "-=-=");
        getPort();
        startThread();
    }
    private void OnApplicationQuit()
    {
        stopThread();
    }

    public static void getPort() {
        string filepath = Application.streamingAssetsPath + "/port.txt";
        try
        {
            if (File.Exists(filepath))
            {
                port = ushort.Parse(File.ReadAllText(Application.streamingAssetsPath + "/port.txt"));
            }
            else
            {
                port = 8900;
            }
        }
        catch {
            // DebugLog.DebugLogInfo(port);
        }
    }
    public static void startThread() {
        if (thread == null) {
            // DebugLog.DebugLogInfo("开始连接");
            thread = new Thread(startCon);
            thread.Start();
        }
    }
    public static void stopThread() {
        try
        {
            // DebugLog.DebugLogInfo( "断开连接");
            sc.close();
            thread.Abort();
            thread = null;
        }
        catch
        {
        }
    }

    public static void Send_message(string str)
    {
        sc = new vesal_network.vesal_socket();

        if (sc.connect("127.0.0.1", port))
        {
            // DebugLog.DebugLogInfo("I'm Unity,Save me!");
            int tmp = sc._sock.Send(Encoding.UTF8.GetBytes(str), SocketFlags.None);
        }
        else {
            //连接失败重连
           Send_message(str);
        }
    }


    
    public static void startCon()
    {
        sc = new vesal_network.vesal_socket();

        if (sc.connect("127.0.0.1", port))
        {
            // DebugLog.DebugLogInfo("I'm Unity,Save me!");
            // int tmp = sc._sock.Send(Encoding.UTF8.GetBytes("I'm Unity,Save me!"), SocketFlags.None);
        }
        else {
            //连接失败重连
            startCon();
        }
        pc = new vesal_network.packet();

        byte[] bytes = new byte[1024];
        while (true)
        {
            if (sc != null && sc._sock != null && !sc._sock.Poll(1000, SelectMode.SelectRead))
            {
                int receiveNumber = sc._sock.Receive(bytes);
                string message = Encoding.UTF8.GetString(bytes, 0, receiveNumber);
                switch (message)
                {
                    case "quit":
                        stopThread();
                        break;
                    case "":
                        UnityEngine.Debug.Log("linsten");
                        break;
                    default:
                        UnityEngine.Debug.Log("得到参数："+message);
                        // if(PublicClass.appOpera!=null)
                        //     PublicClass.appOpera.Get_app(message);
                        rcvMessage=message;
                        break;
                }
                
                // UnityEngine.Debug.Log(string.Format(" 接收客户端{0}消息{1}", sc._sock.RemoteEndPoint.ToString(), message));
                //         try
                //         {
                //             rcvMessage = "断开连接";
                //             thread.Abort();
                //             sc.close();
                //             break;
                //         }
                //         catch
                //         {
                //         }
                // int tmp = sc._sock.Send(Encoding.UTF8.GetBytes(message));
                // UnityEngine.Debug.Log(tmp + "--" + message.Equals("disconnect") + "--" + (message == "disconnect"));
            }
            //Thread.SpinWait(1);
        }
    }
    public static string rcvMessage{set{rcvmessage=value;} get{return rcvmessage;}}
    private  static string rcvmessage=string.Empty;
    public static void Back_platform()
    {
        //  vesal_log.vesal_write_log("++++++++++++++++++++++++++++++call back");
        rcvMessage=string.Empty;
        if (sc != null && sc._sock != null && !sc._sock.Poll(1000, System.Net.Sockets.SelectMode.SelectRead)) {
            // vesal_log.vesal_write_log("-------------------------------------------back");
            sc.send_data(System.Text.Encoding.UTF8.GetBytes("hide"));
        }
    }

    public static void send_platform(string str)
    {
        rcvMessage=string.Empty;
        if (sc != null && sc._sock != null && !sc._sock.Poll(1000, System.Net.Sockets.SelectMode.SelectRead)) {
            sc.send_data(System.Text.Encoding.UTF8.GetBytes(str));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (receiveText != null) {
        //     receiveText.text = rcvMessage;
        // }
        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    vesal_log.vesal_write_log("F1");
        //    ShowWindow(Hwnd, 5);
        //}
        //if (Input.GetKeyDown(KeyCode.F2))
        //{
        //    vesal_log.vesal_write_log("F2");
        //    WindowMin();
        //}
        // if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        // {
        //     ButtonDown.text = "True";
        //     if (sc != null && sc._sock != null && !sc._sock.Poll(1000, System.Net.Sockets.SelectMode.SelectRead)) {
        //         sc.send_data(System.Text.Encoding.UTF8.GetBytes("hello"));
        //     }
        // }
        // else
        // {
        //     ButtonDown.text = "False";
        // }
        //if (Input.GetKeyDown(KeyCode.F3))
        //{
        //    try
        //    {
        //        thread.Abort();
        //        sc.close();
        //        thread = null;
        //    }
        //    catch
        //    {
        //    }
        //}
        // ButtonDownX.text = "X:" + Input.GetAxis("Mouse X") + "--" + Input.mousePosition.x;
        // ButtonDownY.text = "Y:" + Input.GetAxis("Mouse Y") + "--" + Input.mousePosition.y;
    }
    public void loadScene()
    {
       SceneManager.LoadScene("Scenes/RecordPlay");
    }
    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(System.IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
    public static extern System.IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool EnableWindow(IntPtr hwnd, bool enable);
    [DllImport("user32.dll")]
    private static extern bool MoveWindow(IntPtr handle, int x, int y, int width,
    int height, bool redraw);
    public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetParent(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

    [DllImport("kernel32.dll")]
    public static extern void SetLastError(uint dwErrCode);
    static IntPtr Hwnd;
    public static IntPtr GetProcessWnd()
    {
        IntPtr ptrWnd = IntPtr.Zero;
        uint pid = (uint)Process.GetCurrentProcess().Id;  // 当前进程 ID  
        _pid = pid + "";
        bool bResult = EnumWindows(new WNDENUMPROC(delegate (IntPtr hwnd, uint lParam)
        {
            uint id = 0;

            if (GetParent(hwnd) == IntPtr.Zero)
            {
                GetWindowThreadProcessId(hwnd, ref id);
                if (id == lParam)    // 找到进程对应的主窗口句柄  
                {
                    ptrWnd = hwnd;   // 把句柄缓存起来  
                    SetLastError(0);    // 设置无错误  
                    return false;   // 返回 false 以终止枚举窗口  
                }
            }

            return true;

        }), pid);

        return (!bResult && Marshal.GetLastWin32Error() == 0) ? ptrWnd : IntPtr.Zero;
    }
    public void WindowMin()
    {
        vesal_log.vesal_write_log("find:" + FindWindow(null, "Vesal3D系统解剖2_0"));
        ShowWindow(FindWindow(null, "Vesal3D系统解剖2_0"), 7);

        //ShowWindow(Hwnd, 7);
    }
    public static String _pid = "xxx";
    class vesal_log
    {
        public static String _log_path = @"C:\vesal\unity.log";
        static public void vesal_write_log(String logcontent)
        {
            try
            {
                String time_now = DateTime.Now.ToString();
                FileStream fs = new FileStream(_log_path, System.IO.FileMode.Append);
                //获得字节数组
                byte[] data = System.Text.Encoding.Default.GetBytes(time_now + " : [" + _pid + "] " + logcontent + "\r\n\r\n");
                //开始写入
                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();
            }
            catch
            {
            }
        }
    }
}
