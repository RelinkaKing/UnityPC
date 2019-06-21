using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml;
using LiteDB;
using System;


public class AnimationControl : MonoBehaviour
{
    public Camera thisCamera;
    public Camera UICamera;
    public Camera UIBGCamera;
    // Use this for initialization
    public GameObject animationGameObject, parentObject;

    public AnimationState anim;
    //分段进度条
    public List<Slider> progress = new List<Slider>();
    //整体进度条
    public Slider animationSlider;
    //暂停按钮，循环按钮
    public Image playOrParse, onceOrLoop;
    public Sprite spritePlay, spriteParse, spriteOnce, spriteLoop;

    public bool onUISilder, isPlay, isLoop, isLoading, isPlayContinuously;

    int m = -1, playCount;

    float tempProgress;
    public static string url,pwd,aniNo;
    public static AnimationControl instance;
    private void OnEnable()
    {
        instance = this;
        thisCamera = Camera.main;
    }
    void Start()
    {
//#if UNITY_EDITOR
//        url = "C:\\VesalDigital\\PPT\\Data\\vesal.dat";
//        pwd = "C13B491BEB";
//        pwd = "C4AA442CAF";
//        aniNo = "SA0107018";
//#endif
        //初始化
        isLoading = true;
        LoadXml(url, pwd, aniNo);
        tempProgress = 0;
        m = -1;
        UpUiSilder();
        ProgressDivision();
        AnimationPause();
        isLoading = false;
        isPlayContinuously = true;
        PlayContinuouslyORSegmentedPlayback();
    }
    public Text namePrint;
    // Update is called once per frame
    void LateUpdate()
    {   
        //空格暂停
        if (Input.anyKeyDown)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                PlayOrParse();
            }
        }
        if (!onUISilder)
        {
            //一次播放完成
            if (anim.normalizedTime >= effective / total && isPlayContinuously)
            {
                anim.normalizedTime = 0;
                m = -1;
                playCount++;
                if (playCount == 4)
                {
                    AnimationPause();
                    playCount = 0;
                }
            }
            //播放进度
            animationSlider.value = anim.normalizedTime + playCount * effective / total;
            //段落播放进度
            for (int i = 0; i < progress.Count; i++)
            {
                progress[i].value = anim.normalizedTime;
                if (progress[i].value != progress[i].minValue && progress[i].value != progress[i].maxValue)
                {
                    namePrint.text = list[i].name;
                }
            }

            for (int i = 0; i < progress.Count; i++)
            {
                if (i > m)
                {
                    if (progress[i].value == progress[i].maxValue)
                    {
                        //分段播放
                        if (!isPlayContinuously)
                        {
                            AnimationPause();
                            if (i == progress.Count - 1)
                            {
                                anim.normalizedTime = 0;
                                m = -1;
                                playCount++;
                                if (playCount == 4)
                                {
                                    playCount = 0;
                                }
                                break;
                            }
                            else
                            {
                                anim.normalizedTime = progress[i].maxValue;
                            }

                        }
                        m = i;
                    }
                }
            }

        }
    }
    //分段模式切换
    public Sprite[] POSP;
    public Image playOSP;
    public void PlayContinuouslyORSegmentedPlayback()
    {
        if (isPlayContinuously)
        {
            playOSP.sprite = POSP[1];
            isPlayContinuously = false;
        }
        else
        {
            playOSP.sprite = POSP[0];
            isPlayContinuously = true;
        }
    }
    //暂停开关
    public void PlayOrParse()
    {
        if (isPlay)
        {
            AnimationPause();
        }
        else
        {
            AnimationPlay();
        }
    }
    public void AnimationPlay()
    {
        anim.speed = 1;
        playOrParse.sprite = spriteParse;
        isPlay = true;
    }
    public void AnimationPause()
    {
        anim.speed = 0;
        playOrParse.sprite = spritePlay;
        isPlay = false;
    }
    //进度条控制
    public void DownUiSilder()
    {
        onUISilder = true;
        AnimationPause();
    }
    public void sliderChange()
    {
        playCount = (int)(animationSlider.value / (effective / total));
        float valueTemp = animationSlider.value - playCount * effective / total;
        anim.normalizedTime = valueTemp;
        for (int i = 0; i < progress.Count; i++)
        {
            progress[i].value = valueTemp;
            if (valueTemp < progress[i].maxValue && valueTemp >= progress[i].minValue)
            {
                m = i - 1;
            }
        }
    }
    public void UpUiSilder()
    {
        onUISilder = false;
    }
    //总进度条为播放4次
    void ProgressDivision()
    {
        animationSlider.maxValue = effective / total * 4;
    }
    //切换段落
    public void SegmentedButton(int game)
    {
        onUISilder = true;
        anim.normalizedTime = progress[game].minValue;
        namePrint.text = list[game].name;
        m = game - 1;
        onUISilder = false;
    }

    string abObjName, sceneName, animationName;
    float total, effective;
    public Transform progressParent, segmentedParent;
    List<Segmentation> list = new List<Segmentation>();
    //加载XML
    void LoadXml(string url, string password, string xmlName)
    {
        vesal_log.vesal_write_log("开始从DB库中读取XML文件");
        ConnectionString connect1 = new ConnectionString();
        connect1.Filename = url;
        connect1.LimitSize = 10000000000;
        connect1.Journal = false;
        connect1.Mode = LiteDB.FileMode.ReadOnly;
        using (var db = new LiteDatabase(connect1))
        {
            bool isHavScene = db.CollectionExists(xmlName + ".xml");
            if (!isHavScene)
            {
                var stream = db.FileStorage.OpenRead(xmlName + ".xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                XmlElement root = doc.DocumentElement;
                XmlElement animation = (XmlElement)root.FirstChild;
                total = float.Parse(animation.GetAttribute("total"));
                effective = float.Parse(animation.GetAttribute("effective"));
                abObjName = animation.GetAttribute("object");
                try
                {
                    Animation_MouseFollowRotation tmpAMF = this.GetComponent<Animation_MouseFollowRotation>();
                    tmpAMF.minDis = float.Parse(animation.GetAttribute("minDis"));
                    //tmpAMF.maxDis = float.Parse(animation.GetAttribute("maxDis"));
                    Interaction.instance.maxDistance = float.Parse(animation.GetAttribute("maxDis"));
                    Interaction.instance.minDistance = tmpAMF.minDis;
                    tmpAMF.distance = float.Parse(animation.GetAttribute("distance"));
                    Debug.Log("float.Parse(animation.GetAttribute:"+tmpAMF.distance);
                    Camera.main.GetComponent<Interaction>().RotateOpera(transform.parent.rotation.eulerAngles);
                    if (xmlName == "SA0607004" || xmlName == "SA0607005" || xmlName == "SA0607006" || xmlName == "SA0607007")
                        Camera.main.GetComponent<Interaction>().SetTarget(new Vector3(0,0,0), 1);
                    else
                        Camera.main.GetComponent<Interaction>().SetTarget(transform.parent.position, tmpAMF.distance);
                    Camera.main.GetComponent<Interaction>().distance = tmpAMF.distance;
                    try
                    {
                        thisCamera.farClipPlane = float.Parse(animation.GetAttribute("farClipPlane"));
                        thisCamera.fieldOfView = float.Parse(animation.GetAttribute("fieldOfView"));
                    }
                    catch(Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    string color = animation.GetAttribute("color");
                    //主题颜色切换
                    switch (color)
                    {
                        case "白色":
                            thisCamera.backgroundColor = Color.white;
                            uiChange(0);
                            tmpAMF.sprite = tmpAMF.sprite0;
                            break;
                        case "灰色":
                            thisCamera.backgroundColor = new Color(50 / 255f, 50 / 255f, 50 / 255f, 1);
                            uiChange(1);
                            tmpAMF.sprite = tmpAMF.sprite1;
                            break;
                        case "黑色":
                            thisCamera.backgroundColor = Color.black;
                            uiChange(1);
                            tmpAMF.sprite = tmpAMF.sprite1;
                            break;
                        default:
                            break;
                    }
                }
                catch(Exception E)
                {
                    Debug.Log(E.Message);
                    throw E;
                }
                XmlNodeList Segmentations = animation.ChildNodes;
                if (Segmentations.Count == 0)
                {
                    vesal_log.vesal_write_log("无分段信息");
                }
                else
                {
                    //分段信息记录
                    for (int se = 0; se < Segmentations.Count; se++)
                    {
                        Segmentation seg = new Segmentation();
                        seg.start = float.Parse(((XmlElement)Segmentations[se]).GetAttribute("start"));
                        seg.end = float.Parse(((XmlElement)Segmentations[se]).GetAttribute("end"));
                        seg.count = float.Parse(((XmlElement)Segmentations[se]).GetAttribute("count"));
                        seg.name = ((XmlElement)Segmentations[se]).GetAttribute("name");
                        seg.countString = ((XmlElement)Segmentations[se]).GetAttribute("countString");
                        list.Add(seg);
                    }
                    //bool color = true;
                    //分段按钮及进度条生成
                    for (int s = 0; s < list.Count; s++)
                    {
                        GameObject game1 = Instantiate(Resources.Load<GameObject>("Prefab/PPTProgress"));
                        GameObject game2 = Instantiate(Resources.Load<GameObject>("Prefab/Segmented"));
                        game1.transform.SetParent(progressParent);
                        game2.transform.SetParent(segmentedParent);
                        game1.transform.localPosition = Vector3.zero;
                        game2.transform.localPosition = Vector3.zero;
                        game1.transform.localScale = Vector3.one;
                        game2.transform.localScale = Vector3.one;
                        game1.GetComponent<LayoutElement>().preferredWidth = (int)(1000 * list[s].count / effective) + 1;
                        game1.transform.GetChild(3).GetComponent<Text>().text = list[s].countString;
                        game1.GetComponent<Slider>().minValue = list[s].start / total;
                        game1.GetComponent<Slider>().maxValue = list[s].end / total;
                        game2.transform.GetChild(0).GetComponent<Text>().text = (s + 1).ToString() + "." + list[s].name;
                        game2.GetComponent<SegmentedButton>().index = s;
                        progress.Add(game1.GetComponent<Slider>());
                    }
                }
                LoadModels(url, pwd, aniNo);
            }
            else
            {
                vesal_log.vesal_write_log(string.Format("库中不存在此模型场景：{0}", xmlName));
            }
        }
    }

    public GameObject CurrentObj;
    //加载模型
    public void LoadModels(string url, string password, string sceneName)
    {
        string secUrl = string.Empty;
        url = url.Replace("\\", "/");
        secUrl = url.Insert(url.LastIndexOf("/") + 1, "vesal");
        vesal_log.vesal_write_log("开始获取加密文件：" + DateTime.Now.TimeOfDay.ToString());
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
        vesal_log.vesal_write_log("开始获取未加密文件：" + DateTime.Now.TimeOfDay.ToString());
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
            //流组合
            byte[] streams = new byte[streams1.Length + streams2.Length];
            streams1.CopyTo(streams, 0);
            streams2.CopyTo(streams, streams1.Length);
            //加载ab
            AssetBundle curBundleObj = AssetBundle.LoadFromMemory(streams);
            CurrentObj = (GameObject)curBundleObj.LoadAsset(sceneName, typeof(GameObject));
            Debug.Log("animationGameObject = Instantiate(CurrentObj)");
            animationGameObject = Instantiate(CurrentObj);
            //获取动画
            anim = animationGameObject.GetComponent<Animation>()[abObjName];
            anim.normalizedTime = 0;
            Vector3 tmpV = animationGameObject.transform.rotation.eulerAngles;
            animationGameObject.transform.rotation = Quaternion.Euler(tmpV.x,tmpV.y - 180f, tmpV.z);
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.plugin)
            {
                animationGameObject.SetActive(true);
            }
            else {
                animationGameObject.SetActive(false);
            }
            vesal_log.vesal_write_log("模型加载完成：" + DateTime.Now.TimeOfDay.ToString());
            curBundleObj.Unload(false);
            db.Dispose();
        }
    }
        //图片组件
        public Image[] images;
        //图片组
        public Sprite[] sprite0;
        public Sprite[] sprite1;
        //更改组件图片
        public void uiChange(int index)
        {
            switch (index)
            {
                case 0:
                    for (int i = 0; i < images.Length; i++)
                    {
                        images[i].sprite = sprite0[i];
                    }
                    break;
                case 1:
                    for (int i = 0; i < images.Length; i++)
                    {
                        images[i].sprite = sprite1[i];
                    }
                    break;
                default:
                    break;
            }
        }
        public byte[] StreamToBytes(LiteFileStream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        return bytes;
    }
}

public class Segmentation
{
    public float start, end, count;
    public string name, countString;
}

