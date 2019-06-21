using ICSharpCode.SharpZipLib.Zip;
using LiteDB;
using Newtonsoft.Json;
using PPTPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using VesalCommon;

/// <summary>
/// ※微课App 逻辑入口
/// </summary>
public class PPTResourcePool : MonoBehaviour {
    //加载画面Canvas
    public GameObject loadCanvas;
    //幻灯片物体字典，key 页码
    public static Dictionary<int, GameObject> slideObjs = new Dictionary<int, GameObject>();
    //幻灯片数据字典
    public static Dictionary<int, Slide> slides = new Dictionary<int, Slide>();
    //幻灯片物体在场景中位置字典
    public static Dictionary<int, Vector3> slidePoss = new Dictionary<int, Vector3>();
    //幻灯片数据xml
    public static SlideDoc slideDoc;
    //加载文字提示
    public Text loadingText;
    //幻灯片基础面板
    public Transform basePanel;
    //判断是本地播放器还是导出后运行
    public static bool isLocalPlayer = false;

    //public static Dictionary<string, GameObject> models = new Dictionary<string, GameObject>();
    //linkstart message
    //模型UI背景相机
    public Camera ModelBgCamera;
    //模型UI主相机
    public Camera ModelUiCamera;
    //模型展示canvas
    public GameObject m_cvs;
    //标注展示canvas
    public GameObject s_cvs;
    //新标注展示canvas
    public GameObject sign_new_cvs;
    //小地图相机
    public Camera LittleMapCam;
    //传递播放的文件夹名
    string message = string.Empty;
    [Header("Progress")]
    //进度条面板
    public GameObject progressPanel;
    //进度条控制类
    public ShowProgress sp;
    //是否跳过解压
    public static bool isSkipUnzip = false;
    /// <summary>
    /// 初始化进度条
    /// </summary>
    /// <param name="tittle">加载标题</param>
    public void initProgress(string tittle="") {
        progressPanel.SetActive(true);
        sp.Set_Progress(tittle);
    }
    /// <summary>
    /// 设置进度条
    /// </summary>
    /// <param name="progress">进度</param>
    public void setProgress(float progress) {
        sp.current_progress = progress;
    }
    /// <summary>
    /// 异步解压回调
    /// </summary>
    /// <param name="progress">当前解压进度</param>
    public void unzipProgress(float progress) {
        if (progress < 1f)
        {
            setProgress(progress);
        }
        else {
            StartCoroutine(endUnzip());
        }
    }
    /// <summary>
    /// 结束解压，开始加载器预处理
    /// </summary>
    /// <returns></returns>
    IEnumerator endUnzip() {
        initProgress("正在加载幻灯片");
        yield return null;
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer || PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.WeiKePlayer)
        {
            DirectoryInfo di = new DirectoryInfo(PPTHomePageController.TempFilePath);
            PPTGlobal.PPTPath = di.GetDirectories()[0].FullName;
            string id = System.Guid.NewGuid().ToString("N");
            string renamePath = Vesal_DirFiles.get_dir_from_full_path(PPTGlobal.PPTPath) + id;
            //重命名文件夹，替换中文
            Directory.Move(PPTGlobal.PPTPath, renamePath);
            PPTGlobal.PPTPath = renamePath;
            PPTGlobal.PPTPath = PPTGlobal.PPTPath.Replace("\\", "/");
            if (!PPTGlobal.PPTPath.EndsWith("/"))
            {
                PPTGlobal.PPTPath += "/";
            }
        }
        else
        {
            int pos = PublicClass.app.xml_path.LastIndexOf(".");
            string basePath = "";
            if (pos >=0)
            {
                basePath = PublicClass.app.xml_path.Substring(0,pos);
            }
            
            basePath = basePath +  "/";
            message = Vesal_DirFiles.GetFirstDirInDir(basePath);
            Debug.Log("basePath:" + basePath);

            //Vesal_DirFiles.ExtractAppData(ref basePath, ref message);
            string renamePath = basePath + PublicClass.app.app_id;
            if (basePath + message != renamePath)
            {
                //if (Directory.Exists(renamePath)) {
                //    Directory.Delete(renamePath,true);
                //}
                Directory.Move(basePath + message, renamePath);
                message = PublicClass.app.app_id;
            }
            PPTGlobal.PPTPath = basePath + message + "/";
            Debug.Log(PPTGlobal.PPTPath);
        }
        yield return null;
        //progressPanel.SetActive(false);
        PPTGlobal.pptStatus = PPTGlobal.PPTStatus.initial;
    }
    void Update()
    {
        //PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer && 
        if (PPTGlobal.pptStatus == PPTGlobal.PPTStatus.initial)
        {
            PPTGlobal.pptStatus = PPTGlobal.PPTStatus.loading;
            StartCoroutine(loadResource());
        }
    }
    /// <summary>
    /// 结束播放
    /// </summary>
    public void PPTEnd()
    {

        TopCanvas.SetActive(false);

        loadCanvas.SetActive(true);
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer) {
            homePage.SetActive(true);
        }
        loadingText.text = "";
        PPTGlobal.pptStatus = PPTGlobal.PPTStatus.unziping;
        foreach (GameObject obj in slideObjs.Values)
        {
            Destroy(obj);
        }
        for (int i = 0; i < buttonScrollContent.childCount; i++)
        {
            Destroy(buttonScrollContent.GetChild(i).gameObject);

        }
        slideObjs.Clear();
        slideDoc = null;
        slidePoss.Clear();
        slides.Clear();
    }
    /// <summary>
    /// 微课初始化
    /// </summary>
    public void init()
    {
        PPTGlobal.pptStatus = PPTGlobal.PPTStatus.unziping;
        ModelUiCamera.depth = 21;
        LittleMapCam.depth = 20;
        Camera.main.depth = 20;
        ModelBgCamera.depth = 19;
        initProgress("正在解析资源");
        //yield return null;
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.pc || PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.ios || PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.android)
        {
            Debug.Log("init WeiKe");
            //string basePath = AppOpera.WeiKePlayer_path;
            //if (basePath!=string.Empty) {
            //    Vesal_DirFiles.DelectDir(basePath);
            //}
            //Vesal_DirFiles.CreateDir(basePath + PublicClass.app.app_id + "/");
            //StartCoroutine(Vesal_DirFiles.UnZipAsync(basePath + PublicClass.app.ab_path, basePath + PublicClass.app.app_id + "/", unzipProgress, true));

            unzipProgress(1.0F);
        }
        PPTGlobal.PPTPath = PPTGlobal.PPTPath + message + "/";
        Debug.Log(PPTGlobal.PPTPath);
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer || PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.WeiKePlayer)
        {
            if (isSkipUnzip)
            {
                isSkipUnzip = false;
                unzipProgress(1f);
            }
            else {
                if (PPTHomePageController.TempFilePath!=string.Empty) {
                    Vesal_DirFiles.DelectDir(PPTHomePageController.TempFilePath);
                }
                Debug.Log(PPTHomePageController.tmpSelectPath);
                //PPTHomePageController.TempFilePath = Vesal_DirFiles.get_dir_from_full_path(PPTHomePageController.tmpSelectPath)+ System.Guid.NewGuid().ToString("N");
            
                Debug.Log(PPTHomePageController.TempFilePath);
                StartCoroutine(Vesal_DirFiles.UnZipAsync(PPTHomePageController.tmpSelectPath, PPTHomePageController.TempFilePath, unzipProgress, true));
            }

        }
    }
    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="filepath"></param>
    public void mkdir(string filepath)
    {
        string direName = Path.GetDirectoryName(filepath);
        if (!Directory.Exists(direName))
        {
            Directory.CreateDirectory(direName);
        }
    }
    //临时文件夹路径
    string TempFilePath;
    public void OnEnable()
    {
        if (homePage != null)
        {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                homePage.SetActive(true);
            }
            else
            {
                homePage.SetActive(false);
            }
        }

        loadCanvas.SetActive(true);
        //手机常亮
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //横屏显示
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer) {
            slideSliderObj = slideSliderObj2;
            slideSlider = slideSlider2;
            sliderText = sliderText2;
            sliderBottomText = sliderBottomText2;
        }
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer && PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.WeiKePlayer) {
            
            PPTInit();
        }
    }
    /// <summary>
    /// 场景初始化
    /// </summary>
    public void PPTInit() {

        Debug.Log("R PPTInit");
        //Screen.fullScreen = true;
        if (homePage != null) {
         
                homePage.SetActive(false);
            
        }
        Time.timeScale = 1;
        //StartCoroutine(init());
        init();
        //PPTGlobal.pptStatus = PPTGlobal.PPTStatus.loading;
        //StartCoroutine(loadResource());

    }
    //PptPlayer首页
    public GameObject homePage;
  
    //顶层Canvas
    public GameObject TopCanvas;
    //问答Canvas
    public GameObject VqaTopCanvas;
    //画笔Canvas
    public GameObject paintPenCanvas;
    //底部导航按钮父物体
    public Transform buttonScrollContent;
    //幻灯片物体位置y轴偏移值
    float yDis = 0;

    //页选择滑条组件需要物体，PptPlayer与微课播放时位置不同
    [Header("Slider")]
    public GameObject slideSliderObj;
    public Slider slideSlider;
    public Text sliderText;
    public Text sliderBottomText;
    public float sliderNumber;
    [Header("Slider2_WeiKePanel")]
    public GameObject slideSliderObj2;
    public Slider slideSlider2;
    public Text sliderText2;
    public Text sliderBottomText2;
    /// <summary>
    /// 幻灯片页切换，滑条文本显示刷新
    /// </summary>
    public void sliderValueChange()
    {
        sliderText.text = slideSlider.value+"";
        sliderBottomText.text = slideSlider.value + " 页";
    }
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <returns></returns>
    public IEnumerator loadResource() {
        //int n = 0;
        //while (n++<500) {
        //    yield return null;
        //}
        initProgress("正在加载幻灯片");
        slideSlider.maxValue = 1;
        slideSlider.minValue = 1;
        slideSlider.value = 1;
        yDis = 0;
        if (buttonScrollContent.childCount !=0) {
            TopCanvas.SetActive(true);
            //buttonScrollContent.BroadcastMessage("DestroyButton");
            TopCanvas.SetActive(false);
            yield return null;
        }
        if (basePanel.childCount!=0) {
            for (int i = 0;i< basePanel.childCount;i++) {
                Destroy(basePanel.GetChild(i).gameObject);
            }
            yield return null;
        }
        slideObjs.Clear();
        slides.Clear();
        slidePoss.Clear();
        //读取xml加载资源
        if (readXml())
        {
            //Debug.Log(slideDoc.createTime);
            PPTGlobal.SLIDE_SUM = slideDoc.slides.Count;
            PPTGlobal.Set_Height = slideDoc.height;
            PPTGlobal.Set_WIDTH = slideDoc.width;
            slideSlider.maxValue = PPTGlobal.SLIDE_SUM;
            for (int i = 0; i < PPTGlobal.SLIDE_SUM; i++)
            {
                int index = i + 1;
                //loadingText.text = string.Format("第{0}/{1}幻灯片加载中...... \n Tips:暂停时可以控制模型旋转和移动哦！！！", i + 1, PPTGlobal.SLIDE_SUM);
                
                yield return null;
                GameObject navButton = Instantiate(Resources.Load<GameObject>("Prefab/PPT/PPTNavigationButton"), buttonScrollContent);
                NavigationButtonListener tmpListener = navButton.GetComponent<NavigationButtonListener>();
                navButton.transform.localPosition = Vector3.zero;
                navButton.transform.localScale = Vector3.one;
                navButton.transform.localRotation = Quaternion.Euler(Vector3.zero);

                tmpListener.index = index;
                //tmpListener.OnClick += delegate (GameObject gb)
                //{
                //    this.SelectButton(gb);
                //};
                navButton.name = index + "";
                Slide slide = slideDoc.slides[i];
                navButton.GetComponentInChildren<Text>().text = slide.slideName;
                //navButton.transform.SetParent(buttonScrollContent);
                GameObject slideObj = Instantiate(Resources.Load<GameObject>("Prefab/PPT/Slide"), basePanel);
                int tmp = UnityEngine.Random.Range(0, 3);
                switch (tmp)
                {
                    case 0:
                        yDis += Screen.width;
                        break;
                    case 1:
                        yDis -= Screen.width;
                        break;
                    default:
                        break;
                }

                Vector3 tmpV3 = new Vector3(Screen.width * index, yDis, 0);
                slideObj.transform.localPosition += tmpV3;

                slidePoss.Add(slide.pageNum, tmpV3);
                slideObj.name = "slide_" + (index);
                //初始化slide
                initSlide(slideObj, slide);

                slideObjs.Add(slide.pageNum, slideObj);
                slides.Add(slide.pageNum, slide);
                slideObj.BroadcastMessage("InitCompent", slide.pageNum, SendMessageOptions.DontRequireReceiver);
                if (PPTGlobal.SLIDE_SUM > 0)
                {
                    setProgress(((float)index / (float)PPTGlobal.SLIDE_SUM));
                }
            }
            
            setProgress(1f);
            yield return null;
            yield return new WaitForSecondsRealtime(0.01f);
            SendMessage("ControllerInit");
            progressPanel.SetActive(false);
            //yield return null;
        }
        else {
            try {
                if (File.Exists(PublicClass.WeiKePlayer_path + PublicClass.app.ab_path)) {
                    File.Delete(PublicClass.WeiKePlayer_path + PublicClass.app.ab_path);
                }
            }
            catch (Exception e) {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
            try { 
                gameObject.GetComponent<PPTController>().GetComponent<PPTController>().ExitChoice(true);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
    }
    }
    
    public void InitAnimationGroup(GameObject slideObj, PPTAnimation[] anis) {

    }
    /// <summary>
    /// 读取xml，加载资源
    /// </summary>
    /// <returns></returns>
    public bool readXml() {
        string xmlpath = string.Empty;
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            xmlpath = PPTGlobal.PPTPath + "ppt_control.xml";
        }
        else {
            xmlpath = PPTGlobal.PPTPath + "control.xml";
        }
        //Debug.Log(xmlpath);
        if (File.Exists(xmlpath))
        {
            try
            {
                slideDoc = PublicTools.Deserialize<SlideDoc>(xmlpath);
            }
            catch
            {
                Debug.Log("格式转换失败"+xmlpath);
            }
            return true;
        }
        else {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                Debug.Log("获取文件失败：" + xmlpath);
            }
            else {
                loadingText.text = "获取文件失败：" + xmlpath;
                Debug.Log(loadingText.text);
            }
            return false;
        }
    }
    //记录加载出的MP4路径，退出时销毁
    List<string> loadMp4s = new List<string>();
    //按页初始化幻灯片
    public void initSlide(GameObject slideObj, Slide slide) {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer) {
            Debug.Log(slide.pageNum);
            String datapath = PPTGlobal.PPTPath + "data/vesal.dat";
#if UNITY_EDITOR
            // datapath = "C:\\VesalDigital\\PPT\\Data\\vesal.dat";
#endif
            //判断vesal.dat存在，则解析出所有PPTAnimation2D需要的视频，添加VideoElement
            if (File.Exists(datapath) && slide.me!=null) {
                string result = VesalOldLitdbTools.readLiteDbNoInfo(datapath, slide.me.modelId);
                if (result == "PPTAnimation2D")
                {
                    List<VideoElement> videos = new List<VideoElement>();
                    videos.AddRange(slide.videos);
                    ElementBaseClass bc = slide.me;
                    VideoElement ve = new VideoElement();
                    ve.appearTime = bc.appearTime;
                    ve.endTime = bc.endTime;
                    ve.shapeId = bc.shapeId;
                    ve.fileName = slide.me.modelId + ".mp4";
                    ve.w = bc.w;
                    ve.h = bc.h;
                    ve.x = bc.x;
                    ve.y = bc.y;
                    ve.isLoop = true;
                    ve.isPlayAudio = true;
                    ve.id = slide.videos.Length;
                    VesalOldLitdbTools.get2d(datapath, VesalOldLitdbTools.loadKeyDat(PPTGlobal.PPTPath + "data/key.dat"), slide.me.modelId, PPTGlobal.PPTPath+ slide.pageNum);
                    loadMp4s.Add(PPTGlobal.PPTPath + slide.pageNum+"/"+slide.me.modelId+".mp4");
                    videos.Add(ve);
                    videos.Sort();
                    slide.videos = videos.ToArray();
                    slide.me = null;
                }
            }
        }

        if (slide.bgs != null && slide.bgs.Length != 0)
        {
            //初始化背景元素
            initBg(slideObj, slide.bgs);
        }
        if (slide.ies != null && slide.ies.Length != 0)
        {
            //初始化图片元素
            initImages(slideObj, slide.ies);
        }
        if (slide.videos != null && slide.videos.Length != 0)
        {
            //初始化视频元素
            initVideo(slideObj, slide.videos);
        }
        if (slide.audios != null && slide.audios.Length != 0)
        {
            //初始化音频元素
            initAudio(slideObj, slide.audios);
        }


        ////if (slide.qas != null && slide.qas.Length != 0)
        ////{
        ////    initQas(slideObj, slide.qas);
        ////}
        if (slide.vqas != null && slide.vqas.Length != 0)
        {
            //初始化问答元素
            initVqas(slideObj, slide.vqas);
        }
        if (slide.me != null)
        {
            //初始化模型元素
            initModel(slideObj, slide.me);
        }

        //if (slide.ppes != null && slide.ppes.Length != 0) {
        //    initPpes(slideObj,slide.ppes);
        //}
    }
    //模型展示UI的GridLayoutGroup(两侧按钮集合)
    public List<GridLayoutGroup> glgs;
    //记录GridLayoutGroup初始大小
    Dictionary<string, SizeRecord> glgSizes;
    class SizeRecord
    {
        public float x;
        public float y;
    }
    //弃用
    float glgCellSizeX = 0;
    float glgCellSizeY = 0;
    //初始化Grid大小
    public void initGridLayout(float w, float h)
    {
        float x = w / 1920f;
        float y = h / 1080f;
        if (glgSizes == null)
        {
            glgSizes = new Dictionary<string, SizeRecord>();
            
            foreach (GridLayoutGroup glg in glgs)
            {
                glgSizes.Add(glg.name, new SizeRecord
                {
                    x = glg.cellSize.x,
                    y = glg.cellSize.y
                });
            }
        }
        foreach (GridLayoutGroup glg in glgs)
        {
            float tmpWidth = glgSizes[glg.name].y * y;
            //float tmpWidth = Mathf.Min(glgSizes[glg.name].x * x, glgSizes[glg.name].y * y);
            glg.cellSize = new Vector2(tmpWidth,tmpWidth);
        }
    }
    //初始化画笔元素，弃用
    public void initPpes(GameObject slideObj, PaintPenElement[] ppes)
    {
        foreach (PaintPenElement ppe in ppes)
        {
            GameObject ppeObj = new GameObject("ppe_"+ppe.id);
            ppeObj.transform.SetParent(slideObj.transform);
            ppeObj.transform.SetAsFirstSibling();
            PaintPenController tmpPpc = ppeObj.AddComponent<PaintPenController>();
            tmpPpc.ppe = ppe;
        }
    }
    //初始化图片元素
    public void initImages(GameObject slideObj, ImageElement[] ies)
    {
        foreach(ImageElement ie in ies) {
            GameObject image = Instantiate(Resources.Load<GameObject>("Prefab/PPT/ImageElement"), slideObj.transform);
            image.name = "image_" + ie.id;
            ImageController ic = image.GetComponent<ImageController>();
            ic.ie = ie;
        }
    }
    //初始化模型元素
    public void initModel(GameObject slideObj, ModelElement me)
    {
        GameObject model = new GameObject("model");
        model.transform.SetParent(slideObj.transform);
        ModelController mct = model.AddComponent<ModelController>();
        mct.me = me;
        ModelController.ModelUICam = ModelUiCamera;
        ModelController.LittleMapCam = LittleMapCam;
        ModelController.ModelBgCam = ModelBgCamera;
        ModelController.m_cvs = m_cvs;
        ModelController.s_cvs = s_cvs;
        ModelController.sign_new_cvs = sign_new_cvs;
    }
    //初始化问答
    public void initVqas(GameObject slideObj, VqaElement[] vqas)
    {
        foreach (VqaElement tmpVqa in vqas) {
            if (tmpVqa.style == "default") {
                GameObject vqa = Instantiate(Resources.Load<GameObject>("Prefab/PPT/Vqa"), slideObj.transform);
                vqa.name = "Vqa_"+tmpVqa.id;
                VqaController vqac = vqa.GetComponent<VqaController>();
                vqac.currentVqa = tmpVqa;
                //vqac.initVqaDic(vqas);
            }
        }
    }

    //public void initQas(GameObject slideObj, QaElement[] qas)
    //{
    //    GameObject qa = new GameObject("qa");
    //    qa.transform.SetParent(slideObj.transform);
    //    QaController qac= qa.AddComponent<QaController>();
    //    qac.initQaDic(qas);
    //}

    //初始化声音元素
    public void initAudio(GameObject slideObj, AudioElement[] aes) {
        foreach (AudioElement ae in aes)
        {
            GameObject AudioSourceCompent = Instantiate(Resources.Load<GameObject>("Prefab/PPT/AudioSourceCompent"), slideObj.transform);
            AudioSourceCompent.name = "Audio_" + ae.id;
            AudioSourceCompent.GetComponent<AudioController>().currentAe = ae;
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                GameObject AudioSlider = Instantiate(Resources.Load<GameObject>("Prefab/PPT/AudioSlider"), slideObj.transform);
                AudioSourceCompent.GetComponent<AudioController>().audioSlider = AudioSlider.GetComponent<AudioSlider>();
                AudioSlider.SetActive(false);
            }
        }

    }

    //初始化视频元素
    public void initVideo(GameObject slideObj, VideoElement[] ves) {
        foreach (VideoElement ve in ves) {
            GameObject videoCompent = Instantiate(Resources.Load<GameObject>("Prefab/PPT/VideoCompent"), slideObj.transform);
            videoCompent.name = "video_"+ve.id;
            videoCompent.GetComponent<VideoController>().ve = ve;
        }
    }
    //初始化背景元素
    public void initBg(GameObject slideObj, BackGroundElement[] bgs)
    {
        Transform tmpBg = slideObj.transform.Find("BackGround");
        foreach (BackGroundElement bg in bgs)
        {
            GameObject bgCompent = new GameObject("bg_"+bg.id);
            bgCompent.transform.parent = slideObj.transform;
            BgController bgc= bgCompent.AddComponent<BgController>();
            bgc.currentBge = bg;
            bgc.background = tmpBg;
        }
       
    }
    //反序列化xml
    public static T Deserialize<T>(string xmlpath) where T : class
    {
        //去Bom
        Encoding utf8NoBom = new UTF8Encoding(false);
        using (StringReader sr = new StringReader(File.ReadAllText(xmlpath, utf8NoBom)))
        {
            XmlSerializer xmldes = new XmlSerializer(typeof(T));
            return (T)xmldes.Deserialize(sr);
        }
    }
    //获取vesal.dat所在路径
    public static string getDataUrl() {
        string tmpUrl;
        if (PPTResourcePool.isLocalPlayer)
        {
            tmpUrl = PPTGlobal.Vesal_Data_Path;
        }
        else
        {
            tmpUrl = PPTGlobal.PPTPath + "\\data\\";
        }
        tmpUrl += "vesal.dat";
        return tmpUrl;
    }
    public void OnDestroy()
    {
        //退出时删除mp4
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer) {
            if (loadMp4s.Count!=0) {
                foreach (string file in loadMp4s) {
                    if (File.Exists(file)) {
                        File.Delete(file);
                    }
                }
            }
        }
    }
}
[Serializable]
[XmlRoot("SlideDoc")]
public class SlideDoc
{
    [XmlAttribute("name")]
    public string name;
    [XmlAttribute("width")]
    public float width;
    [XmlAttribute("height")]
    public float height;
    [XmlAttribute("createTime")]
    public string createTime;
    [XmlElement("Slide")]
    public List<Slide> slides;
}
[Serializable]
public class Slide {
    [XmlAttribute("SlideName")]
    public string slideName;
    [XmlAttribute("pageNum")]
    public int pageNum;
    [XmlAttribute("totalTime")]
    public float totalTime;
    [XmlArray("BackGrounds"), XmlArrayItem("BackGround")]
    public BackGroundElement[] bgs = null;
    [XmlArray("Videos"), XmlArrayItem("Video")]
    public VideoElement[] videos = null;
    [XmlArray("Audios"), XmlArrayItem("Audio")]
    public AudioElement[] audios = null;
    //[XmlArray("Qas"), XmlArrayItem("Qa")]
    //public QaElement[] qas = null;
    [XmlArray("Qas"), XmlArrayItem("Qa")]
    public VqaElement[] vqas = null;
    [XmlArray("Images"), XmlArrayItem("Image")]
    public ImageElement[] ies = null;
    [XmlElement("Model")]
    public ModelElement me;
    [XmlArray("PaintPens"), XmlArrayItem("PaintPen")]
    public PaintPenElement[] ppes;

    [XmlArray("Animtions"), XmlArrayItem("Animation")]
    public PPTAnimation[] anis;
    //[XmlArray("AnimationGroups"), XmlArrayItem("AnimationGroup")]
    //public AnimationGroup[] animationGroups;
}
//[Serializable]
//public class AnimationGroup {
//    [XmlAttribute("id")]
//    public int id;
//    [XmlAttribute("shapeId")]
//    public int shapeId;
//    [XmlAttribute("action")]
//    public int action;
//    [XmlAttribute("type")]
//    public string type;
//    [XmlElement("Animation")]
//    public List<PPTAnimation> PPTAnimations;
//}
[Serializable]
public class PPTAnimation : IComparable
{
    public int CompareTo(object obj)
    {
        PPTAnimation other = obj as PPTAnimation;
        if (other == null) return 1;
        return other.id.CompareTo(id)*-1;
    }

    [XmlAttribute("id")]
    public int id;
    [XmlAttribute("shapeId")]
    public int shapeId;
    [XmlAttribute("action")]
    public PPTAction action;
    [XmlAttribute("type")]
    public PPTActionType type;
    [XmlAttribute("waittime")]
    public float waittime;
    [XmlAttribute("playfrom")]
    public float playfrom;
    [XmlAttribute("call")]
    public PPTCall call;
    [XmlAttribute("slideTime")]
    public float slideTime;
}
public enum PPTCall
{
    play,
    togglePause,
    stop
}

public enum PPTActionType
{
    Clicked,
    noClick
}
public enum PPTAction{
    changeSlide,
    entr,
    exit,
    emph,
    mediacall
}
/// <summary>
/// 旧版本LiteDb工具
/// </summary>
public class VesalOldLitdbTools {
    /// <summary>
    /// 获取LiteDb passWord
    /// </summary>
    /// <param name="_record_file_path"></param>
    /// <returns></returns>
    public static string loadKeyDat(string _record_file_path)
    {
        try
        {
            using (FileStream fsRead = new FileStream(_record_file_path, System.IO.FileMode.Open))
            {
                int fsLen = (int)fsRead.Length;
                byte[] heByte = new byte[fsLen];
                int r = fsRead.Read(heByte, 0, heByte.Length);
                String tmp = System.Text.Encoding.UTF8.GetString(heByte);
                string result = PublicTools.Base64Helper.Base64Decode(tmp);
                fsRead.Close();
#if UNITY_EDITOR
                // result = "C13B491BEB";
#endif
                return result;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            return "";
        }
    }
    /// <summary>
    /// 获取2D动画
    /// </summary>
    /// <param name="datPath">db路径</param>
    /// <param name="key">密码</param>
    /// <param name="id">动画id</param>
    /// <param name="outpath">输出路径</param>
    public static void get2d(string datPath,string key,string id,string outpath)
    {
        ConnectionString connect1 = new ConnectionString();
        connect1.Filename = datPath;
        connect1.LimitSize = 10000000000;
        connect1.Journal = false;
        connect1.Mode = LiteDB.FileMode.ReadOnly;
        using (var db = new LiteDatabase(connect1))
        {
            foreach (LiteFileInfo tmpInfo in db.FileStorage.FindAll())
            {

                if (tmpInfo.Id.EndsWith(".xml")&&tmpInfo.Id.Replace(".xml", "") == id)
                {
                    //try
                    //{
                        //Console.WriteLine(tmpInfo.Id);

                        XmlDocument doc = new XmlDocument();
                        doc.Load(db.FileStorage.OpenRead(tmpInfo.Id));
                        //doc.Save("./xmls/"+tmpInfo.Id);
                        XmlElement rootElement = doc.DocumentElement;
                        XmlNodeList personNodes = rootElement.GetElementsByTagName("scene");
                        string modeC = ((XmlElement)personNodes[0]).GetAttribute("mode");
                        if (modeC == "Animation2D")
                        {
                            LoadVideo(datPath, key, id,outpath);
                        }
                    //}
                    //catch (Exception e)
                    //{
                    //    Debug.Log(e.Message);
                    //    Debug.Log(e.StackTrace);
                    //}
                }
            }
        }
    }
    /// <summary>
    /// 输出2D视频
    /// </summary>
    /// <param name="url">db路径</param>
    /// <param name="password">密码</param>
    /// <param name="sceneName">动画编号</param>
    /// <param name="outpath">输出路径</param>
    public static void LoadVideo(string url, string password, string sceneName, string outpath)
    {
        ConnectionString connect0 = new ConnectionString();
        connect0.Filename = url;
        connect0.LimitSize = 10000000000;
        connect0.Journal = false;
        connect0.Mode = LiteDB.FileMode.ReadOnly;

        string secUrl = string.Empty;
        url = url.Replace("\\","/");
        secUrl = url.Insert(url.LastIndexOf("/") + 1, "vesal");
        ConnectionString connect1 = new ConnectionString();
        connect1.Filename = secUrl;
        connect1.LimitSize = 10000000000;
        connect1.Password = password;
        connect1.Journal = false;
        connect1.Mode = LiteDB.FileMode.ReadOnly;
        byte[] streams1, streams2;
        using (var db = new LiteDatabase(connect1))
        {
            var stream = db.FileStorage.OpenRead(sceneName + ".assetbundle");
            streams1 = StreamToBytes(stream);
            stream.Dispose();
        }
        ConnectionString connect2 = new ConnectionString();
        connect2.Filename = url;
        connect2.LimitSize = 10000000000;
        connect2.Journal = false;
        connect2.Mode = LiteDB.FileMode.ReadOnly;
        using (var db = new LiteDatabase(connect2))
        {
            var stream = db.FileStorage.OpenRead(sceneName + ".assetbundle");
            streams2 = StreamToBytes(stream);
            stream.Dispose();
            byte[] streams = new byte[streams1.Length + streams2.Length];
            streams1.CopyTo(streams, 0);
            streams2.CopyTo(streams, streams1.Length);
            if (File.Exists(outpath+"/" + sceneName + ".mp4"))
                File.Delete(outpath+"/" + sceneName + ".mp4");
            FileStream fs = new FileStream(outpath+"/" + sceneName + ".mp4", System.IO.FileMode.OpenOrCreate);//初始化文件流
            fs.Write(streams, 0, streams.Length);//将字节数组写入文件流
            fs.Close();//关闭流
            db.Dispose();
        }
    }
    /// <summary>
    /// 流转字节数组
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static byte[] StreamToBytes(LiteFileStream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        return bytes;
    }
    /// <summary>
    /// 读取LiteDb名词xml对应场景信息
    /// </summary>
    /// <param name="url">数据库文件路径</param>
    /// <param name="id">名词编号</param>
    /// <returns></returns>
    public static string readLiteDbNoInfo(string url,string id)
    {
        string mode = "PPTVesal";
        try
        {
            ConnectionString connect1 = new ConnectionString();
            connect1.Filename = url;
            connect1.LimitSize = 10000000000;
            connect1.Journal = false;
            connect1.Mode = LiteDB.FileMode.ReadOnly;
            using (var db = new LiteDatabase(connect1))
            {
                bool isHavScene = db.CollectionExists(id + ".xml");
                if (!isHavScene)
                {
                    var stream = db.FileStorage.OpenRead(id + ".xml");
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
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            mode = "PPTVesal";
        }
        return mode;

    }
}