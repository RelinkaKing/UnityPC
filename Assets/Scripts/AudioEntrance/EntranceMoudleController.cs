using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using Assets.Scripts.Repository;

public class EntranceMoudleController : MonoBehaviour {
	DbRepository<GetStructList> local_db=null;
	GameObject moudelUI;
	bool open=false;

	
	void Start()
	{
		moudelUI=GameObject.Find("EntranceMoudle");
		DebugLog.DebugLogInfo("开启语音名词库");
		local_db=new DbRepository<GetStructList>();
	}

	public void Show_audio_input()
	{
		//语音输入，生成名词列表，点击显示局部模型
		//语音结果显示
		
	}


	public void SelectStructList()
	{
		//连接本地数据库
        var local_db=new DbRepository<GetStructList>();
        local_db.DataService("vesali.db");
        IEnumerable<GetStructList> dbs=local_db.Select<GetStructList>((tempt)=>{
			if(tempt.nounNo==null)
			{
				DebugLog.DebugLogInfo("数据异常，或表为空");
				return false;
			}
			else
			{
				return true;
			}
        });

		GetStructList one_struct=local_db.SelectOne<GetStructList>((Struct)=>{
			if (Struct.nounName == typeof(GetStructList).Name)
			{
				return true;
			}
			return false;
		});
	}
	
	public void Fuzzy_query(string struct_name)
	{
		var local_db=new DbRepository<GetStructList>();
        local_db.DataService("vesali.db");
		IEnumerable<GetStructList> dbs=local_db.Select<GetStructList>((tempt)=>{
			if(tempt.nounNo==null)
			{
				DebugLog.DebugLogInfo("数据异常，或表为空");
				return false;
			}
			else
			{
				return true;
			}
        });
	}

	public void SwitchMoudle()
	{
		open=!open;
		this.gameObject.SetActive (open);
	}
}
