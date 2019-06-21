using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using VesalCommon;
namespace PPTPlayer
{
    /// <summary>
    /// 模型展示控制器
    /// </summary>
    public class ModelController : BaseController
    {
        //单实例
        public static ModelController instance;
        //模型文档信息
        public ModelElement me;
        //public GameObject model;
        //主相机父物体
        public static GameObject emptyBox;
        //主相机
        public static Transform MainCam;
        //模型显示区域背景相机
        public static Camera ModelBgCam;
        //模型操作UI相机
        public static Camera ModelUICam;
        //模型展示Canvas
        public static GameObject m_cvs;
        //标注展示Canvas
        public static GameObject s_cvs;
        //新标注展示Canvas
        public static GameObject sign_new_cvs;
        public static GameObject tmp_sign_new_cvs;
        //小地图相机
        public static Camera LittleMapCam;
        //回滚用临时对象
        public static PlayerCommand playerCommand;
        bool isEnd = false;
        //录屏读取下标
        private int recordUpdate = 0;


        //录屏播放状态
        private bool recordPlay = false;
        //是否无回放
        bool isNoRecord = false;



        public override void Begin()
        {
            if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                try
                {
                    SetModelCam(false);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    Debug.Log(e.StackTrace);
                }
                if (SceneModels.instance != null)
                {
                    if (playerCommand != null)
                    {
                        try
                        {
                            playerCommand.ReadBookMark();
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e.Message);
                            Debug.Log(e.StackTrace);
                        }
                    }
                }
            }


        }
        //上次记录的相机位置参数
        Vector3 lastCamPos = Vector3.zero;
        //上次记录的相机父物体旋转
        Vector3 lastCamParentRot = Vector3.zero;
        public override void Do(float slideTime)
        {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                if (isPageRecording && rd == null)
                {
                    createRecordDoc();
                }
                this.slideTime = slideTime;
                if (isPageRecording)
                {
                    if (emptyBox.transform.rotation.eulerAngles != lastCamParentRot)
                    {
                        addAction(PPTPlayer.RecordAction.cpRot, PublicTools.Vector32Str(emptyBox.transform.rotation.eulerAngles));
                        lastCamParentRot = emptyBox.transform.rotation.eulerAngles;
                    }
                    if (MainCam.position != lastCamPos)
                    {
                        addAction(PPTPlayer.RecordAction.camPos, PublicTools.Vector32Str(MainCam.position));
                        lastCamPos = MainCam.position;
                    }
                }
            }
            else
            {
                if (isEnd)
                {
                    return;
                }
                if (me.appearTime <= slideTime && me.endTime > slideTime)
                {

                    if (SceneModels.instance == null)
                    {
                        Init();
                    }
                    else if (SceneModels.instance != null)
                    {
                        if (!SceneModels.instance.isParentActive())
                        {
                            SceneModels.instance.openTempParent();

                        }
                        if (!isNoRecord && recordPlay)
                        {
                            RecordPlaying(slideTime);
                        }
                    }
                }
                else if (me.endTime <= slideTime && SceneModels.instance != null)
                {
                    SceneModels.instance.closeTempParent();
                }
            }
        }

        public override void GoDie(float slidetime)
        {
            SetModelCam(false);
            if (tmp_sign_new_cvs != null)
            {
                SignNewTest.tmpApp = null;
                tmp_sign_new_cvs.SetActive(false);
                DestroyImmediate(tmp_sign_new_cvs);
            }
            
            if (pac != null)
            {

                pac.ToggleAniObj(false);
                pac.DestroyPPTAniPanel();
                pac = null;
                resetCamera();
                Camera.main.GetComponent<XT_MouseFollowRotation>().uicam = ModelUICam;
            }

            
            if (SceneModels.instance != null)
            {
                SceneModels.instance.closeTempParent();
                SceneModels.instance.destoryTempParent();
            }
            if (playerCommand != null)
            {
                playerCommand = null;
            }
            tmpElement = null;
            isEnd = true;
            Camera.main.GetComponent<ShowSignModel>().DestroyCurrentSign(); 
        }
        public override void DoAnimation(PPTAnimation ani)
        {
            if (ani.shapeId != me.shapeId)
            {
                return;
            }
            if (ani.action == PPTAction.entr)
            {
                if (!isAni)
                {

                    Camera.main.GetComponent<XT_TouchContorl>().GiveUiValue_new(null);
                    if (SceneModels.instance != null)
                    {
                        SceneModels.instance.openTempParent();
                    }
                    if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
                    {

                        me.appearTime = ani.slideTime;
                        instance.tempRecordQueue.Clear();
                        if (m_cvs.activeSelf)
                        {
                            Camera.main.GetComponent<XT_MouseFollowRotation>().To_360();
                        }
                    }
                    SetModelCam(false);
                }
                else
                {

                    if (pac != null)
                    {
                        pac.ToggleAniObj(true);
                    }

                }
            }
            if (ani.action == PPTAction.exit)
            {
                GoDie(0);
                if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
                {

                    if (pac != null)
                    {
                        pac.ToggleAniObj(false);
                        pac.DestroyPPTAniPanel();
                        pac = null;
                        resetCamera();
                        Camera.main.GetComponent<XT_MouseFollowRotation>().uicam = ModelUICam;
                    }

                    me.endTime = ani.slideTime;
                    writeRecordDoc();
                }
            }
            PPTController.isExecuted = true;
        }
        //是否当前页在录制
        bool isPageRecording = false;
        //切换录制开关
        public void ToggleCurrentPageRecord(float slideTime)
        {
            Debug.Log("ToggleCurrentPageRecord MC");
            isPageRecording = !isPageRecording;
            Debug.Log(isPageRecording);
        }

        public override void QuicklyComeIntoBowl(XmlElement element)
        {
            XmlElement currElement = (XmlElement)element.SelectSingleNode("Model[@id='" + me.id + "']");

            if (currElement == null)
            {
                Debug.Log("currElement == NULL !!!!!!!!!!!!1");
                return;
            }
            if (me.appearTime >= me.endTime)
            {
                me.endTime = float.Parse(element.GetAttribute("totalTime"));
            }
            currElement.SetAttribute("appearTime", me.appearTime + "");
            currElement.SetAttribute("endTime", me.endTime + "");
            currElement.RemoveAttribute("shapeId");
        }
        //3D动画面板
        GameObject AniPanel;
        //旧3D动画控制器
        PPTAniPanelControl pac;
        //是旧3D动画
        bool isAni = false;
        //旧vesal.dat路径
        string datPath = string.Empty;
        //加载旧3D动画
        public void loadPPTAni()
        {
            Debug.Log("loadPPTAni");
            string pwd = string.Empty;

            pwd = VesalOldLitdbTools.loadKeyDat(PPTGlobal.PPTPath + "data/key.dat");
            AniPanel = Instantiate(Resources.Load<GameObject>("Prefab/PPT/PPTAniCanvas"));
            pac = AniPanel.GetComponent<PPTAniPanelControl>();
            pac.UICamera.depth = ModelUICam.depth;
            pac.UIBGCamera.depth = ModelBgCam.depth;


            ModelUICam = pac.UICamera;
            ModelBgCam = pac.UIBGCamera;
            InitRect(copy(me));



            pac.initAc(new string[] { datPath, pwd, me.modelId });
            Camera.main.rect = ModelUICam.rect;

            Camera.main.GetComponent<XT_MouseFollowRotation>().uicam = ModelUICam;

            //pac.ToggleAniObj(false);

            PublicClass.currentModle = ModleChoose.AniModel;
        }
        //重置相机索引
        public void resetCamera()
        {
            PPTResourcePool tmp = GameObject.Find("ControllerCanvas").GetComponent<PPTResourcePool>();
            ModelUICam = tmp.ModelUiCamera;
            ModelBgCam = tmp.ModelBgCamera;
            LittleMapCam = tmp.LittleMapCam;
        }

        public override void Init()
        {
            instance = this;
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                isEnd = false;
                Interaction.instance.minDistance = 0.5f;
                resetCamera();
                Camera.main.fieldOfView = 10;

                datPath = PPTGlobal.PPTPath + "data/vesal.dat";
#if UNITY_EDITOR
                // datPath = "C:\\VesalDigital\\PPT\\Data\\vesal.dat";
#endif
                if (File.Exists(datPath))
                {
                    isAni = VesalOldLitdbTools.readLiteDbNoInfo(datPath, me.modelId) == "PPTAnimation";
                }
                else
                {
                    isAni = false;
                }
                if (isAni)
                {

                    loadPPTAni();
                    //    return;
                }
                else
                {
                    me.fileName = pageNum + ".rec";
                    InitRect(copy(me));
                    tempRecordQueue.Clear();
                    try
                    {
                        Camera.main.GetComponent<XT_TouchContorl>().GiveUiValue_new(null);
                        Camera.main.GetComponent<XT_AllButton>().ResetMultipleModel();
                        Camera.main.GetComponent<SplitMode>().closeMode();
                        //Camera.main.GetComponent<SplitMode>().ResetSplitUI();
                        GameObject.Find("ControllerCanvas").GetComponent<PPTResourcePool>().initGridLayout(w, h);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        Debug.Log(e.StackTrace);
                    }

                    PublicClass.currentModle = ModleChoose.MainModel;
                    loadModel();
                    if (isEnd)
                    {
                        return;
                    }
                    if (SceneModels.instance != null)
                    {

                        SceneModels.instance.closeTempParent();
                    }
                }
                
                SetModelCam(false);
                instance.tempRecordQueue.Clear();
                Camera.main.GetComponent<XT_MouseFollowRotation>().Reset360();
                if (isAni || isNewSign)
                {
                    LittleMapCam.rect = new Rect(0, 0, 0, 0);
                }
            }
            else
            {
                base.Init();
                //if (model != null)
                //{
                //    Destroy(model);
                //}
                InitRect(copy(me));

                isEnd = false;
                isNoRecord = false;
                loadModel();
                if (isEnd)
                {
                    return;
                }

                Camera.main.transform.position = new Vector3(0, 0, 20);
                Camera.main.transform.parent.position = Vector3.zero;
                RecordStart();

                //if (isNoRecord)
                //{

                //}
                SetModelCam(false);
                try
                {

                    GameObject.Find("ControllerCanvas").GetComponent<PPTResourcePool>().initGridLayout(w, h);
                    Camera.main.GetComponent<XT_TouchContorl>().GiveUiValue_new(null);
                    Camera.main.GetComponent<XT_AllButton>().ResetMultipleModel();
                    Camera.main.GetComponent<SplitMode>().closeMode();
                    //Camera.main.GetComponent<SplitMode>().ResetSplitUI();
                    if (SceneModels.instance != null)
                    {
                        SceneModels.instance.closeTempParent();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    Debug.Log(e.StackTrace);
                }
            }
            tmpElement = null;
        }

        public override void InitCompent(int pageNum)
        {
            m_cvs.SetActive(true);
            sign_new_cvs.SetActive(false);
            s_cvs.SetActive(false);
            SetModelCam(false);
            this.pageNum = pageNum;
            me.fileName = pageNum + ".rec";

            me.modelId = me.modelId.Replace(".assetbundle", "");

            //加载模型，mark

            if (emptyBox == null)
            {
                emptyBox = Camera.main.transform.parent.gameObject;
                MainCam = Camera.main.transform;



            }

        }
        //SceneModels脚本父物体
        public static GameObject SceneModelsObj;
        bool isNewSign=false;
        public string getType(string modelId) {
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
                return null;
            }
            local_db.Close();
            string result = null;
            
            if (tmpIe.nounType == "1")
            {
                int founded = 0;
                string jsonPath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.LastIndexOf("AppData") + 7) + "/roaming/Vesal/sign/NewAndOldSign.json";
                Dictionary<int, NewAndOldSign> nao = JsonConvert.DeserializeObject<Dictionary<int, NewAndOldSign>>(File.ReadAllText(jsonPath));
                foreach (NewAndOldSign tmpNao in nao.Values)
                {
                    if (modelId == tmpNao.app_id_old && tmpNao.app_id_new != "")
                    {
                        founded = 1;
                        result = "newSign@"+ tmpNao.app_id_new;
                        break;
                    }
                }
                if (founded==0)
                    result = "oldSign@" +tmpIe.nounName ;
            }
            else {
                result = "model";
            }
            return result;
        }
        //加载模型
        public void loadModel()
        {
            try
            {

                string nounName = string.Empty;
                s_cvs.SetActive(false);
                sign_new_cvs.SetActive(false);
                Camera.main.GetComponent<XT_TouchContorl>().enabled = true;
                if (me.NewmodelId != String.Empty)
                    me.modelId = me.NewmodelId;
                else
                    me.NewmodelId = me.modelId;
                string result = getType(me.modelId);
                if (result == null)
                {
                    isEnd = true;
                    return;
                }
                //标注
                if (result != "model")
                {
                    string[] tmpAyyay = result.Split('@');
                    if (tmpAyyay ==null || tmpAyyay.Length!=2) {
                        isEnd = true;
                        return;
                    }
                    m_cvs.SetActive(false);
                    isNewSign = false;
                    if (tmpAyyay[0] == "newSign") {

                        isNewSign = true;
                        me.modelId = tmpAyyay[1];
                    }
                    //Debug.LogError("isNewSign" + isNewSign);
                    if (isNewSign)
                    {
                        Debug.Log("new sigin ....");
                        Camera.main.GetComponent<XT_TouchContorl>().enabled = false;
                        App tmpApp = new App();
                        tmpApp.ab_path= Application.persistentDataPath.Substring(0, Application.persistentDataPath.LastIndexOf("AppData") + 7) + "/roaming/Vesal/sign_ssp_path/SignNewJG.db";
                        tmpApp.app_id = me.modelId;
                        SignNewTest.tmpApp = tmpApp;
                        PublicClass.currentModle = ModleChoose.MainModel;
                        loadSceneModels();
                        if (tmp_sign_new_cvs != null)
                        {
                            DestroyImmediate(tmp_sign_new_cvs);
                        }
                        tmp_sign_new_cvs = Instantiate(sign_new_cvs, sign_new_cvs.transform.parent);
//                        PublicClass.app.app_type = "sign_new";
                        //sign_new_cvs.SetActive(true);
                        tmp_sign_new_cvs.SetActive(true);

                    }
                    else {
                        s_cvs.SetActive(true);
                        PublicClass.currentModle = ModleChoose.SignModel;
                        PublicClass.app = new App();
                        PublicClass.app.struct_name = tmpAyyay[1];
                        Camera.main.GetComponent<ShowSignModel>().Load_ps(me.modelId, "");
                    }
                }
                else
                {
                    m_cvs.SetActive(true);

                    loadSceneModels();
                }

                //throw new Exception("使用新模型加载方法");
            }
            catch (Exception e)
            {
                isEnd = true;
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }

        }
        /// <summary>
        /// 加载场景模型
        /// </summary>
        void loadSceneModels() {
            if (SceneModelsObj != null)
            {
                DestroyImmediate(SceneModelsObj);
            }
            SceneModelsObj = new GameObject("SceneModelsObj");
            SceneModels temp = SceneModelsObj.AddComponent<SceneModels>();

            Debug.Log(me.modelId);
            temp.Init_SceneModels(me.modelId, false);
        }
        float w, h;
        public override void InitRect<T>(T t)
        {
            //相机rect左下角为0，0

            //t.x =  PPTGlobal.Set_WIDTH - (t.x+t.w);
            t.y = PPTGlobal.Set_Height - (t.y + t.h);

            t.x = t.x / PPTGlobal.Set_WIDTH;
            t.y = t.y / PPTGlobal.Set_Height;
            t.h = t.h / PPTGlobal.Set_Height;
            t.w = t.w / PPTGlobal.Set_WIDTH;
            w = t.w * Screen.width;
            h = t.h * Screen.height;
            //Camera.main.rect = new Rect(t.x , t.y , t.w , t.h );
            ModelUICam.GetComponent<Camera>().rect = new Rect(t.x, t.y, t.w, t.h);
            ModelBgCam.GetComponent<Camera>().rect = new Rect(t.x, t.y, t.w, t.h);
            Camera.main.rect = Camera.main.GetComponent<XT_TouchContorl>().getCamRect();
            LittleMapCamRect = new Rect(t.x, (t.y * Screen.height + h * 0.017f) / Screen.height, 0.088f * w / Screen.width, 0.17f * h / Screen.height);
            LittleMapCam.rect = LittleMapCamRect;
            //960 108 768 756
            //base.InitRect(t);
        }
        //小人相机Rect
        Rect LittleMapCamRect;
        public override void Pause()
        {
            if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                if (SceneModels.instance != null)
                {
                    try
                    {
                        SetModelCam(true);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        Debug.Log(e.StackTrace);
                    }
                    try
                    {
                        playerCommand = new PlayerCommand(true);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        Debug.Log(e.StackTrace);
                    }
                }
                    try
                    {
                       playerCommand = new PlayerCommand(true);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        Debug.Log(e.StackTrace);
                    }
            }
        }
        //录制文档子项
        RecordItem tmpElement = null;
        //播放录屏
        private void RecordPlaying(float currentTime)
        {
            if (tempRis == null)
            {
                return;
            }
            if (recordUpdate >= tempRis.Count)
            {
                recordPlay = false;
                tmpElement = null;
                return;
            }

            if (tmpElement == null)
            {
                tmpElement = tempRis[recordUpdate];
            }

            while (currentTime > tmpElement.slideTime)
            {
                try
                {
                    DoRecordAction(tmpElement);
                }
                catch (Exception e)
                {
                    Debug.Log(tmpElement.ra);
                    Debug.Log(tmpElement.raParams);
                    Debug.Log(e.Message);
                    Debug.Log(e.StackTrace);
                }

                //前进至下一次记录
                recordUpdate++;
                if (recordUpdate == tempRis.Count)
                {
                    recordPlay = false;
                    break;
                }
                tmpElement = tempRis[recordUpdate];
            }
        }
        /// <summary>
        /// 回放录制的动作
        /// </summary>
        /// <param name="item"></param>
        public void DoRecordAction(RecordItem item)
        {

            switch (item.ra)
            {
                case RecordAction.camPos:
                    MainCam.transform.position = PublicTools.Str2Vector3(item.raParams);
                    break;
                case RecordAction.cpRot:
                    emptyBox.transform.rotation = Quaternion.Euler(PublicTools.Str2Vector3(item.raParams));
                    break;
                case RecordAction.cancleSelect:
                    SceneModels.instance.CancleSelect();
                    break;
                case RecordAction.ChangeLayer:
                    GameObject.Find("RightMenuManager").transform.Find(item.raParams).GetComponent<RightMenuItem>().ChangeLayer();
                    break;
                case RecordAction.Choose:
                    SceneModels.instance.ChooseModelByName(item.raParams);
                    break;
                case RecordAction.Fade:
                    SceneModels.instance.Fade();
                    break;
                case RecordAction.FadeOthers:
                    SceneModels.instance.FadeOthers();
                    break;
                case RecordAction.Hide:
                    SceneModels.instance.Hide();
                    break;
                case RecordAction.HideOthers:
                    SceneModels.instance.HideOthers();
                    break;
                case RecordAction.reset:
                    Camera.main.GetComponent<XT_AllButton>().ResetBtn.GetComponent<Button>().onClick.Invoke();
                    break;
                case RecordAction.Show:
                    if (item.raParams != null && item.raParams != "")
                    {
                        SceneModels.instance.Show(item.raParams);
                    }
                    else
                    {
                        SceneModels.instance.Show();
                    }
                    break;
                case RecordAction.ShowAll:
                    SceneModels.instance.ShowAll();
                    break;
                case RecordAction.ShowOthers:
                    SceneModels.instance.ShowOthers();
                    break;
                case RecordAction.Splite:
                    SceneModels.instance.SplitModel(item.raParams);
                    break;
                case RecordAction.toggleMs:
                    if (item.raParams.ToLower() == "true")
                    {
                        SceneModels.instance.set_Multi_Selection(true);
                    }
                    else
                    {
                        SceneModels.instance.set_Multi_Selection(false);
                    }
                    break;
                case RecordAction.toggleSp:
                    if (item.raParams.ToLower() == "true")
                    {
                        SceneModels.instance.set_Split_mode(true);
                    }
                    else
                    {
                        SceneModels.instance.set_Split_mode(false);
                    }
                    break;
                default:
                    break;
            }


        }
        //相机视野
        float fieldOfView;
        //录制项的集合
        List<RecordItem> tempRecordQueue = new List<RecordItem>();
        //录制文档
        RecordDoc rd;
        //当前幻灯页时间
        float slideTime;
        /// <summary>
        /// 添加录制动作
        /// </summary>
        /// <param name="recordAction">录制指令</param>
        /// <param name="pms">指令参数</param>
        public static void addAction(RecordAction recordAction, string pms)
        {

            if (instance != null)
            {
                if (!instance.isPageRecording)
                {
                    return;
                }
                instance.tempRecordQueue.Add(new RecordItem
                {
                    slideTime = instance.slideTime,
                    ra = recordAction,
                    raParams = pms
                });
            }
        }
        /// <summary>
        /// 获得RGB参数
        /// </summary>
        /// <param name="xmlEle"></param>
        /// <returns></returns>
        public Color getColor(XmlElement xmlEle)
        {
            float r = float.Parse(xmlEle.GetAttribute("R")) / 255f;
            float g = float.Parse(xmlEle.GetAttribute("G")) / 255f;
            float b = float.Parse(xmlEle.GetAttribute("B")) / 255f;
            return new Color(r, g, b);
        }
        /// <summary>
        /// 设置是否激活相机
        /// </summary>
        /// <param name="active"></param>
        public void SetModelCam(bool active)
        {
            if (ModelUICam != null)
            {
                ModelUICam.enabled = active;
            }
            if (ModelBgCam != null)
            {
                ModelBgCam.enabled = active;
            }
            if (LittleMapCam != null)
            {
                LittleMapCam.enabled = active;
            }
        }

        //开始播放录屏
        private void RecordStart()
        {
            if (tempRis != null)
            {
                tempRis.Clear();
            }
            if (!File.Exists(PPTGlobal.PPTPath + pageNum + "/" + me.fileName))
            {
                isNoRecord = true;
                return;
            }

            //读取录屏信息

            RecordDoc rd = PublicTools.Deserialize<RecordDoc>(PPTGlobal.PPTPath + pageNum + "/" + me.fileName);
            //初始化摄像机位置
            //CamParentPos
            emptyBox.transform.position = PublicTools.Str2Vector3(rd.initCamParentPos);
            emptyBox.transform.rotation = Quaternion.Euler(PublicTools.Str2Vector3(rd.initCamParentRot));
            MainCam.transform.position = PublicTools.Str2Vector3(rd.initCamPos);
            MainCam.transform.rotation = Quaternion.Euler(PublicTools.Str2Vector3(rd.initCamRot));
            tempRis = rd.ris;
            if (tempRis == null)
            {
                tempRis = new List<RecordItem>();
            }
            Camera.main.fieldOfView = rd.fieldOfView != 0 ? rd.fieldOfView : 10;

            Debug.Log("records :" + tempRis.Count);
            recordUpdate = 0;
            recordPlay = true;

            isNoRecord = false;
        }
        /// <summary>
        /// 创建录制文档
        /// </summary>
        public void createRecordDoc()
        {
            rd = new RecordDoc();

            rd.initCamPos = PublicTools.Vector32Str(Camera.main.transform.position);
            rd.initCamRot = PublicTools.Vector32Str(Camera.main.transform.rotation.eulerAngles);
            rd.initCamParentPos = PublicTools.Vector32Str(Camera.main.transform.parent.position);
            rd.initCamParentRot = PublicTools.Vector32Str(Camera.main.transform.parent.rotation.eulerAngles);
            rd.fieldOfView = Camera.main.fieldOfView;
        }
        /// <summary>
        /// 保存录制文档
        /// </summary>
        public void writeRecordDoc()
        {
            Debug.Log("writeRecordDoc");
            if (rd == null)
            {
                return;
            }
            Debug.Log("start save RecordDoc -");
            Debug.Log("start save RecordDoc -" + rd.initCamParentPos);
            rd.ris = this.tempRecordQueue;
            string time = PublicTools.getTime();

            string xmlPath = PPTGlobal.PPTPath + this.pageNum + "/" + me.fileName;
            Debug.Log(xmlPath);
            if (File.Exists(xmlPath))
            {
                File.Move(xmlPath, PPTGlobal.PPTPath + this.pageNum + "/" + time + me.fileName);
            }
            PublicTools.SaveObject(xmlPath, rd);
            Debug.Log("RecordDoc end！");
            rd = null;
        }
        private void Update()
        {
            //if (PPTGlobal.pptStatus == PPTGlobal.PPTStatus.pause && PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer) {
            //    if (LittleMapCam!=null && LittleMapCamRect!=null && LittleMapCam.rect != LittleMapCamRect) {

            //        Debug.Log(LittleMapCamRect);
            //        LittleMapCam.rect = LittleMapCamRect;
            //    }
            //}
        }
        //播放时录制项集合
        public List<RecordItem> tempRis;

    }
    [Serializable]
    [XmlRoot("RecordDoc")]
    public class RecordDoc
    {
        [XmlAttribute("initCamPos")]
        public string initCamPos;
        [XmlAttribute("initCamRot")]
        public string initCamRot;
        [XmlAttribute("initCamParentPos")]
        public string initCamParentPos;
        [XmlAttribute("initCamParentRot")]
        public string initCamParentRot;
        [XmlAttribute("fieldOfView")]
        public float fieldOfView;
        [XmlArray("RecordItems"), XmlArrayItem("RecordItem")]
        public List<RecordItem> ris;
    }
    [Serializable]
    public class RecordItem
    {
        [XmlAttribute("slideTime")]
        public float slideTime;
        [XmlAttribute("RecordAction")]
        public RecordAction ra;
        [XmlAttribute("raParams")]
        public string raParams;
    }
    public enum RecordAction
    {
        //显示
        Show,
        //显示所有
        ShowAll,
        //显示其它
        ShowOthers,
        //选择
        Choose,
        //隐藏
        Hide,
        //隐藏其他
        HideOthers,
        //半透明
        Fade,
        //半透明其它
        ISO,
        //单独显示
        FadeOthers,
        //拆分
        Splite,
        //切换多选
        toggleMs,
        //切换拆分
        toggleSp,
        //右侧菜单,改变层
        ChangeLayer,
        //相机父物体旋转
        cpRot,
        //相机位置移动
        camPos,
        //重置
        reset,
        //取消选择
        cancleSelect,
    }
    class NewAndOldSign
    {
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
}

