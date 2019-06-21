using UnityEngine;
using System.Collections;
//using static PublicClass;
using System.Collections.Generic;

public class CachStateRecord : MonoBehaviour
{

    public static CachStateRecord instance;
    //使用堆栈进行数据结构缓存
    Stack StateStruct = new Stack();

    public List<Model> RecordModelArray = new List<Model>();
    //public static event BackState BackRequestEvent;

    OperatState currentState;                       //当前状态记录

    void Awake()
    {
        instance = this;
    }

    //模型状态，右侧菜单状态，解释框状态
    struct OperatState
    {
        public Model StateModel;
        public OperaIndex CurrentOpera;
    }

    //记录状态步骤
    public void RecordState(OperaIndex currentIndex)
    {
        // if (PublicClass.currentModel == null)
        //     DebugLog.DebugLogInfo("开始记录。。。");
        // else
        // {
        //     //当前选定模型不为空时，开始记录第一条数据
        //     currentState = new OperatState();
        //     DebugLog .DebugLogInfo ("开始生成一条新记录");
        //     currentState.StateModel = PublicClass.currentModel;
        //     currentState.CurrentOpera = currentIndex;                           //记录状态标识未
        //     DebugLog.DebugLogInfo("正在记录模型"+ currentState.StateModel.name);
        //     //RecordModelArray.Add(currentState.StateModel);                  //检视面板显示
        //     DebugLog.DebugLogInfo("记录模型操作记录");
        //     //开启按钮监听功能，按钮按下，发对应事件，记录事件，回退时，进行解析还原操作

        // }        
    }

    //完成记录，压入栈中
    public void EndState()
    {
        if (StateStruct.Contains(currentState))
            Debug.LogError("stack record repeat");
        StateStruct.Push(currentState);
        DebugLog.DebugLogInfo("完成上次选中模型记录");
    }

    //返回上一步，
    public void BackLastStep()
    {
        //强制结束当前记录
        EndState();
        //对栈进行弹出操作
    }

    //单多选切换时，清空选中模型
    public void RevertModelChooseToStart()
    {
        //单选，清空当前选中，上一个选中，

    }
    private void OnDestroy()
    {
        Destroy(this);
    }
}

//回退表示操作状态
public enum OperaIndex
{
    None = 0,

    //模型操作操作
    Hide = 1,
    Transparent = 2,
    HideOther = 3,
    TransparentOther = 4,

    //模型状态
    ChangeModel,              //高亮状态
    Display,
    Normal,

    //选择模式操作
    Simple_BackLast,
    Mutil_BackLast,

    //单击和双击
    Simple_click,
    Mutil_click
}