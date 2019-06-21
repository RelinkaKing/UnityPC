using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ModelElement : ElementBaseClass
{
    [XmlAttribute("modelId")]
    public string modelId = string.Empty;
    public string NewmodelId = string.Empty;
   
}
