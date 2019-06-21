using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Repository;
using Assets.Scripts.Model;
using Assets.Scripts.Infrastructure;
using UnityEngine;
using Newtonsoft.Json;

namespace Assets.Scripts.DbDao
{
    public class CacheOption<T> : IDbDao<T> where T : class, new()
    {
        public void setCache_dbPath(string r, string path, bool is_append = false)
        {
            var repository = new RemoteRepository<GetStruct>();
            repository.Get<Response<T>>(r, (response) =>
            {
                Debug.Log(JsonConvert.SerializeObject(response));
                if (response.List != null && response.List.Count != 0)
                {
                    Debug.Log("============读取数据库缓存更新，从远程拉取数据，更新版本号=================== ");
                    var db = new DbRepository<T>();
                    db.CreateDb(path);
                    if (!is_append)
                    {
                        db.DropTable();
                    }
                    db.CreateTable();
                    db.InsertAll(response.List);//更新远程数据源
                    db.Close();
                }
                else
                {
                    Debug.Log("Struct List data is null ");
                }
            });
        }
        //实现写入缓存共有类
        public void setCache(string r)
        {
            var repository = new RemoteRepository<GetStruct>();

            var version = new DbRepository<TableVersions>();
            version.DataService("vesali.db");
            version.CreateTable();
            TableVersions tv = version.SelectOne<TableVersions>((tmpT) =>
            {
                if (tmpT.table_name == typeof(T).Name)
                {
                    return true;
                }
                return false;

            });
            var struct_version = "-1";
            if (tv != null)
            {
                struct_version = tv.version;
            }
            //var os = "ios";
            //if (PublicClass.platform == asset_platform.android)
            //{
            //    os = "android";
            //}
            //Debug.Log("===当前请求的地址是====" + r + "===当前泛型指定的实例为====" + typeof(T).Name + "  ====当前版本===" + struct_version);
            //vesal_log.vesal_write_log(SystemInfo.deviceUniqueIdentifier+"--------------------------------------------------------设备唯一号");
            //  Debug.Log(SystemInfo.deviceUniqueIdentifier+"--------------------------------------------------------设备唯一号");
            // Debug.Log("======== "+ (int)PublicClass.Quality);//, level = (int)PublicClass.Quality      ?Version=2&softVersion=2.3&os=android&level=1
            Debug.Log(r);
            repository.Get<Response<T>>(r, new GetStruct { Version = struct_version, device = SystemInfo.deviceUniqueIdentifier, os = Enum.GetName(typeof(asset_platform), PublicClass.platform) , level = ((int)PublicClass.Quality).ToString(), softVersion = PublicClass.get_version() }, (response) =>
                       {
                           if (response.List != null && response.List.Count != 0)
                           {
                               Debug.Log("============读取数据库缓存更新，从远程拉取数据，更新版本号=================== "+ typeof(T).Name);
                               //如果有数据,更新数据和版本
                               if (tv == null)
                               {
                                   version.Insert(new TableVersions { table_name = typeof(T).Name, version = response.maxVersion });
                               }
                               else
                               {
                                   version.Update(new TableVersions { table_name = typeof(T).Name, version = response.maxVersion });
                               }
                               version.Close();
                               var db = new DbRepository<T>();
                               db.DataService("vesali.db");
                               db.DropTable();
                               db.CreateTable();
                               db.InsertAll(response.List);//更新远程数据源
                               db.Close();
                           }
                           else
                           {
                               Debug.Log("Struct List data is null "+ struct_version + " " +response.maxVersion);
                               // Debug.Log("============读取数据库缓存=================== ");
                               //读取数据库缓存
                           }
                           PublicClass.data_list_count++;
                       });
            //throw new NotImplementedException();
        }

        public void setCacheJson(string r)
        {
            var repository = new RemoteRepository<GetStruct>();
            Debug.Log(r+"-------------------------------------------------------");
            repository.Get<Response<T>>(r, new GetStruct { device = SystemInfo.deviceUniqueIdentifier,level = ((int)PublicClass.Quality).ToString(), softVersion = PublicClass.get_version() }, (response) =>
            {
                if (response.List != null && response.List.Count != 0)
                {
                    Debug.Log("============读取数据库缓存更新，从远程拉取数据，更新版本号=================== ");
                    var db = new DbRepository<T>();
                    db.DataService("vesali.db");
                    db.DropTable();
                    db.CreateTable();
                    db.InsertAll(response.List);//更新远程数据源
                    db.Close();
                }
                else
                {
                    Debug.Log("-----------------Struct List data is null ");
                    // Debug.Log("============读取数据库缓存=================== ");
                }
            });
        }
    }
}
