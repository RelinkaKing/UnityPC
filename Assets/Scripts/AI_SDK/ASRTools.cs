using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VesalCommon;

public class ASRTools : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    // 延迟时间  
    private float delay = 0.2f;
    private float speechTimeOut = 5f;
    // 按钮是否是按下状态  
    private bool isDown = false;
    // 按钮最后一次是被按住状态时候的时间  
    private float lastIsDownTime;


    public static string ApiId = "11374797";
    public static string ApiKey = "utetZR4hoYHwGQbAW54TTMvs";
    public static string SecretKey = "r95yd24HesFmaFC3rTD02Wu77FmCWFQY";
    private string token = "wait";
    //语种
    private string lan = "zh";                      
    private string grant_Type = "client_credentials";
    private string baiduAPI = "http://vop.baidu.com/server_api";
    const string OAUTH_URL = "https://aip.baidubce.com/oauth/2.0/token";
    private byte[] clipByte;
    public InputField inputField;
    
    //识别结果
    public string audioToString;
    public Text resultText;
    public Button button;
    AudioClip m_audioClip;
    const int HEADER_SIZE = 44;
    string savePath;

    private void OnEnable()
    {

    }
    void Start() {
        savePath = Application.persistentDataPath + "/asrTemp/";
        if(!Directory.Exists(savePath)){
            Directory.CreateDirectory(savePath);
        }
        if (vesal_network.Vesal_Network.get_network_is_acitve())
        {
            StartCoroutine(getBdToken());
        }
        else {
            token = string.Empty;
        }
    }

    bool isStartSpeeh = false;
    bool isIdentify = false;
    void Update() {
        //if (token == string.Empty || token == "" || token == null) {
        //    if (vesal_network.Vesal_Network.get_network_is_acitve())
        //    {
        //        StartCoroutine(getBdToken());
        //    }
        //    token = "wait";
        //}
        if (token == "wait") {
            if (button.enabled) {
                button.enabled = false;
                this.transform.localPosition -= new Vector3(0,0, 10000);
            }
            return;
        }else{
            if (!button.enabled)
            {
                button.enabled = true;
                this.transform.localPosition += new Vector3(0, 0, 10000);
            }
            if (!button.interactable){
                button.interactable = true;
            }
        }
        // 如果按钮是被按下状态  
        if (isDown && !isStartSpeeh)
        {
            // 当前时间 -  按钮最后一次被按下的时间 > 延迟时间0.2秒  
            if (Time.time - lastIsDownTime > delay)
            {
                // 触发长按方法  
//                Debug.Log("长按");
                // 记录按钮最后一次被按下的时间  
                lastIsDownTime = Time.time;
                isStartSpeeh = true;
                
                StartSpeech();
            }
        }
        if(isDown){
            if (Time.time - lastIsDownTime > speechTimeOut)
            {
                Debug.Log("超时");
                stopSpeech();
                inputField.text = "超时";
                //resultText.text = "超时";
            }
        }
        if (!isDown && isStartSpeeh && !isIdentify) {
            stopSpeech();
            //Debug.Log("识别");
            StartCoroutine(startIdentify());
        }
    }
    public void stopSpeech(){
        isDown = false;
        isIdentify = false;
        isStartSpeeh = false;
        Microphone.End(null);
        button.interactable = false;

    }
    public IEnumerator startIdentify() {
        //float count = time;
        //for (float i = time;i>0;i--) {
        //    resultText.text = i+"";
        //    //等待语音输入 结束
        //    yield return StartCoroutine(wait(1f));

        //}
        //resultText.text = "正在识别······";
        inputField.text = "正在识别······";
        string path = save();
        yield return null;
        if(path!=string.Empty){

        StartCoroutine(AsrData(path));
        }else{
            inputField.text = "识别失败";
            yield return new WaitForSecondsRealtime(1.0f);
            inputField.text = "";
              audioToString = string.Empty;
        isStartSpeeh = false;
        isIdentify = false;
        }

    }

    public IEnumerator wait(float time)
    {
        yield return new WaitForSecondsRealtime(time);
    }
     
    public void StartSpeech()
    {

        //resultText.text = "录音中······";
        inputField.text = "录音中······";
        //resultText.gameObject.SetActive(true);
        m_audioClip = Microphone.Start(null, false, 100, 16000);
        
    }


    public string save() {
        if (!Directory.Exists(savePath)) {
            Directory.CreateDirectory(savePath);
        }
        string path = savePath + Guid.NewGuid().ToString().Replace("-", "") + ".wav";
        try
        {
            
            Save(path, m_audioClip);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return string.Empty;
        }
        return path;
    }

    public bool Save(string filename, AudioClip clip)
    {
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }
        string filepath = Path.Combine(Application.persistentDataPath, filename);
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        using (FileStream fileStream = CreateEmpty(filepath))
        {

            ConvertAndWrite(fileStream, clip);

            WriteHeader(fileStream, clip);
        }

        return true; 
    }

    public static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {

        float[] samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        
        Byte[] bytesData = new Byte[samples.Length * 2];
        //File.WriteAllBytes(Application.persistentDataPath+"/asrtemp/test.wav",bytesData);
        int rescaleFactor = 32767; 

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }


    static void WriteHeader(FileStream fileStream, AudioClip clip)
    {

        int hz = clip.frequency;
        int channels = clip.channels;
        int samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        UInt16 one = 1;//表示PCM

        Byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);
    }

    static FileStream CreateEmpty(string filepath)
    {
        FileStream fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++)
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }


    public void DelectDir(string srcPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName);      //删除指定文件
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    // 识别本地文件
    public IEnumerator AsrData(string path)
    {
        var data = File.ReadAllBytes(path);
//        Debug.Log(path);
        yield return StartCoroutine(GetAudioString(baiduAPI,new RequestBody(token, Convert.ToBase64String(data),data.Length)));
        
        try
        {
            
        DelectDir(savePath);
            //this.gameObject.SendMessage("AsrSelect", audioToString);
            //resultText.text = audioToString;
            inputField.text = audioToString;
            //inputField.text
            this.transform.parent.SendMessage("OnTextChange", inputField,SendMessageOptions.DontRequireReceiver);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            Debug.Log("清空异常");            
        }
        yield return new WaitForSecondsRealtime(2f);
        //resultText.gameObject.SetActive(false);
        //resultText.text = "录音中······";
        audioToString = string.Empty;
        isStartSpeeh = false;
        isIdentify = false;
    }

    /// <summary>
    /// query dic to string
    /// </summary>
    /// <param name="querys"></param>
    /// <returns></returns>
    public string ParseQueryString(Dictionary<string, string> querys)
    {
        if (querys.Count == 0)
            return "";
        return querys
            .Select(pair => pair.Key + "=" + pair.Value)
            .Aggregate((a, b) => a + "&" + b);
    }

    public IEnumerator getBdToken() {
       
        var querys = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", ApiKey},
                {"client_secret", SecretKey}
            };
        var url = string.Format("{0}?{1}", OAUTH_URL, ParseQueryString(querys));
        UnityWebRequest uw = new UnityWebRequest(url);
        uw.method = "POST";
        uw.downloadHandler = new DownloadHandlerTexture();
        uw.timeout = 20;

        yield return uw.Send();

        JObject resp= JsonConvert.DeserializeObject(uw.downloadHandler.text) as JObject;
        if (resp != null && resp["access_token"] != null && resp["expires_in"] != null)
        {
            token = resp["access_token"].ToString();
        }
        else {
            token = "wait";
        }

    }

    [Serializable]
    public class RequestBody{
        public RequestBody(string token,string speech,int len) {
            this.token = token;
            this.speech = speech;
            this.len = len;
        }
        //支持自定义词库
        public string dev_pid = "1536";
        public string format = "wav";
        public string token;
        //byte.legth
        public int len;
        public int channel =1;
        public string cuid = "aa";
        public int rate = 16000;
        //base64
        public string speech;
    }
    
    /// <summary>
    /// 把语音转换为文字
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private IEnumerator GetAudioString(string url, RequestBody rb)
    {
        
        string req = JsonConvert.SerializeObject(rb);

        UnityWebRequest uw = new UnityWebRequest(url,"POST");
        uw.uploadHandler = new UploadHandlerRaw(Encoding.Default.GetBytes(req));

        uw.SetRequestHeader("Content-Type", "application/json");
        uw.downloadHandler = new DownloadHandlerTexture();
        uw.timeout = 20;
        yield return uw.Send();
        if (uw.isDone)
        {
//            Debug.Log("end");
            if (uw.error == null)
            {
                Debug.Log(uw.downloadHandler.text);
                JObject getASWJson = JsonConvert.DeserializeObject(uw.downloadHandler.text) as JObject;
                if (getASWJson["err_msg"].ToString() == "success.")
                {
                    audioToString = getASWJson["result"][0].ToString();
                    if (audioToString.Substring(audioToString.Length - 1) == "，")
                        audioToString = audioToString.Substring(0, audioToString.Length - 1);
                }
            }
            else
            {
                Debug.Log(uw.error);
                audioToString = "";
            }
        }


    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        lastIsDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isDown = false;
    }
}
