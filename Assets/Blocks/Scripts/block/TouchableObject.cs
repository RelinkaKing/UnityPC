using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchableObject : MonoBehaviour
{
    DraggableObject dragObject;
    RotatableObject rotateObject;
    private Vector3 destination = Vector3.zero;
    public TouchableObject antagonistic_object;
    TouchableObject landmarks;
    TouchableObject arterySupply;
    TouchableObject innervation;
    TouchableObject lymphaticdrainge;
    bool cancelDye = false;//是否取消染色
    bool available = true;
    Color rootColor;//原本的颜色
    public bool isSpecial = false;//标志是否是特殊属性，比如髋骨的髂骨，耻骨坐骨等等有共同父物体的结构，需要同时隐藏，透明和显示
    public float autoZoomDistance = 0;//自动缩放的距离
    public MeshRenderer m_renderer;//保存自己的渲染器
    public Material m_Material;//要保存自己的初始Material
    public Shader rootShader;//保存自己的初始shader
    public Texture rootTexture;//保存自己的初始Texture
    public bool noChangingState = false;//标志是否禁止切换状态，比如骨单位场景就要这么做
    //----------------------以下与区域计算有关-------------------------
    protected Vector3 max;
    protected Vector3 min;
    //取包围盒xyz轴到core的绝对值的最大值为距离
    public float x1;
    public float x2;

    public float y1;
    public float y2;

    public float z1;
    public float z2;
    //------------------区域计算相关变量到此为止----------------------
    public string key = "";//summary的key

    public SuperGameManager.States state = SuperGameManager.States.Show;
    public Transform m_transform;
    //是否被选中
    public bool isSelected = false;
    //是否透明显示
    public bool isTransparent = false;
    //是否隐藏
    public bool show = true;
    //是否滞留
    //public bool isSticky = false;
    //是否是节点
    public bool isNode = false;
    public float alpha = 1f;

    // Use this for initialization
    void Start()
    {
        m_transform = this.transform;
        //SuperGameManager.Instance.objectsAll.Add(this);
        if (m_transform.GetComponent<Renderer>() != null)
        {
            m_renderer = GetComponent<MeshRenderer>();
            m_Material = m_renderer.material;
            rootShader = m_Material.shader;
            rootColor = m_renderer.material.color;//原本的颜色
            rootTexture = m_Material.mainTexture;

        }


        if (m_transform.childCount == 0)
        {
            isNode = true;


            max = m_transform.GetComponent<Collider>().bounds.max;
            min = m_transform.GetComponent<Collider>().bounds.min;
        }
        if (string.IsNullOrEmpty(key))
        {
            //在这里进行智能分析，目的是适配老版的肌肉注解
            string temp = this.name;
            char[] chars = temp.ToCharArray();
            if (string.Compare(chars[this.name.Length - 1].ToString(), "L") == 0)//如果后缀是L
            {
                temp = temp.Remove(temp.Length - 2);
                key = "I" + temp;
            }
            else if (string.Compare(chars[this.name.Length - 1].ToString(), "R") == 0)//如果后缀是R
            {
                temp = temp.Remove(temp.Length - 2);
                key = "I" + temp;
            }
            else//没有左右之分的结构
            {
                key = "I" + name;
            }

        }

        //取包围盒xyz轴到core的绝对值的最大值为距离
        x1 = max.x;
        x2 = min.x;

        y1 = max.y;
        y2 = min.y;

        z1 = max.z;
        z2 = min.z;
        //如果这个模型包含DragObject组件，则在这里初始化
        dragObject = GetComponent<DraggableObject>();
        if (dragObject != null)
        {
            dragObject.target = m_transform;
            dragObject.dragMovement = Vector3.one;
            //Make sure the object isn't draggable at start
            SetDraggable(false);
        }
        rotateObject = GetComponent<RotatableObject>();
        if (rotateObject != null)
        {
            rotateObject.target = m_transform;
            rotateObject.rotateMovement = 1f;
            //Make sure the object isn't rotatable at start
            SetRotatable(false);
        }
    }
    public string GetSummary()
    {
        if (!string.IsNullOrEmpty(key))
        {
            char[] chars = { ',' };
            string[] tempsa = key.Split(chars);
            string sumary = "";
            for (int i = 0; i < tempsa.Length; i++)
            {
                if (Localization.Get(tempsa[i]).Equals(key))
                {
                    return " ";
                }
                sumary += Localization.Get(tempsa[i]) + "\n" + "\n";
            }
            return sumary;
        }
        return null;
    }
    public string GetNote()
    {
        string note = PlayerPrefs.GetString(this.name);
        if (!string.IsNullOrEmpty(note))
        {
        }
        else//如果是空的
        {
            note = "该结构没有添加过笔记！";
        }
        return note;
    }
    public string GetModelName()
    {
        char[] chars = { '_' };
        string[] tempsa = this.name.Split(chars);
        string modelName = "";
        if (string.Compare(tempsa[tempsa.Length - 1], "L") == 0)//如果后缀是L，那么加上本地化的左
        {
            string temp = "";
            temp = this.name.Substring(0, this.name.Length - 2);//取去掉后缀后的名称
            modelName = Localization.Get(temp) + Localization.Get("L");
        }
        else if (string.Compare(tempsa[tempsa.Length - 1], "R") == 0)//如果后缀是R，那么加上本地化的右
        {
            string temp = "";
            temp = this.name.Substring(0, this.name.Length - 2);//取去掉后缀后的名称
            modelName = Localization.Get(temp) + Localization.Get("R");
        }
        else if (string.Compare(tempsa[tempsa.Length - 1], "L half") == 0)//如果后缀是L half，那么加上本地化的左半
        {
            string temp = "";
            temp = this.name.Substring(0, this.name.Length - 7);//取去掉后缀后的名称
            modelName = Localization.Get(temp) + Localization.Get("L half");
        }
        else if (string.Compare(tempsa[tempsa.Length - 1], "R half") == 0)//如果后缀是R half，那么加上本地化的右半
        {
            string temp = "";
            temp = this.name.Substring(0, this.name.Length - 7);//取去掉后缀后的名称
            modelName = Localization.Get(temp) + Localization.Get("R half");
        }
        else//没有左右之分的结构
        {
            modelName = Localization.Get(this.name);
        }
        return modelName;
    }

    public string ModelName;
    public float timer=0;
    public bool start_timer = false;
    public const float TargetTimer = 1f;

    void OnPress(bool pressed)
    {
        if (enabled && NGUITools.GetActive(gameObject) && dragObject.target != null)
        {
            //点击进入
            if (pressed)
            {
                timer = 0;
                start_timer = true;
                rotateObject.isRotating=false;
                DebugLog.DebugLogInfo("00ff00", "pressed " + pressed);
            }
            else 
            {
                if (timer > TargetTimer && isSelected && !rotateObject.isRotating )
                {
                    this.GetComponent<Block>().DoubleClick();
                    DebugLog.DebugLogInfo("00ff00", "pressed " + pressed);
                }
            }
        }
    }

    private void Update()
    {
        if (start_timer)
        {
            timer += Time.deltaTime;
        }
    }

    void OnClick()
    {
        int id = UICamera.currentTouchID;
        if (id == 1 && !noChangingState)//如果是第二指
        {
            //if (isSpecial)//如果是特殊的
            //{
            //    m_transform.parent.GetComponent<TouchableObject>().HideGroup(false);//調用它的父級進行整體hide
            //}
            //else
            //{
            //    Hide(false);//否則直接单体隐藏就行
            //}

            Transform st = SuperGameManager.Instance.selectedObjects[SuperGameManager.Instance.selectedObjects.Count - 1];//新代码

            return;
        }

        if (id == 0 || id == -1)//如果是第一根指头单击或者是鼠标左键单击
        {
            if (!SuperGameManager.Instance.validClickRequest)
            {
                SuperGameManager.Instance.validClickRequest = true;
                return;
            }


            if (SuperGameManager.Instance.validClickRequest)//非长按模式，那就是单选
            {
                SuperGameManager.Instance.ResetAllObjects();
            }
            SuperGameManager.Instance.selectedObjects.Add(m_transform);
            Selected(false);

            GameCamera.Inst.jiaodian = UICamera.lastWorldPosition;
            SuperGameManager.Instance.core = UICamera.lastWorldPosition;
            destination = UICamera.lastWorldPosition;
            CameraPoint.Instance.SetDestination(destination);
        }
    }



    void OnDrag(Vector2 delta)
    {


        //---------------------------------------------------------------------
        int count = UICamera.touchCount;//检测输入手指的数量，单指还是双指
        int id = UICamera.currentTouchID;
        if (count == 1)
        {
            //如果处于搭积木模式并且自身处于被选中状态
            if (SuperGameManager.Instance.transformType != TransformTools.Type.Select && isSelected)
            {

                return;
            }
            if (id == -1 || id == 0)
            {
                SuperGameManager.Instance.buttonPressed = false;
                //SuperGameManager.Instance.validClickRequest = false;//这里需要注意下
                //旋转摄像机
                CameraPoint.Instance.m_rot.x -= delta.y * GameCamera.Inst.m_rotateSpeed * 0.02f;
                CameraPoint.Instance.m_rot.y += delta.x * GameCamera.Inst.m_rotateSpeed * 0.02f;
                CameraPoint.Instance.transform.eulerAngles = CameraPoint.Instance.m_rot;
                // Debug.Log("第一根手指按下");
            }

            else if (id == -3)//鼠标中键平移
            {
                GameCamera.Inst.m_cameraPoint.Translate(delta.x * GameCamera.Inst.m_panSpeed * (GameCamera.Inst.m_distance / 330) * 0.02f, delta.y * GameCamera.Inst.m_panSpeed * (GameCamera.Inst.m_distance / 330) * 0.02f, 0);
                // Debug.Log("GameCamera的平移速度" + GameCamera.Inst.m_panSpeed);
            }
        }
        else if (count == 2)//双指情况下
        {
            //for (int i = 0; i < GameCamera.Inst.pd.Count; i++)
            //{
            //    GameCamera.Inst.pd[i].SetLocation(UICamera.GetTouch(i).pos.x, UICamera.GetTouch(i).pos.y);
            //}

            //if (GameCamera.Inst.pd.Count == 2)//若当前有两个触控点按下
            //{
            //    float flag = UICamera.GetTouch(0).delta.x * UICamera.GetTouch(1).delta.x + UICamera.GetTouch(0).delta.y * UICamera.GetTouch(1).delta.y;//我觉得在这里如果在加个模的判断会比较好

            //    if (flag > 0 || flag == 0)//说明方向相同，开启平移模式
            //    {
            //        GameCamera.Inst.m_cameraPoint.Translate(UICamera.GetTouch(1).delta.x * GameCamera.Inst.m_panSpeed * (GameCamera.Inst.m_distance / 330) * 0.02f, UICamera.GetTouch(1).delta.y * GameCamera.Inst.m_panSpeed * (GameCamera.Inst.m_distance / 330) * 0.02f, 0);
            //    }
            //    if (flag < 0 || flag == 0)//说明方向相同，开启平移模式
            //    {
                    //SuperPoint sp1 = GameCamera.Inst.pd[0];//从map中取出第一个触控点
                    //SuperPoint sp2 = GameCamera.Inst.pd[1];//从map中取出第二个触控点
                    //float currDis = SuperPoint.calDistance(sp1, sp2);//计算两点距离
                    //if (Mathf.Abs(currDis - GameCamera.Inst.distance) >= 4)//大于一定阈值才执行，试试看这个能不能防抖
                    //{
                    //    GameCamera.Inst.m_distance -= (currDis - GameCamera.Inst.distance) * GameCamera.Inst.m_scaleSpeed * (GameCamera.Inst.m_distance / 330) * Time.deltaTime;
                    //    GameCamera.Inst.distance = currDis;
                    //    GameCamera.Inst.Follow();
                    //}

            //    }

            //}
        }
    }
    void OnScroll(float delta)
    {
        if (delta != 0)
        {
            Debug.Log("delta" + delta);
            GameCamera.Inst.m_distance -= delta * 6000f * (GameCamera.Inst.m_distance / 330) * 0.02f;

            GameCamera.Inst.Follow();
        }
    }

    public float CalculateDistance(Vector3 core)
    {
        float x = Mathf.Abs(x1 - core.x) < Mathf.Abs(x2 - core.x) ? Mathf.Abs(x1 - core.x) : Mathf.Abs(x2 - core.x);
        float y = Mathf.Abs(y1 - core.y) < Mathf.Abs(y2 - core.y) ? Mathf.Abs(y1 - core.y) : Mathf.Abs(y2 - core.y);
        float z = Mathf.Abs(z1 - core.z) < Mathf.Abs(z2 - core.z) ? Mathf.Abs(z1 - core.z) : Mathf.Abs(z2 - core.z);

        float finalDistance = x > y ? x : y;
        finalDistance = finalDistance > z ? finalDistance : z;

        return finalDistance;
    }


    public void CancelDye()
    {
        cancelDye = true;
    }

    public void Solid(bool add)//实体化
    {
        alpha = 1;
        isTransparent = false;
        state = SuperGameManager.States.Show;
        if (m_renderer != null)//如果他有着色器，其实也就代笔他是子物体。。。
        {
            m_transform.gameObject.layer = LayerMask.NameToLayer("Default");
            m_renderer.enabled = true;
            if (isSelected)//如果是选中状态，那么绿色高亮显示
            {
                if (cancelDye)//判断是否取消染色，如果是取消染色的状态，那么和没选中的显示情况一样。。。
                {
                    Reset(false);
                }
                else
                {
                    //m_renderer.material = SuperGameManager.Instance.lightning;
                    m_renderer.material.mainTexture = SuperGameManager.Instance.highLightTexture;
                    m_renderer.material.shader = rootShader;

                    //显示对应线框
                    Block_loadmodel.Instance.SetWireMeshWithName(this.name,true);
                }
            }
            else//如果是非选中状态
            {
                m_renderer.material = m_Material;
                m_renderer.material.mainTexture = rootTexture;
                m_renderer.material.shader = rootShader;
                m_renderer.material.color = rootColor;

                Block_loadmodel.Instance.SetWireMeshWithName(this.name, false);
            }
            if (add)
            {
                if (!SuperGameManager.Instance.objectsInScene.Contains(this))//如果objectsInScene还不包含这个元素，那就加进去
                {
                    SuperGameManager.Instance.objectsInScene.Add(this);
                    SuperGameManager.Instance.objectsInSceneTemp.Add(this);
                }

                if (antagonistic_object != null)//如果这个结构有对应的拮抗体
                {
                    antagonistic_object.Hide(true);
                }
            }
        }
    }

    public void SolidGroup(bool add)//凡是group的方法都是提供给node的父级使用的，来整体控制
    {
        isTransparent = false;
        state = SuperGameManager.States.Show;
        for (int i = 0; i < m_transform.childCount; i++)
        {
            m_transform.GetChild(i).GetComponent<TouchableObject>().Solid(add);
        }
    }

    public void SuperShow(bool show)
    {
        if (show)//如果是show状态
        {
            if (m_renderer != null)//如果他有着色器，其实也就代笔他是子物体。。。
            {
                m_transform.gameObject.layer = LayerMask.NameToLayer("Default");
                m_renderer.enabled = true;

            }
            if (isTransparent)
            {
                state = SuperGameManager.States.Fade;
            }
            else
            {
                state = SuperGameManager.States.Show;
            }
        }
        else//如果是隐藏
        {
            if (m_renderer != null)
            {
                m_transform.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                m_renderer.enabled = false;
                show = false;
            }
            //state = SuperGameManager.States.Hide;//被遮罩遮住并不是真正的隐藏！！！
        }
    }

    public void Fade()//透明化
    {
        //alpha = alpha / 2;
        isTransparent = true;
        state = SuperGameManager.States.Fade;
        if (m_renderer != null)
        {
            m_transform.gameObject.layer = LayerMask.NameToLayer("Default");
            if (isSelected)//如果是选中状态的透明化，那么用lightning_fade材质
            {
                if (cancelDye)//是否取消染色
                {
                    Reset(false);
                }
                else
                {
                    m_renderer.material.shader = SuperGameManager.Instance.transparent;
                    m_renderer.material.mainTexture = SuperGameManager.Instance.highLightTexture;
                    //m_renderer.material = SuperGameManager.Instance.lightning_fade;
                    m_renderer.material.color = new Color(1, 1, 1, alpha / 2);
                }
            }
            else//如果是非选中状态的透明化
            {
                //m_renderer.material = m_Material;
                //m_renderer.material.shader = SuperGameManager.Instance.transparent;
                //m_renderer.material.color = new Color(1, 1, 1, alpha/2);
                //m_renderer.material = m_Material;
                m_renderer.material.shader = SuperGameManager.Instance.transparent;
                m_renderer.material.mainTexture = rootTexture;
                //m_renderer.material.color = new Color(1, 1, 1, alpha/2);
                m_renderer.material.color = new Color(rootColor.r, rootColor.g, rootColor.b, alpha / 2);
            }
            m_renderer.enabled = true;
        }
    }
    public void FadeGroup()
    {
        isTransparent = true;
        state = SuperGameManager.States.Fade;
        for (int i = 0; i < m_transform.childCount; i++)
        {
            m_transform.GetChild(i).GetComponent<TouchableObject>().Fade();
        }
    }
    public void Hide(bool remove)//隐藏方法，分为（需不需要排除）两种情况
    {
        if (m_renderer != null)
        {
            m_transform.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            m_renderer.enabled = false;
        }
        state = SuperGameManager.States.Hide;
        if (remove)
        {
            if (SuperGameManager.Instance.objectsInScene.Contains(this))//如果objectsInScene包含这个元素，那就移除
            {
                SuperGameManager.Instance.objectsInScene.Remove(this);
                SuperGameManager.Instance.objectsInSceneTemp.Remove(this);
            }
        }
    }

    public void HideGroup(bool remove)
    {
        state = SuperGameManager.States.Hide;
        for (int i = 0; i < m_transform.childCount; i++)
        {
            m_transform.GetChild(i).GetComponent<TouchableObject>().Hide(remove);
        }
    }


    public void Selected(bool add)//选中
    {
        if (dragObject != null)
        {
            SetDraggable(true);
        }
        if (rotateObject != null)
        {
            SetRotatable(true);
        }
        isSelected = true;
        cancelDye = false;
        Solid(add);

        GameCamera.Inst.Wizard(m_transform);

        SelectedLabel.instance.ShowSelectedName();
        //----------------------------------------------------------------------------
    }

    public void Reset(bool real)//分真假reset，真reset就是纯粹的reset，没选中，而假reset，实际上仍是选中状态，只不过取消染色状态，看起来似乎是和真reset了一样
    {
        if (dragObject != null)
        {
            SetDraggable(false);
        }
        if (rotateObject != null)
        {
            SetRotatable(false);
        }
        if (real)
        {
            isSelected = false;
        }
        if (m_renderer != null)
        {
            if (isTransparent)
            {
                m_renderer.material = m_Material;
                m_renderer.material.mainTexture = rootTexture;
                m_renderer.material.shader = SuperGameManager.Instance.transparent;
                //m_renderer.material.color = new Color(1, 1, 1, alpha/2);
                m_renderer.material.color = new Color(rootColor.r, rootColor.g, rootColor.b, alpha / 2);
            }
            else
            {
                m_renderer.material = m_Material;
                m_renderer.material.mainTexture = rootTexture;
                m_renderer.material.shader = rootShader;
                m_renderer.material.color = rootColor;

                Block_loadmodel.Instance.SetWireMeshWithName(this.name, false);
            }
        }
    }


    public void SetDraggable(bool draggable)
    {
        if (draggable && UICamera.hoveredObject == gameObject)
        {
            dragObject.dragMovement = Vector3.one;

        }
        else
        {
            //Disable the UIDragObject's movement
            dragObject.dragMovement = Vector3.zero;

        }
    }
    public void SetRotatable(bool rotatable)
    {
        if (rotatable && UICamera.hoveredObject == gameObject)
        {
            rotateObject.rotateMovement = 1f;
            //rotateObject.enabled = false;
        }
        else
        {
            rotateObject.rotateMovement = 0f;
            //rotateObject.enabled = true;

        }
    }
}