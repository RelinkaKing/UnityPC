using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Repository;
using Assets.Scripts.Model;
using VesalCommon;
using XLua;
using Newtonsoft.Json.Linq;
using System.Text;

[Hotfix]
public class BookMarkManager : MonoBehaviour
{
    public static BookMarkManager instance;

    //书签页面
    public GameObject BookmarkPanel;
    public GameObject bookmark;
    //
    public Transform BookmarkParent;
    //
    List<BookMark> BookmarkList;

    int index;

    //List<BookMarkInfo> BookmarkInfoList;

    DbRepository<BookMarkInfo> local_db;
    //public List<Customer> PreBookmarks;
    //BookMarkTool PreBook_tool;
    //BookMarkTool bn_tool;
    public GameObject EditorComplieSprite, EditorSprite, AddBookmarkSprite;
    //书签功能按钮开关
    //public GameObject BookMarkBtn;
    public bool CloseBookMarkComponet = false;

    int nameIndex;
    private void Awake()
    {
        if(instance==null)
            instance = this;
    }
    private void Start()
    {
        if (CloseBookMarkComponet) return;
        BookmarkList = new List<BookMark>();
        //获取本地db库， 记录mark数据表，生成       
        local_db = new DbRepository<BookMarkInfo>();
        local_db.DataService("Command.db");
        local_db.CreateTable();
        local_db.Close();

        local_db.DataService("FixCommand.db");
        local_db.CreateTable();
        local_db.Close();

        ReadBookMark();
        //ReadJson(PublicClass.BookMarkPath);
    }

    public void SaveFixBookmark()
    {
        saveSate.instance.SaveOneBookmark();
    }

    //获取当前属性，保存成书签
    public void SaveOneBookmark()
    {
        DebugLog.DebugLogInfo("开始保存书签");
        BookMark[] marklist = BookmarkParent.GetComponentsInChildren<BookMark>();
        if (marklist.Length == 0)
            index = 0;
        index++;
        DateTime dt=DateTime.Now;
        string bookmarkName =dt.Month.ToString()+"-"+dt.Day.ToString()+"_"+dt.Hour.ToString()+dt.Minute.ToString()+dt.Second.ToString() + "_"+index;
        //bookmarkName = bookmarkName.Replace(':', '_');
        StartCoroutine(ScreenShot(bookmarkName));

    }

    IEnumerator ScreenShot(string shotname)
    {
        //关闭页面
        SetBookmarkPanel(false);

#if UNITY_EDITOR ||UNITY_STANDALONE_WIN

        ScreenCapture.CaptureScreenshot(PublicClass.filePath + shotname + ".png");
        yield return new WaitForSeconds(0.7f);
#else
        //截屏获取图片
        Application.CaptureScreenshot(shotname + ".png");      //截屏
        while(true)
        {
            if(!File.Exists(PublicClass.filePath + shotname + ".png"))
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
        DebugLog.DebugLogInfo("截屏");
        string imagePath = string.Empty;
        // #if UNITY_EDITOR
        //         imagePath = shotname + ".png";
        // #else
        imagePath = PublicClass.filePath + shotname + ".png";
        // #endif
        DebugLog.DebugLogInfo("截屏保存路径：" + imagePath);
        //记录数据

        //创建一个新书签，传入当前时间为id，书签名称，装载图片数据流，序列化模型状态，开关状态以及摄像机参数
        BookMarkInfo bookMarkInfo = new BookMarkInfo()
        {
            //id = index,
            bookmarkType = PublicClass.app.app_id,
            bookmarkName = shotname,
            type = (MainConfig.isSaveFixBookmarkMode) ? "0" : "1",
            bookmarkPicture = File.ReadAllBytes(imagePath),
            modelState = Vesal_DirFiles.Object2Bytes(new SceneModelState()),
            btnState = Vesal_DirFiles.Object2Bytes(new SceneBtnState()),
            cameraParams = Vesal_DirFiles.Object2Bytes(new CameraParams())
        };
        BookMark bookMark = new BookMark();
        bookMark.markInfo = bookMarkInfo;

        //插入书签封装类对象
        if (MainConfig.isSaveFixBookmarkMode)
        {
            local_db.DataService("FixCommand.db");
        }
        else {
            local_db.DataService("Command.db");
        }
        local_db.Insert(bookMarkInfo);
        local_db.Close();
        //.insertOne<Customer, int>(customer, customer._id);
        DebugLog.DebugLogInfo("创建书签 " + shotname);
        BaseCommand command = new PlayerCommand();
        command.ExecuteCommand();
        DebugLog.DebugLogInfo("书签信息 " + bookMarkInfo.bookmarkName);

        //书签列表
        BookmarkList.Add(bookMark);

        CreateBookmarkPanel();

        SetBookmarkPanel(true);
    }

    //int fix_count=0, count=0;
    //bool isStartCheck = false;
    //List<BookMarkInfo> Fix_markInfoList = new List<BookMarkInfo>();
    //void ReadJson(string path)
    //{
    //    List<string> file_list = Vesal_DirFiles.GetAllDirInfoFromPath(path);
    //    isStartCheck = true;
    //    fix_count = file_list.Count;
    //    for (int i = 0; i < file_list.Count; i++)
    //    {
    //        StartCoroutine(loadsavejson(path,SwitchJson));
    //    }
    //}

    //void SwitchJson(string str)
    //{
    //    BookMarkInfo temp_info = JsonTool<BookMarkInfo>.SwitchJsonToClass(str);
    //    if (temp_info.bookmarkName != null && temp_info.bookmarkType == PublicClass.app.app_id)
    //        Fix_markInfoList.Add(JsonTool<BookMarkInfo>.SwitchJsonToClass(str));
    //    count++;
    //}

    //IEnumerator loadsavejson(string path,Action<string> callBack)
    //{
    //    WWW wread = new WWW(path);
    //    yield return wread;
    //    callBack(Encoding.Unicode.GetString(wread.bytes));
    //}

    //void Update()
    //{
    //    if (isStartCheck)
    //    {
    //        if (fix_count == count)
    //        {
    //            ReadBookMark();
    //            isStartCheck = false;
    //        }
    //    }
    //}


    void ReadBookMark()
    {
        local_db.DataService("Command.db");
        IEnumerable<BookMarkInfo> tmpmarkinfo = local_db.Select<BookMarkInfo>((tmp) =>
        {
            if (tmp.bookmarkName != null && tmp.bookmarkType == PublicClass.app.app_id) {
                return true;
            }
            else {
                return false;
            }
       
        });
        
        List<BookMarkInfo> markInfoList = new List<BookMarkInfo>();
        foreach (BookMarkInfo tmp in tmpmarkinfo)
        {
            markInfoList.Add(tmp);
        }
        local_db.Close();

        local_db.DataService("FixCommand.db");
        IEnumerable<BookMarkInfo> fixmarkinfo = local_db.Select<BookMarkInfo>((tmp) =>
        {
            if (tmp.bookmarkName != null && tmp.bookmarkType == PublicClass.app.app_id)
            {
                return true;
            }
            else
            {
                return false;
            }

        });
        foreach (BookMarkInfo tmp in fixmarkinfo)
        {
            markInfoList.Add(tmp);
        }
        local_db.Close();

        for (int i = 0; i < markInfoList.Count; i++)
        {
            BookMark bookMarkExist = new BookMark();
            bookMarkExist.markInfo = markInfoList[i];
            BookmarkList.Add(bookMarkExist);
            CreateBookmarkPanel();
        }
        //BookMarkInfo bookMarkInfo = new BookMarkInfo()
        //{
        //    //id = index,
        //    bookmarkName = shotname,
        //    bookmarkPicture = File.ReadAllBytes(imagePath),
        //    modelState = Vesal_DirFiles.Object2Bytes(new SceneModelState()),
        //    btnState = Vesal_DirFiles.Object2Bytes(new SceneBtnState()),
        //    cameraParams = Vesal_DirFiles.Object2Bytes(new CameraParams())
        //};
        //BookMark bookMark = new BookMark();
        //bookMark.markInfo = bookMarkInfo;

        ////插入书签封装类对象

        //local_db.Insert(bookMarkInfo);
        ////.insertOne<Customer, int>(customer, customer._id);
        //DebugLog.DebugLogInfo("创建书签 " + shotname);
        //BaseCommand command = new PlayerCommand();
        //command.ExecuteCommand();
        //DebugLog.DebugLogInfo("书签信息 " + bookMarkInfo);

        ////书签列表
        //BookmarkList.Add(bookMark);

        //CreateBookmarkPanel();
    }

    ////change bookmark name
    public void UpdateBookmark(BookMarkInfo updateData,string db_name,string new_name)
    {
        local_db.DataService(db_name);
        var tmpmarkinfo = local_db.SelectOne<BookMarkInfo>((tmp) =>
        {
            if (tmp.bookmarkName == updateData .bookmarkName)
            {
                return true;
            }
            else
            {
                return false;
            }

        });
        tmpmarkinfo.bookmarkName = new_name;
        if (tmpmarkinfo != null)
        {
            local_db.Update(tmpmarkinfo);
        }
        else {
            Debug.LogError("rename failed");
        }
        
        local_db.Close();
    }

    ////delete bookmark with name
    public void DeleteBookmark(BookMarkInfo deleteData)
    {
        local_db.DataService("Command.db");
        local_db.Delete(deleteData);
        local_db.Close();
        for (int i = 0; i < BookmarkList.Count; i++)
        {
            if (BookmarkList[i].markInfo == deleteData)
            { BookmarkList.Remove(BookmarkList[i]); }
        }
    }


    public List<string> BookmarkEditorProtect = new List<string>();
    public bool isCreateBookmark = false;
    //生成书签列表
    public void CreateBookmarkPanel()
    {
        isCreateBookmark = true;
        //生产
        ExitEditorModel(true);
        //实例化 List<BookMark> Bookmarks
        for (int i = 0; i < BookmarkList.Count; i++)
        {
            //生成预制体，传递初始化数值
            GameObject tempMark = Instantiate(bookmark);
            tempMark.gameObject.SetActive(true);
            DebugLog.DebugLogInfo("读取书签预制体");
            tempMark.transform.SetParent(BookmarkParent.transform);
            tempMark.transform.localScale = Vector3.one;
            tempMark.transform.localPosition = new Vector3(tempMark.transform.localPosition.x, tempMark.transform.localPosition.y, 1);
            BookMark mark = tempMark.GetComponent<BookMark>();

            
#if UNITY_EDITOR ||UNITY_STANDALONE_WIN
            mark.SetMarkData(BookmarkList[i].markInfo, MoudleController.isLanScaple);
            
#elif UNITY_ANDROID || UNITY_IOS 
            mark.SetMarkData(BookmarkList[i].markInfo, false);
#endif
        }
        BookmarkList.Clear();
        //PreBookmarks.Clear();

        //GameObject tempMark = Instantiate(Resources.Load<GameObject>("Prefab/BookMark"));
        //DebugLog.DebugLogInfo("读取书签预制体");
        //tempMark.transform.SetParent(BookmarkParent.transform);
        //tempMark.transform.localScale = Vector3.one;
        //tempMark.transform.localPosition = new Vector3(tempMark.transform.localPosition.x, tempMark.transform.localPosition.y, 1);
        //BookMark mark = tempMark.GetComponent<BookMark>();
        //mark.SetMarkData(BookmarkList[i].markInfo, false);
        isCreateBookmark = false;
    }

    //编辑书签列表
    public void EditorBookmarkList()
    {
        bool isDone = EditorComplieSprite.activeSelf;
        UIChangeTool.ShowOneObject(EditorSprite, EditorComplieSprite, isDone);
        AddBookmarkSprite.SetActive(isDone);
        BookMark[] marklist = BookmarkParent.GetComponentsInChildren<BookMark>();
        for (int i = 0; i < marklist.Length; i++)
        {
            //if (!BookmarkEditorProtect.Contains(marklist[i].nameText.text))
            //    marklist[i].OpenDeleteUI(!isDone);
            if (!marklist[i].is_fix)
            {
                marklist[i].delete.SetActive(!isDone);
            }
            DebugLog.DebugLogInfo("进入编辑模式，可删除图片");
        }
        DebugLog.DebugLogInfo("进入编辑模式，可删除图片");
    }
    //退出编辑模式
    public void ExitEditorModel(bool isDone)
    {
        //当编辑完成切换编辑图片
        UIChangeTool.ShowOneObject(EditorSprite, EditorComplieSprite, isDone);
        //添加书页图片
        AddBookmarkSprite.SetActive(isDone);
        DebugLog.DebugLogInfo("打开书签");
    }

    public void SetBookmarkPanel(bool switchpanel)
    {
        BookmarkPanel.SetActive(switchpanel);
    }

    private void OnDestroy()
    {
        if (local_db != null)
            local_db.Close();
    }
    private void OnApplicationQuit()
    {
        if (local_db != null)
            local_db.Close();
    }
}