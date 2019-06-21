using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VesalDecrypt;
/// <summary>
/// 视频控制器
/// </summary>
public class VideoController : BaseController
{
    //视频文档对象
    public VideoElement ve;
    //视频显示图形组件
    [Header("Video")]
    public RawImage image;
    public VideoPlayer player;
    public AudioSource source;
    //是否放映中
    public bool isPlaying = false;
    //已放映过
    public bool isPlayed = false;
    //是否放映结束
    public bool isEnd = false;



    public override void Do(float slideTime)
    {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        { //结束并且非循环播放
            if (isEnd && !ve.isLoop)
            {
                return;
            }
            //视频播放完毕
            if (isPlaying && Mathf.Abs((float)player.time - player.frameCount / player.frameRate) < 0.001f)
            {
                isEnd = true;
                image.enabled = false;
                isPlaying = false;
                player.Stop();
                source.Stop();
                return;
            }
            //到结束时间
            if (ve.endTime <= slideTime)
            {
                if (player.isPlaying)
                {
                    player.Stop();
                    source.Stop();
                }
                if (image.enabled)
                {
                    image.enabled = false;
                }
                return;
            }
            //到了出现时间
            if (ve.appearTime <= slideTime)
            {
                if (player.isPlaying && !image.enabled)
                {
                    image.enabled = true;
                }
                if (!player.isPlaying && (!isPlayed || ve.isLoop))
                {
                    Play();
                }
            }
        }
       

            ////结束并且非循环播放
            //if (isEnd  && !ve.isLoop) {
            //    return;
            //}
            ////视频播放完毕
            //if (isPlaying && Mathf.Abs((float)player.time - player.frameCount / player.frameRate) < 0.001f) {
            //    isEnd = true;
            //    image.enabled = false;
            //    isPlaying = false;
            //    player.Stop();
            //    return;
            //}
            ////到结束时间
            //if (ve.endTime<=slideTime) {
            //    if (player.isPlaying) {
            //        player.Stop();
            //    }
            //    if (image.enabled) {
            //        image.enabled = false;
            //    }
            //    return;
            //}
            ////到了出现时间
            //if (ve.appearTime<=slideTime) {
            //    if (player.isPlaying && !image.enabled) {
            //        image.enabled = true;
            //    }
            //    if (!player.isPlaying && (!isPlayed || ve.isLoop))
            //    {
            //        Play();
            //    }

            //}
        }
    /// <summary>
    /// 播放
    /// </summary>
    public void Play()
    {
        isPlaying = true;
        isPlayed = true;
        image.enabled = true;
        isEnd = false;
        player.isLooping = ve.isLoop;
        Debug.Log("player.isLooping "+ player.isLooping);
        player.Play();
        source.Play();

    }
    public override void GoDie(float slideTime)
    {
        if (player.isPlaying)
        {
            player.Stop();
            source.Stop();
        }
        if (image.enabled)
        {
            image.enabled = false;
        }
    }

    public override void Init()
    {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            ve.aniList.Clear();
        }
        base.Init();
        player.Stop();
        source.Stop();
        loadVideo();
        isEnd = false;
        isPlaying = false;
        image.enabled = false;
        isPlayed = false;
        PlayButton.SetActive(false);
        isEnterAniDown = false;
    }
    /// <summary>
    /// 加载视频
    /// </summary>
    private void loadVideo() {
        string basePath = PPTGlobal.PPTPath + pageNum + "/";
        //if (File.Exists(basePath + ve.id + ve.fileName)) {
        //    File.Delete(basePath + ve.id + ve.fileName);
        //}
        player.source = VideoSource.VideoClip;
        //解密
        //encrypt.DecryptFile(basePath + ve.fileName, basePath + ve.id+ ve.fileName, "Vesal17788051918");

        //player.url = basePath + ve.id + ve.fileName;
        player.url = basePath + ve.fileName;
        if (ve.isPlayAudio)
        {
            //在视频中嵌入的音频类型
            player.audioOutputMode = VideoAudioOutputMode.AudioSource;

            //把声音组件赋值给VideoPlayer
            player.SetTargetAudioSource(0, source);
        }
        player.playOnAwake = false;
        player.waitForFirstFrame = true;
        //当VideoPlayer全部设置好的时候调用
        player.prepareCompleted += Prepared;
        //启动播放器
        player.sendFrameReadyEvents = true;
        player.frameReady += OnNewFrame;
        player.Prepare();
        //player.Play();


    }
    //获得视频第几帧的图片
    int framesValue = 0;
    /// <summary>
    /// 等待一帧画面
    /// </summary>
    /// <param name="source"></param>
    /// <param name="frameIdx"></param>
    void OnNewFrame(VideoPlayer source, long frameIdx)
    {
        framesValue++;
        if (framesValue == 2)
        {
            image.texture = source.texture;

            player.frameReady -= OnNewFrame;
            player.sendFrameReadyEvents = false;
            framesValue = 0;
            //player.Pause();
        }
    }
    //开始播放视频
    private void Prepared(VideoPlayer player)
    {
        //player.Play();
        //StartCoroutine(videoEnter());
    }
   

    public override void Pause()
    {

        if (isPlaying)
        {
            player.Pause();
            source.Pause();
        }
    }
    public override void Begin()
    {
        if (isEnd && !ve.isLoop)
        {
            return;
        }

        if (isPlaying) {
            player.Play();
            source.Play();
        }
    }
    
    // Use this for initialization
    void Start () {
		
	}
    //PptPlayer播放按钮
    public GameObject PlayButton;
    //是否进入动画执行
    bool isEnterAniDown = false;
	// Update is called once per frame
	void Update () {
        if (PPTGlobal.pptStatus==PPTGlobal.PPTStatus.play && player != null) {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                if (isEnterAniDown)
                {
                    if (player.isPlaying)
                    {
                        image.texture = player.texture;
                        if (PlayButton.activeSelf)
                        {
                            PlayButton.SetActive(false);
                        }
                    }
                    else
                    {
                        if (!PlayButton.activeSelf)
                        {
                            PlayButton.SetActive(true);
                        }
                    }
                }
            }
            else {
                if (PlayButton.activeSelf)
                {
                    PlayButton.SetActive(false);
                }
            }

            if (isPlaying && Mathf.Abs((float)player.time - player.frameCount / player.frameRate) < 0.001f)
            {
                if (!player.isLooping)
                {
                    isEnd = true;
                    image.enabled = false;
                    isPlaying = false;
                    player.Stop();
                    source.Stop();
                }
                else
                {
                    player.time = 0.0d;
                    source.time = 0.0f;
                    player.Play();
                    source.Play();
                    image.texture = player.texture;
                }
            }

        }

       
    }

    public override void InitCompent(int pageNum)
    {
        PlayButton.GetComponent<Button>().onClick.AddListener(Play);
        this.pageNum = pageNum;
        InitRect(copy(ve));
        image.enabled = false;
        isPlaying = false;
        if (player == null)
        {
            player = gameObject.AddComponent<VideoPlayer>();
            player.playOnAwake = false;
            player.aspectRatio = VideoAspectRatio.Stretch;
        }
        if (ve.isPlayAudio) {

            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            
        }
        PlayButton.SetActive(false);
        isEnterAniDown = false;
    }

    public override void QuicklyComeIntoBowl(XmlElement element)
    {
        XmlElement currElement = (XmlElement)element.SelectSingleNode("Videos/Video[@id='" + ve.id + "']");
        if (currElement == null)
        {
            Debug.Log("currElement == NULL !!!!!!!!!!!!1");
            return;
        }
        if (ve.appearTime >= ve.endTime)
        {
            ve.endTime = float.Parse(element.GetAttribute("totalTime"));
        }
        currElement.SetAttribute("appearTime", ve.appearTime + "");
        currElement.SetAttribute("endTime", ve.endTime + "");
        currElement.RemoveAttribute("shapeId");
    }
    public override void DoSort()
    {
        sortAniList(ve);
    }
    //IEnumerator videoEnter() {
        
        
    //    player.Play();
    //    yield return new WaitForSecondsRealtime(0.001f);
    //    image.texture = player.texture;
    //    yield return new WaitForSecondsRealtime(0.001f);
    //    image.texture = player.texture;
    //    yield return new WaitForSecondsRealtime(0.001f);
    //    player.Pause();
    //}
    public override void DoAnimation(PPTAnimation ani)
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (ani.shapeId != ve.shapeId)
            {
                return;
            }
        }
        Debug.Log(ani.id + " - " + ani.action + " - " + ani.type + " - " + ani.waittime);
        if (ani.action == PPTAction.entr) {
            Debug.Log("video enter");
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                ve.appearTime = ani.slideTime;
                isEnterAniDown = true;
            }
            else {
             Play();
            //image.enabled = true;
            //player.Play();
            image.texture = player.texture;
            }
            //player.Pause();
            //source.Stop();
        }
        if (ani.action == PPTAction.mediacall)
        {
            try
            {
                if (ani.call == PPTCall.play)
                {
                    Play();
                    if (ani.playfrom != 0f) {
                        player.time = ani.playfrom;
                    }
                }
                else if (ani.call == PPTCall.togglePause)
                {
                    Pause();
                }
                else if (ani.call == PPTCall.stop)
                {
                    isEnd = true;
                    image.enabled = false;
                    isPlaying = false;
                    player.Stop();
                    source.Stop();
                }
                if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
                {
                    if (!ve.aniList.Contains(ani))
                    {
                        ve.aniList.Add(ani);
                    }
                }
            }
            catch (Exception e) {
                Debug.Log(e.Message);
            }
            
        }
        else if (ani.action == PPTAction.exit)
        {
            ve.endTime = ani.slideTime;
            player.Stop();
            source.Stop();
            image.enabled = false;

            isEnterAniDown = false;
            PlayButton.SetActive(false);
        }
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            PPTController.isExecuted = true;
        }
    }


}
