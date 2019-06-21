using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Mono.Data.Sqlite;
using UnityEngine;

public class BookMarkTool : MonoBehaviour,IBookMarkTool
{
    private void OnApplicationQuit()
    {
        closeDb();
        
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

    string dbName;

    //static bool tmpFlag = false;
    //创建db库
    public void openDb(string dbName)
    {
        // if (!tmpFlag) {
        //     tmpFlag = true;
        //     this.transition();
        // }
        if (this.connection != null) {
            this.connection.Close();
        }
        this.dbName = dbName;
        //db = new LiteDatabase(PublicClass.filePath  + "/" + dbName + ".db");
        this.connection = new SqliteConnection("data source=" + PublicClass.filePath + dbName + ".db");
        this.connection.Open();
        createTable(dbName);
        //if (!File.Exists(Application.streamingAssetsPath + "/" + dbName))
        //{
        //    //this.connection.Open();
        //    //this.CreateSQLTable(
        //    //    dbName,
        //    //    "CREATE TABLE "+ dbName + "(" +
        //    //    "ID             INT ," +
        //    //    "blobObj           blob,"
        //    //);
        this.connection.Close();
        //    return;
        //}
    }
    public void openDb()
    {
        this.connection = new SqliteConnection("data source=" + PublicClass.filePath + dbName + ".db");
        this.connection.Open();
    }
    public void createTable(string tableName) {

        this.CreateSQLTable(
            tableName,
            "CREATE TABLE IF NOT EXISTS " + tableName + "(" +
            "ID             VARCHAR(255)," +
            "data           blob)"
        );
        this.CreateSQLTable(
            "Customer",
            "CREATE TABLE IF NOT EXISTS Customer (" +
            "ID             VARCHAR(255)," +
            "data           blob)"
        );
        this.CreateSQLTable(
            "Customer",
            "CREATE TABLE IF NOT EXISTS DB_PlayerCommand (" +
            "ID             VARCHAR(255)," +
            "data           blob)"
        );
        
    }
    //public void setCon() {
    //    openDb(dbName);
    //    this.connection = new SqliteConnection("data source=" + PublicClass.filePath + dbName);
    //    if (this.connection != null)
    //    {
    //        this.connection.Open();
    //    }
    //}

    public void closeCon() {
        if (this.connection != null) {
            this.connection.Close();
        }
    }
    /// <summary>
    /// 通过调用SQL语句，在数据库中创建一个表，顶定义表中的行的名字和对应的数据类型
    /// </summary>
    public SqliteDataReader CreateSQLTable(string tableName, string commandStr = null)
    {

        return ExecuteSQLCommand(commandStr);
    }

    /// <summary>
    ///执行SQL命令,并返回一个SqliteDataReader对象
    /// <param name="queryString"></param>
    public SqliteDataReader ExecuteSQLCommand(string queryString,SqliteParameter sp = null)
    {
        
        this.command = this.connection.CreateCommand();
        this.command.CommandText = queryString;
        if (sp != null) {
            this.command.Parameters.Add(sp);
        }
        this.reader = this.command.ExecuteReader();
        return this.reader;
    }


    public void openNewDb(string dbName)
    {
        try
        {
        if(File.Exists(PublicClass.filePath  + "/" + dbName + ".db"))
           File.Delete(PublicClass.filePath  + "/" + dbName + ".db");
           }
           catch
           {}
        //closeDb();
        //if (File.Exists(PublicClass.filePath + "/" + dbName + ".db"))
        //    File.Delete(PublicClass.filePath + "/" + dbName + ".db");
        ////db = new LiteDatabase(PublicClass.filePath  + "/" + dbName + ".db");
        //this.dbName = dbName;
        openDb(dbName);
        openDb();
        string sql = "delete from Customer";
        ExecuteSQLCommand(sql);
        closeDb();
    }


    //插入
    public I insertOne<T, I>(T t,string id) where T : class where I : struct
    {
        openDb();
        try
        {
            //创建数据库同名表
            //LiteCollection<T> tmpColl = db.GetCollection<T>(t.GetType().Name);
            //插入对象   
            //tmpColl.Insert(t);
            string tableName = t.GetType().Name;
            Byte[] data = Object2Bytes(t);
            SqliteParameter sp = new SqliteParameter("@data",data);
            ExecuteSQLCommand("INSERT INTO "+tableName+ " VALUES('"+id+"',@data)",sp);
        }
        catch(Exception e)
        {
            Debug.Log(e.StackTrace);
            Debug.Log(e.Message);
            Debug.LogError(t.GetType().Name);
        }
                    
        I result = default(I);
        closeDb();
        return result;
    }
    public T GetWithId<T>(string formName) where T : class
    {
        openDb();
        //LiteCollection<T> tmpSignInfoColl = db.GetCollection<T>(formName);
        //T temp = tmpSignInfoColl.FindOne(Query.All("_id", Query.Descending));
        //string sql = "select * from "+formName+ "where id = CAST(max(CAST(ID AS DECIMAL)) as VARCHAR(255)) ";
        string sql = "select * from " + formName + " order by CAST(ID AS INTEGER) DESC limit 1";
        ExecuteSQLCommand(sql);
        byte[] data = null;
        //string strFileName = this.reader.GetString(1);
        if (this.reader.Read()) {

            int nDataLen = (int)this.reader.GetBytes(1, 0, null, 0, 0);
            data= new byte[nDataLen];
            this.reader.GetBytes(1, 0, data, 0, nDataLen);
        
        }
        closeDb();
        return (T)Bytes2Object(data);
    }
    //返回所有对象
    public List<T> selectAll<T>() where T : class
    {
        openDb();
        List<T> dataList=new List<T>();
        //LiteCollection<T> tmpSignInfoColl=null;
        //IEnumerable<string> collectNames=db.GetCollectionNames();  
        //foreach (string item in collectNames)
        //{     
        //    tmpSignInfoColl = db.GetCollection<T>(item);
        //    IEnumerable<T> tmp = tmpSignInfoColl.FindAll();
        //    Debug.Log("书签数量："+tmp.Count());
        //    foreach (T i in tmp)
        //    {
        //        dataList.Add(i);
        //    }
        //}          
        Type t = typeof(T);
        string tableName = t.Name;
        string sql = "select * from " + tableName;
        ExecuteSQLCommand(sql);

        while (this.reader.Read()) {
            //string strFileName = this.reader.GetString(1);
            int nDataLen = (int)this.reader.GetBytes(1, 0, null, 0, 0);
            byte[] data = new byte[nDataLen];
            this.reader.GetBytes(1, 0, data, 0, nDataLen);
            dataList.Add((T)Bytes2Object(data));
        }
        closeDb();
        return dataList;
    }



    public int GetDBLength<T>() where T : class
    {
        openDb();
        int length=0;
        //LiteCollection<T> tmpSignInfoColl=null;
        //IEnumerable<string> collectNames=db.GetCollectionNames();  
        //foreach (string item in collectNames)
        //{     
        //    tmpSignInfoColl = db.GetCollection<T>(item);
        //    length+= tmpSignInfoColl.Count();
        //}        
        Type t = typeof(T);
        string tableName = t.Name;
        string sql = "select count(1) from "+ tableName;
        ExecuteSQLCommand(sql);
        if (this.reader.Read()) {
            length= this.reader.GetInt32(0);
        }
        Debug.Log("<color=red>GetDBLength:" + length+"</color>");
        closeDb();
        return length;
    }
    public void UpdateWithName<T, I>(string tableName, T updateData, string id_value) where T : class where I : struct
    {
        openDb();
        //LiteCollection<T> tmpColl = db.GetCollection<T>(tableName);
        //tmpColl.Update(updateData);
        DeleteWithId<T>(tableName, id_value);
        insertOne<T,int>(updateData,id_value);
        closeDb();
        //byte[] data = Object2Bytes(updateData);

    }

    public void DeleteWithId<T>(string tableName, string id)
    {
        openDb();
        //LiteCollection<T> tmpColl = db.GetCollection<T>(tableName);
        //tmpColl.Delete(Query.Contains("_id", id));
        //openDb();
        ExecuteSQLCommand("DELETE FROM " + tableName + " WHERE ID = '" + id + "'");
        closeDb();
    }
    public void DeleteWithId<T>(string formName, int id) where T : class
    {
        openDb();
        //LiteCollection<T> tmpSignInfoColl=  db.GetCollection<T>(formName);
        //tmpSignInfoColl.Delete(Query.EQ("_id", id));
        ExecuteSQLCommand("DELETE FROM " + formName + " WHERE ID = '" + id + "'");
        closeDb();
    }
    public void DeletAll<T>(string formName) where T : class
    {
        openDb();
        ExecuteSQLCommand("DELETE FROM " + formName);
        closeDb();
        //LiteCollection<T> tmpSignInfoColl=  db.GetCollection<T>(formName);
        //IEnumerable<T> tmp=tmpSignInfoColl.FindAll();
        //tmpSignInfoColl.Delete(Query.All());
        //tmpSignInfoColl=  db.GetCollection<T>(formName);
        //Debug.Log("清空后长度： "+tmpSignInfoColl.Count());
    }
    public void closeDb()
    {

        if (this.command != null)
        {
            this.command.Cancel();
        }

        if (this.reader != null)
        {
            this.reader.Close();
        }

        if (this.connection != null)
        {
            this.connection.Close();
        }
        this.command = null;
        this.reader = null;
        this.connection = null;
        // Debug.Log("已经断开数据库连接");
    }

    public O updateOne<T, O>(T t)
        where T : class
        where O : class
    {
        throw new NotImplementedException();
    }

    public O deleteOne<T, O>(T t)
        where T : class
        where O : struct
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// 将对象转换为byte数组
    /// </summary>
    /// <param name="obj">被转换对象</param>
    /// <returns>转换后byte数组</returns>
    public static byte[] Object2Bytes(object obj)
    {
        byte[] buff;
        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter iFormatter = new BinaryFormatter();
            iFormatter.Serialize(ms, obj);
            buff = ms.GetBuffer();
        }
        return buff;
    }

    /// <summary>
    /// 将byte数组转换成对象
    /// </summary>
    /// <param name="buff">被转换byte数组</param>
    /// <returns>转换完成后的对象</returns>
    public static object Bytes2Object(byte[] buff)
    {
        object obj;
        using (MemoryStream ms = new MemoryStream(buff))
        {
            IFormatter iFormatter = new BinaryFormatter();
            obj = iFormatter.Deserialize(ms);
        }
        return obj;
    }

    
}