using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    //最终位置
    //[HideInInspector]
    public Vector3 targetPosition;

    //最终姿态
    //[HideInInspector]
    public Quaternion targetRotation;
    public Vector3 targetAngles;
    //[HideInInspector]
    public Vector3 targetRight;
    //[HideInInspector]
    public Vector3 targetUp;
    //[HideInInspector]
    public Vector3 targetForward;
    public bool isFinished
    {
        get { return isfinished; }
        set
        {
            isfinished = value;
            if (isfinished == true) {
                Block_loadmodel.Instance.SetWireMeshWithName(this.name, false);
                Debug.Log("one  model is conplement");
            }
        }
    }//是否已经归位

    private bool isfinished = false;
    Transform m_transform;

    private void Start()
    {
        m_transform = this.transform;
    }

    void OnDrag(Vector2 delta)
    {
        if (isFinished)//如果已经归位，到此中断
        {
            return;
        }
        //在拖动时检查离最终位置的距离
        CheckPosition(BlockManager.Instance.Difficulty);
        // Debug.Log("现在的难度是：" + BlockManager.Instance.Difficulty);        
        //Debug.Log("现在的距离是：" + Vector3.Distance(m_transform.localPosition, targetPosition));
        // Debug.Log("现在的角度差是x:" + Vector3.Angle(m_transform.right, targetRight) + "y:" +
        //    Vector3.Angle(m_transform.up, targetUp ) + "z:" +
        //    Vector3.Angle(m_transform.forward, targetForward ));

        int count = UICamera.touchCount;//检测输入手指的数量，单指还是双指
        int id = UICamera.currentTouchID;
        if (count == 1)
        {
            if (id == -1 || id == 0) //Debug.Log("第一根手指按下");
            {
                BlockManager.Instance.SetToggleModel(false);
            }
            else if (id == -3)//鼠标中键平移
            {

            }
        }
        else if (count == 2)//双指情况下
        {
            BlockManager.Instance.SetToggleModel(true);
        }
    }

    private void CheckPosition(BlockManager.Difficulties difficulty)
    {
        switch (difficulty)
        {
            case BlockManager.Difficulties.Easy:
                checkCollison(BlockManager.Instance.easy_Pos, BlockManager.Instance.easy_Ang);
                break;
            case BlockManager.Difficulties.Normal:
                checkCollison(BlockManager.Instance.normal_Pos, BlockManager.Instance.normal_Ang);
                break;
            case BlockManager.Difficulties.Hard:
                checkCollison(BlockManager.Instance.hard_Pos, BlockManager.Instance.hard_Ang);
                break;
        }
    }
    private void checkCollison(float positionThreshold, float angleThreshold)
    {
        float distance = Vector3.Distance(m_transform.localPosition, targetPosition);
        if (distance <= positionThreshold)
        {
            print("position ok");
        }
        if (distance <= positionThreshold &&
            Vector3.Angle(m_transform.right, targetRight) < angleThreshold &&
            Vector3.Angle(m_transform.up, targetUp) < angleThreshold &&
            Vector3.Angle(m_transform.forward, targetForward) < angleThreshold
            )//判断为达到正确位置
        {
            TouchableObject tobj = GetComponent<TouchableObject>();
            tobj.SetDraggable(false);
            tobj.SetRotatable(false);
            TweenRotation tween = TweenRotation.Begin(gameObject, 0.5f, targetRotation);
            tween.method = UITweener.Method.EaseInOut;
            UITweener tweener = TweenPosition.Begin(gameObject, 0.5f, targetPosition);
            tweener.method = UITweener.Method.EaseInOut;
            //TweenRotation tween = GetComponent<TweenRotation>();
            tween.from = m_transform.localEulerAngles;

            tween.to = targetAngles;
            float y = tween.from.y;
            if (Mathf.Abs(360 - tween.from.y) < Mathf.Abs(tween.from.y - 0))
            {
                //tween.from = new Vector3(tween.from.x, tween.from.y - 360, tween.from.z);
                tween.to = new Vector3(tween.to.x, tween.to.y + 360, tween.to.z);
            }
            Debug.Log("from:" + tween.from + "to:" + tween.to);
            //tween.PlayForward();
            //BlockManager.Instance.AddScore();
            tween.SetOnFinished(BlockManager.Instance.AddScore);
            isFinished = true;//标志该结构已经归位
            //targetRotation.
        }

    }

    void OnDoubleClick()
    {
        DebugLog.DebugLogInfo("双击-----------------");
        if (BlockManager.Instance.availableBaseCount > 0)
        {
            DebugLog.DebugLogInfo("availableBaseCount--- " + BlockManager.Instance.availableBaseCount);
            BlockManager.Instance.availableBaseCount--;
            TouchableObject tobj = GetComponent<TouchableObject>();
            tobj.SetDraggable(false);
            tobj.SetRotatable(false);
            TweenRotation tween = TweenRotation.Begin(gameObject, 0.5f, targetRotation);
            DebugLog.DebugLogInfo("tween--- " + tween);
            tween.method = UITweener.Method.EaseInOut;
            UITweener tweener = TweenPosition.Begin(gameObject, 0.3f, targetPosition);
            DebugLog.DebugLogInfo("tweener--- " + tweener);
            tweener.method = UITweener.Method.EaseInOut;
            //TweenRotation tween = GetComponent<TweenRotation>();
            tween.from = m_transform.localEulerAngles;

            tween.to = targetAngles;
            float y = tween.from.y;
            if (Mathf.Abs(360 - tween.from.y) < Mathf.Abs(tween.from.y - 0))
            {
                //tween.from = new Vector3(tween.from.x, tween.from.y - 360, tween.from.z);
                tween.to = new Vector3(tween.to.x, tween.to.y + 360, tween.to.z);
            }
            DebugLog.DebugLogInfo("from:" + tween.from + "to:" + tween.to);
            tween.SetOnFinished(BlockManager.Instance.AddScore);
            //BlockManager.Instance.AddScore();
            isFinished = true;//标志该结构已经归位

            this.GetComponent<DraggableObject>().enabled = false;
            this.GetComponent<RotatableObject>().enabled = false;

        }
    }

    public void DoubleClick()
    {
        DebugLog.DebugLogInfo("双击-----------------");
        if (BlockManager.Instance.availableBaseCount > 0)
        {
            DebugLog.DebugLogInfo("availableBaseCount--- " + BlockManager.Instance.availableBaseCount);
            BlockManager.Instance.availableBaseCount--;
            TouchableObject tobj = GetComponent<TouchableObject>();
            tobj.SetDraggable(false);
            tobj.SetRotatable(false);
            TweenRotation tween = TweenRotation.Begin(gameObject, 0.5f, targetRotation);
            DebugLog.DebugLogInfo("tween--- " + tween);
            tween.method = UITweener.Method.EaseInOut;
            UITweener tweener = TweenPosition.Begin(gameObject, 0.3f, targetPosition);
            DebugLog.DebugLogInfo("tweener--- " + tweener);
            tweener.method = UITweener.Method.EaseInOut;
            //TweenRotation tween = GetComponent<TweenRotation>();
            tween.from = m_transform.localEulerAngles;

            tween.to = targetAngles;
            float y = tween.from.y;
            if (Mathf.Abs(360 - tween.from.y) < Mathf.Abs(tween.from.y - 0))
            {
                //tween.from = new Vector3(tween.from.x, tween.from.y - 360, tween.from.z);
                tween.to = new Vector3(tween.to.x, tween.to.y + 360, tween.to.z);
            }
            DebugLog.DebugLogInfo("from:" + tween.from + "to:" + tween.to);
            tween.SetOnFinished(BlockManager.Instance.AddScore);
            //BlockManager.Instance.AddScore();
            isFinished = true;//标志该结构已经归位

            this.GetComponent<DraggableObject>().enabled = false;
            this.GetComponent<RotatableObject>().enabled = false;

        }
    }
}
