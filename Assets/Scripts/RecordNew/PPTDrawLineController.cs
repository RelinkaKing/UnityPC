using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

public class PPTDrawLineController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        statParticles();

    }
    //普通线条
    private LineRenderer line;
    public void Begin()
    {
        if (Particles != null)
        {
            Particles.Play();
        }
    }
    //当前页时间
    float slideTime;
    //PPTController定时广播执行函数
    public void Do(float slideTime)
    {
        this.slideTime = slideTime;
    }
    //结束时销毁
    public void GoDie()
    {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            slideTime = 0;
        }
        ClearAllLine();

    }
    //是否当前页录制中
    bool isPageRecording = false;
    //当前页录制开关
    public void ToggleCurrentPageRecord()
    {
        Debug.Log("ToggleCurrentPageRecord: pptDraw");
        isPageRecording = !isPageRecording;
        if (isPageRecording) {
            this.recordDrawLines.Clear();
        }
    }
    /// <summary>
    /// 保存当前页画笔位置信息
    /// </summary>
    /// <param name="pn"></param>
    public void savePoints(int pn)
    {
        ClearAllLine();
        Debug.Log("savePoints:" + pn);
        DrawLineDoc dld = new DrawLineDoc();
        dld.recordDrawLines = this.recordDrawLines;
        string xmlPath = PPTGlobal.PPTPath + pn + "/DrawLine.rec";
        string Time = PublicTools.getTime();
        if (File.Exists(xmlPath)) {
            File.Move(xmlPath, PPTGlobal.PPTPath + pn + "/"+Time+ "_DrawLine.rec");
        }
        PublicTools.SaveObject(xmlPath, dld);
        Debug.Log("savePoints:" + xmlPath);
        clearXml(xmlPath);
        recordDrawLines.Clear();
    }
    /// <summary>
    /// 清理xml中不需要元素属性（和上一元素属性值相等的属性）
    /// </summary>
    /// <param name="xmlPath"></param>
    public void clearXml(string xmlPath)
    {
        if (!File.Exists(xmlPath))
        {
            return;
        }
        XmlDocument xd = new XmlDocument();
        xd.Load(xmlPath);
        XmlElement lastElement = null;
        XmlElement tempElement = null;

        string[] filter = new string[] { "nl", "pl", "isnew", "isClear" };
        foreach (XmlElement ele in xd.SelectNodes("//DL"))
        {
            if (lastElement != null)
            {
                tempElement = PublicTools.DeepCopy(ele);
                RemoveTargetAttr(lastElement, ele, filter);
                lastElement = tempElement;
            }
            else{
                lastElement = ele;
            }
            
        }
        xd.Save(xmlPath);
    }
    /// <summary>
    /// 移除相同属性值
    /// </summary>
    /// <param name="x1">两个相邻节点</param>
    /// <param name="x2"></param>
    /// <param name="attrs">要比较的属性名</param>
    void RemoveTargetAttr(XmlElement x1, XmlElement x2, string[] attrs)
    {
        foreach (string attr in attrs)
        {

            if (x1.HasAttribute(attr) && x2.HasAttribute(attr))
            {
                if (x1.GetAttribute(attr) == x2.GetAttribute(attr) && !(x1.GetAttribute("isClear") == "true"))
                {
                    x2.RemoveAttribute(attr);
                }
            }
        }
    }
    /// <summary>
    /// 刷新橡皮擦状态，当前页面上没有线则隐藏橡皮
    /// </summary>
    /// <param name="ruber"></param>
    public void isClear(GameObject ruber) {
        if (currentLines.Count == 1 && currentLines[0].name == "NormalLine_1") {
            LineRenderer line = currentLines[0].GetComponent<LineRenderer>();
            int sum = line.positionCount;
            Vector3 tmp =  line.GetPosition(0);
            if (sum == 1) {
                ClearAllLine();
                ruber.SetActive(false);
                return;
            }
            for (int i = 1;i<sum;i++) {
                if (tmp == line.GetPosition(i))
                {
                    continue;
                }
                else {
                    return;
                }

            }
            
            ClearAllLine();
            ruber.SetActive(false);
            return;
            
        }
        if (currentLines.Count ==0) {
            ClearAllLine();
            ruber.SetActive(false);
        }
    }
    //当前线的集合
    List<GameObject> currentLines = new List<GameObject>();
    //初始化正常线
    public void initNormalLine()
    {

        GameObject tmp = GameObject.Find("NormalLine");
        tmp = GameObject.Instantiate(tmp, tmp.transform.parent);
        tmp.name = "NormalLine_1";
        line = tmp.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.material = paintPenMat;
        line.startWidth = 0.08f;
        line.transform.localPosition = Vector3.zero;
        currentLines.Add(tmp);
        NLindex = 0;

    }
    //初始化粒子线
    public void initParticleLine()
    {
        //Particles = GameObject.Find("ParticleDrawLine").GetComponent<ParticleSystem>();
        GameObject tmp = GameObject.Find("ParticleDrawLine");
        tmp = GameObject.Instantiate(tmp, tmp.transform.parent);
        tmp.name = "ParticleDrawLine_1";
        currentLines.Add(tmp);
        Particles = tmp.GetComponent<ParticleSystem>();
        tmp.layer = DrawLineLayer;
    }
    //普通线的Material 
    Material paintPenMat;
    //定义画线层级
    int DrawLineLayer = 12;
    /// <summary>
    /// 初始化画线组件
    /// </summary>
    public void InitCompent()
    {
        //initNormalLine();

        paintPenMat = Resources.Load("Materials/paintPen") as Material;
        gameObject.layer = DrawLineLayer;


        isNewLine = true;

    }
    //粒子系统
    ParticleSystem Particles;
    //画线相机
    public Camera LineCamera;
    /// <summary>
    /// 指定位置显示粒子点
    /// </summary>
    /// <param name="tmp">屏幕坐标</param>
    public void showParticles(Vector3 tmp)
    {
        if (Particles == null)
        {
            return;
        }
        Particles.transform.position = LineCamera.ScreenToWorldPoint(tmp);
    }

    /// <summary>
    /// 开启粒子画线
    /// </summary>
    public void statParticles()
    {

        InitCompent();


    }
    /// <summary>
    /// 停止粒子画线
    /// </summary>
    public void stopParticles()
    {
        if (Particles == null)
        {
            return;
        }
        Particles.Stop();
        Particles.Clear();
        Particles.transform.position = Vector3.up * 1000;

    }
    Vector3 startPoint, endPoint;
    float posX, posY, posXx, posYy;
    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        if (Particles == null)
        {
            return;
        }
        Particles.Pause();

    }
    //是否新起始一条线
    public bool isNewLine = false;
    //普通画线索引
    int NLindex = 0;
    //清理屏幕上所有画线
    public void ClearAllLine()
    {
        isNewLine = true;
        if (currentLines.Count == 0)
        {
            return;
        }
        foreach (GameObject tmpObj in currentLines)
        {
            if (tmpObj != null && tmpObj.activeSelf)
            {
                tmpObj.SetActive(false);
                Destroy(tmpObj);
            }
        }
        currentLines.Clear();
        if (isPageRecording)
        {
            recordDrawLines.Add(new DrawLineRecordItem
            {
                isClear = true,
                isnew = true,
                t = slideTime
            });
        }
    }
    /// <summary>
    /// 画普通线
    /// </summary>
    /// <param name="v2">屏幕坐标</param>
    public void DrawNL(Vector2 v2)
    {

        if (isNewLine)
        {
            initNormalLine();

        }

        if (line != null)
        {
            line.positionCount = NLindex + 1;
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                line.SetPosition(NLindex++, new Vector3(Input.mousePosition.x - Screen.width / 2f, Input.mousePosition.y - Screen.height / 2f));
            }
            else {

                line.SetPosition(NLindex++, new Vector3(v2.x - Screen.width / 2f, v2.y - Screen.height / 2f));
            }

        }

    }
    /// <summary>
    /// 画粒子线
    /// </summary>
    /// <param name="v2">屏幕坐标</param>
    public void DrawPL(Vector2 v2)
    {

        if (isNewLine)
        {
            initParticleLine();

        }
        if (!Particles.isPlaying || Particles.isStopped)
        {
            Particles.Play();
        }

        if (Particles != null)
        {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                Particles.transform.position = LineCamera.ScreenToWorldPoint(Input.mousePosition);
            }
            else {
                Particles.transform.position = LineCamera.ScreenToWorldPoint(v2);
            }
        }

    }
    /// <summary>
    /// 计时，双击清除画线
    /// </summary>
    public void Timer()
    {
        if (isStartTimer) {
            dateTime += Time.deltaTime;
        }
        
       
        if (dateTime > 0.3f) {

            dateTime = 0;
            isStartTimer = false;
        }
    }
    //初次点击
    bool isFirstClick = true;
    //是否开始计时
    bool isStartTimer = false;
    //累计时间
    float dateTime = 0.0f;
    void Update()
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (PPTGlobal.pptStatus == PPTGlobal.PPTStatus.play)
            {
                //if (EventSystem.current.IsPointerOverGameObject()) {
                //    return;
                //}
                bool isTwoMouseButtonDows = false;
                //计时
                Timer();
                if (Input.GetMouseButtonDown(1))
                {
                    if (isStartTimer && dateTime < 0.3f)
                    {
                        //双击
                        //Debug.Log("双击");
                        isStartTimer = false;
                        ClearAllLine();
                        return;

                    }
                    else
                    {
                        isStartTimer = true;
                    }
                    dateTime = 0;
                }


                //双指同时按下为平移
                if (Input.GetMouseButton(1) && Input.GetMouseButton(0))
                {
                    isTwoMouseButtonDows = true;
                }
                //右键画笔
                if (Input.GetMouseButton(1) && !isTwoMouseButtonDows)
                {
                    DrawLineRecordItem dlr = null;
                    if (isPageRecording)
                    {
                        dlr = new DrawLineRecordItem
                        {
                            t = slideTime,
                            isnew = this.isNewLine,
                            nl = PPTController.isDrawNLEnable,
                            pl = PPTController.isDrawPLEnable,

                            pos = PublicTools.Vector22Str(new Vector2(Input.mousePosition.x * 1920f / Screen.width, Input.mousePosition.y * 1080 / Screen.height), "f0")
                        };
                        recordDrawLines.Add(dlr);
                    }
                    if (PPTController.isDrawNLEnable)
                    {
                        DrawNL(Vector2.zero);
                    }
                    //isParticlesPlaying && 
                    if (PPTController.isDrawPLEnable)
                    {
                        DrawPL(Vector2.zero);
                    }
                    isNewLine = false;

                }
                if (Input.GetMouseButtonUp(1) || isTwoMouseButtonDows)
                {
                    isNewLine = true;
                    line = null;
                    if (Particles != null)
                    {
                        //Particles.Clear();
                        Particles.gameObject.SetActive(false);
                        DestroyObject(Particles.gameObject, 0.5f);
                    }
                }
            }
        }
        else
        {
            if (PPTGlobal.pptStatus == PPTGlobal.PPTStatus.play)
            {
                if (isInited)
                {

                    try
                    {
                        while (dls.Count != 0 && float.Parse(dls[0].GetAttribute("t")) < slideTime)
                        {
                            drawLine(dls[0]);
                            dls.RemoveAt(0);
                        }
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
    //记录画线信息集合
    List<DrawLineRecordItem> recordDrawLines = new List<DrawLineRecordItem>();

    [Serializable]
    [XmlRoot("DrawLineDoc")]
    public class DrawLineDoc
    {
        [XmlArray("DLS"), XmlArrayItem("DL")]
        public List<DrawLineRecordItem> recordDrawLines;
    }
    [Serializable]
    public class DrawLineRecordItem
    {
        [XmlAttribute("t")]
        public float t;
        [XmlAttribute("pos")]
        public string pos;
        [XmlAttribute("nl")]
        public bool nl = false;
        [XmlAttribute("pl")]
        public bool pl = false;
        [XmlAttribute("isnew")]
        public bool isnew = false;
        [XmlAttribute("isClear")]
        public bool isClear = false;
    }

    bool nl, pl;
    /// <summary>
    /// 回放画线录制信息
    /// </summary>
    /// <param name="dlr">单个点的文档对象</param>
    void drawLine(XmlElement dlr)
    {
        if (dlr.GetAttribute("isClear") == "true")
        {
            ClearAllLine();
        }
        else
        {
            if (dlr.HasAttribute("isnew"))
            {
                isNewLine = bool.Parse(dlr.GetAttribute("isnew"));
                if (isNewLine && Particles != null)
                {
                    DestroyObject(Particles.gameObject, 0.1f);
                }
            }
            if (dlr.HasAttribute("nl"))
            {
                nl = bool.Parse(dlr.GetAttribute("nl"));
            }
            if (dlr.HasAttribute("pl"))
            {
                pl = bool.Parse(dlr.GetAttribute("pl"));
            }
            Vector2 tmpV2 = PublicTools.Str2Vector2(dlr.GetAttribute("pos"));
            tmpV2 = new Vector2(Screen.width / 1920f * tmpV2.x, Screen.height / 1080f * tmpV2.y);
            if (nl)
            {
                DrawNL(tmpV2);
            }
            if (pl)
            {
                DrawPL(tmpV2);
            }
        }
    }
    //是否已初始化
    bool isInited = false;
    //DrawLineDoc dld;
    //录制回放时，画线点对象的集合
    List<XmlElement> dls = new List<XmlElement>();
    /// <summary>
    /// 画线系统初始化
    /// </summary>
    public void Init()
    {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        {

        isInited = false;
        int pn = PPTController.pageNum;
        dls.Clear();
        string xmlPath = PPTGlobal.PPTPath + pn + "/DrawLine.rec";
        Debug.Log("DrawLine:" + xmlPath);
        if (File.Exists(xmlPath))
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);
                foreach (XmlElement ele in doc.SelectNodes("//DL"))
                {

                    dls.Add(ele);
                }

                //foreach (XmlElement ele in dls) {
                //    Vector2 tmpV2 = PublicTools.Str2Vector2(ele.GetAttribute("pos"));
                //    ele.SetAttribute("pos", PublicTools.Vector22Str(new Vector2(tmpV2.x+130, tmpV2.y)));
                //}
                //doc.Save(xmlPath);

                if (dls.Count != 0)
                {
                    isInited = true;
                }

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
                isInited = false;
            }
        }
        Debug.Log("DrawLine:" + isInited);
        Debug.Log("DrawLine:" + dls.Count);
        }
    }
}
