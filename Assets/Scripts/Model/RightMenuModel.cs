using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Model
{
    [Serializable]
    public class RightMenuModel
    {
        [PrimaryKey]
        public int rm_id { get; set; }
        public string rm_name { get; set; }
        public string add_img { get; set; }
        public string remove_img { get; set; }
        public string normal_img { get; set; }
        public string disable_img { get; set; }

    }
}
