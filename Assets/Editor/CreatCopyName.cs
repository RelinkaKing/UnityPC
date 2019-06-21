using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using VesalCommon;

public class CreatCopyName : MonoBehaviour {


    static List<string> copylist = new List<string>();

    [MenuItem("Tools/AssetPress/OutputAssetsList")]
    public static void CreateAssetsList()
    {
        copylist.Clear();
        SearchName(Application.persistentDataPath);
        Write();
    }


    [MenuItem("Tools/AssetPress/CheckAssetsList")]
    public static void CheckList()
    {
        //待检测
        SearchName(Application.persistentDataPath);
        
        //读
        FileStream fs = new FileStream(Application.persistentDataPath + "/filelist.txt", FileMode.OpenOrCreate);
        StreamReader sr = new StreamReader(fs);
        string strLine = null;
        while ((strLine = sr.ReadLine()) != null)
        {
            string str = strLine.Replace(",", "").Replace("/", "\\");
            if (!FilterCopyList(str))
            {
                Debug.LogError(str+" not found");
            }
        }
        Debug.Log("check complete");
        //关闭流
        sr.Close();
        fs.Close();
    }


    static bool FilterCopyList(string strLine)
    {
        for (int i = 0; i < copylist.Count; i++)
        {
            if (copylist[i].Contains(strLine))
            {
                return true;
            }
        }
        return false;
    }

    static void Write()
    {
        if (File.Exists(Application.persistentDataPath + "/filelist.txt"))
        {
            File.Delete(Application.persistentDataPath + "/filelist.txt");
        }
        FileStream fs = new FileStream(Application.persistentDataPath + "/filelist.txt", FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        //开始写入
        for (int i = 0; i < copylist.Count; i++)
        {
            if (filter(copylist[i]))
            {
                sw.Write(copylist[i].Replace(@"C:/Users/Administrator/AppData/LocalLow/vesal/ruanyikeji_vesal_vesal/", "").Replace("\\","/")+",\r\n");
            }
        }

        ////清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
        fs.Close();
        Debug.Log("写入成功");
    }

    static void SearchName(string root)
    {
        DirectoryInfo di = new DirectoryInfo(root);
        FileSystemInfo[] fis = di.GetFileSystemInfos();
        
        for (int i = 0; i < fis.Length; i++)
        {
            //文件是一个子目录
            if (fis[i] is DirectoryInfo)
            {
                SearchName(fis[i].FullName);
            }
            else
            {
                copylist.Add(fis[i].FullName);
            }
        }
    }

    static bool filter(string str)
    {
        for (int i = 0; i < ignore.Length; i++)
        {
            if (str.Contains(ignore[i]))
                return false;
        }
        return true;
    }

    static string[] ignore = { "Unity\\","temp\\", "filelist.txt", "version.dat" };
}