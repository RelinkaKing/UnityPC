using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class VideoElement : ElementBaseClass
{
    [XmlAttribute("isPlayAudio")]
    public bool isPlayAudio = false;
    [XmlAttribute("isLoop")]
    public bool isLoop = false;

}
