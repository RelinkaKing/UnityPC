using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Hotfix]
public class Interaction : MonoBehaviour
{

    public static Interaction instance;
    // Use this for initialization
    //public Material mat;
    //Material initMat;

    #region 交互系统使用的常量

    //鼠标纵向旋转速度
    public const float rotateSpeedX = 3;
    //鼠标横向旋转速度
    public const float rotateSpeedY = 1;
    //触控纵向旋转调整参数
    public const float rotateTouchY = 0.3f;

    //缩放限制
    public  float minDistance = 0.5f; //最近
    [HideInInspector]
    public  float maxDistance = 15; //最远

    //鼠标缩放速度
    public const float distanceSpeed = 5;

    //触控缩放速度
    public const float distanceSpeedTouch = 0.070f;

    //进入光标模式所需要的时间
    public const float timeCurMode = 1.2f;
    #endregion
    public static float AnimationRotateY = 1;

    [HideInInspector]
    public bool isRotate;

    //获取旋转轴
    [HideInInspector]
    public GameObject rotateAxis;

    //获取正交相机，便于实现平移


    public Camera orthCamera;

    //摄像机到显示物体的Z轴距离
    [HideInInspector]
    public float distance;
    public float fix_dis;

    //摄像机的位置
    [HideInInspector]
    public float x, y,z;

    //记录摄像机上次操作的位置
    [HideInInspector]
    public List<Vector3> lastTransPos = new List<Vector3>();
    [LuaCallCSharp]
    void Awake()
    {
        instance = this;

        rotateAxis = GameObject.Find("CameraBack");
        if (orthCamera == null)
        {
            orthCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        }

    }
    [LuaCallCSharp]
    void Start()
    {
        //transSpeed = Mathf.Tan ((Camera.main.fieldOfView * 0.5f) * Mathf.Deg2Rad);

    }

    public class camera_params
    {
        public vector3 camera_pos;
        public vector3 camera_parent_rot;
        public vector3 camera_parent_pos;
    }

    [LuaCallCSharp]
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("r已被按下");
            DebugLog.DebugLogInfo(PublicClass.app.app_id + "----***-----" + 
                new Vector3(Camera.main.transform.localPosition.x, Camera.main.transform.localPosition.y, Camera.main.transform.localPosition.z)+" "
                + new Vector3(Camera.main.transform.parent.transform.localPosition.x, Camera.main.transform.parent.transform.localPosition.y, Camera.main.transform.parent.transform.localPosition.z)
                + new Vector3(Camera.main.transform.parent.transform.eulerAngles.x, Camera.main.transform.parent.transform.eulerAngles.y, Camera.main.transform.parent.transform.eulerAngles.z) + " " 
                );
            Debug.Log("记录摄像机数据成功");
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKey(KeyCode.UpArrow))
                Interaction.instance.transform.parent.localPosition -= new Vector3(0,0,0.005f);
            if (Input.GetKey(KeyCode.DownArrow))
                Interaction.instance.transform.parent.localPosition += new Vector3(0, 0, 0.005f);
            if (Input.GetKey(KeyCode.LeftArrow))
                Interaction.instance.transform.parent.localPosition += new Vector3(0, 0.005f, 0);
            if (Input.GetKey(KeyCode.RightArrow))
                Interaction.instance.transform.parent.localPosition -= new Vector3(0, 0.005f, 0);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            print(this.rotateAxis.transform.localEulerAngles.ToString());
            vesal_log.vesal_write_log(PublicClass.app.app_id+"----***-----"+ this.rotateAxis.transform.localEulerAngles.ToString()+" z :"+
                this.transform.localPosition.z);
        }
        //cursor.transform.position = Input.mousePosition;
        //Debug.Log(cursor.transform.position);
    }
    //选择操作
    [LuaCallCSharp]
    public void ChooseOpera()
    {
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;

        // if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Model")
        // {
        //     hit.transform.gameObject.GetComponent<Renderer>().material = mat;

        // }
        // //选择模型
    }

    //突出选择操作
    [LuaCallCSharp]
    public void EmphasisOpera()
    {
        //选择模型并将模型置于屏幕中央
        //设置为围绕当前选择模型旋转
    }
    //通过改变摄像机的位置实现平移操作
    [LuaCallCSharp]
    public void TranslateOpera(Vector3 lastPos, Vector3 nowPos)
    {
        transform.Translate((lastPos - nowPos) / orthCamera.orthographicSize * Mathf.Tan((Camera.main.fieldOfView * 0.5f) * Mathf.Deg2Rad) * transform.localPosition.z);

    }
    [LuaCallCSharp]
    public void TranslateOpera(Vector3 target)
    {
        transform.localPosition = target;
    }
    //通过调整摄像机与物体的距离实现缩放操作
    [LuaCallCSharp]
    public void ZoomOpera(float distance)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, distance);
    }
    [LuaCallCSharp]
    public void ResetCameraPos()
    {
        Camera.main.transform.localPosition = new Vector3(0, 0, Camera.main.transform.localPosition.z);
    }

    //通过调整旋转轴改变摄像机的位置实现旋转操作
    [LuaCallCSharp]
    public void RotateOpera(float rotateX, float rotateY)

    {
     //   Debug.Log(rotateAxis.transform.rotation);
        Quaternion rotation = Quaternion.Euler(-y, x, 0);
        //rotateAxis.transform.eulerAngles = Quaternion.Euler(0, 0, 0).eulerAngles;
     //   Debug.Log(rotateAxis.transform.rotation);
      //  Debug.Log(rotation);
        rotateAxis.transform.rotation = rotation;
      //  Debug.Log(rotateAxis.transform.eulerAngles);
    }
    [LuaCallCSharp]
    public void RotateOpera(Vector3 vt)
    {
        Quaternion rotation = Quaternion.Euler(vt);
        rotateAxis.transform.rotation = rotation;
        y = -rotateAxis.transform.rotation.eulerAngles.x;
        x = rotateAxis.transform.rotation.eulerAngles.y;
    }
    [LuaCallCSharp]
    /// <summary>
    /// 自动移动到目标位置（设定位置旋转）
    /// </summary>
    /// <param name="go"></param>
    public void SetTarget(GameObject go)
    {
        // rotateAxis.gameObject.transform.position=go.transform.position;
        iTween.MoveTo(rotateAxis.gameObject, iTween.Hash("position", go.transform.position, "time", 1.5f));
        ZoomOpera(distance);
    }
    [LuaCallCSharp]
    public void SetTarget(Vector3 position, float dis)
    {
        rotateAxis.transform.position = position;
        ZoomOpera(dis);
    }
    [LuaCallCSharp]
    public void Reset(float distance)
    {
        transform.localPosition = new Vector3(0, 0, distance);
    }
    [LuaCallCSharp]
    public void setParamValue()
    {
        print("set params value");
        x = rotateAxis.transform.rotation.eulerAngles.y;
        y = rotateAxis.transform.rotation.eulerAngles.x;
        Debug.Log(x + "       " + y);
    }
    [LuaCallCSharp]
    public void setParamValue2()
    {
        print("set params value");
        Debug.Log(rotateAxis.transform.localRotation);
        Debug.Log(rotateAxis.transform.rotation.eulerAngles);
        x = rotateAxis.transform.rotation.eulerAngles.y;
        y = -rotateAxis.transform.rotation.eulerAngles.x;
        distance = transform.localPosition.z;

        Debug.Log(x + "       " + y);
    }
    [LuaCallCSharp]
    public void setParamValue3()
    {
        print("set params value");
        x =-(180- rotateAxis.transform.localRotation.eulerAngles.y);
        y =-(180- rotateAxis.transform.localRotation.eulerAngles.x);
        z = rotateAxis.transform.rotation.eulerAngles.z;
        distance = transform.localPosition.z;

        Debug.Log(x + "       " + y+"                   "+z);
    }
}