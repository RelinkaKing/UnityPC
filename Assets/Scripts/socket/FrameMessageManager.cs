using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using VesalCommon;
using netcomm;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public enum MESSAGE_TYPE
{
    frame = 0,
    enterc = 1,
    endc = 2
}

public class FrameMessageManager : MonoBehaviour
{
    public Button ControlBtn;
    public Button EndBtn;
    public static FrameMessageManager instance;
    public delegate void RecordDelegate();
    public static event RecordDelegate RecordControlEvent;
    public static event RecordDelegate EndControlEvent;
    public GameObject rawCanvas;
    public GameObject recordBtn;
    public GameObject endBtn;
    public role Program_manager;
    public RenderTextureSendAndReceice render_texture;
    public CheckConnect checkManager;
    float timer = 0;
    bool _send = false;
    bool _end = false;
    bool startControlBtn = false;
    bool startEndBtn = false;
    bool is_start = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        DebugLog.DebugLogInfo("初始化帧传送管理器");
        BtnStartControl();
    }
    
    void Update()
    {
        if (_send)
        {
            timer += Time.deltaTime;
            if (timer > 0.04f)
            {
                Program_manager.SendByteToClient(render_texture.SaveRenderTextureToBytes());
                timer = 0;
            }
        }

        if (_end)
        {
            render_texture.SetFade();
            _end = false;
        }

        if (startControlBtn)
        {
            ControlBtn.interactable = true;
            EndBtn.interactable = false;
            startControlBtn = false;
        }

        if (startEndBtn)
        {
            ControlBtn.interactable = false;
            EndBtn.interactable = true;
            startEndBtn = false;
        }
    }

    public void SendMessage()
    {
        if (Program_manager.role_message == "client")
        {

        }
        else
        {
            Program_manager.SendMessageToClient((byte)1, "start");
        }
    }

    //进入控制模式,录制按钮调用
    public void ControlClient()
    {
        BtnEndControl();
        if (Program_manager.role_message == "client")
        {
            //打卡连接画面
            PublicClass.is_enter_server_control = true;
            render_texture.SetShow();
            //场景配置委托事件
            // serverSceneEvent(true);
        }
        else
        {
            //server 发送
            //JObject o = JObject.FromObject(new Vector2(ScreenData.instance.width, ScreenData.instance.high));
            float ratio = ScreenData.instance.high / (ScreenData.instance.width * 1f);
            //封装对象为字符串进行原生发送
            Program_manager.SendMessageToClient((byte)1,ratio.ToString());
            timer = 0;
            _send = true;
        }
    }

    //退出控制模式
    public void EndControl()
    {
        BtnStartControl();
        if (Program_manager.role_message == "client")
        {
            PublicClass.is_enter_server_control = false;
            ClientEndRecieve();
            if (!InternalSocketMananger.instance.IsWindowShow())
            {
                InternalSocketMananger.instance.HideUnityWidows();
            }
            // serverSceneEvent(false);
        }
        else
        {
            _send = false;
            Program_manager.SendMessageToClient((byte)6, "end");
        }
        //if (SceneManager.GetActiveScene().name == "SceneSwitch")
        //{
        //    InternalSocketMananger.instance.HideUnityWidows();
        //}
    }

    void BtnStartControl()
    {
        startControlBtn = true;
    }
    void BtnEndControl()
    {
        startEndBtn = true;
    }

    //客户端接受消息
    public void ClientResloveMessage(byte cmd, byte[] ma)
    {
        if (Program_manager.role_message == "server")
        return;
        //end
        if (cmd == (byte)6)
        {
            EndClient();
        }//frame
        else if (cmd == (byte)3)
        {
            ClientGetCommandData(ma);
        }//start
        else if (cmd == (byte)1)
        {
            ClientStartEvent(ma);
        }
        else if (cmd == (byte)8)
        {
            //Debug.Log(cmd + "接受到check包");
            //开启一次超时检测
            checkManager.CanGetMessage();
        }
        else
        {
            ClientEndRecieve();
            Debug.Log(cmd + " cmd code error");
        }
    }

    public void ClientStartEvent(byte[] ma)
    {
        Debug.Log("===============get start msg------------------");
        Debug.Log("======get reselution----- " + System.Text.Encoding.UTF8.GetString(ma));
        RecordScreenControl.instance.GetRemoteRatio(ma, true);
        PublicClass.is_enter_server_control = true;
        BtnEndControl();
        render_texture.SetShow();//start show server frame
        is_start = true;
        //InternalSocketMananger.instance.ShowUnityWidows();
    }

    public void ClientGetCommandData(byte[] ma)
    {
        if (is_start == false)
        {
            //RecordScreenControl.instance.GetRemoteRatio(ma, true);
            BtnEndControl();
            //start show server frame
            render_texture.SetShow();
            is_start = true;
            //InternalSocketMananger.instance.ShowUnityWidows();
            PublicClass.is_enter_server_control = true;
        }
        //texture render byte
        if (is_start)
        {
            render_texture.bytes = ma;
        }
    }

    public void EndClient()
    {
        RecordScreenControl.instance.SetRatio();
        BtnStartControl();
        //server end control
        ClientEndRecieve();
        if (!InternalSocketMananger.instance.IsWindowShow())
        {
            InternalSocketMananger.instance.HideUnityWidows();
        }
        PublicClass.is_enter_server_control = false;
    }

    public void ClientEndRecieve()
    {
        _end = true;
    }
}
