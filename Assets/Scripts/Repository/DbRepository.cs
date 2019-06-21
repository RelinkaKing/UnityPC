using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite4Unity3d;
using UnityEngine;

namespace Assets.Scripts.Repository
{
    public class DbRepository<T> : IRepository<T> where T : class, new()
    {
        //public delegate void DoConnection(String str);
        private SQLiteConnection _connection;
        //private string v;

        //public DbRepository(string v)
        //{
        //    this.v = v;
        //}
        public void CreateTable()
        {
            _connection.CreateTable<T>();

        }
        public TableQuery<T> getTableQuerry()
        {
            return _connection.Table<T>();
        }
        public void DropTable()
        {
            _connection.DropTable<T>();
        }

        public void Delete(T instance)
        {
            try
            {
                _connection.Delete(instance);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public void Insert(T instance)
        {
            try
            {
                _connection.Insert(instance);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public void InsertAll(T[] instance)
        {
            try
            {
                _connection.InsertAll(instance);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }


        public void InsertAll(List<T> instance)
        {
            try
            {
                _connection.InsertAll(instance);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public T SelectOne<R>(Func<T, bool> func) where R : class, new()
        {
            return _connection.Table<T>().Where(func).FirstOrDefault();
        }

        public IEnumerable<T> Select<R>(Func<T, bool> func) where R : class, new()
        {
            return _connection.Table<T>().Where(func);
        }
        public void Update(T t)
        {

            _connection.Update(t);
        }

        public void Close()
        {
            try
            {
                _connection.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        public void CreateDb(string dbPath)
        {
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }
        public void DataService(string DatabaseName)
        {
#if UNITY_EDITOR
            var dbPath = string.Format("{0}/{1}", PublicClass.vesal_db_path, DatabaseName); //string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        // var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        var filepath = string.Format("{0}/{1}",PublicClass.vesal_db_path, DatabaseName);

        //沙盒路径下不存在，copy 流文件部分
//        if (!File.Exists(filepath))
//        {
//            Debug.Log("Database not in Persistent path:"+filepath);
//            // if it doesn't ->
//            // open StreamingAssets directory and load the db ->

//#if UNITY_ANDROID 
//            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
//            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
//            // then save to Application.persistentDataPath
//            File.WriteAllBytes(filepath, loadDb.bytes);
//#elif UNITY_IOS
//                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//                // then save to Application.persistentDataPath
//                // File.Copy(loadDb, filepath);
//#elif UNITY_WP8
//                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//                // then save to Application.persistentDataPath
//                File.Copy(loadDb, filepath,true);

//#elif UNITY_WINRT
//        var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//        // then save to Application.persistentDataPath
//        File.Copy(loadDb, filepath,true);
		
//#elif UNITY_STANDALONE_OSX
//        var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//        // then save to Application.persistentDataPath
//        File.Copy(loadDb, filepath);
//#else
//    var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//    // then save to Application.persistentDataPath
//    File.Copy(loadDb, filepath);

//#endif

//            Debug.Log("Database written");
//        }

        var dbPath = filepath;
        // var dbPath = loadDb;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            //DebugLog.DebugLogInfo("Final PATH: " + dbPath);

        }

        public IEnumerable<T> Select(Func<T, bool> func)
        {
            throw new NotImplementedException();
        }
    }
}
