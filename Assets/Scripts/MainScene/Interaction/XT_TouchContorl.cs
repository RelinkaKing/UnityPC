using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XLua;

[Hotfix]
public class XT_TouchContorl : MonoBehaviour
{
    public static XT_TouchContorl Instance;
    [Space(5)]
    //定义光线投射碰撞
    public RaycastHit RayInfo;
    public RaycastHit RayInfo2;
    public RaycastHit[] SignRayInfo;
    private Vector3 _pos = new Vector3();
    private bool _rayResult = false;
    private bool _rayResult2 = false;
    public bool IsClinkSignElement = false;
    public bool IstranslucentOther = false;
    [Space(5)]
    public GameObject littlemapObject;
    [Space(5)]
    //为UI 显示 赋值
    public Text Chinese;
    public Text English;
    public Text Explain;
    // [Space(5)]
    // int SignModelLayerId;
    // int SignUIId;

    [Header("ChooseModel")]
    [SerializeField]
    float dateTime = 0;
    public float dateTimeSecond = 0;
    public float distanceValue; //摄像机视角拉近阈值
    [SerializeField]
    bool isStartTimer = false;

    bool IsStartTimerSecond { get { return isStartTimerSecond; } set { isStartTimerSecond = value; if (isStartTimerSecond) DebugLog.DebugLogInfo("计时"); } }
    bool isStartTimerSecond = false;
    public bool isAtlasModelRotate = false;

    [LuaCallCSharp]
    public void Timer()
    {
        if (isStartTimer)
        {
            dateTime += Time.deltaTime;
        }
        else
        {
            dateTime = 0;
        }
        if (dateTime > 0.6f) { isStartTimer = false; }
    }

    [LuaCallCSharp]
    public void TimerSecond()
    {
        if (isStartTimerSecond)
        {
            dateTimeSecond += Time.deltaTime;
        }
        else
        {
            dateTimeSecond = 0;
        }
        if (dateTimeSecond > 10f)
        {
            isStartTimerSecond = false;
        }
    }

    [SerializeField]
    bool isFirstClick = true;
    Material tempMaterial;
    Color tempColor;

    [LuaCallCSharp]
    public void Awake()
    {
        Instance = this;
    }

    bool isWeiKe = false;
    [LuaCallCSharp]
    private void Start()
    {
        UIcamera = GameObject.Find("UICamera").GetComponent<Camera>();
        if (SceneManager.GetActiveScene().name == "WeiKePlayer" || SceneManager.GetActiveScene().name == "PPTPlayer")
        {
            isWeiKe = true;
        }
        else
        {
            isWeiKe = false;
        }

        //初始化解释切换按钮，解释面板
        if (!PublicClass.isShowExpPanel)
        {
            expPanel.SetActive(false);
        }
        if (PublicClass.isShowExpPanel)
        {
            if(Chinese!=null && Explain!=null && expPanel!=null)
            {
                if (Chinese.text != String.Empty && Explain.text != string.Empty)
                {
                    expPanel.SetActive(true);
                }
            }
        }
        else
        {
            expPanel.SetActive(false);
        }
        if(Explain!=null)
        {
            Explain.text = "";
            lineHeight = Explain.preferredHeight * 0.4f;
        }
        // SignModelLayerId = LayerMask.NameToLayer("Model");
        // SignUIId = LayerMask.NameToLayer("UI");
    }

    [LuaCallCSharp]
    public void Init_UI()
    {
        UI2Texture.gameObject.SetActive(true);
        Explain.gameObject.SetActive(true);
        expPanel.SetActive(true);
    }

    public Camera UI2Camera;
    public RectTransform UI2Texture;
    //是否在ui摄像机范围内
    bool isInModelControlArea = true;

    [LuaCallCSharp]
    public Rect getCamRect()
    {
        float x, y;
        float Canvas_x = UI2Camera.rect.x * Screen.width;
        float Canvas_y = UI2Camera.rect.y * Screen.height;

        float Canvas_width = UI2Camera.rect.width * Screen.width;
        float Canvas_height = UI2Camera.rect.height * Screen.height;

        x = UI2Texture.anchorMin.x * Canvas_width + Canvas_x;
        y = UI2Texture.anchorMin.y * Canvas_height + Canvas_y;
        float w = (UI2Texture.anchorMax.x - UI2Texture.anchorMin.x) * Canvas_width;
        float h = (UI2Texture.anchorMax.y - UI2Texture.anchorMin.y) * Canvas_height;
        return new Rect(x / Screen.width, y / Screen.height, w / Screen.width, h / Screen.height);
    }
    public Camera UIcamera;
    public bool isacuSkin =true;
    [LuaCallCSharp]
    void Update()
    {
        if (PublicClass.appstate != App_State.Running) {
            return;
        }
        // if (CurrentExpModel != null)
        // {
        // if (!(CurrentExpModel.isSeleted && CurrentExpModel.isActive))
        // {
        //     CurrentExpModel = null;
        //     expPanel.SetActive(false);
        //     Chinese.text = "";
        // }
        // }
        if (SceneManager.GetActiveScene().name == "totalScence")
        {
            return;
        }
        if (isWeiKe)
        {

            
            if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer && PPTGlobal.pptStatus != PPTGlobal.PPTStatus.pause)
            {
                return;
            }
        }


            //isInModelControlArea = Camera.main.GetComponent<XT_MouseFollowRotation>().isInUiRect();
            isInModelControlArea = this.GetComponent<XT_MouseFollowRotation>().isInUiRect();
        //图谱模式的点击
        if (isInModelControlArea && PublicClass.currentModle == ModleChoose.SignModel&&isacuSkin)
        {
            //图钉模式的点击
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _rayResult = Physics.Raycast(ray, out RayInfo);
            Ray ray2 = UIcamera.ScreenPointToRay(Input.mousePosition);
            _rayResult2 = Physics.Raycast(ray2, out RayInfo2);
            //当射线与模型碰撞时进入
            if (_rayResult&&!_rayResult2)
            {
                MeshRenderer rayTarget = RayInfo.transform.GetComponent<MeshRenderer>();
                if (rayTarget != null && (!EventSystem.current.IsPointerOverGameObject() || SceneManager.GetActiveScene().name == "WeiKePlayer"))
                {
                    //如果点击鼠标左键，记录按下时鼠标位置
                    if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
                    {
                        // isStartTimerSecond = true;
                        PublicClass.CurrentFingerState = FingerState.SingleFingerDown;
                        _pos = Input.mousePosition;
                    }
                    //如果鼠标抬起时位移很小，则判断为点击操作，将模型选中
                    if (Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
                    {
                        if (dateTimeSecond < 0.3f)
                        {
                            ChooseSignModel(rayTarget.gameObject, TouchRotationModel.fingerTouch);
                        }
                    }
                }
            }
        }

      
            if (isInModelControlArea && PublicClass.currentModle == ModleChoose.MainModel)
            {
                Timer();
                TimerSecond();
                if (SceneModels.instance != null &&
                    ((Input.touchCount >= 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && !dramline.instance.isCanDraw) || (Input.touchCount == 0 && !EventSystem.current.IsPointerOverGameObject()) || isWeiKe))
                {

                    Ray ray = new Ray();
                    try
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                    _rayResult = Physics.Raycast(ray, out RayInfo);

                Ray ray2 = UIcamera.ScreenPointToRay(Input.mousePosition);
                _rayResult2 = Physics.Raycast(ray2, out RayInfo2);
                Ray ray3 = new Ray();
                    RaycastHit UIrayinfo;
                    if(UIcamera!=null)
                      ray3 = UIcamera.ScreenPointToRay(Input.mousePosition);
                //当射线与模型碰撞时进入
                if (_rayResult &&!_rayResult2)
                    {
                    if (isacuSkin)
                    {
                        GameObject rayTarget = RayInfo.transform.gameObject;

                        if (rayTarget != null)
                        {
                            //如果点击鼠标左键，记录按下时鼠标位置
                            if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
                            {
                                PublicClass.CurrentFingerState = FingerState.SingleFingerDown;
                                IsStartTimerSecond = true; //判断是否是单击触控
                                dateTimeSecond = 0;
                                _pos = Input.mousePosition;
                            }
                            else
                            {
                                PublicClass.InitFingerState();
                            }
                            //如果鼠标抬起时位移很小，则判断为点击操作，将模型选中
                            //抬起条件
                            if ((Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)))
                            {
                                if (dateTimeSecond < 0.3f)
                                {
                                    PublicClass.CurrentFingerState = FingerState.SingleFingerSinglePress; //单击
                                    IsStartTimerSecond = false;
                                }
                                if (dateTime < 0.3f && isStartTimer)
                                {
                                    isFirstClick = false; //判断是否第一次点击
                                }
                                if (isFirstClick)
                                {
                                    if (PublicClass.CurrentFingerState == FingerState.SingleFingerSinglePress)
                                    {
                                        SceneModels.instance.ChooseModel(rayTarget);
                                        GiveUiValue_new(rayTarget.GetComponent<Model>());
                                        isStartTimer = true; //开始计时    
                                    }
                                }
                                else
                                {
                                    PublicClass.CurrentFingerState = FingerState.SingleFingerDoublePress; //双击
                                    if (SceneModels.instance.get_Split_mode())
                                    {
                                        SceneModels.instance.ResetModel(rayTarget); //双击重置拆分模型位置
                                    }
                                    else
                                    {
                                        FocusModel(RayInfo.transform.gameObject);
                                        GiveUiValue_new(rayTarget.GetComponent<Model>());
                                    }
                                    isFirstClick = true;
                                }

                            }
                            else
                            {
                                PublicClass.InitFingerState();
                            }
                        }
                    }
                    }
                    else
                    {
                    if (UIcamera != null)
                    {
                        if (_rayResult2)
                            return;
                        if (Physics.Raycast(ray3, out UIrayinfo))
                            return;
                    }
                        if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
                        {
                            PublicClass.CurrentFingerState = FingerState.SingleFingerDown;
                            IsStartTimerSecond = true; //判断是否是单击触控
                            dateTimeSecond = 0;
                            _pos = Input.mousePosition;
                        }
                        else
                        {
                            PublicClass.InitFingerState();
                        }
                        //如果鼠标抬起时位移很小，则判断为点击操作，将模型选中
                        if (Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
                        {

                            if (dateTimeSecond < 0.3f)
                            {
                                DebugLog.DebugLogInfo("空白单机");
                                PublicClass.CurrentFingerState = FingerState.SingleFingerSinglePress; //单击
                                IsStartTimerSecond = false;
                            }
                            if (PublicClass.CurrentFingerState == FingerState.SingleFingerSinglePress)
                            {
                                if (!SceneModels.instance.get_Multi_Selection())
                                {
                                    //取消点击空白处取消选择
                                    SceneModels.instance.CancleSelect();

                                //if (PublicClass.app.app_type == "acu")
                                //    acuTEST.Instance.initacu();


                                    Debug.Log("ClickBlank");
                                    UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
                                    {
                                        name = "ClickBlank",
                                        callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
                                    });
                                }
                            }
                        }
                        else
                        {
                            PublicClass.InitFingerState();
                        }
                    }
                }

            }
        

    }

    [LuaCallCSharp]
    public void ChooseMainModel(GameObject current)
    {
        SceneModels.instance.ChooseModel(current);
        FocusModel(current);
    }

    [LuaCallCSharp]
    public void ChooseMainModel(GameObject[] currents)
    {

    }

    [LuaCallCSharp]
    //聚焦摄像机到具体模型
    public void FocusModel(GameObject current)
    {
        Camera.main.GetComponent<Interaction>().SetTarget(current);

        float dis = Camera.main.GetComponent<Interaction>().distance;
        distanceValue=Interaction.instance.fix_dis;
        if (Camera.main.GetComponent<Interaction>().distance > distanceValue)
        {
            //超过阈值 视角拉近  并移动轴心
            Camera.main.GetComponent<Interaction>().ZoomOpera(distanceValue/1.5f);
        }
        Interaction.instance.ResetCameraPos();
    }

    public Text Sign_Chinese;
    public Text Sign_Note;
    public Text Sign_English;
    public GameObject SignListPanel;
    [LuaCallCSharp]
    public void ChooseSignModel(GameObject current, TouchRotationModel touchmodel)
    {
        try
        {
            SignExplain.SetActive(true);
            littlemapObject.SetActive(false);
            SignListPanel.SetActive(false);
        }
        catch { }
        PublicClass.currentGameobject = current;
        //如果时第一次点击，则上一个model为空
        if (PublicClass.lastGameobject == null)
        {
            PublicClass.lastGameobject = current; //记录  
            tempMaterial = current.transform.GetComponent<MeshRenderer>().material; //记录当前材质
            tempColor = current.transform.GetComponent<MeshRenderer>().material.color;
        }
        else
        {
            PublicClass.lastGameobject.GetComponent<MeshRenderer>().material = tempMaterial;
            PublicClass.lastGameobject.GetComponent<MeshRenderer>().material.color = tempColor;
            PublicClass.lastGameobject = PublicClass.currentGameobject; //更新上一个记录
            tempMaterial = PublicClass.lastGameobject.GetComponent<MeshRenderer>().material;
            tempColor = PublicClass.lastGameobject.GetComponent<MeshRenderer>().material.color;
        }
        MeshRenderer rayTarget = current.transform.GetComponent<MeshRenderer>();

        rayTarget.material.color = new Color(1, 0, 1); // new Color(mainColor.r, mainColor.g, mainColor.b);//

        GiveSignInfo_mobile(current); //加载切片解释
        if (touchmodel == TouchRotationModel.signTouch)
            Camera.main.GetComponent<XT_MouseFollowRotation>().SetSignRotation(PublicClass.currentGameobject.transform); //旋转模型到指定数据  
    }
    [LuaCallCSharp]
    void GiveSignInfo_mobile(GameObject obj)
    {
        Sign_Chinese.text = string.Empty;
        Sign_English.text = string.Empty;
        Sign_Note.text = string.Empty;
        // try
        // {
        TextureIcon go = null;
        if (obj.GetComponentInChildren<TextureIcon>() == null)
        {
            Debug.LogError("无 PS  数据：" + obj.name);
        }
        else
        {
            go = obj.GetComponentInChildren<TextureIcon>();
            Sign_Chinese.text = go.Info.Chinese;// "(" + go.Info.English + ")";
            if (go.Info.English == "" || go.Info.English == null)
            {
                Sign_Chinese.text = go.Info.Chinese;
            }
            Sign_Note.text = go.Info.Note.Trim();
            Sign_English.text = go.Info.English;
        }
        // }
        // catch (Exception e)
        // {
        //     Sign_Chinese.text = string.Empty;
        //     Sign_English.text = string.Empty;
        //     Sign_Note.text = string.Empty;
        //     expPanel.SetActive(false);
        // }
    }
    //注释的背景面板
    public GameObject expPanel;
    public GameObject SignExplain;
    float lineHeight;
    public RectTransform scrollRectTf;
    [LuaCallCSharp]
    public void toggleExpPanel()
    {

        PublicClass.isShowExpPanel = !PublicClass.isShowExpPanel;
        if (!PublicClass.isShowExpPanel)
        {
            expPanel.SetActive(false);
        }
        if (PublicClass.isShowExpPanel)
        {
            if (Chinese.text != String.Empty && Explain.text != string.Empty)
            {
                expPanel.SetActive(true);
            }
        }

    }
    Model CurrentExpModel = null;
    [LuaCallCSharp]
    public void GiveUiValue_new(Model model)
    {
        CurrentExpModel = model;
        if (model == null)
        {
            Explain.text = "";
            Chinese.text = "";
            if (English != null)
            {
                English.text = "";
            }
            expPanel.SetActive(false);
            return;
        }
        if (SceneModels.instance.get_Multi_Selection() || SceneModels.instance.get_Split_mode())
        {
            Explain.text = "";
            Chinese.text = "";
            if (English != null)
            {
                English.text = "";
            }
            //expPanel.SetActive(false);
        }
        else
        {
            try
            {
                //expLinePanel.gameObject.SetActive(false);
                float scrollHeight = scrollRectTf.sizeDelta.y;
                //float linesHeight = expLinePanel.GetComponent<RectTransform>().sizeDelta.y;
                string chinese_name = "";
                string english_name = "";
                string explan_content = "";
                Debug.Log("=================================" + model.name);

                //if (model.isACU)
                //{

                //    DbRepository<acu> local_db2;
                //    local_db2 = new DbRepository<acu>();
                //    local_db2.DataService("vesali.db");
                //    var tmpssp2 = local_db2.SelectOne<acu>((tmp) =>
                //    {
                //        if (tmp.no == model.name)
                //        {
                //            //DebugLog.DebugLogInfo(ss_id.bone_name+" "+ss_id.main_tex+" "+ ss_id.mask_tex);
                //            return true;
                //        }
                //        else
                //        {
                //            return false;
                //        }


                //    });
                //    string name = tmpssp2.guanlian;
                //    var namelist = name.Split(',');
                //    XT_AllButton.Instance.OpenMultipleModel();
                //    for (int i = 0; i < namelist.Length; i++)
                //    {
                //        SceneModels.instance.ChooseModelByName(namelist[i]);
                //    }
                //    XT_AllButton.Instance.ISO();
                //    for (int i = 0; i < namelist.Length; i++)
                //    {
                //        SceneModels.instance.ChooseModelByName(namelist[i]);
                //    }
                //    XT_AllButton.Instance.OpenMultipleModel();


                //}


                if (PublicClass.infoDic.ContainsKey(model.name))
                {
                    if (!model.isSeleted)
                    {
                        //发送消息前，已取消选择
                        UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
                        {
                            name = "ClickBlank",
                            callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
                        });
                    }
                    else
                    {
                        Debug.Log("=" + model.name);
                        ModelInfo tempInfo = PublicClass.infoDic[model.name];
                        chinese_name = tempInfo.Chinese;
                        english_name = tempInfo.English;
                        explan_content = tempInfo.Note.Trim();

                        // ModelInfo tempInfo = PublicClass.infoDic[model.name];

                        JObject data_obj = new JObject();
                        data_obj["littleMap"] = tempInfo.littleMap;
                        data_obj["model_Id"] = tempInfo.model_Id;
                        data_obj["ModelName"] = tempInfo.ModelName;
                        if (String.IsNullOrEmpty(tempInfo.Chinese))
                            data_obj["Chinese"] = "";
                        else
                            data_obj["Chinese"] = ToSixteen(tempInfo.Chinese);

                        if (String.IsNullOrEmpty(tempInfo.English))
                            data_obj["English"] = "";
                        else
                            data_obj["English"] = tempInfo.English;

                        if (String.IsNullOrEmpty(tempInfo.Note))
                            data_obj["Note"] = "";
                        else
                            data_obj["Note"] = ToSixteen(tempInfo.Note);;
                        Debug.Log(data_obj.ToString());
                        UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
                        {
                            name = "model",
                            data = data_obj,
                            callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
                        });
                    }
                }
                if (English == null)
                {
                    Chinese.text = chinese_name + "(" + english_name + ")";
                    Chinese.text = Chinese.text.Replace("()", "");
                }
                else
                {
                    Chinese.text = chinese_name;// + "(" + english_name + ")"; //LoadModel.instance.notes[PublicClass.currentModel.name].chinese;
                    English.text = english_name;
                }

                if (english_name == "" || english_name == null)
                {
                    if (Sign_Chinese != null)
                        Sign_Chinese.text = chinese_name;
                }
                //English.text=english_name ;
                //Explain.text = "";
                //lineHeight = Explain.preferredHeight* 0.4f;

                RestContent(Explain.gameObject.GetComponent<RectTransform>());
                Explain.text = explan_content; //LoadModel.instance.notes[PublicClass.currentModel.name].notes;
                if (PublicClass.isShowExpPanel)
                {
#if UNITY_STANDALONE_WIN
                    if (Explain.text != "") {
                        expPanel.SetActive(true);
                    }
#else
                    expPanel.SetActive(true);
#endif
                }

                if (Explain.text == null || Explain.text == "")
                {
                    // expPanel.SetActive (false);
                }
                if (SceneManager.GetActiveScene().name == "WeiKePlayer")
                {
                    Explain.text = "";
                }
                //showExpPanel();
            }
            catch (Exception e)
            {
                print(e);

                Chinese.text = string.Empty;
                //English.text = string.Empty;
                Explain.text = string.Empty;
                expPanel.SetActive(false);
            }
        }
        if (!SceneManager.GetActiveScene().name.Contains("Player"))
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

#else
            if (expPanel.activeSelf)
            {
                littlemapObject.transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0.12f, 0.24f, 0.135f);
            }
            else
            {
                littlemapObject.transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0, 0.24f, 0.135f);
            }
#endif
        }
    }
    private string ToSixteen(string input)
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

    private string ToMyString(string input)
    {
        string[] hexvaluesplit = input.Split('_');
        string end = string.Empty;
        foreach (string hex in hexvaluesplit)
        {
            int value = Convert.ToInt32(hex, 16);
            string stringvalue = char.ConvertFromUtf32(value);
            char charValue = (char)value;
            end += charValue;
        }
        return end;
    }

    [LuaCallCSharp]
    public void RestContent(RectTransform rt)
    {
        rt.offsetMax = new Vector2(0,0);
    }
    [LuaCallCSharp]
    void GiveSignInfo(GameObject obj)
    {
        TextureIcon go = obj.GetComponentInChildren<TextureIcon>();
        try
        {
            float scrollHeight = scrollRectTf.sizeDelta.y;
            Chinese.text = go.Info.Chinese + "(" + go.Info.English + ")";
            Explain.text = go.Info.Note.Trim();

            if (PublicClass.isShowExpPanel && Explain.text != "")
            {
                expPanel.SetActive(true);
            }
            // if (Explain.text == null || Explain.text == "")
            // {
            //     expPanel.SetActive(false);
            // }
        }
        catch (Exception e)
        {
            Chinese.text = string.Empty;
            Explain.text = string.Empty;
            expPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Destroy(this);
    }
}

public enum TouchRotationModel
{
    fingerTouch,
    signTouch
}