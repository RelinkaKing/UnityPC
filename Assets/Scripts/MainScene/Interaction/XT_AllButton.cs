using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VesalCommon;
using Assets.Scripts.Public;
using XLua;

[Hotfix]
public class XT_AllButton : MonoBehaviour
{
    public GridLayoutGroup bottomButtonGroup;
    public static XT_AllButton Instance;
    #region 属性
    [Header("MainParms")]
    //public GameObject Cameraback;
    // readonly Color blueColor = new Color(79 / 255f, 186 / 255f, 213 / 255f);
    public bool isExplainHide = true;
    //bool isprint = false;

    [Header("CameraParms")]
    public Camera MainCamera;
   // public Camera Camera;
    //public float CameraRectRatio;
    //bool isStartZoomDownCameraRectWight = false;
    //public Camera UICamera;

    [Header("TopBtnGameObjectGroup")]
    public GameObject BackPanel; // 返回界面  1923-0   Y轴范围
    public GameObject ShareListUI; //分享ui
    public GameObject[] ShareHideBtnGroup; //分享需要隐藏的其他ui组
    public GameObject LeftMenu;
    public GameObject ResetBtn;
    public Transform mouseEye;
    public Transform mouseImage;

    public GameObject SettingPanel;
    [Space(5)]
    public GameObject RightMenu;

    bool isShowLeftMenu = true;
    bool isShowRightMenu = true;

    [Header("ExplainBtnGroup")]
    public GameObject ExplainPanel;
    //public GameObject ExplainSign;
    //public GameObject HideBtn;
    //public GameObject HideOtherBtn;
    //public GameObject TranslucentBtn;
    //public GameObject TranslucentOtherBtn;
    Color blue = new Color(79 / 255f, 186 / 255f, 218 / 255f, 0.5f);

    [Header("BookMarkGroup")]
    public GameObject BookMarkPanel; //书签界面
    public bool isEnterBookMark = false;

    [Header("SignGroup")]
    public GameObject SignModelUi;
    public GameObject MainMenuUI;
    public GameObject SignListPanel; //钉子组
    public GameObject SignListContent; //钉子目录
    public GameObject SignExplain; //钉子解释界面    

    [Header("ModelParent")]
    public GameObject ModelParent;

    [Header("LittleMap")]
    public GameObject littlemapObject;
    public GameObject logo;
    public Color color = new Color(8, 101, 142);

    #endregion


    public static bool isPrintMode;
    public static bool isMultiMode;

    PlayerCommand btnBottomState;
    private void Awake()
    {
        Instance = this;
    }
    [LuaCallCSharp]
    private void OnDestroy()
    {
        Destroy(this);
    }
    [LuaCallCSharp]
    private void Start()
    {
        isPrintMode = false;
        isMultiMode = false;
        isStandard = CompareResolution(); //判定移动设备分辨率是否为9：16

        btnBottomState = new PlayerCommand(1);
        if (bottomButtonGroup!=null) {
            bottomButtonGroup.cellSize = new Vector2(bottomButtonGroup.cellSize.x*Screen.width/1080f, bottomButtonGroup.cellSize.y * Screen.height / 1920f);
        }
    }
    [LuaCallCSharp]
    public void Init_UI()
    {
        //openMu.SetActive(true);
    }
    public void SetBtnColor(GameObject btn_obj, bool isChoose)
    {
        if (isChoose)
            btn_obj.GetComponent<Image>().color = blue;
        else
            btn_obj.GetComponent<Image>().color = Color.white;
    }

    public List<Sprite> help_sprites;
    public void Set_help_sprite(bool isLanScaple)
    {
        if (isLanScaple)
        {
            if (PublicClass.app.app_type == "model")
            {
                helpPanel.GetComponent<Image>().sprite = help_sprites[0];

            }
            else if (PublicClass.app.app_type == "sign")
            {
                helpPanel.GetComponent<Image>().sprite = help_sprites[1];

            }

        }
        else
        {
            if (PublicClass.app.app_type == "model")
            {
                helpPanel.GetComponent<Image>().sprite = help_sprites[2];

            }
            else if (PublicClass.app.app_type == "sign")
            {
                helpPanel.GetComponent<Image>().sprite = help_sprites[3];

            }

        }
    }

    //帮助图片
    public GameObject helpPanel;
    //帮助状态
    bool help = false;
    //开启帮助界面
    public void HelpButton()
    {
        //如果帮助界面未打开
        if (!help)
        {
            help = true;
            //打开帮助图片
            helpPanel.SetActive(true);
        }
        else
        {
            help = false;
            //打开帮助图片
            helpPanel.SetActive(false);
        }
    }
    public void CloseHelp()
    {
        help = false;
        //打开帮助图片
        helpPanel.SetActive(false);
    }


    bool isHide;
    public Sprite[] newBottomUI;

    public bool getHide()
    {
        return SceneModels.instance.get_Hide_State();
    }
    #region 操作按钮
    public void Hide() //显示按钮
    {
        SceneModels.instance.Hide();
    }
    public void Translucent() //半透明
    {
        SceneModels.instance.Fade();
    }
    public void ISO() //单独显示
    {
        SceneModels.instance.ISO();
    }
    public void SetFourBtn(GameObject gameObj )
    {
        Model model=gameObj.GetComponent<Model>();
        if(model.isActive)//显示
        {
            if(model.isSeleted)//选中
            {
                if(model.isTranslucent)//透明
                {
                    //隐藏1，实体1，other 1 other 1
                }
                else//高亮
                {
                    //隐藏1，透明1
                }
            }
        }
        else//隐藏
        {
            //显示1，透明0
        }
    }

    public void SetHideBtnAndTranslucentBtn(string hide_index,bool isClose)
    {
        if (!SceneModels.instance.get_Hide_State())
        {
            GameObject.Find("HideButton").GetComponent<Image>().sprite = newBottomUI[0];
            GameObject.Find("TranslucentButton").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("HideButton").GetComponent<Image>().sprite = newBottomUI[1];
            GameObject.Find("TranslucentButton").GetComponent<Button>().interactable = false;
        }
        if(!SceneModels.instance.get_Tran_State())
        {
            GameObject.Find("TranslucentButton").GetComponent<Image>().sprite = newBottomUI[4];//Resources.Load("Bottom/TranSelect_White") as Sprite;
        }
        else
        {
            GameObject.Find("TranslucentButton").GetComponent<Image>().sprite = newBottomUI[5];//Resources.Load("Bottom/T_ShowSelect") as Sprite;
        }
    }

    public void SetHideAndFadeOtherBtn()
    {
        if(!SceneModels.instance.get_HideOther_State())
        {
            GameObject.Find("HideOther").GetComponent<Image>().sprite = newBottomUI[2];// Resources.Load("Bottom/HideOther") as Sprite;
            GameObject.Find("TranslucentOther").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("HideOther").GetComponent<Image>().sprite = newBottomUI[3];//Resources.Load("Bottom/ShowOther") as Sprite;
            GameObject.Find("TranslucentOther").GetComponent<Button>().interactable = false;
        }
        if (!SceneModels.instance.get_TranOther_State())
        {
            GameObject.Find("TranslucentOther").GetComponent<Image>().sprite = newBottomUI[6];//Resources.Load("Bottom/TranOther_White") as Sprite;
        }
        else
        {
            GameObject.Find("TranslucentOther").GetComponent<Image>().sprite = newBottomUI[7];//Resources.Load("Bottom/T_ShowOther") as Sprite;
        }
    }


    public void HideOther() //显示其他
    {
        SceneModels.instance.HideOthers();
    }

    public void TranslucentOther() //半透明其他
    {
        SceneModels.instance.FadeOthers();
    }

    public void ResetBottomBtn()
    {
        btnBottomState.ReadBottomBtnState();
        Debug.Log(222);
        //if(SceneModels.instance.get_TranOther_State)
    }


    //表征多选功能两种状态的图片，open为进入时候的显示
    public GameObject openMu, closeMu;
    bool isMultiUI = false;
    public  GameObject multiSelectBtn;
    //开关多选列表
    public void ResetMultipleModel() {
        if (openMu!=null && closeMu != null) {
            UIChangeTool.ShowOneObject(openMu, closeMu, false);
        }
        isMultiMode = false;
        isExplainHide = false;
        if (SceneModels.instance == null) return;
        SceneModels.instance.set_Multi_Selection(false);
        SceneModels.instance.CancleSelect();
    }
    //开关多选列表
    public void OpenMultipleModel()
    {

        if (SceneModels.instance == null) return;

        if ( SceneModels.instance.get_Split_mode())  return;

        if (SceneModels.instance.get_Multi_Selection()==true)
        {
            Debug.Log("MultiSelectionClose");
            // UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
            // {
            //     name = "MultiSelectionClose",
            //     callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
            // });

            SceneModels.instance.set_Multi_Selection(false);
            isMultiMode = false;
            isExplainHide = false;
            SceneModels.instance.CancleSelect();
            if (openMu != null && closeMu != null)
            {
                UIChangeTool.ShowOneObject(openMu, closeMu, false);
            }
            //进入多选模式将下方解释窗口置为true
            if(XT_TouchContorl.Instance.expPanel!=null)
            XT_TouchContorl.Instance.expPanel.SetActive(true);
            //multiSelectBtn.GetComponent<Image>().color =Color.white;

        }
        else
        {
            Debug.Log("MultiSelectionOpen");
            // UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
            // {
            //     name = "MultiSelectionOpen",
            //     callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
            // });

            SceneModels.instance.set_Multi_Selection(true);
            isExplainHide = true;
            isMultiMode = true;
            //multiSelectBtn.GetComponent<Image>().color = new Color(1,1, 1, 0.5f);
            if (openMu != null && closeMu != null)
            {
                UIChangeTool.ShowOneObject(openMu, closeMu, false);
            }
           
        }       
    }
    //设置多选ui显示状态
    //public void SwitchMultipleBtnUI(bool flag)
    //{
    //    if (flag)
    //    {
    //        closeMu.SetActive(true);
    //        openMu.SetActive(false);
    //        //ExplainPanel.SetActive(false);
    //    }
    //    if (SceneModels.instance != null)
    //    {
    //        SceneModels.instance.set_Multi_Selection(!flag);
    //    }
    //    //PublicClass.isMultiple = !flag;
    //}

    public bool litteMapIsOpen = true;

    public void SetLittleMapSwitch(bool Switch)
    {
        littlemapObject.SetActive(Switch);
        litteMapIsOpen = Switch;
    }

    void littleMapOpen()
    {
        littlemapObject.SetActive(true);
        litteMapIsOpen = true;
    }
    void littleMapClose()
    {
        littlemapObject.SetActive(false);
        litteMapIsOpen = false;
    }
    #endregion

    #region 主界面菜单按钮
    public void BackMainPanel()
    {
        //BackMarkScenes();
    }
    [LuaCallCSharp]
    public void BackCancel()
    {
        isExitPanelDown=false;
        if (BackPanel==null)
        {
            return;
        }
        iTween.MoveTo(BackPanel, iTween.Hash("position", new Vector3(0, 1930, 0), "islocal", true, "time", 0.3f));
        BackPanel.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        BackPanel.transform.Find("Image").gameObject.SetActive(false);
    }

    public bool  isExitPanelDown=false;
    public void ExitButtonClick()
    {
        isExitPanelDown=true;
        // MoudleController.Instance.touchControl.enabled = false;
        BackPanel.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 59 / 255f);
        BackPanel.transform.Find("Image").gameObject.SetActive(true);
        iTween.MoveTo(BackPanel, iTween.Hash("position", Vector3.zero, "islocal", true, "time", 0.3f));
    }
    public void BookMarkPanelClick() //书签按钮  0 -  -1930
    {
        isEnterBookMark = true;
        PublicClass.currentState = RunState.UI;
        BookMarkPanel.SetActive(true);

        // UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
        // {

        //     name = "CloseDown",
        //     callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
        // });
        //iTween.MoveTo(BookMarkPanel, iTween.Hash("position", Vector3.zero, "islocal", true, "time", 0.3f));
    }
    public void BookMarkPanelCompleteClick() //书签按钮  0 -  -1930
    {
        isEnterBookMark = false;
        PublicClass.currentState = RunState.Playing;
        BookMarkPanel.SetActive(false);
        //iTween.MoveTo(BookMarkPanel, iTween.Hash("position", new Vector3(0, -1930, 0), "islocal", true, "time", 0.3f));
    }

    public void MenuHideBgLClick() //左侧点击的隐藏显示   1423-177  Y轴
    {
        if (isShowLeftMenu)
        {
            LeftMenu.SetActive(false);
        }
        else
        {
            LeftMenu.SetActive(true);
        }
        isShowLeftMenu = !isShowLeftMenu;
    }
    public void MenuHideBgRClick()
    {
        if (isShowRightMenu)
        {
            CloseRightMenu();
        }
        else
        {
            OpenRightMenu();

        }
    }
    // 刷新右侧菜单状态显示
    public void ResetRightMenu()
    {

    }
    public void CloseRightMenu()
    {
        RightMenu.SetActive(false);
        isShowRightMenu = false;
    }
    void OpenRightMenu()
    {
        RightMenu.SetActive(true);
        isShowRightMenu = true;
        iTween.MoveTo(ExplainPanel, iTween.Hash("position", new Vector3(0, -993, 0), "islocal", true, "time", 0.3f));
        littleMapOpen();
        isExplainHide = true;
    }

    //public void ResetAllRightMenu()
    //{
    //    RightMenu[] tempArray = RightMenu.transform.GetComponentsInChildren<RightMenu>();
    //    for (int i = 0; i < tempArray.Length; i++)
    //    {
    //        tempArray[i].ResetUIShow();
    //    }
    //}
    public void ExplainPanelMove() // -706  ,-1031
    {
        if (isExplainHide)
        {
            iTween.MoveTo(ExplainPanel, iTween.Hash("position", new Vector3(0, -752, 0), "islocal", true, "time", 0.3f));
            littleMapClose();
            logo.SetActive(false);
        }
        else
        {
            iTween.MoveTo(ExplainPanel, iTween.Hash("position", new Vector3(0, -1023, 0), "islocal", true, "time", 0.3f));
            littleMapOpen();
            logo.SetActive(true);
        }
        isExplainHide = !isExplainHide;
    }
    public void SetExplainPanelMove(bool flag) // -706  ,-1031
    {
        // print("回退解释框flag "+flag);
        if (flag)
        {
            iTween.MoveTo(ExplainPanel, iTween.Hash("position", new Vector3(0, -752, 0), "islocal", true, "time", 0.3f));
            littleMapClose();
            logo.SetActive(false);
        }
        else
        {
            iTween.MoveTo(ExplainPanel, iTween.Hash("position", new Vector3(0, -1023, 0), "islocal", true, "time", 0.3f));
            littleMapOpen();
            logo.SetActive(true);
        }
        isExplainHide = !isExplainHide;

    }
    //打开设置界面
    public void OpenOrCloseSettingPanel(bool isOpen)
    {
        SettingPanel.SetActive(isOpen);
    }
    bool isshowexplane;
    //操作画笔摄像机开关
    public void EnterOrExitprintingModel(bool isprint)
    {
        isPrintMode = isprint;
        for (int i = 0; i < ShareHideBtnGroup.Length; i++)
        {
            ShareHideBtnGroup[i].gameObject.SetActive(!isprint);
        }
        if (ExplainPanel != null)
        {
            if (isprint)
            {
                isshowexplane = ExplainPanel.activeSelf;
                ExplainPanel.SetActive(false);
            }
            else
                ExplainPanel.SetActive(isshowexplane);

        }
        SendDrawMessage(isprint);
            ShareListUI.gameObject.SetActive(isprint); //开关分享ui
        dramline.instance.OpenOrCloseOperar(isprint); //开关    
        // DebugLog.DebugLogInfo(isprint ? "Enter painting" : "Exit painting");
    }
    [LuaCallCSharp]
    void SendDrawMessage(bool isopen)
    {
        //if (isopen)
        //{
        //    UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
        //    {

        //        name = "openBrush",
        //        callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
        //    });
        //}
        //else
        //{
        //    UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
        //    {

        //        name = "closeBrush",
        //        callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
        //    });
        //}

    }
    #endregion

    #region 钉子界面
    //图谱还原位置和模型按钮
    public void ResetView()
    {
        Camera.main.GetComponent<XT_MouseFollowRotation>().Reset();
        // PublicClass.currentState = RunState.Playing;
    }
    public void ResetModel()
    {
        //分割模式不重置旋转轴
        SceneModels.instance.ResetAllModels(!SceneModels.instance.get_Split_mode());
        SceneModels.instance.isISO = false;
    }

    //打开钉子目录界面
    [LuaCallCSharp]
    public void OpenOrCloseSignContentPanel(bool open)
    {
        SignListPanel.SetActive(open ? true : false);
        SignExplain.SetActive(open ? false : true);
    }

    //显示图钉UI
    public void ShowSignModelUI()
    {
        SignModelUi.SetActive(true);
        MainMenuUI.SetActive(false);
        ExplainPanel.SetActive(false);
        littlemapObject.transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0, 0.24f, 0.135f);
        //主界面ui关闭
    }
    public void CloseSignModelUI()
    {
        //打开主界面  关闭图钉界面
        SignModelUi.SetActive(false);
        MainMenuUI.SetActive(true);

    }
    #endregion

    #region ui 全分辨率适应方案
    [LuaCallCSharp]
    private void SetCameraSize()
    {
        // 9:16
        float standard_width = 1080f;
        float standard_height = 1920f;

        // the screen size
        float device_width = 0f;
        float device_height = 0f;

        device_width = Screen.width;
        device_height = Screen.height;
        float adjustor = 0f;
        float standard_aspect = standard_width / standard_height;
        float device_aspect = device_width / device_height;

        if (device_aspect < standard_aspect)
        {
            adjustor = standard_aspect / device_aspect;
            //camera.orthographicSize = adjustor;
        }
    }
    [LuaCallCSharp]
    private void SetBackgroundSize()
    {
        // the design size
        float standard_width = 1080f;
        float standard_height = 1920f;
        float device_width = Screen.width;
        float device_height = Screen.height;
        if (transform != null)
        {
            float standard_aspect = standard_width / standard_height;
            float device_aspect = device_width / device_height;

            float scale = 0f;

            if (device_aspect > standard_aspect)
            { //按宽度适配
                scale = device_aspect / standard_aspect;
                transform.localScale = new Vector3(scale, 1, 1);
            }
            else
            { //按高度适配
                scale = standard_aspect / device_aspect;
                transform.localScale = new Vector3(1, scale, 1);
            }
        }
    }

    private bool isStandard = true;
    [LuaCallCSharp]
    private bool CompareResolution()
    {
        float standard_width = 1080f;
        float standard_height = 1920f;
        float decive_wight = Screen.width;
        float decive_height = Screen.height;
        float standard_adapter = standard_width / standard_height; //9：16
        float adapter = decive_wight / decive_height;

        //print(standard_adapter+"   "+ adapter);
        return (standard_adapter > adapter || standard_adapter == adapter);
    }
    #endregion
    [LuaCallCSharp]
    public void ExistAtlas()
    {
        CloseSignModelUI();
        Camera.main.GetComponent<XT_AllButton>().littlemapObject.GetComponentInChildren<Camera>().rect = new Rect(0, 0f, 0.24f, 0.135f);
        Camera.main.GetComponent<XT_AllButton>().MainCamera.fieldOfView = 10;
    }

    [LuaCallCSharp]
    public void OpenMouseChooseModel()
    {
        if (mouseImage.gameObject.activeSelf)
        {
            //state open   excute close
            Destroy(GameObject.Find("MouseUI"));
            mouseEye.gameObject.SetActive(true);
            mouseImage.gameObject.SetActive(false);
        }
        else
        {
            GameObject mouseUI = Instantiate(Resources.Load<GameObject>("Prefab/MouseUI"));
            Rect newUIRect = mouseUI.GetComponent<RectTransform>().rect;
            newUIRect.position = Vector2.zero;
            mouseUI.transform.SetParent(GameObject.Find("MainUICanvas").transform);
            mouseUI.transform.localPosition = Vector3.zero;
            mouseUI.transform.localScale = Vector3.one;
            mouseUI.name = "MouseUI";
            mouseUI.transform.SetSiblingIndex(4);
            mouseEye.gameObject.SetActive(false);
            mouseImage.gameObject.SetActive(true);
        }
    }

    public void Quit()
    {
        ExitProgram();
        BackCancel(); //返回平台  

        SendBackMessage(true);
        //UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
        //{
        //   name = "back",
        //   callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
        //});
    }
    public void ClickSearch()
    {
        UnityMessageManager.Instance.SendMessageToRN("clickSearch", null, "");
        Debug.Log("clickSearch");
    }
    [LuaCallCSharp]
    public void SendBackMessage(bool isback)
    {
        if (isback)
        {
           

            Debug.Log("back");
            UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
            {
                 
                name = "back",
                callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
            });
        }
        else
        {
           // StateData.Instance.StatePath.Clear();
            Debug.Log("exit");
            UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
            {
                name = "exit",
                callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
            });
        }
    }

    //返回平台
    public void ExitProgram()
    {
        DebugLog.DebugLogInfo("back SceneSwitch");
        try
        {
            DestroyImmediate(GameObject.Find("temp_parent"));
            ManageModel.Instance.Destory_Transform_temp_child();
        }
        catch (Exception e)
        {
            DebugLog.DebugLogInfo(e.Message);
            DebugLog.DebugLogInfo(e.StackTrace);
        }
        Unity_Tools.ui_return_to_platform();
    }

}