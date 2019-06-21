using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

//二维码识别生成控制类
public class QRCode : MonoBehaviour
{
    #region 扫描二维码
    //定义一个用于存储调用电脑或手机摄像头画面的RawImage
    public RawImage m_cameraTexture;

    //摄像头实时显示的画面
    private WebCamTexture m_webCameraTexture;

    //申请一个读取二维码的变量
    private BarcodeReader m_barcodeRender = new BarcodeReader();

    //多久检索一次二维码
    private float m_delayTime = 2.0f;
    #endregion

    #region 生成二维码
    //用于显示生成的二维码RawImage
    public RawImage m_QRCode;

    //申请一个写二维码的变量
    private BarcodeWriter m_barcodeWriter;
    #endregion

    #region 闪光灯
    public Button flashButton;

    //AndroidJavaClass AndroidCamera;
    //AndroidJavaObject androidCamera;
    //AndroidJavaObject cameraParameter;
    bool isFlashOn = false;
    #endregion
    //    04-27 12:08:39.010 10403-10424/? E/Unity: Unable to find AudioPluginMsHRTF
    //04-27 12:08:39.014 10403-10424/? E/Unity: Unable to find AudioPluginOculusSpatializer
    //04-27 12:08:39.018 10403-10424/? E/Unity: Unable to find libAudioPluginOculusSpatializer
    #region 扫描二维码
    void Start()
    {
        height = -Line.transform.parent.GetComponent<RectTransform>().rect.height;
        rt = Line.transform.GetComponent<RectTransform>();
#if UNITY_ANDROID
        //try
        //{
        //    AndroidCamera = new AndroidJavaClass("android.hardware.Camera");
        //    //1：using Vuforia;
        //    //2：CameraDevice.Instance.SetFlashTorchMode(true);
        //    if (flashButton != null)
        //    {
        //        flashButton.onClick.AddListener(delegate
        //        {
        //            if (!isFlashOn)
        //            {
        //                startFlash();
        //            }
        //            else
        //            {
        //                stopFlash();
        //            }
        //            isFlashOn = !isFlashOn;
        //        });
        //    }
        //}
        //catch (Exception e){
        //    Debug.Log(e.Message);
        //    Debug.Log(e.StackTrace);
        //    flashButton.gameObject.SetActive(false);
        //}
#endif
    }
    int RotationAngle = 0;
    public GameObject Line;
    /// <summary>
    /// 获取相机权限
    /// </summary>
    /// <returns></returns>
    public IEnumerator getAuth()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            StartCoroutine(Camera.main.transform.GetComponent<ClassPanelControll>().requestFailed("相机授权失败！"));
            hideQrScanPanel();
        }
        else
        {
            startQrScan();
        }
    }
    /// <summary>
    /// 开始二维码扫描
    /// </summary>
    public void startQrScan()
    {
        try
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                StartCoroutine(getAuth());
            }
            else
            {
                //调用摄像头并将画面显示在屏幕RawImage上
                WebCamDevice[] tDevices = WebCamTexture.devices;    //获取所有摄像头
                string tDeviceName = tDevices[0].name;  //获取第一个摄像头，用第一个摄像头的画面生成图片信息
                m_webCameraTexture = new WebCamTexture(tDeviceName, Screen.width, Screen.width);//名字,宽,高

                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + m_webCameraTexture.videoRotationAngle);
                if (RotationAngle != -90)
                {
                    RotationAngle = -90;
                    m_cameraTexture.transform.rotation = Quaternion.Euler(new Vector3(0, 0, RotationAngle));
                }
                m_cameraTexture.texture = m_webCameraTexture;   //赋值图片信息
                m_webCameraTexture.Play();  //开始实时显示

                InvokeRepeating("CheckQRCode", 0, m_delayTime);
            }
        }
        catch (Exception e)
        {
            Debug.Log("开启扫码失败！！！！！！");
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            StartCoroutine(Camera.main.transform.GetComponent<ClassPanelControll>().requestFailed("开启相机失败！"));
            hideQrScanPanel();
        }
    }
    /// <summary>
    /// 检索二维码方法
    /// </summary>
    void CheckQRCode()
    {
        //存储摄像头画面信息贴图转换的颜色数组
        Color32[] m_colorData = m_webCameraTexture.GetPixels32();

        //将画面中的二维码信息检索出来
        var tResult = m_barcodeRender.Decode(m_colorData, m_webCameraTexture.width, m_webCameraTexture.height);
        Debug.Log(m_delayTime);
        if (tResult != null)
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + tResult.Text);
            QrScanPanel.SetActive(false);
            ClassPanelControll cpc = Camera.main.GetComponent<ClassPanelControll>();
            cpc.inputClassId.text = tResult.Text;
            cpc.inputClassId.transform.parent.GetComponent<InputField>().text = tResult.Text;
            cpc.addClass();
            CancelInvoke();
        }
        if (tmpTime != m_delayTime)
        {
            tmpTime = m_delayTime;
            CancelInvoke();
            InvokeRepeating("CheckQRCode", 0, m_delayTime);
        }
    }
    float tmpTime = 2.0f;
    #endregion
    public void addTime(bool flag)
    {
        if (flag)
        {
            m_delayTime += 0.1f;
        }
        else
        {
            m_delayTime -= 0.1f;
        }
    }
    #region 传递字符串生成二维码
    float height;
    //扫描线
    RectTransform rt;
    private void FixedUpdate()
    {
        if (QrScanPanel.activeSelf)
        {

            if (rt.anchoredPosition.y < (height + 10))
            {
                //rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
            }
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 0.01f * 1000);
        }
    }
    void Update()
    {
        if (QrScanPanel.activeSelf)
        {

            if (rt.anchoredPosition.y < (height + 10))
            {
                //rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
            }
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            //在这种写法中  宽高必须256  否则报错
            ShowQRCode("123456", 256, 256);
        }
#endif
    }
    /// <summary>
    /// 显示绘制的二维码
    /// </summary>
    /// <param name="s_formatStr">扫码信息</param>
    /// <param name="s_width">码宽</param>
    /// <param name="s_height">码高</param>
    void ShowQRCode(string s_str, int s_width, int s_height)
    {
        //定义Texture2D并且填充
        Texture2D tTexture = new Texture2D(s_width, s_height);

        //绘制相对应的贴图纹理
        tTexture.SetPixels32(GeneQRCode(s_str, s_width, s_height));

        tTexture.Apply();

        //赋值贴图
        m_QRCode.texture = tTexture;
    }
    // 二维码扫码面板
    public GameObject QrScanPanel;

    public Text target;
    //显示二维码面板
    public void showQrScanPanel()
    {
        QrScanPanel.SetActive(true);
        startQrScan();
    }
    //切换隐藏面板
    public void hideQrScanPanel()
    {
        QrScanPanel.SetActive(false);
        //if (isFlashOn) {
        //    stopFlash();
        //}
        CancelInvoke();
    }
    /// <summary>
    /// 返回对应颜色数组
    /// </summary>
    /// <param name="s_formatStr">扫码信息</param>
    /// <param name="s_width">码宽</param>
    /// <param name="s_height">码高</param>
    Color32[] GeneQRCode(string s_formatStr, int s_width, int s_height)
    {
        //设置中文编码格式，否则中文不支持
        QrCodeEncodingOptions tOptions = new QrCodeEncodingOptions();
        tOptions.CharacterSet = "UTF-8";
        //设置宽高
        tOptions.Width = s_width;
        tOptions.Height = s_height;
        //设置二维码距离边缘的空白距离
        tOptions.Margin = 3;

        //重置申请写二维码变量类       (参数为：码格式（二维码、条形码...）    编码格式（支持的编码格式）    )
        m_barcodeWriter = new BarcodeWriter { Format = BarcodeFormat.QR_CODE, Options = tOptions };

        //将咱们需要隐藏在码后面的信息赋值上
        return m_barcodeWriter.Write(s_formatStr);
    }
    #endregion
#if UNITY_ANDROID
    #region 闪光灯开关
    //public void startFlash()
    //{
    //    try
    //    {
    //        if (null == androidCamera)
    //        {
    //            androidCamera = AndroidCamera.CallStatic<AndroidJavaObject>("open");
    //        }
    //        if (null == cameraParameter)
    //        {
    //            cameraParameter = androidCamera.Call<AndroidJavaObject>("getParameters");
    //        }
    //        cameraParameter.Call("setFlashMode", new AndroidJavaObject("java.lang.String", "torch"));
    //        androidCamera.Call("setParameters", cameraParameter);
    //    }
    //    catch (AndroidJavaException e)
    //    {
    //        Debug.LogError(e.StackTrace);
    //    }
    //}

    //public void stopFlash()
    //{
    //    try
    //    {
    //        if (null == cameraParameter)
    //        {
    //            cameraParameter = androidCamera.Call<AndroidJavaObject>("getParameters");
    //        }
    //        cameraParameter.Call("setFlashMode", new AndroidJavaObject("java.lang.String", "off"));
    //        androidCamera.Call("setParameters", cameraParameter);
    //        //mCamera = null;
    //        //
    //    }
    //    catch (AndroidJavaException e)
    //    {
    //        Debug.LogError(e.StackTrace);
    //    }
    //}
    #endregion
#endif
}