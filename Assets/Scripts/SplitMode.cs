using UnityEngine;
using UnityEngine.UI;
using System;

public class SplitMode : MonoBehaviour
{


    //public static SplitMode _instance;

    public static bool isSpliteMode;
    //按钮图片
    //public Sprite[] orthogonalImage;
    void Awake()
    {

        PublicClass.splitmode = this;
        try
        {

        im_360.sprite = enable_360;
        }
        catch (System.Exception)
        {
        }
        // AllModel(GameObject.Find("temp_parent").transform);
    }

    public void closeMode()
    {
        split.sprite = splits[1];
        if (SceneModels.instance != null) {
            SceneModels.instance.set_Split_mode(false);
        }
        try
        {
            
        im_360.sprite = enable_360;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }

    public Image im_360;
    public Sprite enable_360;
    public Sprite disable_360;

    //拆分模式切换
    public void OpenSplitMode()
    {
        if (SceneModels.instance.get_Multi_Selection() == true) return;
        if (SceneModels.instance != null && SceneModels.instance.get_Split_mode())
        {
            try
            {
                
            im_360.sprite = enable_360;
            }
            catch (System.Exception)
            {
            }
            split.sprite = splits[1];
            SceneModels.instance.set_Split_mode(false);
            isSpliteMode = false;

        }
        else
        {
            try
            {
            im_360.sprite = disable_360;
            }
            catch (System.Exception)
            {

            }
            split.sprite = splits[0];
            SceneModels.instance.set_Split_mode(true);
            SceneModels.instance.CancleSelect();
            SceneModels.instance.set_Multi_Selection(false);
            isSpliteMode = true;
            //重置多选图标
            //Camera.main.GetComponent<XT_AllButton>().SwitchMultipleBtnUI(true);
        }
    }


    //重置模型位置
    public void ResetPosition()
    {

        closeMode();
    }

    public RaycastHit RayInfo2;
    private bool _rayResult2 = false;
    GameObject target = null;
    bool targetchange = true;
    private Vector3 screenPoint;
    private Vector3 offset;

    private void LateUpdate()
    {
        if (PublicClass.appstate != App_State.Running)
        {
            return;
        }
        //拆分模式判断
        if (SceneModels.instance != null && SceneModels.instance.get_Split_mode())
        {
            //发射射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _rayResult2 = Physics.Raycast(ray, out RayInfo2);
            //碰撞判断
            if (_rayResult2)
            {
                //拖动模型开始
                if (Input.GetMouseButtonDown(0))
                {
                    if (targetchange)
                    {
                        target = RayInfo2.transform.gameObject;
                        targetchange = false;
                        if (target != null)
                        {
                            SceneModels.instance.ChooseModel(target);
                            screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);
                            offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                        }
                    }
                }
                //触摸
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (targetchange)
                    {
                        target = RayInfo2.transform.gameObject;
                        targetchange = false;
                        if (target != null)
                        {
                            SceneModels.instance.ChooseModel(target);
                            screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);
                            offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, screenPoint.z));
                        }
                    }
                }

            }
            //拖动模型过程
            if (Input.GetMouseButton(0))
            {
                if (target != null)
                {
                    Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                    SpliteMove(curScreenPoint, offset);
                }
            }
            //触摸拖动
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (target != null)
                {
                    Vector3 curScreenPoint = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, screenPoint.z);
                    SpliteMove(curScreenPoint, offset);
                }
            }

            //拖动模型结束
            if (Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                target = null;
                targetchange = true;
            }
        }
    }
    public void SpliteMove(Vector3 v, Vector3 os)
    {
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(v) + os;
        target.transform.position = curPosition;
        SceneModels.instance.relationModelSplite(target.name, curPosition);
    }



    //拆分按钮图片
    public Sprite[] splits;
    //拆分按钮
    public Image split;
}