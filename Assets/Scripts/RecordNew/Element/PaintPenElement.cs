using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PaintPenElement : ElementBaseClass
{
    [XmlAttribute("type")]
    public string type = string.Empty;

}
