using System;
using System.Collections;
using System.Collections.Generic;
using SQLite4Unity3d;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class texture_mask
    {
        [PrimaryKey]
        public int id { get; set; }
        public String name { get; set; }
        public byte[] tex_data { get; set; }
    }
}