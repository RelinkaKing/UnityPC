using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 旧画笔元素控制器（现用全局画笔系统代替）
/// </summary>
public class PaintPenController : BaseController
{

    public PaintPenElement ppe;
    /// <summary>  
    /// 首先绘制椭圆的公式  
    /// 椭圆的参数方程x=acosθ,y=bsinθ；  
    /// </summary>  

    public Transform trans;
    public float w = 30f;//椭圆长  
    public float h = 20f; //椭圆高  
    public int angle = 360;
    [Range(0, 360)]
    public int speed = 3;
    private Vector3[] vec;
    private int index = 0;
    private LineRenderer line;
    bool isShowEd = false;
    MyCircleQueue<Vector3> ccq;
    public override void Begin()
    {
        if (isParticlesPlaying)
        {
            Particles.Play();
        }
    }

    public override void Do(float slideTime)
    {
        if (ppe.appearTime <= slideTime && !isShowEd) {
            topPanel.SetActive(true);
            if (line == null) {
                setPositions();
            }
            statParticles();
            show();
        }
        if (ppe.endTime <= slideTime)
        {
            GoDie(slideTime);
        }
    }
    Transform slide;
    public override void GoDie(float slideTime)
    {
        index = 0;
        transform.SetParent(slide, false);
        transform.SetAsFirstSibling();
        if (line != null) {
            line.enabled = false;
        }
        stopParticles();
    }
    void show() {
        

        transform.SetParent(topPanel.transform, false);
        transform.SetAsLastSibling();
        transform.localPosition = new Vector3(posX, posY, 0);
        Debug.Log(ppe.type+":"+posX);
        Debug.Log(ppe.type + ":" + posY);
        transform.localScale = new Vector3(1, 1, 1);
        line.enabled = false;
        isShowEd = true;
    }
   
    void setPositions() {

        float x, y;
        line = gameObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.material = paintPenMat;
        line.startWidth = 0.14f;
        if (ppe.type == "LINE") {
            angle = (int)Vector3.Distance(startPoint,endPoint);
            vec = new Vector3[angle];
            
            for (int i = 0; i < angle; i++)
            {
                vec[i] = Vector3.MoveTowards(startPoint, endPoint, i*3);
            }
        } else if (ppe.type == "ELLIPSE") {
            angle = 380;
            vec = new Vector3[angle];
            //ccq = new MyCircleQueue<Vector3>(angle);
            //Vector3 tmpV3 = new Vector3(posX, posY, 0);

            //椭圆
            for (int i = 0; i < angle; i++)
            {

                // Mathf.Deg2Rad 单位角度的弧 相当于 1° 的弧度  
                x = w / 2f * Mathf.Cos(i * Mathf.Deg2Rad);
                y = h / 2f * Mathf.Sin(i * Mathf.Deg2Rad);
                
                vec[i] = trans.position + new Vector3(x, y, 0);
                //if (i% speed==0) {
                //    ccq.Push(vec[i]+ tmpV3);
                //}
            }
        }

        //设置线由多少个点构成  
        line.positionCount = angle;
        //绘制点的坐标  
        line.SetPositions(vec);
    }

    public override void Init()
    {
        base.Init();
        isShowEd = false;
        InitRect(copy(ppe));
    }

    static GameObject topPanel;
    Material paintPenMat;
    public override void InitCompent(int pageNum)
    {
        trans = transform;
        paintPenMat = Resources.Load("Materials/paintPen") as Material;
        gameObject.layer = 12;
        slide = trans.parent;
        this.pageNum = pageNum;
        if (Particles == null)
        {
            topPanel = GameObject.Find("ControllerCanvas").GetComponent<PPTResourcePool>().paintPenCanvas;
            //Particles = GameObject.Find("ParticleDrawLine").GetComponent<ParticleSystem>();
            GameObject tmp = GameObject.Find("ParticleDrawLine");
            tmp = GameObject.Instantiate(tmp, tmp.transform.parent);
            tmp.name = "ParticleDrawLine_" + pageNum + "_" + ppe.id;
            Particles = tmp.GetComponent<ParticleSystem>();
            LineCamera = GameObject.Find("TopCamera").GetComponent<Camera>();
        }
    }

    public override void InitRect<T>(T t)
    {
        
        PPTGlobal.SLIDE_WIDTH = Screen.width;
        PPTGlobal.SLIDE_Height = Screen.height;
            t = transXY(t);
            t = transWH(t);
        if (ppe.type == "ELLIPSE") {
            w = t.w;
            h = t.h;
            float xx = w / 2f * Mathf.Cos(0 * Mathf.Deg2Rad);
            posX = t.x + w - xx;
            posY = t.y;
        } else if (ppe.type == "LINE") {
            posX = t.x;
            posY = t.y;
            startPoint = new Vector3(t.x + t.w / 2, t.y + t.h / 2, 0);
            endPoint = new Vector3(t.x - t.w / 2, t.y - t.h / 2, 0);
        }
    }
    public ParticleSystem Particles;
    public static Camera LineCamera;

    public void showParticles(Vector3 tmp) {
        //粒子画线过程
        //LineCamera.transform.TransformPoint(tmp);
        if (ppe.type == "ELLIPSE")
        {
            Particles.transform.localPosition = tmp+ trans.localPosition;
        }
        else if(ppe.type == "LINE"){
            Particles.transform.localPosition = tmp;
        }
        //Particles.transform.position = LineCamera.ScreenToWorldPoint(tmp);
    }

    bool isParticlesPlaying = false;
    public void statParticles() {
        Debug.Log("statParticles");
        Particles.Play();
        isParticlesPlaying = true;
    }
    public void stopParticles(){
        Particles.Stop();
        Particles.Clear();
        Particles.transform.position = Vector3.up * 1000;
        isParticlesPlaying = false;
    }
    Vector3 startPoint, endPoint;
    float posX,posY, posXx, posYy;
    public override void Pause()
    {
        if (isParticlesPlaying) {
            Particles.Pause();
        }
    }

    void Update()
    {
        
        if (PPTGlobal.pptStatus == PPTGlobal.PPTStatus.play) {
            if (isParticlesPlaying) {

                if (index >= vec.Length)
                {
                    index = 0;
                    GoDie(0);
                }
                    showParticles(vec[index]);
                if (ppe.type == "ELLIPSE")
                {
                    index+=4;
                }
                else {
                    index ++;
                }
                //index+=4;
            }
        }
    }
}

public class MyCircleQueue<T>
{
    /// <summary>  
    /// 队列数组  
    /// </summary>  
    private T[] _queue;
    /// <summary>  
    /// 队首索引  
    /// </summary>  
    private int _front;
    /// <summary>  
    /// 队尾索引  
    /// </summary>  
    private int _rear;

    /// <summary>  
    /// 队列的内存大小，但实际可用大小为_capacity-1  
    /// </summary>  
    private int _capacity;

    public MyCircleQueue(int queueSize)
    {
        if (queueSize < 1)
            throw new IndexOutOfRangeException("传入的队列长度不能小于1。");

        //设置队列容量  
        _capacity = queueSize;

        //创建队列数组  
        _queue = new T[queueSize];

        //初始化队首和队尾索引  
        _front = _rear = 0;
    }

    /// <summary>  
    /// 添加一个元素  
    /// </summary>  
    /// <param name="item"></param>  
    public void Push(T item)
    {
        //队列已满  
        if (GetNextRearIndex() == _front)
        {
            //扩大数组  
            T[] newQueue = new T[2 * _capacity];

            if (newQueue == null)
                throw new ArgumentOutOfRangeException("数据容量过大，超出系统内存大小。");
            //队列索引尚未回绕  
            if (_front == 0)
            {
                //将旧队列数组数据转移到新队列数组中  
                Array.Copy(_queue, newQueue, _capacity);
            }
            else
            {
                //如果队列回绕，刚需拷贝再次，  
                //第一次将队首至旧队列数组最大长度的数据拷贝到新队列数组中  
                Array.Copy(_queue, _front, newQueue, _front, _capacity - _rear - 1);
                //第二次将旧队列数组起始位置至队尾的数据拷贝到新队列数组中  
                Array.Copy(_queue, 0, newQueue, _capacity, _rear + 1);
                //将队尾索引改为新队列数组的索引  
                _rear = _capacity + 1;
            }

            _queue = newQueue;
            _capacity *= 2;
        }

        //累加队尾索引，并添加当前项  
        _rear = GetNextRearIndex();
        _queue[_rear] = item;
    }

    /// <summary>  
    /// 获取队首元素  
    /// </summary>  
    /// <returns></returns>  
    public T FrontItem()
    {
        if (IsEmpty())
            throw new ArgumentOutOfRangeException("队列为空。");
        Debug.Log(GetNextFrontIndex());
        return _queue[GetNextFrontIndex()];
    }

    /// <summary>  
    /// 获取队尾元素  
    /// </summary>  
    /// <returns></returns>  
    public T RearItem()
    {
        if (IsEmpty())
            throw new ArgumentOutOfRangeException("队列为空。");

        return _queue[_rear];
    }

    /// <summary>  
    /// 弹出一个元素  
    /// </summary>  
    /// <returns></returns>  
    public T Pop()
    {
        if (IsEmpty())
            throw new ArgumentOutOfRangeException("队列为空。");

        _front = GetNextFrontIndex();
        return _queue[_front];
    }

    /// <summary>  
    /// 队列是否为空  
    /// </summary>  
    /// <returns></returns>  
    public bool IsEmpty()
    {
        return _front == _rear;
    }
    /// <summary>  
    /// 获取下一个索引  
    /// </summary>  
    /// <returns></returns>  
    private int GetNextRearIndex()
    {
        if (_rear + 1 == _capacity)
        {
            return 0;
        }
        return _rear + 1;
    }

    /// <summary>  
    /// 获取下一个索引  
    /// </summary>  
    /// <returns></returns>  
    private int GetNextFrontIndex()
    {
        if (_front + 1 == _capacity)
        {
            _front = 0;
            return _front;
        }
        _front++;
        return _front + 1;
    }
}
