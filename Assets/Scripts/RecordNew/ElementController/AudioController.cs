using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class AudioController : BaseController
{
    //声音组件
    AudioSource source;
    //音频元素文档对象
    public AudioElement currentAe;
    //www直接加载wav文件
    WWW www = null;
    public AudioSlider audioSlider;
    public override void Begin()
    {
        if (source != null) {
            source.Play();
        }
    }
    private void Update()
    {
        if (PPTGlobal.pptStatus == PPTGlobal.PPTStatus.pause) {
            if (source!=null && source.isPlaying) {
                source.Pause();
            }
        }
    }
    public override void Do(float slideTime)
    {
        if (PPTGlobal.PPTEnv!=PPTGlobal.PPTEnvironment.PPTPlayer) {
            if (currentAe.appearTime <= slideTime && slideTime <= currentAe.endTime)
            {
                if (!source.isPlaying)
                {
                    if (source.clip == null && www == null)
                    {
                        play();
                    }
                    else {
                        source.Play();
                    }
                }
            }
            if (slideTime >= currentAe.endTime)
            {
                source.Stop();
                source.clip = null;
                //index++;
                //if (aeDic.ContainsKey(index)) {
                //    currentAe = aeDic[index];
                //}
            }
        }
    }

    public override void DoSort()
    {
        sortAniList(currentAe);
    }
    public override void QuicklyComeIntoBowl(XmlElement element)
    {

        XmlElement currElement = (XmlElement)element.SelectSingleNode("Audios/Audio[@id='" + currentAe.id + "']");
        if (currElement == null)
        {
            Debug.Log("currElement == NULL !!!!!!!!!!!!1");
            return;
        }
        currElement.RemoveAllAttributes();
        currElement.SetAttribute("appearTime", currentAe.appearTime + "");
        if (currentAe.appearTime >= currentAe.endTime)
        {
            currentAe.endTime = float.Parse(element.GetAttribute("totalTime"));
        }
        currElement.SetAttribute("endTime", currentAe.endTime + "");
        currElement.SetAttribute("id", currentAe.id + "");
        currElement.SetAttribute("Filename", currentAe.fileName + "");
        currElement.RemoveAttribute("shapeId");

      
    }

    public void togglePause() {
        if (source.isPlaying)
        {
            Pause();
        }
        else {
            play();
        }
    }
    public override void DoAnimation(PPTAnimation ani)
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (ani.shapeId != currentAe.shapeId)
            {
                return;
            }
            PPTController.isExecuted = true;
            if (ani.action == PPTAction.entr)
            {
                currentAe.appearTime = ani.slideTime;
                audioSlider.gameObject.SetActive(true);
            }
        }
        if (ani.action == PPTAction.mediacall || ani.action == PPTAction.entr)
        {
            try
            {
                if (ani.call == PPTCall.play)
                {

                    play();
                }
                else if (ani.call == PPTCall.togglePause)
                {

                    Pause();
                }
                else if (ani.call == PPTCall.stop)
                {

                    source.Stop();
                }
                if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
                {
                    if (!currentAe.aniList.Contains(ani))
                    {
                        currentAe.aniList.Add(ani);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        else if (ani.action == PPTAction.exit)
        {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                currentAe.endTime = ani.slideTime;
                audioSlider.gameObject.SetActive(false);
            }
            source.Stop();
            source.clip = null;
        }
    }

    public override void GoDie(float slideTime)
    {
        
        //base.GoDie();
        source.Stop();
        source.clip = null;
    }

    public override void Init()
    {
        base.Init();
        currentAe.aniList.Clear();
        source.clip = null;
        www = null;
        
        source.time = 0;
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            audioSlider.init(source);
        }
    }
    /// <summary>  
    /// 获取当前本地时间戳  
    /// </summary>  
    /// <returns></returns>        
    public long GetCurrentTimeUnix()
    {
        TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
        long t = (long)cha.TotalSeconds;
        return t;
    }
    /// <summary>  
    /// 时间戳转换为本地时间对象  
    /// </summary>  
    /// <returns></returns>        
    public DateTime GetUnixDateTime(long unix)
    {
        //long unix = 1500863191;  
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        DateTime newTime = dtStart.AddSeconds(unix);
        return newTime;
    }

    /// <summary>
    /// 加载音频文件
    /// </summary>
    /// <param name="fileName">关联资源文件名</param>
    /// <returns></returns>
    public IEnumerator loadAudio(string fileName)
    {
        string filePath = "file:///" + PPTGlobal.PPTPath + pageNum + "/" + fileName;
        filePath = filePath.Replace("\\", "/");
        WWW www = new WWW(filePath);
        
        yield return www;
        
        if (www.isDone && www.error == null)
        {
            tmpClip = www.GetAudioClip();
            
        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(filePath);
            tmpClip = null;
        }
    }
    /// <summary>
    /// 播放函数
    /// </summary>
    public void play()
    {
        if (tmpClip != null)
        {
            source.clip = tmpClip;
            if((source.time / source.clip.length) >0.99f){
                source.time = 0f;
            }
            source.Play();
            
        }
    }
    //加载的临时音频
    AudioClip tmpClip;
    public override void InitCompent(int pageNum)
    {
        this.pageNum = pageNum;
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        StartCoroutine(loadAudio(currentAe.fileName));
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            audioSlider.init(source);
            audioSlider.playButton.onClick.AddListener(togglePause);
        }

    }


    public override void Pause()
    {
        if (source != null)
        {
            source.Pause();
        }
    }

}
