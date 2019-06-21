using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class LayserSubModel
    {
        [PrimaryKey]
        public string id { get; set; }
        public string layer_id { get; set; }
        public string sm_id { get; set; }
        public string sm_Name { get; set; }
    }
}
