using UnityEngine;
using System.Collections;

public class SuperPoint
{
    float oldX;//上一次此触控点的X坐标
    float oldY;//上一次此触控点的Y坐标
    bool hasOld = false;//是否已经存在上一次触控位置的标志位
    float x;//触控点当前X坐标
    float y;//触控点当前Y坐标

    public void Init(float x, float y)//构造器
    {
        this.x = x;//初始化x坐标
        this.y = y;//初始化y坐标
    }
    public void SetLocation(float x, float y)//设置触控点新位置的方法
    {
        oldX = this.x;//把原来的位置记录为旧位置
        oldY = this.y;
        this.x = x;
        this.y = y;//设置新位置
    }
    public static float calDistance(SuperPoint a, SuperPoint b)//计算两个触控点距离的方法
    {
        float result = 0;
        result = (float) Mathf.Sqrt((a.x - b.x)*(a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        return result;
    }
}
