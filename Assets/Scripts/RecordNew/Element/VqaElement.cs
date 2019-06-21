using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class VqaElement : ElementBaseClass
{
    
    int type = 1;
    public string style = "default";
    public Vqa vqa;
}
[Serializable]
[XmlRoot("library")]
public class VqaRoot{
    [XmlElement("Question")]
    public Vqa vqa;
}
[Serializable]
public class Vqa {
    [XmlAttribute("id")]
    public string id;
    [XmlAttribute("content")]
    public string content;
    [XmlAttribute("type")]
    public int type;
    [XmlAttribute("picture")]
    public string picture;
    [XmlElement("Answer")]
    public List<Answer> answers;

}
[Serializable]
public class Answer
{
    [XmlAttribute("content")]
    public string content;
    [XmlAttribute("picture")]
    public string picture;
    [XmlAttribute("isAnswer")]
    public string isAnswer;
}