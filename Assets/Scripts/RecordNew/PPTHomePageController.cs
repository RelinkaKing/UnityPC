using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VesalCommon;
/// <summary>
/// ※PptPlayer 播放器场景逻辑入口
/// </summary>
public class PPTHomePageController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }
    bool firstEnter = true;
    //解压文件至临时文件夹
    public static string TempFilePath = string.Empty;
    //弹窗选择文件路径
    public static string tmpSelectPath = string.Empty;

    //播放按钮
    public GameObject playButton;
    //提示文字
    public Text notText;
    //选项父物体
    public Transform parentTransForm;
    //单选框组件
    public ToggleGroup tg;
    //提示找不到文件
    public GameObject emptyFilePanel;
    public Text emptyFileText;

    //当前选择的文件UI子项
    public GameObject currentFileObj;
    //删除
    public GameObject deleButton;
    //当前选项文件全路径
    public static string currentFilePath;
    /// 搜索到的文件字典，key 文件全路径，value 文件选项
    Dictionary<string, GameObject> fileDic = new Dictionary<string, GameObject>();

    /// <summary>
    /// 初次加载判断，若在DataManager里接收文件路径参数则跳过解压缩，否则在此处查找命令行参数是否指向文件。
    /// </summary>
    void Update()
    {
        if (firstEnter)
        {
            if (PPTResourcePool.isSkipUnzip)
            {
                //play();
                Debug.Log("gameObject.BroadcastMessage");
                gameObject.BroadcastMessage("PPTInit");
            }
            else
            {
                Debug.Log("gameObject.BroadcastMessage-----------------");
                string[] CommandLineArgs = Environment.GetCommandLineArgs();
                foreach (string tmpStr in CommandLineArgs)
                {

                    if (tmpStr.Contains(".vsl") || tmpStr.Contains(".VSL"))
                    {
                        Debug.Log("GetCommandLineArgs::" + tmpStr);
                        //SelectFile(tmpStr);
                        currentFilePath = tmpStr;
                        play();
                        break;
                    }
                }
            }
#if UNITY_EDITOR
            //SelectFile("D:\\git\\PPTPlayerOutPut\\WK0000001_5.vsl");
            //play();
            //currentFilePath = "D:\\git\\PPTPlayerOutPut\\演示文稿1.vsl";
            //play();
#endif
            firstEnter = false;
        }
    }
   
    public void exit()
    {
        Application.Quit();
    }

    public void OnEnable()
    {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer && PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.WeiKePlayer)
        {
            transform.GetComponent<PPTHomePageController>().enabled = false;
            return;
        }
        //regist();
        deleButton.SetActive(false);
        notText.text = "请选择vsl文件！";
        if (!PPTResourcePool.isSkipUnzip)
        {
            TempFilePath = getTempPath();
            refreshFile();
        }
    }
    public static string getTempPath()
    {
        //TempFilePath = @Environment.GetEnvironmentVariable("USERPROFILE") + "\\LOCALS~1\\Temp\\";
        string tempPath = Application.temporaryCachePath + "\\";
        //tempPath = @"C:\\VesalTemp\\";
        tempPath += "VesalTemp\\";
        if (!Directory.Exists(tempPath))
        {
            Directory.CreateDirectory(tempPath);
            //tempPath = @"C:\\VesalTemp\\";
        }


        try
        {
            DelectDir(tempPath);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        string id = System.Guid.NewGuid().ToString("N") + " Z/";
        tempPath += id;
        tempPath = tempPath.Replace("\\", "/");
        Directory.CreateDirectory(tempPath);
        Debug.Log(tempPath);
        return tempPath;
    }
    public void PPTEnd()
    {
        notText.text = "请选择vsl文件！";
    }

    /// <summary>
    /// 删除所选文件
    /// </summary>
    public void deleteFile()
    {
        if (currentFileObj == null)
        {
            return;
        }

        Destroy(currentFileObj);
        string path = currentFileObj.GetComponentInChildren<Text>().text;
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception E)
        {
            Debug.Log(E.Message);
        }
        Transform tmpTfs = null;
        for (int i = 0; i < tg.transform.childCount; i++)
        {
            tmpTfs = tg.transform.GetChild(i);
            if (tmpTfs.GetComponentInChildren<Text>().text == path)
            {
                continue;
            }
            else
            {
                break;
            }
        }
        if (tmpTfs == null || tmpTfs.GetComponentInChildren<Text>().text == path)
        {

            currentFilePath = string.Empty;
            emptyFilePanel.SetActive(true);
            emptyFileText.text = "未检测到vsl文件，请刷新或手动选择！";
            deleButton.SetActive(false);
        }
        else
        {
            deleButton.SetActive(true);
            Toggle tmpTog = tmpTfs.GetComponentInChildren<Toggle>();
            tmpTog.isOn = true;
            currentFileObj = tmpTog.gameObject;
            currentFilePath = tmpTfs.GetComponentInChildren<Text>().text;
        }

    }
    /// <summary>
    /// 缩放RectTransform 宽度
    /// </summary>
    /// <param name="obj"></param>
    public void setSizeDelta(GameObject obj)
    {
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 1920f * obj.transform.GetComponent<RectTransform>().sizeDelta.x * 0.95f, obj.transform.GetComponent<RectTransform>().sizeDelta.y);
    }

    /// <summary>
    /// 添加文件列表选项
    /// </summary>
    /// <param name="tmpInfo"></param>
    /// <returns></returns>
    public GameObject addItem(FileInfo tmpInfo)
    {
        GameObject g = Instantiate(Resources.Load<GameObject>("Prefab/PPT/fileItem"), parentTransForm);
        g.SetActive(true);
        //g.GetComponent<RectTransform>().anchoredPosition = new Vector2(g.GetComponent<RectTransform>().anchoredPosition.x, originObject.GetComponent<RectTransform>().anchoredPosition.y);
        g.GetComponentInChildren<Text>().text = tmpInfo.FullName.Replace("\\", "/");
        fileDic.Add(tmpInfo.FullName.Replace("\\", "/"), g);
        g.transform.Find("Time").GetComponent<Text>().text = tmpInfo.CreationTime + " ~ " + tmpInfo.LastWriteTime;
        setSizeDelta(g);
        //setSizeDelta(g.transform.Find("Text").gameObject);
        //setSizeDelta(g.transform.Find("Time").gameObject);
        //setSizeDelta(g.transform.Find("Background").gameObject);
        //setSizeDelta(g.transform.Find("bottomLine").gameObject);
        Toggle t = g.GetComponent<Toggle>();
        t.group = tg;
        UIEventListener btnListener = t.gameObject.AddComponent<UIEventListener>();
        btnListener.OnClickEvent += delegate (GameObject gb)
        {
            this.SelectFile(gb);
        };
        return g;
    }
    /// <summary>
    /// 刷新文件列表视图
    /// </summary>
    /// <param name="fiList"></param>
    public void refreshScrollView(List<FileInfo> fiList)
    {
        bool flag = true;
        foreach (FileInfo tmpInfo in fiList)
        {

            GameObject g = addItem(tmpInfo);
            Toggle t = g.GetComponent<Toggle>();
            if (flag)
            {
                t.isOn = true;
                currentFileObj = g;
                deleButton.SetActive(true);
                SelectFile(g);
                flag = false;
            }
            else
            {
                t.isOn = false;
            }
        }
        emptyFilePanel.SetActive(false);
        notText.text = "请选择vsl文件！";
    }
    /// <summary>
    /// 全盘符搜索vesalplayer
    /// </summary>
    public void refreshFile()
    {
        notText.text = "请选择vsl文件！";
        int sumTg = tg.transform.childCount;
        for (int i = 0; i < sumTg; i++)
        {
            Destroy(tg.transform.GetChild(i).gameObject);
        }
        fileDic.Clear();
        emptyFilePanel.SetActive(true);
        emptyFileText.text = "正在搜索vsl文件！";

        List<FileInfo> tmpDiList = new List<FileInfo>();

        //65-90
        for (int i = 65; i < 91; i++)
        {
            String tmpStr = (char)i + ":/";
            try
            {
                if (Directory.Exists(tmpStr))
                {
                    tmpStr += "vesalplayer/";
                    if (!Directory.Exists(tmpStr))
                    {
                        Directory.CreateDirectory(tmpStr);
                    }
                    else
                    {

                        String[] tmpFis = Directory.GetFiles(tmpStr, "*.vsl");
                        foreach (String tmpFile in tmpFis)
                        {
                            Debug.Log(tmpFile);
                            FileInfo fi = new FileInfo(tmpFile);
                            if (fi.Extension.ToLower() == ".vsl")
                            {
                                tmpDiList.Add(fi);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }
        //SelectQuery selectQuery = new SelectQuery("select * from win32_logicaldisk");
        //ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);
        //int i = 0;
        //foreach (ManagementObject disk in searcher.Get())
        //{
        //    //获取驱动器盘符
        //    this.listBox1.Items.Add(disk["Name"].ToString());
        //}



        if (tmpDiList.Count == 0)
        {
            emptyFileText.text = "未检测到vsl文件，请刷新或手动选择！";
        }
        else
        {
            refreshScrollView(tmpDiList);
        }
    }

    /// <summary>
    /// 选择文件列表中一项
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="unzip"></param>
    public void SelectFile(GameObject obj, bool unzip = true)
    {
        currentFileObj = obj;

        currentFilePath = obj.GetComponentInChildren<Text>().text;
        obj.GetComponentInChildren<Toggle>().isOn = true;
        Debug.Log(currentFilePath);
        if (unzip)
        {
            Debug.Log("unzip");
            //SelectFile(currentFilePath);
        }
    }
    /// <summary>
    /// 选择文件
    /// </summary>
    /// <param name="filepath"></param>
    public void SelectFile(string filepath = "")
    {
        //notText.text = "正在解析......";
        if (filepath == "")
        {
            tmpSelectPath = getFile(); //getFilePath();//getFile();
            filepath = tmpSelectPath.Replace("\\", "/"); ;
        }
        else
        {
            tmpSelectPath = filepath;
        }

        tmpSelectPath = tmpSelectPath.Replace("\\", "/");
        try
        {
            DelectDir(TempFilePath);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        //tmpSelectPath = extractFile(tmpSelectPath);



        PPTGlobal.PPTPath = tmpSelectPath;
        PPTGlobal.PPTPath = PPTGlobal.PPTPath.Replace("\\", "/");
        if (!PPTGlobal.PPTPath.EndsWith("/"))
        {
            PPTGlobal.PPTPath += "/";
        }
        //if (!File.Exists(PPTGlobal.PPTPath + "ppt_control.xml"))
        //{
        //    notText.text = "请选择有效的vsl文件！";
        //}
        //else {
        //notText.text = "请选择vsl文件,点击播放！";
        if (fileDic.ContainsKey(filepath))
        {
            SelectFile(fileDic[filepath], false);
        }
        else
        {
            GameObject g = addItem(new FileInfo(filepath));
            SelectFile(g, false);
        }
        //SwitchToThisWindow(System.Diagnostics.Process.GetCurrentProcess().Handle,true);
        //ShowWindow(GetForegroundWindow(), 3);
        //notText.text = PPTGlobal.PPTPath;
        //}
    }
    //public string lastPlayPath = string.Empty;
    
    /// <summary>
    /// 播放
    /// </summary>
    public void play()
    {
        SelectFile(currentFilePath);
        //if (lastPlayPath != currentFilePath) {
        //    lastPlayPath = currentFilePath;
        //}
        Debug.Log("play " + PPTGlobal.PPTPath);
        //if (!File.Exists(PPTGlobal.PPTPath + "ppt_control.xml"))
        //{
        //    notText.text = "请选择有效的vsl文件！";
        //}
        //else {
        //}
        gameObject.BroadcastMessage("PPTInit");
    }
    /// <summary>
    /// 解压缩文件并返回临时文件夹路径
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public string extractFile(string filePath)
    {

        Vesal_DirFiles.UnZip(filePath, TempFilePath);
        DirectoryInfo di = new DirectoryInfo(TempFilePath);
        try
        {
            return di.GetDirectories()[0].FullName;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        return string.Empty;
    }
    /// <summary>
    /// 删除文件夹
    /// </summary>
    /// <param name="srcPath"></param>
    public static void DelectDir(string srcPath)
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
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern System.IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    /// <summary>
    /// 弹窗选择文件
    /// </summary>
    /// <returns></returns>
    public string getFile()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        //ofn.filter = "Excel files(*.xls)|*.xls|Word files(*.doc)|*.doc;";
        ofn.filter = "*.vsl\0*.VSL\0\0";
        //ofn.filter = "All Files\0*.*\0\0";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;

        ofn.initialDir = Application.streamingAssetsPath; //默认路径

        ofn.defExt = "VSL";

        //注意 一下项目不一定要全选 但是0x00000008项不要缺少
        //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST|OFN_NOCHANGEDIR
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        GetOpenFileName(ofn);

        Debug.Log("Selected file with full path:" + ofn.file);

        return ofn.file;
    }
    /// <summary>
    /// 弹窗选择文件夹
    /// </summary>
    /// <returns></returns>
    public string getFilePath()
    {
        OpenDialogDir ofn2 = new OpenDialogDir();

        ofn2.pszDisplayName = new string(new char[2000]); ;     // 存放目录路径缓冲区    
        ofn2.lpszTitle = "Open Folder";// 标题    
                                       //ofn2.ulFlags = ofn2.BIF_EDITBOX; 崩溃


        IntPtr pidlPtr = DllOpenFileDialog.SHBrowseForFolder(ofn2);
        char[] charArray = new char[2000];
        for (int i = 0; i < 2000; i++)
            charArray[i] = '\0';

        DllOpenFileDialog.SHGetPathFromIDList(pidlPtr, charArray);
        string fullDirPath = new String(charArray);
        fullDirPath = fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));

        return fullDirPath;
    }
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}



[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDialogDir
{
    public IntPtr hwndOwner = IntPtr.Zero;
    public IntPtr pidlRoot = IntPtr.Zero;
    public String pszDisplayName = null;
    public String lpszTitle = null;
    public UInt32 ulFlags = 0;
    public IntPtr lpfn = IntPtr.Zero;
    public IntPtr lParam = IntPtr.Zero;
    public int iImage = 0;
}

public class DllOpenFileDialog
{

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

}
