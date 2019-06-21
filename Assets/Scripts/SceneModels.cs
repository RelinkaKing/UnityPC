using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PPTPlayer;
using UnityEngine;

public class SceneModels : MonoBehaviour
{
    public static SceneModels instance;
    //初始化模型列表
    List<string> init_scene_model_name;
    //范围模型列表
    public List<string> scope_scene_model_name;
    //当前场景内的模型列表
    List<string> current_scene_model_name;
    //选择的临时列表
    List<Model> selection_scene_model_name;
    Dictionary<string, float> ModelDisplayFormartDic; //记录每个模型初始格式（透明-2，正常1，隐藏0等）
    //Dictionary<string, List<string>> modelToRelation = new Dictionary<string, List<string>>();
    Dictionary<string, List<Model>> modelToRelation = null;
    //Dictionary<string, List<Model>> relationToModel = new Dictionary<string, List<Model>>();
    Dictionary<string, Model> Models = new Dictionary<string, Model>();
    //所有模型位置
    Dictionary<string, Vector3> scope_scene_model_position = new Dictionary<string, Vector3>();
    //旧子模型数据修补
    List<fix_submodel> old_submodel;
    private bool is_multi_Mode = false;
    private bool isSplit = false; //分割模式

    private bool isHide = false;//隐藏模式
    private bool isTran = false;//透明
    private bool isHideOther = false;//隐藏其他
    private bool isFadeOther = false;//透明其他

    //Transform temp_parent;

    RightMenuManager rmm;

    public int get_scope_model_numbers()
    {
        return scope_scene_model_name.Count;
    }

    public void set_Split_mode(bool split)
    {
        ModelController.addAction(RecordAction.toggleSp, split+"");
        isSplit = split;
    }
    public bool get_Split_mode()
    {
        return isSplit;
    }
    public void set_Multi_Selection(bool select)
    {
        ModelController.addAction(RecordAction.toggleMs, select+"");
        is_multi_Mode = select;
    }
    public bool get_Multi_Selection()
    {
        return is_multi_Mode;
    }

    public bool get_Hide_State()
    {
        return isHide;
    }
    public bool get_Tran_State()
    {
        return isTran;
    }

    public bool get_HideOther_State()
    {
        return isHideOther;
    }

    public bool get_TranOther_State()
    {
        return isFadeOther;
    }

    public GameObject GetSelectModel()
    {
        if (selection_scene_model_name.Count > 0)
            return selection_scene_model_name[0].gameObject;
        else
            return null;
    }
    //发送隐藏信息以提高性能
    public void SendHideMessage(string name)
    {
        if (rmm != null)
        {

            rmm.BroadcastMessage("HideAllLayer", name, SendMessageOptions.DontRequireReceiver);
        }
    }
    //发送显示信息恢复显示

    public void SendShowMessage(string name)
    {
        if (rmm != null)
        {

            rmm.BroadcastMessage("ShowAllLayer", name, SendMessageOptions.DontRequireReceiver);
        }
    }


    string init_nounNo;
    string scope_nounNo;

    public void InitVariate()
    {
        //补充这个两个状态初始化，因为有static instance存在
        is_multi_Mode = false;
        //分割模式
        isSplit = false;
        camParentPos = Vector3.zero;
        dis = 0;
        if (instance != null)
            DestroyImmediate(instance.gameObject, true);
        instance = this;
        init_scene_model_name = new List<string>();
        scope_scene_model_name = new List<string>();
        current_scene_model_name = new List<string>();
        selection_scene_model_name = new List<Model>();
        Models = new Dictionary<string, Model>();
        //modelToRelation = new Dictionary<string, List<string>>();
        //relationToModel = new Dictionary<string, List<Model>>();
        scope_scene_model_position = new Dictionary<string, Vector3>();
        old_submodel = new List<fix_submodel>();
        init_nounNo = string.Empty;
        GameObject rmObj = GameObject.Find("RightMenuManager");
        rmm = null;
        if (rmObj != null)
        {
            rmm = rmObj.GetComponent<RightMenuManager>();
        }

    }
    public void Init_SceneModels(string nounNo, bool isAsync = true)
    {
        InitVariate();
        init_nounNo = nounNo;
        bool inidSuccess = InitData(init_nounNo);
        //SetCameraPosition(PublicClass.Transform_parent, camParentPos, dis);
        if (!isAsync && inidSuccess)
        {
            init2(true);
            init3(nounNo);
        }
    }
    public void Init_SceneModelsByList(List<string> initList, List<string> scopeList, string[] rm_list = null, GetStructList tmpIe = null, bool isAsync = true)
    {
        InitVariate();
        if (scopeList == null)
            scopeList = initList;
        bool inidSuccess = InitDataByList(initList, scopeList, rm_list, tmpIe);
        if (!isAsync && inidSuccess)
        {
            init2(true);
            InitDataByList_phase2();
        }
    }
    // selection 
    // public string LastModelName { get { return null; } set { } }
    // public string CurrentModelName { get { return null; } set { } }

    public void BackSelectModel(List<Model> selection_model)
    {
        selection_scene_model_name.Clear();
        selection_scene_model_name.AddRange(selection_model);
        try
        {
            ChooseModel(selection_scene_model_name[selection_scene_model_name.Count - 1]);
        }
        catch
        {

        }
    }

    public void Show()
    {
        ModelController.addAction(RecordAction.Show,"");
        CommandManager.instance.RecordModelStata();
        for (int i = 0; i < selection_scene_model_name.Count; i++)
        {
            selection_scene_model_name[i].BecomeDisplay();
        }
    }
    public void Show(string nounNo)
    {
        ModelController.addAction(RecordAction.Show, nounNo);
        CommandManager.instance.RecordModelStata();
        List<string> templist = new List<string>();
        List<string> scope_list = new List<string>();
        scope_list.AddRange(Models.Keys);
        templist.AddRange(getListByNo(nounNo));
        for (int i = 0; i < templist.Count; i++)
        {
            for (int j = 0; j < scope_list.Count; j++)
            {
                if (templist[i] == scope_list[j])
                {
                    Models[scope_list[j]].BecomeDisplay();
                    continue;
                }
            }

        }
    }
    public void ShowAll()
    {
        ModelController.addAction(RecordAction.ShowAll, "");
        CommandManager.instance.RecordModelStata();
        List<string> scope_list = new List<string>();
        scope_list.AddRange(Models.Keys);
        for (int i = 0; i < scope_list.Count; i++)
        {
            for (int j = 0; j < scope_scene_model_name.Count; j++)
            {
                if (scope_list[i] == scope_scene_model_name[j])
                {
                    Models[scope_list[i]].BecomeDisplay();
                    continue;
                }
            }
        }
    }
    public void ShowOthers()
    {
        ModelController.addAction(RecordAction.ShowOthers, "");
        CommandManager.instance.RecordModelStata();
        for (int i = 0; i < scope_scene_model_name.Count; i++)
        {
            if (!Models[scope_scene_model_name[i]].isActive)
            {
                Models[scope_scene_model_name[i]].BecomeDisplay();
            }
            else
                Models[scope_scene_model_name[i]].BecomeHide();

        }
    }
    public bool isISO;
    GameObject[] thismodel;
    Transform[] parent;
    Vector3[] pos;
    Vector3 Campos;
    Vector3 CamBoxpos;
    public void ISO()
    {

        int NUM = selection_scene_model_name.Count;
        Debug.Log(NUM);

        if (NUM > 0)
        {
            ModelController.addAction(RecordAction.ISO, "");
            //bool isHide=false;


            if (!isISO)
            {
                thismodel = new GameObject[NUM];
                parent = new Transform[NUM];
                pos = new Vector3[NUM];
                float[] x = new float[NUM];
                float[] y = new float[NUM];
                float[] z = new float[NUM];
                CamBoxpos = Camera.main.transform.parent.localPosition;
                Campos = Camera.main.transform.localPosition;
                for (int i = 0; i < NUM; i++)
                {
                    thismodel[i] = selection_scene_model_name[i].gameObject;
               //     thismodel[i].GetComponent<Model>().BeSignShow();
                }
                CommandManager.instance.RecordModelStata();
                Camera.main.transform.localPosition = new Vector3(0,0,Campos.z);
                Camera.main.transform.parent.localPosition = Vector3.zero;
                for (int i = 0; i < NUM; i++)
                {


                    parent[i] = thismodel[i].transform.parent;
                    pos[i] = thismodel[i].transform.position;
                    x[i] = pos[i].x;
                    y[i] = pos[i].y;
                    z[i] = pos[i].z;

                   

                    thismodel[i].transform.SetParent(GameObject.Find("ModelParent").transform);

                    // thismodel[i].transform.position = Vector3.zero;
                }
                //   Debug.Log(parent[1].name);
                ArrayList list = new ArrayList(x);
                list.Sort();
                float min = (float)Convert.ToDouble(list[0]);
                float max = (float)Convert.ToDouble(list[list.Count - 1]);
                float midel = (min + max) / 2.0f;

                //for (int i = 0; i < NUM; i++)
                //{
                //    thismodel[i].transform.position -= new Vector3(midel, 0, 0);
                //}
                Camera.main.transform.parent.localPosition+= new Vector3(midel, 0, 0);

                list = new ArrayList(y);
                list.Sort();
                min = (float)Convert.ToDouble(list[0]);
                max = (float)Convert.ToDouble(list[list.Count - 1]);
                midel = (min + max) / 2.0f;
                //for (int i = 0; i < NUM; i++)
                //{
                //    thismodel[i].transform.position -= new Vector3(0, midel, 0);
                //}
                Camera.main.transform.parent.localPosition += new Vector3(0, midel, 0);
                list = new ArrayList(z);
                list.Sort();
                min = (float)Convert.ToDouble(list[0]);
                max = (float)Convert.ToDouble(list[list.Count - 1]);
                midel = (min + max) / 2.0f;
                //for (int i = 0; i < NUM; i++)
                //{
                //    thismodel[i].transform.position -= new Vector3(0, 0, midel);
                //}
                Camera.main.transform.parent.localPosition += new Vector3(0, 0, midel);
            }
            else
            {

                Camera.main.transform.localPosition = Campos;
                Camera.main.transform.parent.localPosition = CamBoxpos;
                //for (int i = 0; i < NUM; i++)
                //{
                //    thismodel[i].transform.SetParent(parent[i]);
                //    thismodel[i].transform.position = pos[i];
                //}
            }

            isISO = !isISO;
            foreach (Model m in Models.Values)
            {
                if (!selection_scene_model_name.Contains(m))
                {
                    if (isISO)
                    {
                        m.BecomeHide();
                    }
                    else
                    {
                        m.BecomeNormal();
                        m.BecomeDisplay();
                    }
                }
            }
        }
    }



    public void Hide()
    {
        ModelController.addAction(RecordAction.Hide, "");
        //bool isHide=false;
        CommandManager.instance.RecordModelStata();
        for (int i = 0; i < selection_scene_model_name.Count; i++)
        {
            Debug.Log(selection_scene_model_name[i].name);
            if (selection_scene_model_name[i].isActive)
            {
                selection_scene_model_name[i].BecomeHide();

                relationModelHideOrNormal(selection_scene_model_name[i].name, true);
                //selection_scene_model_name.Remove(selection_scene_model_name[i]);
                isHide = true;
            }


            //第二次点击显示选择，后续功能需要，不能删
            else
            {
                selection_scene_model_name[i].BecomeDisplay();
                relationModelHideOrNormal(selection_scene_model_name[i].name, false);
                isHide = false;
            }
        }
       // selection_scene_model_name.Clear();

        //return isHide;
    }

    public void relationModelHideOrNormal(string modelName, bool isHide)
    {
        if (modelToRelation.ContainsKey(modelName))
        {
            foreach (Model tmpModel in modelToRelation[modelName])
            {
                if (isHide)
                {
                    tmpModel.BecomeHide();
                }
                else
                {
                    tmpModel.BecomeNormal();
                    tmpModel.BecomeDisplay();
                }
            }

            //foreach (string tmpKey in modelToRelation[modelName])
            //{
            //    if (relationToModel.ContainsKey(tmpKey))
            //    {
                    
            //    }
            //}
        }

    }

    public void SplitModel(string param)
    {
        ModelController.addAction(RecordAction.Splite, param);
        CommandManager.instance.RecordModelStata();
        try
        {
            string[] pms = param.Split('$');
            string modelName = pms[0];
            Vector3 pos = PublicTools.Str2Vector3(pms[1]);
            if (Models.ContainsKey(modelName))
            {
                Models[modelName].transform.position = pos;
                relationModelSplite(modelName, pos);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void relationModelSplite(string modelName, Vector3 currentPos)
    {
        try
        {
            if (modelToRelation.ContainsKey(modelName))
            {
                //偏移量
                Vector3 offset = currentPos - SceneModels.instance.getPos(modelName);
                //foreach (string tmpKey in modelToRelation[modelName])
                //{
                //    if (relationToModel.ContainsKey(tmpKey))
                //    {
                //        foreach (Model tmpModel in relationToModel[tmpKey])
                //        {
                //            tmpModel.transform.position = SceneModels.instance.getPos(tmpModel.name) + offset;
                //        }
                //    }
                //}
                foreach (Model tmpModel in modelToRelation[modelName])
                {
                    tmpModel.transform.position = SceneModels.instance.getPos(tmpModel.name) + offset;
                    //if (relationToModel.ContainsKey(tmpKey))
                    //{
                    //    foreach (Model tmpModel in relationToModel[tmpKey])
                    //    {
                    //        tmpModel.transform.position = SceneModels.instance.getPos(tmpModel.name) + offset;
                    //    }
                    //}
                    //if (relationToModel.ContainsKey(tmpKey))
                    //{
                    //    foreach (Model tmpModel in relationToModel[tmpKey])
                    //    {
                    //        tmpModel.transform.position = SceneModels.instance.getPos(tmpModel.name) + offset;
                    //    }
                    //}
                }

            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }

    }
    public void Hide(string nounNo)
    {
        ModelController.addAction(RecordAction.Hide, nounNo);
        CommandManager.instance.RecordModelStata();
        List<string> templist = new List<string>();
        List<string> scope_list = new List<string>();
        scope_list.AddRange(Models.Keys);
        templist.AddRange(getListByNo(nounNo));
        for (int i = 0; i < templist.Count; i++)
        {
            for (int j = 0; j < scope_list.Count; j++)
            {
                if (templist[i] == scope_list[j])
                {
                    Models[scope_list[j]].BecomeHide();
                    continue;
                }
            }

        }
    }
    //bool isHideOther = false;
    public void HideOthers()
    {
        ModelController.addAction(RecordAction.HideOthers, "");
        CommandManager.instance.RecordModelStata();
        isHideOther = !isHideOther;
        foreach (Model m in Models.Values)
        {
            if (!selection_scene_model_name.Contains(m))
            {
                if (isHideOther)
                {
                    m.BecomeHide();
                }
                else
                {
                    m.BecomeNormal();
                    m.BecomeDisplay();
                }
            }
        }
        //for (int i = 0; i < scope_scene_model_name.Count; i++)
        //{
        //    for (int j = 0; j < selection_scene_model_name.Count; j++)
        //    {
        //        if (isHideOther)
        //        {

        //            if (scope_scene_model_name[i] == selection_scene_model_name[j].name)
        //            {
        //                try
        //                {

        //                    Models[scope_scene_model_name[i]].BecomeDisplay();
        //                }
        //                catch (System.Exception)
        //                {
        //                    print(scope_scene_model_name[i]);
        //                }
        //            }
        //            else
        //            {
        //                try
        //                {

        //                    Models[scope_scene_model_name[i]].BecomeHide();
        //                }
        //                catch (System.Exception)
        //                {
        //                    print(scope_scene_model_name[i]);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (scope_scene_model_name[i] == selection_scene_model_name[j].name)
        //            {
        //                try
        //                {

        //                    Models[scope_scene_model_name[i]].BecomeHide();
        //                }
        //                catch (System.Exception)
        //                {
        //                    print(scope_scene_model_name[i]);
        //                }
        //            }
        //            else
        //                try
        //                {

        //                    Models[scope_scene_model_name[i]].BecomeDisplay();
        //                }
        //                catch (System.Exception)
        //                {
        //                    print(scope_scene_model_name[i]);
        //                }
        //        }

        //    }

        //}
    }

    public void Fade()
    {
        ModelController.addAction(RecordAction.Fade, "");
        CommandManager.instance.RecordModelStata();
        for (int i = 0; i < selection_scene_model_name.Count; i++)
        {

            //再次点击恢复显示，新版本用得到，不能删。
            if (selection_scene_model_name[i].isTranslucent)
            {
                selection_scene_model_name[i].BecomeNormal();
                selection_scene_model_name.Remove(selection_scene_model_name[i]);
                isTran = false;
            }


            else
            {
                selection_scene_model_name[i].BecomeTranslucent();

               // selection_scene_model_name.Remove(selection_scene_model_name[i]);
                isTran = true;
            }
        }
        selection_scene_model_name.Clear();
    }
    public void Fade(string nounNo)
    {
        ModelController.addAction(RecordAction.Fade, nounNo);
        CommandManager.instance.RecordModelStata();
        List<string> templist = new List<string>();
        List<string> scope_list = new List<string>();
        scope_list.AddRange(Models.Keys);
        templist.AddRange(getListByNo(nounNo));
        for (int i = 0; i < templist.Count; i++)
        {
            for (int j = 0; j < scope_list.Count; j++)
            {
                if (templist[i] == scope_list[j])
                {
                    Models[scope_list[j]].BecomeTranslucent();
                    continue;
                }
            }

        }
    }

    //bool isFadeOther = false;
    public void FadeOthers()
    {
        ModelController.addAction(RecordAction.FadeOthers, "");
        CommandManager.instance.RecordModelStata();
        isFadeOther = !isFadeOther;
        foreach (Model m in Models.Values)
        {
            if (!selection_scene_model_name.Contains(m))
            {
                if (isFadeOther)
                {
                    m.BecomeTranslucent();
                }
                else
                {
                    m.BecomeNormal();
                }
            }
        }
        //for (int i = 0; i < init_scene_model_name.Count; i++)
        //{
        //    for (int j = 0; j < selection_scene_model_name.Count; j++)
        //    {
        //        if (isFadeOther)
        //        {

        //            if (init_scene_model_name[i] == selection_scene_model_name[j].name)
        //            {
        //                try
        //                {

        //                    Models[init_scene_model_name[i]].BecomeNormal();
        //                }
        //                catch
        //                {
        //                    print(init_scene_model_name[i]);
        //                }
        //            }
        //            else
        //            {
        //                try
        //                {

        //                    Models[init_scene_model_name[i]].BecomeTranslucent();
        //                }
        //                catch
        //                {
        //                    print(init_scene_model_name[i]);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (init_scene_model_name[i] == selection_scene_model_name[j].name)
        //            {
        //                try
        //                {

        //                    Models[init_scene_model_name[i]].BecomeTranslucent();
        //                }
        //                catch
        //                {
        //                    print(init_scene_model_name[i]);
        //                }
        //            }
        //            else
        //            {
        //                try
        //                {

        //                    Models[init_scene_model_name[i]].BecomeNormal();
        //                }
        //                catch
        //                {
        //                    print(init_scene_model_name[i]);
        //                }
        //            }
        //        }
        //    }
        //}
    }

    public void ResetModel(GameObject g)
    {
        CommandManager.instance.CanOperaCommand(false);
        Model tmpM = g.GetComponent<Model>();
        if (tmpM != null)
        {
            tmpM.BecomeDisplay();
            tmpM.BecomeNormal();
            tmpM.transform.position = scope_scene_model_position[tmpM.name];
        }
    }
    public void ResetAllModels(bool isAutoSetPosition = true)
    {
        CommandManager.instance.CanOperaCommand(false);
        foreach (Model m in Models.Values)
        {
            //显示C集合，隐藏B集合其它模型
            if (init_scene_model_name.Contains(m.name))
            {
                m.BecomeDisplay();
                m.BecomeNormal();
            }
            else
            {
                m.BecomeHide();
            }
            //重置拆分
            m.transform.position = scope_scene_model_position[m.name];
        }
        if (CameraSynchronization.instance != null)
        {
            //重置小人
            for (int i = 0; i < CameraSynchronization.instance.littleMap.Length; i++)
            {
                CameraSynchronization.instance.littleMap[i].material = CameraSynchronization.instance.littleMapTexture[0];
            }
        }
        selection_scene_model_name.Clear();
        if (isAutoSetPosition)
        {
            SetCameraPosition(PublicClass.Transform_parent, camParentPos, dis);
        }

        //if (init_nounNo != "")
        //{
        //    InitTransparency(init_nounNo);
        //}
    }

    //public bool isGiveModelHided(List<string> modelNames) { return false; }
    //public List<string> filterNotExistName(List<string> querryNames) {return null; }
    //public void Translation(Vector3 vect) { }
    //public void Rotation(Vector3 v3) { }
    //public void Zoom(float z_vect) { }
    /// <summary>
    /// 取消选择
    /// </summary>
    public void CancleSelect()
    {
        if (selection_scene_model_name != null)
        {
            foreach (Model m in selection_scene_model_name)
            {
                //m.BecomeDisplay();
                if (m != null)
                {
                    try
                    {
                        if (!m.isTranslucent)
                        {
                            m.BecomeNormal();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
            selection_scene_model_name.Clear();
        }
    }

    public ModelInfo ChooseModel(GameObject g)
    {
        Model tmpM = g.GetComponent<Model>();

        if (tmpM != null)
        {
            ChooseModel(tmpM);
            if (PublicClass.infoDic.ContainsKey(tmpM.name))
                return PublicClass.infoDic[tmpM.name];
        }
        return null;
    }

    public ModelInfo GetModelInfo(GameObject g)
    {
        Model tmpM = g.GetComponent<Model>();

        if (tmpM != null)
        {
            if (PublicClass.infoDic.ContainsKey(tmpM.name))
                return PublicClass.infoDic[tmpM.name];
        }
        return null;
    }
    public void showModel(string ModelName)
    {
        //if (Models.ContainsKey(ModelName))
        //{
        //    if (!Models[ModelName].isActive)
        //    {
        //        Models[ModelName].isActive = true;

        //    }
        //    ChooseModel(Models[ModelName]);
        //}
    }
    public void ChooseModelByName(string ModelName)
    {
        if (Models.ContainsKey(ModelName))
        {
            ChooseModel(Models[ModelName]);
        }
    }

    public bool IsChooseModelByName(string ModelName)
    {
        if (Models.ContainsKey(ModelName))
        {
            ChooseModel(Models[ModelName]);
            return true;
        }
        return false;
    }

    public bool IsChooseModelByNameSelect(string ModelName)
    {
        if (Models.ContainsKey(ModelName))
        {
            if(!Models[ModelName].isSeleted)
            ChooseModel(Models[ModelName]);
            return true;
        }
        return false;
    }
    public bool IsChooseModelByNameFadeTOHidi(string ModelName)
    {
        if (Models.ContainsKey(ModelName))
        {

            Models[ModelName].isTranslucent = false;
            if(Models[ModelName].isSeleted)
                ChooseModel(Models[ModelName]);
            ChooseModel(Models[ModelName]);
            return true;
        }
        return false;
    }
    public void hidemodel(string ModelName,bool isshow)
    {
        if (Models.ContainsKey(ModelName))
        {
            Models[ModelName].isActive = isshow;
            Models[ModelName].gameObject.SetActive(isshow);
        }
    }

        public Model GetModel(string ModelName)
    {
        if (Models.ContainsKey(ModelName))
        {
            return Models[ModelName];
        }
        else
            return null;
    }

    public void ChooseModel(string nounNo)
    {
        OperaModelByNounNo(ChooseModel, nounNo);
    }
    public void OperaModelByNounNo(Action<Model> ac, string nounNo)
    {
        OperaModelByNameList(ac, getListByNo(nounNo));

    }

    public void DisplayModels(List<string> names)
    {
        OperaModelByNameList((model) => { model.BecomeHide(); }, scope_scene_model_name);
        OperaModelByNameList((model) => { model.BecomeDisplay(); }, names);
    }

    public void OperaModelByNameList(Action<Model> ac, List<string> names)
    {
        if (ac != null)
        {
            foreach (string tmpName in names)
            {
                if (Models.ContainsKey(tmpName))
                {
                    Model m = Models[tmpName];
                    ac(m);
                }
            }
        }
    }
    public void OperaModelByModelList(Action<Model> ac, List<Model> names)
    {
        if (ac != null)
        {
            foreach (Model tmpModel in names)
            {
                ac(tmpModel);
            }
        }
    }
    public void inithide()
    {
        isHide=false;
    }
    public void ChooseModel(Model m)
    {
        ModelController.addAction(RecordAction.Choose, m.name);
        if (!is_multi_Mode && selection_scene_model_name.Count != 0)
        {
            if (selection_scene_model_name.Contains(m))
            {
                m.BecomeNormal();
                // selection_scene_model_name.Remove(m);
            }
            else
                CancleSelect();
        }
        if (!selection_scene_model_name.Contains(m))
        {
            //XT_AllButton.Instance.ResetBottomBtn();
            selection_scene_model_name.Add(m);
            m.BecomeDisplay();
            m.BecomeHight();
        }
        else
        {

            selection_scene_model_name.Remove(m);
            m.BecomeNormal();
            //if (TouchCtrl.isCurMode)
            //{
            //    m.BecomeHight();
            //}
        }

    }
    public Model getLastChoose()
    {
        if (selection_scene_model_name.Count != 0)
        {
            return selection_scene_model_name[selection_scene_model_name.Count - 1];
        }
        else
        {
            return null;
        }
    }
    public List<Model> getSelects()
    {
        return selection_scene_model_name;
    }
    public Vector3 getPos(string name)
    {
        if (scope_scene_model_position.ContainsKey(name))
        {
            return scope_scene_model_position[name];
        }
        throw new Exception("getPos null");
    }

    string[] temp_rm_list;
    GetStructList temp_tmpIe;

    public void SetCamera(GetStructList tmpIe)
    {
        if (temp_tmpIe != null)
        {
            camParentPos = Vector3.zero;
            camParentRot = Vector3.zero;
            if (!(temp_tmpIe.camPos == "" || temp_tmpIe.camPos == null))
                {
                    dis = PublicTools.Str2Vector3(temp_tmpIe.camPos).z;
                }
                if (temp_tmpIe != null && !(temp_tmpIe.camParentPos == "" || temp_tmpIe.camParentPos == null))
                {
                    camParentPos = PublicTools.Str2Vector3(temp_tmpIe.camParentPos);
                }
                if (!(temp_tmpIe.camParentRot == "" || temp_tmpIe.camParentRot == null))
                {
                    camParentRot = PublicTools.Str2Vector3(temp_tmpIe.camParentRot);
                }
            SetCameraPosition(PublicClass.Transform_parent, camParentPos, camParentRot, dis);
        }
        Interaction.instance.setParamValue2();
    }

    bool InitDataByList(List<string> initList, List<string> scopeList, string[] rm_list = null, GetStructList tmpIe = null)
    {
        temp_rm_list = rm_list;
        temp_tmpIe = tmpIe;

        dis = 0f;
        camParentPos = Vector3.zero;
        camParentRot = Vector3.zero;
        if (temp_tmpIe != null)
        {
            try
            {
                if (!(temp_tmpIe.camPos == "" || temp_tmpIe.camPos == null))
                {
                    dis = PublicTools.Str2Vector3(temp_tmpIe.camPos).z;
                }
                if (temp_tmpIe != null && !(temp_tmpIe.camParentPos == "" || temp_tmpIe.camParentPos == null))
                {
                    camParentPos = PublicTools.Str2Vector3(temp_tmpIe.camParentPos);
                }
                if (!(temp_tmpIe.camParentRot == "" || temp_tmpIe.camParentRot == null))
                {
                    camParentRot = PublicTools.Str2Vector3(temp_tmpIe.camParentRot);
                }
            }
            catch (Exception e)
            {
                dis = 0f;
                camParentPos = Vector3.zero;
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }

        DebugLog.DebugLogInfo("scopeList count :" + scopeList.Count);

        destoryTempParent();
        PublicClass.Transform_parent.gameObject.SetActive(true);
        //重新获取显示模组
        init_scene_model_name.Clear();
        init_scene_model_name.AddRange(initList);
        scope_scene_model_name.Clear();
        if (scopeList != null)
        {
            scope_scene_model_name.AddRange(scopeList);
        }
        else
        {
            scope_scene_model_name.AddRange(initList);
        }
        Models.Clear();
        
        return true;

    }

    void InitDataByList_phase2()
    {
        DebugLog.DebugLogInfo("关联模型 ");
        modelToRelation = null;
        //定义关联模型
        InitRelation();
        current_scene_model_name.AddRange(init_scene_model_name);
        DebugLog.DebugLogInfo("当前场景模型长度 " + init_scene_model_name.Count);

        //生成右侧菜单
        if (temp_rm_list != null && temp_rm_list.Length > 0)
        {
            InitSceneModelByNo(temp_rm_list);
        }
        //        if (scopeList != null)
        //        {
        //显示bc交集
        for (int i = 0; i < scope_scene_model_name.Count; i++)
        {
            if (!init_scene_model_name.Contains(scope_scene_model_name[i]))
            {
                try
                {

                    Models[scope_scene_model_name[i]].BecomeHide();
                }
                catch (System.Exception)
                {
                    print(scope_scene_model_name[i] + "数据表缺失，或 公共库不存在 ");
                }
            }
        }
        SetCameraPosition(PublicClass.Transform_parent, camParentPos, dis); 
    }

    int init_model_cousor = 0;

    public IEnumerator init2(Action<bool> callBack, bool canFail = false)
    {
        if (PublicClass.get_server_interface == "http://118.24.119.234:8083/vesal-jiepao-test/server?type=0")
            canFail = false;
        yield return null;
        for (int i = init_model_cousor; i < scope_scene_model_name.Count; i++)
        {
            int j = -1;

            if (PublicClass.id_model_dic.TryGetValue(scope_scene_model_name[i], out j))
            {
                //GameObject temp = Instantiate(PublicClass.AllModels[j].gameObject, temp_parent.transform, true);
                Model temp_model = PublicClass.AllModels[j];
                temp_model.name = temp_model.name.Replace("(Clone)", "");
                if (Models.ContainsKey(temp_model.name))
                {
                    //Debug.LogError("重复：" + temp_model.name);
                    continue;
                }
// modelToRelation.Add(temp_model.name, new List<string>());
                Models.Add(temp_model.name, temp_model);
                scope_scene_model_position.Add(temp_model.name, temp_model.transform.position);
                temp_model.BecomeDisplay();
                if (ModelDisplayFormartDic.ContainsKey(temp_model.name))
                {
                    if (ModelDisplayFormartDic[temp_model.name] < -1.5f)
                    {
                        temp_model.BecomeTranslucent();
                    }
                    else if (Mathf.Abs( ModelDisplayFormartDic[temp_model.name]) < 0.1f )
                    {
                        temp_model.BecomeHide();
                    }
                    else
                    {
                        temp_model.BecomeNormal();
                    }

                }
                else
                {
                    temp_model.BecomeNormal();
                }
                init_model_cousor = i + 1;

            }
            else
            {
                if (!canFail)
                {
                    init_model_cousor = i;
                    Debug.LogError(init_model_cousor);
                    break;
                }
                else
                {
                    init_model_cousor = i + 1;
                }
            }
            //if (PublicClass.Quality == Run_Quality.GOOD)
            //{
                if (init_model_cousor % 25 == 0)
                {
                    yield return null;
                }
            //}
            //else
            //{
            //    if (init_model_cousor % 100 == 0)
            //    {
            //        yield return null;
            //    }
            //}
        }
        SetCameraPosition(PublicClass.Transform_parent, camParentPos, camParentRot, dis);
        Debug.Log("init_model_cousor " + init_model_cousor + " -- scope_scene_model_name.Count:" + scope_scene_model_name.Count);
        if (init_model_cousor < scope_scene_model_name.Count)
        {
            if (callBack != null)
            {
                callBack(false);
            }
        }
        else
        {
            //yield return null;
#if UNITY_EDITOR
            //查错
            for (int i = 0; i < scope_scene_model_name.Count; i++)
            {
                int j = -1;
                if (!PublicClass.id_model_dic.ContainsKey(scope_scene_model_name[i]))
                {
                    Debug.LogError(scope_scene_model_name[i]);
                }
            }
#endif
            if (callBack != null)
            {
                callBack(true);
            }
        }
    //}
    }

    public bool init2(bool canFail = false)
    {
        for (int i = init_model_cousor; i < scope_scene_model_name.Count; i++)
        {
            int j = -1;
            //Debug.LogError("PublicClass.id_model_dic:" + (PublicClass.id_model_dic == null));
            if (PublicClass.id_model_dic.TryGetValue(scope_scene_model_name[i], out j))
            {
                //GameObject temp = Instantiate(PublicClass.AllModels[j].gameObject, temp_parent.transform, true);
                Model temp_model = PublicClass.AllModels[j];
                temp_model.name = temp_model.name.Replace("(Clone)", "");
                if (Models.ContainsKey(temp_model.name))
                {
                    Debug.LogError("重复：" + temp_model.name);
                    continue;
                }
                //modelToRelation.Add(temp_model.name, new List<string>());
                Models.Add(temp_model.name, temp_model);
                scope_scene_model_position.Add(temp_model.name, temp_model.transform.position);
                temp_model.BecomeDisplay();
                temp_model.BecomeNormal();
                init_model_cousor = i + 1;
            }
            else
            {
                if (!canFail)
                {
                    init_model_cousor = i;
                    Debug.LogError(init_model_cousor+ scope_scene_model_name[i]);
                    break;
                }
                else
                {
                    init_model_cousor = i + 1;
                }
            }
        }
        DebugLog.DebugLogInfo("Models cousor:::::" + init_model_cousor + " model name" + scope_scene_model_name[Math.Max(1, init_model_cousor) - 1]);
        SetCameraPosition(PublicClass.Transform_parent, camParentPos, camParentRot, dis);
        Debug.Log("init_model_cousor " + init_model_cousor + " -- scope_scene_model_name.Count:" + scope_scene_model_name.Count);
        if (init_model_cousor < scope_scene_model_name.Count)
        {
            return false;
        }
        else
        {
            //yield return null;
#if UNITY_EDITOR
            string templist = "";
            //查错
            for (int i = 0; i < scope_scene_model_name.Count; i++)
            {
                int j = -1;
                if (!PublicClass.id_model_dic.ContainsKey(scope_scene_model_name[i]))
                {
                    templist += scope_scene_model_name[i]+",";
                    Debug.LogError(scope_scene_model_name[i]);
                }
            }
            vesal_log.vesal_write_log(templist);
#endif
            return true;
        }
    }

    public void init3(string nounNo)
    {
        InitDataByList_phase2();
        //InitTransparency(nounNo);
    }
    public IEnumerator init3(string nounNo, Action clallBack)
    {
        int count = 0;
        yield return null;
        //long useTime = PublicTools.GetTimeStamp();

        //定义关联模型
        //InitRelation();
        modelToRelation = null;
        InitRelation();
        
        //useTime = PublicTools.GetTimeStamp() - useTime;
        //Debug.Log("InitRelation:"+ (useTime));
        yield return null;
        current_scene_model_name.AddRange(init_scene_model_name);
        DebugLog.DebugLogInfo("当前场景模型长度 " + init_scene_model_name.Count);
        
        yield return null;
        //useTime = PublicTools.GetTimeStamp();
        //        if (scopeList != null)
        //        {
        //显示bc交集
        if (scope_nounNo != init_nounNo) {
            for (int i = 0; i < scope_scene_model_name.Count; i++)
            {
                if (!init_scene_model_name.Contains(scope_scene_model_name[i]))
                {
                    Models[scope_scene_model_name[i]].BecomeHide();
                }
                //count++;
                //if (count % 100 == 0)
                //{
                //    yield return null;
                //}
            }
        }

        //useTime = PublicTools.GetTimeStamp() - useTime;
        //Debug.Log("显示bc交集:" + (useTime));
        yield return null;
        //useTime = PublicTools.GetTimeStamp();
        //List<string> names = new List<string>();
        //Dictionary<string, float> tmpDic = getSubmodelDic(nounNo);
        //if (tmpDic != null)
        //{
        //    foreach (string key in tmpDic.Keys)
        //    {
        //        //names.Add(tmpModel.name);

        //        //if (init_scene_model_name.Contains(key))
        //        //{

        //        //}
        //        if (Models.ContainsKey(key))
        //        {
        //            setTrans(Models[key], tmpDic[key]);
        //        }
        //        else
        //        {
        //            Debug.Log("sub model "+key +"  is not found  in ab assets");
        //        }

        //        //count++;
        //        //if (count % 100 == 0)
        //        //{
        //        //    yield return null;
        //        //}
        //    }
        //}
        //useTime = PublicTools.GetTimeStamp() - useTime;
        //Debug.Log("set trans:" + (useTime));
        yield return null;
        //useTime = PublicTools.GetTimeStamp();
        //if (PublicClass.app.app_type == "acu")
        //{
        //    SetCameraPosition(PublicClass.Transform_parent, camParentPos, camParentRot, dis + 2);
        //}
        //else
        //    SetCameraPosition(PublicClass.Transform_parent, camParentPos, camParentRot, dis);


        //生成右侧菜单
        if (temp_rm_list != null && temp_rm_list.Length > 0)
        {
            InitSceneModelByNo(temp_rm_list);
            //InitSceneModelByNo(temp_rm_list);
        }
        //useTime = PublicTools.GetTimeStamp() - useTime;
        //Debug.Log("Rm:" + (useTime));
        clallBack();
    }








    bool InitData(string nounNo)
    {
        string[] rm_list = { };

        //if (GameObject.Find("temp_parent"))
        //{
        //    DestroyImmediate(GameObject.Find("temp_parent"));
        //}
        string nounName = string.Empty;
        var local_db = new DbRepository<GetStructList>();
        //获取名词范围
        local_db.DataService("vesali.db");
        GetStructList tmpIe = local_db.SelectOne<GetStructList>((tempNo) =>
        {
            if (tempNo.nounNo == nounNo)
            {
                return true;
            }
            else
            {
                return false;
            }

        });
        if (tmpIe == null)
        {
            local_db.Close();
            Debug.LogError("GetStructList: " + nounNo + "; null");
            return false;
        }
        scope_nounNo = tmpIe.collection_b;

        //根据范围生成右侧菜单
        local_db.DataService("vesali.db");
        GetStructList scope_Ie = local_db.SelectOne<GetStructList>((tempNo) =>
        {
            if (tempNo.nounNo == scope_nounNo)
            {
                return true;
            }
            else
            {
                return false;
            }

        });
        if (scope_Ie.rm_list != null && scope_Ie.rm_list != "")
        {
        rm_list = scope_Ie.rm_list.Split(',');
        }
        local_db.Close();
        if (nounNo == string.Empty)
            DebugLog.DebugLogInfo("资源id解析失败 " + nounName);

        old_submodel.AddRange(Readjson<List<fix_submodel>>(PublicClass.filePath + "/sign/old_sign_submodel_fix.json"));
        List<string> initList = getListByObj(tmpIe);//getListByNo(nounNo);
        ModelDisplayFormartDic = getSubmodelDicByObj(tmpIe);
        List<string> scopeList = getListByObj(scope_Ie);//getListByNo(scope_nounNo);
        InitDataByList(initList, scopeList, rm_list, tmpIe);
        //        InitTransparency(nounNo);
        //初始化submodel补充表
        return true;
    }
    public float dis;
    public Vector3 camParentPos = Vector3.zero;
    public Vector3 camParentRot = Vector3.zero;
    public static void SetCameraPosition()
    {
        SetCameraPosition(PublicClass.Transform_parent, instance.camParentPos, instance.dis);
    }

    public static void ResetDistance(Transform temp_parent)
    {
        Vector3 tempT = initGmPos(temp_parent);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        try { XT_MouseFollowRotation.instance.InitSlider(Interaction.instance.minDistance, Interaction.instance.maxDistance, Interaction.instance.fix_dis); }
        catch (Exception e) { }
#endif
        Interaction.instance.SetTarget(tempT, Interaction.instance.fix_dis);
        Interaction.instance.setParamValue();
    }

    /// <summary>
    /// 摄像机参数设定，
    /// </summary>
    /// <param name="temp_parent"></param>
    /// <param name="camParentPos"></param>
    /// <param name="camParentRot"></param>
    /// <param name="dis"></param>
    public static void SetCameraPosition(Transform temp_parent, Vector3 camParentPos, Vector3 camParentRot, float dis = 0)
    {
        if (PublicClass.app.app_type == "acu"&& dis!=0)
        {
             dis += 2;
        }
        print(camParentRot + "==================== ");
        //计算模型位置
        Vector3 tempT = Vector3.zero;
        if (camParentPos != Vector3.zero)
        {
            tempT = camParentPos;
        }
        else
        {
            tempT = initGmPos(temp_parent);
        }
        float distance = 0f;
        if (dis != 0f) //外部参数干预
        {
            distance = dis;
        }
        else //全自动定位，效果一般
        {
            distance = calculateFitDis(temp_parent.gameObject);
        }
        Interaction.instance.distance = distance;
        Interaction.instance.fix_dis = distance;
        Interaction.instance.maxDistance = 2 * distance - 0.5f;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        try { XT_MouseFollowRotation.instance.InitSlider(Interaction.instance.minDistance, Interaction.instance.maxDistance, distance); }
        catch (Exception e) { }
#endif
        Interaction.instance.SetTarget(tempT, distance);
        Interaction.instance.RotateOpera(camParentRot);
        Interaction.instance.setParamValue2(); 
    }


    /// <summary>
    /// dis =0  默认全自动，否则查表定位（可优化为 vector3 参数控制摄像机具体位置）
    /// </summary>
    public static void SetCameraPosition(Transform temp_parent, Vector3 camParentPos, float dis = 0)
    {
        print(camParentPos + "==================== ");
        Camera.main.transform.localPosition = new Vector3(0, 0, Camera.main.transform.localPosition.z);
        Camera.main.transform.parent.rotation = Quaternion.Euler(Vector3.zero);
        //计算模型位置
        Vector3 tempT = Vector3.zero;
        if (camParentPos != Vector3.zero)
        {
            tempT = camParentPos;
        }
        else
        {
            tempT = initGmPos(temp_parent);
        }
        float distance = 0f;
        // PublicData.dis =Mathf.Max( PublicData.dis ,minDistance);
        if (dis != 0f) //外部参数干预
        {
            distance = dis;
             DebugLog.DebugLogInfo("数据定位距离结果：" + distance);
        }
        else //全自动定位，效果一般
        {
            distance = calculateFitDis(temp_parent.gameObject);
            //            DebugLog.DebugLogInfo("自动定位----计算距离结果：" + distance);
        }
        Interaction.instance.distance = distance;
        if (Interaction.instance.fix_dis==0)
        {
            Interaction.instance.fix_dis = distance;
        }

        Interaction.instance.minDistance = distance /2.5f;
        Interaction.instance.maxDistance = 1.2f * distance ;

        // XT_MouseFollowRotation.instance.distanceControl.minValue = .5f;
        // XT_MouseFollowRotation.instance.distanceControl.maxValue = 2 * distance - XT_MouseFollowRotation.instance.minDistance;
        // XT_MouseFollowRotation.instance.distanceControl.value = distance;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        try { XT_MouseFollowRotation.instance.InitSlider(Interaction.instance.minDistance, Interaction.instance.maxDistance, distance); }
        catch (Exception e) { }
        
#endif
        Interaction.instance.SetTarget(tempT, Interaction.instance.fix_dis);
        Interaction.instance.setParamValue();
    }
    public void InitSceneModelByNo(string[] rmids)
    {

        if (rmm == null)
        {
            GameObject rmObj = GameObject.Find("RightMenuManager");
            rmm = null;
            if (rmObj != null)
            {
                rmm = rmObj.GetComponent<RightMenuManager>();
            }
        }

        if (rmm != null && rmids != null && rmids.Length > 0)
        {
            Debug.Log("rmm.InitRightMenuManager");
            rmm.InitRightMenuManager(rmids);
        }

    }
    //public IEnumerator InitSceneModelByNoAsync(string[] rmids)
    //{

    //    if (rmm == null)
    //    {
    //        GameObject rmObj = GameObject.Find("RightMenuManager");
    //        rmm = null;
    //        if (rmObj != null)
    //        {
    //            rmm = rmObj.GetComponent<RightMenuManager>();
    //        }
    //    }
    //    yield return null;
    //    if (rmm != null && rmids != null && rmids.Length > 0)
    //    {
    //        Debug.Log("rmm.InitRightMenuManager");
    //        StartCoroutine(rmm.InitRightMenuManagerAsync(rmids));
            
    //    }

    //}
    //依据Center参考大小与距离 ， 计算高亮物体最佳缩放距离
    public static float calculateFitDis(GameObject obj)
    {
        Bounds objBounds;
        Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
        objBounds = new Bounds(obj.transform.position, Vector3.zero);
        for (int i = 0; i < renders.Length; i++)
        {
            objBounds.Encapsulate(renders[i].bounds);
        }
        float length = Vector3.Distance(objBounds.max, objBounds.center);
        float Corners = GetCorners(length); //返回摄像机距离
        return Corners;
    }

    public static float GetCorners(float length, float param = 1.2f)
    {
        Transform tx = Camera.main.transform;
        Camera theCamera = Camera.main.GetComponent<Camera>();
        float halfFOV = (theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float height = length / Mathf.Tan(halfFOV);
        return height*param;
    }

    //计算模型的中心点
    static Vector3 initGmPos(Transform parent)
    {
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        if (renders.Length == 0)
        {
            List<Renderer> tmpList = new List<Renderer>();
            foreach (Model tmp in SceneModels.instance.Models.Values)
            {
                Renderer tmpRender = tmp.GetComponent<Renderer>();
                if (tmpRender != null)
                {
                    tmpList.Add(tmpRender);
                }
            }
            renders = tmpList.ToArray();
        }

        if (renders.Length == 0)
        {
            return Vector3.zero;
        }
        Vector3 center = Vector3.zero;
        for (int i = 0; i < renders.Length; i++)
        {
            center += renders[i].bounds.center;
        }
        center /= renders.Length;
        return center;
    }
    public Dictionary<string, float> getSubmodelDicByObj(GetStructList gsl)
    {
        Dictionary<string, float> result = null;
        for (int i = 0; i < old_submodel.Count; i++)
        {
            if (old_submodel[i].app_id==PublicClass.app.app_id)
            {
                result = old_submodel[i].submodel.ToObject<Dictionary<string, float>>();
                return result;
            }
        }
        try
        {
            result = JsonConvert.DeserializeObject<Dictionary<string, float>>(gsl.submodelList);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
        return result;
    }
    public Dictionary<string, float> getSubmodelDic(string nouNo)
    {
        var local_db = new DbRepository<GetStructList>();
        //获取名词范围
        local_db.DataService("vesali.db");
        GetStructList tmpSt = local_db.SelectOne<GetStructList>((tempNo) =>
        {
            if (tempNo.nounNo == nouNo)
            {
                return true;
            }
            else
            {
                return false;
            }

        });
        if (tmpSt == null)
        {
            local_db.Close();
            Debug.Log(nouNo + "; null");
            return null;
        }
        local_db.Close();

        return getSubmodelDicByObj(tmpSt);
    }
    public List<string> getListByObj(GetStructList gsl)
    {
        Dictionary<string, float> tmpDic = getSubmodelDicByObj(gsl);
        if (tmpDic != null)
        {
            List<string> temp = new List<string>(tmpDic.Keys);
            if (PublicClass.app.app_type == "sign_new" && !PublicClass.app.struct_name.Contains("骨学结构")&& !PublicClass.app.struct_name.Contains("骨性标志")
                && !PublicClass.app.app_id.Contains("SA090C001") && !PublicClass.app.app_id.Contains("SA090C002") && !PublicClass.app.app_id.Contains("SA090C003")
                && !PublicClass.app.app_id.Contains("SA090C004") && !PublicClass.app.app_id.Contains("SA090C005") && !PublicClass.app.app_id.Contains("SA0C03001"))
            {
                MainConfig.isLoadLocalSignNew = true;
                for (int i = 0; i < temp.Count; i++)
                {
                    temp[i] = "o_" + temp[i];
                }
            }
            else
            {
                MainConfig.isLoadLocalSignNew = false;
            }
            return temp;
        }
        return new List<string>();
    }
    //获取 SA 编号
    public List<string> getListByNo(string no)
    {

        Dictionary<string, float> tmpDic = getSubmodelDic(no);
        if (tmpDic != null)
        {
            return new List<string>(tmpDic.Keys);
        }
        return new List<string>();
    }
    public bool isParentActive()
    {
        return PublicClass.Transform_parent.gameObject.activeSelf;
    }
    public void closeTempParent()
    {
        //temp_parent.gameObject.SetActive(false);
        try
        {
            PublicClass.Transform_parent.gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void openTempParent()
    {
        try
        {
            PublicClass.Transform_parent.gameObject.SetActive(true);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        //temp_parent.gameObject.SetActive(true);
    }
    public void destoryTempParent()
    {
        try
        {
            //Transform tmp = new GameObject("tmp").transform;
            for (int i = 0; i < PublicClass.AllModels.Count; i++)
            {
                Model tmpModel = PublicClass.AllModels[i];
                tmpModel.BecomeNormal();
                tmpModel.BecomeHide();
                if (scope_scene_model_position.ContainsKey(tmpModel.name))
                {
                    tmpModel.transform.position = scope_scene_model_position[tmpModel.name];
                }
                string tmpName = tmpModel.name + "_clone";
                if (TransModelResetDic.ContainsKey(tmpName))
                {
                    DestroyImmediate(tmpModel.gameObject);
                    PublicClass.AllModels[i] = TransModelResetDic[tmpName];
                    PublicClass.AllModels[i].name = tmpName.Replace("_clone", "");
                    PublicClass.AllModels[i].BecomeNormal();
                    PublicClass.AllModels[i].BecomeHide();
                }
            }
            PublicClass.Transform_parent.gameObject.SetActive(false);
            PublicClass.Transform_temp.gameObject.SetActive(false);
            TransModelResetDic.Clear();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        //Destroy(temp_parent.gameObject);
    }
    /// <summary>
    /// 初始化模型关联关系
    /// </summary>
    //public void InitRelation()
    //{
    //    if (modelToRelation == null)
    //    {
    //        modelToRelation = new Dictionary<string, List<string>>();

    //    }
    //    else
    //    {
    //        return;
    //    }

    //    var local_db = new DbRepository<ModelRelationModel>();
    //    local_db.DataService("vesali.db");
    //    relationToModel.Clear();

    //    Dictionary<string, List<string>> tmpDic = new Dictionary<string, List<string>>();
    //    foreach (string tmpKey in modelToRelation.Keys)
    //    {
    //        IEnumerable<ModelRelationModel> mrms = local_db.Select<ModelRelationModel>((tempNo) =>
    //        {
    //            if (tempNo.name == tmpKey)
    //            {
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }

    //        });
    //        if (mrms != null && mrms.Count() != 0)
    //        {
    //            List<string> tmpList = new List<string>();
    //            foreach (ModelRelationModel tmpMrm in mrms)
    //            {
    //                tmpList.Add(tmpMrm.hide_id);
    //                if (!relationToModel.ContainsKey(tmpMrm.hide_id))
    //                {
    //                    relationToModel.Add(tmpMrm.hide_id, new List<Model>());
    //                }
    //            }
    //            tmpDic.Add(tmpKey, tmpList);
    //            //modelToRelation[tmpKey] = tmpList;
    //        }

    //    }
    //    modelToRelation = tmpDic;

    //    Dictionary<string, List<Model>> tmpRelationToModel = new Dictionary<string, List<Model>>();

    //    foreach (string tmpKey in relationToModel.Keys)
    //    {
    //        IEnumerable<ModelRelationModel> mrms = local_db.Select<ModelRelationModel>((tempNo) =>
    //        {
    //            if (tempNo.hide_id == tmpKey)
    //            {
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }

    //        });
    //        if (mrms != null && mrms.Count() != 0)
    //        {
    //            List<Model> tmpList = new List<Model>();
    //            foreach (ModelRelationModel tmpMrm in mrms)
    //            {
    //                if (Models.ContainsKey(tmpMrm.name))
    //                {
    //                    tmpList.Add(Models[tmpMrm.name]);
    //                }

    //            }
    //            if (tmpList.Count > 1)
    //            {
    //                tmpRelationToModel.Add(tmpKey, tmpList);
    //            }

    //        }
    //    }
    //    relationToModel = tmpRelationToModel;

    //    //Debug.Log(modelToRelation.Count);
    //    //Debug.Log(relationToModel.Count);
    //    local_db.Close();
    //}
    public void InitRelation()
    {
        if (modelToRelation == null)
        {
            modelToRelation = new Dictionary<string, List<Model>>();

        }
        else
        {
            return;
        }

        var local_db = new DbRepository<ModelRelationModel>();
        local_db.DataService("vesali.db");
        IEnumerable<ModelRelationModel> mrms = local_db.Select<ModelRelationModel>((tempNo) =>
        {
                return true;
        });
        Dictionary<string, List<string>> hidToName = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> NameToHid = new Dictionary<string, List<string>>();
        foreach (ModelRelationModel tmpMrm in mrms) {
            string tmpHid = tmpMrm.hide_id;
            string tmpName = tmpMrm.name;
            if (!hidToName.ContainsKey(tmpHid))
            {
                hidToName.Add(tmpHid, new List<string>());
            }
            hidToName[tmpHid].Add(tmpName);

            if (!NameToHid.ContainsKey(tmpName))
            {
                NameToHid.Add(tmpName, new List<string>());
            }
            NameToHid[tmpName].Add(tmpHid);
            
            //tmpMrm
        }
        foreach (string tmpKey in NameToHid.Keys) {
            foreach (string tmpHid in NameToHid[tmpKey]) {
                foreach (string tmpModelName in hidToName[tmpHid]) {
                    if (!modelToRelation.ContainsKey(tmpKey))
                    {
                        modelToRelation.Add(tmpKey, new List<Model>());
                    }
                    if (Models.ContainsKey(tmpModelName)) {
                        modelToRelation[tmpKey].Add(Models[tmpModelName]);
                    }
                    
                }
            }
        }
        local_db.Close();
    }
    //public IEnumerator InitRelationAsync()
    //{
    //    if (modelToRelation.Count != 0)
    //    {
    //        yield return null;
    //        var local_db = new DbRepository<ModelRelationModel>();
    //    local_db.DataService("vesali.db");
    //    //relationToModel.Clear();
    //        yield return null;
    //        Dictionary<string, List<string>> tmpDic = new Dictionary<string, List<string>>();
    //    foreach (string tmpKey in modelToRelation.Keys)
    //    {
    //            yield return null;
    //            IEnumerable<ModelRelationModel> mrms = local_db.Select<ModelRelationModel>((tempNo) =>
    //        {
    //            if (tempNo.name == tmpKey)
    //            {
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }

    //        });
    //        if (mrms != null && mrms.Count() != 0)
    //        {
    //            List<string> tmpList = new List<string>();
    //            foreach (ModelRelationModel tmpMrm in mrms)
    //            {
    //                tmpList.Add(tmpMrm.hide_id);
    //                if (!relationToModel.ContainsKey(tmpMrm.hide_id))
    //                {
    //                    relationToModel.Add(tmpMrm.hide_id, new List<Model>());
    //                }
    //            }
    //            tmpDic.Add(tmpKey, tmpList);
    //            //modelToRelation[tmpKey] = tmpList;
    //        }

    //    }
    //    modelToRelation = tmpDic;

    //    Dictionary<string, List<Model>> tmpRelationToModel = new Dictionary<string, List<Model>>();

    //    foreach (string tmpKey in relationToModel.Keys)
    //    {
    //            yield return null;
    //            IEnumerable<ModelRelationModel> mrms = local_db.Select<ModelRelationModel>((tempNo) =>
    //        {
    //            if (tempNo.hide_id == tmpKey)
    //            {
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }

    //        });
    //        if (mrms != null && mrms.Count() != 0)
    //        {
    //            List<Model> tmpList = new List<Model>();
    //            foreach (ModelRelationModel tmpMrm in mrms)
    //            {
    //                if (Models.ContainsKey(tmpMrm.name))
    //                {
    //                    tmpList.Add(Models[tmpMrm.name]);
    //                }

    //            }
    //            if (tmpList.Count > 1)
    //            {
    //                tmpRelationToModel.Add(tmpKey, tmpList);
    //            }

    //        }
    //    }
    //    relationToModel = tmpRelationToModel;

    //    //Debug.Log(modelToRelation.Count);
    //    //Debug.Log(relationToModel.Count);
    //    local_db.Close();
    //    }
    //}

    /// <summary>
    /// 初始化需要直接 半透明的模型
    /// </summary>
    public void InitTransparency(string nounNo)
    {
        List<string> names = new List<string>();
        Dictionary<string, float> tmpDic = getSubmodelDic(nounNo);
        if (tmpDic == null)
        {
            Debug.Log("InitTransparency null");
            return;
        }
        foreach (string key in tmpDic.Keys)
        {
            try
            {
                if (init_scene_model_name.Contains(key))
                {
                    setTrans(Models[key], tmpDic[key]);
                }
            }
            catch (Exception e)
            {
#if UNITY_EDTOR
                Debug.Log(init_scene_model_name.Contains(key));
                Debug.Log(Models.ContainsKey(key));
                Debug.Log(key);
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
#endif
            }
        }
    }
    public Dictionary<string, Model> TransModelResetDic = new Dictionary<string, Model>();
    public void setTrans(Model tmpModel, float tmpTransparency)
    {
        //Debug.Log("setTrans:" + tmpModel.name);
        if (TransModelResetDic != null)
        {
            if (!TransModelResetDic.ContainsKey(tmpModel.name))
            {
                Model cloneModel = GameObject.Instantiate(tmpModel, tmpModel.transform.parent);
                cloneModel.name = tmpModel.name + "_clone";
                cloneModel.BecomeHide();
                TransModelResetDic.Add(cloneModel.name, cloneModel);
            }
        }
        try
        {
            //Debug.Log(tmpModel.name +"---"+tmpTransparency);
            if (tmpTransparency == -2f)
            {
                tmpModel.BecomeDisplay();
                tmpModel.BecomeTranslucent();

            }
            else if (tmpTransparency >= 0f && tmpTransparency <= 1f)
            {
                tmpModel.BecomeDisplay();
                Color tmpColor = tmpModel.translucentMat.GetColor("_Color");
                tmpColor.a = tmpTransparency;
                tmpModel.translucentMat.SetColor("_Color", tmpColor);
            }
            else
            {
                tmpModel.BecomeDisplay();
                tmpModel.BecomeNormal();
            }
            //删除碰撞体，所有材质置为透明
            if (tmpTransparency == -2f || tmpTransparency >= 0f && tmpTransparency <= 1f)
            {
                MeshCollider mc = tmpModel.GetComponent<MeshCollider>();
                if (mc != null)
                {
                    mc.enabled = false;
                }
                tmpModel.normalMat = tmpModel.translucentMat;
                tmpModel.highlightMat = tmpModel.translucentMat;
            }
        }
        catch (Exception e)
        {
            tmpModel.BecomeDisplay();
            tmpModel.BecomeNormal();
            Debug.Log(e.Message);
        }
    }
    public Dictionary<string, Model> getModels()
    {
        return Models;
    }
    public Model[] Get_scope_models()
    {
        return Models.Values.ToArray();
    }

    public Model[] Get_init_models()
    {
        List<Model> init_models = new List<Model>();//[init_scene_model_name.Count];
        for (int i = 0; i < init_scene_model_name.Count; i++)
        {
            if (Models.ContainsKey(init_scene_model_name[i]))
            {
                init_models.Add( Models[init_scene_model_name[i]]);
                // Debug.Log("初始模型" + init_models[i]);
            }
        }
        return init_models.ToArray();
    }

    public Model[] Get_init_models_by_old()
    {
        List<Model> init_models = new List<Model>();//[init_scene_model_name.Count];
        for (int i = 0; i < init_scene_model_name.Count; i++)
        {
            if (Models.ContainsKey(init_scene_model_name[i]))
            {
                init_models.Add(Models[init_scene_model_name[i]]);
                // Debug.Log("初始模型" + init_models[i]);
            }
        }
        return init_models.ToArray();
    }

    public void InitOldAbModel()
    {

    }


    /// <summary>
    /// 旋转到固定角度
    /// </summary>
    /// <param name="temp_parent"></param>
    /// <param name="camParentPos"></param>
    /// <param name="dis"></param>
    public void SetCameraPosition(Transform temp_parent, Vector3 camPos, Vector3 camParentPos, Vector3 camParentRot)
    {
        Camera.main.transform.parent.position = camParentPos;
        iTween.RotateTo(Camera.main.transform.parent.gameObject, iTween.Hash("rotation", camParentRot, "time", 1f));
        iTween.MoveTo(Camera.main.transform.gameObject, iTween.Hash("position", camPos, "time", 1f));
        StartCoroutine(setpos(camParentRot.z));
    }

    public void SetSignPosition(Transform temp_parent, GetStructList tmpss)
    {
        Camera.main.transform.localPosition = PublicTools.Str2Vector3(tmpss.camPos);
        Camera.main.transform.parent.localPosition = PublicTools.Str2Vector3(tmpss.camParentPos);
        Camera.main.transform.parent.localRotation = Quaternion.Euler(PublicTools.Str2Vector3(tmpss.camParentRot));
        Interaction.instance.setParamValue2();
    }

    public void SetSignPosition(Transform temp_parent, Vector3 camPos, Vector3 camParentPos, Vector3 camParentRot)
    {
        Debug.Log(camPos.z);
        iTween.RotateTo(Camera.main.transform.parent.gameObject, iTween.Hash("rotation", camParentRot, "time", 1f));
        iTween.MoveTo(Camera.main.transform.parent.gameObject, iTween.Hash("position", camParentPos, "time", 1f));
        iTween.MoveTo(Camera.main.transform.gameObject, iTween.Hash("position", camPos, "time", 1f));
        StartCoroutine(setpos(camParentRot.z));
    }
    static IEnumerator setpos(float z)
    {
        yield return new WaitForSeconds(1);
        if (z < 90 && z > -90)
            Interaction.instance.setParamValue2();
        else
            Interaction.instance.setParamValue3();
    }

    public T Readjson<T>(string path) where T : new()
    {
        string jsonfile = path;
        T instance = new T();
        using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o = (JObject)JToken.ReadFrom(reader);
                Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                if (r.ContainsKey("app_id_submodel"))
                {
                    instance = JsonConvert.DeserializeObject<T>(r["app_id_submodel"].ToString());
                }
            }
        }
        return instance;
    }
}