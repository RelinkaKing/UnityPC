using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// 所有元素控制器父类
/// </summary>
public class BaseController : MonoBehaviour {
    //页索引
    public int pageNum = 1;
    //上一动画ID
    public int lastAniId = -1;
    //public Dictionary<int,T> initDic<T>(T[] elements) where T:ElementBaseClass
    //{
    //    Dictionary<int, T> baseDic = new Dictionary<int, T>();
    //    for (int i = 0; i < elements.Length; i++)
    //    {
    //        baseDic.Add(elements[i].id, elements[i]);
    //    }
    //    return baseDic;
    //}
    
    /// <summary>
    /// 快到碗里来~函数，收集所有元素的xml信息
    /// </summary>
    /// <param name="element"></param>
    public virtual void QuicklyComeIntoBowl(XmlElement element) {

    }
    public virtual void DoSort() {

    }
    /// <summary>
    /// 动画文档对象设置自增id
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    public void sortAniList<T>(T t) where T:ElementBaseClass
    {
        List<PPTAnimation> aniList = t.aniList;
        aniList.Sort();
        int count = 0;
        foreach (PPTAnimation tmpPPTAni in aniList) {
            tmpPPTAni.id = count;
            count++;
        }
    }
    public void compareList() {

    }
    /// <summary>
    /// 执行动画
    /// </summary>
    /// <param name="ani"></param>
    public virtual void DoAnimation(PPTAnimation ani)
    {

        //throw new System.NotImplementedException("Not Implemente <color=Blue>DoAnimation</color>!!!");
    }
    /// <summary>
    /// 初始化动画
    /// </summary>
    /// <param name="ani"></param>
    public virtual void InitAnimation(PPTAnimation ani) {
        
        throw new System.NotImplementedException("Not Implemente <color=Blue>InitAnimationGroup</color>!!!");
    }
    /// <summary>
    /// 开始动画
    /// </summary>
    public virtual void StartPPTAnimation() {
        throw new System.NotImplementedException("Not Implemente <color=Blue>StartPPTAnimation</color>!!!");
    }
    /// <summary>
    /// 结束，销毁元素
    /// </summary>
    /// <param name="slideTime"></param>
    public virtual void GoDie(float slideTime)
    {
        throw new System.NotImplementedException("Not Implemente <color=Blue>GoDie</color>!!!");
    }
    /// <summary>
    /// 定时执行函数
    /// </summary>
    /// <param name="slideTime"></param>
    public virtual void Do(float slideTime) {
        throw new System.NotImplementedException("Not Implemente <color=Blue>GoDie</color>!!!");
    }
    /// <summary>
    /// 初始化函数
    /// </summary>
    public virtual void Init()
    {
        //Debug.Log("Init Ani:" + AniIndex);
        AniIndex = 0;
        //  throw new System.NotImplementedException("Not Implemente <color=Blue>Init</color>!!!");
    }
    /// <summary>
    /// 初始化所需组件
    /// </summary>
    /// <param name="pageNum">页索引</param>
    public virtual void InitCompent(int pageNum) {
        throw new System.NotImplementedException("Not Implemente <color=Blue>Init</color>!!!");
    }
    /// <summary>
    /// 深拷贝一个元素屏幕位置信息（操作后不改变原元素信息）
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public ElementBaseClass copy(ElementBaseClass target)
    {
        return new ElementBaseClass
        {
            x = target.x,
            y = target.y,
            w = target.w,
            h = target.h
        };
    }
    //动画索引
    int AniIndex = 0;
    //public void runAni(ElementBaseclass ebc, float slidetime, Action<PPTAnimation> act)
    //{
    //    if (ebc.aniList.Count != 0)
    //    {
    //        while (AniIndex < ebc.aniList.Count && ebc.aniList[AniIndex].slideTime < slidetime)
    //        {
    //            act(ebc.aniList[AniIndex]);
    //            AniIndex++;
    //        }
    //    }

    //}
    /// <summary>
    /// 执行动画
    /// </summary>
    /// <param name="ebc">自身元素文档信息</param>
    /// <param name="slidetime">当前幻灯片播放时间</param>
    public void runAni(ElementBaseClass ebc, float slidetime)
    {
        if (ebc.aniList.Count != 0)
        {
            while (AniIndex < ebc.aniList.Count && ebc.aniList[AniIndex].slideTime < slidetime)
            {
                DoAnimation(ebc.aniList[AniIndex]);
                AniIndex++;
            }
        }

    }
    /// <summary>
    /// 初始化元素矩形信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    //1屏幕宽高改变
    //2坐标原点左上角0，0 ， 下为y+
    public virtual void InitRect<T>(T t) where T :  ElementBaseClass
    {
        PPTGlobal.SLIDE_WIDTH = Screen.width;
        PPTGlobal.SLIDE_Height = Screen.height;
        t = transXY(t);
        t = transWH(t);
        //float x, float y, float w, float h
        RectTransform tmpTf = gameObject.GetComponent<RectTransform>();
        tmpTf.anchoredPosition = new Vector2(t.x , t.y );
        tmpTf.sizeDelta = new Vector2(t.w, t.h);
        //throw new System.NotImplementedException("Not Implemente <color=Blue>InitRect</color>!!!");
    }
    //改变x,y,源坐标轴左上角0，0 ， 下为y+
    //目标坐标轴中心为0，0，上为y+
    public T transXY<T>(T t) where T : ElementBaseClass
    {
        t.x += 0.5f * t.w;
        t.y += 0.5f * t.h;
        t.x = t.x - 0.5f * PPTGlobal.Set_WIDTH;
        t.y = -(t.y - 0.5f * PPTGlobal.Set_Height);
        return t;
    }
    //改变w,h
    public T transWH<T>(T t) where T : ElementBaseClass {
        t.x = PPTGlobal.SLIDE_WIDTH / PPTGlobal.Set_WIDTH * t.x;
        t.y = PPTGlobal.SLIDE_Height / PPTGlobal.Set_Height * t.y;

        t.h = PPTGlobal.SLIDE_Height / PPTGlobal.Set_Height * t.h;
        t.w = PPTGlobal.SLIDE_WIDTH / PPTGlobal.Set_WIDTH * t.w;
        return t;
    }
    //暂停
    public virtual void Pause()
    {
        throw new System.NotImplementedException("Not Implemente <color=Blue>Pause</color>!!!");
    }
    //继续
    public virtual void Begin()
    {
        throw new System.NotImplementedException("Not Implemente <color=Blue>Begin</color>!!!");
    }
  
}
