﻿using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


enum FingerTouchState
{
    NonFingerTouch,
    OneFingerTouch,
    TwoFingerTouch
}




public class XT_MouseFollowRotation : MonoBehaviour
{
    //记录手指状态
    FingerTouchState FingerState = FingerTouchState.NonFingerTouch;
    //获取鼠标/键盘的操纵状态
    TouchCtrl touchCtrl;

    //判断是否隐藏模型；
    bool isHideMode = false;

    //单例
    public static XT_MouseFollowRotation instance = new XT_MouseFollowRotation();
    [Header("CameraParms")]
    //缩放系数
    public float distance = 12;
    //public float resetDistance = 12;
    //public float minDistance;//缩放限制
    //public float maxDistance;
    //public float sign_distance;     //图钉模式摄像机距离
    //public float distanceSpeed;
    public float x, x1; //摄像头位置
    public float y, y1, z;
    public Vector3 emptyPosition;
    //public float RotationY;                //空物体参数  图谱模式中
    //public float RotationX;
    //public float RotationZ;
    public bool IsRotate = false;
    // public float translation = 0.1f;//0.00052f * 100;

    public GameObject emptyBox;     //旋转轴
                                    //    // public GameObject ModelPosition;
                                    //     public float positionX;
                                    //     public float positionY;
                                    //     public float positionZ;
                                    // float sign_positionY;           //图钉模式空物体  Y 位置
    float xSpeed = 7f; //滑动系数
    float ySpeed = 3f;
    //记录上一次手指触摸位置判断用户时放大还是缩小手势
    Vector2 oldPosition1;
    Vector2 oldPosition2;
    //Vector3 oldPosition;
    //Vector3 direction;
    float zoom = 0.070f;//0.068f;
    public bool isAutoRotation = false;


    //记录上一次的位置
    public List<Vector3> lastTransPos = new List<Vector3>();
    //public List<Vector3> currentTransPos = new List<Vector3>();

    public Camera uicam;
    //RuntimePlatform platform;

    // [Header("TextInfo")]
    // public Text testInputText;
    // public Text TopNameLabel;
    // public Text SearchTopLabelName;
    // public Text SearchTopLabel;


    // public struct cameraParms
    // {
    //     public float Distance;
    //     public float MisDis;
    //     public float MaxDis;
    //     public float ResetDis;
    // }
    // //图钉模式摄像机参数
    // public cameraParms signCameraParms = new cameraParms();
    // //图谱模式摄像机参数
    // public cameraParms mapCameraParms = new cameraParms();

    private void Awake()
    {
        touchCtrl = GameObject.Find("TouchCtrl").GetComponent<TouchCtrl>();
        instance = this;
        //StartCoroutine(StartParamsSet ());
        PublicClass.currentState = RunState.Playing;

    }
    bool isWeiKe = false;
    private void Start()
    {

        dramline.instance.DontRoate += DontRotate;              //添加禁止旋转事件
        if (SceneManager.GetActiveScene().name == "WeiKePlayer")
        {
            isWeiKe = true;
        }
        else
        {
            isWeiKe = false;
        }
        //  EnterMapMode();
        //记录初始位置
        // RecordCameraParams();
    }


    /// <summary>
    /// 自动移动到目标位置（设定位置旋转）
    /// </summary>
    /// <param name="go"></param>
    public void SetTarget(GameObject go)
    {
        emptyPosition = go.transform.position;
        iTween.MoveTo(emptyBox.gameObject, iTween.Hash("position", emptyPosition, "time", 1.5f));
        SetCameraBack(0, 0, distance);
    }
    public void SetTarget(Vector3 position, float dis)
    {
        // DebugLog.DebugLogInfo(position);
        x = 0;
        y = 0;
        z = 0;
        distance = dis;
        emptyPosition = position;
        emptyBox.transform.position = position;
        //iTween.MoveTo(emptyBox.gameObject, iTween.Hash("position", position, "time", 1.5f));
        // iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(0, 0, dis), "time", 1.5f, "easytype", EaseType.linear, "islocal", true));
        SetCameraBack(0, 0, distance);
        setParamValue();
    }

    public void SetCameraBack(float px, float py, float pz)
    {
        x1 = px;
        y1 = py;
        transform.localPosition = new Vector3(x1, y1, distance);
        //iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(x1, y1, distance), "time", 1.5f, "easytype", EaseType.linear, "islocal", true));
    }

    public void SetCameraDistance(float dis)
    {
        distance = dis;
        iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(x1, y1, dis), "time", 1.5f, "easytype", EaseType.linear, "islocal", true));
    }

    public void SetCameraBackRotation(float rx, float ry, float rz)
    {
        y = rx;
        x = ry;
        iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(rx, ry, rz), "time", 1));
    }

    public void SetCameraParams(float px, float py, float dis, float rx, float ry, float ex, float ey, float ez)
    {
        Debug.Log("设定书签摄像机参数：" + px + " " + py + " " + dis + " " + rx + " " + ry + " " + ex + " " + ey + " " + ez);
        emptyPosition = new Vector3(ex, ey, ez);
        x1 = px;
        y1 = py;
        y = rx;
        x = ry;
        distance = dis;
        iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(rx, ry, 0), "time", 1));
        iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(x1, y1, distance), "time", 1.5f, "easytype", EaseType.linear, "islocal", true));
        iTween.MoveTo(emptyBox, iTween.Hash("position", emptyPosition, "time", 1.5f, "easytype", EaseType.linear, "islocal", true));
        Invoke("OpenRevocationCommandFlag", 1.6f);
    }

    void OpenRevocationCommandFlag()
    {
//        PublicClass.AllowRecordCommand = true;
    }

    public void SetSignRotation(Transform SignTransform)
    {
        isAutoRotation = true;
        Camera.main.GetComponent<XT_TouchContorl>().isAtlasModelRotate = true;
        DebugLog.DebugLogInfo("旋转期间禁止点击");
        iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(SignTransform.rotation.eulerAngles.x,
            SignTransform.rotation.eulerAngles.y, 0), "easetype", "linear", "time", 1));
        Invoke("InvokeStopAutoRotate", 1f);
    }

    void InvokeStopAutoRotate()
    {
        //x = emptyBox.transform.rotation.eulerAngles.y;
        //y = emptyBox.transform.rotation.eulerAngles.x;
        //z = emptyBox.transform.rotation.eulerAngles.z;
        //Quaternion rotation = Quaternion.Euler(y, x, 0);
        //emptyBox.transform.rotation = rotation;
        Camera.main.GetComponent<XT_TouchContorl>().isAtlasModelRotate = false;
        DebugLog.DebugLogInfo("开启下一次点击");
        Interaction.instance.setParamValue2();
    }

    public void ChangeRotaionAxis()
    {
        Hashtable args = new Hashtable();
        //插值的起点和终点，数据类型可为 float、double、Vector3、Vector2、Color、Rect
        args.Add("from", 0.0f);
        args.Add("to", 1.0f);
        args.Add("speed", 10f);
        args.Add("time", 1f);
        args.Add("loopType", iTween.LoopType.none);
    }

    public void Reset()
    {
        DebugLog.DebugLogInfo("________" + SceneModels.instance.get_Split_mode());
        if (SceneModels.instance.get_Split_mode()) return;

        if(SceneModels.instance.get_Multi_Selection())
        {
            XT_TouchContorl.Instance.expPanel.SetActive(true);
        }
        // 重新定位距离
        // SceneModels.SetCameraPosition();

        if(distanceControl!=null)
            distanceControl.value = Camera.main.transform.localPosition.z;
        // distance = resetDistance;
        // //平移
        // x1 = 0;
        // y1 = 0;
        // //旋转
        // x = 0;
        // y = 0;
        // PublicClass.currentState = RunState.UI;
        // iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(0, 0, 0), "time", 1));
        // iTween.MoveTo(emptyBox, iTween.Hash("position", new Vector3(positionX, positionY, positionZ), "islocal", true, "time", 1f));
        // iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(0, 0, distance), "islocal", true, "time", 1f));
        // Invoke("ChangeState", 1f);
    }

    public void ChangeState()
    {
        PublicClass.currentState = RunState.Playing;
    }

    public void ResetCamera()
    {
        iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(0, 0, distance), "time", 1.5f, "easytype", EaseType.linear, "islocal", true));
        Invoke("ReCam", 1.5f);
    }
    void ReCam()
    {
        x1 = 0;
        y1 = 0;
    }

    //进入图谱模式，摄像机参数变化
    // public void EnterMapMode()
    // {
    //     SetPositionRotationDistance(ModleChoose.MainModel, distance, new Vector3(positionX, positionY, positionZ), new Vector3(RotationX, RotationY, RotationZ));
    // }
    // //进入图钉模式，摄像机参数变化
    // public void EnterSignMode()
    // {
    //     distance = sign_distance;
    //     SetPositionRotationDistance(ModleChoose.SignModel, sign_distance, new Vector3(positionX, positionY, positionZ), new Vector3(RotationX, RotationY, RotationZ));
    // }
    // public void SetPositionRotationDistance(ModleChoose whichMode, float distance, Vector3 LocalPosition, Vector3 LocalRotation)
    // {

    //     //移动主摄像机深度距离
    //     this.transform.localPosition = new Vector3(0, 0, distance);
    //     PublicClass.cameraBackAxisPosition = LocalPosition;
    //     PublicClass.cameraBackAxisRotation = LocalRotation;
    //     if (whichMode == ModleChoose.MainModel)
    //     {
    //         mapCameraParms.Distance = distance;
    //     }
    //     if (whichMode == ModleChoose.SignModel)
    //     {
    //         signCameraParms.Distance = distance;
    //     }
    //     emptyPosition = LocalPosition;
    //     iTween.MoveTo(emptyBox, iTween.Hash("position", LocalPosition, "islocal", true, "time", 1f));
    //     iTween.RotateTo(emptyBox, iTween.Hash("rotation", LocalRotation, "time", 1));
    // }

    bool IsEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)//函数返回真为放大，返回假为缩小
    {
        //函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势
        var leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        var leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
        if (leng1 < leng2)
        {
            return true; //放大手势
        }
        else
        {
            return false;//缩小手势
        }
    }
    private bool isB = false, isR = false, isD = false, isS = false;
    [Header("orthogonalImage")]
    public Image forB, lorR, torD, Show;
    int rotateCount = 0;

    public void ResetDBCamera()
    {
        DbRepository<GetStructList> local_db2;
        //local_db2 = new DbRepository<MotorAnatomy_submodel>();
        local_db2 = new DbRepository<GetStructList>();
        local_db2.DataService("vesali.db");
        Debug.Log(PublicClass.app.app_id);
        GetStructList scope_Ie = local_db2.SelectOne<GetStructList>((tempNo) =>
        {
            if (tempNo.nounNo == PublicClass.app.app_id)
            {
                Debug.Log(tempNo.nounName);
                //DebugLog.DebugLogInfo(ss_id.bone_name+" "+ss_id.main_tex+" "+ ss_id.mask_tex);
                return true;
            }
            else
            {
                return false;
            }
        });
        Camera.main.transform.parent.localPosition = PublicTools.Str2Vector3(scope_Ie.camParentPos);
        Camera.main.transform.parent.rotation = Quaternion.Euler(PublicTools.Str2Vector3(scope_Ie.camParentRot));
        Camera.main.transform.localPosition = new Vector3(0,0, PublicTools.Str2Vector3(scope_Ie.camPos).z);
        float z = Camera.main.transform.rotation.z;
        if (z < 90 && z > -90)
            Interaction.instance.setParamValue2();
        else
            Interaction.instance.setParamValue3();



    }


    public class camera_params
    {
        public vector3 camera_pos;
        public vector3 camera_parent_rot;
        public vector3 camera_parent_pos;
    }
    void changeCamera(string camera)
    {

        Debug.Log(camera);
        camera_params str = JsonConvert.DeserializeObject<camera_params>(camera);

        Camera.main.transform.parent.position = PublicTools.vector2Vecotor(str.camera_parent_pos);
        Camera.main.transform.parent.rotation = Quaternion.Euler(PublicTools.vector2Vecotor(str.camera_parent_rot));
        Camera.main.transform.position = PublicTools.vector2Vecotor(str.camera_pos);
        float z = str.camera_parent_rot.z;
        if (z < 90 && z > -90)
            Interaction.instance.setParamValue2();
        else
            Interaction.instance.setParamValue3();

    }


    public void FtoLtoBtoR()
    {
        switch (rotateCount)
        {
            case 0:
                iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(0, 90 * rotateCount, 0), "time", 1));
                Invoke("XY0", 1);
                isB = false;
                rotateCount = 1;
                break;
            case 2:
                iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(0, 90 * rotateCount, 0), "time", 1));
                Invoke("XY1", 1);
                isB = true;
                rotateCount = 3;
                break;
            case 1:
                iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(0, 90 * rotateCount, 0), "time", 1));
                Invoke("XY2", 1);
                isR = false;
                rotateCount = 2;
                break;
            case 3:
                iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(0, 90 * rotateCount, 0), "time", 1));
                Invoke("XY3", 1);
                isR = true;
                rotateCount = 0;
                break;
        }
    }
    void XY0()
    {
        x = 0;
        y = 0;
    }
    void XY1()
    {
        x = 180;
        y = 0;
    }
    void XY2()
    {
        x = 90;
        y = 0;
    }
    void XY3()
    {
        x = -90;
        y = 0;
    }
    public void TorD()
    {
        if (isD)
        {
            iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(90, 0, 0), "time", 1));
            Invoke("XY4", 1);
            isD = false;
        }
        else
        {
            iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(-90, 0, 0), "time", 1));
            Invoke("XY5", 1);
            isD = true;
        }
    }
    void XY4()
    {
        x = 0;
        y = 90;
    }
    void XY5()
    {
        x = 0;
        y = -90;
    }

    public void DontRotate()
    {
        IsRotate = false;
        DebugLog.DebugLogInfo("rotatestate " + IsRotate);
    }

    int coutX = 1;
    int coutY = 1;
    public void Rot_X()
    {
        isAutoRotation = true;

        iTween.RotateTo(Camera.main.transform.parent.gameObject, iTween.Hash("rotation", new Vector3(0, (coutX % 4) * 90, 0), "islocal", true, "time", 0.5, "oncomplete", "StopRotation", "oncompletetarget", gameObject));
        coutX++;
        if (coutX == 5)
        {
            coutX = 1;
        }
        //iTween.RotateTo(Camera.main.transform.parent.gameObject, iTween.Hash("rotation", Camera.main.transform.parent.rotation.eulerAngles + new Vector3(0, 90, 0),"time", 0.5, "oncomplete", "StopRotation", "oncompletetarget", gameObject));
    }
    public void Rot_Y()
    {
        isAutoRotation = true;
        float xTarget = ((coutY % 2) == 1) ? 90 : -90;
        float zTarget = (xTarget == 90) ? 0 : 180;


        //90 0 0
        //-90 0 180
        //iTween.RotateTo(Camera.main.transform.parent.gameObject, iTween.Hash("rotation", new Vector3((coutY%4)* 90,0, 0), "islocal", true, "time", 0.5, "oncomplete", "StopRotation", "oncompletetarget", gameObject));
        iTween.RotateTo(Camera.main.transform.parent.gameObject, iTween.Hash("islocal", true, "rotation", new Vector3(xTarget, 0, zTarget), "time", 0.5, "oncomplete", "StopRotation", "oncompletetarget", gameObject));
        coutY++;
        if (coutY == 3)
        {
            coutY = 1;
        }
    }
    public void StopRotation()
    {
        Interaction.instance.x=emptyBox.transform.rotation.eulerAngles.y;
        Interaction.instance.y=-emptyBox.transform.rotation.eulerAngles.x;
        // x = emptyBox.transform.rotation.eulerAngles.y;
        // y = emptyBox.transform.rotation.eulerAngles.x;
        // z = emptyBox.transform.rotation.eulerAngles.z;
        // x1 = transform.localPosition.x;
        // y1 = transform.localPosition.y;
        // distance = transform.localPosition.z;
        isAutoRotation = false;
    }
    //缩放滑动条
    public Slider distanceControl;
    bool disCon = false;

    bool is_360 = false;
    //自动旋转速度
    float moveSpeed = 30;

    //旋转开关切换
    public void To_360()
    {
        if (SceneModels.instance != null && SceneModels.instance.get_Split_mode())
        {
            return;
        }
        is_360 = !is_360;
        print("--------"+is_360);
    }

    public void InitSlider(float min,float max,float dis)
    {
        distanceControl.minValue=min;
        distanceControl.maxValue=max;
        distanceControl.value=dis;
    }
    public void Reset360() { is_360 = false; }
    //在缩放滑动条上按下按键
    public void disDown()
    {
        disCon = true;
    }
    //在缩放滑动条上抬起按键
    public void disUp()
    {
        disCon = false;
    }
    public void setParamValue()
    {
        //print("set params value");
        //print(emptyBox.transform.rotation.eulerAngles);
        //print(transform.localPosition);
        x = emptyBox.transform.rotation.eulerAngles.y;
        y = emptyBox.transform.rotation.eulerAngles.x;
        z = emptyBox.transform.rotation.eulerAngles.z;
        x1 = transform.localPosition.x;
        y1 = transform.localPosition.y;
        distance = transform.localPosition.z;
    }

    public void setParamValue2()
    {
        x = emptyBox.transform.rotation.eulerAngles.y;
        y = -emptyBox.transform.rotation.eulerAngles.x;
    }

    public bool isInUiRect()
    {
        bool isInModelControlArea = true;
        try
        {
            Rect rect = uicam.rect;
            if ((Input.mousePosition.x > rect.x * Screen.width) && (Input.mousePosition.x < (rect.x + rect.width) * Screen.width) &&
                (Input.mousePosition.y > rect.y * Screen.height) && (Input.mousePosition.y < (rect.y + rect.height) * Screen.height))
            {
                isInModelControlArea = true;
            }
            else
            {
                isInModelControlArea = false;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
        return isInModelControlArea;
    }
    private void Update()
    {

        //是否处于旋转状态
        if (is_360)
        {
            //根据旋转时间改变控制参数
            Interaction.instance.x += Time.deltaTime * moveSpeed;
            Interaction.instance.RotateOpera( Interaction.instance.x, Interaction.instance.y);
            // //根据旋转时间改变控制参数
            // x += Time.deltaTime * moveSpeed;
            // Quaternion rotation = Quaternion.Euler(y, x, z);
            // emptyBox.transform.rotation = rotation;
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        //滑动条缩放
        if (disCon)
        {
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, Camera.main.transform.localPosition.y, distanceControl.minValue+distanceControl.maxValue - distanceControl.value);
            distance = Camera.main.transform.localPosition.z;
        }
        else
        {
            if( distanceControl!=null)
                distanceControl.value = Camera.main.transform.localPosition.z;
        }
#endif

        if (Input.GetMouseButton(0) && !disCon)
        {
            is_360 = false;
            isAutoRotation = false;
        }
        //Debug.Log(isAutoRotation +"--"+disCon+"--"+ (isWeiKe && PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer && PPTGlobal.pptStatus != PPTGlobal.PPTStatus.pause));
        if (isAutoRotation || disCon || (isWeiKe&& PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.PPTPlayer && PPTGlobal.pptStatus != PPTGlobal.PPTStatus.pause))
        {
            setParamValue();
            return;
        }
        bool isInModelControlArea = isInUiRect();
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer && (Input.GetMouseButtonDown(0) || Input.touchCount!=0)) {
            try {
                if (PPTController.currentSlideObj != null) {
                    PPTController.currentSlideObj.BroadcastMessage("SetModelCam", isInModelControlArea,SendMessageOptions.DontRequireReceiver);
                }
            }
            catch (Exception e) {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }

        //#if UNITY_ANDROID || UNITY_IPHONE
        //移动端判断如下 
        // if (isInModelControlArea && (PublicClass.currentModle == ModleChoose.SignModel || (SceneModels.instance != null && !SceneModels.instance.get_Split_mode())) && ((Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) || isWeiKe))
        //#else 
        //PC端判断如下 
        //if (isInModelControlArea && (PublicClass.currentModle == ModleChoose.SignModel || (SceneModels.instance != null && !SceneModels.instance.get_Split_mode())) && (!EventSystem.current.IsPointerOverGameObject() || isWeiKe))
        //#endif
        if (isInModelControlArea && (PublicClass.currentModle == ModleChoose.SignModel|| PublicClass.currentModle == ModleChoose.AniModel || (SceneModels.instance != null && !SceneModels.instance.get_Split_mode())))
        {
            if (XT_AllButton.isPrintMode) return;

            if (Input.touchCount >= 1)
            {

                if ((Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) || isWeiKe)
                {
                    //新的操控模式
                    if (Input.touchCount > 1)
                    {
                        if (FingerState != FingerTouchState.TwoFingerTouch)
                        {
                            //enter_special_two_finger_state();
                            FingerState = FingerTouchState.TwoFingerTouch;
                        }
                        touchCtrl.TouchTwoFinger();

                    }
                    else if (Input.touchCount == 1)
                    {
                        if (FingerState == FingerTouchState.TwoFingerTouch)
                        {
                            //leave_special_two_finger_state();
                            FingerState = FingerTouchState.OneFingerTouch;
                        }
                        touchCtrl.TouchOneFinger();
                    }
                }
            }


            else
            {
                if ((!EventSystem.current.IsPointerOverGameObject() || isWeiKe))
                {
                    if (FingerState == FingerTouchState.TwoFingerTouch)
                    {
                        //leave_special_two_finger_state();
                        FingerState = FingerTouchState.NonFingerTouch;
                    }
                    touchCtrl.MouseClick();
                    touchCtrl.MouseDrag();
                    touchCtrl.MouseRollWheel();
                }

            }
        }

        //性能测试
        //if(Input.GetKeyDown(KeyCode.K))
        //{
        //    recordState = new PlayerCommand();
        //    Debug.Log("记录模型状态");
        //    SceneModels.instance.SendHideMessage("淋巴");
        //}
        //if(Input.GetKeyDown(KeyCode.B))
        //{
        //    recordState.RevocationCommand();
        //    Debug.Log("读取模型状态");
        //}

        #region 旧的操控模式（目前已弃用） 鼠标操控

        // //清空上一帧的碰撞点位置
        // if (!Input.GetMouseButton(1) /*&& Input.touchCount != 2*/)
        // {
        //     lastTransPos.Clear();
        // }


        // //鼠标右键平移

        // if (Input.GetMouseButton(1))
        // {
        //     if (lastTransPos.Count == 0)
        //     {
        //         lastTransPos.Add(uicam.ScreenToWorldPoint(Input.mousePosition));
        //     }
        //     else
        //     {
        //         Vector3 tmp = uicam.ScreenToWorldPoint(Input.mousePosition);
        //         moveCam(lastTransPos[0], tmp);
        //         lastTransPos[0] = tmp;
        //     }
        // }

        // //鼠标左键旋转
        // if (Input.GetMouseButton(0))
        // {
        //     PublicClass.currentState = RunState.Playing;
        //     PublicClass.CurrentFingerState = FingerState.SingleFingerRotate;
        //     //根据触摸点计算X与Y位置
        //     x += Input.GetAxis("Mouse X") * xSpeed;
        //     y += Input.GetAxis("Mouse Y") * ySpeed;
        //     Quaternion rotation = Quaternion.Euler(y, x, z);
        //     emptyBox.transform.rotation = rotation;
        // }

        // //鼠标滚轮缩放

        // if (Input.GetAxis("Mouse ScrollWheel") != 0)
        // {
        //     distance -= Input.GetAxis("Mouse ScrollWheel") * distanceSpeed;
        //     distance = Mathf.Clamp(distance, minDistance, maxDistance);
        //     Vector3 position = new Vector3(transform.localPosition.x, transform.localPosition.y, distance);
        //     transform.localPosition = position;
        // }


        // #endregion

        // #region   触控板操作

        //如果有触控点操作

        //if (Input.touchCount >= 1)
        //{
        //    if (Input.touchCount != 2)
        //    {
        //        lastTransPos.Clear();
        //    }
        //    //判断触摸数量为单点触摸
        //    if (Input.touchCount == 1)
        //    {
        //        PublicClass.OpenPreRecordState = true;
        //        //触摸类型为移动触摸时，旋转模型
        //        if (Input.GetTouch(0).phase == TouchPhase.Moved)
        //        {
        //            DebugLog.DebugLogInfo("单指移动  坐标：" + Input.GetTouch(0).position);

        //            //将当前触摸状态切换为单指旋转
        //            PublicClass.CurrentFingerState = FingerState.SingleFingerRotate;
        //            XT_AllButton.Instance.SignExplain.SetActive(false);
        //            x += Input.GetTouch(0).deltaPosition.x;
        //            y += Input.GetTouch(0).deltaPosition.y;

        //            DebugLog.DebugLogInfo("旋转轴向参数： x: " + x + " y: " + y);
        //        }

        //        Quaternion rotation_iphone = Quaternion.Euler(y, x, 0);
        //        emptyBox.transform.rotation = rotation_iphone;
        //    }
        //    //判断触摸数量为多点触摸
        //    else if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        //    {
        //        PublicClass.OpenPreRecordState = true;
        //        PublicClass.currentState = RunState.Playing;
        //        //前两只手指触摸类型都为移动触摸
        //        //计算出当前两点触摸点的位置
        //        Vector2 tempPosition1 = Input.GetTouch(0).position;
        //        Vector2 tempPosition2 = Input.GetTouch(1).position;
        //        DebugLog.DebugLogInfo("touch_1: " + tempPosition1 + "  touch_2 : " + tempPosition2);
        //        float flag = Input.GetTouch(0).deltaPosition.x * Input.GetTouch(1).deltaPosition.x + Input.GetTouch(0).deltaPosition.y * Input.GetTouch(1).deltaPosition.y;
        //        DebugLog.DebugLogInfo(Input.GetTouch(0).deltaPosition.x + "  " + Input.GetTouch(1).deltaPosition.x + "  " + Input.GetTouch(0).deltaPosition.y + "  " + Input.GetTouch(1).deltaPosition.y);
        //        //函数返回真为放大，返回假为缩小
        //        //双指实现缩放
        //        if (flag < 0)
        //        {
        //            DebugLog.DebugLogInfo("缩放： flag:  " + flag);
        //            if (IsEnlarge(oldPosition1, oldPosition2, tempPosition1, tempPosition2))
        //            {
        //                if (distance > minDistance)
        //                {
        //                    distance -= zoom * distance;
        //                }
        //            }
        //            else
        //            {
        //                if (distance < maxDistance)
        //                {
        //                    distance += zoom * distance;
        //                }
        //            }
        //            //备份上一次触摸点的位置，用于对比
        //            oldPosition1 = tempPosition1;
        //            oldPosition2 = tempPosition2;
        //            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        //            Vector3 position = new Vector3(transform.localPosition.x, transform.localPosition.y, distance);
        //            transform.localPosition = position;
        //        }
        //        if (flag >= 0)//说明方向相同，开启平移模式
        //        {
        //            // direction = (Input.GetTouch(0).deltaPosition + Input.GetTouch(1).deltaPosition) / 2;  //获取手指touch最后一帧移动的xy轴距离
        //            // Vector3 temp = new Vector3(-direction.x * distance * translation * Screen.width / 1080 * Screen.height / 1920,
        //            //     -direction.y * distance * translation * Screen.width / 1080 * Screen.height / 1920, 0);
        //            // transform.Translate(temp, Space.Self);
        //            // DebugLog.DebugLogInfo("每帧移动向量 temp: (" + temp.x + "," + temp.y + ")");

        //            if (lastTransPos.Count == 0)
        //            {
        //                lastTransPos.Add(uicam.ScreenToWorldPoint((Input.GetTouch(0).position + Input.GetTouch(1).position) / 2));
        //            }
        //            else
        //            {
        //                Vector3 tmp = uicam.ScreenToWorldPoint((Input.GetTouch(0).position + Input.GetTouch(1).position) / 2);
        //                moveCam(lastTransPos[0], tmp);
        //                lastTransPos[0] = tmp;
        //            }
        //        }
        //    }
        //}
        #endregion
    }


    public float transSpeed = 2.5f;
    void moveCam(Vector3 last, Vector3 nowPos)
    {
        Vector3 dis = last - nowPos;

        float halfFOV = (Camera.main.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float height = Mathf.Tan(halfFOV);
        transSpeed = height;

        Camera.main.transform.Translate((last - nowPos) / uicam.orthographicSize * (distanceControl.value * transSpeed));
        //Camera.main.transform.Translate((last - nowPos) / uicam.orthographicSize * Mathf.Pow((distanceControl.value / distanceControl.maxValue),2) / uicam.orthographicSize);
        x1 = transform.localPosition.x;
        y1 = transform.localPosition.y;
    }


    public void OnGUI()
    {
        // GUIStyle bb = new GUIStyle();
        // bb.normal.background = null;    //这是设置背景填充的
        // bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
        // bb.fontSize = 40;       //当然，这是字体大小


        // if (Input.touchCount >= 1)
        // {
        //     GUI.Label(new Rect((Screen.width / 2) - 100, 0, 200, 200), "touch_1: " + Input.GetTouch(0).deltaPosition, bb);
        // }
        // if (Input.touchCount >= 2)
        // {
        //     GUI.Label(new Rect((Screen.width / 2) - 100, 50, 200, 200), "touch_2: " + Input.GetTouch(1).deltaPosition, bb);
        // }
    }

    Vector3[] GetCorners(float distance)
    {
        Transform tx = this.transform;
        Camera theCamera = this.GetComponent<Camera>();
        Vector3[] corners = new Vector3[4];

        float halfFOV = (theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = theCamera.aspect;

        float height = distance * Mathf.Tan(halfFOV);
        float width = height * aspect;

        // UpperLeft
        corners[0] = tx.position - (tx.right * width);
        corners[0] += tx.up * height;
        corners[0] += tx.forward * distance;

        // UpperRight
        corners[1] = tx.position + (tx.right * width);
        corners[1] += tx.up * height;
        corners[1] += tx.forward * distance;

        // LowerLeft
        corners[2] = tx.position - (tx.right * width);
        corners[2] -= tx.up * height;
        corners[2] += tx.forward * distance;

        // LowerRight
        corners[3] = tx.position + (tx.right * width);
        corners[3] -= tx.up * height;
        corners[3] += tx.forward * distance;

        return corners;
    }

    public float GetCameraWindowLength()
    {
        Vector3[] Corners = GetCorners(distance);
        Vector3 v3 = Corners[0] - Corners[2];
        return v3.y / 2;
    }

    private void OnDestroy()
    {
        Destroy(this);
    }

    //对于双指模型的特殊处理，重点解决大模型下性能物体
    //List<string> tempModelBox;


    //用于记录性能优化前模型状态
    PlayerCommand recordState;
    //进入双指模式，对于性能差的手机开启模型优化
    public void enter_special_two_finger_state()
    {
        #if UNITY_ANDROID
        vesal_log.vesal_write_log("enter special_two_finger_state obj num " + SceneModels.instance.get_scope_model_numbers());
        vesal_log.vesal_write_log("enter special_two_finger_state fps:" + ShowFPS.f_Fps);
        gameObject.GetComponent<Camera>().allowMSAA = false;
        if (SceneModels.instance.get_scope_model_numbers() < PublicClass.ThresholdModelNumber)
            return;
        else if (ShowFPS.f_Fps < 8)
        {
            vesal_log.vesal_write_log("关闭淋巴系统");
            isHideMode = true;
            recordState = new PlayerCommand();
            SceneModels.instance.SendHideMessage("淋巴");

            if (ShowFPS.f_Fps < 5)
                SceneModels.instance.SendHideMessage("神经");
            SceneModels.instance.SendHideMessage("静脉");
            SceneModels.instance.SendHideMessage("动脉");

        }
        #endif
        //tempModelBox = new List<string>();

        //List<Ray> AllRays = new List<Ray>();
        //Vector3 Point = new Vector3();

        //for(int x=50;x<Screen.width-50;x+=50)
        //    for (int y = 100; y < Screen.height - 100; y += 80)
        //    {
        //        Point.x=x;
        //        Point.y=y;
        //        AllRays.Add(Camera.main.ScreenPointToRay(Point));
        //    }
        //for(int k=0;k<AllRays.Count;k++)
        //{
        //    RaycastHit hit;
        //    if (Physics.Raycast(AllRays[k], out hit))
        //    {
        //        tempModelBox.Add( hit.transform.GetComponent<Model>().gameObject.name);
        //    }
        //}
        //SceneModels.instance.specialHideOthers(tempModelBox);
    }

    //离开双指状态时恢复显示细小模型
    public void leave_special_two_finger_state()
    {
        #if UNITY_ANDROID
        //if (SceneModels.instance.get_scope_model_numbers() < PublicClass.ThresholdModelNumber)
        //    return;
        //SceneModels.instance.specialRestoreOthers();
        if (isHideMode == true)
        {
            //SceneModels.instance.SendShowMessage("淋巴");
            // SceneModels.instance.SendShowMessage("神经");
            recordState.RevocationCommand();
            vesal_log.vesal_write_log("leave special_two_finger_state obj num " + isHideMode.ToString());
            isHideMode = false;
        }

        gameObject.GetComponent<Camera>().allowMSAA = true;
        #endif
    }

}