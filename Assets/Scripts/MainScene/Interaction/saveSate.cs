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
using System.Text;

[Hotfix]
public class saveSate : MonoBehaviour
{
    public static saveSate instance;


    //书签页面
    public GameObject BookmarkPanel;
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
    public GameObject BookMarkBtn;
    public bool CloseBookMarkComponet = false;

    int nameIndex;
    [LuaCallCSharp]
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    //获取当前属性，保存成书签
    [LuaCallCSharp]
    public void SaveOneBookmark()
    {
        DebugLog.DebugLogInfo("开始保存书签");
        DateTime dt = DateTime.Now;
        string bookmarkName = dt.Month.ToString() + "-" + dt.Day.ToString() + "_" + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString() + "_" + index;
        StartCoroutine(ScreenShot(bookmarkName));       
    }
    [LuaCallCSharp]
    IEnumerator ScreenShot(string shotname)
    {
        //关闭页面
        //  SetBookmarkPanel(false);
        Debug.Log("11111111111111111111111");
        BookMarkInfo bookMarkInfo = new BookMarkInfo()
        {
            //id = index,
            bookmarkType = PublicClass.app.app_id,
            bookmarkName = shotname,
            //  bookmarkPicture = File.ReadAllBytes(imagePath),
            modelState = Vesal_DirFiles.Object2Bytes(new SceneModelState()),
            btnState = Vesal_DirFiles.Object2Bytes(new SceneBtnState()),
            cameraParams = Vesal_DirFiles.Object2Bytes(new CameraParams())
        };
        BookMark bookMark = new BookMark();
        bookMark.markInfo = bookMarkInfo;

        //插入书签封装类对象
       // local_db.DataService("Command.db");
       // local_db.Insert(bookMarkInfo);
      //  local_db.Close();
        //.insertOne<Customer, int>(customer, customer._id);
        DebugLog.DebugLogInfo("创建书签 " + shotname);
        BaseCommand command = new PlayerCommand();
        command.ExecuteCommand();
        DebugLog.DebugLogInfo("书签信息 " + bookMarkInfo);

        string strJson = JsonConvert.SerializeObject(bookMarkInfo);
       // StateData.Instance.StatePath.Add(strJson);

        //string path = @PublicClass.filePath + "StateSave/" + PublicClass.app.app_id+".Json";
        string path = @PublicClass.BookMarkPath + PublicClass.app.app_id+".Json";
        StreamWriter sw;
        FileInfo file = new FileInfo(path);
        if (!file.Exists)
        {
            //如果此文件不存在则创建
            sw = file.CreateText();
        }
        else
        {
            //如果此文件存在则打开
            file.Delete();
            sw = file.CreateText();
        }
        sw.WriteLine(strJson);
        //注意释放资源
        sw.Close();
        sw.Dispose();


        yield return 0;
        Debug.Log("保存完成");
        yield return null;
        //记录数据

        //创建一个新书签，传入当前时间为id，书签名称，装载图片数据流，序列化模型状态，开关状态以及摄像机参数
      
    }

    SceneModelState modelState;
    SceneBtnState btnState;
    CameraParams cameraParams;
    string ma;
   [LuaCallCSharp]
    public void SetMarkData(string path)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path);
        }
        catch (Exception e)
        {

        }
        ma = sr.ReadToEnd();
        //关闭流
        sr.Close();
        //销毁流
        sr.Dispose();
        //将数组链表容器返回




        //StateData.Instance.isReturn = false;

        Debug.Log(ma);
           
      //  string ma = StateData.Instance.StatePath[StateData.Instance.StatePath.Count - 1];
        BookMarkInfo mark = JsonConvert.DeserializeObject<BookMarkInfo>(ma); 
       // mark = book[book.Count - 1];
        BookMarkInfo markInfo =mark;
        Debug.Log(markInfo.ToString());
        //类对象反序列化
        btnState = (SceneBtnState)Vesal_DirFiles.Bytes2Object(markInfo.btnState);
        modelState = (SceneModelState)Vesal_DirFiles.Bytes2Object(markInfo.modelState);
        cameraParams = (CameraParams)Vesal_DirFiles.Bytes2Object(markInfo.cameraParams);
        playRecord = new PlayerCommand(modelState, btnState, cameraParams);
        LoadBookMark();
    }

    IEnumerator loadsavejson(string path)
    {
        WWW wread = new WWW(path);
        yield return wread;
        ma = Encoding.Unicode.GetString(wread.bytes);
    }




    PlayerCommand playRecord;

    public void LoadBookMark()
    {
        DebugLog.DebugLogInfo("打开开关按钮");

        playRecord.ReadBookMark();

        DebugLog.DebugLogInfo("BookMarkManager.instance  " + (BookMarkManager.instance.BookmarkPanel == null));
        DebugLog.DebugLogInfo("BookMarkManager.instance  " + BookMarkManager.instance.BookmarkPanel.name);
        BookMarkManager.instance.SetBookmarkPanel(false);
        CommandManager.instance.ClearRecordStack();
        PublicClass.currentState = RunState.Playing;

    }

}


 