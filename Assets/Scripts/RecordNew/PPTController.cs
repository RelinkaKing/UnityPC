using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VesalCommon;
/// <summary>
/// ※微课App & PptPlayer 逻辑控制器
/// </summary>
public class PPTController : MonoBehaviour
{
    //是否暂停状态
    public bool isPasing;
    [Header("Canvas")]
    //顶层Canvas
    public GameObject topCanvas;
    //加载Canvas
    public GameObject loadCanvas;
    //退出Canvas
    public GameObject quitCanvas;
    //画笔Canvas
    public GameObject paintPenCanvas;
    [Header("quit")]
    //退出面板
    public GameObject quitAppPanel;
    //退出提示文本
    public Text quitText;
    //是否开启录制
    public bool isEnableRecord = false;
    //开启录制按钮
    public GameObject RecordButton;

    [Header("Slide")]
    //每页幻灯片对应的时间
    public Dictionary<int, float> slideTotaltimes = new Dictionary<int, float>();
    //当前幻灯片时间轴
    public float slideTime;
    //暂停状态切换
    bool pauseflag = false;
    //累计时间
    private float AccumilatedTime = 0f;
    //每帧刷新时间
    private float FrameLength = 0.04f; //50 miliseconds  
    //帧数统计
    private int GameFrame = 0;
    //幻灯片
    public static int pageNum = 0;
    //当前幻灯片物体
    public static GameObject currentSlideObj;
    //当前幻灯片XML对象
    Slide currentSlide;
    //当前幻灯片播放时间总长
    public float totalTime;
    [Header("Camera")]
    //UI相机物体
    public GameObject UIcameraObj;
    //所有幻灯片父物体
    public Transform basePanel;
    //顶层Canvas Scaler
    CanvasScaler topCanvasScaler;
    [Header("Panel")]
    //微课播放暂停时面板
    public GameObject WeiKePnael;
    //PptPlayer播放暂停时面板
    public GameObject PPTPlayerFunPanel;
    /// <summary>
    /// 隐藏PptPlayer基础按钮
    /// </summary>
    public void HideBaseButton()
    {
        FuntionMenu.SetActive(false);
        rubberButton.SetActive(false);
        FuntionMenuDisableIm.SetActive(false);
        FuntionMenuEnableIm.SetActive(false);
        ParticleDrawLineButtonEnableIm.SetActive(false);
        ParticleDrawLineButtonDisableIm.SetActive(false);
        NLButtonEnableIm.SetActive(false);
        NLButtonDisableIm.SetActive(false);
        enableRecord.SetActive(false);
        disableRecord.SetActive(false);
    }
    /// <summary>
    /// 播放前初始化
    /// </summary>
    public void PPTInit()
    {
        Debug.Log("C PPTInit");
        pauseflag = false;
        GameFrame = 0;
        slideTime = 0;
        pageNum = 0;
        AccumilatedTime = 0;
        totalTime = 0;
        currentSlideObj = null;
        currentSlide = null;
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.WeiKePlayer) {
            quitText.text = "是否退出?";
        }
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            WeiKePnael.SetActive(false);
            DestroyObject(WeiKePnael);
            quitText.text = "是否退出?";
            if (isEnableRecord)
            {
                RecordButton.SetActive(true);
                exportButton.gameObject.SetActive(true);

            }
            else
            {
                RecordButton.SetActive(false);
                exportButton.gameObject.SetActive(false);
            }
            rubberButton.SetActive(false);
            FuntionMenuDisableIm.SetActive(true);
            FuntionMenuEnableIm.SetActive(false);
            ParticleDrawLineButtonEnableIm.SetActive(false);
            ParticleDrawLineButtonDisableIm.SetActive(true);

            NLButtonEnableIm.SetActive(false); ;
            NLButtonDisableIm.SetActive(true); ;

            isDrawNLEnable = false;
            isDrawPLEnable = false;

            enableRecord.SetActive(false);
            disableRecord.SetActive(true);

            slideTotaltimes.Clear();

            ParticleDrawLineButtonEnableIm.transform.parent.gameObject.SetActive(isDrawNLEnable);
            toggleDrawLineEnable();
            //toggleDrawNLEnable();
        }
        else {
            WeiKePnael.SetActive(true);
            PPTPlayerFunPanel.SetActive(false);
            DestroyObject(PPTPlayerFunPanel);
        }

        quitAppPanel.SetActive(false);

        NavigationButtonListener.controller = gameObject;
        topCanvasScaler = topCanvas.GetComponent<CanvasScaler>();
        //topCanvas.GetComponent<CanvasScaler>().scaleFactor = Screen.width / 1920f;
    }
    /// <summary>
    /// 播放结束时重置参数
    /// </summary>
    public void PPTEnd()
    {
        GameFrame = 0;
        slideTime = 0;
    }
    void Start()
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            HideBaseButton();
        }
        else
        {
            PPTInit();
        }
    }
    /// <summary>
    /// 控制器初始化
    /// </summary>
    public void ControllerInit()
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            FuntionMenu.SetActive(false);
            SwitchRight.SetActive(true);
            SwitchLeft.SetActive(true);
        }
        pageNum = 0;
        loadCanvas.SetActive(false);
        changeSlide();
        //mark 添加下方导航
    }
    /// <summary>
    /// 返回上一场景
    /// </summary>
    public void BackToSceneSwitch()
    {
        SceneManager.LoadScene("SceneSwitch");
    }
    /// <summary>
    /// 广播时间轴及动作指令
    /// </summary>
    /// <param name="str"></param>
    /// <param name="isOnlyCurrentSlide"></param>
    public void Broadcast(string str, bool isOnlyCurrentSlide = true)
    {
        if (isOnlyCurrentSlide)
        {
            if (currentSlideObj != null)
            {
                currentSlideObj.BroadcastMessage(str, slideTime, SendMessageOptions.DontRequireReceiver);
                paintPenCanvas.BroadcastMessage(str, slideTime, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            BroadcastMessage(str, slideTime, SendMessageOptions.DontRequireReceiver);
            paintPenCanvas.BroadcastMessage(str, slideTime, SendMessageOptions.DontRequireReceiver);
        }
    }
    /// <summary>
    /// 广播动画指令
    /// </summary>
    /// <param name="ani"></param>
    /// <param name="str"></param>
    /// <param name="isOnlyCurrentSlide"></param>
    public void BroadcastAni(PPTAnimation ani, string str = "DoAnimation", bool isOnlyCurrentSlide = true)
    {
        if (isOnlyCurrentSlide)
        {
            if (currentSlideObj != null)
            {
                currentSlideObj.BroadcastMessage(str, ani, SendMessageOptions.DontRequireReceiver);
                paintPenCanvas.BroadcastMessage(str, ani, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            BroadcastMessage(str, ani, SendMessageOptions.DontRequireReceiver);
            paintPenCanvas.BroadcastMessage(str, ani, SendMessageOptions.DontRequireReceiver);
        }
    }

    //public void SwitchPPTAnimation(int groupId) {

    //    if (currentSlideObj != null)
    //    {
    //        currentSlideObj.BroadcastMessage("StartPPTAnimation", groupId, SendMessageOptions.DontRequireReceiver);
    //        paintPenCanvas.BroadcastMessage("StartPPTAnimation", groupId, SendMessageOptions.DontRequireReceiver);
    //    }

    //}


    //AnimationGroup[] currentGroups;
    //int AnimationGroupIndex = 0;
    //AnimationGroup currentGroup;
    //AnimationGroup nextGroup;

    //当前幻灯片 所有动画Xml对象
    PPTAnimation[] currentPPTAnimations;
    //当前动画指针
    int currentPPTAnimationIndex = 0;
    //当前动画Xml对象
    PPTAnimation currentPPTAnimation;
    //是否动画被执行
    public static bool isExecuted = false;
    //PPTAnimation nextPPTAnimation;

    /// <summary>
    /// 切换幻灯片
    /// </summary>
    void changeSlide()
    {
        bool lastone = false;
        gameObject.GetComponent<PPTResourcePool>().slideSlider.value = pageNum;
        gameObject.GetComponent<PPTResourcePool>().sliderValueChange();
        Time.timeScale = 0;
        GameFrame = 0;
        PPTGlobal.pptStatus = PPTGlobal.PPTStatus.change;
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            VqaTopCanvas.BroadcastMessage("closeVqa", SendMessageOptions.DontRequireReceiver);

            //VqaTopCanvas.SetActive(false);

            if (slideTotaltimes.ContainsKey(pageNum))
            {
                slideTotaltimes.Remove(pageNum);
            }
            slideTotaltimes.Add(pageNum, slideTime);

            if (isPageRecording)
            {
                ToggleCurrentPageRecord();
                return;
            }
            //Broadcast("GoDie");
            currentPPTAnimation = null;
            isExecuted = false;
        }
        slideTime = 0;
        //if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        //{
        //}
        Broadcast("GoDie");
        pageNum++;
        if (pageNum > PPTGlobal.SLIDE_SUM && PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            pageNum = PPTGlobal.SLIDE_SUM;
            lastone = true;
        }
        //nextGroup = null;

        //nextPPTAnimation = null;
        if (pageNum > 0 && pageNum <= PPTGlobal.SLIDE_SUM)
        {

            currentSlideObj = PPTResourcePool.slideObjs[pageNum];
            currentSlide = PPTResourcePool.slides[pageNum];
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {

                if (PPTResourcePool.slides[pageNum].anis != null && PPTResourcePool.slides[pageNum].anis.Length != 0)
                {
                    currentPPTAnimations = PPTResourcePool.slides[pageNum].anis;
                    currentPPTAnimation = currentPPTAnimations[0];
                    currentPPTAnimationIndex = 0;
                }
            }
            else
            {
                totalTime = currentSlide.totalTime;
            }
            //if (currentSlide.animationGroups != null && currentSlide.animationGroups.Length != 0) {
            //    AnimationGroupIndex = 0;
            //    currentGroups = currentSlide.animationGroups;
            //    if (currentGroups[0].type != "Clicked")
            //    {
            //        currentGroup = currentGroups[0];
            //        AnimationGroupIndex++;
            //    }
            //    else {
            //        nextGroup = currentGroups[0];
            //    }
            //}
            Broadcast("Init");
            //UIcameraObj.MoveTo(Vector3.right * PPTGlobal.SLIDE_WIDTH, 1f, 0f);
            Time.timeScale = 1;

            //iTween.MoveTo(basePanel.gameObject, iTween.Hash("islocal", true,"x", -Screen.width * pageNum, "time", 1.5f, "oncomplete", "beginSlide", "oncompletetarget", gameObject));

            //currentEaseType = getRandowEaseType();

            try
            {
                basePanel.gameObject.transform.localPosition = new Vector3(-Screen.width * pageNum, -PPTResourcePool.slidePoss[pageNum].y, basePanel.gameObject.transform.localPosition.z);
                //iTween.MoveTo(basePanel.gameObject, iTween.Hash("islocal", true, "easeType", currentEaseType, "x", -Screen.width * pageNum, "y", -PPTResourcePool.slidePoss[pageNum].y, "time", 2.5f, "oncomplete", "beginSlide", "oncompletetarget", gameObject));
                beginSlide();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }

            if (lastone && PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                PPTGlobal.pptStatus = PPTGlobal.PPTStatus.play;
                StartCoroutine(endTipPanelShow());

            }

        }
        else if (pageNum > PPTGlobal.SLIDE_SUM)
        {
            Time.timeScale = 1;
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                //toggleFuntionPanel();
                //ExitButton();
                pageNum--;
                PPTGlobal.pptStatus = PPTGlobal.PPTStatus.play;
                StartCoroutine(endTipPanelShow());
            }
            else
            {
                //pageNum--;
                PPTGlobal.pptStatus = PPTGlobal.PPTStatus.pause;
                Broadcast("GoDie");
                //PausePlaying();            
                //结束
                ExitButton();
            }
        }
    }
    //放映结束提示面板
    public GameObject endTipPanel;
    /// <summary>
    /// 最后一页播放完时显示提示面板
    /// </summary>
    /// <returns></returns>
    public IEnumerator endTipPanelShow()
    {
        quitCanvas.SetActive(true);
        endTipPanel.SetActive(true);
        yield return null;
        yield return new WaitForSecondsRealtime(2.0f);
        quitCanvas.SetActive(false);
        endTipPanel.SetActive(false);
    }

    //iTween.EaseType currentEaseType;
    //public iTween.EaseType getRandowEaseType()
    //{
    //    iTween.EaseType tmpResult= iTween.EaseType.punch;
    //    while (tmpResult == iTween.EaseType.punch) {
    //        iTween.EaseType[] types = Enum.GetValues(typeof(iTween.EaseType)) as iTween.EaseType[];
    //        int tmp = UnityEngine.Random.Range(0, types.Length);
    //        tmpResult = types[tmp];
    //    }

    //    return tmpResult;
    //}


    //橡皮擦
    public GameObject rubberButton;
    //功能菜单按钮
    public GameObject FuntionMenu;
    public GameObject FuntionMenuEnableIm;
    public GameObject FuntionMenuDisableIm;
    //粒子画线按钮
    public GameObject ParticleDrawLineButton;
    public GameObject ParticleDrawLineButtonEnableIm;
    public GameObject ParticleDrawLineButtonDisableIm;
    //普通画线
    public GameObject NLButtonEnableIm;
    public GameObject NLButtonDisableIm;
    //是否开启普通画线
    public static bool isDrawNLEnable = false;
    //是否开启粒子画线
    public static bool isDrawPLEnable = false;

    /// <summary>
    /// 切换功能面板
    /// </summary>
    public void toggleFuntionPanel()
    {
        if (isDrawPLEnable)
        {
            paintPenCanvas.GetComponent<PPTDrawLineController>().ClearAllLine();
        }

        rubberButton.SetActive(true);
        paintPenCanvas.GetComponent<PPTDrawLineController>().isClear(rubberButton);

        SwitchRight.SetActive(!SwitchRight.activeSelf);
        SwitchLeft.SetActive(!SwitchLeft.activeSelf);
        FuntionMenu.SetActive(!FuntionMenu.activeSelf);
        gameObject.GetComponent<PPTResourcePool>().slideSlider.value = pageNum;
        gameObject.GetComponent<PPTResourcePool>().sliderValueChange();
        PPTPlayerFunPanel.SetActive(!PPTPlayerFunPanel.activeSelf);
        FuntionMenuDisableIm.SetActive(!FuntionMenuDisableIm.activeSelf);
        FuntionMenuEnableIm.SetActive(!FuntionMenuEnableIm.activeSelf);
        topCanvas.SetActive(!topCanvas.activeSelf);
        if (FuntionMenuDisableIm.activeSelf)
        {
            PauseButtonObj.SetActive(false);
            exitButtonObj.SetActive(false);
            goOn();
            Time.timeScale = 1;
        }
        else
        {
            PauseButtonObj.SetActive(true);
            exitButtonObj.SetActive(true);
            PausePlaying();
            Time.timeScale = 0;
        }

    }
    /// <summary>
    /// 开启普通画线
    /// </summary>
    public void toggleDrawLineEnable()
    {
        //ParticleDrawLineButtonEnableIm.SetActive(!ParticleDrawLineButtonEnableIm.activeSelf);
        //ParticleDrawLineButtonDisableIm.SetActive(!ParticleDrawLineButtonDisableIm.activeSelf);
        //isDrawPLEnable = !isDrawPLEnable;


        isDrawPLEnable = false;
        isDrawNLEnable = true;
        ParticleDrawLineButtonEnableIm.transform.parent.gameObject.SetActive(isDrawPLEnable);
        NLButtonEnableIm.transform.parent.gameObject.SetActive(isDrawNLEnable);
    }
    /// <summary>
    /// 开启粒子画线
    /// </summary>
    public void toggleDrawNLEnable()
    {
        //NLButtonEnableIm.SetActive(!NLButtonEnableIm.activeSelf);
        //NLButtonDisableIm.SetActive(!NLButtonDisableIm.activeSelf);
        //isDrawNLEnable = !isDrawNLEnable;

        isDrawPLEnable = true;
        isDrawNLEnable = false;
        ParticleDrawLineButtonEnableIm.transform.parent.gameObject.SetActive(isDrawPLEnable);
        NLButtonEnableIm.transform.parent.gameObject.SetActive(isDrawNLEnable);
    }
    [Header("Switch")]
    //左右切换按钮开关
    public GameObject EnableSwitch;
    public GameObject DisableSwitch;
    //左切换按钮，上一页
    public GameObject SwitchLeft;
    //右切换按钮，下一个动作
    public GameObject SwitchRight;

    /// <summary>
    /// 开关 左右切换按钮
    /// </summary>
    public void toggleSwitchEnable()
    {
        EnableSwitch.SetActive(!EnableSwitch.activeSelf);
        DisableSwitch.SetActive(!DisableSwitch.activeSelf);
        // SwitchLeft.SetActive(EnableSwitch.activeSelf);
        SwitchRight.SetActive(EnableSwitch.activeSelf);
    }
    /// <summary>
    /// 切换至上一页
    /// </summary>
    public void lastSlide() {
        Debug.Log(pageNum);
        if (pageNum >= 1)
        {
            navButtonSelect(Mathf.Max(1, pageNum - 1));
        }
    }
    //长按计时
    private float time;
    // Update is called once per frame
    void Update()
    {
        

    

        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (Input.touchCount == 1)
            {
                time += Time.deltaTime;//当只有一个手指按屏幕时,开始计时
                if (Input.touches[0].phase == TouchPhase.Ended && Input.touches[0].phase != TouchPhase.Canceled)
                {
                    //canceled的意思是系统取消跟踪触摸,如果用户把屏幕放到他的脸上uop超过五个触摸点时的状态
                    time = 0;
                }
            }

            

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.LeftAlt) && PPTGlobal.pptStatus != PPTGlobal.PPTStatus.initial)
            {
                if (!isEnableRecord)
                {
                    isEnableRecord = true;
                    RecordButton.SetActive(true);
                    exportButton.gameObject.SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                //PublicTools.ZipDirectory("D:\\vesalplayer\\asd\\", @"" + exportPath + "test.zip");
                //WriteResult(pageNum);
            }
            if (PPTGlobal.pptStatus == PPTGlobal.PPTStatus.play || PPTGlobal.pptStatus == PPTGlobal.PPTStatus.pause)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    toggleFuntionPanel();
                }
                if (Input.touches != null && Input.touches.Length > 0)
                {
                    if (time > 2f)
                    {
                        time = 0f;
                        toggleFuntionPanel();
                    }
                }
            }

            if (PPTGlobal.pptStatus == PPTGlobal.PPTStatus.play)
            {
                //if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
                //{
                //    //PPTGlobal.pptStatus = PPTGlobal.PPTStatus.pause;
                //    //PausePlaying();

                //}
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ExitButton();
                }


                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Camera.main.GetComponent<XT_MouseFollowRotation>().To_360();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.PageUp))
                {
                    lastSlide();
                }
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.PageDown))
                {

                    //直接跳至下一个click事件
                    nextAni(true);

                }
                else if (currentPPTAnimation != null && currentPPTAnimation.type != PPTActionType.Clicked)
                {

                    nextAni();
                }

                //if (slideTime >= totalTime) {
                //    changeSlide();
                //    return;
                //}

                //Time.deltaTime ~= 0.01
                //Basically same logic as FixedUpdate, but we can scale it by adjusting FrameLength   
                AccumilatedTime = AccumilatedTime + Time.deltaTime;
                slideTime += Time.deltaTime;
                lastExecuteTime += Time.deltaTime;
                //in case the FPS is too slow, we may need to update the game multiple times a frame   
                while (AccumilatedTime > FrameLength)
                {
                    GameFrameTurn();
                    AccumilatedTime = AccumilatedTime - FrameLength;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape))
            {
                ExitButton();
            }
            if (topCanvasScaler != null && topCanvasScaler.scaleFactor != Screen.width / 1920f)
            {

                topCanvasScaler.scaleFactor = Screen.width / 1920f;
            }
            if (PPTGlobal.pptStatus == PPTGlobal.PPTStatus.play)
            {
                if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
                {
                    PPTGlobal.pptStatus = PPTGlobal.PPTStatus.pause;
                    PausePlaying();
                    return;
                }

                if (slideTime >= totalTime)
                {
                    changeSlide();
                    return;
                }
                //Time.deltaTime ~= 0.01
                //Basically same logic as FixedUpdate, but we can scale it by adjusting FrameLength   
                AccumilatedTime = AccumilatedTime + Time.deltaTime;
                slideTime += Time.deltaTime;
                //in case the FPS is too slow, we may need to update the game multiple times a frame   
                while (AccumilatedTime > FrameLength)
                {
                    GameFrameTurn();
                    AccumilatedTime = AccumilatedTime - FrameLength;
                }
            }
        }

    }
    //上一次动画执行时间累计
    float lastExecuteTime = 0;
    //执行动画后清空累计时间
    public void executeAnimation()
    {
        lastExecuteTime = 0;
    }
    public void eventMonitor()
    {

    }
    /// <summary>
    /// 下一个动画
    /// </summary>
    /// <param name="isToClickAni">是否触发至下一个点击动画</param>
    public void nextAni(bool isToClickAni = false)
    {
        if (currentPPTAnimation == null)
        {
            changeSlide();
            return;
        }
        currentPPTAnimation.slideTime = slideTime;
        isExecuted = false;
        if (isToClickAni || lastExecuteTime > currentPPTAnimation.waittime)
        {
            BroadcastAni(currentPPTAnimation);
            lastExecuteTime = 0;
        }



        if (currentPPTAnimation.type == PPTActionType.Clicked)
        {
            isToClickAni = false;
        }
        if (isExecuted || isToClickAni)
        {
            currentPPTAnimationIndex++;
            if (currentPPTAnimations.Length > currentPPTAnimationIndex)
            {
                currentPPTAnimation = currentPPTAnimations[currentPPTAnimationIndex];
            }
            else
            {
                currentPPTAnimation = null;
            }
        }
        isExecuted = false;
        if (isToClickAni)
        {
            nextAni(true);
        }
    }
    /// <summary>
    /// 帧同步，定时执行函数
    /// </summary>
    public void GameFrameTurn()
    {

        Broadcast("Do");
        GameFrame++;
    }
    //暂停按钮
    public GameObject PauseButtonObj;
    //退出按钮
    public GameObject exitButtonObj;
    /// <summary>
    /// 暂停播放
    /// </summary>
    public void PausePlaying()
    {
        gameObject.GetComponent<PPTResourcePool>().slideSlider.value = pageNum;
        gameObject.GetComponent<PPTResourcePool>().sliderValueChange();
        Broadcast("Pause");
        //Time.timeScale = 0;
        PPTGlobal.pptStatus = PPTGlobal.PPTStatus.pause;
        topCanvas.SetActive(true);
        VqaTopCanvas.SetActive(true);
        topCanvas.BroadcastMessage("changeButtonColor", pageNum, SendMessageOptions.DontRequireReceiver);

        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer) {
            if (!PPTPlayerFunPanel.activeSelf) {
                PauseButtonObj.SetActive(false);
                exitButtonObj.SetActive(false);
            }
        }
    }
    /// <summary>
    /// navigation导航按钮选择
    /// </summary>
    /// <param name="i"></param>
    void navButtonSelect(int i)
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (FuntionMenuEnableIm.activeSelf)
            {
                toggleFuntionPanel();
            }
            FuntionMenuDisableIm.SetActive(true);
            FuntionMenuEnableIm.SetActive(false);
        }
        VqaTopCanvas.BroadcastMessage("closeVqa", SendMessageOptions.DontRequireReceiver);
        topCanvas.BroadcastMessage("changeButtonColor", i, SendMessageOptions.DontRequireReceiver);
        pageNum = i - 1;
        topCanvas.SetActive(false);
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            VqaTopCanvas.SetActive(false);
        }
        changeSlide();
    }
    /// <summary>
    /// 开始幻灯片播放
    /// </summary>
    void beginSlide()
    {
        PPTGlobal.pptStatus = PPTGlobal.PPTStatus.play;
    }
    /// <summary>
    /// 开始按钮功能切换
    /// </summary>
    public void startButton() {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            toggleFuntionPanel();
        }
        else {
            goOn();
        }
    }
    //问答Canvas
    public GameObject VqaTopCanvas;
    /// <summary>
    /// 继续播放
    /// </summary>
    public void goOn()
    {
        int tmpNumber = (int)gameObject.GetComponent<PPTResourcePool>().slideSlider.value;
        if (tmpNumber != pageNum)
        {
            navButtonSelect(tmpNumber);
            return;
        }
        VqaTopCanvas.BroadcastMessage("closeVqa", SendMessageOptions.DontRequireReceiver);
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            FuntionMenuDisableIm.SetActive(true);
            FuntionMenuEnableIm.SetActive(false);
            //toggleFuntionPanel();
        }
        else
        {
            VqaTopCanvas.SetActive(false);
            topCanvas.SetActive(false);
        }

        Time.timeScale = 1;
        
        

        if (slideTime == 0 && pageNum == PPTGlobal.SLIDE_SUM + 1)
        {
            pageNum = PPTGlobal.SLIDE_SUM - 1;
            changeSlide();
        }
        else
        {
            Broadcast("Begin");
            //Broadcast("closeVqa");
            PPTGlobal.pptStatus = PPTGlobal.PPTStatus.play;
        }
    }
    public void Exit()
    {

    }
    public void startQaAsk()
    {

    }

    public void OnGUI()
    {
        //GUIStyle bb = new GUIStyle();
        //bb.normal.background = null;    //这是设置背景填充的
        //bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
        //bb.fontSize = 40;       //当然，这是字体大小

        //////居中显示FPS
        ////GUI.Label(new Rect((Screen.width / 2) - 40, 0, 200, 200), "FPS: " + m_FPS, bb);

        //GUI.Label(new Rect((Screen.width / 2) - 100, 0, 200, 200), "reallyTime: " + System.DateTime.Now.Second, bb);
        //GUI.Label(new Rect((Screen.width / 2) - 100, 50, 200, 200), " slideTime: " + slideTime, bb);
        //GUI.Label(new Rect((Screen.width / 2) - 100, 100, 200, 200), PPTGlobal.SLIDE_SUM + 1 == pageNum ? "The End " : string.Format("slide:{0} | totalTime: {1}", pageNum, totalTime), bb);

    }
    /// <summary>
    /// 旧问答返回函数(弃用)
    /// </summary>
    /// <param name="text"></param>
    public void qaBack(Text text)
    {
        if (text.text == "返回")
        {
            ExitQa();
        }
    }
    public void ExitQa()
    {
        Broadcast("endQa");
    }
    /// <summary>
    /// 退出按钮
    /// </summary>
    public void ExitButton()
    {

        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            //FuntionMenuDisableIm.SetActive(true);
            //FuntionMenuEnableIm.SetActive(false);
        }
        PausePlaying();
        topCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        VqaTopCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        quitCanvas.SetActive(true);
        //打开退出界面
        quitAppPanel.SetActive(true);

    }
    /// <summary>
    /// 退出应用
    /// </summary>
    public void exieApp()
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {

            Debug.Log("Application");
            Application.Quit();
        }
    }

    /// <summary>
    /// 确认、取消按钮
    /// </summary>
    /// <param name="isConfirm">是否是确认按钮</param>
    public void ExitChoice(bool isConfirm)
    {
        topCanvas.GetComponent<GraphicRaycaster>().enabled = true;
        VqaTopCanvas.GetComponent<GraphicRaycaster>().enabled = true;
        if (isConfirm)
        //如果是确认按钮
        {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer || PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.WeiKePlayer)
            {
                try {
                    gameObject.BroadcastMessage("PPTEnd");
                    HideBaseButton();
                }
                catch (Exception e) {
                    Debug.Log(e.Message);
                }
                Application.Quit();
                //FuntionMenu.SetActive(false);
                //rubberButton.SetActive(false);

            }
            else
            {
                Camera.main.GetComponent<XT_AllButton>().ExitProgram();
            }
        }

        quitAppPanel.SetActive(false);
        quitCanvas.SetActive(false);
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (!PPTPlayerFunPanel.activeSelf)
            {
                goOn();
            }
        }
    }
    //是否当前页在录制
    bool isPageRecording = false;
    //录制开关
    public GameObject enableRecord;
    public GameObject disableRecord;
    //询问保存面板
    public GameObject askSavePanel;
    /// <summary>
    /// 开关当前页录制
    /// </summary>
    public void ToggleCurrentPageRecord()
    {
        if (!isPageRecording)
        {
            Debug.Log("StartCurrentPageRecord:" + pageNum);
            navButtonSelect(pageNum);
            isPageRecording = true;
        }
        else
        {
            isPageRecording = false;

            if (slideTotaltimes.ContainsKey(pageNum))
            {
                slideTotaltimes.Remove(pageNum);
            }
            slideTotaltimes.Add(pageNum, slideTime);

            StopCurrentPageRecord();
        }
        //paintPenCanvas.BroadcastMessage("ToggleCurrentPageRecord");
        Broadcast("ToggleCurrentPageRecord");
        enableRecord.SetActive(isPageRecording);
        disableRecord.SetActive(!isPageRecording);
    }
    /// <summary>
    /// 停止当前页录制
    /// </summary>
    public void StopCurrentPageRecord()
    {
        toggleFuntionPanel();
        topCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        VqaTopCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        quitCanvas.SetActive(true);
        askSavePanel.SetActive(true);
    }
    /// <summary>
    /// 保存当前页录制
    /// </summary>
    public void SaveCurrentPageRecord()
    {
        Debug.Log("SaveCurrentPageRecord");
        try
        {
            paintPenCanvas.BroadcastMessage("savePoints", pageNum, SendMessageOptions.DontRequireReceiver);
            Broadcast("writeRecordDoc");
            WriteResult(pageNum);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        cancleSaveRecord();
    }
    /// <summary>
    /// 取消当前页录制
    /// </summary>
    public void cancleSaveRecord()
    {
        quitCanvas.SetActive(false);
        askSavePanel.SetActive(false);
        topCanvas.GetComponent<GraphicRaycaster>().enabled = true;
        VqaTopCanvas.GetComponent<GraphicRaycaster>().enabled = true;
    }
    //导出路径
    string exportPath = "C:/vesalplayer/export/";
    //导出按钮
    public Button exportButton;
    //导出时等待文本
    public Text saveWaitPanelText;
    /// <summary>
    /// 开始导出
    /// </summary>
    public void StartExport()
    {
        StartCoroutine(exportRecord());
    }
    /// <summary>
    /// 录制文件导出协程
    /// </summary>
    /// <returns></returns>
    public IEnumerator exportRecord()
    {
        saveWaitPanelText.text = "正在导出录制文件";
        exportButton.interactable = false;
        yield return null;
        try
        {
            List<Slide> tempslides = new List<Slide>();
            SlideDoc tmpDoc = PublicTools.DeepCopy(PPTResourcePool.slideDoc);
            string xmlpath;
            foreach (Slide tempSlide in tmpDoc.slides)
            {
                int tempPageNum = tempSlide.pageNum;
                xmlpath = PPTGlobal.PPTPath + tempPageNum + "/control_" + tempPageNum + ".xml";
                if (File.Exists(xmlpath))
                {
                    try
                    {
                        SlideDoc tempPageDoc = PublicTools.Deserialize<SlideDoc>(xmlpath);
                        tempslides.Add(tempPageDoc.slides[0]);
                    }
                    catch
                    {
                        Debug.Log("格式转换失败" + xmlpath);
                    }

                }
                else
                {
                    //如果此页没有录制，默认保存PPT导出记录
                    tempslides.Add(tempSlide);
                }
            }
            xmlpath = PPTGlobal.PPTPath + "control.xml";
            if (File.Exists(xmlpath))
            {
                File.Delete(xmlpath);
            }
            tmpDoc.slides = tempslides;
            PublicTools.SaveObject(xmlpath, tmpDoc);

            string sourcePath = PPTGlobal.PPTPath.Replace("\\", "/");
            if (sourcePath.EndsWith("/"))
            {
                sourcePath = sourcePath.Substring(0, sourcePath.Length - 1);
            }
            string DirectName = sourcePath.Substring(sourcePath.LastIndexOf("/") + 1);
            Debug.Log(@"EXPORT:" + PPTGlobal.PPTPath);
            if (!Directory.Exists(exportPath)) {
                Vesal_DirFiles.CreateDir(exportPath);
            }
            Vesal_DirFiles.ZipDirectory(PPTGlobal.PPTPath.Substring(0, PPTGlobal.PPTPath.Length - 1), @"" + exportPath + DirectName + ".vsl");
            //Debug.LogError("压缩文件");
            Debug.Log(@"EXPORT:" + exportPath + DirectName + ".vsl");
            //打开文件夹

            OpenWeiKePlayer(exportPath, DirectName);
            //打开播放器加载资源
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
        yield return null;
        exportButton.interactable = true;
        saveWaitPanelText.text = "导出完成！";
        //saveWaitPanel.SetActive(false);
        //quitCanvas.SetActive(false);
    }
    //备份文件夹中vsl文件
    public void BackUpVsl(string path)
    {
        string time = PublicTools.getTime();
        string backPath = path + "backup/";
        if (!Directory.Exists(backPath))
        {
            Directory.CreateDirectory(backPath);
        }
        try
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is FileInfo)
                {
                    File.Move(i.FullName, backPath + time + "_" + i.Name);
                }

            }
            System.Diagnostics.Process.Start(path);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }
    /// <summary>
    /// 打开微课播放器
    /// </summary>
    /// <param name="exportPath">导出路径</param>
    /// <param name="fileName">播放文件名</param>
    public void OpenWeiKePlayer(string exportPath, string fileName)
    {
        //"WeiKePlayer_Data/StreamingAssets/"
        if (File.Exists(exportPath + fileName + ".vsl"))
        {
            foreach (System.Diagnostics.Process pro in System.Diagnostics.Process.GetProcessesByName("WeiKePlayer"))
            {
                pro.Kill();
            }
            //exportPath + DirectName+".vsl"
            String path = Vesal_DirFiles.get_dir_from_full_path(Application.dataPath) + "WeiKePlayer_Data/StreamingAssets/WeiKePlayer/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            BackUpVsl(path);
            try
            {
                File.Copy(exportPath + fileName + ".vsl", path + fileName + ".vsl");
            }
            catch(Exception e) {
                Debug.Log(e.Message);
            }
            string exePath = Vesal_DirFiles.get_dir_from_full_path(Application.dataPath) + "WeiKePlayer.exe";
            Debug.Log(exePath);
            if (File.Exists(exePath))
            {
                System.Diagnostics.ProcessStartInfo ps = new System.Diagnostics.ProcessStartInfo(exePath);
                ps.Arguments = exportPath + fileName + ".vsl";
                
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo = ps;
                p.Start();
                p.WaitForInputIdle();
            }
        }


    }
    /// <summary>
    /// 重放
    /// </summary>
    public void RePlay()
    {
        Debug.Log("RePlay:" + pageNum);

    }
    /// <summary>
    /// 写单页control xml结果
    /// </summary>
    /// <param name="pageNum"></param>
    public void WriteResult(int pageNum)
    {
        string xmlpath = PPTGlobal.PPTPath + pageNum + "/control_" + pageNum + ".xml";
        Debug.Log(xmlpath);
        if (File.Exists(xmlpath))
        {
            Debug.Log("存在旧文件：" + xmlpath.Replace(".xml", "_" + PublicTools.getTime() + ".xml"));
            File.Move(xmlpath, xmlpath.Replace(".xml", "_" + PublicTools.getTime() + ".xml"));
        }

        PPTResourcePool.slideObjs[pageNum].BroadcastMessage("DoSort", SendMessageOptions.DontRequireReceiver);

        SlideDoc tmpDoc = PublicTools.DeepCopy(PPTResourcePool.slideDoc);
        List<Slide> tempslides = new List<Slide>();
        foreach (Slide tempSlide in tmpDoc.slides)
        {
            if (tempSlide.pageNum == pageNum)
            {
                tempslides.Add(PublicTools.DeepCopy(tempSlide));
                break;
            }
        }
        tmpDoc.slides = tempslides;

        PublicTools.SaveObject(xmlpath, tmpDoc);
        clearXml(xmlpath);


        //document.Save(xmlpath);
    }


    /// <summary>
    /// 保存控制信息xml,并清理是需要的信息
    /// </summary>
    /// <param name="xmlpath">文件路径</param>
    public void clearXml(string xmlpath)
    {
        if (!File.Exists(xmlpath))
        {
            return;
        }
        XmlDocument xd = new XmlDocument();
        xd.Load(xmlpath);

        foreach (XmlElement slide in xd.SelectNodes("//Slide"))
        {
            int pageNum = int.Parse(slide.GetAttribute("pageNum"));
            Debug.Log(pageNum);
            List<XmlElement> tmpList = new List<XmlElement>();
            slide.SetAttribute("totalTime", (slideTotaltimes.ContainsKey(pageNum) ? slideTotaltimes[pageNum] : 0f) + "");
            Debug.Log("SAVE clearXml:slideTime:" + slideTime + "   slideTotaltimes DIC:" + (slideTotaltimes.ContainsKey(pageNum) ? slideTotaltimes[pageNum] : 0f));
            foreach (XmlElement element in slide.ChildNodes)
            {
                if (element.Name == "Animtions" || (!element.HasAttributes && !element.HasChildNodes))
                {
                    tmpList.Add(element);
                }

            }

            foreach (XmlElement element in tmpList)
            {
                slide.RemoveChild(element);
            }

            Debug.Log(pageNum + "--" + PPTResourcePool.slideObjs[pageNum].name);
            PPTResourcePool.slideObjs[pageNum].BroadcastMessage("QuicklyComeIntoBowl", slide, SendMessageOptions.DontRequireReceiver);
        }
        foreach (XmlElement slide in xd.SelectNodes("//Animation"))
        {
            slide.RemoveAttribute("type");
            slide.RemoveAttribute("waittime");
            slide.RemoveAttribute("shapeId");

            if (slide.ParentNode.ParentNode.Name != "Video" && slide.ParentNode.ParentNode.Name != "Audio")
            {
                slide.RemoveAttribute("playfrom");
                slide.RemoveAttribute("call");
            }

        }
        xd.Save(xmlpath);
    }
}
