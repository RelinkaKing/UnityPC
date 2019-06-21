using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Mono.Data.Sqlite;
using Assets.Scripts.Repository;
using SQLite4Unity3d;
using System.Linq.Expressions;

public class QaDao : MonoBehaviour
{
    //LiteDatabase db;
    //LiteCollection<ErrorQa> errorQaColl;
    //LiteCollection<SignInfo> signInfoColl;

    public string dbName = "QaDataBase";
    public static QaDao instance;
    private void OnApplicationQuit()
    {
        closeDb();
        //   this.CloseSQLConnection();
        // Debug.Log("quit!");
    }
    /// <summary>
    /// 数据库连接对象
    /// </summary>
    private SqliteConnection connection;
    /// <summary>
    /// 数据库命令
    /// </summary>
    private SqliteCommand command;
    /// <summary>
    /// 数据读取定义
    /// </summary>
    private SqliteDataReader reader;

    DbRepository<ErrorQa> errQa;
    //创建db库
    public void openDb()
    {
        closeDb();
        errQa = new DbRepository<ErrorQa>();
        errQa.CreateDb(PublicClass.filePath+"ErrQa.db");
#if UNITY_EDITOR
        errQa.DropTable();
#endif
        errQa.CreateTable();
    }
    public void closeDb() {
        if (this.errQa != null)
        {
            this.errQa.Close();
        }
    }
    private void OnEnable()
    {
        instance = this;
    }



    [Serializable]
    public class ErrorQa
    {
        public ErrorQa() { }

        public int CompareTo(object obj)
        {
            ErrorQa other = obj as ErrorQa;
            if (other == null) return 1;
            return other.count.CompareTo(count) * -1;
        }
        public ErrorQa(string id, int count, string bankName, string libName)
        {
            this.id = id;
            this.count = count;
            this.bankName = bankName;
            this.libName = libName;
        }
        [PrimaryKey]
        [AutoIncrement]
        public int pkid { get; set; }
        public string id { get; set; }
        public int count { get; set; }
        public string bankName { get; set; }
        public string libName { get; set; }
    }
   


    public Dictionary<string, int> getTopErrorQa(int limit)
    {
        openDb();

        TableQuery<ErrorQa> tq = errQa.getTableQuerry();
        tq = tq.OrderByDescending<int>((tmpQa) => (tmpQa.count)) .Take(limit);
        Dictionary<string, int> tmpDic = new Dictionary<string, int>();
        foreach (ErrorQa tmpQa in tq) {
            tmpDic.Add(tmpQa.id, tmpQa.count);
        }
        
        closeDb();
        return tmpDic;
    }

    public List<string> getTopErrorQaId(int limit, string bankName, string libName)
    {
        openDb();
        List<string> tmpList = new List<string>();
        TableQuery<ErrorQa> tq = errQa.getTableQuerry();
        tq = tq.Where((tmpQa) => (
            tmpQa.bankName == bankName && tmpQa.libName == libName
        )).OrderByDescending<int>((tmpQa) => (tmpQa.count)).Take(limit);
        
        foreach (ErrorQa tmpQa in tq)
        {
            tmpList.Add(tmpQa.id);
        }
        closeDb();
        //this.CloseSQLConnection();
        return tmpList;
    }

    public void recordErrorQa(string id, string bankName, string libName, bool isError)
    {
        openDb();
        try
        {
            InsertData(id, bankName, libName, isError);
        }
        catch (SystemException e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
        finally
        {
            closeDb();
            //this.CloseSQLConnection();
        }

    }

    public void InsertData(string id, string bankName, string libName, bool isError)
    {
        

        ErrorQa tmpQa = errQa.SelectOne<ErrorQa>((errQa) => (errQa.id == id && errQa.bankName == bankName && errQa.libName == libName));
       
        int count = isError ? 1 : -1;
        if (tmpQa != null)
        {
            count += tmpQa.count;
        }
        if (count > 0)
        {
            if (tmpQa != null)
            {
                tmpQa.count = count;
                //errorQaColl.Update(tmpQa);
                updateEqa(tmpQa);
            }
            else
            {
                tmpQa = new ErrorQa(id, count, bankName, libName);
                //errorQaColl.Insert(tmpQa);
                insertEqa(tmpQa);
            }
        }
        else
        {
            if (tmpQa != null)
            {
                //errorQaColl.Delete(tmpQa.id);
                deleteEqa(tmpQa);
            }
        }
    }
    public void insertEqa(ErrorQa e)
    {
        if (errQa != null) {
            errQa.Insert(e);
        }
    }
    public void updateEqa(ErrorQa e) {
        if (errQa != null) {
            errQa.Update(e);
        }
    }
    public void deleteEqa(ErrorQa e)
    {
        if (errQa != null)
        {
            errQa.Delete(e);
        }
    }

   
    private void OnDestroy() {
        closeDb();
    }
}