using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Scripts.Model;
using VesalCommon;
using System.IO;
using System;

public class UpdateAbByte : MonoBehaviour {

    [MenuItem("DB/AB_Info/SplitByte")]
    public static void SplitByte()
    {
        PublicClass.InitStaticData();
        string db_path = PublicClass.filePath + "Mediacl/RA0901001_1/";
        List<string> namelist = new List<string>();
        namelist.AddRange(Vesal_DirFiles.GetAllDirInfoFromPath(db_path));
        Debug.Log(db_path);
        for (int i = 0; i < namelist.Count; i++)
        {
            Debug.Log(namelist[i]);
           Vesal_DirFiles.SaveAbfile(namelist[i], PublicClass.filePath+Vesal_DirFiles.get_file_name_from_full_path(namelist[i]), false,false);
        }
    }
    static List<string> db_list;
    static int count;
    static int index;

    [MenuItem("DB/AB_Info/combineDb")]
    public static void update_app_fix()
    {
        index = 0;
        PublicClass.InitStaticData();
        db_list = new List<string>();
        db_list.AddRange(Vesal_DirFiles.ReadFileWithLine(PublicClass.filePath + "Mediacl/222.txt"));
        count = db_list.Count;
        update_db_process(db_list[index], count_load);
    }

    static void count_load()
    {
        Debug.Log(index+ " index");
        index++;
        if (count > index)
        {
            update_db_process(db_list[index], count_load);
        }
    }

    static void update_db_process(string fixdb,Action count_load)
    {
        string tmpdb = PublicClass.vesal_db_path + "temp.db";
        Vesal_DirFiles.DelFile(tmpdb);
        if (File.Exists(fixdb))
        {
            File.Copy(fixdb, tmpdb);
        }
        string[] fname = Directory.GetFiles(PublicClass.MedicalPath);
        
        //打开数据库进行更新
        List<string> fix_tab_list = new List<string>();
        if (File.Exists(tmpdb))
        {
            fix_tab_list = PublicTools.get_table_list("temp.db");
            if (fix_tab_list.Count != 0)
            {
                int tab_count = 0;
                foreach (string tab_name in fix_tab_list)
                {
                    switch (tab_name)
                    {
                        case "GetSubModel":
                            PublicTools.update_GetSubModel_db("temp.db");
                            break;
                        case "GetStructList":
                            PublicTools.update_GetStructList_db("temp.db");
                            break;
                        case "GetStructAbList":
                            PublicTools.update_GetStructAbList_db("temp.db");
                            break;
                        case "LayserSubModel":
                            PublicTools.update_LayserSubModel_db("temp.db");
                            break;
                        case "ModelRelationModel":
                            PublicTools.update_ModelRelationModel_db("temp.db");
                            break;
                        case "RightMenuLayerModel":
                            PublicTools.update_RightMenuLayerModel_db("temp.db");
                            break;
                        case "RightMenuModel":
                            PublicTools.update_RightMenuModel_db("temp.db");
                            break;
                        case "SignNewInfo":
                            PublicTools.update_SignNewInfo_db("temp.db");
                            break;
                        case "GetTextureModelList":
                            PublicTools.update_GetTextureModelList_db("temp.db");
                            break;
                        case "noun_no_info":
                            PublicTools.update_noun_no_info_db("temp.db");
                            break;
                        case "AbInfo":
                            PublicTools.update_ab_info_db("temp.db");
                            break;
                    }
                    tab_count++;
                    
                }
            }
            else
            {
                PublicTools.update_GetSubModel_db("temp.db");
                PublicTools.update_GetStructList_db("temp.db");
                PublicTools.update_GetStructAbList_db("temp.db");
                PublicTools.update_LayserSubModel_db("temp.db");
                PublicTools.update_ModelRelationModel_db("temp.db");
                PublicTools.update_RightMenuLayerModel_db("temp.db");
                PublicTools.update_RightMenuModel_db("temp.db");
                PublicTools.update_SignNewInfo_db("temp.db");
                PublicTools.update_GetTextureModelList_db("temp.db");
                PublicTools.update_noun_no_info_db("temp.db");
                PublicTools.update_ab_info_db("temp.db");
                
            }
            Vesal_DirFiles.DelFile(tmpdb);
        }

        if (count_load != null)
        {
            count_load();
        }
    }
}
