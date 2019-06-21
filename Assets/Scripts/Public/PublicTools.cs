using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Model;
using Assets.Scripts.Repository;

public class PublicTools : MonoBehaviour {

    public static string read_version_from_file(string fname_path)
    {
        try
        {
            string temp;
            if (!File.Exists(fname_path))
                return null;
            FileStream fs = new FileStream(fname_path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            temp = bf.Deserialize(fs) as string;
            fs.Close();
            return temp;
        }
        catch
        {
            return null;
        }
    }

    public static bool save_version_to_file(string fname_path, string dd_list)
    {
        if (File.Exists(fname_path))
        {
            File.Delete(fname_path);
        }
        try
        {
            FileStream fs = new FileStream(fname_path, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, dd_list);
            fs.Flush();
            fs.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            return false;
        }
    }


    public static bool version_is_larger(string v1, string v2)
    {
        string[] digital1 = v1.Split('.');
        string[] digital2 = v2.Split('.');
        try
        {
            if (int.Parse(digital1[0]) > int.Parse(digital2[0]))
                return true;
            if (int.Parse(digital1[0]) < int.Parse(digital2[0]))
                return false;
            if (int.Parse(digital1[1]) > int.Parse(digital2[1]))
                return true;
            if (int.Parse(digital1[1]) < int.Parse(digital2[1]))
                return false;
            if (int.Parse(digital1[2]) > int.Parse(digital2[2]))
                return true;
            if (int.Parse(digital1[2]) < int.Parse(digital2[2]))
                return false;

        }
        catch
        {
        }
        return false;
    }

    public static string getId() {
        return SystemInfo.deviceUniqueIdentifier;
    }
   
    public static  Vector3 vector2Vecotor(vector3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static Vector3 Str2Vector3(string data)
    {
        if (data == string.Empty || data == null)
        {
            return Vector3.zero;
        }
        data = data.Replace("(", "");
        data = data.Replace(")", "");
        string[] datas = data.Split(',');

        Vector3 result = Vector3.zero;
        try
        {
            result = new Vector3(float.Parse(datas[0]), float.Parse(datas[1]), float.Parse(datas[2]));
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }
        return result;
    }
    public static Vector2 Str2Vector2(string data)
    {
        if (data == string.Empty || data == null)
        {
            return Vector3.zero;
        }
        data = data.Replace("(", "");
        data = data.Replace(")", "");
        string[] datas = data.Split(',');

        Vector2 result = Vector2.zero;
        try
        {
            result = new Vector2(float.Parse(datas[0]), float.Parse(datas[1]));
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }
        return result;
    }
    public static string Vector32Str(Vector3 tmp, String format = "f6")
    {
        return "(" + tmp.x.ToString(format) + "," + tmp.y.ToString(format) + "," + tmp.z.ToString(format) + ")";
    }

    public static string Vector22Str(Vector2 tmp, String format = "f6")
    {
        return "(" + tmp.x.ToString(format) + "," + tmp.y.ToString(format) + ")";
    }
    public static string Vector22Str(Vector3 tmp, String format = "f6")
    {
        return "(" + tmp.x.ToString(format) + "," + tmp.y.ToString(format) + ")";
    }
   

    public static bool SaveObject<T>(string path, T t) where T : class
    {
        XmlSerializer xmldes = new XmlSerializer(typeof(T));

        using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                xmldes.Serialize(sw, t, ns);
            }
        }

        return false;
    }

    public static T Deserialize<T>(string xmlpath) where T : class
    {
        //去Bom
        Encoding utf8NoBom = new UTF8Encoding(false);
        using (StringReader sr = new StringReader(File.ReadAllText(xmlpath, utf8NoBom)))
        {
            XmlSerializer xmldes = new XmlSerializer(typeof(T));
            return (T)xmldes.Deserialize(sr);
        }
    }
    public static string getTime(string format = "yyyy_MM_dd_HH_mm_ss")
    {
        string time = Convert.ToDateTime(System.DateTime.Now).ToString(format);//24小时
        return time;
    }

    /// <summary>
    /// 获取当前时间戳
    /// </summary>
    /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>
    /// <returns></returns>
    public static long GetTimeStamp(bool bflag = true)
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret;
        if (bflag)
            ret = Convert.ToInt64(ts.TotalSeconds);
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    }

    /// <summary>
    /// 深拷贝一个对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T DeepCopy<T>(T obj)
    {
        object retval;
        using (MemoryStream ms = new MemoryStream())
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            xml.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            retval = xml.Deserialize(ms);
            ms.Close();
        }
        return (T)retval;
    }
    public static bool isTargetInSourceList(string name, string sourceList, char split = ',', string suffix = "")
    {
        string[] lowAbs = sourceList.Split(split);
        string target = name + suffix;
        for (int i = 0; i < lowAbs.Length; ++i)
        {
            if (lowAbs[i].Trim() == target)
            {
                return true;
            }
            if (PublicClass.Quality == Run_Quality.POOL) {
                if ("s_"+lowAbs[i].Trim() == target)
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 遍历删除所有目标类型App下子目录中解压的文件夹
    /// </summary>
    /// <param name="srcPath"></param>
    public void DelectAppDir(string srcPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    if (srcPath == i.FullName)
                    {
                        continue;
                    }
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    FileSystemInfo[] subFileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                    foreach (FileSystemInfo j in subFileinfo)
                    {
                        //Debug.Log(j.FullName);
                        if (j.FullName == i.FullName)
                        {
                            continue;
                        }
                        if (j is DirectoryInfo)            //判断是否文件夹
                        {
                            DirectoryInfo subSubDir = new DirectoryInfo(j.FullName);
                            subSubDir.Delete(true);          //删除子目录和文件
                        }
                    }

                }
                else
                {
                    //File.Delete(i.FullName);      //删除指定文件
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }


    /// <summary>
    /// 重新拟合模型几何中心
    /// </summary>
    /// <param name="parent"> 需要重定义几何中心的模型</param>
    /// <param name="isChangeScale">是否改变模型缩放比（Vector3.one）</param>
    /// <param name="isChangePostaion">是否改变模型位置（Vector3.zero）</param>
    /// <param name="isChangeRotation">是否改变模型旋转角（Quaternion.Euler(Vector3.zero)）</param>
    /// <param name="isOverTurn">模型是否绕Y旋转180度</param>
    public static void fitModel(Transform parent, bool isChangeScale = true, bool isChangePostaion = true, bool isChangeRotation = true, bool isOverTurn = false)
    {
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        //记录初始模型信息
        Vector3 postion = parent.position;
        Quaternion rotation = parent.rotation;
        Vector3 scale = parent.localScale;
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        parent.localScale = Vector3.one;
        Vector3 center = Vector3.zero;

        for (int i = 0; i < renders.Length; i++)
        {
            center += renders[i].bounds.center;
        }
        //拟合几何中心
        center /= parent.GetComponentsInChildren<Renderer>().Length;

        Bounds bounds = new Bounds(center, Vector3.zero);

        for (int i = 0; i < renders.Length; i++)
        {
            bounds.Encapsulate(renders[i].bounds);
        }

        parent.localScale = scale;

        //相对当前位置与目标几何中心位移
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).position -= center;
        }
        
        if (isChangePostaion)
        {
            parent.position = Vector3.zero;
        }
        else {
            parent.position = postion;
        }
        if (isChangeRotation)
        {
            parent.rotation = rotation;
        }
        else {
            parent.rotation = Quaternion.Euler(Vector3.zero);
        }
        if (isChangeScale) {
            parent.localScale = Vector3.one;
        }
        else {
            parent.localScale = scale;
        }

        if (isOverTurn) {
            //绕Y180转正，可有可无
            parent.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        //if (targetBounds != null) {
        //    Bounds centerBounds = targetBounds.transform.GetComponent<Renderer>().bounds;

        //    //物体距离中心点最远比 估算 缩放比
        //    float tmp = Vector3.Distance(centerBounds.max, centerBounds.center) * 0.7f / Vector3.Distance(bounds.max, bounds.center);
        //    parent.localScale = Vector3.one * tmp;
        //    parent.position = targetBounds.transform.position;
        //}
        
    }
    public class Base64Helper
    {
        public static string Base64Encode(string source)
        {
            return Base64Encode(Encoding.UTF8, source);
        }

        public static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ 0x55);
            }

            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }

        public static string Base64Decode(string result)
        {
            return Base64Decode(Encoding.UTF8, result);
        }

        public static string Base64Decode(Encoding encodeType, string result)
        {
            string decode = string.Empty;
            byte[] bytes = Convert.FromBase64String(result);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ 0x55);
            }

            try
            {
                decode = encodeType.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
    }

    internal static string Vector32Str(vector3 vector3)
    {
        throw new NotImplementedException();
    }

    public static List<string> get_table_list(string dbfile)
    {
        List<string> tablelist = new List<string>();
        DbRepository<FixTabs> temp_db;
        temp_db = new DbRepository<FixTabs>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<FixTabs> res = temp_db.Select<FixTabs>((temp_fbx) =>
        {
            if (temp_fbx.tab_name != "")
                return true;
            else
                return false;
        });
        if (res.GetEnumerator().MoveNext())
        {
            foreach (FixTabs item in res)
            {
                if (item.tab_name != "")
                    tablelist.Add(item.tab_name);
            }
        }
        temp_db.Close();
        return tablelist;
    }

    public static void update_GetSubModel_db(string dbfile)
    {
        DbRepository<GetSubModel> temp_db;
        temp_db = new DbRepository<GetSubModel>();
            temp_db.DataService(dbfile);
                    temp_db.CreateTable();
            IEnumerable<GetSubModel> res = temp_db.Select<GetSubModel>((temp_fbx) =>
            {
                if (temp_fbx.name != "")
                    return true;
                else
                    return false;
            });
            if (res.GetEnumerator().MoveNext())
            {
                DbRepository<GetSubModel> anim_db;
                anim_db = new DbRepository<GetSubModel>();
                anim_db.DataService("vesali.db");
                foreach (GetSubModel item in res)
                {
                    GetSubModel fbx_info = anim_db.SelectOne<GetSubModel>((temp_fbx_anim) =>
                    {
                        if (temp_fbx_anim.name == item.name)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
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


    public static void update_GetStructList_db(string dbfile)
    {
        DbRepository<GetStructList> temp_db;

        temp_db = new DbRepository<GetStructList>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<GetStructList> res = temp_db.Select<GetStructList>((temp_fbx) =>
        {
            if (temp_fbx.nounNo != "")
                return true;
            else
                return false;
        });
        if (res.GetEnumerator().MoveNext())
        {
            DbRepository<GetStructList> anim_db;
            anim_db = new DbRepository<GetStructList>();
            anim_db.DataService("vesali.db");
            foreach (GetStructList item in res)
            {
                GetStructList fbx_info = anim_db.SelectOne<GetStructList>((temp_fbx_anim) =>
                {
                    if (temp_fbx_anim.nounNo == item.nounNo)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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


    public static void update_LayserSubModel_db(string dbfile)
    {
        DbRepository<LayserSubModel> temp_db;

        temp_db = new DbRepository<LayserSubModel>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<LayserSubModel> res = temp_db.Select<LayserSubModel>((temp_fbx) =>
        {
            if (temp_fbx.id != "")
                return true;
            else
                return false;
        });

        if (res.GetEnumerator().MoveNext())
        {

            DbRepository<LayserSubModel> anim_db;
            anim_db = new DbRepository<LayserSubModel>();
            anim_db.DataService("vesali.db");
            foreach (LayserSubModel item in res)
            {
                LayserSubModel fbx_info = anim_db.SelectOne<LayserSubModel>((temp_fbx_anim) =>
                {
                    if (temp_fbx_anim.id == item.id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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


    public static void update_ModelRelationModel_db(string dbfile)
    {
        DbRepository<ModelRelationModel> temp_db;

        temp_db = new DbRepository<ModelRelationModel>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<ModelRelationModel> res = temp_db.Select<ModelRelationModel>((temp_fbx) =>
        {
            if (temp_fbx.id >= 0)
                return true;
            else
                return false;
        });


        if (res.GetEnumerator().MoveNext())
        {
            DbRepository<ModelRelationModel> anim_db;
            anim_db = new DbRepository<ModelRelationModel>();
            anim_db.DataService("vesali.db");
            foreach (ModelRelationModel item in res)
            {
                ModelRelationModel fbx_info = anim_db.SelectOne<ModelRelationModel>((temp_fbx_anim) =>
                {
                    if (temp_fbx_anim.id == item.id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

    public static void update_RightMenuLayerModel_db(string dbfile)
    {
        DbRepository<RightMenuLayerModel> temp_db;

        temp_db = new DbRepository<RightMenuLayerModel>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<RightMenuLayerModel> res = temp_db.Select<RightMenuLayerModel>((temp_fbx) =>
        {
            if (temp_fbx.layer_id !="")
                return true;
            else
                return false;
        });


        if (res.GetEnumerator().MoveNext())
        {
            DbRepository<RightMenuLayerModel> anim_db;
            anim_db = new DbRepository<RightMenuLayerModel>();
            anim_db.DataService("vesali.db");
            foreach (RightMenuLayerModel item in res)
            {
                RightMenuLayerModel fbx_info = anim_db.SelectOne<RightMenuLayerModel>((temp_fbx_anim) =>
                {
                    if (temp_fbx_anim.layer_id == item.layer_id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
    public static void update_RightMenuModel_db(string dbfile)
    {
        DbRepository<RightMenuModel> temp_db;

        temp_db = new DbRepository<RightMenuModel>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<RightMenuModel> res = temp_db.Select<RightMenuModel>((temp_fbx) =>
        {
            if (temp_fbx.rm_id >= 0)
                return true;
            else
                return false;
        });


        if (res.GetEnumerator().MoveNext())
        {
            DbRepository<RightMenuModel> anim_db;
            anim_db = new DbRepository<RightMenuModel>();
            anim_db.DataService("vesali.db");
            foreach (RightMenuModel item in res)
            {
                RightMenuModel fbx_info = anim_db.SelectOne<RightMenuModel>((temp_fbx_anim) =>
                {
                    if (temp_fbx_anim.rm_id == item.rm_id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

    public static void update_SignNewInfo_db(string dbfile)
    {
        DbRepository<SignNewInfo> temp_db;

        temp_db = new DbRepository<SignNewInfo>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<SignNewInfo> res = temp_db.Select<SignNewInfo>((temp_fbx) =>
        {
            if (temp_fbx.id != "")
                return true;
            else
                return false;
        });


        if (res.GetEnumerator().MoveNext())
        {
            DbRepository<SignNewInfo> anim_db;
            anim_db = new DbRepository<SignNewInfo>();
            anim_db.DataService("vesali.db");
            foreach (SignNewInfo item in res)
            {
                SignNewInfo fbx_info = anim_db.SelectOne<SignNewInfo>((temp_fbx_anim) =>
                {
                    if (temp_fbx_anim.id == item.id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

    public static void update_GetStructAbList_db(string dbfile)
    {
        DbRepository<GetStructAbList> temp_db;

        temp_db = new DbRepository<GetStructAbList>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<GetStructAbList> res = temp_db.Select<GetStructAbList>((temp_fbx) =>
        {
            if (temp_fbx.id != "")
                return true;
            else
                return false;
        });


        if (res.GetEnumerator().MoveNext())
        {
            DbRepository<GetStructAbList> anim_db;
            anim_db = new DbRepository<GetStructAbList>();
            anim_db.DataService("vesali.db");
            foreach (GetStructAbList item in res)
            {
                GetStructAbList fbx_info = anim_db.SelectOne<GetStructAbList>((temp_fbx_anim) =>
                {
                    if (temp_fbx_anim.id == item.id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

    public static void update_GetTextureModelList_db(string dbfile)
    {
        DbRepository<GetTextureModelList> temp_db;

        temp_db = new DbRepository<GetTextureModelList>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<GetTextureModelList> res = temp_db.Select<GetTextureModelList>((temp_fbx) =>
        {
            if (temp_fbx.tex_name!= "")
                return true;
            else
                return false;
        });


        if (res.GetEnumerator().MoveNext())
        {
            DbRepository<GetTextureModelList> anim_db;
            anim_db = new DbRepository<GetTextureModelList>();
            anim_db.DataService("vesali.db");
            foreach (GetTextureModelList item in res)
            {
                GetTextureModelList fbx_info = anim_db.SelectOne<GetTextureModelList>((temp_fbx_anim) =>
                {
                    if (temp_fbx_anim.tex_name == item.tex_name)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

    public static void update_noun_no_info_db(string dbfile)
    {
        DbRepository<noun_no_info> temp_db;

        temp_db = new DbRepository<noun_no_info>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<noun_no_info> res = temp_db.Select<noun_no_info>((temp_fbx) =>
        {
            if (temp_fbx.sn_id != "")
                return true;
            else
                return false;
        });


        if (res.GetEnumerator().MoveNext())
        {
            DbRepository<noun_no_info> anim_db;
            anim_db = new DbRepository<noun_no_info>();
            anim_db.DataService("vesali.db");
            foreach (noun_no_info item in res)
            {
                noun_no_info fbx_info = anim_db.SelectOne<noun_no_info>((temp_fbx_anim) =>
                {
                    if (temp_fbx_anim.sn_id == item.sn_id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

    public static void update_ab_info_db(string dbfile)
    {
        DbRepository<AbInfo> temp_db;

        temp_db = new DbRepository<AbInfo>();
        temp_db.DataService(dbfile);
        temp_db.CreateTable();
        IEnumerable<AbInfo> res = temp_db.Select<AbInfo>((temp_fbx) =>
        {
            if (temp_fbx.ab_name != "")
                return true;
            else
                return false;
        });


        if (res.GetEnumerator().MoveNext())
        {
            DbRepository<AbInfo> anim_db;
            anim_db = new DbRepository<AbInfo>();
            anim_db.DataService("vesali.db");
            foreach (AbInfo item in res)
            {
                AbInfo fbx_info = anim_db.SelectOne<AbInfo>((temp_fbx_anim) =>
                {
                    if (temp_fbx_anim.ab_name == item.ab_name)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

    public static void Model_AB_dic_update()
    {
        PublicClass.Model_AB_dic.Clear();
        var local_db = new DbRepository<AbInfo>();
        local_db.DataService("vesali.db");
        var dbs = local_db.Select<AbInfo>((tempt) =>
        {
            return true;
        });

        foreach (var i in dbs)
        {
            string[] submodellist = i.submodel_list.Split(',');
            foreach (string model in submodellist)
            {
                if (!PublicClass.Model_AB_dic.ContainsKey(model))
                    PublicClass.Model_AB_dic.Add(model, i.ab_name);
            }
        }
        local_db.Close();
    }



}
