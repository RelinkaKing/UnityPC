using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour {

    void OnDrag(Vector2 delta)
    {
        
        //---------------------------------------------------------------------
        int count = UICamera.touchCount;//检测输入手指的数量，单指还是双指
        int id = UICamera.currentTouchID;
        if (count == 1)
        {
            if (id == -1 || id == 0)
            {
                SuperGameManager.Instance.buttonPressed = false;
                SuperGameManager.Instance.validClickRequest = false;
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
                //if (SuperGameManager.Instance.touchMode == SuperGameManager.TouchMode.None)//需要先判断
                //{
                //if (flag > 0)//说明方向相同，开启平移模式
                //{
                //SuperGameManager.Instance.touchMode = SuperGameManager.TouchMode.Pan;
                //}

                //else if (flag < 0 || flag == 0)//说明方向相反，开启缩放模式,这次我要把这里的代码解决掉！！！
                //{
                //SuperGameManager.Instance.touchMode = SuperGameManager.TouchMode.Zoom;
                //}
                //}
                //else if (SuperGameManager.Instance.touchMode == SuperGameManager.TouchMode.Pan)
                //{
                //if (flag > 0 || flag == 0)//说明方向相同，开启平移模式
                //{
                //    GameCamera.Inst.m_cameraPoint.Translate(UICamera.GetTouch(1).delta.x * GameCamera.Inst.m_panSpeed * (GameCamera.Inst.m_distance / 330) * 0.02f, UICamera.GetTouch(1).delta.y * GameCamera.Inst.m_panSpeed * (GameCamera.Inst.m_distance / 330) * 0.02f, 0);
                //}
                ////}
                ////else if (SuperGameManager.Instance.touchMode == SuperGameManager.TouchMode.Zoom)//说明方向相反，开启缩放模式,这次我要把这里的代码解决掉！！！
                //{
                //    if (flag < 0 || flag == 0)//说明方向相同，开启平移模式
                //    {
                //        SuperPoint sp1 = GameCamera.Inst.pd[0];//从map中取出第一个触控点
                //        SuperPoint sp2 = GameCamera.Inst.pd[1];//从map中取出第二个触控点
                //        float currDis = SuperPoint.calDistance(sp1, sp2);//计算两点距离
                //        if (Mathf.Abs(currDis - GameCamera.Inst.distance) >= 4)//大于一定阈值才执行，试试看这个能不能防抖
                //        {
                //            GameCamera.Inst.m_distance -= (currDis - GameCamera.Inst.distance) * GameCamera.Inst.m_scaleSpeed * (GameCamera.Inst.m_distance / 330) * Time.deltaTime;
                //            GameCamera.Inst.distance = currDis;
                //            GameCamera.Inst.Follow();
                //        }

                //    }
                //}
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

    void OnPress(bool pressed)
    {
        if (pressed)//按下
        {
            
            //要排除是双指或双指以上的模式
            if (!(UICamera.touchCount >= 2))
            {
                GameCamera.Inst.jiaodian = UICamera.lastWorldPosition;//试试这个新学到的
                //SuperGameManager.Instance.core = UICamera.lastWorldPosition;
                
                
            }
            //-------------------------------------------
            SuperGameManager.Instance.buttonPressed = true;
            SuperGameManager.Instance.validClickRequest = true;
            int id = UICamera.currentTouchID;
            SuperPoint sp = new SuperPoint();
            sp.Init(UICamera.GetTouch(id).pos.x, UICamera.GetTouch(id).pos.y);
            //加入直接先判断是否存在
            if (GameCamera.Inst.pd.ContainsKey(id))
            {
                GameCamera.Inst.pd.Remove(id);
            }
            //无论是主点还是辅点按下皆向map中放入一个新点
            GameCamera.Inst.pd.Add(id, sp);
            if (UICamera.touchCount == 2)
            {
                if (GameCamera.Inst.pd.Count == 2)//如果已经有两个触控点按下
                {
                    //SuperPoint sp1 = GameCamera.Inst.pd[0];//从map中取出第一个点
                    //SuperPoint sp2 = GameCamera.Inst.pd[1];//从map中取出第二个点
                    //GameCamera.Inst.distance = SuperPoint.calDistance(sp1, sp2);//计算两个触控点之间的距离
                    //Debug.Log("这两个触控点的初始距离是" + GameCamera.Inst.distance);
                }
            }
        }
        else//抬起
        {
            SuperGameManager.Instance.validClickRequest = true;
            //-----------------------------------------------------------------------
            SuperGameManager.Instance.buttonPressed = false;
            SuperGameManager.Instance.time = 0f;
            if (UICamera.currentTouchID == 0)//如果是主点抬起
            {
                GameCamera.Inst.pd.Clear();
                SuperGameManager.Instance.touchMode = SuperGameManager.TouchMode.None;
            }
            else if (UICamera.currentTouchID == 1)//如果是辅点抬起
            {
                GameCamera.Inst.pd.Remove(1);
                SuperGameManager.Instance.validClickRequest = false;
            }
        }
    }

    void OnClick()
    {
        int id = UICamera.currentTouchID;

        if (id == 0 || id == -1)//如果是鼠标左键单击或者是第一根指头单击
        {
            if (!SuperGameManager.Instance.validClickRequest)
            {
                SuperGameManager.Instance.validClickRequest = true;
                return;
            }

            if (SuperGameManager.Instance.validClickRequest)//非长按模式，那就是单选
            {
                //先判断目前被选中的物体有几个，如果只有一个，那就可以取消被选中的物体的染色，如果正处于多选状态，那么不执行任何操作
                if (SuperGameManager.Instance.selectedObjects.Count == 1)//说明只有一个物体被选中
                {
                    SuperGameManager.Instance.selectedObjects[0].GetComponent<TouchableObject>().CancelDye();
                    SuperGameManager.Instance.selectedObjects[0].GetComponent<TouchableObject>().Reset(false);
                }
            }
        }
    }
}
