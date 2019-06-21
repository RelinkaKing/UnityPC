using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
public class BaseCommand : object
{
    //执行命令  
    public virtual void ExecuteCommand() { }

    //撤销命令  
    public virtual void RevocationCommand() { }
}

[Serializable]
//场景模型状态记录
public class SceneModelState
{

    //模型状态表
    public Dictionary<string, modelState> SceneModelStateDict;

    //初始化对指定模型进行状态记录
    public SceneModelState()
    {
        if (SceneModels.instance == null)
        {
            return;
        }
        SceneModelStateDict = new Dictionary<string, modelState>();
        Model[] tempAllmodel = SceneModels.instance.Get_scope_models();
        for (int i = 0; i < tempAllmodel.Length; i++)
        {
            modelState tempState = new modelState();
            tempState.isActive = tempAllmodel[i].isActive;
            tempState.isFade = tempAllmodel[i].isTranslucent;
            tempState.isSeleted = tempAllmodel[i].isSeleted;
            tempState.position = new vector3
            {
                x = tempAllmodel[i].transform.position.x,
                y = tempAllmodel[i].transform.position.y,
                z = tempAllmodel[i].transform.position.z
            };
            SceneModelStateDict.Add(tempAllmodel[i].name, tempState);
        }
    }
}

[Serializable]
public class modelState
{
    public bool isFade; //隐藏
    public bool isActive; //显示
    public bool isSeleted; //选定高亮
    public vector3 position; //位置
}

[Serializable]
public class CameraParams
{
    public vector3 rotateAxis;
    public float positionx;
    public float positiony;
    public float distance;
    public vector3 rotation;

    public CameraParams()
    {
        if (Interaction.instance == null)
        {
            return;
        }
        positionx = Interaction.instance.gameObject.transform.localPosition.x;
        positiony = Interaction.instance.gameObject.transform.localPosition.y;
        distance = Interaction.instance.distance;
        rotation = new vector3
        {
            x = Interaction.instance.y,
            y = Interaction.instance.x,
            z = 0,
        };
        rotateAxis = new vector3
        {
            x = Interaction.instance.rotateAxis.transform.localPosition.x,
            y = Interaction.instance.rotateAxis.transform.localPosition.y,
            z = Interaction.instance.rotateAxis.transform.localPosition.z,
        };
        //PublicClass.OpenPreRecordState = false;
        // Debug.Log("书签摄像机参数："+positionx+" "+positiony+" "+Distance+" "+rotation.x+" "+rotation.y+" "+EmptyPosition.x+" "+EmptyPosition.y+" "+EmptyPosition.z);
    }
}

[Serializable]
public struct vector3
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
//场景按钮标识位
public class SceneBtnState
{
    //状态位

    public bool isMultiBtnUI = false;
    public bool isSplitBtnUI = false;
    public bool isTranUI = false;
    public bool isTranOtherUI = false;
    public bool isHideUI = false;
    public bool isHideOtherUI = false;
    public bool isExplainBtnSwitch = true;
    public bool isLittleMapSwitch = true;
    public bool isExplainChange = false; //解释文字是否变化
    public bool isExplainShow = false;
    public string chinese = string.Empty;
    public string english = string.Empty;
    public string note = string.Empty;

    public Dictionary<int, bool> rightIndexUIFlags;
    public Dictionary<int, int[,]> rightIndexUIIndexs;

    //定义数据存储格式，
    public SceneBtnState()
    {
        if (SceneModels.instance == null)
        {
            return;
        }
        isMultiBtnUI = SceneModels.instance.get_Multi_Selection();
        isSplitBtnUI = SceneModels.instance.get_Split_mode();
        isTranUI = SceneModels.instance.get_Tran_State();
        isTranOtherUI = SceneModels.instance.get_TranOther_State();
        isHideUI = SceneModels.instance.get_Hide_State();
        isHideOtherUI = SceneModels.instance.get_HideOther_State();

        // UIBtnFlag.UpdateUIState();
        // rightIndexUIIndexs=new Dictionary<int,int[,]>();
        // rightIndexUIFlags=new Dictionary<int,bool>();
        // List<int> typeAIndexs=new List<int>();
        // typeAIndexs.AddRange(UIBtnFlag.rightIndexUIFlags.Keys);
        // List<int> typeBIndexs=new List<int>();
        // typeBIndexs.AddRange(UIBtnFlag.rightIndexUIIndexs.Keys);

        // for (int i = 0; i < typeBIndexs.Count; i++)
        // {
        //     rightIndexUIIndexs.Add(typeBIndexs[i],UIBtnFlag.rightIndexUIIndexs[typeBIndexs[i]]);
        //     // Debug.Log("右侧分层"+typeBIndexs[i]+" 状态 "+rightIndexUIIndexs[typeBIndexs[i]].GetLength(0)+"  "+rightIndexUIIndexs[typeBIndexs[i]].GetLength(1));
        // }
        // for (int i = 0; i < typeAIndexs.Count; i++)
        // {
        //     rightIndexUIFlags.Add(typeAIndexs[i],UIBtnFlag.rightIndexUIFlags[typeAIndexs[i]]);
        //     // Debug.Log("右侧开关"+typeAIndexs[i]+" 状态 "+rightIndexUIFlags[typeAIndexs[i]]);
        // }
        // isLittleMapSwitch=UIBtnFlag.isLittleMapSwitch;
        // isExplainBtnSwitch=UIBtnFlag.isExplainBtnSwitch;
        // MultiBtnUIFlag=UIBtnFlag.isSwitchMultiBtn;
        // isExplainChange = UIBtnFlag.isExplainChange;
        // isExplainShow = UIBtnFlag.isExplainShow;
        // if(isExplainShow)
        // {
        //     chinese=UIBtnFlag.chinese;
        //     english=UIBtnFlag.english;
        //     note=UIBtnFlag.note;
        // }
        // Debug.Log("多选状态："+MultiBtnUIFlag+" 解释变化 "+ isExplainChange  + " 解释框显示 "+ isExplainShow+ " 详细信息 "+ isExplainBtnSwitch);
        // Debug.Log("__________________________________________________________________");
    }
}

//回退ui堆栈记录的临时全局标识位
public class UIBtnFlag
{
    //初始化静态标志位
    public static bool isSwitchMultiBtn = false; //是否多选
    public static bool isExplainBtnSwitch = true; //解释框是否移动
    public static bool isExplainShow = false; //解释框是否打开
    public static bool isExplainChange = false; //解释文字是否变化
    public static string chinese = string.Empty;
    public static string english = string.Empty;
    public static string note = string.Empty;
    public static bool isLittleMapSwitch = true;

    public static Dictionary<int, bool> rightIndexUIFlags;
    public static Dictionary<int, int[,]> rightIndexUIIndexs;

    public static void InitUIFlag()
    {
        // Debug.Log("初始化ui属性");
        isSwitchMultiBtn = false; //是否多选
        isExplainBtnSwitch = true; //解释框是否移动
        isLittleMapSwitch = true;
        isExplainShow = false; //解释框是否打开
        isExplainChange = false; //解释文字是否变化
        chinese = string.Empty;
        english = string.Empty;
        note = string.Empty;
    }

    //右侧按钮记录初始化
    public static void InitRightMenuFlag()
    {
        Debug.Log("InitRightMenuFlag");
        rightIndexUIIndexs = new Dictionary<int, int[,]>();
        rightIndexUIFlags = new Dictionary<int, bool>();
        //for (int i = 0; i < CommandManager.instance.LayerButton.Count; i++)
        //{
        //    rightIndexUIIndexs.Add(CommandManager.instance.LayerButton[i].index,new int [1,0]);
        //}
        //for (int i = 0; i < CommandManager.instance.ChangeUIBtn.Count; i++)
        //{
        //    rightIndexUIFlags.Add(CommandManager.instance.ChangeUIBtn[i].index, true);   
        //}
    }

    //用于初始化和获取当前ui状态
    public static void UpdateUIState()
    {

        // isSwitchMultiBtn =PublicClass.isMultiple;
        // isExplainBtnSwitch=!XT_AllButton.Instance.logo.activeSelf;
        // isLittleMapSwitch=XT_AllButton.Instance.litteMapIsOpen;  
        // isExplainShow=XT_AllButton.Instance.ExplainPanel.activeSelf;
        // rightIndexUIIndexs=new Dictionary<int,int[,]>();
        // rightIndexUIFlags=new Dictionary<int,bool>();
        // //动态获取场景信息
        // for (int i = 0; i < CommandManager.instance.LayerButton.Count; i++)
        // {
        //     int boolLayer=CommandManager.instance.LayerButton[i].addOrRemove?1:0;
        //     // Debug.Log(CommandManager.instance.LayerButton[i].typrBlayer+" "+boolLayer);
        //     // Debug.Log(i+" "+CommandManager.instance.LayerButton[i].index);
        //     rightIndexUIIndexs.Add(CommandManager.instance.LayerButton[i].index,new int[CommandManager.instance.LayerButton[i].typrBlayer,boolLayer]);
        // }

        // for (int i = 0; i < CommandManager.instance.ChangeUIBtn.Count; i++)
        // {
        //     if(CommandManager.instance.ChangeUIBtn[i].Isclose)
        //     {
        //         rightIndexUIFlags.Add(CommandManager.instance.ChangeUIBtn[i].index,false);
        //     }
        //     else
        //     {
        //         rightIndexUIFlags.Add(CommandManager.instance.ChangeUIBtn[i].index,true);           
        //     }
        // }
    }
}