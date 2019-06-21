using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Assets.Scripts.DbDao;
using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using UnityEngine;
using VesalCommon;

public class MakeStaticDBTable<T> : MonoBehaviour where T : class, new() {

    public static void Update_db_table(string dbfile) 
    {
        DbRepository<T> temp_db;
        temp_db = new DbRepository<T>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<T> res = temp_db.Select<T>((temp_fbx) =>
        {
            if (temp_fbx != null)
                return true;
            else
                return false;
        });

        if (res.GetEnumerator().MoveNext())
        {
            DbRepository<T> anim_db;
            anim_db = new DbRepository<T>();
            anim_db.DataService("vesali.db");
            foreach (T item in res)
            {
                T fbx_info = anim_db.SelectOne<T>((temp_fbx_anim) =>
                {
                    // if (temp_fbx_anim.ab_name == item.ab_name)
                    // {
                    //     return true;
                    // }
                    // else
                    // {
                        return false;
                    // }
                });


                if (fbx_info != null)
                {
                    anim_db.Update(item);
                }
                else
                {
                    anim_db.Insert(item);
                }
            }
            anim_db.Close();
        }
        temp_db.Close();
    }


    //fetch json 到本地
    public static void sign_UpdateTable(string api,string db_path,bool is_append)
    {
        var cache = new CacheOption<T>();
        print("v1/app/xml ："+api);
        print(db_path);
        // cache.setCache_dbPath(PublicClass.server_ip + "v1/app/xml/getTempSignNew",db_path);
        cache.setCache_dbPath(api,db_path,is_append);
        print("getTempSignNew success");
    }

    //贴图记录工具
    public static void SetTextureDataToDB(string texture_path,string db_fullname)
    {
        print("<Color=#00ff00> db路径 ："+db_fullname+" </color>");
        List<string> list = new List<string>();
        list.AddRange(Vesal_DirFiles.GetAllDirInfoFromPath(texture_path));
        print("<Color=#00ff00> 贴图列表长度 ："+list.Count+"</color>");
        DbRepository<texture_mask> temp_db=new DbRepository<texture_mask>();
        // temp_db.DataService("StartStop.db");
        temp_db.CreateDb(db_fullname);      
        temp_db.CreateTable();
        //db 原数据
        List<string> mask_name_list=new List<string>();
        var tmpssp = temp_db.Select<texture_mask>((ss_id) =>
        {
            if (ss_id != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        });
        for (int i = 0; i < list.Count; i++)
        {
            texture_mask temp_mask = new texture_mask
            {
                id =i+1,
                name=Vesal_DirFiles.get_file_name_from_full_path(list[i]),
                tex_data =File.ReadAllBytes(list[i]),// Vesal_DirFiles.Object2Bytes(new SceneModelState()),
            };
            bool has_record=false;
            foreach (texture_mask tmp in tmpssp)
            {
                if(temp_mask.name==tmp.name)
                {
                    temp_db.Update(temp_mask);
                    has_record=true;
                    print("update : "+temp_mask.name);
                }
            }
            if(has_record==false)
                temp_db.Insert(temp_mask);
        }

        Debug.Log("写入mask texture 数据成功");
        temp_db.Close();
    }


    public static void opera_condition(string db_path,Action<List<T>> ac)
    {
        DbRepository<T> temp_db=new DbRepository<T>();
        temp_db.CreateDb(db_path);      
        temp_db.CreateTable();
        //db 原数据
        List<T> mask_name_list=new List<T>();
        var tmpssp = temp_db.Select<T>((ss_id) =>
        {
            if (ss_id != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        });
        foreach (var item in tmpssp)
        {
            mask_name_list.Add(item);
        }
        Debug.Log("写入mask texture 数据成功");
        temp_db.Close();
        ac(mask_name_list);
    }


    public static void update_condition(List<T> struct_lsit,string db_path)
    {
        DbRepository<T> temp_db=new DbRepository<T>();
        temp_db.CreateDb(db_path);      
        foreach (var item in struct_lsit)
        {
            temp_db.Update(item);
        }
        Debug.Log("写入mask texture 数据成功");
        temp_db.Close();
    }
}