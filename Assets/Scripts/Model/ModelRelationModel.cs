using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class ModelRelationModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public string name { get; set; }
        public string ch_name { get; set; }
        public string hide_id { get; set; }

    }
}
