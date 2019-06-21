using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

[Hotfix]
public class dramline : MonoBehaviour
{
    public static dramline instance;

    private GameObject clone;
    private LineRenderer line;
    public List<GameObject> lines = new List<GameObject>();
    int i;
    //带有LineRender物体  
    public GameObject target;
    public Color lineColor;
    public float wighth;
    public Color[] color;
    public Image[] colorButton;
    //public Camera uiCamera;//获取ui相机
    //public List<GameObject> lineWriteObjs = new List<GameObject>();//存储划线预制件的实例
    //private GameObject clone;
    //private LineRenderer line;
    //private int i;
    [LuaCallCSharp]
    void Awake()
    {
        instance = this;
    }
    //[LuaCallCSharp]
    //void Start()
    //{
    //    this.gameObject.SetActive(false);
    //}
    //[LuaCallCSharp]
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))//点下鼠标左键生成划线预制件 并保存在list中
    //    {
    //        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Cube")) as GameObject;
    //        lineWriteObjs.Add(obj);
    //        clone = (GameObject)Instantiate(obj, obj.transform.position, transform.rotation);//克隆一个带有LineRender的物体
    //        lineWriteObjs.Add(clone);
    //        line = clone.GetComponent<LineRenderer>();//获得该物体上的LineRender组件 
    //        i = 0;

    //    }
    //    if (Input.GetMouseButton(0))
    //    {
    //        i++;
    //        line.positionCount = i;//设置顶点数  
    //        line.SetPosition(i - 1, uiCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)));//设置顶点位置  
    //    }
    //}
    [LuaCallCSharp]
    public void ClearDrawLine()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            Destroy(lines[i].gameObject);
        }
        lines.Clear();
        ChooseColor(0);
    }
    public bool isCanDraw;
    void Update()
    {
        if (isCanDraw)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //实例化对象  
                clone = (GameObject)Instantiate(target, target.transform.position, Quaternion.identity);
                clone.SetActive(true);
                lines.Add(clone);
                //获得该物体上的LineRender组件  
                line = clone.GetComponent<LineRenderer>();
                //设置起始和结束的颜色  
                line.material.SetColor("_Color", lineColor);
                line.SetColors(lineColor, lineColor);
                //设置起始和结束的宽度  
                line.SetWidth(wighth, wighth);
                //计数  
                i = 0;
            }
            if (Input.GetMouseButton(0))
            {
                //每一帧检测，按下鼠标的时间越长，计数越多  
                i++;
                //设置顶点数  
                line.SetVertexCount(i);
                //设置顶点位置(顶点的索引，将鼠标点击的屏幕坐标转换为世界坐标)  
                line.SetPosition(i - 1, this.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15)));


            }
        }

    }



    //public GameObject DrawPanel;                //画笔界面
    //public Camera printingCamera;               //this.camera
    ////切换画笔ui图片
    [LuaCallCSharp]
    public void OpenOrCloseOperar(bool isPriceUI)
    {
        isCanDraw = isPriceUI;
        if (!isPriceUI)
        {
            ClearDrawLine();
        }
       // DrawPanel.SetActive(isPriceUI);                       //开关画笔界面
        //printingCamera.gameObject.SetActive(isPriceUI);       //开关画笔摄像机
        //if (!isPriceUI)
        //{
        //    ClearDrawLine();
        //}
    }
    //[LuaCallCSharp]
    //public void ForbidDraw()
    //{
    //    // PublicClass.isprinting = false;
    //}

    public delegate void ModelCameraRotate();
    public event ModelCameraRotate DontRoate;
    //private void OnDestroy()
    //{
    //    Destroy(this);
    //}

    //public Material drawColor_mat;

    public void ChooseColor(int num)
    {
        lineColor = color[num];

        for (int i = 0; i < colorButton.Length; i++)
        {
            colorButton[i].enabled = false;
        }
        colorButton[num].enabled = true;
    }

    //public void OpenColorKit()
    //{

    //}
}
