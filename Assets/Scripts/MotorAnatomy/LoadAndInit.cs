using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using VesalCommon;
using XLua;

[Hotfix]
public class LoadAndInit : MonoBehaviour
{
    public TimelineControll timelineControll;
    public showMode showMode;
    GameObject GO;
    public string fa;
    public Transform MainCamera;
    public CameraCtl cameraCtl;
    public showMessage showMessage;
    public string timelinePath;
    public testPlayLoad testPlayLoad;
    bool isAutoRotation;
    [LuaCallCSharp]
    public void Start()
    {
        if (Screen.orientation == ScreenOrientation.AutoRotation)
        {
            isAutoRotation = true;
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }
    string db_path = @"C:\Users\Administrator\Desktop\vesali.db";
    [LuaCallCSharp]
    public void Init(string db_path, GameObject model, string timeLine_path)
    {

        QualitySettings.blendWeights = BlendWeights.FourBones;
        GO = model;
        this.timelinePath = timeLine_path;
        AssetBundle ab = AssetBundle.LoadFromFile(timeLine_path);
        PlayableAsset wallPrefab = ab.LoadAsset<PlayableAsset>(Vesal_DirFiles.get_file_name_from_full_path(timelinePath).Split('_')[0]);
        PlayableDirector pad = model.GetComponent<PlayableDirector>();
        pad.playableAsset = wallPrefab;

        var timeline = pad.playableAsset as TimelineAsset;
        foreach (var track in timeline.GetOutputTracks())
        {
            Debug.Log(track.name);
        }
        pad.SetGenericBinding(timeline.GetOutputTrack(0), GO);

        this.db_path = db_path;
        timelineControll.timeLine = pad;
        showMode.model = model;
        // showMode.Init();

        ab.Unload(false);
        loaddb();
    }

    string[] muscleEng;
    string[] muscleCng;

    DbRepository<MotorAnatomy> local_db;
    DbRepository<MotorAnatomy_submodel> local_db2;
    [LuaCallCSharp]
    public void loaddb()
    {
        local_db = new DbRepository<MotorAnatomy>();
        local_db.DataService(db_path);
        string id = PublicClass.app.app_id;
        var tmpssp = local_db.SelectOne<MotorAnatomy>((tmp) =>
        {
            if (tmp.ID == id) 
            {
                //DebugLog.DebugLogInfo(ss_id.bone_name+" "+ss_id.main_tex+" "+ ss_id.mask_tex);
                return true;
            }
            else
            {
                return false;
            }

        });

        local_db.Close();
        Debug.Log(tmpssp.MuscleName);
        string[] modelname = tmpssp.MuscleName.Split(',');
        List<System.String> listS = new List<System.String>(modelname);
        local_db2 = new DbRepository<MotorAnatomy_submodel>();
        local_db2.DataService(db_path);
        var tmpssp2 = local_db2.Select<MotorAnatomy_submodel>((tmp) =>
        {
            if (listS.Exists(p => p == tmp.ModelName))
            {
                //DebugLog.DebugLogInfo(ss_id.bone_name+" "+ss_id.main_tex+" "+ ss_id.mask_tex);
                return true;
            }
            else
            {
                return false;
            }


        });

        local_db.Close();
        List<MotorAnatomy_submodel> texInfoList = new List<MotorAnatomy_submodel>();

        foreach (MotorAnatomy_submodel tmp in tmpssp2)
        {
            texInfoList.Add(tmp);
        }
        int num = texInfoList.Count;
        muscleEng = new string[num];
        muscleCng = new string[num];

        for (int i = 0; i < num; i++)
        {
            muscleEng[i] = texInfoList[i].ModelName;
            muscleCng[i] = texInfoList[i].ChineseName;
        }


        InitModel(tmpssp);
        InitTimeLine(tmpssp);
        InitPos(tmpssp);
        InitText(tmpssp);
        try
        {
            testPlayLoad.Close();
        }
        catch (System.Exception)
        {
        }
    }

    public void IsAutoRotation()
    {
        if (isAutoRotation)
            Screen.orientation = ScreenOrientation.AutoRotation;
    }
    [LuaCallCSharp]
    private void InitModel(MotorAnatomy tmpssp)
    {
        showMode.fa = fa;
        showMode.showname = tmpssp.ModelName;
        showMode.activeMuscleName = tmpssp.MainMuscleName;
        showMode.secondaryMuscleName = tmpssp.SecondaryMuscleName;
        showMode.MuscleCngName = muscleCng;
        showMode.MuscleEngName = muscleEng;
        showMode.muscleTex = GO.GetComponent<TextureData>().textures;
        showMode.Init();
    }
    [LuaCallCSharp]
    private void InitTimeLine(MotorAnatomy tmpssp)
    {
        timelineControll.stageFrame = int.Parse(tmpssp.StageFrame);
        timelineControll.Init();
    }
    [LuaCallCSharp]
    private void InitPos(MotorAnatomy tmpssp)
    {
        string[] POS = tmpssp.ModelPosition.Split(',');
        GO.transform.localPosition = new Vector3(float.Parse(POS[0]), float.Parse(POS[1]), 0);
        MainCamera.localPosition = new Vector3(0, 0, -float.Parse(POS[2]));
        cameraCtl.Init();
    }
    [LuaCallCSharp]
    private void InitText(MotorAnatomy tmpssp)
    {
        showMessage.msg = tmpssp.Details;
        showMessage.JointName = tmpssp.JointName;
        showMessage.MaxAngle = tmpssp.MaxAngle;
        showMessage.SportsName = tmpssp.NameCng;
        showMessage.init();
    }
}

