using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : BaseController
{
    //图片元素文档对象
    public ImageElement ie;
    //图片UI
    public Image img;
    //是否在显示
    bool isShowing = false;
    public override void Begin()
    {
        //base.Begin();
    }

    public override void Do(float slideTime)
    {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (!isShowing)
            {
                if (ie.appearTime <= slideTime)
                {
                    img.enabled = true;
                    isShowing = true;
                }
            }
            if (isShowing)
            {
                if (ie.endTime <= slideTime)
                {
                    img.enabled = false;
                }
                else
                {
                    this.runAni(ie, slideTime);
                }
            }
        }
         
        }

    public override void GoDie(float slideTime)
    {
        img.enabled = false;
        //base.GoDie();
    }

    public override void Init()
    {
        base.Init();
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            ie.aniList.Clear();
        }
        //base.Init();
        isShowing = false;
        img.enabled = false;
    }

    public override void InitCompent(int pageNum)
    {
        this.pageNum = pageNum;

        InitRect(copy(ie));
        isShowing = false;
        initImage(ie.fileName);
        img.enabled = false;
    }

    public override void InitRect<T>(T t)
    {
        base.InitRect(t);
    }

    public override void Pause()
    {
        //base.Pause();
    }
    /// <summary>
    /// 初始化图片
    /// </summary>
    /// <param name="fileName">关联资源名</param>
    public void initImage(string fileName)
    {
        
        string filepath = PPTGlobal.PPTPath + pageNum + "/" + fileName;
        if (File.Exists(filepath))
        {
            FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            fileStream.Close();
            Texture2D texture = new Texture2D(4096, 4096);

            texture.LoadImage(bytes);
            
            Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            
            img.sprite = sp;

        }
    }
    public override void QuicklyComeIntoBowl(XmlElement element)
    {
        XmlElement currElement = (XmlElement)element.SelectSingleNode("Images/Image[@id='" + ie.id + "']");
        if (currElement == null)
        {
            Debug.Log("currElement == NULL !!!!!!!!!!!!1");
            return;
        }
        if (ie.appearTime >= ie.endTime)
        {
            ie.endTime = float.Parse(element.GetAttribute("totalTime"));
        }
        currElement.SetAttribute("appearTime", ie.appearTime + "");
        currElement.SetAttribute("endTime", ie.endTime + "");
        currElement.RemoveAttribute("shapeId");
    }
    public override void DoSort()
    {
        sortAniList(ie);
    }
    public override void DoAnimation(PPTAnimation ani)
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {

            if (ani.shapeId != ie.shapeId) {
            return;
            }
       
            if (!ie.aniList.Contains(ani))
            {
                ie.aniList.Add(ani);
            }
            else {
                //return;
            }
            lastAniId = -1;
        }
        Debug.Log(ani.id + " - " + ani.action + " - " + ani.type + " - " + ani.waittime);
        if (ani.action == PPTAction.entr)
        {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                ie.appearTime = ani.slideTime;
                PPTController.isExecuted = true;
            }
            img.enabled = true;
            isShowing = true;
        } else if (ani.action == PPTAction.exit) {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                ie.endTime = ani.slideTime;
            }
            isShowing = false;
            img.enabled = false;
            PPTController.isExecuted = true;
        } else if (ani.action == PPTAction.emph) {
            StartCoroutine(ImageEmph());
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
            {
                if (!ie.aniList.Contains(ani))
            {
                ie.aniList.Add(ani);
            }
            lastAniId = ani.id;
            PPTController.isExecuted = true;
            }
        }
    }

    public IEnumerator ImageEmph() {
        int count = 5;
        while (count > 0) {
            count--;
            this.gameObject.transform.RotateAround(this.transform.position,Vector3.forward,10f);
            yield return new WaitForSecondsRealtime(0.1f);
            this.gameObject.transform.RotateAround(this.transform.position, Vector3.forward, -20f);
            yield return new WaitForSecondsRealtime(0.1f);
            this.gameObject.transform.RotateAround(this.transform.position, Vector3.forward, 10f);
            yield return new WaitForSecondsRealtime(0.1f);
        }
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            lastAniId = -1;
        }
    }
}
