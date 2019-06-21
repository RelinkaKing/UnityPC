using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using LiteDB;
using System.IO;
using System.Xml;

public class Animation2DControl : MonoBehaviour
{
    //滑动条
    public Slider slider;
    //视频播放器
    public VideoPlayer videoPlayer;
    bool onUISilder, isloading;
    //播放/暂停按钮，循环按钮
    public Image playOrParse, onceOrLoop;
    public Sprite spritePlay, spriteParse, spriteOnce, spriteLoop;
    // Use this for initialization
    void Start()
    {
        isloading = true;
        onUISilder = false;
        LoadVideo(LoadModel_ppt.url, LoadModel_ppt.password, LoadModel_ppt.sceneName);
    }

    // Update is called once per frame
    bool firstPause = false;
    float t = 0;
    void Update()
    {
        if (videoPlayer.isPrepared)
        {
            isloading = false;
            //自动播放0.1s显示画面
            if (!firstPause)
            {
                t += Time.deltaTime;
                if (t >= 0.1f)
                {
                    videoPlayer.isLooping = true;
                    LoopSwitch();
                    AnimationPause();
                    firstPause = true;
                }
            }
        }
        if (!isloading)
        {
            if (!onUISilder)
            {
                //进度显示
                slider.maxValue = videoPlayer.frameCount / videoPlayer.frameRate;
                slider.value = (float)videoPlayer.time;
                if (!videoPlayer.isPlaying)
                {
                    playOrParse.sprite = spritePlay;
                }
                else
                {
                    playOrParse.sprite = spriteParse;
                }
            }
        }

        if (Input.anyKeyDown)
        {
            //空格暂停
            if (Input.GetKey(KeyCode.Space))
            {
                PlayOrParse();
            }
        }
    }

    byte[] StreamToBytes(LiteFileStream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        return bytes;
    }

    //加载视频
    void LoadVideo(string url, string password, string sceneName)
    {
        vesal_log.vesal_write_log("开始从DB库中读取XML文件");
        ConnectionString connect0 = new ConnectionString();
        connect0.Filename = url;
        connect0.LimitSize = 10000000000;
        connect0.Journal = false;
        connect0.Mode = LiteDB.FileMode.ReadOnly;
        using (var db = new LiteDatabase(connect0))
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
                for (int node = 0; node < personNodes.Count; node++)
                {
                    string color = ((XmlElement)personNodes[node]).GetAttribute("color");
                    vesal_log.vesal_write_log("UI颜色设置：" + color);
                    switch (color)
                    {
                        case "白色":
                            Camera.main.backgroundColor = Color.white;
                            break;
                        case "灰色":
                            Camera.main.backgroundColor = new Color(50 / 255f, 50 / 255f, 50 / 255f, 1);
                            break;
                        case "黑色":
                            Camera.main.backgroundColor = Color.black;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        string secUrl = string.Empty;
        secUrl = url.Insert(url.LastIndexOf("\\") + 1, "vesal");
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
            byte[] streams = new byte[streams1.Length + streams2.Length];
            streams1.CopyTo(streams, 0);
            streams2.CopyTo(streams, streams1.Length);
            if (File.Exists(Application.persistentDataPath + "/" + sceneName + ".mp4"))
                File.Delete(Application.persistentDataPath + "/" + sceneName + ".mp4");
            FileStream fs = new FileStream(Application.persistentDataPath + "/" + sceneName + ".mp4", System.IO.FileMode.OpenOrCreate);//初始化文件流
            tempVideoPath = Application.persistentDataPath + "/" + sceneName + ".mp4";
            fs.Write(streams, 0, streams.Length);//将字节数组写入文件流
            fs.Close();//关闭流

            videoPlayer.url = Application.persistentDataPath + "/" + sceneName + ".mp4";
            db.Dispose();
        }
    }
    string tempVideoPath;
    //删除本地资源
    private void OnDestroy()
    {
        if (File.Exists(tempVideoPath))
            File.Delete(tempVideoPath);
    }
    //暂停开关
    public void PlayOrParse()
    {
        if (videoPlayer.isPlaying)
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
        videoPlayer.Play();
    }
    public void AnimationPause()
    {
        videoPlayer.Pause();
    }
    //拖动滑动条
    public void DownUiSilder()
    {
        onUISilder = true;
        AnimationPause();
    }
    public void sliderChange()
    {
        videoPlayer.time = slider.value;
    }
    public void UpUiSilder()
    {
        onUISilder = false;
    }
    //循环开关
    public void LoopSwitch()
    {
        if (videoPlayer.isLooping)
        {
            onceOrLoop.sprite = spriteOnce;
            videoPlayer.isLooping = false;
        }
        else
        {
            onceOrLoop.sprite = spriteLoop;
            videoPlayer.isLooping = true;
        }
    }
}
