using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class ab_list_info
    {
        public string id { get; set; }
        public string ab_name { get; set; }
        public string noun_no { get; set; }
    }
}