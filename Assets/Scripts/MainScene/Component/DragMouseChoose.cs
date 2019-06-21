using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

//鼠标选中功能
//模型高亮 与 恢复  实时
//鼠标范围限制，跟随手指移动
[Hotfix]
public class DragMouseChoose : MonoBehaviour
{

    [Header("DragUI_common")] public bool OnlyDray = true;
    float offsetX;
    float offsetY;
    float a = 0.75f;
    InputField tempfiled;
    float x;
    float y;
    private bool _rayResult = false;
    Model LastModel = null;
    Model currentModel;
    // [ConditionalHide("OnlyDray",false)]public Vector3 offsetv{get{return  new Vector3(x,y,0);}}
    // [ConditionalHide("OnlyDray",false)]public Button ChooseBtn;
    // [ConditionalHide("OnlyDray",false)]public Text modelName;
    // [ConditionalHide("OnlyDray",false)]public float offset=1;
    // [ConditionalHide("OnlyDray",false)]public RaycastHit RayInfo;
    // [ConditionalHide("OnlyDray",false)]public Camera UIcamera;
    public Vector3 offsetv { get { return new Vector3(x, y, 0); } }
    public Button ChooseBtn;
    public Text modelName;
    public float offset = 1;
    public RaycastHit RayInfo;
    public Camera UIcamera;
    [LuaCallCSharp]
    private void Start()
    {
//        PublicClass.IsEnterMouseMoudel = true;
//        PublicClass.AllowRecordCommand = false;
        UIcamera = GameObject.Find("UICamera").GetComponent<Camera>();
    }

    [LuaCallCSharp]
    public void ChooseModel()
    {
        // PublicClass.currentModel = currentModel;
        // if (!PublicClass.isMultiple)
        // {
        //     //如果时第一次点击，则上一个model为空
        //     if (PublicClass.lastModel == null)
        //     {
        //         PublicClass.lastModel = currentModel;//记录
        //     }
        //     else
        //     {
        //         PublicClass.lastModel.BecomeNormal();
        //         //覆盖上一个材质
        //         PublicClass.lastModel = PublicClass.currentModel;                      //完成记录
        //     }
        // }
        // else
        //     PublicClass.currentModels.Add(PublicClass.currentModel);
        // PublicClass.currentModel.BecomeHight();       //更换当前材质  
        // XT_TouchContorl.Instance.GiveUiValue_new(PublicClass.currentModel);
    }
    [LuaCallCSharp]
    public void BeginDrag()
    {
        XT_MouseFollowRotation.instance.IsRotate = false;
        PublicClass.currentState = RunState.UI;
        offsetX = transform.localPosition.x - Input.mousePosition.x;
        offsetY = transform.localPosition.y - Input.mousePosition.y;
    }
    [LuaCallCSharp]
    public void EndDrag()
    {
        XT_MouseFollowRotation.instance.IsRotate = true;
        PublicClass.currentState = RunState.Playing;
        PublicClass.currentModle = ModleChoose.MainModel;
 //       PublicClass.IsEnterMouseMoudel = false;
    }
    [LuaCallCSharp]
    public void GetMoveSpeedInput()
    {
        tempfiled = GameObject.Find("InputField_move").GetComponent<InputField>();
    }
    [LuaCallCSharp]
    public void getValueInput()
    {
        GetMoveSpeedInput();
        a = float.Parse(tempfiled.text); 
    }
    [LuaCallCSharp]
    public void OnDrag()
    {

        // //使用Lerp方法实现 这里的Time.deltaTime是指移动速度可以自己添加变量方便控制  
        // this.transform.position= Vector3.Lerp(this.transform.position,dis,Time.deltaTime);  
        // //使用MoveTowards方法实现，这个方法是匀速运动  
        // this.transform.position = Vector3.MoveTowards(this.transform.position, dis, Time.deltaTime);  
        // //使用SmoothDamp方式实现,给定时间可以获取到速度  
        // Vector3 speed = Vector3.zero;  
        // this.transform.position = Vector3.SmoothDamp(this.transform.position, dis,ref speed, 0.1f);  
        // Debug.Log(speed);  

        Vector2 _pos = Vector2.one;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform,
                    Input.mousePosition, UIcamera, out _pos);
        Debug.Log("pos:" + _pos);
        transform.localPosition = new Vector3(offsetX + Input.mousePosition.x, offsetY + Input.mousePosition.y, 0);

        if (!OnlyDray)
        {
            x = UIcamera.WorldToScreenPoint(this.transform.position).x;
            y = UIcamera.WorldToScreenPoint(this.transform.position).y;
            //发射射线，使模型高亮
            //发射射线
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
            _rayResult = Physics.Raycast(ray, out RayInfo);
            if (_rayResult)
            {
                Model rayTarget = RayInfo.transform.GetComponent<Model>();
                if (rayTarget != null)
                {
                    currentModel = rayTarget;
                    if (LastModel == null)
                    {
                        LastModel = rayTarget;
                    }
                    else
                    {
                        if (LastModel.isSeleted)
                            LastModel.BecomeHight();
                        else if (LastModel.isTranslucent)
                            LastModel.BecomeTranslucent();
                        else
                            LastModel.BecomeNormal();
                        LastModel = currentModel;
                    }
                    rayTarget.BecomeMouseColor();
                    // modelName.text=LoadModel.instance.notes[rayTarget.name].chinese;
                }
            }
            else
            {
                modelName.text = string.Empty;
                if (currentModel != null)
                {
                    if (currentModel.isSeleted)
                        currentModel.BecomeHight();
                    else if (currentModel.isTranslucent)
                        currentModel.BecomeTranslucent();
                    else
                        currentModel.BecomeNormal();
                }
            }
            ChooseBtn.gameObject.SetActive(_rayResult);
            modelName.gameObject.transform.parent.gameObject.SetActive(_rayResult);
        }

    }
    private void OnDestroy()
    {
        if (currentModel != null)
        {
            currentModel.BecomeNormal();
        }
        if (LastModel != null)
        {
            LastModel.BecomeNormal();
        }
//        PublicClass.IsEnterMouseMoudel = false;
//        PublicClass.AllowRecordCommand = true;
    }
}