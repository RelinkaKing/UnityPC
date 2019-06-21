using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Network;
using Newtonsoft.Json;
using UnityEngine;
public class App_tool
{
    public static IEnumerator testMethod(string dict_name, string fyId, string app_type, string SA_id, Action<App> ac = null)
    {
        var httpRequest = new HttpRequest
        {
            Url = "http://api.vesal.cn:8000/vesal-jiepao-prod/v1/app/struct/initMyStruct?token=1&plat=pc&fyId=" + fyId + "&version=1&appVersion=" + PublicClass.get_version(),
            //Url = "http://118.24.119.234:8083/vesal-jiepao-test/v1/app/struct/initMyStruct?token=1&plat=android&fyId=" + fyId + "&version=1&appVersion=2.4.0",
            Method = HttpMethod.Get
        };
        Debug.Log(httpRequest.Url);
        Debug.Log("httpResponse.IsSuccess" + httpRequest.Parameters);
        HttpClient.Instance.SendAsync(httpRequest, httpResponse =>
        {
            Debug.Log("httpResponse.IsSuccess" + httpResponse.IsSuccess);
            if (httpResponse.IsSuccess)
            {
                Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(httpResponse.Data);
                foreach (string tmpKey in r.Keys)
                {
                    UnityEngine.Debug.Log(tmpKey);
                    UnityEngine.Debug.Log(r[tmpKey].ToString());
                }
                if (r.ContainsKey("List"))
                {

                    List<Dictionary<string, object>> tmpData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(r["List"].ToString());
                    foreach (Dictionary<string, object> tmpDic in tmpData)
                    {
                        foreach (string tmpKey in tmpDic.Keys)
                        {
                            /////李哲微课第三讲—骨骼的类型
                            //if (tmpKey == "dict_name" && tmpDic[tmpKey].ToString() == dict_name)
                            //{
                            UnityEngine.Debug.Log(tmpKey);
                            UnityEngine.Debug.Log(tmpDic[tmpKey].ToString());
                            List<App> tmpAppData = JsonConvert.DeserializeObject<List<App>>(tmpDic["StructList"].ToString());
                            foreach (App tmpApp in tmpAppData)
                            {
                                // if (tmpApp.app_id == "SA0104001")SA0101051 SA0101035 SA0101040
                                //if (tmpApp.app_id == "SA0000000")
                                // if (tmpApp.app_type == "sign")
                                if (tmpApp.app_type == app_type && (tmpApp.app_id.ToLower() == SA_id.ToLower() || SA_id == ""))
                                {
                                    UnityEngine.Debug.Log(tmpApp.struct_name + " " + tmpApp.app_type);
                                    UnityEngine.Debug.Log(tmpApp.struct_name);
                                    UnityEngine.Debug.Log(tmpApp.ab_list);
                                    UnityEngine.Debug.Log(tmpApp.ab_path);
                                    UnityEngine.Debug.Log(tmpApp.app_id);
                                    UnityEngine.Debug.Log(SA_id);
                                    //tmpApp.ab_list = "";
                                    ac(tmpApp);

                                    return;
                                }
                            }
                        }
                        //}
                    }

                }
                //onSuccess(r);
            }
            //TODO:异常处理
        });
        yield return null;
        //Load_Scene("WeiKePlayer");
    }

    public static string testMethod_signnew(string app_type, string SA_id, Action<App> ac = null)
    {
        string m_json = "{\"youke_use\":\"disabled\",\"platform\":\"android,ios,pc,ppt\",\"struct_state\":null,\"fy_id\":\"5\",\"fy_name\":null,\"Introduce\":\"\",\"struct_id\":\"397\",\"struct_sort\":\"39\",\"function_type\":\"5\",\"struct_sell_amount\":\"0\",\"visibel_identity\":null,\"struct_name\":\"手指屈曲\",\"struct_code\":\"SA0C05001\",\"ab_path\":\"\",\"xml_path\":null,\"model_scope\":null,\"app_type\":\"" +app_type+
            "\",\"app_id\":\"" +SA_id+
            "\",\"app_version\":\"1\",\"update_flag\":null,\"ab_list\":\"\",\"isTablet\":null,\"JumpState\":null,\"signModelName\":null,\"tissueModelName\":null}";
        return m_json;
    }

    public static IEnumerator testMethod_acu(string app_type, string SA_id, string fyId, Action<App> ac = null)
    {
        var httpRequest = new HttpRequest
        {
            Url = "http://api.vesal.cn:8000/vesal-jiepao-prod/v1/app/struct/initMyStruct?token=1&plat=pc&fyId=" + fyId + "&version=1&appVersion=" + PublicClass.get_version(),
            //"http://114.115.210.145:8083/vesal-jiepao-test/v2/app/struct/getHomeStruct?plat=android&appVersion=3.2.0&business=anatomy&funType=RTGZ&token=1",
            Method = HttpMethod.Get
        };
        HttpClient.Instance.SendAsync(httpRequest, httpResponse =>
        {
            Debug.Log("Url: " + httpRequest.Url);
            Debug.Log("httpResponse.IsSuccess" + httpResponse.IsSuccess);
            if (httpResponse.IsSuccess)
            {
                Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(httpResponse.Data);
                foreach (string tmpKey in r.Keys)
                {
                    UnityEngine.Debug.Log(tmpKey);
                    UnityEngine.Debug.Log(r[tmpKey].ToString());
                }
                if (r.ContainsKey("List"))
                {
                    List<Dictionary<string, object>> tmpData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(r["List"].ToString());
                    foreach (Dictionary<string, object> tmpDic in tmpData)
                    {
                        foreach (string tmpKey in tmpDic.Keys)
                        {
                            UnityEngine.Debug.Log(tmpKey);
                            List<App> tmpAppData = JsonConvert.DeserializeObject<List<App>>(tmpDic["StructList"].ToString());
                            foreach (App tmpApp in tmpAppData)
                            {
                                if (tmpApp.app_type == app_type && (tmpApp.app_id.ToLower() == SA_id.ToLower() || SA_id == ""))
                                {
                                    UnityEngine.Debug.Log(tmpApp.struct_name + " " + tmpApp.app_type);
                                    UnityEngine.Debug.Log(tmpApp.struct_name);
                                    UnityEngine.Debug.Log(tmpApp.ab_list);
                                    UnityEngine.Debug.Log(tmpApp.ab_path);
                                    UnityEngine.Debug.Log(tmpApp.app_id);
                                    UnityEngine.Debug.Log(SA_id);
                                    ac(tmpApp);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        });
        yield return null;
    }

    public static IEnumerator testMethod_wk_lz(string dict_name, string fyId, string app_type, string SA_id, Action<App> ac = null)
    {
        var httpRequest = new HttpRequest
        {
            Url = "http://api.vesal.cn:8000/vesal-jiepao-prod/v1/app/struct/initMyStruct?token=1&plat=pc&fyId=" + fyId + "&version=1&appVersion=2.4.0",
            Method = HttpMethod.Get
        };
        HttpClient.Instance.SendAsync(httpRequest, httpResponse =>
        {
            Debug.Log("Url: " + httpRequest.Url);
            Debug.Log("httpResponse.IsSuccess" + httpResponse.IsSuccess);
            if (httpResponse.IsSuccess)
            {
                Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(httpResponse.Data);
                foreach (string tmpKey in r.Keys)
                {
                    UnityEngine.Debug.Log(tmpKey);
                    UnityEngine.Debug.Log(r[tmpKey].ToString());
                }
                if (r.ContainsKey("List"))
                {
                    List<Dictionary<string, object>> tmpData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(r["List"].ToString());
                    foreach (Dictionary<string, object> tmpDic in tmpData)
                    {
                        foreach (string tmpKey in tmpDic.Keys)
                        {
                            UnityEngine.Debug.Log(tmpKey);
                            List<App> tmpAppData = JsonConvert.DeserializeObject<List<App>>(tmpDic["StructList"].ToString());
                            foreach (App tmpApp in tmpAppData)
                            {
                                if (tmpApp.app_type == app_type && (tmpApp.app_id.ToLower() == SA_id.ToLower() || SA_id == ""))
                                {
                                    UnityEngine.Debug.Log(tmpApp.struct_name + " " + tmpApp.app_type);
                                    UnityEngine.Debug.Log(tmpApp.struct_name);
                                    UnityEngine.Debug.Log(tmpApp.ab_list);
                                    UnityEngine.Debug.Log(tmpApp.ab_path);
                                    UnityEngine.Debug.Log(tmpApp.app_id);
                                    UnityEngine.Debug.Log(SA_id);
                                    ac(tmpApp);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        });
        yield return null;
    }

    public static string testMethod()
    {
        string m_json = "{\"youke_use\":\"disabled\",\"platform\":\"android,ios,pc,ppt\",\"struct_state\":null,\"fy_id\":\"5\",\"fy_name\":null,\"Introduce\":\"\",\"struct_id\":\"397\",\"struct_sort\":\"39\",\"function_type\":\"5\",\"struct_sell_amount\":\"0\",\"visibel_identity\":null,\"struct_name\":\"手指屈曲\",\"struct_code\":\"DA0100039\",\"ab_path\":\"\",\"xml_path\":null,\"model_scope\":null,\"app_type\":\"animation\",\"app_id\":\"DA0100039\",\"app_version\":\"1\",\"update_flag\":null,\"ab_list\":\"\",\"isTablet\":null,\"JumpState\":null,\"signModelName\":null,\"tissueModelName\":null}";
        return m_json;
    }
}