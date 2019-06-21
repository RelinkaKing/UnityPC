using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCamera : MonoBehaviour
{
    public float targetDistance = -160f;
    public bool noControlRegion = false;//标志是否禁止计算区域
    public Dictionary<int, SuperPoint> pd = new Dictionary<int,SuperPoint>();//触控点map
    public float distance = 0;//主、辅点的距离
    public bool inited = false;
    float d = 0;//记录鼠标在按下状态时的位移值，当左键抬起时清空
    bool panned = false;//鼠标有没有移动过
    public int s = 0;
    int count = 0;
    //public TouchableObject[] preObjectsInScene;
    //public TouchableObject[] objectsInScene;
    
    public Vector3 jiaodian = new Vector3();
    public ArrayList names1;//用于层次按钮的显示
    public ArrayList names2;//用于层次按钮的显示
    public static GameCamera Inst = null;
    //摄像机距离焦点的距离
    public float m_distance;
    public float m_distanceMax;
    public float m_distanceMin;//焦距范围
    //摄像机的角度
    public Vector3 m_rot;
    //摄像机的移动速度
    public float m_rotateSpeed = 53;
    public float m_panSpeed = 0f;
    public float m_scaleSpeed = 24;
    //Transform组件
    protected Transform m_transform;
    //摄像机的焦点
    public Transform m_cameraPoint;
    //记录手指触屏的位置
    Vector2 m_screenpos = new Vector2();
    
    //标识是否处于单选模式
    public bool singleSelect = true;
    //是否是单击
    bool isSingleTapUp = false;
    private string objName;
    
    void Awake()
    {
        Inst = this;
        m_transform = this.transform;
    }
    // Use this for initialization
    void Start()
    {
        //获得摄像机的焦点
        m_cameraPoint = CameraPoint.Instance.transform;
        m_distance = m_transform.localPosition.z;
        // Debug.Log("m_distance" + m_distance);
        //允许多点触屏
        //Input.multiTouchEnabled = true;
        m_rot = new Vector3(0, 180, 0);
        names1 = new ArrayList();
        names2 = new ArrayList();
        //objectsInScene = new List<TouchableObject>();
        //preObjectsInScene = GameObject.FindObjectsOfType<TouchableObject>();
        //foreach (TouchableObject tou in GameCamera.Inst.preObjectsInScene)
        //{
            //if (tou.isNode && tou.state != SuperGameManager.States.Hide)
            //{
                //objectsInScene.Add(tou);
                //objectsInSceneTemp.Add(tou);
            //}
        //}
        
    }
    /**
    [ContextMenu("CalObjectsInScene")]
    public void HideModel()
    {


        objectsInScene = GameObject.FindObjectsOfType<TouchableObject>();
        foreach (TouchableObject obj in GameCamera.Inst.objectsInScene)
        {
            if (obj.transform.IsChildOf(muscularSystem) || obj.transform.IsChildOf(digestiveSystem) || obj.transform.IsChildOf(digestiveSystem)
                || obj.transform.IsChildOf(respiratorySystem) || obj.transform.IsChildOf(cardiovascularSystem)
                || obj.transform.IsChildOf(lymphaticSystem)
                || obj.transform.IsChildOf(endocrineSystem) || obj.transform.IsChildOf(reproductiveSystem) || obj.transform.IsChildOf(urinarySystem)
                || obj.transform.IsChildOf(nervousSystem) //|| obj.transform.IsChildOf(skinSystem)
                )
            {
                obj.Hide();
                //print("确实是他的节点啊");
            }
        } 
    }
    **/
    // Update is called once per frame
    void Update()
    {
        if (!inited)
        {
            //ButtonSpawnController.instance.HideModel();
            //SuperGameManager.Instance.Init();
            inited = true;           
        }
    } 
    
    //摄像机对齐到焦点的位置和角度
    public void Follow()
    {
        m_distance = Mathf.Min(m_distance, m_distanceMax);
        m_distance = Mathf.Max(m_distanceMin, m_distance);
        m_transform.localPosition = new Vector3(0, 0, m_distance);
        //Debug.Log("m_distance" + m_distance);
    }
    
    public void Wizard(Transform transform)//关键代码
    {
        names1.Clear();
        names2.Clear();
        //如果他有子对象
        if (transform.childCount != 0)
        {
            //那么先把它的所有子对象加进names进行管理
            for (int i = 0; i < transform.childCount; i++)
            {
                names2.Add(transform.GetChild(i));
            }
            names1.Add(transform);
            while (transform.parent != null)//如果他还有有父对象
            {
                transform = transform.parent;
                names1.Add(transform);
            }
        }
        else//如果他没有子对象
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                names2.Add(transform.parent.GetChild(i));
            }
            while (transform.parent != null)//如果他还有有父对象
            {
                transform = transform.parent;
                names1.Add(transform);
            }
        }
    }

    
}
