using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightMenuManager : MonoBehaviour
{

    //public GameObject RightMenuItem;
    DbRepository<RightMenuModel> local_db;
    //public void InitRightMenuManager(string[] rmids)
    //{
    //    InitRightMenuManager2(rmids);
    //}
    public IEnumerator InitRightMenuManagerAsync(string[] rmids)
    {
        yield return null;
        //InitRightMenuManager(rmids, SceneModels.instance.getModels());
        Dictionary<string, Model> allModels = SceneModels.instance.getModels();
        if (local_db == null)
        {
            local_db = new DbRepository<RightMenuModel>();
            local_db.DataService("vesali.db");
        }
        clearChilds();
        List<int> tmpRmids = new List<int>();
        yield return null;
        foreach (string tmpRmid in rmids)
        {
            try
            {
                tmpRmids.Add(int.Parse(tmpRmid));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
                continue;
            }
        }
        yield return null;
        IEnumerable<RightMenuModel> rmms = getRmms(tmpRmids);
        if (rmms == null)
        {
            Destroy(this.gameObject);
        }
        yield return null;
        // Debug.Log(rmms);
        StartCoroutine(InitSubItems(rmms, allModels,initresult));
        //if (!InitSubItems(rmms, allModels))
        //{
        //    clearChilds();
        //}
        yield return null;

    }
    public void initresult(bool result) {
        if (!result) {
            clearChilds();

        }
        local_db.Close();
        local_db = null;
    }
    public void clearChilds()
    {

        for (int i = 0; i < transform.childCount; i++)
        {

            Transform tmpTfs = transform.GetChild(i);
            //if (tmpTfs.name != RightMenuItem.name)
            //{
            //}
            Destroy(tmpTfs.gameObject);
        }
    }
    public void InitRightMenuManager(string[] rmids)
    {
        //long useTime = PublicTools.GetTimeStamp();
        //int stepcount = 0;


        if (local_db == null)
        {
            local_db = new DbRepository<RightMenuModel>();
            local_db.DataService("vesali.db");
        }
        clearChilds();
        List<int> tmpRmids = new List<int>();

        foreach (string tmpRmid in rmids)
        {
            tmpRmids.Add(int.Parse(tmpRmid));
        }
        //stepcount++;
        //useTime = PublicTools.GetTimeStamp() - useTime;
        //useTime = PublicTools.GetTimeStamp();

        IEnumerable<RightMenuModel> rmms = getRmms(tmpRmids);
        if (rmms == null)
        {
            Destroy(this.gameObject);
        }
        //stepcount++;
        //useTime = PublicTools.GetTimeStamp() - useTime;
        //Debug.Log("stepcount:" + stepcount +" -- " + (useTime));
        //useTime = PublicTools.GetTimeStamp();
        // Debug.Log(rmms);
        if (!InitSubItems(rmms))
        {
            clearChilds();
        }
        //stepcount++;
        //useTime = PublicTools.GetTimeStamp() - useTime;
        //Debug.Log("stepcount:" + stepcount + " -- " + (useTime));
        local_db.Close();
        local_db = null;

    }
    bool InitSubItems(IEnumerable<RightMenuModel> rmms)
    {
        //long useTime = PublicTools.GetTimeStamp(false);
        //int subStepcount = 0;



        Dictionary<string, Model> allModels = SceneModels.instance.getModels();
        try
        {
            var local_db3 = new DbRepository<RightMenuLayerModel>();
            local_db3.DataService("vesali.db");

            var local_db2 = new DbRepository<LayserSubModel>();
            local_db2.DataService("vesali.db");

            //subStepcount++;
            //useTime = PublicTools.GetTimeStamp(false) - useTime;
            //Debug.Log("subStepcount :" + subStepcount + " -- " + (useTime));
            //useTime = PublicTools.GetTimeStamp(false);

            IEnumerable<LayserSubModel> tmplsms = local_db2.Select<LayserSubModel>((tmplsm) =>
            {
                return true;
            });
            Dictionary<string, List<string>> tmpLsmDc = new Dictionary<string, List<string>>();
            foreach (LayserSubModel tmpSm in tmplsms) {
                if (!tmpLsmDc.ContainsKey(tmpSm.layer_id)) {
                    tmpLsmDc.Add(tmpSm.layer_id,new List<string>());
                }
                tmpLsmDc[tmpSm.layer_id].Add(tmpSm.sm_Name);
            }

            //subStepcount++;
            //useTime = PublicTools.GetTimeStamp(false) - useTime;
            //Debug.Log("subStepcount :" + subStepcount + " -- " + (useTime));
            //useTime = PublicTools.GetTimeStamp(false);

           // long count1 = 0;
            //long count2 = 0;
            //long count3 = useTime;
            foreach (RightMenuModel rmm in rmms)
            {

                RightMenuGroup tmprmg = new RightMenuGroup();
                tmprmg.groupName = rmm.rm_name;
                tmprmg.addImage_base64 = rmm.add_img;
                tmprmg.removeImage_base64 = rmm.remove_img;
                tmprmg.normalImage_base64 = rmm.normal_img;
                tmprmg.disableImage_base64 = rmm.disable_img;

                int tmpMax = 0, tmpMin = 0;
                IEnumerable<RightMenuLayerModel> rmlms = local_db3.Select<RightMenuLayerModel>((tmp) =>
                {
                    if (tmp.rm_id == rmm.rm_id)
                    {

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
               
                //useTime = PublicTools.GetTimeStamp(false);

                foreach (RightMenuLayerModel tmplm in rmlms)
                {
                    List<Model> tmpList = new List<Model>();
                    List<string> tmpModelNames = tmpLsmDc[tmplm.layer_id];
                    for (int i = 0;i<tmpModelNames.Count;i++) {
                        string tmpName = tmpModelNames[i];
                        if (allModels.ContainsKey(tmpName))
                        {
                            tmpList.Add(allModels[tmpName]);
                        }
                    }
                    //foreach (LayserSubModel tmpSm in lsms)
                    //{
                    //    if (allModels.ContainsKey(tmpSm.sm_Name))
                    //    {
                    //        tmpList.Add(allModels[tmpSm.sm_Name]);
                    //    }
                    //}
                    if (tmpList.Count > 0)
                    {
                        tmpMax = Mathf.Max(tmpMax, tmplm.layer);
                        tmpMin = Mathf.Min(tmpMin, tmplm.layer);
                        tmprmg.layers.Add(tmplm.layer, new RightMenuLayer
                        {
                            models = tmpList
                        });
                    }
                }

                //subStepcount++;
                //useTime = PublicTools.GetTimeStamp(false) - useTime;
                //Debug.Log("subStepcount :" + subStepcount + " -- " + (useTime));
                //count1 += useTime;
                //useTime = PublicTools.GetTimeStamp(false);
                if (tmprmg.layers.Count > 0)
                {
                    

                    tmprmg.maxlayerNo = tmpMax;
                    tmprmg.minlayerNo = tmpMin;
                    GameObject tmpItem = Instantiate(Resources.Load<GameObject>("Prefab/RightMenuItem"), transform);

        

                    tmpItem.transform.SetParent(this.transform);
                    tmpItem.transform.localScale = Vector3.one;
                    tmpItem.transform.localPosition = new Vector3(tmpItem.transform.localPosition.x, tmpItem.transform.localPosition.y, 0);
                    tmpItem.SetActive(true);
                    tmpItem.name = rmm.rm_name;

                   
                    //useTime = PublicTools.GetTimeStamp(false);

                    tmpItem.GetComponent<RightMenuItem>().InitRightMenuItem(tmprmg);
                    //tmpItem.GetComponent<RightMenuItem>().RefreshImage();

                    //subStepcount++;
                    //useTime = PublicTools.GetTimeStamp(false) - useTime;
                    //count2 += useTime;
                    //Debug.Log("subStepcount :" + subStepcount + " -- " + (useTime));
                    //useTime = PublicTools.GetTimeStamp(false);

                }
                //subStepcount++;
              
            }
            //Debug.Log("COUNT1"+count1);
            //Debug.Log("COUNT2" + count2);
            //subStepcount++;
            //useTime = PublicTools.GetTimeStamp(false) - useTime;
            //Debug.Log("subStepcount :" + subStepcount + " -- " + (useTime));
            //useTime = PublicTools.GetTimeStamp(false);
            local_db3.Close();
            local_db2.Close();
            if (this.transform.childCount != 0)
            {
                gameObject.BroadcastMessage("RefreshImage", null);
            }
            //subStepcount++;
            //useTime = PublicTools.GetTimeStamp(false) - useTime;
            //Debug.Log("subStepcount :" + subStepcount + " -- " + (useTime));
            //subStepcount++;
            //useTime = PublicTools.GetTimeStamp(false) - count3;
            //Debug.Log("subStepcount total:" + subStepcount + " -- " + (useTime));

            
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            return false;
        }


        return true;
    }
    IEnumerator InitSubItems(IEnumerable<RightMenuModel> rmms, Dictionary<string, Model> allModels, Action<bool> callback)
    {
        yield return null;
        var local_db3 = new DbRepository<RightMenuLayerModel>();
        var local_db2 = new DbRepository<LayserSubModel>();
        try
        {
            local_db3.DataService("vesali.db");
            local_db2.DataService("vesali.db");
        }
        catch {
            if (callback != null) {
                callback(false);
            }
        }


        yield return null;

        foreach (RightMenuModel rmm in rmms)
        {
            RightMenuGroup tmprmg = new RightMenuGroup();
            //try
            //{


                tmprmg.groupName = rmm.rm_name;
                tmprmg.addImage_base64 = rmm.add_img;
                tmprmg.removeImage_base64 = rmm.remove_img;
                tmprmg.normalImage_base64 = rmm.normal_img;
                tmprmg.disableImage_base64 = rmm.disable_img;

                int tmpMax = 0, tmpMin = 0;
                IEnumerable<RightMenuLayerModel> rmlms = local_db3.Select<RightMenuLayerModel>((tmp) =>
                {
                    if (tmp.rm_id == rmm.rm_id)
                    {

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

                foreach (RightMenuLayerModel tmplm in rmlms)
                {
                    List<Model> tmpList = new List<Model>();
                    IEnumerable<LayserSubModel> lsms = local_db2.Select<LayserSubModel>((tmplsm) =>
                    {
                        if (tmplsm.layer_id == tmplm.layer_id)
                        {


                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });

                    foreach (LayserSubModel tmpSm in lsms)
                    {
                        if (allModels.ContainsKey(tmpSm.sm_Name))
                        {
                            tmpList.Add(allModels[tmpSm.sm_Name]);
                        }
                    }
                    if (tmpList.Count > 0)
                    {
                        tmpMax = Mathf.Max(tmpMax, tmplm.layer);
                        tmpMin = Mathf.Min(tmpMin, tmplm.layer);
                        tmprmg.layers.Add(tmplm.layer, new RightMenuLayer
                        {
                            models = tmpList
                        });
                    }
                }

                if (tmprmg.layers.Count > 0)
                {
                    tmprmg.maxlayerNo = tmpMax;
                    tmprmg.minlayerNo = tmpMin;
                    GameObject tmpItem = Instantiate(Resources.Load<GameObject>("Prefab/RightMenuItem"), transform);
                    tmpItem.transform.SetParent(this.transform);
                    tmpItem.transform.localScale = Vector3.one;
                    tmpItem.transform.localPosition = new Vector3(tmpItem.transform.localPosition.x, tmpItem.transform.localPosition.y, 0);
                    tmpItem.SetActive(true);
                    tmpItem.name = rmm.rm_name;
                    tmpItem.GetComponent<RightMenuItem>().InitRightMenuItem(tmprmg);
                    tmpItem.GetComponent<RightMenuItem>().RefreshImage();

                }
            //}
            //catch
            //{
            //    if (callback != null)
            //    {
            //        callback(false);
            //    }
            //}
            yield return null;
        }
        yield return null;
        try
        {
            local_db3.Close();
            local_db2.Close();
        }
        catch
        {
            if (callback != null)
            {
                callback(false);
            }
        }

      
        yield return null;
        if (this.transform.childCount != 0)
        {
            gameObject.BroadcastMessage("RefreshImage", null);
        }

        yield return null;
        if (callback != null) {
            callback(true);
        }
    }


    IEnumerable<RightMenuModel> getRmms(List<int> tmpRmids)
    {
        //var local_db = new DbRepository<RightMenuModel>();
        //local_db.DataService("vesali.db");
        List<RightMenuModel> result = new List<RightMenuModel>();
        foreach (int tmpId in tmpRmids)
        {
            RightMenuModel rmm = local_db.SelectOne<RightMenuModel>((tempt) =>
            {
                if (tempt.rm_id == tmpId)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            });
            if (rmm != null)
            {
                result.Add(rmm);
            }
        }

        return result;
    }
    void Start()
    {

    }

    //Update is called once per frame
    void Update()
    {

    }
}
