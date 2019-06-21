using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VesalCommon;

public class CommandManager : MonoBehaviour, ICommandMannager
{
    public static CommandManager instance;
    //public XT_MouseFollowRotation MouseFollowRotation;

    int indexId;
    public Button Backbtn;
    DbRepository<CommandList> local_db;
    public bool CloseCommandCompent = false;

    private void Awake()
    {
        instance = this;

    }

    void Start()
    {
        if (CloseCommandCompent) return;
        local_db = new DbRepository<CommandList>();
        //获取名词范围
        local_db.DataService("Command.db");
        local_db.DropTable();
        local_db.CreateTable();
        local_db.Close();
        indexId = 0;
    }

    /// <summary>  
    /// 执行新的命令  
    /// </summary>  
    public void ExecutiveCommand(BaseCommand command)
    {
        if (CloseCommandCompent) return;
        indexId++;
        CommandList db_command = new CommandList
        {
            id = indexId,
            modelInfo = Vesal_DirFiles.Object2Bytes(new SceneModelState()),
        };
        local_db.DataService("Command.db");
        local_db.Insert(db_command);
        local_db.Close();
        DebugLog.DebugLogInfo("插入命令 " + indexId);
        command.ExecuteCommand();
        this.CanOperaCommand(true);
    }

    public List<string> IdList = new List<string>();

    public void RemoveLastString()
    {
        try
        {
            string popid = IdList[IdList.Count - 1];
            IdList.Remove(popid);
        }
        catch (System.Exception)
        {
            throw;
        }

    }

    /// <summary>  
    /// 撤销上一个命令  
    /// </summary>  
    public void RevocationCommand()
    {
        if (CloseCommandCompent) return;
        if (indexId >= 0)
        {
            local_db.DataService("Command.db");

            CommandList popDocument = local_db.SelectOne<CommandList>((temp_command) =>
            {
                if (indexId == temp_command.id)
                {
                    DebugLog.DebugLogInfo("查询数据：" + indexId);
                    return true;
                }
                else
                    return false;
            });

            try
            {
                SceneModelState modelstaste = (SceneModelState)Vesal_DirFiles.Bytes2Object(popDocument.modelInfo);
                PlayerCommand command = new PlayerCommand(modelstaste);
                command.RevocationCommand();
            }
            catch
            {
                // Debug.LogError("命令记录不存在 id :"+id);
            }
            DebugLog.DebugLogInfo("删除数据：" + indexId);
            local_db.Delete(popDocument);
            local_db.Close();
            indexId--;
            this.CanOperaCommand(indexId != 0);
        }
    }

    //控制回退按钮的开关
    public void CanOperaCommand(bool val)
    {
        if (CloseCommandCompent) return;
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if(Backbtn!=null)
        Backbtn.gameObject.SetActive(val);
        #else
        Backbtn.interactable = val;
        #endif
        if (!val)
        {
            Debug.Log("命令栈清空");
            ClearRecordStack();
        }
    }

    //记录场景模型状态
    public void RecordModelStata()
    {
        if (CloseCommandCompent) return;
        BaseCommand command = new PlayerCommand();
        ExecutiveCommand(command);
    }
    public void ClearRecordStack()
    {
        if (CloseCommandCompent) return;
        local_db.DataService("Command.db");

        local_db.DropTable();
        local_db.CreateTable();
        local_db.Close();
        indexId = 0; //初始化记录id
    }
    private void OnDestroy()
    {
        if (CloseCommandCompent) return;
        //local_db.Close();
    }

    private void OnApplicationQuit()
    {
        if (CloseCommandCompent) return;
        //local_db.Close();
    }

    public string LastId
    {
        get
        {
            string popid = IdList[IdList.Count - 1];
            lastid = popid;
            return popid;
        }
        set
        {
            IdList.Add(value);
            lastid = value;
        }
    }

    private string lastid;

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
}

public interface ICommandMannager
{
    void ExecutiveCommand(BaseCommand command);
    void RevocationCommand();
}