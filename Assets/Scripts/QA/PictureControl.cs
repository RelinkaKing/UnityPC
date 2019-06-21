using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
/// <summary>
/// 图片题控制器
/// </summary>
public class PictureControl : MonoBehaviour
{
    //图片预制体
    public GameObject obj;
    //显示图片
    public Image image;
    int width = 1080;
    int height = 640;

    void Start()
    {

    }

    void Update()
    {

    }

    /// <summary>
    /// 初始化图片组件参数
    /// </summary>
    /// <param name="url"></param>
    public void initImage(string url)
    {
        obj.SetActive(true);
        Sprite sp = null;
        Debug.Log(url);
        Texture2D texture = null;
        texture = new Texture2D(width, height);
        //在线
        if (url.Contains("http"))
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.Send();
            while (!request.isDone)
            {
                Debug.Log("loading image");
            }
            Debug.Log("done");

            texture.LoadImage(request.downloadHandler.data);

        }
        else
        {
            //本地加载
            string filepath;
            filepath = PublicClass.filePath + url;
            Debug.Log(filepath);
            if (!File.Exists(filepath))
            {
                //加载404图片
                filepath = GlobalVariable.missingPicPath;
            }
            FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            fileStream.Close();
            texture.LoadImage(bytes);
        }
        sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        image.sprite = sp;
    }
    /// <summary>
    /// 停止显示此题
    /// </summary>
    public void stop()
    {
        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }
    }
}
