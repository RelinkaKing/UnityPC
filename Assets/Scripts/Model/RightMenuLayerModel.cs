using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Model
{
    [Serializable]
    public class RightMenuLayerModel
    {
        public int layer { get; set; }
        [PrimaryKey]
        public string layer_id { get; set; }
        public int rm_id { get; set; }

    }
}
