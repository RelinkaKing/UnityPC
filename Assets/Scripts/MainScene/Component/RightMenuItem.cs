using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RightMenuItem : MonoBehaviour
{

    public Image image;
    //public RectTransform bgimageTf;
    public Image blueImgeTf;
    public Image grayImgeTf;
    public RectTransform imageTf;
    RightMenuGroup rmg;
    bool isAdd;

    bool gyinfo;
    Gyroscope go;
    void Start()
    {
        gyinfo = SystemInfo.supportsGyroscope;
        go = Input.gyro;
        go.enabled = true;
    }
    //累计时间
    private float AccumilatedTime = 0f;
    //刷新时间
    private float FrameLength = 0.5f;

    void Update()
    {

        AccumilatedTime = AccumilatedTime + Time.deltaTime;
        if (AccumilatedTime > FrameLength)
        {
            if (isInit && (PublicClass.appstate== App_State.Running))
            {
                RefreshImage();
            }
            AccumilatedTime = 0;
        }

    }
    bool isInit = false;
    public void InitRightMenuItem(RightMenuGroup rmg)
    {
        this.rmg = rmg;
        if (rmg.layers.Count > 2)
        {
            //pass
        }
        else if (rmg.layers.Count == 2)
        {

            rmg.removeImage_base64 = rmg.normalImage_base64;
            rmg.addImage_base64 = rmg.disableImage_base64;
        }
        else if (rmg.layers.Count == 1)
        {
            rmg.minlayerNo = 0;
            rmg.layers.Add(0, new RightMenuLayer());
            InitRightMenuItem(rmg);
            return;
        }
        else
        {
            Destroy(this.gameObject);
        }

        try
        {
            InitImg();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            Destroy(this.gameObject);
        }
        isInit = true;
    }

    public void InitImg()
    {
        rmg.addImage = Base64ToTexture2d(rmg.addImage_base64);
        rmg.removeImage = Base64ToTexture2d(rmg.removeImage_base64);

        rmg.addImage_base64 = string.Empty;
        rmg.removeImage_base64 = string.Empty;
        rmg.normalImage_base64 = string.Empty;
        rmg.disableImage_base64 = string.Empty;
    }

    public Sprite Base64ToTexture2d(string Base64STR)
    {
        Texture2D pic = new Texture2D(247, 247, TextureFormat.RGBA4444, false, true);
        byte[] data = System.Convert.FromBase64String(Base64STR);
        pic.LoadImage(data);
        //data = pic.EncodeToPNG();
        //pic.LoadImage(data);

        //pic = new Texture2D(4096, 4096, TextureFormat.RGBA4444, false, true);
        //pic.LoadImage(File.ReadAllBytes(PublicClass.filePath + "tmp.PNG"));
#if UNITY_EDITOR
        // File.WriteAllBytes(PublicClass.filePath + System.Guid.NewGuid().ToString("N") + "tmp.PNG", data);
        // Debug.Log(data.Length);
        //File.WriteAllBytes(PublicClass.filePath + System.Guid.NewGuid().ToString("N")+"tmp.PNG", data);
#endif
        //pic.Compress(true);
        //pic.filterMode = FilterMode.Bilinear;

        //AssetBundle ab = AssetBundle.LoadFromMemory(File.ReadAllBytes(PublicClass.filePath + "pifu.assetbundle"));
        //GameObject tmp = ab.LoadAsset<GameObject>("pifu") as GameObject;
        //tmp = GameObject.Instantiate(tmp);
        //Sprite result = tmp.GetComponentInChildren<Image>().sprite;
        //pic = result.texture;
        //Destroy(tmp);
        //ab.Unload(false);
        return Sprite.Create(pic, new Rect(0, 0, pic.width, pic.height), new Vector2(0.5f, 0.5f));
    }

    public void ChangeLayer(bool flag = false)
    {
        if (!flag)
        {
            CommandManager.instance.RecordModelStata();
        }
        isInit = false;
        RefreshImage();
        Debug.Log(isAdd);
        if (isAdd)
        {
            addLayer();
        }
        else
        {
            removeLayer();
        }
        RefreshImage();
        isInit = true;

    }

    //隐藏一些模型
    public void HideAllLayer(string itemName)
    {

        if (gameObject.name != itemName) return;
        while (!isAdd)
        {
            ChangeLayer(true);

        }

    }

    //显示隐藏模型

    public void ShowAllLayer(string itemName)
    {
        if (gameObject.name != itemName) return;
        while (isAdd)
        {
            ChangeLayer(true);

        }


    }

    public void Record_txt(string layer,List<Model> models)
    {
        string str="";
        foreach (var item in models)
        {
            str+=","+item.name;
        }
        vesal_log.vesal_write_log("layer :"+layer+"-------------"+str);
    }


    public void addLayer()
    {

        while (rmg.currentlayerNo < rmg.maxlayerNo)
        {
            rmg.currentlayerNo += 1;
            if (rmg.layers.ContainsKey(rmg.currentlayerNo))
            {
                Record_txt(rmg.currentlayerNo.ToString(),rmg.layers[rmg.currentlayerNo].models);
                foreach (Model tmpModel in rmg.layers[rmg.currentlayerNo].models)
                {
                    tmpModel.BecomeDisplay();
                    tmpModel.BecomeNormal();
                }

                return;
            }
        }

    }

    public void removeLayer()
    {

        while (rmg.currentlayerNo > rmg.minlayerNo)
        {
            if (rmg.layers.ContainsKey(rmg.currentlayerNo))
            {
                foreach (Model tmpModel in rmg.layers[rmg.currentlayerNo].models)
                {
                    tmpModel.BecomeHide();

                }

                rmg.currentlayerNo -= 1;
                return;
            }
            rmg.currentlayerNo -= 1;
        }

    }
    public List<Model> models = new List<Model>();
    public List<Model> getguge()
    {
        models.Clear();
        foreach (Model tmpModel in rmg.layers[1].models)
        {
            models.Add(tmpModel);
         }
        return models;
    }

    //更新图标
    public void RefreshImage()
    {

        int tmpLayer = getCurrentMaxLayer();
        rmg.currentlayerNo = tmpLayer;
        if (rmg.currentlayerNo == rmg.minlayerNo)
        {
            isAdd = true;
            if (image.sprite != rmg.addImage)
            {

                image.sprite = rmg.addImage;

            }
        }
        else if (rmg.currentlayerNo == rmg.maxlayerNo)
        {
            isAdd = false;
            if (image.sprite != rmg.removeImage)
            {
                image.sprite = rmg.removeImage;
            }
        }
        //bgimageTf.sizeDelta = new Vector2(imageTf.sizeDelta.x, imageTf.sizeDelta.y * (rmg.maxlayerNo- rmg.currentlayerNo) / (float)rmg.maxlayerNo);
        blueImgeTf.fillAmount = (rmg.currentlayerNo) / (float)rmg.maxlayerNo;
        //if (gyinfo)
        //{
        //    Vector3 a = go.attitude.eulerAngles;
        //    a = new Vector3(-a.x, -a.y, a.z); 
        //最终角度不每次都转
        //    blueImgeTf.transform.Rotate(a);
        //    bgimageTf.transform.Rotate(a);
        //}
    }
    public int getCurrentMaxLayer()
    {
        int tmpLayer = rmg.maxlayerNo;
        for (; tmpLayer > rmg.minlayerNo; tmpLayer--)
        {
            if (rmg.layers.ContainsKey(tmpLayer))
            {
                foreach (Model tmpModel in rmg.layers[tmpLayer].models)
                {
                    if (tmpModel.isActive)
                    {
                        return tmpLayer;
                    }
                }
            }
        }
        return rmg.minlayerNo;
    }
}
public class RightMenuGroup
{
    public string groupName;
    public int currentlayerNo;
    public int minlayerNo;
    public int maxlayerNo;
    public Dictionary<int, RightMenuLayer> layers = new Dictionary<int, RightMenuLayer>();

    public string addImage_base64;
    public Sprite addImage;
    public string removeImage_base64;
    public Sprite removeImage;
    public string normalImage_base64;
    public Sprite normalImage;
    public string disableImage_base64;
    public Sprite disableImage;

}
public class RightMenuLayer
{
    public List<Model> models = new List<Model>();
}