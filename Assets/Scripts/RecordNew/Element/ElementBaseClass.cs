using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
/// <summary>
/// 所有播放元素父类
/// </summary>
public class ElementBaseClass
{
    //是否已经初始化
    [XmlIgnore]
    public bool isInit;
    //是否已经结束
    [XmlIgnore]
    public bool isEnd;
    //当前元素展示矩形信息
    [XmlIgnore]
    public Rect rect;
    //动画id
    [XmlAttribute("shapeId")]
    public int shapeId;
    //动画集合
    [XmlArray("Animtions"), XmlArrayItem("Animation")]
    public List<PPTAnimation> aniList;
    [XmlAttribute("id")]
    public int id;
    //出现时间
    [XmlAttribute("appearTime")]
    public float appearTime;
    //结束时间
    [XmlAttribute("endTime")]
    public float endTime;
    //元素所需资源文件名
    [XmlAttribute("Filename")]
    public string fileName;
    //当前元素展示位置信息
    [XmlAttribute("x")]
    public float x;
    [XmlAttribute("y")]
    public float y;
    [XmlAttribute("w")]
    public float w;
    [XmlAttribute("h")]
    public float h;
    /// <summary>
    /// 依据出现时间排序
    /// </summary>
    /// <param name="obj">另一个元素</param>
    /// <returns></returns>
    public int CompareTo(object obj)
    {
        ElementBaseClass other = obj as ElementBaseClass;
        if (other == null) return 1;
        return other.appearTime.CompareTo(appearTime) * -1;
    }

}

public enum ElementType
{
    Model,
    Video,
    Background,
    Audio,
    Qa
}