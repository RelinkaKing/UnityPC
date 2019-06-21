using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class BgController : BaseController
{
    //Dictionary<int, BackGroundElement> bgDic;
    public Transform background;
    Image img;
    public BackGroundElement currentBge;
    bool isBgShowing = false;
    int index = 0;
    
    //public void initBgDic(BackGroundElement[] bgs)
    //{
    //    bgDic = initDic(bgs);
    //}
    public override void Do(float slideTime) {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (currentBge != null && !currentBge.isEnd)
            {
                if (!isBgShowing && currentBge.appearTime <= slideTime && currentBge.endTime > slideTime)
                {
                    initImage(currentBge.fileName);
                }
                else if (currentBge.endTime <= slideTime)
                {
                    index++;
                    isBgShowing = false;

                    currentBge.isEnd = true;

                }
            }
        }
        }

    public override void Begin()
    {
        //base.Begin();
    }

    public override void GoDie(float slideTime)
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {

            if (currentBge.endTime == 0f) {
            currentBge.endTime = slideTime;
            }
        }
        //base.GoDie();
    }

    public override void Init()
    {
        base.Init();
        isBgShowing = false;
        index = 0;
        if (currentBge != null) {
            currentBge.isEnd = false;
        }
        img = background.GetComponent<Image>();
        if (currentBge.appearTime == 0.0)
        {
            initImage(currentBge.fileName);
        }
        
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            //currentBge.appearTime = 0f;
            currentBge.aniList.Clear();
        }
    }
    /// <summary>
    /// 初始化背景图片
    /// </summary>
    /// <param name="fileName"></param>
    public void initImage(string fileName)
    {
        isBgShowing = true;
        string filepath = PPTGlobal.PPTPath +pageNum+"/"+ fileName;
        if (File.Exists(filepath))
        {
            FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            fileStream.Close();
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32,false,true);
            texture.LoadImage(bytes);
            Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f),100);
            img.sprite = sp;
        }
    }
    

    public override void Pause()
    {
        //base.Pause();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void InitCompent(int pageNum)
    {
        this.pageNum = pageNum;
        Init();
    }
    
    public override void DoAnimation(PPTAnimation ani)
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            if (ani.shapeId != currentBge.shapeId)
            {
                return;
            }
            if (!currentBge.aniList.Contains(ani)) {
                currentBge.aniList.Add(ani);
            }
            PPTController.isExecuted = true;
        }
        if (ani.action == PPTAction.entr) {
            currentBge.appearTime = ani.slideTime;
            initImage(currentBge.fileName);
        } else if (ani.action == PPTAction.exit) {
            currentBge.endTime = ani.slideTime;
            img.sprite = null;
        }
    }
    public override void DoSort()
    {
        sortAniList(currentBge);
    }
    public override void QuicklyComeIntoBowl(XmlElement element)
    {

        XmlElement currElement =  (XmlElement)element.SelectSingleNode("BackGrounds/BackGround[@id='" + currentBge.id+"']");
        if (currElement == null)
        {
            Debug.Log("currElement == NULL !!!!!!!!!!!!1");
            return;
        }

        currElement.RemoveAllAttributes();
        currElement.SetAttribute("appearTime", currentBge.appearTime+"");
        if (currentBge.appearTime>= currentBge.endTime) {
            currentBge.endTime = float.Parse(element.GetAttribute("totalTime"));
        }
        currElement.SetAttribute("endTime", currentBge.endTime + "");
        currElement.SetAttribute("id", currentBge.id + "");
        currElement.SetAttribute("Filename", currentBge.fileName+ "");
    }
}
