using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Scripts.Model;

public class CreateDB : MonoBehaviour {

	public static void Sign_new_db_info()
	{
		string db_path=PublicClass.filePath+"sign_ssp_path/SignNew_1.db";
		MakeStaticDBTable<SignNewInfo>.sign_UpdateTable(PublicClass.server_ip + "v1/app/xml/getTempSignNew",db_path,false);
	}
	public static void Append_Sign_new_db_info()
	{
		string db_path=PublicClass.filePath+"sign_ssp_path/SignNew_1.db";
		MakeStaticDBTable<SignNewInfo>.sign_UpdateTable(PublicClass.server_ip + "v1/app/xml/getTempSignNew",db_path,true);
	}


	public static void Start_Stop_db_info()
	{
		string db_path=PublicClass.filePath+"sign_ssp_path/StartStop_1.db";
		//接口暂无
		//http://118.24.119.234:8083/vesal-jiepao-test/v1/app/xml/getAllStartStop
		MakeStaticDBTable<SignNewInfo>.sign_UpdateTable(PublicClass.server_ip + "v1/app/xml/getAllStartStop",db_path,false);
	}
	public static void Append_Start_Stop_db_info()
	{
		string db_path=PublicClass.filePath+"sign_ssp_path/StartStop_1.db";
		//接口暂无
		MakeStaticDBTable<SignNewInfo>.sign_UpdateTable(PublicClass.server_ip + "v1/app/xml/getAllStartStop",db_path,true);
	}

	[MenuItem("DB/SetTexture/Sign_new_info")]
	public static void SetTextureToDB_sign_new()
	{
		string db_path=PublicClass.filePath+"sign_ssp_path/SignNew_1.db";
		MakeStaticDBTable<texture_mask>.SetTextureDataToDB(PublicClass.filePath+"StartStop",db_path);
	}

	[MenuItem("DB/SetTexture/Start_stop_info")]
	public static void SetTextureToDB_Start_stop()
	{
		Inint();
		string db_path=PublicClass.filePath+"sign_ssp_path/StartStop_1.db";
		MakeStaticDBTable<texture_mask>.SetTextureDataToDB(PublicClass.filePath+"StartStop",db_path);
	}

	public static void CaculateTextureCout()
	{
		Inint();
		string db_path=PublicClass.filePath+"sign_ssp_path/StartStop_1.db";
	}

	static void Inint()
	{
		PublicClass.InitStaticData();
	}

	public static void Opera_db()
	{
		string db_path=PublicClass.filePath+"sign_ssp_path/StartStop_1.db";
		MakeStaticDBTable<StartStopInfo>.opera_condition(db_path,(list)=>{
			//list startstopinfo
			List<string> opera_list=new List<string>();
			foreach (var item in list)
			{
				if(item.qr==null && item.zr==null && item.start_desc==null&& item.stop_desc==null)
				{
					opera_list.Add(item.mod_name);
					print(item.mod_name+" info missing ");
				}
			}
		});
	}

}
