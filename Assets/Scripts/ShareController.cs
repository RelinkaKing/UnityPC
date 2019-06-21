using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VesalCommon;
using System.IO;
using Newtonsoft.Json.Linq;

public class ShareController : MonoBehaviour
{

//    public ShareSDK ssdk;
    public string objname;
    public Camera LogoCamera;                       //水印摄像机
    public bool IsOpenLogoWithScreenShots = false;    //是否在截屏时添加水印
    public bool IsOpenShareFeatures = false;        //是否开启截屏功能；
    int shotIndex = 0;
    string screenshotName;                          //截图名字
    public GameObject SharePanel;                   //分享界面

    public GameObject DrawLinePanel;
    public GameObject ShareUsed;
    /// <summary>
    /// 是否开启水印控制
    /// </summary>
    void IsOpenLogo()
    {
        // LogoCamera.gameObject.SetActive(IsOpenLogoWithScreenShots ? true : false);
    }

  

    public void SetSharePanel(bool iseopen)
    {
        SharePanel.SetActive(iseopen);
    }



    //分享返回
    public void CancelBtn()
    {
        SetSharePanel(false);
        IsOpenShareFeatures = false;
    }

    [SerializeField]
    private Text _text;

    public void StartShot()
    {
        if (DrawLinePanel.activeInHierarchy)
        {
            DrawLinePanel.SetActive(false);
        }

        ShareUsed.SetActive(true);

        StartCoroutine(ScreenShotCut());
    }

    //截屏操作
    IEnumerator ScreenShotCut()
    {
        Vesal_DirFiles.DelFile(PublicClass.filePath + "share.png");
        //添加水印，截屏完成后，自动调用分享界面
#if UNITY_EDITOR
        ScreenCapture.CaptureScreenshot(PublicClass.filePath + "share.png");
        yield return new WaitForSeconds(0.7f);
#else
        //截屏获取图片
        ScreenCapture.CaptureScreenshot("share.png");      //截屏
        while(true)
        {
            if(!File.Exists(PublicClass.filePath + "share.png"))
            {
                yield return null;
            }
            else
            {
                break;
            }
        }
#endif
        yield return null;
        Debug.Log("截屏完成 shot completed");

        DrawLinePanel.SetActive(true);
        ShareUsed.SetActive(false);
        Debug.Log(PublicClass.filePath + "share.png");
        JObject data_obj = new JObject();
        data_obj["PicturePath"] = PublicClass.filePath + "share.png";
        //UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
        //{
        //    name = "share",
        //    data = data_obj,
        //    callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
        //});



        // Logo.gameObject.SetActive(true);
        // Application.CaptureScreenshot(ScreenshotName);      //截屏
        // yield return new WaitForSeconds(0.7f);              //等待
        // Debug.Log("截屏完成");
        // Logo.gameObject.SetActive(false);
        //_text.text = "截屏完成";
        //_text.color = new Color(0,0,0,1);
        //_text.CrossFadeAlpha(0f, 0.3f, true);//淡入
        //_text.CrossFadeAlpha(4f, 1f, true);//淡出
    }


}