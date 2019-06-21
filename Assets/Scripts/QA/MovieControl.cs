using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
/// <summary>
/// 视频题控制器
/// </summary>
public class MovieControl : MonoBehaviour
{
    //1播放 0停止
    int isplaying = 0;
    //图像
    public RawImage image;
    //播放器
    public VideoPlayer vPlayer;
    //音源
    public AudioSource source;
    //视频播放预制体
    public GameObject obj;


    void Start()
    {

        vPlayer.playOnAwake = false;
        source.playOnAwake = false;
        source.Pause();
    }

    void OnGUI()
    {

    }

    void Update()
    {
        if (isplaying == 1)
        {

            if (!vPlayer.isPlaying)
            {
                vPlayer.Play();
            }
            if (vPlayer.isPlaying)
            {
                //把图像赋给RawImage
                image.texture = vPlayer.texture;

            }
        }


    }

    /// <summary>
    /// 初始化视频组件
    /// </summary>
    /// <param name="url"></param>
    public void initMovie(string url)
    {
        obj.SetActive(true);
        isplaying = 0;
        vPlayer.playOnAwake = false;
        source.playOnAwake = false;
        source.Pause();
        if (url.Contains("http"))
        {
            //设置为URL模式
            vPlayer.source = VideoSource.Url;
            //设置播放路径
            vPlayer.url = url;
        }
        else
        {
            string filepath = PublicClass.filePath + url;
            if (!File.Exists(filepath))
            {
                obj.SetActive(false);
                transform.GetComponent<PictureControl>().initImage("404");
                return;
            }
            vPlayer.source = VideoSource.VideoClip;
            vPlayer.url = filepath;
        }
        //在视频中嵌入的音频类型
        vPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        //把声音组件赋值给VideoPlayer
        vPlayer.SetTargetAudioSource(0, source);
        //当VideoPlayer全部设置好的时候调用
        vPlayer.prepareCompleted += Prepared;
        //启动播放器
        vPlayer.Prepare();
    }

    public void OnPlay()
    {

    }
    public void OnPause()
    {

    }
    /// <summary>
    /// 停止此题
    /// </summary>
    public void stop()
    {
        transform.GetComponent<PictureControl>().stop();
        isplaying = 0;
        image.texture = null;
        vPlayer.Stop();
        source.Stop();
        obj.SetActive(false);
    }
    /// <summary>
    /// 加载预备
    /// </summary>
    /// <param name="player"></param>
    void Prepared(VideoPlayer player)
    {
        player.Play();
        source.Play();
        isplaying = 1;
    }
}
