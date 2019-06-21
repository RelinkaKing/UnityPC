using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Model
{
    [Serializable]
    public class AbInfo
    {
        [PrimaryKey]
        public string ab_name { get; set; }
        public string submodel_list { get; set; }
    }
}
