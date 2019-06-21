using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class GetStructAbList
    {
        public string ab_list { get; set; }
        [PrimaryKey]
        public string id { get; set; }
        public string plat { get; set; }
        public string replace_app_id { get; set; }
        public string app_id { get; set; }
        public string level { get; set; }
    }
}